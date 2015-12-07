/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using Microsoft.AspNet.Http;

namespace TextMetal.Middleware.Solder.Context
{
	public sealed class HttpContextAccessorContextualStorageStrategy : IContextualStorageStrategy
	{
		#region Constructors/Destructors

		public HttpContextAccessorContextualStorageStrategy()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly IHttpContextAccessor __ = null;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets a value indicating if the current application domain is running under ASP.NET.
		/// </summary>
		public static bool IsInHttpContext
		{
			get
			{
				return (object)new HttpContextAccessorContextualStorageStrategy().__.HttpContext != null;
			}
		}

		#endregion

		#region Methods/Operators

		public static string GetApplicationRootPhysicalPath()
		{
			return null; //xx.MapPath("~/");
		}

		public T GetValue<T>(string key)
		{
			return (T)this.__.HttpContext.Items[key];
		}

		public bool HasValue(string key)
		{
			return this.__.HttpContext.Items.ContainsKey(key);
		}

		public void RemoveValue(string key)
		{
			this.__.HttpContext.Items.Remove(key);
		}

		public void SetValue<T>(string key, T value)
		{
			this.__.HttpContext.Items[key] = value;
		}

		#endregion
	}
}