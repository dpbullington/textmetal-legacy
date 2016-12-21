/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public interface IInboundAdapter : IAdapter
	{
		#region Methods/Operators

		void SetInboundMessageEventHandler(InboundMessageEventHandler inboundMessageEventHandler);

		void StartInboundMessaging();

		void StopInboundMessaging();

		#endregion
	}
}