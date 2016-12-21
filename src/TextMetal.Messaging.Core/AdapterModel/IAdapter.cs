/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public interface IAdapter : IIntegrationComponent
	{
		#region Methods/Operators

		void ClearEndpoints();

		IReadOnlyCollection<IntegrationEndpoint> GetEndpoints();

		void SetEndpoints(IEnumerable<IntegrationEndpoint> endpoints);

		#endregion
	}
}