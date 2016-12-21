/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public class MemoryInboundAdapter : InboundAdapter
	{
		#region Constructors/Destructors

		public MemoryInboundAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private Timer timer;

		#endregion

		#region Methods/Operators

		protected override void CoreStartedInboundMessaging()
		{
		}

		protected override void CoreStartingInboundMessaging()
		{
			Task t = new Task((cts) => this.SendDummyMessages((CancellationTokenSource)cts, 10000), this.CancellationTokenSource);

			t.Start();
		}

		protected override void CoreStoppedInboundMessaging()
		{
		}

		protected override void CoreStoppingInboundMessaging()
		{
		}

		private void SendDummyMessages(CancellationTokenSource cts, int count)
		{
			IIntegrationMessage integrationMessage;
			Stream stream;

			for (int i = 0; i < count; i++)
			{
				if (cts.IsCancellationRequested)
					return;

				integrationMessage = this.IntegrationFactory.CreateMessage();
				//this.WriteLogSynchronized("INBOUND: Integration message ID '{0}' created on thread '{1}'.", integrationMessage.RunTimeId, Thread.CurrentThread.ManagedThreadId);

				stream = new MemoryStream(Encoding.UTF8.GetBytes(integrationMessage.RunTimeId.ToString("N")));

				// should this be done here???
				integrationMessage.DataStream = stream;
				integrationMessage.ContentType = "text/plain";
				integrationMessage.ContentEncoding = null; // deferred until stream to dropbox

				//Thread.Sleep(2500);
				this.PostInboundMessage(IntegrationEndpoint.Empty, integrationMessage);
			}
		}

		#endregion
	}
}