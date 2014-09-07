/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
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
		#region Methods/Operators

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

		protected override IDictionary<string, ArgumentSlot> GetArgumentMap()
		{
			IDictionary<string, ArgumentSlot> argumentMap;

			argumentMap = new Dictionary<string, ArgumentSlot>();
			/*argumentMap.Add("test", new ArgumentSlot()
			{
				Required = true,
				Bounded = true
			});*/

			return argumentMap;
		}

		#endregion
	}
}