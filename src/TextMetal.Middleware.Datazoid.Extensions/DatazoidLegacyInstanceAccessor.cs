/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Datazoid.Primitives;
using TextMetal.Middleware.Solder.Injection;

namespace TextMetal.Middleware.Datazoid.Extensions
{
	[Obsolete("Stop using this")]
	public static class DatazoidLegacyInstanceAccessor
	{
		#region Properties/Indexers/Events

		[Obsolete("Stop using this")]
		public static IAdoNetYieldingFascade AdoNetYieldingFascade
		{
			get
			{
				return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IAdoNetYieldingFascade>(String.Empty, true);
			}
		}

		#endregion
	}
}