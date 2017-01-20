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
		[DependencyMagicMethod]
		public static void OnAssemblyLoaderEvent(AssemblyDependencyDomain assemblyDependencyDomain)
		{
#if DEBUG
			/* THIS METHOD SHOULD NOT BE DEFINED IN RELEASE/PRODUCTION BUILDS */
		
			if ((object)assemblyDependencyDomain == null)
				throw new ArgumentNullException(nameof(assemblyDependencyDomain));

			__OnlyWhen._PROFILE_ThenPrint(string.Format("OnAssemblyLoaderEvent"));
#endif
		}

		#endregion
	}
}