/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public interface IReceiveStage : IStage
	{
		#region Methods/Operators

		IEnumerable<IIntegrationMessage> Execute(IPipeliningContext pipeliningContext, IIntegrationMessage integrationMessage);

		#endregion
	}
}