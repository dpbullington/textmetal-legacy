/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Dictionary
{
	public class NullDictionaryAdapter : DictionaryAdapter<AdapterSpecificConfiguration>, INullAdapter
	{
		#region Constructors/Destructors

		public NullDictionaryAdapter()
		{
		}

		#endregion

		#region Methods/Operators

		protected override object CoreGetAlternativeValueFromId(DictionaryConfiguration dictionaryConfiguration, IColumn column, object surrogateId)
		{
			if ((object)dictionaryConfiguration == null)
				throw new ArgumentNullException(nameof(dictionaryConfiguration));

			if ((object)column == null)
				throw new ArgumentNullException(nameof(column));

			if ((object)surrogateId == null)
				throw new ArgumentNullException(nameof(surrogateId));

			return null;
		}

		protected override void CoreInitialize()
		{
		}

		protected override void CorePreloadCache(Func<IEnumerable<IRecord>, IEnumerable<IRecord>> recordCallback, DictionaryConfiguration dictionaryConfiguration, IDictionary<string, IDictionary<long, object>> substitutionCacheRoot)
		{
			if ((object)recordCallback == null)
				throw new ArgumentNullException(nameof(recordCallback));

			if ((object)dictionaryConfiguration == null)
				throw new ArgumentNullException(nameof(dictionaryConfiguration));

			if ((object)substitutionCacheRoot == null)
				throw new ArgumentNullException(nameof(substitutionCacheRoot));
		}

		protected override void CoreTerminate()
		{
		}

		#endregion
	}
}