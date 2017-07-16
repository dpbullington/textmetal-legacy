/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Datazoid.Primitives;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Datazoid.Extensions
{
	[Obsolete("Stop using this")]
	public static class DatazoidLegacyInstanceAccessor
	{
		#region Fields/Constants

		//private static readonly IAdoNetBufferingFascade adoNetBufferingLegacyInstance = AssemblyDomain.Default.DependencyManager.ResolveDependency<IAdoNetBufferingFascade>(String.Empty, false);
		//private static readonly IAdoNetStreamingFascade adoNetStreamingFascade = AssemblyDomain.Default.DependencyManager.ResolveDependency<IAdoNetStreamingFascade>(String.Empty, false);

		#endregion

		#region Properties/Indexers/Events

		public static IAdoNetBufferingFascade AdoNetBufferingLegacyInstance
		{
			get
			{
				return new AdoNetBufferingFascade(new ReflectionFascade(new DataTypeFascade()), new DataTypeFascade());
				//return adoNetBufferingLegacyInstance;
			}
		}

		public static IAdoNetStreamingFascade AdoNetStreamingFascade
		{
			get
			{
				return new AdoNetStreamingFascade(new ReflectionFascade(new DataTypeFascade()), new DataTypeFascade());
				//return adoNetStreamingFascade;
			}
		}

		#endregion
	}
}