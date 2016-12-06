/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Datazoid.Primitives;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Tactics
{
	public interface ITacticParameter : IAdoNetParameter
	{
		#region Properties/Indexers/Events

		IDictionary<string, object> ParameterFixups
		{
			get;
		}

		#endregion
	}
}