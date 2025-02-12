﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Middleware.Solder.Executive;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.Hosting.Tool
{
	/// <summary>
	/// Entry point class for the application.
	/// </summary>
	public class TextMetalConsoleApplication : SyncExecutableApplicationFascade
	{
		#region Constructors/Destructors

		[DependencyInjection]
		public TextMetalConsoleApplication([DependencyInjection] IDataTypeFascade dataTypeFascade, [DependencyInjection] IAppConfigFascade appConfigFascade, [DependencyInjection] IReflectionFascade reflectionFascade, [DependencyInjection] IAssemblyInformationFascade assemblyInformationFascade)
			: base(dataTypeFascade, appConfigFascade, reflectionFascade, assemblyInformationFascade)
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

		public static int Run(string[] args)
		{
			using (SyncExecutableApplicationFascade program = AssemblyDomain.Default.DependencyManager.ResolveDependency<SyncExecutableApplicationFascade>(string.Empty, true))
				return program.EntryPoint(args);
		}

		[DependencyMagicMethod]
		public static void OnDependencyMagic(IDependencyManager dependencyManager)
		{
			if ((object)dependencyManager == null)
				throw new ArgumentNullException(nameof(dependencyManager));

			dependencyManager.AddResolution<SyncExecutableApplicationFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<SyncExecutableApplicationFascade>(TransientActivatorAutoWiringDependencyResolution<TextMetalConsoleApplication>.From(dependencyManager)));
			dependencyManager.AddResolution<ITextMetalToolHost>(string.Empty, false, new SingletonWrapperDependencyResolution<ITextMetalToolHost>(TransientActivatorAutoWiringDependencyResolution<TextMetalToolHost>.From(dependencyManager)));
			dependencyManager.AddResolution<IAdoNetBufferingFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IAdoNetBufferingFascade>(TransientActivatorAutoWiringDependencyResolution<AdoNetBufferingFascade>.From(dependencyManager)));
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
				Console.WriteLine("Debugger launch result: '{0}'", System.Diagnostics.Debugger.Launch() && System.Diagnostics.Debugger.IsAttached);

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

						if (!TryParseCommandLineArgumentProperty(argumentValue, out key, out value))
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

			using (ITextMetalToolHost textMetalToolHost = AssemblyDomain.Default.DependencyManager.ResolveDependency<ITextMetalToolHost>(string.Empty, true))
				textMetalToolHost.Host((object)args != null ? args.Length : -1, args, argz, templateFilePath, sourceFilePath, baseDirectoryPath, sourceStrategyAqtn, strictMatching, properties);

			return 0;
		}

		#endregion
	}
}