/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
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
		#region Fields/Constants

		private static readonly IAdoNetBufferingFascade adoNetBufferingLegacyInstance = AssemblyDependencyDomain.Default.DependencyManager.ResolveDependency<IAdoNetBufferingFascade>(String.Empty, true);
		private static readonly IAdoNetStreamingFascade adoNetStreamingFascade = AssemblyDependencyDomain.Default.DependencyManager.ResolveDependency<IAdoNetStreamingFascade>(String.Empty, true);

		#endregion

		#region Properties/Indexers/Events

		public static IAdoNetBufferingFascade AdoNetBufferingLegacyInstance
		{
			get
			{
				return adoNetBufferingLegacyInstance;
			}
		}

		public static IAdoNetStreamingFascade AdoNetStreamingFascade
		{
			get
			{
				return adoNetStreamingFascade;
			}
		}

		#endregion
	}
}