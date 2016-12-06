/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Source
{
	public interface ISourceAdapter : IAdapter
	{
		#region Properties/Indexers/Events

		IEnumerable<IColumn> UpstreamMetadata
		{
			get;
		}

		#endregion

		#region Methods/Operators

		IEnumerable<IRecord> PullData(TableConfiguration tableConfiguration);

		#endregion
	}
}