/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Diagnostics;

namespace TextMetal.Middleware.Common
{
	public static class OnlyWhen
	{
		#region Methods/Operators

		[Conditional("DEBUG")]
		public static void _DEBUG_ThenPrint(string message)
		{
			/* THIS METHOD SHOULD NOT BE DEFINED IN RELEASE/PRODUCTION BUILDS */
			//Debug.WriteLine(message);
		}

		#endregion
	}
}