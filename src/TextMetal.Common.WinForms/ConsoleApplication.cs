/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.WinForms
{
	public abstract class ConsoleApplication : ExecutableApplication
	{
		#region Constructors/Destructors

		protected ConsoleApplication()
		{
		}

		#endregion

		#region Methods/Operators

		protected override void DisplayExceptionMessage(string exceptionMessage)
		{
			ConsoleColor oldConsoleColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(exceptionMessage);
			Console.ForegroundColor = oldConsoleColor;

			Console.WriteLine("The operation failed to complete.");
		}

		#endregion
	}
}