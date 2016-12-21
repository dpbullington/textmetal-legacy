/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Messaging.Core.MessageModel;
using TextMetal.Messaging.Core.PipelineModel;

namespace TextMetal.Messaging.Core
{
	public sealed class DefaultIntegrationFactory : IIntegrationFactory
	{
		#region Constructors/Destructors

		public DefaultIntegrationFactory()
		{
		}

		#endregion

		#region Methods/Operators

		public IIntegrationMessage CreateMessage(Guid runTimeId)
		{
			return IntegrationMessage.RehydrateInstance(runTimeId);
		}

		public IIntegrationMessage CreateMessage()
		{
			return new IntegrationMessage();
		}

		public IPipeliningContext CreatePipeliningContext()
		{
			return new PipeliningContext(Guid.NewGuid());
		}

		public IReceivePipeline CreateReceivePipeline()
		{
			IReceivePipeline receivePipeline;

			receivePipeline = new PassThruReceivePipeline();

			receivePipeline.Initialize();
			receivePipeline.Freeze();

			return receivePipeline;
		}

		public ISendPipeline CreateSendPipeline()
		{
			ISendPipeline sendPipeline;

			sendPipeline = new PassThruSendPipeline();

			sendPipeline.Initialize();
			sendPipeline.Freeze();

			return sendPipeline;
		}

		#endregion
	}
}