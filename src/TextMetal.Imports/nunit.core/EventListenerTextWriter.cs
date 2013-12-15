// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System.IO;
using System.Text;

namespace NUnit.Core
{
	using System;

	public class EventListenerTextWriter : TextWriter
	{
		#region Constructors/Destructors

		public EventListenerTextWriter(EventListener eventListener, TestOutputType type)
		{
			this.eventListener = eventListener;
			this.type = type;
		}

		#endregion

		#region Fields/Constants

		private EventListener eventListener;
		private TestOutputType type;

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

		public override void Write(char aChar)
		{
			this.eventListener.TestOutput(new TestOutput(aChar.ToString(), this.type));
		}

		public override void Write(string aString)
		{
			this.eventListener.TestOutput(new TestOutput(aString, this.type));
		}

		public override void WriteLine(string aString)
		{
			this.eventListener.TestOutput(new TestOutput(aString + this.NewLine, this.type));
		}

		#endregion
	}

	/// <summary>
	/// This wrapper adds buffering to improve cross-domain performance.
	/// </summary>
	public class BufferedEventListenerTextWriter : TextWriter
	{
		#region Constructors/Destructors

		public BufferedEventListenerTextWriter(EventListener eventListener, TestOutputType type)
		{
			this.eventListener = eventListener;
			this.type = type;
		}

		#endregion

		#region Fields/Constants

		private const int MAX_BUFFER = 1024;
		private EventListener eventListener;
		private StringBuilder sb = new StringBuilder(MAX_BUFFER);
		private TestOutputType type;

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
					TestOutput output = new TestOutput(this.sb.ToString(), this.type);
					this.eventListener.TestOutput(output);
					this.sb.Length = 0;
				}
			}
		}

		public override void Write(char ch)
		{
			lock (this.sb)
			{
				this.sb.Append(ch);
				this.CheckBuffer();
			}
		}

		public override void Write(string str)
		{
			lock (this.sb)
			{
				this.sb.Append(str);
				this.CheckBuffer();
			}
		}

		public override void WriteLine(string str)
		{
			lock (this.sb)
			{
				this.sb.Append(str);
				this.sb.Append(base.NewLine);
				this.CheckBuffer();
			}
		}

		#endregion
	}
}