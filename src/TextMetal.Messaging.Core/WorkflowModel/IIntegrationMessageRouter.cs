/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.WorkflowModel
{
	public interface IIntegrationMessageRouter : IIntegrationComponent
	{
		#region Methods/Operators

		void Route(IIntegrationMessage integrationMessage);

		#endregion
	}
}