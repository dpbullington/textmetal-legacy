/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Middleware.Datazoid.Primitives;
using TextMetal.Middleware.Oxymoron.Legacy.Hosting.Tool;
using TextMetal.Middleware.Solder.Executive;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.ConsoleTool.Ox
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

		#region Fields/Constants

		private const string CMDLN_TOKEN_PROPERTY = "property";
		private const string CMDLN_TOKEN_SOURCEFILE = "sourcefile";

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
			AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.AddResolution<ConsoleApplicationFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<ConsoleApplicationFascade>(new TransientActivatorAutoWiringDependencyResolution<Program>()));
			AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.AddResolution<IToolHost>(string.Empty, false, new SingletonWrapperDependencyResolution<IToolHost>(new TransientDefaultConstructorDependencyResolution<ToolHost>()));
			AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.AddResolution<IAdoNetStreamingFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IAdoNetStreamingFascade>(new TransientActivatorAutoWiringDependencyResolution<AdoNetStreamingFascade>()));

			using (ConsoleApplicationFascade program = AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<ConsoleApplicationFascade>(string.Empty, true))
				return program.EntryPoint(args);
		}

		protected override IDictionary<string, ArgumentSpec> GetArgumentMap()
		{
			IDictionary<string, ArgumentSpec> argumentMap;

			argumentMap = new Dictionary<string, ArgumentSpec>();
			argumentMap.Add(CMDLN_TOKEN_SOURCEFILE, new ArgumentSpec<string>(true, true));
			argumentMap.Add(CMDLN_TOKEN_PROPERTY, new ArgumentSpec<string>(false, false));

			return argumentMap;
		}

		protected override int OnStartup(string[] args, IDictionary<string, IList<object>> arguments)
		{
			Dictionary<string, object> argz;
			string templateFilePath;
			string sourceFilePath;
			IDictionary<string, IList<string>> properties;
			IList<object> argumentValues;
			IList<string> propertyValues;
			bool hasProperties;

			if ((object)args == null)
				throw new ArgumentNullException(nameof(args));

			if ((object)arguments == null)
				throw new ArgumentNullException(nameof(arguments));

			// required
			properties = new Dictionary<string, IList<string>>();

			sourceFilePath = (string)arguments[CMDLN_TOKEN_SOURCEFILE].Single();

			hasProperties = arguments.TryGetValue(CMDLN_TOKEN_PROPERTY, out argumentValues);

			argz = new Dictionary<string, object>();
			argz.Add(CMDLN_TOKEN_SOURCEFILE, sourceFilePath);
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

			// minimal viable configuration via code example
			/*using (IToolHost toolHost = new ToolHost())
			{
				toolHost.Host(new ObfuscationConfiguration()
							{
								SourceAdapterConfiguration = new AdapterConfiguration()
															{
																AdapterAqtn = typeof(NullSourceAdapter).AssemblyQualifiedName
															},
								DestinationAdapterConfiguration = new AdapterConfiguration()
																{
																	AdapterAqtn = typeof(NullDestinationAdapter).AssemblyQualifiedName
																},
								TableConfiguration = new TableConfiguration(),
								HashConfiguration = new HashConfiguration()
													{
														Multiplier = 0,
														Seed = 0
													}
							});
			}*/

			using (IToolHost toolHost = AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IToolHost>(string.Empty, true))
				toolHost.Host(sourceFilePath);

			return 0;
		}

		#endregion
	}
}