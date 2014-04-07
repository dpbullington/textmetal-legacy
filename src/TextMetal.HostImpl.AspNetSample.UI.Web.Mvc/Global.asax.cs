/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

//[assembly: TextMetal.Common.Solder.DependencyManagement.DependencyRegistration]
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel;
using TextMetal.HostImpl.AspNetSample.UI.Web.Shared;

using TextMetal.HostImpl.Web.AspNet;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc
{
	/// <summary>
	/// Note: For instructions on enabling IIS6 or IIS7 classic mode, visit http://go.microsoft.com/?LinkId=9394801
	/// </summary>
	//[TextMetal.Common.Solder.DependencyManagement.DependencyRegistration]
	public class MvcApplication : HttpApplication
	{
		#region Methods/Operators

		public static void OnDepenndencyRegistration()
		{
			Stuff.Set<IRepository, Repository>("");
			Stuff.Set<ISession, WebSession>("NonStickySession");
			Stuff.Set<ISession, WebStickySession>("StickySession");
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			this.OnApplicationError(sender, e);
		}

		protected void Application_Start()
		{
			TextMetalViewEngine.CallMeInGlobalAsax();

			if (WebConfig.RequireHttps)
				GlobalFilters.Filters.Add(new RequireHttpsAttribute());

			// PARTIAL TRUST HOSTING PREVENTS AUTO-WIRING
			OnDepenndencyRegistration();

			AreaRegistration.RegisterAllAreas();

			//WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}

		#endregion

		//[TextMetal.Common.Solder.DependencyManagement.DependencyRegistration]
	}
}