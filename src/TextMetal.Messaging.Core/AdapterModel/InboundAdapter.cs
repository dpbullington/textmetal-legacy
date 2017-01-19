/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public abstract class InboundAdapter : Adapter, IInboundAdapter
	{
		#region Constructors/Destructors

		protected InboundAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly BlockingCollection<Tuple<IntegrationEndpoint, IIntegrationMessage>> inboundMessageQueue = new BlockingCollection<Tuple<IntegrationEndpoint, IIntegrationMessage>>();
		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		private InboundMessageEventHandler inboundMessageEventHandler;
		private Thread messagePumpThread;

		#endregion

		#region Properties/Indexers/Events

		private BlockingCollection<Tuple<IntegrationEndpoint, IIntegrationMessage>> InboundMessageQueue
		{
			get
			{
				return this.inboundMessageQueue;
			}
		}

		protected CancellationTokenSource CancellationTokenSource
		{
			get
			{
				return this.cancellationTokenSource;
			}
			set
			{
				this.cancellationTokenSource = value;
			}
		}

		private InboundMessageEventHandler InboundMessageEventHandler
		{
			get
			{
				return this.inboundMessageEventHandler;
			}
			set
			{
				this.inboundMessageEventHandler = value;
			}
		}

		private Thread MessagePumpThread
		{
			get
			{
				return this.messagePumpThread;
			}
			set
			{
				this.messagePumpThread = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
			base.CoreInitialize();

			this.WriteLogSynchronized("INBOUND: Initialized on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);
		}

		protected abstract void CoreStartedInboundMessaging();

		protected abstract void CoreStartingInboundMessaging();

		protected abstract void CoreStoppedInboundMessaging();

		protected abstract void CoreStoppingInboundMessaging();

		protected override void CoreTerminate()
		{
			this.WriteLogSynchronized("INBOUND: Terminated on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			// this will block...
			//this.StopInboundMessaging();

			base.CoreTerminate();
		}

		private void DispatchInboundMessage(Tuple<IntegrationEndpoint, IIntegrationMessage> queuedMessageTuple)
		{
			if ((object)queuedMessageTuple == null)
				throw new ArgumentNullException(nameof(queuedMessageTuple));

			this.WriteLogSynchronized("INBOUND: Dispatch inbound message on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			ThreadPool.QueueUserWorkItem((state) =>
										{
											Tuple<IntegrationEndpoint, IIntegrationMessage> _state = (Tuple<IntegrationEndpoint, IIntegrationMessage>)state;

											if ((object)this.InboundMessageEventHandler != null)
												this.InboundMessageEventHandler(this, new InboundMessageEventArgs(_state.Item1, _state.Item2));
										}, queuedMessageTuple);
		}

		private bool GetInboundMessage(out Tuple<IntegrationEndpoint, IIntegrationMessage> queuedMessageTuple)
		{
			this.AssertReady();
			this.WriteLogSynchronized("INBOUND: Get next inbound message on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			try
			{
				if (!this.InboundMessageQueue.TryTake(out queuedMessageTuple, -1, this.CancellationTokenSource.Token))
					return false;
			}
			catch (OperationCanceledException)
			{
				this.WriteLogSynchronized("__'{0}' remain on thread '{1}'.", this.InboundMessageQueue.Count, Thread.CurrentThread.ManagedThreadId);
				queuedMessageTuple = null;
				return false;
			}

			return true;
		}

		protected void PostInboundMessage(IntegrationEndpoint integrationEndpoint, IIntegrationMessage integrationMessage)
		{
			if ((object)integrationEndpoint == null)
				throw new ArgumentNullException(nameof(integrationEndpoint));

			if ((object)integrationMessage == null)
				throw new ArgumentNullException(nameof(integrationMessage));

			this.AssertReady();
			this.WriteLogSynchronized("INBOUND: Post inbound message on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			this.InboundMessageQueue.Add(new Tuple<IntegrationEndpoint, IIntegrationMessage>(integrationEndpoint, integrationMessage));
		}

		private void PumpInboundMessages()
		{
			Tuple<IntegrationEndpoint, IIntegrationMessage> queuedMessageTuple;

			bool isNew;
			EventWaitHandle eventWaitHandle;

			isNew = this.TryGetEventWaitHandle(out eventWaitHandle);

			{
				try
				{
					while (this.GetInboundMessage(out queuedMessageTuple))
					{
						//this.TranslateInboundMessage(queuedMessageTuple);
						this.DispatchInboundMessage(queuedMessageTuple);
					}
				}
				finally
				{
					eventWaitHandle.Set();
				}
			}
		}

		public void SetInboundMessageEventHandler(InboundMessageEventHandler inboundMessageEventHandler)
		{
			this.AssertReady();
			this.WriteLogSynchronized("INBOUND: Setting inbound message event handler on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			lock (this.SyncLock) // TODO: change this to reader/writer lock
			{
				if ((object)inboundMessageEventHandler != null &&
					(object)this.InboundMessageEventHandler != null)
					throw new InvalidOperationException("The inbound adapter instance is already posting/dispatching message events.");

				this.InboundMessageEventHandler = inboundMessageEventHandler;
			}
		}

		public void StartInboundMessaging()
		{
			this.WriteLogSynchronized("INBOUND: Starting inbound messaging on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			this.CoreStartingInboundMessaging();

			this.MessagePumpThread = new Thread(this.PumpInboundMessages);
			this.MessagePumpThread.Name = "Inbound Adapter Message Pump Thread";
			this.MessagePumpThread.Start();

			this.CoreStartedInboundMessaging();
		}

		public void StopInboundMessaging()
		{
			bool isNew;
			EventWaitHandle eventWaitHandle;

			const bool DO_ABORT = false;
			const int ABORT_TO = 1000 * 5;

			this.WriteLogSynchronized("INBOUND: Stopping inbound messaging on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			this.CoreStoppingInboundMessaging();

			this.CancellationTokenSource.Cancel();
			isNew = this.TryGetEventWaitHandle(out eventWaitHandle);

			using (eventWaitHandle)
			{
				if (DO_ABORT)
				{
					if (!eventWaitHandle.WaitOne(ABORT_TO))
						Environment.FailFast(string.Empty); //this.MessagePumpThread.Abort();
				}
				else
					eventWaitHandle.WaitOne();
			}

			this.CoreStoppedInboundMessaging();
			this.CancellationTokenSource.Dispose();
			this.WriteLogSynchronized("'{0}' remain on thread '{1}'.", this.InboundMessageQueue.Count, Thread.CurrentThread.ManagedThreadId);
		}

		private bool TryGetEventWaitHandle(out EventWaitHandle eventWaitHandle)
		{
			bool isNew;

			eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "message_pump_" + this.GetType().GetTypeInfo().GUID.ToString("N"), out isNew);

			return isNew;
		}

		#endregion
	}
}