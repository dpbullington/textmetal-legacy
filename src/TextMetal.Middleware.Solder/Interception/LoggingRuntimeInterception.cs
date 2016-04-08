/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.Interception
{
	public class LoggingRuntimeInterception : RuntimeInterception
	{
		#region Constructors/Destructors

		public LoggingRuntimeInterception()
		{
		}

		#endregion

		#region Methods/Operators

		protected override void OnAfterInvoke(bool proceedWithInvocation, IRuntimeInvocation runtimeInvocation, ref Exception thrownException)
		{
			var oldConsoleColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("after invoke: {0}", runtimeInvocation.TargetMethod.ToString());
			Console.ForegroundColor = oldConsoleColor;
		}

		protected override void OnBeforeInvoke(IRuntimeInvocation runtimeInvocation, out bool proceedWithInvocation)
		{
			var oldConsoleColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine("before invoke: {0}", runtimeInvocation.TargetMethod.ToString());
			Console.ForegroundColor = oldConsoleColor;
			proceedWithInvocation = true;
		}

		protected override void OnMagicalSpellInvoke(IRuntimeInvocation runtimeInvocation)
		{
			var oldConsoleColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("magic invoke: {0}", runtimeInvocation.TargetMethod.ToString());
			Console.ForegroundColor = oldConsoleColor;

			base.OnMagicalSpellInvoke(runtimeInvocation);
		}

		#endregion
	}
}