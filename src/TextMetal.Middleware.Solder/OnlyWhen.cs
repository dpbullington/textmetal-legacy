/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Diagnostics;

using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Framework.Configuration;

using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Runtime;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Solder
{
	public static class OnlyWhen
	{
		#region Methods/Operators

		[Conditional("PROFILE")]
		public static void _PROFILE_ThenPrint(string message)
		{
			/* THIS METHOD SHOULD NOT BE DEFINED IN RELEASE/PRODUCTION BUILDS */
			Debug.WriteLine(message);
		}

		[AssemblyLoaderEventSinkMethod]
		public static void ThisAssemblyDependencyRegistration(AssemblyLoaderEventType assemblyLoaderEventType, AssemblyLoaderContainerContext assemblyLoaderContainerContext)
		{
			const string APP_CONFIG_FILE_NAME = "appconfig.json";

			if ((object)assemblyLoaderContainerContext == null)
				throw new ArgumentNullException(nameof(assemblyLoaderContainerContext));

			switch (assemblyLoaderEventType)
			{
				case AssemblyLoaderEventType.Startup:
					assemblyLoaderContainerContext.DependencyManager.AddResolution<PlatformServices>(string.Empty, false, new SingletonWrapperDependencyResolution<PlatformServices>(new InstanceDependencyResolution<PlatformServices>(PlatformServices.Default)));
					assemblyLoaderContainerContext.DependencyManager.AddResolution<IConfigurationRoot>(string.Empty, false, new SingletonWrapperDependencyResolution<IConfigurationRoot>(new TransientFactoryMethodDependencyResolution<IConfigurationRoot>(() => AppConfigFascade.LoadAppConfigFile(APP_CONFIG_FILE_NAME))));

					assemblyLoaderContainerContext.DependencyManager.AddResolution<IDataTypeFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IDataTypeFascade>(new TransientDefaultConstructorDependencyResolution<DataTypeFascade>()));
					assemblyLoaderContainerContext.DependencyManager.AddResolution<IReflectionFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IReflectionFascade>(new TransientActivatorAutoWiringDependencyResolution<ReflectionFascade>()));
					assemblyLoaderContainerContext.DependencyManager.AddResolution<IAppConfigFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IAppConfigFascade>(new TransientActivatorAutoWiringDependencyResolution<AppConfigFascade>()));
					assemblyLoaderContainerContext.DependencyManager.AddResolution<IAdoNetLiteFascade>(string.Empty, false, new SingletonWrapperDependencyResolution<IAdoNetLiteFascade>(new TransientActivatorAutoWiringDependencyResolution<AdoNetLiteFascade>()));
					break;
				case AssemblyLoaderEventType.Shutdown:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(assemblyLoaderEventType), assemblyLoaderEventType, null);
			}
		}

		#endregion
	}
}