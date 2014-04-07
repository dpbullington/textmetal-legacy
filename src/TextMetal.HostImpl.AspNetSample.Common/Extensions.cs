/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace TextMetal.HostImpl.AspNetSample.Common
{
	public static class Extensions
	{
		#region Methods/Operators

		public static Dictionary<TFirstKey, Dictionary<TSecondKey, TValue>> Pivot<TSource, TFirstKey, TSecondKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TFirstKey> firstKeySelector, Func<TSource, TSecondKey> secondKeySelector, Func<IEnumerable<TSource>, TValue> aggregate)
		{
			var retVal = new Dictionary<TFirstKey, Dictionary<TSecondKey, TValue>>();

			var l = source.ToLookup(firstKeySelector);
			foreach (var item in l)
			{
				var dict = new Dictionary<TSecondKey, TValue>();
				retVal.Add(item.Key, dict);
				var subdict = item.ToLookup(secondKeySelector);
				foreach (var subitem in subdict)
					dict.Add(subitem.Key, aggregate(subitem));
			}

			return retVal;
		}

		#endregion
	}
}