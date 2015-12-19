/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Diagnostics;

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

		[AssemblyLoaderSubscriberMethod]
		public static void ThisAssemblyDependencyRegistration()
		{
			AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.AddResolution<IDataTypeFascade>(string.Empty, new SingletonDependencyResolution(new ConstructorDependencyResolution<DataTypeFascade>()));
			AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.AddResolution<IReflectionFascade>(string.Empty, new SingletonDependencyResolution(ActivatorDependencyResolution.OfType<IReflectionFascade, IDataTypeFascade>()));
			AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.AddResolution<IAppConfigFascade>(string.Empty, new SingletonDependencyResolution(ActivatorDependencyResolution.OfType<IAppConfigFascade, string, IDataTypeFascade>()));
			AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.AddResolution<IAdoNetLiteFascade>(string.Empty, new SingletonDependencyResolution(ActivatorDependencyResolution.OfType<IAdoNetLiteFascade, IReflectionFascade>()));
		}

		#endregion
	}
}