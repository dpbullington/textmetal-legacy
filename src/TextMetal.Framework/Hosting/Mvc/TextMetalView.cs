/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Framework.Hosting.Mvc
{
#if USE_ASP_NET_LIBS
	public class TextMetalView : IView
	{
		#region Constructors/Destructors

		public TextMetalView(string viewName)
		{
			if ((object)viewName == null)
				throw new ArgumentNullException("viewName");

			if (DataTypeFascade.Instance.IsWhiteSpace(viewName))
				throw new ArgumentOutOfRangeException("viewName");

			this.viewName = viewName;
		}

		public TextMetalView(string viewName, string masterName)
		{
			if ((object)viewName == null)
				throw new ArgumentNullException("viewName");

			if ((object)masterName == null)
				throw new ArgumentNullException("masterName");

			if (DataTypeFascade.Instance.IsWhiteSpace(viewName))
				throw new ArgumentOutOfRangeException("viewName");

			if (DataTypeFascade.Instance.IsWhiteSpace(masterName))
				throw new ArgumentOutOfRangeException("masterName");

			this.viewName = viewName;
			this.masterName = masterName;
		}

		#endregion

		#region Fields/Constants

		private readonly string masterName;
		private readonly string viewName;

		#endregion

		#region Properties/Indexers/Events

		private string MasterName
		{
			get
			{
				return this.masterName;
			}
		}

		private string ViewName
		{
			get
			{
				return this.viewName;
			}
		}

		#endregion

		#region Methods/Operators

		public void Render(ViewContext viewContext, TextWriter writer)
		{
			string viewFilePath;
			string masterPageFilePath;

			if ((object)viewContext == null)
				throw new ArgumentNullException("viewContext");

			if ((object)writer == null)
				throw new ArgumentNullException("writer");

			viewFilePath = viewContext.HttpContext.Server.MapPath(this.ViewName.SafeToString());

			if (!string.IsNullOrEmpty(this.MasterName))
				masterPageFilePath = viewContext.HttpContext.Server.MapPath(this.MasterName);

			new WebHost().Host(viewContext.HttpContext.ApplicationInstance.Context, viewFilePath, viewContext.ViewData.Model, writer);
		}

		#endregion
	}
#endif
}