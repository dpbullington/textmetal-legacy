/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Web;

namespace TextMetal.Middleware.Common.Strategies.ContextualStorage.ApplicationSpecific
{
	public class HttpSessionApplicationSpecificStorageStrategy : IApplicationSpecificStorageStrategy
	{
		#region Constructors/Destructors

		public HttpSessionApplicationSpecificStorageStrategy()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		public bool IsSafeCrossPrincipal
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region Methods/Operators

		public T GetValue<T>(string key)
		{
			object value;

			if ((object)HttpContext.Current == null)
				return default(T);

			value = HttpContext.Current.Session[key];

			return (T)value;
		}

		public bool HasValue(string key)
		{
			if ((object)HttpContext.Current == null)
				return false;

			return (object)HttpContext.Current.Session[key] != null;
		}

		public void RemoveValue(string key)
		{
			if ((object)HttpContext.Current == null)
				return;

			HttpContext.Current.Session.Remove(key);
		}

		public void SetValue<T>(string key, T value)
		{
			if ((object)HttpContext.Current == null)
				return;

			HttpContext.Current.Session.Add(key, value);
		}

		#endregion
	}
}