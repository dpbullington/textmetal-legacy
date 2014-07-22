/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;

using TextMetal.Common.Core;
using TextMetal.Common.Core.StringTokens;
using TextMetal.Common.Xml;
using TextMetal.Framework.Core;
using TextMetal.Framework.HostingModel;
using TextMetal.Framework.InputOutputModel;
using TextMetal.Framework.SourceModel.Primative;
using TextMetal.Framework.TemplateModel;

namespace TextMetal.HostImpl.Web.AspNet
{
	public sealed class WebHost : IWebHost
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the WebHost class.
		/// </summary>
		public WebHost()
		{
		}

		#endregion

		#region Methods/Operators

		public void Host(HttpContext httpContext, string templateFilePath, object source, TextWriter textWriter)
		{
			DateTime startUtc = DateTime.UtcNow, endUtc;
			IDictionary<string, IList<string>> properties;
			IXmlPersistEngine xpe;
			TemplateConstruct template;
			Dictionary<string, object> globalVariableTable;
			string toolVersion;
			string templateDirectoryPath;
			ISourceStrategy sourceStrategy;

			if ((object)templateFilePath == null)
				throw new ArgumentNullException("templateFilePath");

			if ((object)textWriter == null)
				throw new ArgumentNullException("textWriter");

			if (DataType.IsWhiteSpace(templateFilePath))
				throw new ArgumentOutOfRangeException("templateFilePath");

			properties = new Dictionary<string, IList<string>>();

			toolVersion = Assembly.GetAssembly(typeof(IXmlPersistEngine)).GetName().Version.ToString();
			templateFilePath = Path.GetFullPath(templateFilePath);
			templateDirectoryPath = Path.GetDirectoryName(templateFilePath);

			sourceStrategy = new NullSourceStrategy();

			xpe = new XmlPersistEngine();
			xpe.RegisterWellKnownConstructs();

			using (IInputMechanism inputMechanism = new FileInputMechanism(templateDirectoryPath, xpe, sourceStrategy)) // relative to template
			{
				template = (TemplateConstruct)inputMechanism.LoadTemplate(templateFilePath);

				using (IOutputMechanism outputMechanism = new TextWriterOutputMechanism(textWriter))
				{
					//outputMechanism.WriteObject(template, "#template.xml");
					//outputMechanism.WriteObject(source, "#source.xml");

					outputMechanism.LogTextWriter.WriteLine("['{0:O}' (UTC)]\tText templating started.", startUtc);

					using (ITemplatingContext templatingContext = new TemplatingContext(xpe, new Tokenizer(true), inputMechanism, outputMechanism, properties))
					{
						templatingContext.Tokenizer.RegisterWellKnownTokenReplacementStrategies(templatingContext);

						// globals
						globalVariableTable = new Dictionary<string, object>();

						var environment = new
										{
											ARGC = 0,
											ARGV = new string[] { },
											ARGS = new Dictionary<string, object>(),
											CommandLine = Environment.CommandLine,
											CurrentDirectory = Environment.CurrentDirectory,
											CurrentManagedThreadId = Environment.CurrentManagedThreadId,
											ExitCode = Environment.ExitCode,
											Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
											Is64BitProcess = Environment.Is64BitProcess,
											MachineName = Environment.MachineName,
											NewLine = Environment.NewLine,
											OSVersion = Environment.OSVersion,
											ProcessorCount = Environment.ProcessorCount,
											SystemDirectory = Environment.SystemDirectory,
											SystemPageSize = Environment.SystemPageSize,
											TickCount = Environment.TickCount,
											UserDomainName = Environment.UserDomainName,
											UserInteractive = Environment.UserInteractive,
											UserName = Environment.UserName,
											Version = Environment.Version,
											WorkingSet = Environment.WorkingSet,
											Variables = Environment.GetEnvironmentVariables()
										};
						globalVariableTable.Add("ToolVersion", toolVersion);
						globalVariableTable.Add("Environment", environment);

						// add HTTP request properties to GVT
						foreach (string key in httpContext.Request.Params.AllKeys)
						{
							string value = httpContext.Request.Params[key];
							globalVariableTable.Add(key, value);
						}

						templatingContext.VariableTables.Push(globalVariableTable);
						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();
						templatingContext.VariableTables.Pop();
					}

					endUtc = DateTime.UtcNow;
					outputMechanism.LogTextWriter.WriteLine("['{0:O}' (UTC)]\tText templating completed with duration: '{1}'.", endUtc, (endUtc - startUtc));
				}
			}
		}

		#endregion
	}
}