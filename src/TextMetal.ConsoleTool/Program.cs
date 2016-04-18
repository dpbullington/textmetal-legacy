/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using TextMetal.Framework.Hosting.Tool;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Runtime;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.ConsoleTool
{
	/// <summary>
	/// Entry point class for the application.
	/// </summary>
	internal class Program : ConsoleApplicationFascade
	{
		#region Constructors/Destructors

		[DependencyInjection]
		public Program([DependencyInjection] IDataTypeFascade dataTypeFascade, [DependencyInjection] IAppConfigFascade appConfigFascade, [DependencyInjection] IReflectionFascade reflectionFascade)
			: base(dataTypeFascade, appConfigFascade, reflectionFascade)
		{
		}

		#endregion

		#region Fields/Constants

		private const string CMDLN_DEBUGGER_LAUNCH = "debug";
		private const string CMDLN_TOKEN_BASEDIR = "basedir";
		private const string CMDLN_TOKEN_PROPERTY = "property";
		private const string CMDLN_TOKEN_SOURCEFILE = "sourcefile";
		private const string CMDLN_TOKEN_SOURCESTRATEGY_AQTN = "sourcestrategy";
		private const string CMDLN_TOKEN_STRICT = "strict";
		private const string CMDLN_TOKEN_TEMPLATEFILE = "templatefile";

		#endregion

		#region Methods/Operators

		private static int DnxMain(string[] args)
		{
			AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.AddResolution<ConsoleApplicationFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<ConsoleApplicationFascade>(new TransientActivatorAutoWiringDependencyResolution<Program>()));

			using (ConsoleApplicationFascade program = AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<ConsoleApplicationFascade>(string.Empty, true))
				return program.EntryPoint(args);
		}

		/// <summary>
		/// The entry point method for this application.
		/// </summary>
		/// <param name="args"> The command line arguments passed from the executing environment. </param>
		/// <returns> The resulting exit code. </returns>
		[STAThread]
		public static int Main(string[] args)
		{
			bool enableDnxDebugQuirksMode;

			enableDnxDebugQuirksMode = EnableDnxDebugQuirksMode;
			Console.WriteLine("EnableDnxDebugQuirksMode: '{0}'", enableDnxDebugQuirksMode);

			if (!enableDnxDebugQuirksMode)
				return DnxMain(args);
			else
			{
				try
				{
					return DnxMain(args);
				}
				catch (Exception)
				{
					// required to do poor mans' "just in time debugging" with F5 in VS 2015 (DNX) for now
					Debugger.Break();
					throw;
				}
			}
		}

		protected override IDictionary<string, ArgumentSpec> GetArgumentMap()
		{
			IDictionary<string, ArgumentSpec> argumentMap;

			argumentMap = new Dictionary<string, ArgumentSpec>();
			argumentMap.Add(CMDLN_TOKEN_TEMPLATEFILE, new ArgumentSpec<string>(true, true));
			argumentMap.Add(CMDLN_TOKEN_SOURCEFILE, new ArgumentSpec<string>(true, true));
			argumentMap.Add(CMDLN_TOKEN_BASEDIR, new ArgumentSpec<string>(true, true));
			argumentMap.Add(CMDLN_TOKEN_SOURCESTRATEGY_AQTN, new ArgumentSpec<string>(true, true));
			argumentMap.Add(CMDLN_TOKEN_STRICT, new ArgumentSpec<bool>(true, true));
			argumentMap.Add(CMDLN_TOKEN_PROPERTY, new ArgumentSpec<string>(false, false));
			argumentMap.Add(CMDLN_DEBUGGER_LAUNCH, new ArgumentSpec<bool>(false, true));

			return argumentMap;
		}

		protected override int OnStartup(string[] args, IDictionary<string, IList<object>> arguments)
		{
			Dictionary<string, object> argz;
			string templateFilePath;
			string sourceFilePath;
			string baseDirectoryPath;
			string sourceStrategyAqtn;
			bool strictMatching;
			bool debuggerLaunch = false;
			IDictionary<string, IList<string>> properties;
			IList<object> argumentValues;
			IList<string> propertyValues;
			bool hasProperties;

			if ((object)args == null)
				throw new ArgumentNullException(nameof(args));

			if ((object)arguments == null)
				throw new ArgumentNullException(nameof(arguments));

			if (arguments.ContainsKey(CMDLN_DEBUGGER_LAUNCH))
				debuggerLaunch = (bool)arguments[CMDLN_DEBUGGER_LAUNCH].Single();

			if (debuggerLaunch)
				Console.WriteLine("Debugger launch result: '{0}'", Debugger.Launch() && Debugger.IsAttached);

			// required
			properties = new Dictionary<string, IList<string>>();

			templateFilePath = (string)arguments[CMDLN_TOKEN_TEMPLATEFILE].Single();
			sourceFilePath = (string)arguments[CMDLN_TOKEN_SOURCEFILE].Single();
			baseDirectoryPath = (string)arguments[CMDLN_TOKEN_BASEDIR].Single();
			sourceStrategyAqtn = (string)arguments[CMDLN_TOKEN_SOURCESTRATEGY_AQTN].Single();
			strictMatching = (bool)arguments[CMDLN_TOKEN_STRICT].Single();

			hasProperties = arguments.TryGetValue(CMDLN_TOKEN_PROPERTY, out argumentValues);

			argz = new Dictionary<string, object>();
			argz.Add(CMDLN_TOKEN_TEMPLATEFILE, templateFilePath);
			argz.Add(CMDLN_TOKEN_SOURCEFILE, sourceFilePath);
			argz.Add(CMDLN_TOKEN_BASEDIR, baseDirectoryPath);
			argz.Add(CMDLN_TOKEN_SOURCESTRATEGY_AQTN, sourceStrategyAqtn);
			argz.Add(CMDLN_TOKEN_STRICT, strictMatching);
			argz.Add(CMDLN_DEBUGGER_LAUNCH, arguments.ContainsKey(CMDLN_DEBUGGER_LAUNCH) ? (object)debuggerLaunch : null);
			argz.Add(CMDLN_TOKEN_PROPERTY, hasProperties ? (object)argumentValues : null);

			if (hasProperties)
			{
				if ((object)argumentValues != null)
				{
					foreach (string argumentValue in argumentValues)
					{
						string key, value;

						if (!this.TryParseCommandLineArgumentProperty(argumentValue, out key, out value))
							continue;

						if (!properties.ContainsKey(key))
							properties.Add(key, propertyValues = new List<string>());
						else
							propertyValues = properties[key];

						// duplicate values are ignored
						if (propertyValues.Contains(value))
							continue;

						propertyValues.Add(value);
					}
				}
			}

			using (IToolHost toolHost = new ToolHost(this.ReflectionFascade) /* TODO: use dependency manager here */)
				toolHost.Host((object)args != null ? args.Length : -1, args, argz, templateFilePath, sourceFilePath, baseDirectoryPath, sourceStrategyAqtn, strictMatching, properties);

			return 0;
		}

		#endregion
	}
}