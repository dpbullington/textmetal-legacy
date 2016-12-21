/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Messaging.Core.MessageModel;
using TextMetal.Messaging.Core.PipelineModel;

namespace TextMetal.Messaging.Core
{
	public interface IIntegrationFactory
	{
		#region Methods/Operators

		IIntegrationMessage CreateMessage();

		IIntegrationMessage CreateMessage(Guid runTimeId);

		IPipeliningContext CreatePipeliningContext();

		IReceivePipeline CreateReceivePipeline();

		ISendPipeline CreateSendPipeline();

		#endregion
	}
}