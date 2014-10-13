/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Common.Core.AmbientExecutionContext
{
	public sealed class ThreadStaticExecutionPathStorage : IExecutionPathStorage
	{
		#region Constructors/Destructors

		public ThreadStaticExecutionPathStorage()
		{
		}

		#endregion

		#region Fields/Constants

		[ThreadStatic]
		private static IDictionary<string, object> __tls_context;

		#endregion

		#region Methods/Operators

		public object GetValue(string key)
		{
			object value;

			__tls_context = __tls_context ?? new Dictionary<string, object>();

			__tls_context.TryGetValue(key, out value);
			return value;
		}

		public void RemoveValue(string key)
		{
			__tls_context = __tls_context ?? new Dictionary<string, object>();

			if (__tls_context.ContainsKey(key))
				__tls_context.Remove(key);
		}

		public void SetValue(string key, object value)
		{
			__tls_context = __tls_context ?? new Dictionary<string, object>();

			if (__tls_context.ContainsKey(key))
				__tls_context.Remove(key);

			__tls_context.Add(key, value);
		}

		#endregion
	}
}