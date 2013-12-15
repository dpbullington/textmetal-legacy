// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System.IO;
using System.Text;

namespace NUnit.Util
{
	using System;

	/// <summary>
	/// Class used for receiving console output from the running test and displaying it.
	/// </summary>
	public class ConsoleWriter : TextWriter
	{
		#region Constructors/Destructors

		public ConsoleWriter(TextWriter console)
		{
			this.console = console;
		}

		#endregion

		#region Fields/Constants

		private TextWriter console;

		#endregion

		#region Properties/Indexers/Events

		public override Encoding Encoding
		{
			get
			{
				return Encoding.Default;
			}
		}

		#endregion

		#region Methods/Operators

		public override void Close()
		{
			//console.Close ();
		}

		public override void Flush()
		{
			this.console.Flush();
		}

		public override Object InitializeLifetimeService()
		{
			return null;
		}

		public override void Write(char c)
		{
			this.console.Write(c);
		}

		public override void Write(String s)
		{
			this.console.Write(s);
		}

		public override void WriteLine(string s)
		{
			this.console.WriteLine(s);
		}

		#endregion
	}
}