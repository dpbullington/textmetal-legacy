/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Web;

using TextMetal.HostImpl.AspNetSample.Common;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Shared
{
	public class WebSession : ISession
	{
		#region Constructors/Destructors

		public WebSession()
		{
		}

		#endregion

		#region Methods/Operators

		public void FreeValue(string key)
		{
			HttpContext.Current.Session.Remove(key);
		}

		public T GetValue<T>(string key)
		{
			object value;

			value = HttpContext.Current.Session[key];

			return (T)value;
		}

		public bool HasValue(string key)
		{
			return (object)HttpContext.Current.Session[key] != null;
		}

		public void SetValue<T>(string key, T value)
		{
			HttpContext.Current.Session.Add(key, value);
		}

		#endregion
	}
}