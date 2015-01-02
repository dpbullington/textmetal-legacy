/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Solder.DependencyManagement;

[assembly: DependencyRegistration]

namespace TextMetal.Common.Core
{
	[DependencyRegistration]
	public static class ProjectAssemblyDependency
	{
		#region Methods/Operators

		[DependencyRegistration]
		public static void ProvokeProjectDependencyRegistration()
		{
			DependencyManager.AppDomainInstance.AddResolution<IDataType>("", SingletonDependencyResolution.LazyConstructorOfType<DataType>());
		}

		#endregion
	}
}