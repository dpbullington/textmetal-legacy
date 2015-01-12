/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

using TextMetal.Common.Core;
using TextMetal.Common.Solder.DependencyManagement;

namespace TextMetal.Common.WebApps.Mvc
{
	[HandleError]
	public abstract class BaseController : Controller
	{
		#region Constructors/Destructors

		protected BaseController()
		{
			this.assemblyInformation = new AssemblyInformation(Assembly.GetAssembly(this.GetType()));
		}

		#endregion

		#region Fields/Constants

		private readonly AssemblyInformation assemblyInformation;

		#endregion

		#region Properties/Indexers/Events

		protected AssemblyInformation AssemblyInformation
		{
			get
			{
				return this.assemblyInformation;
			}
		}

		protected virtual long? CurrentUserId
		{
			get
			{
				return null;
			}
		}

		protected virtual bool CurrentUserMustChangePassword
		{
			get
			{
				return false;
			}
		}

		protected virtual string CurrentUserMustChangePasswordActionName
		{
			get
			{
				return "changepassword";
			}
		}

		protected virtual string CurrentUserMustChangePasswordControllerName
		{
			get
			{
				return "user";
			}
		}

		protected virtual string CurrentUserMustLoginActionName
		{
			get
			{
				return "login";
			}
		}

		protected virtual string CurrentUserMustLoginControllerName
		{
			get
			{
				return "user";
			}
		}

		protected virtual bool CurrentUserWasAuthenticated
		{
			get
			{
				return (object)this.CurrentUserId != null;
			}
		}

		protected IApplicationStorage StandardWebSession
		{
			get
			{
				return DependencyManager.AppDomainInstance.ResolveDependency<IApplicationStorage>(ProjectConstants.DEP_SEL_STANDARD_WEB_SESSION);
			}
		}

		protected IApplicationStorage StickyWebSession
		{
			get
			{
				return DependencyManager.AppDomainInstance.ResolveDependency<IApplicationStorage>(ProjectConstants.DEP_SEL_STICKY_WEB_SESSION);
			}
		}

		#endregion

		#region Methods/Operators

		protected abstract void ClearSession();

		protected TViewModel CreateModel<TViewModel>()
			where TViewModel : BaseViewModel, new()
		{
			TViewModel model;

			model = new TViewModel();
			model.AssemblyInformation = this.AssemblyInformation;
			model.CurrentUserWasAuthenticated = this.CurrentUserWasAuthenticated;
			model.CurrentUserId = this.CurrentUserId;
			model.CurrentUserMustChangePassword = this.CurrentUserMustChangePassword;

			this.OnCreateModel<TViewModel>(model);

			return model;
		}

		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			AuthenticationRequirementAttribute authenticationRequirementAttribute;

			base.OnAuthorization(filterContext);

			authenticationRequirementAttribute = Reflexion.Instance.GetOneAttribute<AuthenticationRequirementAttribute>(filterContext.ActionDescriptor);

			if ((object)authenticationRequirementAttribute == null)
				throw new InvalidOperationException(string.Format("'{0}' missing on action '{1}'.", typeof(AuthenticationRequirementAttribute).FullName, filterContext.ActionDescriptor.ActionName));

			if (authenticationRequirementAttribute.AuthenticationOutcome == AuthenticationOutcome.None)
			{
				filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotFound);
				return;
			}

			if (authenticationRequirementAttribute.AuthenticationOutcome >= AuthenticationOutcome.Authenticated)
			{
				if (!this.CurrentUserWasAuthenticated)
				{
					filterContext.Result =
						new RedirectToRouteResult(
							new RouteValueDictionary(new
													{
														controller = this.CurrentUserMustLoginControllerName,
														action = this.CurrentUserMustLoginActionName
													}));

					return;
				}

				//this.OnTrueRoleBasedAuthorization();
			}

			if (this.CurrentUserMustChangePassword &&
				!(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == this.CurrentUserMustChangePasswordControllerName &&
				filterContext.ActionDescriptor.ActionName.ToLower() == this.CurrentUserMustChangePasswordActionName))
			{
				filterContext.Result =
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = this.CurrentUserMustChangePasswordControllerName,
													action = this.CurrentUserMustChangePasswordActionName
												}));

				return;
			}
		}

		protected abstract void OnCreateModel<TViewModel>(TViewModel model)
			where TViewModel : BaseViewModel, new();

		#endregion
	}
}