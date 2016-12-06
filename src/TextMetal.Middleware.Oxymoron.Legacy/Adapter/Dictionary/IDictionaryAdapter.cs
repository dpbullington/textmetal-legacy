/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Dictionary
{
	public interface IDictionaryAdapter : IAdapter
	{
		#region Methods/Operators

		object GetAlternativeValueFromId(DictionaryConfiguration dictionaryConfiguration, IColumn column, object surrogateId);

		void InitializePreloadCache(Func<IEnumerable<IRecord>, IEnumerable<IRecord>> recordCallback, DictionaryConfiguration dictionaryConfiguration, IDictionary<string, IDictionary<long, object>> substitutionCacheRoot);

		#endregion
	}
}