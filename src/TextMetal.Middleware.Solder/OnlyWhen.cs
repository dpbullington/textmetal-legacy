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

		[AssemblyLoaderEventSinkMethod]
		public static void ThisAssemblyDependencyRegistration(IDependencyManager dependencyManager)
		{
			if ((object)dependencyManager == null)
				throw new ArgumentNullException(nameof(dependencyManager));

			dependencyManager.AddResolution<IDataTypeFascade>(string.Empty, false, new SingletonWrapperDependencyResolution(TransientDefaultConstructorDependencyResolution.Create<DataTypeFascade>()));
			dependencyManager.AddResolution<IReflectionFascade>(string.Empty, false, new SingletonWrapperDependencyResolution(TransientActivatorAutoWiringDependencyResolution.OfType<IReflectionFascade, IDataTypeFascade>()));
			dependencyManager.AddResolution<IAppConfigFascade>(string.Empty, false, new SingletonWrapperDependencyResolution(TransientActivatorAutoWiringDependencyResolution.OfType<IAppConfigFascade, string, IDataTypeFascade>()));
			dependencyManager.AddResolution<IAdoNetLiteFascade>(string.Empty, false, new SingletonWrapperDependencyResolution(TransientActivatorAutoWiringDependencyResolution.OfType<IAdoNetLiteFascade, IReflectionFascade>()));
		}

		#endregion
	}
}