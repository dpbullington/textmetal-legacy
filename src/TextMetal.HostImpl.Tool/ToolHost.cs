/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using TextMetal.Common.Core;
using TextMetal.Common.Core.Tokenization;
using TextMetal.Common.Xml;
using TextMetal.Framework.Core;
using TextMetal.Framework.HostingModel;
using TextMetal.Framework.InputOutputModel;
using TextMetal.Framework.TemplateModel;

namespace TextMetal.HostImpl.Tool
{
	/// <summary>
	/// This class contains code to bootstrap TextMetal proper. This code is a specific implementation for TextMetal 'tool' hosting, concerned with leveraging file paths. Other host implementations will vary (see web host sample for instance). This code can be used by any interactive or batch application (console, windows, WPF, service, etc.).
	/// </summary>
	public sealed class ToolHost : IToolHost
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ToolHost class.
		/// </summary>
		public ToolHost()
		{
		}

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
		}

		/// <summary>
		/// Provides a hosting shim between a 'tool' host and the underlying TextMetal run-time.
		/// </summary>
		/// <param name="argc"> The raw argument count passed into the host. </param>
		/// <param name="argv"> The raw arguments passed into the host. </param>
		/// <param name="args"> The parsed arguments passed into the host. </param>
		/// <param name="templateFilePath"> The file path of the input TextMetal template file to execute. </param>
		/// <param name="sourceFilePath"> The file path (or source specific URI) of the input data source to leverage. </param>
		/// <param name="baseDirectoryPath"> The root output directory path to place output arifacts (since this implementation uses file output mechanics). </param>
		/// <param name="sourceStrategyAqtn"> The assembly qualified type name for the ISourceStrategy to instantiate and execute. </param>
		/// <param name="strictMatching"> A value indicating whether to use strict matching semantics for tokens. </param>
		/// <param name="properties"> Arbitrary dictionary of string lists used to further customize the text templating process. The individual components or template files can use the properties as they see fit. </param>
		public void Host(int argc, string[] argv, IDictionary<string, object> args, string templateFilePath, string sourceFilePath, string baseDirectoryPath, string sourceStrategyAqtn, bool strictMatching, IDictionary<string, IList<string>> properties)
		{
			DateTime startUtc = DateTime.UtcNow, endUtc;
			IXmlPersistEngine xpe;
			TemplateConstruct template;
			object source;
			Dictionary<string, object> globalVariableTable;
			string toolVersion;
			Type sourceStrategyType;
			ISourceStrategy sourceStrategy;

			if ((object)templateFilePath == null)
				throw new ArgumentNullException("templateFilePath");

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException("sourceFilePath");

			if ((object)baseDirectoryPath == null)
				throw new ArgumentNullException("baseDirectoryPath");

			if ((object)sourceStrategyAqtn == null)
				throw new ArgumentNullException("sourceStrategyAqtn");

			if ((object)properties == null)
				throw new ArgumentNullException("properties");

			if (DataType.Instance.IsWhiteSpace(templateFilePath))
				throw new ArgumentOutOfRangeException("templateFilePath");

			if (DataType.Instance.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException("sourceFilePath");

			if (DataType.Instance.IsWhiteSpace(baseDirectoryPath))
				throw new ArgumentOutOfRangeException("baseDirectoryPath");

			if (DataType.Instance.IsWhiteSpace(sourceStrategyAqtn))
				throw new ArgumentOutOfRangeException("sourceStrategyAqtn");

			toolVersion = new AssemblyInformation(Assembly.GetAssembly(typeof(IXmlPersistEngine))).AssemblyVersion;
			templateFilePath = Path.GetFullPath(templateFilePath);
			baseDirectoryPath = Path.GetFullPath(baseDirectoryPath);

			sourceStrategyType = Type.GetType(sourceStrategyAqtn, false);

			if ((object)sourceStrategyType == null)
				throw new InvalidOperationException(string.Format("Failed to load source strategy from assembly qualified type name '{0}'.", sourceStrategyAqtn));

			if (!typeof(ISourceStrategy).IsAssignableFrom(sourceStrategyType))
				throw new InvalidOperationException(string.Format("Source strategy type '{0}' is not assignable to '{1}'.", sourceStrategyType, typeof(ISourceStrategy)));

			sourceStrategy = (ISourceStrategy)Activator.CreateInstance(sourceStrategyType);

			xpe = new XmlPersistEngine();
			xpe.RegisterWellKnownConstructs();

			using (IInputMechanism inputMechanism = new FileInputMechanism(templateFilePath, xpe, sourceStrategy)) // relative to template directory
			{
				source = inputMechanism.LoadSource(sourceFilePath, properties);

				if ((object)source == null)
					return;

				template = (TemplateConstruct)inputMechanism.LoadTemplate(templateFilePath);

				using (IOutputMechanism outputMechanism = new FileOutputMechanism(baseDirectoryPath, "#textmetal.log", Encoding.UTF8, xpe)) // relative to base directory
				{
					outputMechanism.WriteObject(template, "#template.xml");
					outputMechanism.WriteObject(source, "#source.xml");

					outputMechanism.LogTextWriter.WriteLine("['{0:O}' (UTC)]\tText templating started.", startUtc);

					using (ITemplatingContext templatingContext = new TemplatingContext(xpe, new Tokenizer(strictMatching), inputMechanism, outputMechanism, properties))
					{
						templatingContext.Tokenizer.RegisterWellKnownTokenReplacementStrategies(templatingContext);

						// globals
						globalVariableTable = new Dictionary<string, object>();

						var environment = new
										{
											ARGC = argc,
											ARGV = argv,
											ARGS = args,
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
						globalVariableTable.Add("StartUtc", startUtc);

						// add properties to GVT
						foreach (KeyValuePair<string, IList<string>> property in properties)
						{
							if (property.Value.Count == 0)
								continue;

							if (property.Value.Count == 1)
								globalVariableTable.Add(property.Key, property.Value[0]);
							else
								globalVariableTable.Add(property.Key, property.Value);
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