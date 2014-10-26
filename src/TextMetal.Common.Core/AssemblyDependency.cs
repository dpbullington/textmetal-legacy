/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Solder.DependencyManagement;

[assembly: DependencyRegistration]

namespace TextMetal.Common.Core
{
	[DependencyRegistration]
	public static class AssemblyDependency
	{
		#region Methods/Operators

		[DependencyRegistration]
		public static void ProvokeDependencyRegistration()
		{
			DependencyManager.AppDomainInstance.AddResolution<IDataType>("", SingletonDependencyResolution.LazyConstructorOfType<DataType>());
		}

		#endregion
	}
}