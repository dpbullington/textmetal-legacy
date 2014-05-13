/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Web.Http;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc
{
	public static class WebApiConfig
	{
		#region Methods/Operators

		public static void Register(HttpConfiguration config)
		{
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new
						{
							id = RouteParameter.Optional
						}
				);
		}

		#endregion
	}
}