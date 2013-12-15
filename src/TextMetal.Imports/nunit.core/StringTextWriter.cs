// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System.IO;
using System.Text;

namespace NUnit.Core
{
	// TODO: This class is not currently being used. Review to
	// see if we will use it again, otherwise drop it.

	#region StringTextWriter

	/// <summary>
	/// Use this wrapper to ensure that only strings get passed accross the AppDomain
	/// boundary.  Otherwise tests will break when non-remotable objects are passed to
	/// Console.Write/WriteLine.
	/// </summary>
	public class StringTextWriter : TextWriter
	{
		#region Constructors/Destructors

		public StringTextWriter(TextWriter aTextWriter)
		{
			this.theTextWriter = aTextWriter;
		}

		#endregion

		#region Fields/Constants

		protected TextWriter theTextWriter;

		#endregion

		#region Properties/Indexers/Events

		public override Encoding Encoding
		{
			get
			{
				return this.theTextWriter.Encoding;
			}
		}

		#endregion

		#region Methods/Operators

		public override void Close()
		{
			this.Flush();
			this.theTextWriter.Close();
		}

		public override void Flush()
		{
			this.theTextWriter.Flush();
		}

		public override void Write(char aChar)
		{
			this.theTextWriter.Write(aChar);
		}

		public override void Write(string aString)
		{
			this.theTextWriter.Write(aString);
		}

		public override void WriteLine(string aString)
		{
			this.theTextWriter.WriteLine(aString);
		}

		#endregion
	}

	#endregion

	#region BufferedStringTextWriter

	/// <summary>
	/// This wrapper derives from StringTextWriter and adds buffering
	/// to improve cross-domain performance. The buffer is flushed whenever
	/// it reaches or exceeds a maximum size or when Flush is called.
	/// </summary>
	public class BufferedStringTextWriter : StringTextWriter
	{
		#region Constructors/Destructors

		public BufferedStringTextWriter(TextWriter aTextWriter)
			: base(aTextWriter)
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly int MAX_BUFFER = 1000;
		private StringBuilder sb = new StringBuilder(MAX_BUFFER);

		#endregion

		#region Methods/Operators

		private void CheckBuffer()
		{
			if (this.sb.Length >= MAX_BUFFER)
				this.Flush();
		}

		public override void Flush()
		{
			if (this.sb.Length > 0)
			{
				lock (this.sb)
				{
					this.theTextWriter.Write(this.sb.ToString());
					this.sb.Length = 0;
				}
			}

			this.theTextWriter.Flush();
		}

		public override void Write(char aChar)
		{
			lock (this.sb)
			{
				this.sb.Append(aChar);
				this.CheckBuffer();
			}
		}

		public override void Write(string aString)
		{
			lock (this.sb)
			{
				this.sb.Append(aString);
				this.CheckBuffer();
			}
		}

		public override void WriteLine(string aString)
		{
			lock (this.sb)
			{
				this.sb.Append(aString);
				this.sb.Append('\n');
				this.CheckBuffer();
			}
		}

		#endregion
	}

	#endregion
}