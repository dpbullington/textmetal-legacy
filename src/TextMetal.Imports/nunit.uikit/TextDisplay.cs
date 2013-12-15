// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

using NUnit.Core;
using NUnit.Util;

namespace NUnit.UiKit
{
	/// <summary>
	/// The TextDisplay interface is implemented by object - generally
	/// controls - that display text.
	/// </summary>
	public interface TextDisplay : TestObserver
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// The output types handled by this display
		/// </summary>
		TextDisplayContent Content
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Clears the display
		/// </summary>
		void Clear();

		/// <summary>
		/// Gets the current text - used mainly for testing
		/// </summary>
		string GetText();

		/// <summary>
		/// Appends text to the display
		/// </summary>
		/// <param name="text"> The text to append </param>
		void Write(string text);

		void Write(TestOutput output);

		/// <summary>
		/// Appends text to the display followed by a newline
		/// </summary>
		/// <param name="text"> The text to append </param>
		void WriteLine(string text);

		#endregion
	}
}