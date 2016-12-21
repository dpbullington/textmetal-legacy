/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public abstract class Stage : IntegrationComponent, IStage
	{
		#region Constructors/Destructors

		protected Stage()
		{
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
		}

		protected override void CoreTerminate()
		{
		}

		#endregion
	}
}