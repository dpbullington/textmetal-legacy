/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.Data.Common;

using TextMetal.Middleware.Datazoid.Primitives;
using TextMetal.Middleware.Datazoid.UoW;
using TextMetal.Middleware.Solder.Injection;

namespace TextMetal.Middleware.Datazoid.Extensions
{
	[Obsolete("Stop using this")]
	public static class DatazoidLegacyInstanceAccessor
	{
		#region Properties/Indexers/Events

		[Obsolete("Stop using this")]
		public static IAdoNetBufferingFascade AdoNetBufferingLegacyInstance
		{
			get
			{
				return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IAdoNetBufferingFascade>(String.Empty, true);
			}
		}

		[Obsolete("Stop using this")]
		public static IAdoNetStreamingFascade AdoNetStreamingFascade
		{
			get
			{
				return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IAdoNetStreamingFascade>(String.Empty, true);
			}
		}

		#endregion
	}
}