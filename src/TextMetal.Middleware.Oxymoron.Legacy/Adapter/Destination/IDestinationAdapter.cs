/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Destination
{
	public interface IDestinationAdapter : IAdapter
	{
		#region Properties/Indexers/Events

		IEnumerable<IColumn> UpstreamMetadata
		{
			set;
		}

		#endregion

		#region Methods/Operators

		void PushData(TableConfiguration tableConfiguration, IEnumerable<IRecord> sourceDataEnumerable);

		#endregion
	}
}