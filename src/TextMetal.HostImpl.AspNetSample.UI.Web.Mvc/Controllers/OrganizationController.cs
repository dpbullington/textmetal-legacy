/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

using TextMetal.Common.Core;
using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel;
using TextMetal.HostImpl.AspNetSample.DomainModel.Tables;
using TextMetal.HostImpl.AspNetSample.UI.Web.Shared;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Controllers
{
	[HandleError]
	public class OrganizationController : TextMetalController
	{
		#region Methods/Operators

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("create")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.None)]
		public ActionResult CreateGet()
		{
			return this.Load(null);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("create")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.None)]
		public ActionResult CreatePost()
		{
			return this.Save(null);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("dashboard")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult DashboardGet()
		{
			return this.Load(Current.OrganizationId);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("delete")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult DeleteGet()
		{
			return this.Load(Current.OrganizationId);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("delete")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult DeletePost()
		{
			IOrganization organization;

			organization = this.Repository.LoadOrganization((int)Current.OrganizationId); // current OK no filter

			// in the case of a organization, it is a LOGICAL delete!
			organization.LogicalDelete = true;

			if (!this.Repository.SaveOrganization(organization))
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "A conflict error occured.", Severity.Error) });

				return this.View(organization);
			}

			Current.Exit();

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
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult EditGet()
		{
			return this.Load(Current.OrganizationId);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("edit")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult EditPost()
		{
			return this.Save(Current.OrganizationId);
		}

		private ActionResult Load(int? organizationId)
		{
			IOrganization organization;

			if ((object)organizationId == null)
				organization = new Organization();
			else
				organization = this.Repository.LoadOrganization((int)organizationId); // current OK no filter

			return this.View(organization);
		}

		private ActionResult Save(int? organizationId)
		{
			IOrganization organization;
			IEnumerable<Message> messages;

			if ((object)organizationId == null)
				organization = new Organization();
			else
				organization = this.Repository.LoadOrganization((int)organizationId); // any=current OK no filter

			if (!this.TryUpdateModel(organization))
				return this.View(organization);

			messages = organization.Validate();

			if ((object)messages == null)
				throw new InvalidOperationException();

			if (messages.Count() > 0)
			{
				this.ViewData.Add("_ShowMessages", messages);

				return this.View(organization);
			}

			if (!this.Repository.SaveOrganization(organization))
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "A conflict error occured.", Severity.Error) });

				return this.View(organization);
			}

			Current.OrganizationId = organization.OrganizationId;

			return
				new RedirectToRouteResult(
					new RouteValueDictionary(new
											{
												controller = "Organization",
												action = "Dashboard"
											}));
		}

		#endregion
	}
}