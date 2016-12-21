/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public interface ISendPipeline : IPipeline
	{
		#region Methods/Operators

		IIntegrationMessage Execute(IPipeliningContext pipeliningContext);

		#endregion
	}
}