/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.WinForms;
using TextMetal.HostImpl.WindowsTool.Forms;

namespace TextMetal.HostImpl.WindowsTool
{
	/// <summary>
	/// Entry point class for the application.
	/// </summary>
	internal class Program : WindowsApplication<MainForm, SplashForm>
	{
		#region Constructors/Destructors

		public Program()
		{
		}

		#endregion

		#region Methods/Operators

		protected override IDictionary<string, ArgumentSpec> GetArgumentMap()
		{
			IDictionary<string, ArgumentSpec> argumentMap;

			argumentMap = new Dictionary<string, ArgumentSpec>();
			/*argumentMap.Add("test", new ArgumentSlot()
			{
				Required = true,
				Bounded = true
			});*/

			return argumentMap;
		}

		/// <summary>
		/// The entry point method for this application.
		/// </summary>
		/// <param name="args"> The command line arguments passed from the executing environment. </param>
		/// <returns> The resulting exit code. </returns>
		[STAThread]
		public static int Main(string[] args)
		{
			using (Program program = new Program())
				return program.EntryPoint(args);
		}

		#endregion
	}
}