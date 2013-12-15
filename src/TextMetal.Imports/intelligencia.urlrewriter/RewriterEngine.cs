// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

using Intelligencia.UrlRewriter.Configuration;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter
{
	/// <summary>
	/// The core RewriterEngine class.
	/// </summary>
	public class RewriterEngine
	{
		#region Constructors/Destructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="httpContext"> The HTTP context facade. </param>
		/// <param param name="configurationManager"> The configuration manager facade. </param>
		/// <param name="configuration"> The URL rewriter configuration. </param>
		public RewriterEngine(
			IHttpContext httpContext,
			IConfigurationManager configurationManager,
			IRewriterConfiguration configuration)
		{
			if (httpContext == null)
				throw new ArgumentNullException("httpContext");
			if (configurationManager == null)
				throw new ArgumentNullException("configurationManager");
			if (configuration == null)
				throw new ArgumentNullException("configuration");
			this._httpContext = httpContext;
			this._configurationManager = configurationManager;
			this._configuration = configuration;
		}

		#endregion

		#region Fields/Constants

		private const string ContextOriginalQueryString = "UrlRewriter.NET.OriginalQueryString";
		private const string ContextQueryString = "UrlRewriter.NET.QueryString";
		private const string ContextRawUrl = "UrlRewriter.NET.RawUrl";
		private const char EndChar = (char)65535;
		private IRewriterConfiguration _configuration;
		private IConfigurationManager _configurationManager;
		private IHttpContext _httpContext;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The original query string.
		/// </summary>
		public string OriginalQueryString
		{
			get
			{
				return (string)this._httpContext.GetItem(ContextOriginalQueryString);
			}
			set
			{
				this._httpContext.SetItem(ContextOriginalQueryString, value);
			}
		}

		/// <summary>
		/// The final querystring, after rewriting.
		/// </summary>
		public string QueryString
		{
			get
			{
				return (string)this._httpContext.GetItem(ContextQueryString);
			}
			set
			{
				this._httpContext.SetItem(ContextQueryString, value);
			}
		}

		/// <summary>
		/// The raw url.
		/// </summary>
		public string RawUrl
		{
			get
			{
				return (string)this._httpContext.GetItem(ContextRawUrl);
			}
			set
			{
				this._httpContext.SetItem(ContextRawUrl, value);
			}
		}

		#endregion

		#region Methods/Operators

		private void AppendCookies(RewriteContext context)
		{
			for (int i = 0; i < context.ResponseCookies.Count; i++)
				this._httpContext.SetResponseCookie(context.ResponseCookies[i]);
		}

		private void AppendHeaders(RewriteContext context)
		{
			foreach (string headerKey in context.ResponseHeaders)
				this._httpContext.SetResponseHeader(headerKey, context.ResponseHeaders[headerKey]);
		}

		/// <summary>
		/// Expands the given input based on the current context.
		/// </summary>
		/// <param name="context"> The current context </param>
		/// <param name="input"> The input to expand. </param>
		/// <returns> The expanded input </returns>
		public string Expand(RewriteContext context, string input)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (input == null)
				throw new ArgumentNullException("input");

			/* replacement :- $n
                         * |	${[a-zA-Z0-9\-]+}
                         * |	${fn( <replacement> )}
                         * |	${<replacement-or-id>:<replacement-or-value>:<replacement-or-value>}
                         * 
                         * replacement-or-id :- <replacement> | <id>
                         * replacement-or-value :- <replacement> | <value>
                         */

			/* $1 - regex replacement
             * ${propertyname}
             * ${map-name:value}				map-name is replacement, value is replacement
             * ${map-name:value|default-value}	map-name is replacement, value is replacement, default-value is replacement
             * ${fn(value)}						value is replacement
             */

			using (StringReader reader = new StringReader(input))
			{
				using (StringWriter writer = new StringWriter())
				{
					char ch = (char)reader.Read();
					while (ch != EndChar)
					{
						if (ch == '$')
							writer.Write(this.Reduce(context, reader));
						else
							writer.Write(ch);

						ch = (char)reader.Read();
					}

					return writer.GetStringBuilder().ToString();
				}
			}
		}

		private bool HandleDefaultDocument(RewriteContext context)
		{
			Uri uri = new Uri(this._httpContext.RequestUrl, context.Location);
			UriBuilder b = new UriBuilder(uri);
			b.Path += "/";
			uri = b.Uri;
			if (uri.Host == this._httpContext.RequestUrl.Host)
			{
				string filename = this._httpContext.MapPath(uri.AbsolutePath);
				if (Directory.Exists(filename))
				{
					foreach (string document in this._configuration.DefaultDocuments)
					{
						string pathName = Path.Combine(filename, document);
						if (File.Exists(pathName))
						{
							context.Location = new Uri(uri, document).AbsolutePath;
							return true;
						}
					}
				}
			}

			return false;
		}

		private void HandleError(RewriteContext context)
		{
			// Return the status code.
			this._httpContext.SetStatusCode((int)context.StatusCode);

			// Get the error handler if there is one.
			if (this._configuration.ErrorHandlers.ContainsKey((int)context.StatusCode))
			{
				IRewriteErrorHandler handler = this._configuration.ErrorHandlers[(int)context.StatusCode];

				try
				{
					this._configuration.Logger.Debug(MessageProvider.FormatString(Message.CallingErrorHandler));

					// Execute the error handler.
					this._httpContext.HandleError(handler);
				}
				catch (HttpException)
				{
					throw;
				}
				catch (Exception exc)
				{
					this._configuration.Logger.Fatal(exc.Message, exc);
					throw new HttpException((int)HttpStatusCode.InternalServerError, HttpStatusCode.InternalServerError.ToString());
				}
			}
			else
				throw new HttpException((int)context.StatusCode, context.StatusCode.ToString());
		}

		private void ProcessRules(RewriteContext context)
		{
			const int MaxRestart = 10; // Controls the number of restarts so we don't get into an infinite loop

			IList<IRewriteAction> rewriteRules = this._configuration.Rules;
			int restarts = 0;
			for (int i = 0; i < rewriteRules.Count; i++)
			{
				// If the rule is conditional, ensure the conditions are met.
				IRewriteCondition condition = rewriteRules[i] as IRewriteCondition;
				if (condition == null || condition.IsMatch(context))
				{
					// Execute the action.
					IRewriteAction action = rewriteRules[i];
					RewriteProcessing processing = action.Execute(context);

					// If the action is Stop, then break out of the processing loop
					if (processing == RewriteProcessing.StopProcessing)
					{
						this._configuration.Logger.Debug(MessageProvider.FormatString(Message.StoppingBecauseOfRule));
						break;
					}
					else if (processing == RewriteProcessing.RestartProcessing)
					{
						this._configuration.Logger.Debug(MessageProvider.FormatString(Message.RestartingBecauseOfRule));

						// Restart from the first rule.
						i = 0;

						if (++restarts > MaxRestart)
							throw new InvalidOperationException(MessageProvider.FormatString(Message.TooManyRestarts));
					}
				}
			}
		}

		private string Reduce(RewriteContext context, StringReader reader)
		{
			string result;
			char ch = (char)reader.Read();
			if (Char.IsDigit(ch))
			{
				string num = ch.ToString();
				if (Char.IsDigit((char)reader.Peek()))
				{
					ch = (char)reader.Read();
					num += ch.ToString();
				}
				if (context.LastMatch != null)
				{
					Group group = context.LastMatch.Groups[Convert.ToInt32(num)];
					result = (group == null) ? String.Empty : group.Value;
				}
				else
					result = String.Empty;
			}
			else if (ch == '<')
			{
				string expr;

				using (StringWriter writer = new StringWriter())
				{
					ch = (char)reader.Read();
					while (ch != '>' && ch != EndChar)
					{
						if (ch == '$')
							writer.Write(this.Reduce(context, reader));
						else
							writer.Write(ch);
						ch = (char)reader.Read();
					}

					expr = writer.GetStringBuilder().ToString();
				}

				if (context.LastMatch != null)
				{
					Group group = context.LastMatch.Groups[expr];
					result = (group == null) ? String.Empty : group.Value;
				}
				else
					result = String.Empty;
			}
			else if (ch == '{')
			{
				string expr;
				bool isMap = false;
				bool isFunction = false;

				using (StringWriter writer = new StringWriter())
				{
					ch = (char)reader.Read();
					while (ch != '}' && ch != EndChar)
					{
						if (ch == '$')
							writer.Write(this.Reduce(context, reader));
						else
						{
							if (ch == ':')
								isMap = true;
							else if (ch == '(')
								isFunction = true;
							writer.Write(ch);
						}
						ch = (char)reader.Read();
					}

					expr = writer.GetStringBuilder().ToString();
				}

				if (isMap)
				{
					Match match = Regex.Match(expr, @"^([^\:]+)\:([^\|]+)(\|(.+))?$");
					string mapName = match.Groups[1].Value;
					string mapArgument = match.Groups[2].Value;
					string mapDefault = match.Groups[4].Value;
					result = this._configuration.TransformFactory.GetTransform(mapName).ApplyTransform(mapArgument);
					if (result == null)
						result = mapDefault;
				}
				else if (isFunction)
				{
					Match match = Regex.Match(expr, @"^([^\(]+)\((.+)\)$");
					string functionName = match.Groups[1].Value;
					string functionArgument = match.Groups[2].Value;
					IRewriteTransform tx = this._configuration.TransformFactory.GetTransform(functionName);
					result = (tx == null) ? expr : tx.ApplyTransform(functionArgument);
				}
				else
					result = context.Properties[expr];
			}
			else
				result = ch.ToString();

			return result;
		}

		/// <summary>
		/// Resolves an Application-path relative location
		/// </summary>
		/// <param name="location"> The location </param>
		/// <returns> The absolute location. </returns>
		public string ResolveLocation(string location)
		{
			if (location == null)
				throw new ArgumentNullException("location");

			string appPath = this._httpContext.ApplicationPath;
			if (appPath.Length > 1)
				appPath += "/";

			return location.Replace("~/", appPath);
		}

		/// <summary>
		/// Performs the rewriting.
		/// </summary>
		public void Rewrite()
		{
			string originalUrl = this._httpContext.RawUrl.Replace("+", " ");
			this.RawUrl = originalUrl;

			this._configuration.Logger.Debug(MessageProvider.FormatString(Message.StartedProcessing, originalUrl));

			// Create the context
			RewriteContext context = new RewriteContext(this, originalUrl, this._httpContext, this._configurationManager);

			// Process each rule.
			this.ProcessRules(context);

			// Append any headers defined.
			this.AppendHeaders(context);

			// Append any cookies defined.
			this.AppendCookies(context);

			// Rewrite the path if the location has changed.
			this._httpContext.SetStatusCode((int)context.StatusCode);
			if ((context.Location != originalUrl) && ((int)context.StatusCode < 400))
			{
				if ((int)context.StatusCode < 300)
				{
					// Successful status if less than 300
					this._configuration.Logger.Info(MessageProvider.FormatString(Message.RewritingXtoY, this._httpContext.RawUrl, context.Location));

					// To verify that the url exists on this server:
					//  VerifyResultExists(context);

					// To ensure that directories are rewritten to their default document:
					//  HandleDefaultDocument(context);

					this._httpContext.RewritePath(context.Location);
				}
				else
				{
					// Redirection
					this._configuration.Logger.Info(MessageProvider.FormatString(Message.RedirectingXtoY, this._httpContext.RawUrl, context.Location));

					this._httpContext.SetRedirectLocation(context.Location);
				}
			}
			else if ((int)context.StatusCode >= 400)
				this.HandleError(context);
			// To ensure that directories are rewritten to their default document:
			//  else if (HandleDefaultDocument(context))
			//  {
			//      _contextFacade.RewritePath(context.Location);
			//  }

			// Sets the context items.
			this.SetContextItems(context);
		}

		private void SetContextItems(RewriteContext context)
		{
			this.OriginalQueryString = new Uri(this._httpContext.RequestUrl, this._httpContext.RawUrl).Query.Replace("?", "");
			this.QueryString = new Uri(this._httpContext.RequestUrl, context.Location).Query.Replace("?", "");

			// Add in the properties as context items, so these will be accessible to the handler
			foreach (string key in context.Properties.Keys)
				this._httpContext.SetItem(String.Format("Rewriter.{0}", key), context.Properties[key]);
		}

		private void VerifyResultExists(RewriteContext context)
		{
			if ((String.Compare(context.Location, this._httpContext.RawUrl) != 0) &&
				((int)context.StatusCode < 300))
			{
				Uri uri = new Uri(this._httpContext.RequestUrl, context.Location);
				if (uri.Host == this._httpContext.RequestUrl.Host)
				{
					string filename = this._httpContext.MapPath(uri.AbsolutePath);
					if (!File.Exists(filename))
					{
						this._configuration.Logger.Debug(MessageProvider.FormatString(Message.ResultNotFound, filename));
						context.StatusCode = HttpStatusCode.NotFound;
					}
					else
						this.HandleDefaultDocument(context);
				}
			}
		}

		#endregion
	}
}