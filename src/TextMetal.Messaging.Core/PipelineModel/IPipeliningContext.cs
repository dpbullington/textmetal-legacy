/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public interface IPipeliningContext : IDisposable
	{
		#region Properties/Indexers/Events

		Guid CorrelationId
		{
			get;
		}

		#endregion
	}
}