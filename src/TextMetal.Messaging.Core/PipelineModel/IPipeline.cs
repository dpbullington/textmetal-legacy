/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public interface IPipeline : IIntegrationComponent
	{
		#region Methods/Operators

		void AddStageType(string stageName, Type stageType);

		void ClearStageTypes();

		IPipeliningContext CreateContext();

		IReadOnlyDictionary<string, Type> GetStageTypes();

		#endregion
	}
}