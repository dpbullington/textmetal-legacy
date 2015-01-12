/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

using TextMetal.Common.Core;
using TextMetal.Common.Data.Framework;
using TextMetal.Common.Solder.DependencyManagement;
using TextMetal.HostImpl.Web.AspNet;

namespace TextMetal.Common.WebApps.Core
{
	public abstract class BaseHttpApplication : HttpApplication
	{
		#region Constructors/Destructors

		protected BaseHttpApplication(bool requireHttps)
		{
			this.requireHttps = requireHttps;
			this.assemblyInformation = new AssemblyInformation(Assembly.GetAssembly(this.GetType()));
		}

		#endregion

		#region Fields/Constants

		private readonly AssemblyInformation assemblyInformation;
		private readonly bool requireHttps;

		#endregion

		#region Properties/Indexers/Events

		protected AssemblyInformation AssemblyInformation
		{
			get
			{
				return this.assemblyInformation;
			}
		}

		public bool RequireHttps
		{
			get
			{
				return this.requireHttps;
			}
		}

		protected abstract string SendEventLogEmailTemplateName
		{
			get;
		}

		#endregion

		#region Methods/Operators

		protected virtual void Application_Error(object sender, EventArgs e)
		{
			this.OnApplicationError(sender, e);
		}

		protected virtual void Application_Start()
		{
			TextMetalViewEngine.CallMeInGlobalAsax();

			if (this.RequireHttps)
				GlobalFilters.Filters.Add(new RequireHttpsAttribute());
		}

		private void OnApplicationError(object sender, EventArgs e)
		{
			HandleWebApplicationError(this.Server, this.Request, this.SendEventLogEmailTemplateName);
		}

		public static void HandleWebApplicationError(HttpServerUtility server, HttpRequest request, string sendEventLogEmailTemplateName)
		{
			Exception ex;
			HttpException httpEx;
			string diagnosticContextInfo = "";

			if ((object)server == null)
				throw new ArgumentNullException("server");

			if ((object)request == null)
				throw new ArgumentNullException("request");

			if ((object)sendEventLogEmailTemplateName == null)
				throw new ArgumentNullException("sendEventLogEmailTemplateName");

			ex = server.GetLastError();

			if ((object)ex == null)
				return;

			httpEx = ex as HttpException;

			if ((object)httpEx != null)
			{
				if (httpEx.GetHttpCode() == 404)
					return; // exclude Page Not Founds...

				if (httpEx is HttpUnhandledException)
					ex = httpEx.InnerException ?? ex;
			}

			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Server.MachineName", server.MachineName);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Server.ScriptTimeout", server.ScriptTimeout);

			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.AcceptTypes", string.Join(", ", request.AcceptTypes ?? new string[] { }));
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.AnonymousID", request.AnonymousID);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.AppRelativeCurrentExecutionFilePath", request.AppRelativeCurrentExecutionFilePath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ApplicationPath", request.ApplicationPath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Browser", request.Browser);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ClientCertificate", request.ClientCertificate);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ContentEncoding", request.ContentEncoding);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ContentLength", request.ContentLength);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ContentType", request.ContentType);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Cookies", request.Cookies);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.CurrentExecutionFilePath", request.CurrentExecutionFilePath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.CurrentExecutionFilePathExtension", request.CurrentExecutionFilePathExtension);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.FilePath", request.FilePath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Files", request.Files);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Filter", request.Filter);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Form", request.Form);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Headers", request.Headers);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.HttpMethod", request.HttpMethod);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.IsAuthenticated", request.IsAuthenticated);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.IsLocal", request.IsLocal);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.IsSecureConnection", request.IsSecureConnection);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.LogonUserIdentity", request.LogonUserIdentity);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Path", request.Path);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.PathInfo", request.PathInfo);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.PhysicalApplicationPath", request.PhysicalApplicationPath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.PhysicalPath", request.PhysicalPath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.QueryString", request.QueryString);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.RawUrl", request.RawUrl);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ReadEntityBodyMode", request.ReadEntityBodyMode);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.RequestType", request.RequestType);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ServerVariables", request.ServerVariables);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.TimedOutToken", request.TimedOutToken);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.TotalBytes", request.TotalBytes);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Unvalidated", request.Unvalidated);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Url", request.Url);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.UrlReferrer", request.UrlReferrer);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.UserAgent", request.UserAgent);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.UserHostAddress", request.UserHostAddress);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.UserHostName", request.UserHostName);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.UserLanguages", request.UserLanguages);

			DependencyManager.AppDomainInstance.ResolveDependency<IModelRepository>(string.Empty).TryWriteEventLogEntry(Reflexion.Instance.GetErrors(ex, 0));
			DependencyManager.AppDomainInstance.ResolveDependency<IModelRepository>(string.Empty).TrySendEmailTemplate(sendEventLogEmailTemplateName, new
																																					{
																																						Error = Reflexion.Instance.GetErrors(ex, 0),
																																						Context = diagnosticContextInfo
																																					});
		}

		#endregion
	}
}