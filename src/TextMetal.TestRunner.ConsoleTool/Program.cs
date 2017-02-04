/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TextMetal.Middleware.Solder.Abstractions;
using TextMetal.Middleware.Solder.Executive;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.TestRunner.ConsoleTool
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

		/// <summary>
		/// The entry point method for this application.
		/// </summary>
		/// <param name="args"> The command line arguments passed from the executing environment. </param>
		/// <returns> The resulting exit code. </returns>
		[STAThread]
		public static int Main(string[] args)
		{
			AssemblyDomain.UseRuntimeAdapter<NetFxRuntimeAdapter>();
			AssemblyDomain.Default.DependencyManager.AddResolution<ConsoleApplicationFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<ConsoleApplicationFascade>(new TransientActivatorAutoWiringDependencyResolution<Program>()));
			
			using (ConsoleApplicationFascade program = AssemblyDomain.Default.DependencyManager.ResolveDependency<ConsoleApplicationFascade>(string.Empty, true))
				return program.EntryPoint(args);
		}

		protected override IDictionary<string, ArgumentSpec> GetArgumentMap()
		{
			IDictionary<string, ArgumentSpec> argumentMap;

			argumentMap = new Dictionary<string, ArgumentSpec>();

			return argumentMap;
		}

		protected override int OnStartup(string[] args, IDictionary<string, IList<object>> arguments)
		{
			if ((object)args == null)
				throw new ArgumentNullException(nameof(args));

			if ((object)arguments == null)
				throw new ArgumentNullException(nameof(arguments));

			return 0;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private sealed class NetFxRuntimeAdapter : RuntimeAdapter
		{
			#region Constructors/Destructors

			public NetFxRuntimeAdapter()
				: this(AppDomain.CurrentDomain)
			{
			}

			private NetFxRuntimeAdapter(AppDomain appDomain)
			{
				if ((object)appDomain == null)
					throw new ArgumentNullException(nameof(appDomain));

				this.appDomain = appDomain;

				// hook assembly load context events
				this.AppDomain.AssemblyLoad += this.AppDomain_AssemblyLoad;
				this.AppDomain.DomainUnload += this.AppDomain_DomainUnload;
			}

			#endregion

			#region Fields/Constants

			private readonly AppDomain appDomain;

			#endregion

			#region Properties/Indexers/Events

			private AppDomain AppDomain
			{
				get
				{
					return this.appDomain;
				}
			}

			#endregion

			#region Methods/Operators

			private void AppDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs e)
			{
				if ((object)sender == null)
					throw new ArgumentNullException(nameof(sender));

				if ((object)e == null)
					throw new ArgumentNullException(nameof(e));

				this.OnAssemblyLoaded(e.LoadedAssembly);
			}

			private void AppDomain_DomainUnload(object sender, EventArgs e)
			{
				if ((object)sender == null)
					throw new ArgumentNullException(nameof(sender));

				if ((object)e == null)
					throw new ArgumentNullException(nameof(e));

				this.OnRuntimeTeardown();
			}

			public override void Dispose()
			{
				// unhook assembly load context events
				this.AppDomain.DomainUnload -= this.AppDomain_DomainUnload;
				this.AppDomain.AssemblyLoad -= this.AppDomain_AssemblyLoad;
			}

			public override IEnumerable<Assembly> GetLoadedAssemblies()
			{
				return this.AppDomain.GetAssemblies();
			}

			public override Assembly LoadAssembly(AssemblyName assemblyName)
			{
				if ((object)assemblyName == null)
					throw new ArgumentNullException(nameof(assemblyName));

				return this.AppDomain.Load(assemblyName);
			}

			#endregion
		}

		#endregion
	}
}