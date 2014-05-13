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
using TextMetal.HostImpl.AspNetSample.ServiceModel.User;
using TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models;
using TextMetal.HostImpl.AspNetSample.UI.Web.Shared;

using _User = TextMetal.HostImpl.AspNetSample.DomainModel.Tables.User;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Controllers
{
	[HandleError]
	public class UserController : TextMetalController
	{
		#region Constructors/Destructors

		public UserController()
			: this(new UserService())
		{
		}

		public UserController(IUserService userService)
		{
			if ((object)userService == null)
				throw new ArgumentNullException("userService");

			this.userService = userService;
		}

		#endregion

		#region Fields/Constants

		private readonly IUserService userService;

		#endregion

		#region Properties/Indexers/Events

		public static int SessionLockFailedLoginCountPolicy
		{
			get
			{
				return ServiceModel.User.UserService.FAILED_LOGIN_COUNT_POLICY + 1;
			}
		}

		private IUserService UserService
		{
			get
			{
				return this.userService;
			}
		}

		#endregion

		#region Methods/Operators

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("changepassword")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Authenticated)]
		public ActionResult ChangePasswordGet()
		{
			var model = this.CreateModel<UserViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("changepassword")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Authenticated)]
		public ActionResult ChangePasswordPost()
		{
			UserViewModel model;
			EditUserRequest request;
			EditUserResponse response;

			model = this.CreateModel<UserViewModel>();

			if (!this.TryUpdateModel(model))
				return this.View(model);

			if (model.PasswordClearText != model.PasswordClearTextConfirm)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "Passwords do not match.", Severity.Error) });

				return this.View(model);
			}

			request = new EditUserRequest()
					{
						UserId = Current.UserId,
						MustChangePassword = Current.MustChangePassword,
						PasswordClearText = model.PasswordClearText ?? "",
					};

			response = this.UserService.EditUser(request);

			if ((object)response == null)
				throw new InvalidOperationException();

			if ((object)response.Messages != null && response.Messages.Any())
			{
				this.ViewData.Add("_ShowMessages", response.Messages);

				return this.View(model);
			}

			Current.MustChangePassword = response.MustChangePassword;

			return
				new RedirectToRouteResult(
					new RouteValueDictionary(new
											{
												controller = "Welcome",
												action = "Index"
											}));
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("create")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.None)]
		public ActionResult CreateGet()
		{
			var model = this.CreateModel<UserViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("create")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.None)]
		public ActionResult CreatePost()
		{
			UserViewModel model;
			CreateUserRequest request;
			CreateUserResponse response;

			model = this.CreateModel<UserViewModel>();

			if (!this.TryUpdateModel(model))
				return this.View(model);

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

			request = new CreateUserRequest()
					{
						EmailAddress = model.EmailAddress ?? "",
						UserName = model.Username ?? "",
						PasswordClearText = model.PasswordClearText ?? "",
						SecurityQuestion = model.SecurityQuestion ?? "",
						SecurityAnswerClearText = model.SecurityAnswerClearText ?? ""
					};

			response = this.UserService.CreateUser(request);

			if ((object)response == null)
				throw new InvalidOperationException();

			if ((object)response.Messages != null && response.Messages.Any())
			{
				this.ViewData.Add("_ShowMessages", response.Messages);

				return this.View(model);
			}

			return
				new RedirectToRouteResult(
					new RouteValueDictionary(new
											{
												controller = "Welcome",
												action = "Index"
											}));
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("edit")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Authenticated)]
		public ActionResult EditGet()
		{
			var user = this.UserService.LoadUser(new LoadUserRequest()
												{
													UserId = Current.UserId
												});

			var model = this.CreateModel<UserViewModel>();
			model.EmailAddress = user.EmailAddress;
			model.EmailAddressConfirm = user.EmailAddress;
			model.Username = user.UserName;
			model.UsernameConfirm = user.UserName;
			model.PasswordClearText = "";
			model.PasswordClearTextConfirm = "";
			model.SecurityQuestion = user.Question;
			model.SecurityQuestionConfirm = user.Question;
			model.SecurityAnswerClearText = "";
			model.SecurityAnswerClearTextConfirm = "";

			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("edit")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Authenticated)]
		public ActionResult EditPost()
		{
			UserViewModel model;
			EditUserRequest request;
			EditUserResponse response;

			model = this.CreateModel<UserViewModel>();

			if (!this.TryUpdateModel(model))
				return this.View(model);

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

			request = new EditUserRequest()
					{
						UserId = Current.UserId,
						MustChangePassword = Current.MustChangePassword,
						EmailAddress = model.EmailAddress ?? "",
						UserName = model.Username ?? "",
						PasswordClearText = model.PasswordClearText ?? "",
						SecurityQuestion = model.SecurityQuestion ?? "",
						SecurityAnswerClearText = model.SecurityAnswerClearText ?? ""
					};

			response = this.UserService.EditUser(request);

			if ((object)response == null)
				throw new InvalidOperationException();

			if ((object)response.Messages != null && response.Messages.Any())
			{
				this.ViewData.Add("_ShowMessages", response.Messages);

				return this.View(model);
			}

			return
				new RedirectToRouteResult(
					new RouteValueDictionary(new
											{
												controller = "Welcome",
												action = "Index"
											}));
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("forgotpassword")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult ForgotPasswordGet()
		{
			var model = this.CreateModel<UserViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("forgotpassword")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult ForgotPasswordPost()
		{
			UserViewModel model;
			ForgotPasswordRequest request;
			ForgotPasswordResponse response;

			model = this.CreateModel<UserViewModel>();

			if (!this.TryUpdateModel(model))
				return this.View(model);

			if (model.Username != model.UsernameConfirm)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "Usernames do not match.", Severity.Error) });

				return this.View(model);
			}

			if (model.SecurityAnswerClearText != model.SecurityAnswerClearTextConfirm)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "Security answers do not match.", Severity.Error) });

				return this.View(model);
			}

			request = new ForgotPasswordRequest()
					{
						UserName = model.Username ?? "",
						SecurityAnswerClearText = model.SecurityAnswerClearText ?? "",
						FailedLoginCount = Current.FailedLoginCount
					};

			if (request.FailedLoginCount >= SessionLockFailedLoginCountPolicy)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "The session is locked.", Severity.Error) });

				return this.View(model);
			}

			response = this.UserService.ForgotPassword(request);

			if ((object)response == null)
				throw new InvalidOperationException();

			Current.FailedLoginCount = response.FailedLoginCount;

			if ((object)response.Messages != null && response.Messages.Any())
			{
				this.ViewData.Add("_ShowMessages", response.Messages);

				return this.View(model);
			}

			model = this.CreateModel<UserViewModel>();
			this.ModelState.Clear();

			if (!response.EmailSent)
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "An email was attempted but failed to be sent.", Severity.Error) });
			else
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "An email has been sent to the registered email address containing the username.", Severity.Error) });

			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("forgotusername")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult ForgotUsernameGet()
		{
			var model = this.CreateModel<UserViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("forgotusername")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult ForgotUsernamePost()
		{
			UserViewModel model;
			ForgotUsernameRequest request;
			ForgotUsernameResponse response;

			model = this.CreateModel<UserViewModel>();

			if (!this.TryUpdateModel(model))
				return this.View(model);

			if (model.EmailAddress != model.EmailAddressConfirm)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "Email addresses do not match.", Severity.Error) });

				return this.View(model);
			}

			request = new ForgotUsernameRequest()
					{
						EmailAddress = model.EmailAddress ?? "",
						FailedLoginCount = Current.FailedLoginCount
					};

			if (request.FailedLoginCount >= SessionLockFailedLoginCountPolicy)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "The session is locked.", Severity.Error) });

				return this.View(model);
			}

			response = this.UserService.ForgotUsername(request);

			if ((object)response == null)
				throw new InvalidOperationException();

			Current.FailedLoginCount = response.FailedLoginCount;

			if ((object)response.Messages != null && response.Messages.Any())
			{
				this.ViewData.Add("_ShowMessages", response.Messages);

				return this.View(model);
			}

			model = this.CreateModel<UserViewModel>();
			this.ModelState.Clear();

			if (!response.EmailSent)
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "An email was attempted but failed to be sent.", Severity.Error) });
			else
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "An email has been sent to the registered email address containing the username.", Severity.Error) });

			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("login")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult LoginGet()
		{
			var model = this.CreateModel<UserViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("login")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Anonymous)]
		public ActionResult LoginPost()
		{
			UserViewModel model;
			AuthenticateUserRequest request;
			AuthenticateUserResponse response;

			model = this.CreateModel<UserViewModel>();

			if (!this.TryUpdateModel(model))
				return this.View(model);

			request = new AuthenticateUserRequest()
					{
						UserName = model.Username ?? "",
						PasswordClearText = model.PasswordClearText ?? "",
						RememberMe = model.RememberMe,
						FailedLoginCount = Current.FailedLoginCount
					};

			if (request.FailedLoginCount >= SessionLockFailedLoginCountPolicy)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "The session is locked.", Severity.Error) });

				return this.View(model);
			}

			response = this.UserService.AuthenticateUser(request);

			if ((object)response == null)
				throw new InvalidOperationException();

			Current.FailedLoginCount = response.FailedLoginCount;

			if ((object)response.Messages != null && response.Messages.Any())
			{
				this.ViewData.Add("_ShowMessages", response.Messages);

				return this.View(model);
			}

			Current.UserId = response.UserId;
			Current.MemberId = response.MemberId;
			Current.OrganizationId = response.OrganizationId;
			Current.ShouldRememberMeToken = response.RememberMeToken;
			Current.MustChangePassword = response.MustChangePassword;
			Current.SignUpOrganizationNotFinalized = null;

			return
				new RedirectToRouteResult(
					new RouteValueDictionary(new
											{
												controller = "Welcome",
												action = "Index"
											}));
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("logout")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Authenticated)]
		public ActionResult LogoutGet()
		{
			var model = this.CreateModel<UserViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("logout")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Authenticated)]
		public ActionResult LogoutPost()
		{
			var model = this.CreateModel<UserViewModel>();

			this.ClearSession(); // force session end

			return
				new RedirectToRouteResult(
					new RouteValueDictionary(new
											{
												controller = "Welcome",
												action = "Index"
											}));
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("suspendaccount")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Authenticated)]
		public ActionResult SuspendAccountGet()
		{
			var model = this.CreateModel<UserViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("suspendaccount")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Authenticated)]
		public ActionResult SuspendAccountPost()
		{
			UserViewModel model;
			SuspendAccountRequest request;
			SuspendAccountResponse response;

			model = this.CreateModel<UserViewModel>();

			if (!this.TryUpdateModel(model))
				return this.View(model);

			if (!model.SuspendAccount)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "The user account was not suspended.", Severity.Error) });

				return this.View(model);
			}

			if (model.PasswordClearText != model.PasswordClearTextConfirm)
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "Passwords do not match.", Severity.Error) });

				return this.View(model);
			}

			request = new SuspendAccountRequest()
					{
						UserId = Current.UserId,
						PasswordClearText = model.PasswordClearText ?? ""
					};

			response = this.UserService.SuspendAccount(request);

			if ((object)response == null)
				throw new InvalidOperationException();

			if ((object)response.Messages != null && response.Messages.Any())
			{
				this.ViewData.Add("_ShowMessages", response.Messages);

				return this.View(model);
			}

			this.ClearSession(); // force end session

			return
				new RedirectToRouteResult(
					new RouteValueDictionary(new
											{
												controller = "Welcome",
												action = "Index"
											}));
		}

		#endregion
	}
}