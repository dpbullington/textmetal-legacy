/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public abstract class ReceiveStage : Stage, IReceiveStage
	{
		#region Constructors/Destructors

		protected ReceiveStage()
		{
		}

		#endregion

		#region Methods/Operators

		public abstract IEnumerable<IIntegrationMessage> Execute(IPipeliningContext pipeliningContext, IIntegrationMessage integrationMessage);

		#endregion
	}
}