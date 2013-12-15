// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

using Intelligencia.UrlRewriter.Configuration;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter
{
	/// <summary>
	/// Encapsulates all rewriting information about an individual rewrite request.  This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// This class cannot be created directly.  It will be provided to actions and conditions
	/// by the framework.
	/// </remarks>
	public sealed class RewriteContext
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="engine"> The rewriting engine. </param>
		/// <param name="rawUrl"> The initial, raw URL. </param>
		/// <param name="httpContext"> The HTTP context facade. </param>
		/// <param name="configurationManager"> The configuration manager facade. </param>
		internal RewriteContext(
			RewriterEngine engine,
			string rawUrl,
			IHttpContext httpContext,
			IConfigurationManager configurationManager)
		{
			if (engine == null)
				throw new ArgumentNullException("engine");
			if (httpContext == null)
				throw new ArgumentNullException("httpContext");
			if (configurationManager == null)
				throw new ArgumentNullException("configurationManager");

			this._engine = engine;
			this._configurationManager = configurationManager;
			this._location = rawUrl;
			this._method = httpContext.HttpMethod;
			this._mapPath = httpContext.MapPath;

			// Initialise the Properties collection from all the server variables, headers and cookies.
			foreach (string key in httpContext.ServerVariables.AllKeys)
				this._properties.Add(key, httpContext.ServerVariables[key]);
			foreach (string key in httpContext.RequestHeaders.AllKeys)
				this._properties.Add(key, httpContext.RequestHeaders[key]);
			foreach (string key in httpContext.RequestCookies.AllKeys)
				this._properties.Add(key, httpContext.RequestCookies[key].Value);
		}

		#endregion

		#region Fields/Constants

		private IConfigurationManager _configurationManager;
		private RewriterEngine _engine;
		private Match _lastMatch;
		private string _location;
		private MapPath _mapPath;
		private string _method;
		private NameValueCollection _properties = new NameValueCollection();
		private HttpCookieCollection _responseCookies = new HttpCookieCollection();
		private NameValueCollection _responseHeaders = new NameValueCollection();
		private HttpStatusCode _statusCode = HttpStatusCode.OK;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The configuration manager.
		/// </summary>
		public IConfigurationManager ConfigurationManager
		{
			get
			{
				return this._configurationManager;
			}
		}

		/// <summary>
		/// Last matching pattern from a match (if any).
		/// </summary>
		public Match LastMatch
		{
			get
			{
				return this._lastMatch;
			}
			set
			{
				this._lastMatch = value;
			}
		}

		/// <summary>
		/// The current location being rewritten.
		/// </summary>
		/// <remarks>
		/// This property starts out as Request.RawUrl and is altered by various rewrite actions.
		/// </remarks>
		public string Location
		{
			get
			{
				return this._location;
			}
			set
			{
				this._location = value;
			}
		}

		/// <summary>
		/// The request method (GET, PUT, POST, HEAD, DELETE).
		/// </summary>
		public string Method
		{
			get
			{
				return this._method;
			}
		}

		/// <summary>
		/// The properties for the context, including headers and cookie values.
		/// </summary>
		public NameValueCollection Properties
		{
			get
			{
				return this._properties;
			}
		}

		/// <summary>
		/// Collection of output cookies.
		/// </summary>
		/// <remarks>
		/// This is the collection of cookies to send in the response.  For the cookies
		/// received in the request, use the <see cref="RewriteContext.Properties"> Properties </see> property.
		/// </remarks>
		public HttpCookieCollection ResponseCookies
		{
			get
			{
				return this._responseCookies;
			}
		}

		/// <summary>
		/// Output response headers.
		/// </summary>
		/// <remarks>
		/// This collection is the collection of headers to add to the response.
		/// For the headers sent in the request, use the <see cref="RewriteContext.Properties"> Properties </see> property.
		/// </remarks>
		public NameValueCollection ResponseHeaders
		{
			get
			{
				return this._responseHeaders;
			}
		}

		/// <summary>
		/// The status code to send in the response.
		/// </summary>
		public HttpStatusCode StatusCode
		{
			get
			{
				return this._statusCode;
			}
			set
			{
				this._statusCode = value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Expands the given input using the last match, properties, maps and transforms.
		/// </summary>
		/// <param name="input"> The input to expand. </param>
		/// <returns> The expanded form of the input. </returns>
		public string Expand(string input)
		{
			return this._engine.Expand(this, input);
		}

		/// <summary>
		/// Maps the given URL to the absolute local path.
		/// </summary>
		/// <param name="url"> The URL to map. </param>
		/// <returns> The absolute local file path relating to the url. </returns>
		public string MapPath(string url)
		{
			return this._mapPath(url);
		}

		/// <summary>
		/// Resolves the location to an absolute reference.
		/// </summary>
		/// <param name="location"> The application-referenced location. </param>
		/// <returns> The absolute location. </returns>
		public string ResolveLocation(string location)
		{
			return this._engine.ResolveLocation(location);
		}

		#endregion
	}
}