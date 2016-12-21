/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public sealed class PassThruSendPipeline : SendPipeline
	{
		#region Constructors/Destructors

		public PassThruSendPipeline()
		{
		}

		#endregion

		#region Methods/Operators

		protected override void ApplyStageTypes()
		{
			base.ApplyStageTypes();
			this.AddStageType("PassThruSendStage", typeof(PassThruSendStage));
		}

		#endregion
	}
}