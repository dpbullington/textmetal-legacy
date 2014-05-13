/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

using TextMetal.Common.Core;
using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel;
using TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models;
using TextMetal.HostImpl.AspNetSample.UI.Web.Shared;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Controllers
{
	[HandleError]
	public abstract class TextMetalController : BaseController
	{
		#region Constructors/Destructors

		protected TextMetalController()
			: this(Stuff.Get<IRepository>(""))
		{
		}

		protected TextMetalController(IRepository repository)
			: base(repository)
		{
		}

		#endregion

		#region Properties/Indexers/Events

		public static bool UserWasAuthenticated
		{
			get
			{
				return (object)Current.UserId != null;
			}
		}

		#endregion

		#region Methods/Operators

		protected void ClearSession()
		{
			Current.Exit();
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			RequiresCurrentAttribute requiresCurrentAttribute;

			base.OnActionExecuting(filterContext);

			requiresCurrentAttribute = Reflexion.GetOneAttribute<RequiresCurrentAttribute>(filterContext.ActionDescriptor);

			if (requiresCurrentAttribute != null &&
				(object)Current.dummy == null)
			{
				filterContext.Result =
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "Dummy",
													action = "Choose"
												}));

				return;
			}
		}

		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			SecureActionAttribute secureActionAttribute;

			base.OnAuthorization(filterContext);

			secureActionAttribute = Reflexion.GetOneAttribute<SecureActionAttribute>(filterContext.ActionDescriptor);

			if ((object)secureActionAttribute == null)
				throw new InvalidOperationException(string.Format("'{0}' missing on action '{1}'.", typeof(SecureActionAttribute).FullName, filterContext.ActionDescriptor.ActionName));

			if (secureActionAttribute.AllowedSecurityRole == SecurityRoleEnum.None)
			{
				filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotFound);
				return;
			}

			if (secureActionAttribute.AllowedSecurityRole != SecurityRoleEnum.Anonymous)
			{
				if ((object)Current.UserId == null)
				{
					filterContext.Result =
						new RedirectToRouteResult(
							new RouteValueDictionary(new
													{
														controller = "User",
														action = "Login"
													}));

					return;
				}
			}

			if (Current.MustChangePassword &&
				!(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == "user" &&
				filterContext.ActionDescriptor.ActionName.ToLower() == "changepassword"))
			{
				filterContext.Result =
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "User",
													action = "ChangePassword"
												}));

				return;
			}
		}

		protected override void OnCreateModel<TViewModel>(TViewModel model)
		{
			TextMetalViewModel m;

			m = (TextMetalViewModel)(object)model;
			m.UserWasAuthenticated = UserWasAuthenticated;
		}

		#endregion
	}
}