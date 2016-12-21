/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;

using TextMetal.Middleware.Datazoid.Primitives;
using TextMetal.Middleware.Solder.Executive;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Mz.ConsoleTool
{
	/// <summary>
	/// Entry point class for the application.
	/// </summary>
	internal class Program : ConsoleApplicationFascade
	{
		#region Constructors/Destructors

		[DependencyInjection]
		public Program([DependencyInjection] IDataTypeFascade dataTypeFascade, [DependencyInjection] IAppConfigFascade appConfigFascade, [DependencyInjection] IReflectionFascade reflectionFascade, [DependencyInjection] IAssemblyInformationFascade assemblyInformationFascade)
			: base(dataTypeFascade, appConfigFascade, reflectionFascade, assemblyInformationFascade)
		{
		}

		#endregion

		#region Methods/Operators

		private static int DnxMain(string[] args)
		{
			AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.AddResolution<ConsoleApplicationFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<ConsoleApplicationFascade>(new TransientActivatorAutoWiringDependencyResolution<Program>()));
			//AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.AddResolution<IToolHost>(string.Empty, false, new SingletonWrapperDependencyResolution<IToolHost>(new TransientActivatorAutoWiringDependencyResolution<ToolHost>()));
			AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.AddResolution<IAdoNetBufferingFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IAdoNetBufferingFascade>(new TransientActivatorAutoWiringDependencyResolution<AdoNetBufferingFascade>()));

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

			return argumentMap;
		}

		protected override int OnStartup(string[] args, IDictionary<string, IList<object>> arguments)
		{
			new ConsoleIntegrationEngine().RunHost();

			//using (IToolHost toolHost = AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IToolHost>(string.Empty, true))
			//toolHost.Host((object)args != null ? args.Length : -1, args, argz, templateFilePath, sourceFilePath, baseDirectoryPath, sourceStrategyAqtn, strictMatching, properties);

			return 0;
		}

		#endregion
	}
}