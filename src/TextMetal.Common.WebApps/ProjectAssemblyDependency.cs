/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Core;
using TextMetal.Common.Solder.DependencyManagement;
using TextMetal.Common.WebApps.Core;

[assembly: DependencyRegistration]

namespace TextMetal.Common.WebApps
{
	[DependencyRegistration]
	public static class ProjectAssemblyDependency
	{
		#region Methods/Operators

		[DependencyRegistration]
		public static void ProvokeProjectDependencyRegistration()
		{
			DependencyManager.AppDomainInstance.AddResolution<IApplicationStorage>(string.Empty, new SingletonDependencyResolution(new ConstructorDependencyResolution<HttpSessionApplicationStorage>()));
			DependencyManager.AppDomainInstance.AddResolution<IApplicationStorage>(ProjectConstants.DEP_SEL_STANDARD_WEB_SESSION, new SingletonDependencyResolution(new ConstructorDependencyResolution<HttpSessionApplicationStorage>()));
			DependencyManager.AppDomainInstance.AddResolution<IApplicationStorage>(ProjectConstants.DEP_SEL_STICKY_WEB_SESSION, new SingletonDependencyResolution(new ConstructorDependencyResolution<HttpCookieApplicationStorage>()));
		}

		#endregion
	}
}