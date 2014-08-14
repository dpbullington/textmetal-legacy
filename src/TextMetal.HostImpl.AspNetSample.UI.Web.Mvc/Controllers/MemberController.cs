/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

using TextMetal.Common.Core;
using TextMetal.Common.Data.Framework;
using TextMetal.Common.Data.Framework.LinqToSql;
using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel;
using TextMetal.HostImpl.AspNetSample.DomainModel.Tables;
using TextMetal.HostImpl.AspNetSample.ServiceModel.Member;
using TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models;
using TextMetal.HostImpl.AspNetSample.UI.Web.Shared;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Controllers
{
	[HandleError]
	public class MemberController : TextMetalController
	{
		#region Constructors/Destructors

		public MemberController()
			: this(new MemberService())
		{
		}

		public MemberController(IMemberService memberService)
		{
			if ((object)memberService == null)
				throw new ArgumentNullException("memberService");

			this.memberService = memberService;
		}

		#endregion

		#region Fields/Constants

		private readonly IMemberService memberService;

		#endregion

		#region Properties/Indexers/Events

		private IMemberService MemberService
		{
			get
			{
				return this.memberService;
			}
		}

		#endregion

		#region Methods/Operators

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("create")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult CreateGet()
		{
			IList<IListItem<int?>> securityRoles;
			var model = this.CreateModel<MemberViewModel>();

			securityRoles = this.Repository.GetSecurityRoles().ToList();
			securityRoles.Insert(0, ListItem<int?>.Empty);
			model.MemberSecurityRoles = securityRoles.Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.SafeToString() });

			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("create")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult CreatePost()
		{
			MemberViewModel model;
			CreateMemberRequest request;
			CreateMemberResponse response;
			IList<IListItem<int?>> securityRoles;

			model = this.CreateModel<MemberViewModel>();

			securityRoles = this.Repository.GetSecurityRoles().ToList();
			securityRoles.Insert(0, ListItem<int?>.Empty);
			model.MemberSecurityRoles = securityRoles.Select(x => new SelectListItem()
																{
																	Text = x.Text, Value = x.Value.SafeToString()
																});

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

			request = new CreateMemberRequest()
					{
						EmailAddress = model.EmailAddress ?? "",
						UserName = model.Username ?? "",
						PasswordClearText = model.PasswordClearText ?? "",
						SecurityQuestion = model.SecurityQuestion ?? "",
						SecurityAnswerClearText = model.SecurityAnswerClearText ?? "",
						MemberName = model.MemberName ?? "",
						MemberSecurityRoleId = model.MemberSecurityRoleId
					};

			response = this.MemberService.CreateMember(request);

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
												controller = "Member",
												action = "List"
											}));
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("edit")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult EditGet(int id)
		{
			MemberViewModel model;
			LoadMemberRequest request;
			LoadMemberResponse response;
			IList<IListItem<int?>> securityRoles;

			model = this.CreateModel<MemberViewModel>();

			securityRoles = this.Repository.GetSecurityRoles().ToList();
			securityRoles.Insert(0, ListItem<int?>.Empty);
			model.MemberSecurityRoles = securityRoles.Select(x => new SelectListItem()
																{
																	Text = x.Text, Value = x.Value.SafeToString()
																});

			request = new LoadMemberRequest()
					{
						OrganizationId = (int)Current.OrganizationId,
						MemberId = id
					};

			response = this.MemberService.LoadMember(request);

			if ((object)response == null)
				throw new InvalidOperationException();

			model.EmailAddress = response.EmailAddress;
			model.EmailAddressConfirm = response.EmailAddress;
			model.MemberName = response.MemberName;
			model.SecurityQuestion = response.SecurityQuestion;
			model.SecurityQuestionConfirm = response.SecurityQuestion;
			model.Username = response.UserName;
			model.UsernameConfirm = response.UserName;
			model.MemberSecurityRoleId = response.MemberSecurityRoleId;

			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("edit")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult EditPost(int id)
		{
			MemberViewModel model;
			EditMemberRequest request;
			EditMemberResponse response;
			IList<IListItem<int?>> securityRoles;

			model = this.CreateModel<MemberViewModel>();

			securityRoles = this.Repository.GetSecurityRoles().ToList();
			securityRoles.Insert(0, ListItem<int?>.Empty);
			model.MemberSecurityRoles = securityRoles.Select(x => new SelectListItem()
																{
																	Text = x.Text, Value = x.Value.SafeToString()
																});

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

			request = new EditMemberRequest()
					{
						MemberId = id,
						UserId = id,
						EmailAddress = model.EmailAddress ?? "",
						UserName = model.Username ?? "",
						PasswordClearText = model.PasswordClearText ?? "",
						SecurityQuestion = model.SecurityQuestion ?? "",
						SecurityAnswerClearText = model.SecurityAnswerClearText ?? "",
						MemberName = model.MemberName ?? "",
						MemberSecurityRoleId = model.MemberSecurityRoleId
					};

			response = this.MemberService.EditMember(request);

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
												controller = "Member",
												action = "List"
											}));
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("list")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult ListGet()
		{
			MemberViewModel model;
			ListMembersRequest request;
			ListMembersResponse response;

			model = this.CreateModel<MemberViewModel>();

			request = new ListMembersRequest()
					{
						OrganizationId = (int)Current.OrganizationId
					};

			response = this.MemberService.ListMembers(request);

			if ((object)response == null)
				throw new InvalidOperationException();

			model.MemberResults = response.Results;

			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("remove")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult RemoveGet(int id)
		{
			var model = this.CreateModel<MemberViewModel>();
			return this.View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("remove")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult RemovePost(int id)
		{
			MemberViewModel model;
			RemoveMemberRequest request;
			RemoveMemberResponse response;

			model = this.CreateModel<MemberViewModel>();

			if (!this.TryUpdateModel(model))
				return this.View(model);

			request = new RemoveMemberRequest()
					{
						OrganizationId = (int)Current.OrganizationId,
						MemberId = id,
					};

			response = this.MemberService.RemoveMember(request);

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
												controller = "Member",
												action = "List"
											}));
		}

		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		[ActionName("search")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public JsonResult SearchGetJson(string value)
		{
			JsonResult result;
			IEnumerable<IMember> members;
			IModelQuery modelQuery;

			modelQuery = new LinqTableQuery<IMember>(e => e.OrganizationId == (int)Current.OrganizationId && e.LogicalDelete == false);

			members = this.Repository.Find<IMember>(modelQuery);

			members = members.Where(c => DataType.IsNullOrWhiteSpace(value) || c.MemberName.SafeToString().StartsWith(value)).ToList();

			result = new JsonResult();
			result.Data = members.Select(e => new
											{
												Text = e.MemberName.SafeToString(),
												Value = e.MemberId.SafeToString()
											});

			result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

			return result;
		}

		#endregion
	}
}