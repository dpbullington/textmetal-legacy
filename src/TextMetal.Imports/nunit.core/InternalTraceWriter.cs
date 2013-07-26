// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NUnit.Core
{
	/// <summary>
	/// 	A trace listener that writes to a separate file per domain
	/// 	and process using it.
	/// </summary>
	public class InternalTraceWriter : TextWriter
	{
		#region Constructors/Destructors

		public InternalTraceWriter(string logName)
		{
			int pId = Process.GetCurrentProcess().Id;
			string domainName = AppDomain.CurrentDomain.FriendlyName;

			string fileName = logName
				.Replace("%p", pId.ToString())
				.Replace("%a", domainName);

			string logDirectory = NUnitConfiguration.LogDirectory;
			if (!Directory.Exists(logDirectory))
				Directory.CreateDirectory(logDirectory);

			string logPath = Path.Combine(logDirectory, fileName);
			this.writer = new StreamWriter(logPath, true);
			this.writer.AutoFlush = true;
		}

		#endregion

		#region Fields/Constants

		private StreamWriter writer;

		#endregion

		#region Properties/Indexers/Events

		public override Encoding Encoding
		{
			get
			{
				return this.writer.Encoding;
			}
		}

		#endregion

		#region Methods/Operators

		public override void Close()
		{
			if (this.writer != null)
			{
				this.writer.Flush();
				this.writer.Close();
				this.writer = null;
			}
		}

		public override void Flush()
		{
			if (this.writer != null)
				this.writer.Flush();
		}

		public override void Write(char value)
		{
			this.writer.Write(value);
		}

		public override void Write(string value)
		{
			base.Write(value);
		}

		public override void WriteLine(string value)
		{
			this.writer.WriteLine(value);
		}

		#endregion
	}
}