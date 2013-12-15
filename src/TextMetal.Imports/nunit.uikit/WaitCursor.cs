// ****************************************************************
// Copyright 2002-2003, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Windows.Forms;

namespace CP.Windows.Forms
{
	/// <summary>
	/// Utility class used to display a wait cursor
	/// while a long operation takes place and
	/// guarantee that it will be removed on exit.
	/// Use as follows:
	/// using ( new WaitCursor() )
	/// {
	/// // Long running operation goes here
	/// }
	/// </summary>
	public class WaitCursor : IDisposable
	{
		#region Constructors/Destructors

		public WaitCursor()
		{
			this.control = null;
			this.cursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
		}

		public WaitCursor(Control control)
		{
			this.control = control;
			this.cursor = control.Cursor;
			control.Cursor = Cursors.WaitCursor;
		}

		#endregion

		#region Fields/Constants

		private Control control;
		private Cursor cursor;

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
			if (this.control != null)
				this.control.Cursor = this.cursor;
			else
				Cursor.Current = this.cursor;
		}

		#endregion
	}
}