using System.IO;
using System.Web;

namespace TextMetal.HostImpl.Web.AspNet
{
	public interface IWebHost
	{
		#region Methods/Operators

		void Host(HttpContext httpContext, string templateFilePath, object source, TextWriter textWriter);

		#endregion
	}
}