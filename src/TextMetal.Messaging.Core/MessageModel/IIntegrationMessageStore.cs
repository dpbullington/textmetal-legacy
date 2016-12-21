/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Messaging.Core.MessageModel
{
	public interface IIntegrationMessageStore : IIntegrationComponent
	{
		#region Methods/Operators

		void BeginDispatchingMessages(Action<object, IIntegrationMessage> postMessageCallback);

		void DropMessage(IIntegrationMessage integrationMessage);

		void EndDispatchingMessages();

		void MarkMessage(IIntegrationMessage integrationMessage, Guid messageStateId);

		#endregion
	}
}