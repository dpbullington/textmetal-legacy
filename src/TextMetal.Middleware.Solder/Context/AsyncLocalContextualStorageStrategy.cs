/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Threading;

/* CERTIFICATION OF UNIT TESTING: dpbullington@gmail.com / 2016-02-22 / 100% code coverage */

namespace TextMetal.Middleware.Solder.Context
{
	public sealed class AsyncLocalContextualStorageStrategy : IContextualStorageStrategy
	{
		#region Constructors/Destructors

		public AsyncLocalContextualStorageStrategy()
		{
			this.tlsContext = new AsyncLocal<IDictionary<string, object>>();
			this.TlsContext.Value = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
		}

		#endregion

		#region Fields/Constants

		private readonly AsyncLocal<IDictionary<string, object>> tlsContext = new AsyncLocal<IDictionary<string, object>>();

		#endregion

		#region Properties/Indexers/Events

		private AsyncLocal<IDictionary<string, object>> TlsContext
		{
			get
			{
				return this.tlsContext;
			}
		}

		#endregion

		#region Methods/Operators

		public T GetValue<T>(string key)
		{
			object value;
			this.TlsContext.Value.TryGetValue(key, out value);
			return (T)value;
		}

		public bool HasValue(string key)
		{
			return this.TlsContext.Value.ContainsKey(key);
		}

		public void RemoveValue(string key)
		{
			if (this.TlsContext.Value.ContainsKey(key))
				this.TlsContext.Value.Remove(key);
		}

		public void SetValue<T>(string key, T value)
		{
			if (this.TlsContext.Value.ContainsKey(key))
				this.TlsContext.Value.Remove(key);

			this.TlsContext.Value.Add(key, value);
		}

		#endregion
	}
}