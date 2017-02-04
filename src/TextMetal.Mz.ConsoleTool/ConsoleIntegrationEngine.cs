/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Threading;

using TextMetal.Messaging.Core;
using TextMetal.Messaging.Core.AdapterModel;
using TextMetal.Messaging.Core.MessageModel;
using TextMetal.Messaging.Core.PipelineModel;
using TextMetal.Messaging.Core.WorkflowModel;
using TextMetal.Messaging.Store;
using TextMetal.Middleware.Solder.Injection;

namespace TextMetal.Mz.ConsoleTool
{
	public class ConsoleIntegrationEngine
	{
		#region Constructors/Destructors

		public ConsoleIntegrationEngine()
		{
		}

		#endregion

		#region Methods/Operators

		private void __HostInboundMessageEventHandler(IInboundAdapter sender, InboundMessageEventArgs e)
		{
			if ((object)sender == null)
				throw new ArgumentNullException("sender");

			if ((object)e == null)
				throw new ArgumentNullException("e");

			Console.WriteLine("ENGINE: Inbound message notification on thread {0}.", Thread.CurrentThread.ManagedThreadId);
			Thread.Sleep(1000);
			e.IntegrationMessage.Dispose();
		}

		private void HostInboundMessageEventHandler(IInboundAdapter sender, InboundMessageEventArgs e)
		{
			IIntegrationFactory integrationFactory;
			IIntegrationMessageStore integrationMessageStore;
			IIntegrationMessage originalIntegrationMessage;
			IEnumerable<IIntegrationMessage> integrationMessages;

			if ((object)sender == null)
				throw new ArgumentNullException("sender");

			if ((object)e == null)
				throw new ArgumentNullException("e");

			//System.Console.WriteLine("ENGINE: Inbound message notification on thread {0}.", Thread.CurrentThread.ManagedThreadId);

			integrationFactory = AssemblyDomain.Default.DependencyManager.ResolveDependency<IIntegrationFactory>(string.Empty, false);
			integrationMessageStore = AssemblyDomain.Default.DependencyManager.ResolveDependency<IIntegrationMessageStore>(string.Empty, false);

			// get the message
			originalIntegrationMessage = e.IntegrationMessage;

			if ((object)originalIntegrationMessage == null)
				throw new InvalidOperationException("Inbound message was invalid.");

			using (originalIntegrationMessage)
			{
				// get receive pipeline
				using (IReceivePipeline receivePipeline = integrationFactory.CreateReceivePipeline())
				{
					// inbound pipeline processing
					using (IPipeliningContext pipeliningContext = receivePipeline.CreateContext())
						integrationMessages = receivePipeline.Execute(pipeliningContext, originalIntegrationMessage);

					if ((object)integrationMessages == null)
						throw new InvalidOperationException("Inbound messages were invalid.");

					// persist to dropbox
					foreach (IIntegrationMessage integrationMessage in integrationMessages)
					{
						if ((object)integrationMessage.DataStream != null)
						{
							using (integrationMessage.DataStream)
								integrationMessageStore.DropMessage(integrationMessage);
						}
					}
				}
			}
		}

		private void OutboundMessageNotification(IOutboundAdapter targetOutboundAdapter)
		{
			IIntegrationMessage integrationMessage;

			if ((object)targetOutboundAdapter == null)
				throw new ArgumentNullException("targetOutboundAdapter");

			// peek the message the inbound queue
			//integrationMessage = targetOutboundAdapter.PeekOutboundMessage();

			//if ((object)integrationMessage == null)
			//throw new InvalidOperationException("Inbound message was invalid.");

			// outbound pipeline processing
			//this.SendPipeline.Stages.ForEach.ProcessStream(messageId, ref inboundMessageStream);

			//System.Console.WriteLine("ENGINE: Outbound message notification on thread {0}.", Thread.CurrentThread.ManagedThreadId);
		}

