/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyModel;

using TextMetal.Middleware.Solder.Primitives;
using TextMetal.Middleware.Solder.Utilities;

using DependencyMagicMethod = System.Action<TextMetal.Middleware.Solder.Injection.IDependencyManager>;

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// Serves as a mechansim to detect loading of the singular "app domain" (in .NET Framework parlaence).
	/// This constructor should remain private unless the notion of wrapping
	/// an assembly load context and dependency context extends beyond the runtime defaults.
	/// </summary>
	public sealed class AssemblyDependencyDomain : IDisposable
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

			this.Initialize();
		}

		#endregion

		#region Fields/Constants

		private const string APP_CONFIG_FILE_NAME = "appconfig.json";
		private readonly IDictionary<Assembly, IReadOnlyCollection<DependencyMagicMethod>> assemblyDependencyMagicMethods = new Dictionary<Assembly, IReadOnlyCollection<DependencyMagicMethod>>();
		private readonly AssemblyLoadContext assemblyLoadContext;
		private readonly DependencyContext dependencyContext;
		private readonly IDependencyManager dependencyManager = new DependencyManager();
		private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		private bool disposed;
		private bool initialized;

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

		private IDictionary<Assembly, IReadOnlyCollection<DependencyMagicMethod>> AssemblyDependencyMagicMethods
		{
			get
			{
				return this.assemblyDependencyMagicMethods;
			}
		}

		private AssemblyLoadContext AssemblyLoadContext
		{
			get
			{
				return this.assemblyLoadContext;
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

		private ReaderWriterLockSlim ReaderWriterLock
		{
			get
			{
				return this.readerWriterLock;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current instance has been disposed.
		/// </summary>
		public bool Disposed
		{
			get
			{
				return this.disposed;
			}
			private set
			{
				this.disposed = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current instance has been initialized.
		/// </summary>
		private bool Initialized
		{
			get
			{
				return this.initialized;
			}
			set
			{
				this.initialized = value;
			}
		}

		#endregion

		#region Methods/Operators

		private static IEnumerable<Assembly> DiscoverRuntimeLibraryAssemblies(DependencyContext dependencyContext)
		{
			if ((object)dependencyContext == null)
				return new Assembly[] { };

			return dependencyContext.RuntimeLibraries
				.SelectMany(library => library.GetDefaultAssemblyNames(dependencyContext))
				.Select(assemblyName => Assembly.Load(assemblyName));
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

		private Assembly AssemblyLoadContext_Resolving(AssemblyLoadContext assemblyLoadContext, AssemblyName assemblyName)
		{
			return null;
		}

		/// <summary>
		/// Private thread-safe method which dismantles an assembly/dependency domain.
		/// Note that the AssemblyLoadContext.Unloading event can/will
		/// executeon a thread different that that of the main thread.
		/// </summary>
		/// <param name="assemblyLoadContext"> </param>
		private void AssemblyLoadContext_Unloading(AssemblyLoadContext assemblyLoadContext)
		{
			if ((object)assemblyLoadContext == null)
				throw new ArgumentNullException(nameof(assemblyLoadContext));

			if (this.AssemblyLoadContext != assemblyLoadContext)
				this.HaltAndCatchFire(new DependencyException(string.Format("Assembly load context mismatch during unload notificaiton.")));

			// simply dispose
			this.Dispose();
		}

		public void Dispose()
		{
			if (this.Disposed)
				return;

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				// cop a writer lock
				this.ReaderWriterLock.EnterWriteLock();

				try
				{
					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::{1} --> {2}", nameof(AssemblyDependencyDomain), nameof(this.Dispose), Environment.CurrentManagedThreadId));

					this.AssemblyDependencyMagicMethods.Clear();

					if ((object)this.DependencyManager != null)
						this.DependencyManager.Dispose();

					// unhook the assembly events
					this.AssemblyLoadContext.Unloading -= this.AssemblyLoadContext_Unloading;
					//this.AssemblyLoadContext.Resolving -= this.AssemblyLoadContext_Resolving;
				}
				finally
				{
					this.ReaderWriterLock.ExitWriteLock();
				}
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
				this.Disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Fire order is not guaranteed.
		/// </summary>
		/// <param name="assemblyType"> </param>
		private void FireDependencyMagicMethods(Type assemblyType)
		{
			MethodInfo[] methodInfos;
			DependencyMagicMethodAttribute dependencyMagicMethodAttribute;
			DependencyMagicMethod dependencyMagicMethod;

			IReflectionFascade reflectionFascade;

			// dogfooding here ;)
			//reflectionFascade = new ReflectionFascade(new DataTypeFascade());
			reflectionFascade = this.DependencyManager.ResolveDependency<IReflectionFascade>(string.Empty, false);

			methodInfos = assemblyType.GetMethods(BindingFlags.Public | BindingFlags.Static);

			if ((object)assemblyType == null)
				throw new ArgumentNullException(nameof(assemblyType));

			if ((object)methodInfos != null)
			{
				foreach (MethodInfo methodInfo in methodInfos)
				{
					dependencyMagicMethodAttribute = reflectionFascade.GetOneAttribute<DependencyMagicMethodAttribute>(methodInfo);

					if ((object)dependencyMagicMethodAttribute == null)
						continue;

					if (!methodInfo.IsStatic)
						continue;

					if (!methodInfo.IsPublic)
						continue;

					if (methodInfo.ReturnType != typeof(void))
						continue;

					if (methodInfo.GetParameters().Count() != 1 ||
						methodInfo.GetParameters()[0].ParameterType != typeof(IDependencyManager))
						continue;

					dependencyMagicMethod = (DependencyMagicMethod)(methodInfo.CreateDelegate(typeof(DependencyMagicMethod), null /* static */));

					if ((object)dependencyMagicMethod == null)
						continue;

					// notify
					OnlyWhen._PROFILE_ThenPrint(string.Format("{1}::{0}", methodInfo.Name, methodInfo.DeclaringType.FullName));

					lock (this) // should we track which we have seen?
						dependencyMagicMethod(this.DependencyManager);
				}
			}
		}

		public void HaltAndCatchFire(Exception fatalException)
		{
			if ((object)fatalException == null)
				throw new ArgumentNullException(nameof(fatalException));

			OnlyWhen._PROFILE_ThenPrint(string.Format("Brick {0}", Environment.CurrentManagedThreadId));

			Environment.FailFast(string.Empty, fatalException);
		}

		/// <summary>
		/// Private thread-safe method which bootstraps an assembly/dependency domain.
		/// </summary>
		private void Initialize()
		{
			if (this.Initialized)
				this.HaltAndCatchFire(new DependencyException("This instance has already been initialized."));

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(AssemblyDependencyDomain).FullName);

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				// cop a writer lock
				this.ReaderWriterLock.EnterWriteLock();

				try
				{
					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::{1} --> {2}", nameof(AssemblyDependencyDomain), nameof(this.Initialize), Environment.CurrentManagedThreadId));

					// add trusted dependencies
					{
						IDataTypeFascade dataTypeFascade;
						IReflectionFascade reflectionFascade;
						IConfigurationRoot configurationRoot;
						IAppConfigFascade appConfigFascade;
						IAssemblyInformationFascade assemblyInformationFascade;

						dataTypeFascade = new DataTypeFascade();
						reflectionFascade = new ReflectionFascade(dataTypeFascade);
						configurationRoot = LoadAppConfigFile(APP_CONFIG_FILE_NAME);
						appConfigFascade = new AppConfigFascade(configurationRoot, dataTypeFascade);
						assemblyInformationFascade = new AssemblyInformationFascade(reflectionFascade, Assembly.GetEntryAssembly());

						this.DependencyManager.AddResolution<IConfigurationRoot>(string.Empty, false, new SingletonWrapperDependencyResolution<IConfigurationRoot>(new InstanceDependencyResolution<IConfigurationRoot>(configurationRoot)));
						this.DependencyManager.AddResolution<IDataTypeFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IDataTypeFascade>(new InstanceDependencyResolution<IDataTypeFascade>(dataTypeFascade)));
						this.DependencyManager.AddResolution<IReflectionFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IReflectionFascade>(new InstanceDependencyResolution<IReflectionFascade>(reflectionFascade)));
						this.DependencyManager.AddResolution<IAppConfigFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IAppConfigFascade>(new InstanceDependencyResolution<IAppConfigFascade>(appConfigFascade)));
						this.DependencyManager.AddResolution<IAssemblyInformationFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IAssemblyInformationFascade>(new InstanceDependencyResolution<IAssemblyInformationFascade>(assemblyInformationFascade)));
					}

					// hook the assembly events
					//this.AssemblyLoadContext.Resolving += this.AssemblyLoadContext_Resolving;
					this.AssemblyLoadContext.Unloading += this.AssemblyLoadContext_Unloading;

					// probe known assemblies at build time
					this.ScanAssemblies(DiscoverRuntimeLibraryAssemblies(this.DependencyContext));
				}
				finally
				{
					this.ReaderWriterLock.ExitWriteLock();
				}
			}
			finally
			{
				this.Initialized = true;
				this.ReaderWriterLock.ExitUpgradeableReadLock();
			}
		}

		public Assembly LoadAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(AssemblyDependencyDomain).FullName);

			assembly = this.AssemblyLoadContext.LoadFromAssemblyName(assemblyName);

			// probe explicit dynamically loaded assmblies
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
							// TODO: in the future we could support real auto-wire via type probing
							this.FireDependencyMagicMethods(assemblyType);
						}
					}
				}
			}
		}

		private void ScanAssembly(Assembly assembly)
		{
			Assembly[] assemblies;

			if ((object)assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			assemblies = new Assembly[] { assembly };

			this.ScanAssemblies(assemblies);
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