/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Threading;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public abstract class OutboundAdapter : Adapter, IOutboundAdapter
	{
		#region Constructors/Destructors

		protected OutboundAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly Queue<IIntegrationMessage> outboundMessageQueue = new Queue<IIntegrationMessage>();
		private Action<IOutboundAdapter> outboundMessageNotificationCallback;

		#endregion

		#region Properties/Indexers/Events

		private Queue<IIntegrationMessage> OutboundMessageQueue
		{
			get
			{
				return this.outboundMessageQueue;
			}
		}

		private Action<IOutboundAdapter> OutboundMessageNotificationCallback
		{
			get
			{
				return this.outboundMessageNotificationCallback;
			}
			set
			{
				this.outboundMessageNotificationCallback = value;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void DisableOutboundMessageNotifications()
		{
			this.AssertReady();
			this.WriteLogSynchronized("OUTBOUND: Disabling outbound message notifications on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			lock (this.SyncLock)
			{
				this.OutboundMessageNotificationCallback = null;
			}
		}

		public void EnableOutboundMessageNotifications(Action<IOutboundAdapter> outboundMessageCallback)
		{
			if ((object)outboundMessageCallback == null)
				throw new ArgumentNullException("outboundMessageCallback");

			this.AssertReady();
			this.WriteLogSynchronized("OUTBOUND: Enabling outbound message notifications on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			lock (this.SyncLock)
			{
				if ((object)this.OutboundMessageNotificationCallback != null)
					throw new InvalidOperationException("The outbound adapter instance is already posting/dispatching message events.");

				this.OutboundMessageNotificationCallback = outboundMessageCallback;
			}
		}

		protected abstract void EnqueueOutboundMessageInternal(IIntegrationMessage integrationMessage);

		protected IIntegrationMessage PostThenDispatchOutboundMessage()
		{
			IIntegrationMessage integrationMessage;

			this.AssertReady();
			this.WriteLogSynchronized("INBOUND: post/dispatch inbound message on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			lock (this.SyncLock) // TODO: change this to reader/writer lock
			{
				integrationMessage = this.OutboundMessageQueue.Dequeue();

				if ((object)this.OutboundMessageNotificationCallback != null)
					this.OutboundMessageNotificationCallback(this);
			}

			return integrationMessage;
		}

		public virtual void SetNextOutboundMessage(IIntegrationMessage integrationMessage)
		{
			if ((object)integrationMessage == null)
				throw new ArgumentNullException("integrationMessage");

			this.AssertReady();
			this.WriteLogSynchronized("OUTBOUND: Set next outbound message on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			lock (this.SyncLock) // TODO: change this to reader/writer lock
				this.OutboundMessageQueue.Enqueue(integrationMessage);
		}

		#endregion
	}
}