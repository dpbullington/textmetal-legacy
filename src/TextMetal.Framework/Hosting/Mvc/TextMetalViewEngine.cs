/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Framework.Hosting.Mvc
{
#if USE_ASP_NET_LIBS
	public class TextMetalViewEngine : VirtualPathProviderViewEngine
	{
		#region Constructors/Destructors

		public TextMetalViewEngine()
		{
			this.MasterLocationFormats = new string[]
										{
											"~/Views/{1}/{0}" + this.TEXT_METAL_PAGE_EXTENSION,
											"~/Views/{1}/{0}.master",
											"~/Views/Shared/{0}" + this.TEXT_METAL_PAGE_EXTENSION,
											"~/Views/Shared/{0}.master"
										};

			this.AreaMasterLocationFormats = new string[]
											{
												"~/Areas/{2}/Views/{1}/{0}" + this.TEXT_METAL_PAGE_EXTENSION,
												"~/Areas/{2}/Views/{1}/{0}.master",
												"~/Areas/{2}/Views/Shared/{0}" + this.TEXT_METAL_PAGE_EXTENSION,
												"~/Areas/{2}/Views/Shared/{0}.master"
											};

			this.ViewLocationFormats = new string[]
										{
											"~/Views/{1}/{0}" + this.TEXT_METAL_PAGE_EXTENSION,
											"~/Views/{1}/{0}.aspx",
											"~/Views/{1}/{0}.ascx",
											"~/Views/Shared/{0}" + this.TEXT_METAL_PAGE_EXTENSION,
											"~/Views/Shared/{0}.aspx",
											"~/Views/Shared/{0}.ascx"
										};

			this.AreaViewLocationFormats = new string[]
											{
												"~/Areas/{2}/Views/{1}/{0}" + this.TEXT_METAL_PAGE_EXTENSION,
												"~/Areas/{2}/Views/{1}/{0}.aspx",
												"~/Areas/{2}/Views/{1}/{0}.ascx",
												"~/Areas/{2}/Views/Shared/{0}" + this.TEXT_METAL_PAGE_EXTENSION,
												"~/Areas/{2}/Views/Shared/{0}.aspx",
												"~/Areas/{2}/Views/Shared/{0}.ascx"
											};

			this.PartialViewLocationFormats = this.ViewLocationFormats;
			this.AreaPartialViewLocationFormats = this.AreaViewLocationFormats;
		}

		#endregion

		#region Fields/Constants

		private readonly string TEXT_METAL_PAGE_EXTENSION = ".tmpx";

		#endregion

		#region Methods/Operators

		public static void CallMeInGlobalAsax()
		{
			if (ViewEngines.Engines.Count(e => e.GetType() == typeof(TextMetalViewEngine)) == 0)
				ViewEngines.Engines.Add(new TextMetalViewEngine());
		}

		protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
		{
			if ((object)controllerContext == null)
				throw new ArgumentNullException("controllerContext");

			if ((object)partialPath == null)
				throw new ArgumentNullException("partialPath");

			if (DataTypeFascade.Instance.IsWhiteSpace(partialPath))
				throw new ArgumentOutOfRangeException("partialPath");

			if (partialPath.EndsWith(this.TEXT_METAL_PAGE_EXTENSION))
				return new TextMetalView(partialPath);
			else
				return new WebFormView(controllerContext, partialPath, null);
		}

		protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
		{
			if ((object)controllerContext == null)
				throw new ArgumentNullException("controllerContext");

			if ((object)viewPath == null)
				throw new ArgumentNullException("viewPath");

			if ((object)masterPath == null)
				throw new ArgumentNullException("masterPath");

			if (DataTypeFascade.Instance.IsWhiteSpace(viewPath))
				throw new ArgumentOutOfRangeException("viewPath");

			if (viewPath.EndsWith(this.TEXT_METAL_PAGE_EXTENSION) && DataTypeFascade.Instance.IsNullOrWhiteSpace(masterPath))
				return new TextMetalView(viewPath);
			else if (viewPath.EndsWith(this.TEXT_METAL_PAGE_EXTENSION) && !String.IsNullOrEmpty(masterPath))
				return new TextMetalView(viewPath, masterPath);
			else
				return new WebFormView(controllerContext, viewPath, masterPath);
		}

		protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
		{
			if ((object)controllerContext == null)
				throw new ArgumentNullException("controllerContext");

			if ((object)virtualPath == null)
				throw new ArgumentNullException("virtualPath");

			return base.FileExists(controllerContext, virtualPath);
		}

		#endregion
	}
#endif
}