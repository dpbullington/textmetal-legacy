/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public sealed class PassThruReceiveStage : ReceiveStage
	{
		#region Constructors/Destructors

		public PassThruReceiveStage()
		{
		}

		#endregion

		#region Methods/Operators

		public override IEnumerable<IIntegrationMessage> Execute(IPipeliningContext pipeliningContext, IIntegrationMessage integrationMessage)
		{
			//return new IIntegrationMessage[] { };
			return new IIntegrationMessage[] { integrationMessage };
		}

		#endregion
	}
}