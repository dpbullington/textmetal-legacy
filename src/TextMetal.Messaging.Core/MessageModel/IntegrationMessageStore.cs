/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Threading;

namespace TextMetal.Messaging.Core.MessageModel
{
	public abstract class IntegrationMessageStore : IntegrationComponent, IIntegrationMessageStore
	{
		#region Constructors/Destructors

		protected IntegrationMessageStore()
		{
		}

		#endregion

		#region Fields/Constants

		private Timer dispatchMessageTimer;

		#endregion

		#region Properties/Indexers/Events

		private Timer DispatchMessageTimer
		{
			get
			{
				return this.dispatchMessageTimer;
			}
			set
			{
				this.dispatchMessageTimer = value;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void BeginDispatchingMessages(Action<object, IIntegrationMessage> postMessageCallback)
		{
			if ((object)postMessageCallback == null)
				throw new ArgumentNullException(nameof(postMessageCallback));

			this.DispatchMessageTimer = new Timer(state => this.DispatchMessagesInternal((Action<object, IIntegrationMessage>)state), postMessageCallback, 0, 1000);
		}

		protected override void CoreInitialize()
		{
		}

		protected override void CoreTerminate()
		{
			this.EndDispatchingMessages();
		}

		protected abstract void DispatchMessagesInternal(Action<object, IIntegrationMessage> postMessageCallback);

		public virtual void DropMessage(IIntegrationMessage integrationMessage)
		{
			if ((object)integrationMessage == null)
				throw new ArgumentNullException(nameof(integrationMessage));

			this.DropMessageInternal(integrationMessage);
		}

		protected abstract void DropMessageInternal(IIntegrationMessage integrationMessage);

		public virtual void EndDispatchingMessages()
		{
			if ((object)this.DispatchMessageTimer != null)
			{
				this.DispatchMessageTimer.Dispose();
				this.DispatchMessageTimer = null;
			}
		}

		public virtual void MarkMessage(IIntegrationMessage integrationMessage, Guid messageStateId)
		{
			if ((object)integrationMessage == null)
				throw new ArgumentNullException(nameof(integrationMessage));

			this.MarkMessageInternal(integrationMessage, messageStateId);
		}

		protected abstract void MarkMessageInternal(IIntegrationMessage integrationMessage, Guid messageStateId);

		#endregion
	}
}