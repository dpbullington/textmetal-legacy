/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Common.Fascades.AdoNet;
using TextMetal.Middleware.Common.Fascades.Utilities;
using TextMetal.Middleware.Solder.IoC;

[assembly: DependencyRegistration]

namespace TextMetal.Middleware.Common
{
	[DependencyRegistration]
	public static class ProjectAssemblyDependency
	{
		#region Methods/Operators

		[DependencyRegistration]
		public static void ProvokeProjectDependencyRegistration()
		{
			//DependencyManager.AppDomainInstance.AddResolution<IAdoNetFascade>(string.Empty, new ConstructorDependencyResolution<AdoNetFascade>());
			//DependencyManager.AppDomainInstance.AddResolution<IAppConfigFascade>(string.Empty, new ConstructorDependencyResolution<AppConfigFascade>());
			//DependencyManager.AppDomainInstance.AddResolution<IAssemblyInformationFascade>(string.Empty, new ConstructorDependencyResolution<AssemblyInformationFascade>());
			//DependencyManager.AppDomainInstance.AddResolution<IDataTypeFascade>(string.Empty, new ConstructorDependencyResolution<DataTypeFascade>());
			//DependencyManager.AppDomainInstance.AddResolution<IReflectionFascade>(string.Empty, new ConstructorDependencyResolution<ReflectionFascade>());
		}

		#endregion
	}
}