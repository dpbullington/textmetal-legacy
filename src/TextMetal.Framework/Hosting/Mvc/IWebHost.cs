/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Framework.Hosting.Mvc
{
#if USE_ASP_NET_LIBS
	public interface IWebHost
	{
		#region Methods/Operators

		void Host(HttpContext httpContext, string templateFilePath, object source, TextWriter textWriter);

		#endregion
	}
#endif
}