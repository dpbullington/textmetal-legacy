// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Runtime.InteropServices;

namespace NUnit.UiKit
{
	/// <summary>
	/// 	Summary description for GuiAttachedConsole.
	/// </summary>
	public class GuiAttachedConsole
	{
		#region Constructors/Destructors

		public GuiAttachedConsole()
		{
			AllocConsole();
		}

		#endregion

		#region Methods/Operators

		[DllImport("Kernel32.dll")]
		private static extern bool AllocConsole();

		[DllImport("Kernel32.dll")]
		private static extern bool FreeConsole();

		public void Close()
		{
			FreeConsole();
		}

		#endregion
	}
}