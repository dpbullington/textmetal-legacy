/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Hosting;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy
{
	public interface IOxymoronEngine : IDisposable
	{
		#region Properties/Indexers/Events

		ObfuscationConfiguration ObfuscationConfiguration
		{
			get;
		}

		IOxymoronHost OxymoronHost
		{
			get;
		}

		IDictionary<string, IDictionary<long, object>> SubstitutionCacheRoot
		{
			get;
		}

		#endregion

		#region Methods/Operators

		long GetBoundedHash(long? size, object value);

		object GetObfuscatedValue(IColumn column, object columnValue);

		IEnumerable<IRecord> GetObfuscatedValues(IEnumerable<IRecord> records);

		#endregion
	}
}