/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public abstract class SendStage : Stage, ISendStage
	{
		#region Constructors/Destructors

		protected SendStage()
		{
		}

		#endregion

		#region Methods/Operators

		public abstract IIntegrationMessage Execute(IPipeliningContext pipeliningContext);

		#endregion
	}
}