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

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// Serves as a mechansim to detect loading of the singular "app domain" (in .NET Framework parlaence)
	/// in an app model agnostic manner. This constructor should remain private unless the notion
	/// of wrapping an assembly load contet and dependency context extends beyond the runtime defaults.
	/// </summary>
	public sealed class AgnosticAppDomain
	{
		#region Constructors/Destructors

		private AgnosticAppDomain(AssemblyLoadContext assemblyLoadContext, DependencyContext dependencyContext)
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

			this.SetUpApplicationDomain();
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
		private readonly IList<Action<AssemblyLoaderEventType, AgnosticAppDomain>> eventSinkMethods = new List<Action<AssemblyLoaderEventType, AgnosticAppDomain>>();
		private readonly IReflectionFascade reflectionFascade;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the singleton, current instance associated with the current app model agnostic "app domain".
		/// This is lazy loaded on demad using inner class static field initialization mneumonics.
		/// </summary>
		public static AgnosticAppDomain TheOnlyAllowedInstance
		{
			get
			{
				return AgnosticAppDomainLazySingleton.instance;
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
		/// Gets the dependency manager instance associated with the current assembly loader container context.
		/// </summary>
		public IDependencyManager DependencyManager
		{
			get
			{
				return this.dependencyManager;
			}
		}

		private IList<Action<AssemblyLoaderEventType, AgnosticAppDomain>> EventSinkMethods
		{
			get
			{
				return this.eventSinkMethods;
			}
		}

		private IReflectionFascade ReflectionFascade
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
				Environment.FailFast(string.Format("Assembly load context mismatch during unload notificaiton."));

			this.TearDownApplicationDomain();
		}

		public Assembly LoadAssembly(AssemblyName assemblyName)
		{
			return this.AssemblyLoadContext.LoadFromAssemblyName(assemblyName);
		}

		/// <summary>
		/// Private method that will scan all asemblies specified to perform assembly loader subscription method execution.
		/// </summary>
		/// <param name="assemblies"> An enumerable of assemblies to scan for assembly loader subscription methods. </param>
		private void ScanAssemblies(IEnumerable<Assembly> assemblies)
		{
			Type[] assemblyTypes;
			MethodInfo[] methodInfos;
			AssemblyLoaderEventSinkMethodAttribute assemblyLoaderEventSinkMethodAttribute;
			Action<AssemblyLoaderEventType, AgnosticAppDomain> assemblyLoaderEventSinkMethod;

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
									assemblyLoaderEventSinkMethodAttribute = this.ReflectionFascade.GetOneAttribute<AssemblyLoaderEventSinkMethodAttribute>(methodInfo);

									if ((object)assemblyLoaderEventSinkMethodAttribute == null)
										continue;

									if (!methodInfo.IsStatic)
										continue;

									if (!methodInfo.IsPublic)
										continue;

									if (methodInfo.ReturnType != typeof(void))
										continue;

									if (methodInfo.GetParameters().Count() != 2 ||
										methodInfo.GetParameters()[0].ParameterType != typeof(AssemblyLoaderEventType) ||
										methodInfo.GetParameters()[1].ParameterType != typeof(AgnosticAppDomain))
										continue;

									assemblyLoaderEventSinkMethod = (Action<AssemblyLoaderEventType, AgnosticAppDomain>)(methodInfo.CreateDelegate(typeof(Action<AssemblyLoaderEventType, AgnosticAppDomain>), null /* static */));

									if ((object)assemblyLoaderEventSinkMethod == null)
										continue;

									if (this.EventSinkMethods.Contains(assemblyLoaderEventSinkMethod))
										throw new NotSupportedException(string.Format("Method '{0}' of type '{1}' has been sunk before. This is likely a defect in the framework - report this to the project team.", methodInfo.Name, methodInfo.DeclaringType.FullName));

									// notify
									OnlyWhen._PROFILE_ThenPrint(string.Format("{1}::{0}", methodInfo.Name, methodInfo.DeclaringType.FullName));

									this.EventSinkMethods.Add(assemblyLoaderEventSinkMethod);
									assemblyLoaderEventSinkMethod(AssemblyLoaderEventType.Startup, this);
								}
							}
						}
					}
				}
			}
		}

		public void ScanAssembly<TAssemblyOfTargetType>()
		{
			Type assemblyOfTargetType;

			assemblyOfTargetType = typeof(TAssemblyOfTargetType);

			this.ScanAssembly(assemblyOfTargetType);
		}

		public void ScanAssembly(Type assemblyOfTargetType)
		{
			Assembly assembly;

			if ((object)assemblyOfTargetType == null)
				throw new ArgumentNullException(nameof(assemblyOfTargetType));

			assembly = assemblyOfTargetType.GetTypeInfo().Assembly;

			this.ScanAssembly(assembly);
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
		/// Private thread-safe method which bootstraps an "app domain".
		/// </summary>
		private void SetUpApplicationDomain()
		{
			lock (this)
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("SetUpApplicationDomain {0}", Environment.CurrentManagedThreadId));

				// add trusted dependencies
				this.DependencyManager.AddResolution<IConfigurationRoot>(string.Empty, false, new SingletonWrapperDependencyResolution<IConfigurationRoot>(new InstanceDependencyResolution<IConfigurationRoot>(this.ConfigurationRoot)));
				this.DependencyManager.AddResolution<IDataTypeFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IDataTypeFascade>(new InstanceDependencyResolution<IDataTypeFascade>(this.DataTypeFascade)));
				this.DependencyManager.AddResolution<IReflectionFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IReflectionFascade>(new InstanceDependencyResolution<IReflectionFascade>(this.ReflectionFascade)));
				this.DependencyManager.AddResolution<IAppConfigFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IAppConfigFascade>(new InstanceDependencyResolution<IAppConfigFascade>(this.AppConfigFascade)));
				this.DependencyManager.AddResolution<IAssemblyInformationFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IAssemblyInformationFascade>(new InstanceDependencyResolution<IAssemblyInformationFascade>(this.AssemblyInformationFascade)));

				// hook the unload event
				this.AssemblyLoadContext.Unloading += this.AssemblyLoadContext_OnUnloading;

				// probe known assemblies - does not probe dynamically loaded assmblies yet
				this.ScanAssemblies(DiscoverAssemblies(this.DependencyContext));
			}
		}

		/// <summary>
		/// Private thread-safe method which dismantles an "app domain".
		/// Note that the AssemblyLoadContext.Unloading event can/will
		/// executeon a thread different that that of the main thread.
		/// </summary>
		private void TearDownApplicationDomain()
		{
			lock (this)
			{
				// notify
				foreach (Action<AssemblyLoaderEventType, AgnosticAppDomain> eventSinkMethod in this.EventSinkMethods)
					eventSinkMethod(AssemblyLoaderEventType.Shutdown, this);

				// cleanup
				this.EventSinkMethods.Clear();

				if ((object)this.DependencyManager != null)
					this.DependencyManager.Dispose();

				this.AssemblyLoadContext.Unloading -= this.AssemblyLoadContext_OnUnloading;

				OnlyWhen._PROFILE_ThenPrint(string.Format("TearDownApplicationDomain {0}", Environment.CurrentManagedThreadId));
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// http://www.yoda.arachsys.com/csharp/singleton.html
		/// </summary>
		private class AgnosticAppDomainLazySingleton
		{
			#region Constructors/Destructors

			/// <summary>
			/// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			/// </summary>
			static AgnosticAppDomainLazySingleton()
			{
			}

			#endregion

			#region Fields/Constants

			internal static readonly AgnosticAppDomain instance = new AgnosticAppDomain(AssemblyLoadContext.Default, DependencyContext.Default);

			#endregion
		}

		#endregion
	}
}