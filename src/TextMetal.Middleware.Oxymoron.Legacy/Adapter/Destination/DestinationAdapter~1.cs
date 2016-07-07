/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Destination
{
	public abstract class DestinationAdapter<TAdapterSpecificConfiguration> : Adapter<TAdapterSpecificConfiguration>, IDestinationAdapter
		where TAdapterSpecificConfiguration : AdapterSpecificConfiguration, new()
	{
		#region Constructors/Destructors

		protected DestinationAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private IEnumerable<IColumn> upstreamMetadata;

		#endregion

		#region Properties/Indexers/Events

		public IEnumerable<IColumn> UpstreamMetadata
		{
			set
			{
				this.upstreamMetadata = value;
			}
			protected get
			{
				return this.upstreamMetadata;
			}
		}

		#endregion

		#region Methods/Operators

		protected abstract void CorePushData(TableConfiguration tableConfiguration, IEnumerable<IRecord> sourceDataEnumerable);

		public void PushData(TableConfiguration tableConfiguration, IEnumerable<IRecord> sourceDataEnumerable)
		{
			this.CorePushData(tableConfiguration, sourceDataEnumerable);
		}

		#endregion
	}
}