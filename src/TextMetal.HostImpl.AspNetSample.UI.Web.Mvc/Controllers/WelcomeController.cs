/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Web.Mvc;

using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.ServiceModel.Welcome;
using TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models;
using TextMetal.HostImpl.AspNetSample.UI.Web.Shared;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Controllers
{
	[HandleError]
	public class WelcomeController : TextMetalController
	{
		#region Constructors/Destructors

		public WelcomeController()
			: this(new WelcomeService())
		{
		}

		public WelcomeController(IWelcomeService welcomeService)
		{
			if ((object)welcomeService == null)
				throw new ArgumentNullException("welcomeService");

			this.welcomeService = welcomeService;
		}

		#endregion

		#region Fields/Constants

		private readonly IWelcomeService welcomeService;

		#endregion

		#region Properties/Indexers/Events

		private IWelcomeService WelcomeService
		{
			get
			{
				return this.welcomeService;
			}
		}

		#endregion

		#region Methods/Operators

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("about")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult AboutGet()
		{
			var model = this.CreateModel<WelcomeViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("contact")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult ContactGet()
		{
			var model = this.CreateModel<WelcomeViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("index")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult IndexGet()
		{
			var model = this.CreateModel<WelcomeViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("license")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult LicenseGet()
		{
			var model = this.CreateModel<WelcomeViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("privacy")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult PrivacyGet()
		{
			var model = this.CreateModel<WelcomeViewModel>();
			return this.View(model);
		}

		#endregion
	}
}