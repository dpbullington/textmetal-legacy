/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Threading;

namespace TextMetal.Middleware.Solder.Context
{
	public sealed class ThreadLocalContextualStorageStrategy : IContextualStorageStrategy
	{
		#region Constructors/Destructors

		public ThreadLocalContextualStorageStrategy()
		{
		}

		#endregion

		#region Fields/Constants

		static ThreadLocal<IDictionary<string, object>> __tls_context = new ThreadLocal<IDictionary<string, object>>();

		#endregion

		#region Methods/Operators

		public T GetValue<T>(string key)
		{
			object value;
			__tls_context.Value.TryGetValue(key, out value);
			return (T)value;
		}

		public bool HasValue(string key)
		{
			return __tls_context.Value.ContainsKey(key);
		}

		public void RemoveValue(string key)
		{
			if (__tls_context.Value.ContainsKey(key))
				__tls_context.Value.Remove(key);
		}

		public void SetValue<T>(string key, T value)
		{
			if (__tls_context.Value.ContainsKey(key))
				__tls_context.Value.Remove(key);

			__tls_context.Value.Add(key, value);
		}

		#endregion
	}
}