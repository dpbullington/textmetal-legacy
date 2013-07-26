// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// 	This enum defines indicators telling how instances of IStackTraceView
	/// 	should deal with item order in their list.
	/// </summary>
	public enum ErrorListOrderPolicy
	{
		/// <summary>
		/// 	Tells IStackTraceView to order items in the list in the same
		/// 	order they appear in the stack trace.
		/// </summary>
		InitialOrder,

		/// <summary>
		/// 	Tells IStackTraceView to order items in the list in the reverse
		/// 	order they appear in the stack trace. At Test Driven Development time
		/// 	this value is useful to point out the location where a test is expected
		/// 	to fail.
		/// </summary>
		ReverseOrder,
	}

	/// <summary>
	/// 	The interface through which SourceCodeDisplay interacts with the error list.
	/// 
	/// 	Direct implementations are:
	/// 	- ErrorList
	/// </summary>
	public interface IStackTraceView
	{
		#region Properties/Indexers/Events

		event EventHandler SelectedItemChanged;

		bool AutoSelectFirstItem
		{
			get;
			set;
		}

		ErrorListOrderPolicy ListOrderPolicy
		{
			get;
			set;
		}

		ErrorItem SelectedItem
		{
			get;
		}

		string StackTrace
		{
			get;
			set;
		}

		#endregion
	}
}