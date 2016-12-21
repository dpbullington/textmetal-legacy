/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public interface IOutboundAdapter : IAdapter
	{
		#region Methods/Operators

		void DisableOutboundMessageNotifications();

		void EnableOutboundMessageNotifications(Action<IOutboundAdapter> outboundMessageCallback);

		void SetNextOutboundMessage(IIntegrationMessage integrationMessage);

		#endregion
	}
}