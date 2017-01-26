/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Solder.Extensions
{
	//[Obsolete("Stop using this")]
	public static class SolderFascadeAccessor
	{
		#region Fields/Constants

		private static readonly IDataTypeFascade dataTypeFascade = AssemblyDependencyDomain.Default.DependencyManager.ResolveDependency<IDataTypeFascade>(String.Empty, false);
		private static readonly IReflectionFascade reflectionFascade = AssemblyDependencyDomain.Default.DependencyManager.ResolveDependency<IReflectionFascade>(String.Empty, false);

		#endregion

		#region Properties/Indexers/Events

		public static IDataTypeFascade DataTypeFascade
		{
			get
			{
				return dataTypeFascade;
			}
		}

		public static IReflectionFascade ReflectionFascade
		{
			get
			{
				return reflectionFascade;
			}
		}

		#endregion
	}
}