﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using TextMetal.Framework.Core;
using TextMetal.Framework.InputOutput;
using TextMetal.Framework.Source;
using TextMetal.Framework.Template;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.Hosting.Tool
{
	/// <summary>
	/// This class contains code to bootstrap TextMetal proper. This code is a specific implementation for TextMetal 'tool' hosting, concerned with leveraging file paths. Other host implementations will vary (see web host sample for instance). This code can be used by any interactive or batch application (console, windows, WPF, service, etc.).
	/// </summary>
	public sealed class TextMetalToolHost : ITextMetalToolHost
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TextMetalToolHost class.
		/// </summary>
		[DependencyInjection]
		public TextMetalToolHost([DependencyInjection] IReflectionFascade reflectionFascade)
		{
			if ((object)reflectionFascade == null)
				throw new ArgumentNullException(nameof(reflectionFascade));

			this.reflectionFascade = reflectionFascade;
		}

		#endregion

		#region Fields/Constants

		private readonly IReflectionFascade reflectionFascade;

		#endregion

		#region Properties/Indexers/Events

		private IReflectionFascade ReflectionFascade
		{
			get
			{
				return this.reflectionFascade;
			}
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
				throw new ArgumentNullException(nameof(templateFilePath));

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException(nameof(sourceFilePath));

			if ((object)baseDirectoryPath == null)
				throw new ArgumentNullException(nameof(baseDirectoryPath));

			if ((object)sourceStrategyAqtn == null)
				throw new ArgumentNullException(nameof(sourceStrategyAqtn));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(templateFilePath))
				throw new ArgumentOutOfRangeException(nameof(templateFilePath));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException(nameof(sourceFilePath));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(baseDirectoryPath))
				throw new ArgumentOutOfRangeException(nameof(baseDirectoryPath));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(sourceStrategyAqtn))
				throw new ArgumentOutOfRangeException(nameof(sourceStrategyAqtn));

			toolVersion = new AssemblyInformationFascade(this.ReflectionFascade, typeof(TextMetalToolHost).GetTypeInfo().Assembly).AssemblyVersion;
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

			// TODO: this was a bad idea and need to be fixed in a next version.
			using (IInputMechanism inputMechanism = new FileInputMechanism(templateFilePath, xpe, sourceStrategy)) // relative to template directory
			{
				source = inputMechanism.LoadSource(sourceFilePath, properties);

				if ((object)source == null)
					return;

				template = (TemplateConstruct)inputMechanism.LoadTemplate(templateFilePath);

				using (IOutputMechanism outputMechanism = new FileOutputMechanism(baseDirectoryPath, "#textmetal.log", Encoding.UTF8, xpe)) // relative to base directory
				{
					outputMechanism.LogTextWriter.WriteLine("[DIAGNOSTIC INFOMRATION]", startUtc);

					outputMechanism.LogTextWriter.WriteLine("argv: '{0}'", string.Join(" ", argv));
					outputMechanism.LogTextWriter.WriteLine("toolVersion: '{0}'", toolVersion);
					outputMechanism.LogTextWriter.WriteLine("baseDirectoryPath: \"{0}\"", baseDirectoryPath);
					outputMechanism.LogTextWriter.WriteLine("sourceFilePath: \"{0}\"", sourceFilePath);
					outputMechanism.LogTextWriter.WriteLine("templateFilePath: \"{0}\"", templateFilePath);
					outputMechanism.LogTextWriter.WriteLine("sourceStrategyType: '{0}'", sourceStrategyType.FullName);

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
											CurrentManagedThreadId = Environment.CurrentManagedThreadId,
											HasShutdownStarted = Environment.HasShutdownStarted,
											NewLine = Environment.NewLine,
											ProcessorCount = Environment.ProcessorCount,
											StackTrace = Environment.StackTrace,
											TickCount = Environment.TickCount,
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