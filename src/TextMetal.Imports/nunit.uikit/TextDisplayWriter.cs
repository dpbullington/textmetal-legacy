// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.IO;
using System.Text;

using NUnit.Util;

namespace NUnit.UiKit
{
	/// <summary>
	/// Summary description for TextDisplayWriter.
	/// </summary>
	public class TextDisplayWriter : TextWriter, TestObserver
	{
		#region Constructors/Destructors

		public TextDisplayWriter(TextDisplay textDisplay)
		{
			this.textDisplay = textDisplay;
		}

		#endregion

		#region Fields/Constants

		private TextDisplay textDisplay;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The encoding in use for this TextWriter.
		/// </summary>
		public override Encoding Encoding
		{
			get
			{
				return Encoding.Default;
			}
		}

		#endregion

		#region Methods/Operators

		public void Clear()
		{
			this.textDisplay.Clear();
		}

		private void OnTestOutput(object sender, TestEventArgs args)
		{
		}

		public void Subscribe(ITestEvents events)
		{
			events.TestOutput += new TestEventHandler(this.OnTestOutput);
		}

		/// <summary>
		/// Write a single char
		/// </summary>
		/// <param name="c"> The char to write </param>
		public override void Write(char c)
		{
			Write(c.ToString());
		}

		/// <summary>
		/// Write a string
		/// </summary>
		/// <param name="s"> The string to write </param>
		public override void Write(String s)
		{
			this.textDisplay.Write(s);
		}

		/// <summary>
		/// Write a string followed by a newline.
		/// </summary>
		/// <param name="s"> The string to write </param>
		public override void WriteLine(string s)
		{
			this.Write(s + Environment.NewLine);
		}

		#endregion
	}
}