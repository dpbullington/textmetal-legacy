/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Web;

using TextMetal.Common.Core;
using TextMetal.HostImpl.AspNetSample.Common;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Shared
{
	public class WebStickySession : ISession
	{
		#region Constructors/Destructors

		public WebStickySession()
		{
		}

		#endregion

		#region Methods/Operators

		public void FreeValue(string key)
		{
			//HttpContext.Current.Response.Cookies.Remove(key);
			HttpCookie httpCookie;

			httpCookie = new HttpCookie(key, null);
			httpCookie.Expires = DateTime.UtcNow.AddYears(-1);

			HttpContext.Current.Response.Cookies.Add(httpCookie);
		}

		public T GetValue<T>(string key)
		{
			HttpCookie httpCookie;
			T value;

			httpCookie = HttpContext.Current.Request.Cookies[key];

			if ((object)httpCookie == null)
				return default(T);

			if (!DataType.TryParse(httpCookie.Value, out value))
				return default(T);

			return value;
		}

		public bool HasValue(string key)
		{
			HttpCookie httpCookie;

			httpCookie = HttpContext.Current.Request.Cookies[key];

			return (object)httpCookie != null && (object)httpCookie.Value != null;
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