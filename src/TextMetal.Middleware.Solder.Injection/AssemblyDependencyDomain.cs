/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyModel;

using TextMetal.Middleware.Solder.Primitives;
using TextMetal.Middleware.Solder.Utilities;

using DependencyMagicMethod = System.Action<TextMetal.Middleware.Solder.Injection.AssemblyDependencyDomain>;

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// Serves as a mechansim to detect loading of the singular "app domain" (in .NET Framework parlaence).
	/// This constructor should remain private unless the notion of wrapping
	/// an assembly load context and dependency context extends beyond the runtime defaults.
	/// </summary>
	public sealed class AssemblyDependencyDomain
	{
		#region Constructors/Destructors

		private AssemblyDependencyDomain(AssemblyLoadContext assemblyLoadContext, DependencyContext dependencyContext)
		{
			if ((object)assemblyLoadContext == null)
				throw new ArgumentNullException(nameof(assemblyLoadContext));

			if ((object)dependencyContext == null)
				throw new ArgumentNullException(nameof(dependencyContext));

			this.assemblyLoadContext = assemblyLoadContext;
			this.dependencyContext = dependencyContext;

			// trusted dependencies
			this.dataTypeFascade = new DataTypeFascade();
			this.reflectionFascade = new ReflectionFascade(this.DataTypeFascade);
			this.configurationRoot = LoadAppConfigFile(APP_CONFIG_FILE_NAME);
			this.appConfigFascade = new AppConfigFascade(this.ConfigurationRoot, this.DataTypeFascade);
			this.assemblyInformationFascade = new AssemblyInformationFascade(this.ReflectionFascade, Assembly.GetEntryAssembly());

			this.SetUp();
		}

		#endregion

		#region Fields/Constants

		private const string APP_CONFIG_FILE_NAME = "appconfig.json";
		private readonly IAppConfigFascade appConfigFascade;
		private readonly IAssemblyInformationFascade assemblyInformationFascade;
		private readonly AssemblyLoadContext assemblyLoadContext;
		private readonly IConfigurationRoot configurationRoot;
		private readonly IDataTypeFascade dataTypeFascade;
		private readonly DependencyContext dependencyContext;
		private readonly IDependencyManager dependencyManager = new DependencyManager();
		private readonly IReflectionFascade reflectionFascade;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the singleton, default instance associated with the current assembly/dependency domain.
		/// This is lazy loaded on demad using inner class static field initialization mneumonics.
		/// </summary>
		public static AssemblyDependencyDomain Default
		{
			get
			{
				return AssemblyDependencyDomainLazySingleton.instance;
			}
		}

		private IAppConfigFascade AppConfigFascade
		{
			get
			{
				return this.appConfigFascade;
			}
		}

		internal IAssemblyInformationFascade AssemblyInformationFascade
		{
			get
			{
				return this.assemblyInformationFascade;
			}
		}

		private AssemblyLoadContext AssemblyLoadContext
		{
			get
			{
				return this.assemblyLoadContext;
			}
		}

		private IConfigurationRoot ConfigurationRoot
		{
			get
			{
				return this.configurationRoot;
			}
		}

		private IDataTypeFascade DataTypeFascade
		{
			get
			{
				return this.dataTypeFascade;
			}
		}

		private DependencyContext DependencyContext
		{
			get
			{
				return this.dependencyContext;
			}
		}

		/// <summary>
		/// Gets the dependency manager instance associated with the current assembly/dependency domain.
		/// </summary>
		public IDependencyManager DependencyManager
		{
			get
			{
				return this.dependencyManager;
			}
		}

		internal IReflectionFascade ReflectionFascade
		{
			get
			{
				return this.reflectionFascade;
			}
		}

		#endregion

		#region Methods/Operators

		private static IEnumerable<Assembly> DiscoverAssemblies(DependencyContext dependencyContext)
		{
			if ((object)dependencyContext == null)
				return new Assembly[] { };

			return dependencyContext.RuntimeLibraries
				.SelectMany(library => library.GetDefaultAssemblyNames(dependencyContext))
				.Select(Assembly.Load);
		}

		private static AssemblyDependencyDomain FromDefaultRuntimeContexts()
		{
			return new AssemblyDependencyDomain(AssemblyLoadContext.Default, DependencyContext.Default);
		}

		private static IConfigurationRoot LoadAppConfigFile(string appConfigFilePath)
		{
			IConfigurationBuilder configurationBuilder;
			JsonConfigurationSource configurationSource;
			IConfigurationProvider configurationProvider;
			IConfigurationRoot configurationRoot;

			if ((object)appConfigFilePath == null)
				throw new ArgumentNullException(nameof(appConfigFilePath));

			configurationBuilder = new ConfigurationBuilder();
			configurationSource = new JsonConfigurationSource() { Optional = false, Path = appConfigFilePath };
			configurationProvider = new JsonConfigurationProvider(configurationSource);
			configurationBuilder.Add(configurationSource);
			configurationRoot = configurationBuilder.Build();

			return configurationRoot;
		}

		private void AssemblyLoadContext_OnUnloading(AssemblyLoadContext assemblyLoadContext)
		{
			if ((object)assemblyLoadContext == null)
				throw new ArgumentNullException(nameof(assemblyLoadContext));

			if (this.AssemblyLoadContext != assemblyLoadContext)
				this.HaltAndCatchFire(new DependencyException(string.Format("Assembly load context mismatch during unload notificaiton.")));

			this.TearDown();
		}

		public void HaltAndCatchFire(Exception fatalException)
		{
			if ((object)fatalException == null)
				throw new ArgumentNullException(nameof(fatalException));

			OnlyWhen._PROFILE_ThenPrint(string.Format("Brick {0}", Environment.CurrentManagedThreadId));

			Environment.FailFast(string.Empty, fatalException);
		}

		public Assembly LoadAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;

			assembly = this.AssemblyLoadContext.LoadFromAssemblyName(assemblyName);

			if ((object)assembly != null)
				this.ScanAssembly(assembly);

			return assembly;
		}

		/// <summary>
		/// Private method that will scan all asemblies specified to perform dependency magic.
		/// </summary>
		/// <param name="assemblies"> An enumerable of assemblies to scan for dependency magic methods. </param>
		private void ScanAssemblies(IEnumerable<Assembly> assemblies)
		{
			Type[] assemblyTypes;
			MethodInfo[] methodInfos;
			DependencyMagicMethodAttribute dependencyMagicMethodAttribute;
			DependencyMagicMethod dependencyMagicMethod;

			if ((object)assemblies != null)
			{
				foreach (Assembly assembly in assemblies)
				{
					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}.", assembly.FullName));

					try
					{
						assemblyTypes = assembly.ExportedTypes.ToArray();
					}
					catch (ReflectionTypeLoadException rtlex)
					{
						// is this even needed?
						assemblyTypes = rtlex.Types.Where(t => (object)t != null).ToArray();
					}

					if ((object)assemblyTypes != null)
					{
						foreach (Type assemblyType in assemblyTypes)
						{
							methodInfos = assemblyType.GetMethods(BindingFlags.Public | BindingFlags.Static);

							if ((object)methodInfos != null)
							{
								foreach (MethodInfo methodInfo in methodInfos)
								{
									dependencyMagicMethodAttribute = this.ReflectionFascade.GetOneAttribute<DependencyMagicMethodAttribute>(methodInfo);

									if ((object)dependencyMagicMethodAttribute == null)
										continue;

									if (!methodInfo.IsStatic)
										continue;

									if (!methodInfo.IsPublic)
										continue;

									if (methodInfo.ReturnType != typeof(void))
										continue;

									if (methodInfo.GetParameters().Count() != 1 ||
										methodInfo.GetParameters()[0].ParameterType != typeof(AssemblyDependencyDomain))
										continue;

									dependencyMagicMethod = (DependencyMagicMethod)(methodInfo.CreateDelegate(typeof(DependencyMagicMethod), null /* static */));

									if ((object)dependencyMagicMethod == null)
										continue;

									// notify
									OnlyWhen._PROFILE_ThenPrint(string.Format("{1}::{0}", methodInfo.Name, methodInfo.DeclaringType.FullName));

									dependencyMagicMethod(this);
								}
							}
						}
					}
				}
			}
		}

		public void ScanAssembly(Assembly assembly)
		{
			Assembly[] assemblies;

			if ((object)assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			assemblies = new Assembly[] { assembly };

			this.ScanAssemblies(assemblies);
		}

		/// <summary>
		/// Private thread-safe method which bootstraps an assembly/dependency domain.
		/// </summary>
		private void SetUp()
		{
			lock (this)
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("SetUp {0}", Environment.CurrentManagedThreadId));

				// add trusted dependencies
				this.DependencyManager.AddResolution<IConfigurationRoot>(string.Empty, false, new SingletonWrapperDependencyResolution<IConfigurationRoot>(new InstanceDependencyResolution<IConfigurationRoot>(this.ConfigurationRoot)));
				this.DependencyManager.AddResolution<IDataTypeFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IDataTypeFascade>(new InstanceDependencyResolution<IDataTypeFascade>(this.DataTypeFascade)));
				this.DependencyManager.AddResolution<IReflectionFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IReflectionFascade>(new InstanceDependencyResolution<IReflectionFascade>(this.ReflectionFascade)));
				this.DependencyManager.AddResolution<IAppConfigFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IAppConfigFascade>(new InstanceDependencyResolution<IAppConfigFascade>(this.AppConfigFascade)));
				this.DependencyManager.AddResolution<IAssemblyInformationFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IAssemblyInformationFascade>(new InstanceDependencyResolution<IAssemblyInformationFascade>(this.AssemblyInformationFascade)));

				// hook the unload event
				this.AssemblyLoadContext.Unloading += this.AssemblyLoadContext_OnUnloading;

				// probe known assemblies at build time - does not probe dynamically loaded assmblies yet
				this.ScanAssemblies(DiscoverAssemblies(this.DependencyContext));
			}
		}

		/// <summary>
		/// Private thread-safe method which dismantles an assembly/dependency domain.
		/// Note that the AssemblyLoadContext.Unloading event can/will
		/// executeon a thread different that that of the main thread.
		/// </summary>
		private void TearDown()
		{
			lock (this)
			{
				if ((object)this.DependencyManager != null)
					this.DependencyManager.Dispose();

				this.AssemblyLoadContext.Unloading -= this.AssemblyLoadContext_OnUnloading;

				OnlyWhen._PROFILE_ThenPrint(string.Format("TearDown {0}", Environment.CurrentManagedThreadId));
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// http://www.yoda.arachsys.com/csharp/singleton.html
		/// </summary>
		private class AssemblyDependencyDomainLazySingleton
		{
			#region Constructors/Destructors

			/// <summary>
			/// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			/// </summary>
			static AssemblyDependencyDomainLazySingleton()
			{
			}

			#endregion

			#region Fields/Constants

			internal static readonly AssemblyDependencyDomain instance = FromDefaultRuntimeContexts();

			#endregion
		}

		#endregion
	}
}