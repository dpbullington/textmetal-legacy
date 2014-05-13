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
using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel.Tables;
using TextMetal.HostImpl.AspNetSample.ServiceModel.Welcome;
using TextMetal.HostImpl.AspNetSample.UI.Web.Shared;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Controllers
{
	[HandleError]
	public class WidgetController : TextMetalController
	{
		#region Constructors/Destructors

		public WidgetController()
			: this(new WelcomeService())
		{
		}

		public WidgetController(IWelcomeService welcomeService)
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
		[ActionName("create")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult CreateGet()
		{
			return this.Load(null);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("create")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult CreatePost()
		{
			return this.Save(null);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("delete")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult DeleteGet(int id)
		{
			IEnumerable<Widget> widgets;
			Widget widget;

			widgets =
				this.Repository.FindWidgets(
					q => q.Where(cu => cu.WidgetId == id && cu.LogicalDelete == false));

			widget = widgets.SingleOrDefault();

			if ((object)widget == null)
				throw new InvalidOperationException("not found");

			if (!this.Repository.DiscardWidget(widget))
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "A conflict error occured.", Severity.Error) });

				return this.View(widget);
			}

			return
				new RedirectToRouteResult(
					new RouteValueDictionary(new
											{
												controller = "Widget",
												action = "List"
											}));
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("edit")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult EditGet(int id)
		{
			return this.Load(id);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[ActionName("edit")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult EditPost(int id)
		{
			return this.Save(id);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		[ActionName("list")]
		[SecureAction(AllowedSecurityRole = SecurityRoleEnum.Master)]
		public ActionResult ListGet()
		{
			IEnumerable<Widget> widgets;

			widgets =
				this.Repository.FindWidgets(
					q => q.Where(cu => cu.LogicalDelete == false).OrderBy(cu => cu.WidgetName));

			return this.View(widgets);
		}

		private ActionResult Load(int? widgetId)
		{
			IEnumerable<Widget> widgets;
			Widget widget;

			if ((object)widgetId == null)
				widget = new Widget();
			else
			{
				widgets =
					this.Repository.FindWidgets(
						q => q.Where(cu => cu.WidgetId == (int)widgetId && cu.LogicalDelete == false));

				widget = widgets.SingleOrDefault();

				if ((object)widget == null)
					throw new InvalidOperationException("not found");
			}

			return this.View(widget);
		}

		private ActionResult Save(int? widgetId)
		{
			IEnumerable<Widget> widgets;
			Widget widget;
			Message[] messages;
			bool wasNew;

			if (wasNew = ((object)widgetId == null))
				widget = new Widget();
			else
			{
				widgets =
					this.Repository.FindWidgets(
						q => q.Where(cu => cu.WidgetId == (int)widgetId && cu.LogicalDelete == false));

				widget = widgets.SingleOrDefault();

				if ((object)widget == null)
					throw new InvalidOperationException("not found");
			}

			if (!this.TryUpdateModel(widget))
				return this.View(widget);

			messages = widget.Validate();

			if ((object)messages == null)
				throw new InvalidOperationException();

			if (messages.Length > 0)
			{
				this.ViewData.Add("_ShowMessages", messages);

				return this.View(widget);
			}

			if (!this.Repository.SaveWidget(widget))
			{
				this.ViewData.Add("_ShowMessages", new[] { new Message("", "A conflict error occured.", Severity.Error) });

				return this.View(widget);
			}

			return
				new RedirectToRouteResult(
					new RouteValueDictionary(new
											{
												controller = "Widget",
												action = "List"
											}));
		}

		#endregion
	}
}