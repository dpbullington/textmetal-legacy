/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Diagnostics;

using TextMetal.Middleware.Solder.Injection;

using __OnlyWhen = TextMetal.Middleware.Solder.Primitives.OnlyWhen;

namespace TextMetal.Middleware.Solder
{
	public static class OnlyWhen
	{
		#region Methods/Operators

		[Conditional("DEBUG")]
		[DependencyMagicMethod]
		public static void OnDependencyMagic(IDependencyManager dependencyManager)
		{
#if DEBUG
			/* THIS METHOD SHOULD NOT BE DEFINED IN RELEASE/PRODUCTION BUILDS */

			if ((object)dependencyManager == null)
				throw new ArgumentNullException(nameof(dependencyManager));

			__OnlyWhen._PROFILE_ThenPrint(string.Format("OnDependencyMagic"));
#endif
		}

		#endregion
	}
}