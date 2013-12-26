using System.IO;
using System.Web;

namespace TextMetal.HostImpl.Web.AspNet
{
	public interface IWebHost
	{
		void Host(HttpContext httpContext, string templateFilePath, object source, TextWriter textWriter);
	}
}