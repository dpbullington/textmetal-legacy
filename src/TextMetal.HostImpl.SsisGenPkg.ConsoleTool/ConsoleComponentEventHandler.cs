/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using Microsoft.SqlServer.Dts.Runtime;

namespace TextMetal.HostImpl.SsisGenPkg.ConsoleTool
{
	public sealed class ConsoleComponentEventHandler : IDTSComponentEvents
	{
		#region Methods/Operators

		public void FireBreakpointHit(BreakpointTarget breakpointTarget)
		{
		}

		public void FireCustomEvent(string eventName, string eventText, ref object[] arguments, string subComponent, ref bool fireAgain)
		{
		}

		public bool FireError(int errorCode, string subComponent, string description, string helpFile, int helpContext)
		{
			Console.WriteLine("[Error] {0}: {1}", subComponent, description);
			return true;
		}

		public void FireInformation(int informationCode, string subComponent, string description, string helpFile, int helpContext, ref bool fireAgain)
		{
		}

		public void FireProgress(string progressDescription, int percentComplete, int progressCountLow, int progressCountHigh, string subComponent, ref bool fireAgain)
		{
		}

		public bool FireQueryCancel()
		{
			return false;
		}

		public void FireWarning(int warningCode, string subComponent, string description, string helpFile, int helpContext)
		{
		}

		#endregion
	}
}