/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Diagnostics;

namespace TextMetal.Middleware.Solder.Primitives
{
	public static class OnlyWhen
	{
		#region Methods/Operators

		[Conditional("DEBUG")]
		public static void _PROFILE_ThenPrint(string message)
		{
#if DEBUG
			/* THIS METHOD SHOULD NOT BE DEFINED IN RELEASE/PRODUCTION BUILDS */

			ConsoleColor oldConsoleColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(message);
			Console.ForegroundColor = oldConsoleColor;
#endif
		}

		#endregion
	}
}