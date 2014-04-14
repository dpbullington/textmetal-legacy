/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

using TextMetal.Common.Core;
using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel;
using TextMetal.HostImpl.AspNetSample.ServiceModel.SignUp;
using TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models;
using TextMetal.HostImpl.AspNetSample.UI.Web.Shared;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Controllers
{
	[HandleError]
	public class SignUpController : TextMetalController
	{
		#region Constructors/Destructors

		public SignUpController()
			: this(new SignUpService())
		{
		}

		public SignUpController(ISignUpService signUpService)
		{
			if ((object)signUpService == null)
				throw new ArgumentNullException("signUpService");

			this.signUpService = signUpService;
		}

		#endregion

		#region Fields/Constants

		private readonly ISignUpService signUpService;

		#endregion

		#region Properties/Indexers/Events

		public static SignUpViewModel CuurentSignUpViewModel
		{
			get
			{
				SignUpViewModel value;

				value = Stuff.Session.GetValue<SignUpViewModel>("CuurentSignUpViewModel");

				return value;
			}
			set
			{
				Stuff.Session.FreeValue("CuurentSignUpViewModel");
				Stuff.Session.SetValue<SignUpViewModel>("CuurentSignUpViewModel", value);
			}
		}

		private ISignUpService SignUpService
		{
			get
			{
				return this.signUpService;
			}
		}

		#endregion

		#region Methods/Operators

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("done")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Authenticated)]
		public ActionResult DoneGet()
		{
			SignUpViewModel model;

			if (!WebConfig.AllowSignUps)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "SignUp",
													action = "Index"
												}));
			}

			model = CuurentSignUpViewModel ?? this.CreateModel<SignUpViewModel>();
			this.OnCreateModel(model);

			if (UserWasAuthenticated && !(model.SignUpCompleted ?? false))
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "Welcome",
													action = "Index"
												}));
			}

			if (!model.SignUpCompleted ?? false)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "SignUp",
													action = string.Format("Step{0:00}", 1)
												}));
			}

			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("index")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult IndexGet(string edition)
		{
			var model = CuurentSignUpViewModel ?? this.CreateModel<SignUpViewModel>();

			if (!WebConfig.AllowSignUps)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "New sign-ups are not currently being accepted. Please send an email to <a href=\"mailto:preview@textmetal.com\">preview@textmetal.com</a> if you wish to be reconsidered.", Severity.Information) });

				return this.View();
			}

			if (UserWasAuthenticated)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "Welcome",
													action = "Index"
												}));
			}

			if ((object)model.StepCompleted != null)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "SignUp",
													action = string.Format("Step{0:00}", (int)model.StepCompleted)
												}));
			}

			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("step01")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult Step01Get()
		{
			var model = CuurentSignUpViewModel ?? this.CreateModel<SignUpViewModel>();

			if (!WebConfig.AllowSignUps)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "SignUp",
													action = "Index"
												}));
			}

			if (UserWasAuthenticated)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "Welcome",
													action = "Index"
												}));
			}

			CuurentSignUpViewModel = model;

			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("step01")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult Step01Post()
		{
			SignUpViewModel model;
			const int CURRENT_STEP = 1;

			if (!WebConfig.AllowSignUps)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "SignUp",
													action = "Index"
												}));
			}

			if (UserWasAuthenticated)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "Welcome",
													action = "Index"
												}));
			}

			model = CuurentSignUpViewModel ?? this.CreateModel<SignUpViewModel>();

			if ((model.StepCompleted ?? 0) < (CURRENT_STEP - 1))
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "SignUp",
													action = string.Format("Step{0:00}", CURRENT_STEP)
												}));
			}

			if (!this.TryUpdateModel(model))
				return this.View(model);

			model.StepCompleted = 1;

			return
				new RedirectToRouteResult(
					new RouteValueDictionary(new
											{
												controller = "SignUp",
												action = "Step02"
											}));
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("step02")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult Step02Get()
		{
			SignUpViewModel model;
			const int CURRENT_STEP = 2;

			if (!WebConfig.AllowSignUps)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "SignUp",
													action = "Index"
												}));
			}

			if (UserWasAuthenticated)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "Welcome",
													action = "Index"
												}));
			}

			model = CuurentSignUpViewModel ?? this.CreateModel<SignUpViewModel>();

			if ((model.StepCompleted ?? 0) < (CURRENT_STEP - 1))
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "SignUp",
													action = string.Format("Step{0:00}", CURRENT_STEP - 1)
												}));
			}

			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("step02")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult Step02Post()
		{
			SignUpViewModel model;
			SignUpRequest request;
			SignUpResponse response;
			const int CURRENT_STEP = 1;

			if (!WebConfig.AllowSignUps)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "SignUp",
													action = "Index"
												}));
			}

			if (UserWasAuthenticated)
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "Welcome",
													action = "Index"
												}));
			}

			model = CuurentSignUpViewModel ?? this.CreateModel<SignUpViewModel>();

			if ((model.StepCompleted ?? 0) < (CURRENT_STEP - 1))
			{
				return
					new RedirectToRouteResult(
						new RouteValueDictionary(new
												{
													controller = "SignUp",
													action = string.Format("Step{0:00}", CURRENT_STEP - 1)
												}));
			}

			if (!this.TryUpdateModel(model))
				return this.View(model);

			model.StepCompleted = 2;

			//+++++++++++++

			if (model.Username != model.UsernameConfirm)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "Usernames do not match.", Severity.Error) });

				return this.View(model);
			}

			if (model.EmailAddress != model.EmailAddressConfirm)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "Email addresses do not match.", Severity.Error) });

				return this.View(model);
			}

			if (model.PasswordClearText != model.PasswordClearTextConfirm)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "Passwords do not match.", Severity.Error) });

				return this.View(model);
			}

			if (model.SecurityQuestion != model.SecurityQuestionConfirm)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "Security questions do not match.", Severity.Error) });

				return this.View(model);
			}

			if (model.SecurityAnswerClearText != model.SecurityAnswerClearTextConfirm)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "Security answers do not match.", Severity.Error) });

				return this.View(model);
			}

			request = new SignUpRequest()
					{
						// step 1
						OrganizationName = model.OrganizationName ?? "",
						
						// step 2
						EmailAddress = model.EmailAddress ?? "",
						UserName = model.Username ?? "",
						PasswordClearText = model.PasswordClearText ?? "",
						SecurityQuestion = model.SecurityQuestion ?? "",
						SecurityAnswerClearText = model.SecurityAnswerClearText ?? "",
						MemberName = model.MemberName ?? ""
					};

			response = this.SignUpService.SignUp(request);

			if ((object)response == null)
				throw new InvalidOperationException();

			if ((object)response.Messages != null && response.Messages.Any())
			{
				this.ViewData.Add("_ShowMessages", response.Messages);

				return this.View(model);
			}

			Current.UserId = response.UserId;
			Current.MemberId = response.MemberId;
			Current.OrganizationId = response.OrganizationId;

			model.SignUpCompleted = true;

			//+++++++++++++

			return
				new RedirectToRouteResult(
					new RouteValueDictionary(new
											{
												controller = "SignUp",
												action = "Done"
											}));
		}

		#endregion
	}
}