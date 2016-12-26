/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Diagnostics;

using __OnlyWhen = TextMetal.Middleware.Solder.Primitives.OnlyWhen;
using TextMetal.Middleware.Solder.Injection;

namespace TextMetal.Middleware.Solder
{
	public static class OnlyWhen
	{
		#region Methods/Operators

		[Conditional("DEBUG")]
		[AssemblyLoaderEventSinkMethod]
		public static void OnAssemblyLoaderEvent(AssemblyLoaderEventType assemblyLoaderEventType, AgnosticAppDomain agnosticAppDomain)
		{
#if DEBUG
			/* THIS METHOD SHOULD NOT BE DEFINED IN RELEASE/PRODUCTION BUILDS */
		
			if ((object)agnosticAppDomain == null)
				throw new ArgumentNullException(nameof(agnosticAppDomain));

			switch (assemblyLoaderEventType)
			{
				case AssemblyLoaderEventType.Startup:
				case AssemblyLoaderEventType.Shutdown:
					__OnlyWhen._PROFILE_ThenPrint(assemblyLoaderEventType.ToString());
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(assemblyLoaderEventType), assemblyLoaderEventType, null);
			}
#endif
		}

		#endregion
	}
}