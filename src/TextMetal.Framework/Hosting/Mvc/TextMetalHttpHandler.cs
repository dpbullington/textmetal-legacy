/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Framework.Hosting.Mvc
{
#if USE_ASP_NET_LIBS
	public class TextMetalHttpHandler : IHttpHandler
	{
		#region Constructors/Destructors

		public TextMetalHttpHandler()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region Methods/Operators

		public void ProcessRequest(HttpContext context)
		{
			string originalPath;
			string viewFilePath;

			if ((object)context == null)
				throw new ArgumentNullException("context");

			originalPath = context.Request.Path.SafeToString();
			viewFilePath = context.Server.MapPath(originalPath);

			new WebHost().Host(context, viewFilePath, new object(), context.Response.Output);
		}

		#endregion
	}
#endif
}