/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
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
		private readonly IList<Action<AssemblyLoaderEventType, AssemblyLoaderContainerContext>> eventSinkMethods = new List<Action<AssemblyLoaderEventType, AssemblyLoaderContainerContext>>();

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the current app setting for disabling of assembly loader subscription method execution.
		/// </summary>
		private static bool EnvVarEnableAssemblyLoaderSubscriptionMethodExecution
		{
			get
			{
				string key;
				string svalue;
				bool ovalue;

				key = string.Format("SOLDER_ENABLE_ASMLDR_EVENTS");
				svalue = Environment.GetEnvironmentVariable(key);

				if ((object)svalue == null)
					return false;

				if (!DataTypeFascade.Instance.TryParse<bool>(svalue, out ovalue))
					return false;

				return ovalue;
			}
		}

		/// <summary>
		/// Gets the singleton instance associated with the current assembly loader container context.
		/// Most applications will use this instance instead of creating their own instance.
		/// </summary>
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

		private IList<Action<AssemblyLoaderEventType, AssemblyLoaderContainerContext>> EventSinkMethods
		{
			get
			{
				return this.eventSinkMethods;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Private method that will scan all asemblies specified to perform assembly loader subscription method execution.
		/// </summary>
		/// <param name="assemblies"> An array of assemblies to scan for assembly loader subscription methods. </param>
		private void ScanAssemblies(Assembly[] assemblies)
		{
			Type[] assemblyTypes;
			MethodInfo[] methodInfos;
			AssemblyLoaderEventSinkMethodAttribute assemblyLoaderEventSinkMethodAttribute;
			Action<AssemblyLoaderEventType, AssemblyLoaderContainerContext> assemblyLoaderEventSinkMethod;

			if ((object)assemblies != null)
			{
				foreach (Assembly assembly in assemblies)
				{
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

							methodInfos = assemblyType.GetMethods(BindingFlags.Public | BindingFlags.Static);

							if ((object)methodInfos != null)
							{
								foreach (MethodInfo methodInfo in methodInfos)
								{
									assemblyLoaderEventSinkMethodAttribute = ReflectionFascade.Instance.GetOneAttribute<AssemblyLoaderEventSinkMethodAttribute>(methodInfo);

									if ((object)assemblyLoaderEventSinkMethodAttribute == null)
										continue;

									if (!methodInfo.IsStatic)
										continue;

									if (!methodInfo.IsPublic)
										continue;

									if (methodInfo.ReturnType != typeof(void))
										continue;

									if (methodInfo.GetParameters().Count() == 1 &&
										methodInfo.GetParameters()[0].ParameterType == typeof(IDependencyManager))
									{
										Console.WriteLine("Method '{0}' of type '{1}' uses an unsupported method signature. The migration is to use the new signature '{2} ({3}, {4})', multi-path the logic based on event type, and access the in-effect dependency manager from the context paramter value.", methodInfo.Name, methodInfo.DeclaringType.FullName, typeof(void).Name, nameof(AssemblyLoaderEventType), nameof(AssemblyLoaderContainerContext));
										continue;
									}

									if (methodInfo.GetParameters().Count() != 2 ||
										methodInfo.GetParameters()[0].ParameterType != typeof(AssemblyLoaderEventType) ||
										methodInfo.GetParameters()[1].ParameterType != typeof(AssemblyLoaderContainerContext))
										continue;

									assemblyLoaderEventSinkMethod = (Action<AssemblyLoaderEventType, AssemblyLoaderContainerContext>)(methodInfo.CreateDelegate(typeof(Action<AssemblyLoaderEventType, AssemblyLoaderContainerContext>), null /* static */));

									if ((object)assemblyLoaderEventSinkMethod == null)
										continue;

									if (this.EventSinkMethods.Contains(assemblyLoaderEventSinkMethod))
										throw new NotSupportedException(string.Format("Method '{0}' of type '{1}' has been sunk before. This is likely a defect in the framework - report this to the project team.", methodInfo.Name, methodInfo.DeclaringType.FullName));

									// notify
									//Console.WriteLine("{1}::{0}.", methodInfo.Name, methodInfo.DeclaringType.Name);
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
			ILibraryManager libraryManager;
			PlatformServices platformServices;

			Console.WriteLine("SetUpApplicationDomain");

			platformServices = PlatformServices.Default;

			if ((object)platformServices == null)
				return; //throw new InvalidOperationException(string.Format("Platform services default instance was invalid."));

			libraryManager = platformServices.LibraryManager;

			if ((object)libraryManager == null)
				return; //throw new InvalidOperationException(string.Format("Platform services library manager was invalid."));

			if (!EnvVarEnableAssemblyLoaderSubscriptionMethodExecution)
				return;

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
																								})).Where(a => (object)a != null);

			this.ScanAssemblies(assemblies.ToArray());
		}

		/// <summary>
		/// Private thread-safe method which dismantles an "app domain".
		/// </summary>
		private void TearDownApplicationDomain()
		{
			// notify
			foreach (Action<AssemblyLoaderEventType, AssemblyLoaderContainerContext> eventSinkMethod in this.EventSinkMethods)
				eventSinkMethod(AssemblyLoaderEventType.Shutdown, this);

			// cleanup
			this.EventSinkMethods.Clear();

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