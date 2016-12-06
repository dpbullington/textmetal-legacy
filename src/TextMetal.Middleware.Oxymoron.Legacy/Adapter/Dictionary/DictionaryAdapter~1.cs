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
	public abstract class DictionaryAdapter<TAdapterSpecificConfiguration> : Adapter<TAdapterSpecificConfiguration>, IDictionaryAdapter
		where TAdapterSpecificConfiguration : AdapterSpecificConfiguration, new()
	{
		#region Constructors/Destructors

		protected DictionaryAdapter()
		{
		}

		#endregion

		#region Methods/Operators

		protected abstract object CoreGetAlternativeValueFromId(DictionaryConfiguration dictionaryConfiguration, IColumn column, object surrogateId);

		protected abstract void CorePreloadCache(Func<IEnumerable<IRecord>, IEnumerable<IRecord>> recordCallback, DictionaryConfiguration dictionaryConfiguration, IDictionary<string, IDictionary<long, object>> substitutionCacheRoot);

		public object GetAlternativeValueFromId(DictionaryConfiguration dictionaryConfiguration, IColumn column, object surrogateId)
		{
			return this.CoreGetAlternativeValueFromId(dictionaryConfiguration, column, surrogateId);
		}

		public void InitializePreloadCache(Func<IEnumerable<IRecord>, IEnumerable<IRecord>> recordCallback, DictionaryConfiguration dictionaryConfiguration, IDictionary<string, IDictionary<long, object>> substitutionCacheRoot)
		{
			this.CorePreloadCache(recordCallback, dictionaryConfiguration, substitutionCacheRoot);
		}

		#endregion
	}
}