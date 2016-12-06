/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Source
{
	public abstract class SourceAdapter<TAdapterSpecificConfiguration> : Adapter<TAdapterSpecificConfiguration>, ISourceAdapter
		where TAdapterSpecificConfiguration : AdapterSpecificConfiguration, new()
	{
		#region Constructors/Destructors

		protected SourceAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private IEnumerable<IColumn> upstreamMetadata;

		#endregion

		#region Properties/Indexers/Events

		public IEnumerable<IColumn> UpstreamMetadata
		{
			get
			{
				return this.upstreamMetadata;
			}
			protected set
			{
				this.upstreamMetadata = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected abstract IEnumerable<IRecord> CorePullData(TableConfiguration tableConfiguration);

		public IEnumerable<IRecord> PullData(TableConfiguration tableConfiguration)
		{
			if ((object)tableConfiguration == null)
				throw new ArgumentNullException(nameof(tableConfiguration));

			return this.CorePullData(tableConfiguration);
		}

		#endregion
	}
}