/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Web;

using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Shared
{
	public static class GlobalAsax
	{
		#region Methods/Operators

		public static void OnApplicationError(this HttpApplication httpApplication, object sender, EventArgs e)
		{
			Exception ex;
			HttpException httpEx;
			string diagnosticContextInfo = "";

			ex = httpApplication.Server.GetLastError();

			if ((object)ex == null)
				return;

			httpEx = ex as HttpException;

			if ((object)httpEx != null)
			{
				if (httpEx.GetHttpCode() == 404)
					return; // exclude Page Not Founds...
			}

			if (ex is HttpUnhandledException)
				ex = ex.InnerException ?? ex;

			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Server.MachineName", httpApplication.Server.MachineName);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Server.ScriptTimeout", httpApplication.Server.ScriptTimeout);

			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.AcceptTypes", string.Join(", ", httpApplication.Request.AcceptTypes ?? new string[] { }));
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.AnonymousID", httpApplication.Request.AnonymousID);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.AppRelativeCurrentExecutionFilePath", httpApplication.Request.AppRelativeCurrentExecutionFilePath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ApplicationPath", httpApplication.Request.ApplicationPath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Browser", httpApplication.Request.Browser);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ClientCertificate", httpApplication.Request.ClientCertificate);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ContentEncoding", httpApplication.Request.ContentEncoding);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ContentLength", httpApplication.Request.ContentLength);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ContentType", httpApplication.Request.ContentType);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Cookies", httpApplication.Request.Cookies);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.CurrentExecutionFilePath", httpApplication.Request.CurrentExecutionFilePath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.CurrentExecutionFilePathExtension", httpApplication.Request.CurrentExecutionFilePathExtension);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.FilePath", httpApplication.Request.FilePath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Files", httpApplication.Request.Files);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Filter", httpApplication.Request.Filter);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Form", httpApplication.Request.Form);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Headers", httpApplication.Request.Headers);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.HttpMethod", httpApplication.Request.HttpMethod);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.IsAuthenticated", httpApplication.Request.IsAuthenticated);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.IsLocal", httpApplication.Request.IsLocal);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.IsSecureConnection", httpApplication.Request.IsSecureConnection);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.LogonUserIdentity", httpApplication.Request.LogonUserIdentity);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Path", httpApplication.Request.Path);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.PathInfo", httpApplication.Request.PathInfo);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.PhysicalApplicationPath", httpApplication.Request.PhysicalApplicationPath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.PhysicalPath", httpApplication.Request.PhysicalPath);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.QueryString", httpApplication.Request.QueryString);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.RawUrl", httpApplication.Request.RawUrl);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ReadEntityBodyMode", httpApplication.Request.ReadEntityBodyMode);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.RequestType", httpApplication.Request.RequestType);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.ServerVariables", httpApplication.Request.ServerVariables);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.TimedOutToken", httpApplication.Request.TimedOutToken);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.TotalBytes", httpApplication.Request.TotalBytes);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Unvalidated", httpApplication.Request.Unvalidated);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.Url", httpApplication.Request.Url);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.UrlReferrer", httpApplication.Request.UrlReferrer);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.UserAgent", httpApplication.Request.UserAgent);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.UserHostAddress", httpApplication.Request.UserHostAddress);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.UserHostName", httpApplication.Request.UserHostName);
			diagnosticContextInfo += string.Format("{0}={1};\r\n", "Request.UserLanguages", httpApplication.Request.UserLanguages);

			Stuff.Get<IRepository>("").TryWriteEventLogEntry(Reflexion.GetErrors(ex, 0));
			Stuff.Get<IRepository>("").TrySendEmailTemplate(EmailTemplateResourceNames.EVENT_LOG, new
																								{
																									Error = Reflexion.GetErrors(ex, 0),
																									Context = diagnosticContextInfo
																								});
		}

		#endregion
	}
}