		/// <summary>
		/// TODO this sucks
		/// </summary>
		public void RunHost()
		{
			IIntegrationFactory integrationFactory;
			IIntegrationMessageStore integrationMessageStore;
			IIntegrationMessageRouter integrationMessageRouter;

			IList<IInboundAdapter> inboundAdapters;
			IList<IOutboundAdapter> outboundAdapters;

			//System.Console.WriteLine("ENGINE: Ran on thread {0}.", Thread.CurrentThread.ManagedThreadId);

			//using (ImpersonationScope ctx = new ImpersonationScope("dev_test", ".", "pass@word1", ImpersonationScope.LogonType.LOGON32_LOGON_INTERACTIVE, ImpersonationScope.LogonProvider.LOGON32_PROVIDER_DEFAULT))
			{
				integrationFactory = new DefaultIntegrationFactory();
				integrationMessageStore = new SqlDbMessageStore();
				integrationMessageRouter = new DefaultIntegrationMessageRouter();

				integrationMessageStore.Freeze();
				integrationMessageStore.Initialize();

				AssemblyDomain.Default.DependencyManager.AddResolution<IIntegrationFactory>(string.Empty, false, new SingletonWrapperDependencyResolution<IIntegrationFactory>(new InstanceDependencyResolution<IIntegrationFactory>(integrationFactory)));
				AssemblyDomain.Default.DependencyManager.AddResolution<IIntegrationMessageStore>(string.Empty, false, new SingletonWrapperDependencyResolution<IIntegrationMessageStore>(new InstanceDependencyResolution<IIntegrationMessageStore>(integrationMessageStore)));

				inboundAdapters = new List<IInboundAdapter>();
				inboundAdapters.Add(new MemoryInboundAdapter());
				//inboundAdapters.Add(new FileSystemInboundAdapter());

				outboundAdapters = new List<IOutboundAdapter>();
				outboundAdapters.Add(new FileSystemOutboundAdapter());

				foreach (IInboundAdapter inboundAdapter in inboundAdapters)
				{
					inboundAdapter.Initialize();
					inboundAdapter.SetEndpoints(new IntegrationEndpoint[] { new IntegrationEndpoint(new IntegrationAddress(new Uri("d:\\#dev_temp\\_in_drop_box_"), DefaultIdentity.Instance), new IntegrationBinding(new object(), new object(), new object()), new IntegrationContract(), typeof(void)) });
					inboundAdapter.Freeze();
					inboundAdapter.SetInboundMessageEventHandler(this.__HostInboundMessageEventHandler);
					inboundAdapter.StartInboundMessaging();
				}

				foreach (IOutboundAdapter outboundAdapter in outboundAdapters)
				{
					outboundAdapter.SetEndpoints(new IntegrationEndpoint[] { new IntegrationEndpoint(new IntegrationAddress(new Uri("d:\\#dev_temp\\_out_drop_box_"), DefaultIdentity.Instance), new IntegrationBinding(new object(), new object(), new object()), new IntegrationContract(), typeof(void)) });
					outboundAdapter.Freeze();
					outboundAdapter.Initialize();
					outboundAdapter.EnableOutboundMessageNotifications(this.OutboundMessageNotification);
				}

				// this really should be a message router thingy instead of direct to adapter (TODO)
				integrationMessageStore.BeginDispatchingMessages((sender, msg) =>
																{
																	//outboundAdapters[0].SetNextOutboundMessage(msg);
																	//integrationMessageRouter.Route(msg);
																}
				);

				{
					Console.WriteLine("<<< WAITING >>>");
					Console.ReadKey();
				}

				integrationMessageStore.EndDispatchingMessages();

				foreach (IOutboundAdapter outboundAdapter in outboundAdapters)
				{
					outboundAdapter.DisableOutboundMessageNotifications();
					outboundAdapter.Terminate();
				}

				foreach (IInboundAdapter inboundAdapter in inboundAdapters)
				{
					inboundAdapter.StopInboundMessaging();
					inboundAdapter.SetInboundMessageEventHandler(null);
					inboundAdapter.Terminate();
				}

				outboundAdapters.Clear();
				inboundAdapters.Clear();

				integrationMessageStore.Terminate();

				AssemblyDomain.Default.DependencyManager.RemoveResolution<IIntegrationFactory>(string.Empty, false);
				AssemblyDomain.Default.DependencyManager.RemoveResolution<IIntegrationMessageStore>(string.Empty, false);
			}
		}

		#endregion
	}
}