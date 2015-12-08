using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.PlatformAbstractions;

using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Solder.Runtime
{
	public sealed class AssemblyLoaderContainerContext
	{
		#region Constructors/Destructors

		/// <summary>
		/// Serves as a constructor to detect loading of the "app domain" in an app model agnostic manner.
		/// </summary>
		private AssemblyLoaderContainerContext()
		{
			this.SetUpApplicationDomain();
		}

		/// <summary>
		/// Serves as a finalizer to detect unloading of the "app domain" in an app model agnostic manner.
		/// </summary>
		~AssemblyLoaderContainerContext()
		{
			this.TearDownApplicationDomain();
		}

		#endregion

		#region Fields/Constants

		private readonly IDependencyManager dependencyManager = new DependencyManager();

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the current app setting for disabling of resolution auto-wiring.
		/// </summary>
		private static bool DisableResolutionAutoWire
		{
			get
			{
				string key;
				bool value;

				key = string.Format("{0}::DisableResolutionAutoWire", typeof(AssemblyLoaderContainerContext).FullName);

				if (!AppConfigFascade.Instance.HasAppSetting(key))
					return true;

				value = AppConfigFascade.Instance.GetAppSetting<bool>(key);

				return value;
			}
		}

		public static AssemblyLoaderContainerContext TheOnlyAllowedInstance
		{
			get
			{
				return LazySingleton.lazyInstance;
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

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Private method that will scan all asemblies specified to perform auto-wiring of dependencies.
		/// </summary>
		/// <param name="assemblies"> An arry of ssemblies to scan and load dependency resolutions automatically ("auto-wire" feature). </param>
		private void ScanAssemblies(Assembly[] assemblies)
		{
			Type[] assemblyTypes;
			MethodInfo[] methodInfos;
			DependencyRegistrationAttribute dependencyRegistrationAttribute;
			Action dependencyRegistrationMethod;

			if ((object)assemblies != null)
			{
				foreach (Assembly assembly in assemblies)
				{
					dependencyRegistrationAttribute = ReflectionFascade.Instance.GetOneAttribute<DependencyRegistrationAttribute>(assembly);

					if ((object)dependencyRegistrationAttribute == null)
						continue;

					// http://stackoverflow.com/questions/7889228/how-to-prevent-reflectiontypeloadexception-when-calling-assembly-gettypes
					try
					{
						assemblyTypes = assembly.GetTypes();
						//assemblyTypes = assembly.DefinedTypes.Select(t => t.AsType());
					}
					catch (ReflectionTypeLoadException rtlex)
					{
						assemblyTypes = rtlex.Types.Where(t => (object)t != null).ToArray();
					}

					if ((object)assemblyTypes != null)
					{
						foreach (Type assemblyType in assemblyTypes)
						{
							var _assemblyTypeInfo = assemblyType.GetTypeInfo();

							dependencyRegistrationAttribute = ReflectionFascade.Instance.GetOneAttribute<DependencyRegistrationAttribute>(assemblyType);

							if ((object)dependencyRegistrationAttribute == null)
								continue;

							if (!_assemblyTypeInfo.IsPublic)
								continue;

							methodInfos = assemblyType.GetMethods(BindingFlags.Public | BindingFlags.Static);

							if ((object)methodInfos != null)
							{
								foreach (MethodInfo methodInfo in methodInfos)
								{
									dependencyRegistrationAttribute = ReflectionFascade.Instance.GetOneAttribute<DependencyRegistrationAttribute>(methodInfo);

									if ((object)dependencyRegistrationAttribute == null)
										continue;

									if (!methodInfo.IsStatic)
										continue;

									if (!methodInfo.IsPublic)
										continue;

									dependencyRegistrationMethod = (Action)(methodInfo.CreateDelegate(typeof(Action), null /* static */));

									if ((object)dependencyRegistrationMethod == null)
										continue;

									dependencyRegistrationMethod();
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Private thread-safe method which bootstraps an "app domain" for dependency management.
		/// </summary>
		private void SetUpApplicationDomain()
		{
			Console.WriteLine("SetUpApplicationDomain");
			if (!DisableResolutionAutoWire)
				return;

			ILibraryManager libraryManager = null;

			if ((object)libraryManager != null)
			{
				var assemblies = libraryManager.GetLibraries().SelectMany(l => l.Assemblies.Select(an =>
																									{
																										try
																										{
																											return Assembly.Load(an);
																										}
																										catch (ReflectionTypeLoadException)
																										{
																											return null;
																										}
																									}));

				this.ScanAssemblies(assemblies.ToArray());
			}
		}

		/// <summary>
		/// Private thread-safe method which dismantles an "app domain" for dependency management.
		/// </summary>
		private void TearDownApplicationDomain()
		{
			// cleanup
			if ((object)this.DependencyManager != null)
				this.DependencyManager.Dispose();

			Console.WriteLine("TearDownApplicationDomain");
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// http://www.yoda.arachsys.com/csharp/singleton.html
		/// </summary>
		private class LazySingleton
		{
			#region Constructors/Destructors

			/// <summary>
			/// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			/// </summary>
			static LazySingleton()
			{
			}

			#endregion

			#region Fields/Constants

			internal static readonly AssemblyLoaderContainerContext lazyInstance = new AssemblyLoaderContainerContext();

			#endregion
		}

		#endregion
	}
}