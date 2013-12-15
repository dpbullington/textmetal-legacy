/*
	(The unified framework is) copyright ©2012 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

/*
	****************************************************************
	NUnit code:
	Copyright © 2002-2012 Charlie Poole
	Copyright © 2002-2004 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov
	Copyright © 2000-2002 Philip A. Craig
	This is free software licensed under the NUnit license (zlib/libpng license compatable).
	You may obtain a copy of the license at http://nunit.org
	****************************************************************
*/

using System;

namespace TestingFramework.Runner.Console
{
	internal class Program
	{
		#region Methods/Operators

		[STAThread]
		private static int Main(string[] args)
		{
			return NUnit.ConsoleRunner.Runner.Main(args);
		}

		#endregion
	}
}