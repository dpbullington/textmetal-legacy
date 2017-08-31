/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

using TextMetal.Middleware.Solder.Primitives;
using TextMetal.Middleware.Solder.Utilities;

using DependencyMagicMethod = System.Action<TextMetal.Middleware.Solder.Injection.IDependencyManager>;

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// Serves as a logical run-time boundary for assemblies.
	/// </summary>
	public sealed class AssemblyDomain : IDisposable
	{
		#region Constructors/Destructors

		static AssemblyDomain()
		{
			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::{1} --> {2}", nameof(AssemblyDomain), nameof(AssemblyDomain), Environment.CurrentManagedThreadId));

			lock (defaultSyncObj)
			{
				if ((object)Default != null)
					throw new DependencyException(string.Format("The default instance has already been initialized."));

				Default = new AssemblyDomain();
			}
		}

		private AssemblyDomain()
			: this(AppDomain.CurrentDomain)
		{
		}

		private AssemblyDomain(AppDomain appDomain)
		{
			if ((object)appDomain == null)
				throw new ArgumentNullException(nameof(appDomain));

			this.appDomain = appDomain;

			this.Initialize();
		}

		#endregion

		#region Fields/Constants

		private const string APP_CONFIG_FILE_NAME = "appconfig.json";
		private static readonly object defaultSyncObj = new object();
		private static AssemblyDomain @default;
		private readonly AppDomain appDomain;
		private readonly IDependencyManager dependencyManager = new DependencyManager();
		private readonly IDictionary<AssemblyName, Assembly> knownAssemblies = new Dictionary<AssemblyName, Assembly>();
		private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		private bool disposed;
		private bool initialized;

		#endregion

		#region Properties/Indexers/Events

		public static AssemblyDomain Default
		{
			get
			{
				return @default;
			}
			private set
			{
				@default = value;
			}
		}

		private AppDomain AppDomain
		{
			get
			{
				return this.appDomain;
			}
		}

		/// <summary>
		/// Gets the dependency manager instance associated with the current assembly domain.
		/// </summary>
		public IDependencyManager DependencyManager
		{
			get
			{
				return this.dependencyManager;
			}
		}

		private IDictionary<AssemblyName, Assembly> KnownAssemblies
		{
			get
			{
				return this.knownAssemblies;
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

		public void Dispose()
		{
			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::{1} --> {2}", nameof(AssemblyDomain), nameof(this.Dispose), Environment.CurrentManagedThreadId));

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				if (this.Disposed)
					return;

				// cop a writer lock
				this.ReaderWriterLock.EnterWriteLock();

				try
				{
					if ((object)this.DependencyManager != null)
						this.DependencyManager.Dispose();

					if ((object)this.AppDomain != null)
					{
						// unhook appp domain events
						this.AppDomain.AssemblyLoad -= this.AppDomain_AssemblyLoad;
						this.AppDomain.DomainUnload -= this.AppDomain_DomainUnload;

						// DO NOT DISPOSE HERE
						//this.AppDomain.Dispose();
					}
				}
				finally
				{
					this.Disposed = true;
					this.ReaderWriterLock.ExitWriteLock();
				}
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
				GC.SuppressFinalize(this);
			}
		}

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

					// execute (assuming under existing SRWL)
					dependencyMagicMethod(this.DependencyManager);
				}
			}
		}

		/// <summary>
		/// Private thread-safe method which bootstraps an assembly/dependency domain.
		/// </summary>
		private void Initialize()
		{
			IEnumerable<Assembly> assemblies;

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				if (this.Initialized)
					throw new DependencyException(string.Format("This instance has already been initialized."));

				if (this.Disposed)
					throw new ObjectDisposedException(typeof(AssemblyDomain).FullName);

				// cop a writer lock
				this.ReaderWriterLock.EnterWriteLock();

				try
				{
					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::{1} --> {2}", nameof(AssemblyDomain), nameof(this.Initialize), Environment.CurrentManagedThreadId));

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

					// hook app domain events
					this.AppDomain.DomainUnload += this.AppDomain_DomainUnload;
					this.AppDomain.AssemblyLoad += this.AppDomain_AssemblyLoad;

					// probe known assemblies at run-time
					assemblies = this.AppDomain.GetAssemblies();
					this.ScanAssemblies(assemblies);

					this.Initialized = true;
				}
				finally
				{
					this.ReaderWriterLock.ExitWriteLock();
				}
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
			}
		}

		public Assembly LoadAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;

			if ((object)assemblyName == null)
				throw new ArgumentNullException(nameof(assemblyName));

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				if (this.Disposed)
					throw new ObjectDisposedException(typeof(AssemblyDomain).FullName);

				assembly = this.AppDomain.Load(assemblyName);

				// probe explicit dynamically loaded assmblies
				if ((object)assembly != null)
					this.ScanAssembly(assembly);

				return assembly;
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
			}
		}

		private void OnAssemblyLoaded(Assembly assembly)
		{
			if ((object)assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				if (this.Disposed)
					throw new ObjectDisposedException(typeof(AssemblyDomain).FullName);

				// probe implicit dynamically loaded assmblies
				if ((object)assembly != null)
					this.ScanAssembly(assembly);
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
			}
		}

		private void OnRuntimeTeardown()
		{
			// simply dispose
			this.Dispose();
		}

		/// <summary>
		/// Private method that will scan all asemblies specified to perform dependency magic.
		/// </summary>
		/// <param name="assemblies"> An enumerable of assemblies to scan for dependency magic methods. </param>
		private void ScanAssemblies(IEnumerable<Assembly> assemblies)
		{
			if ((object)assemblies != null)
			{
				foreach (Assembly assembly in assemblies)
					this.ScanAssembly(assembly);
			}
		}

		private void ScanAssembly(Assembly assembly)
		{
			AssemblyName assemblyName;

			Type[] assemblyTypes;

			if ((object)assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			assemblyName = assembly.GetName();

			if (this.KnownAssemblies.ContainsKey(assemblyName))
				return;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}.", assembly.FullName));

			// track which ones we have seen - not sure if AN is fully ==...
			this.KnownAssemblies.Add(assemblyName, assembly);

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

		#endregion
	}
}