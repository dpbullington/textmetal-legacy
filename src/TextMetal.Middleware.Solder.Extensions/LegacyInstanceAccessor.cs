/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.Injection;

namespace TextMetal.Middleware.Solder.Utilities
{
	[Obsolete("Stop using this")]
	public static class LegacyInstanceAccessor
	{
		#region Properties/Indexers/Events

		[Obsolete("Stop using this")]
		public static IAdoNetLiteFascade AdoNetLiteLegacyInstance
		{
			get
			{
				return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IAdoNetLiteFascade>(String.Empty, true);
			}
		}

		[Obsolete("Stop using this")]
		public static IDataTypeFascade DataTypeFascadeLegacyInstance
		{
			get
			{
				return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IDataTypeFascade>(String.Empty, true);
			}
		}

		[Obsolete("Stop using this")]
		public static IReflectionFascade ReflectionFascadeLegacyInstance
		{
			get
			{
				return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IReflectionFascade>(String.Empty, true);
			}
		}

		#endregion
	}
}