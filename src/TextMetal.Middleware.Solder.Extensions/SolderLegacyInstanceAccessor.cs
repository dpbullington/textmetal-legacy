/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Solder.Extensions
{
	[Obsolete("Stop using this")]
	public static class SolderLegacyInstanceAccessor
	{
		#region Fields/Constants

		private static readonly IDataTypeFascade dataTypeFascadeLegacyInstance = AssemblyDependencyDomain.Default.DependencyManager.ResolveDependency<IDataTypeFascade>(String.Empty, true);
		private static readonly IReflectionFascade reflectionFascadeLegacyInstance = AssemblyDependencyDomain.Default.DependencyManager.ResolveDependency<IReflectionFascade>(String.Empty, true);

		#endregion

		#region Properties/Indexers/Events

		public static IDataTypeFascade DataTypeFascadeLegacyInstance
		{
			get
			{
				return dataTypeFascadeLegacyInstance;
			}
		}

		public static IReflectionFascade ReflectionFascadeLegacyInstance
		{
			get
			{
				return reflectionFascadeLegacyInstance;
			}
		}

		#endregion
	}
}