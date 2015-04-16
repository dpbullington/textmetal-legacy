/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Web;

using TextMetal.Middleware.Common.Fascades.Utilities;

namespace TextMetal.Middleware.Common.Strategies.ContextualStorage.ApplicationSpecific
{
	public class HttpCookieApplicationSpecificStorageStrategy : IApplicationSpecificStorageStrategy
	{
		#region Constructors/Destructors

		public HttpCookieApplicationSpecificStorageStrategy()
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
			HttpCookie httpCookie;
			T value;

			httpCookie = HttpContext.Current.Request.Cookies[key];

			if ((object)httpCookie == null)
				return default(T);

			if (!DataTypeFascade.Instance.TryParse(httpCookie.Value, out value))
				return default(T);

			return value;
		}

		public bool HasValue(string key)
		{
			HttpCookie httpCookie;

			httpCookie = HttpContext.Current.Request.Cookies[key];

			return (object)httpCookie != null && (object)httpCookie.Value != null;
		}

		public void RemoveValue(string key)
		{
			//HttpContext.Current.Response.Cookies.Remove(key);
			HttpCookie httpCookie;

			httpCookie = new HttpCookie(key, null);
			httpCookie.Expires = DateTime.UtcNow.AddYears(-1);

			HttpContext.Current.Response.Cookies.Add(httpCookie);
		}

		public void SetValue<T>(string key, T value)
		{
			HttpCookie httpCookie;

			httpCookie = new HttpCookie(key, value.SafeToString());
			httpCookie.Expires = DateTime.MaxValue;

			HttpContext.Current.Response.Cookies.Add(httpCookie);
		}

		#endregion
	}
}