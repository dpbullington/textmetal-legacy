// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;

namespace NUnit.Core
{
	public class Logger
	{
		#region Constructors/Destructors

		public Logger(string name)
		{
			this.fullname = this.name = name;
			int index = this.fullname.LastIndexOf('.');
			if (index >= 0)
				this.name = this.fullname.Substring(index + 1);
		}

		#endregion

		#region Fields/Constants

		private string fullname;
		private string name;

		#endregion

		#region Methods/Operators

		public void Debug(string message)
		{
			this.Log(InternalTraceLevel.Verbose, message);
		}

		public void Debug(string message, params object[] args)
		{
			this.Log(InternalTraceLevel.Verbose, message, args);
		}

		public void Error(string message)
		{
			this.Log(InternalTraceLevel.Error, message);
		}

		public void Error(string message, params object[] args)
		{
			this.Log(InternalTraceLevel.Error, message, args);
		}

		public void Error(string message, Exception ex)
		{
			if (InternalTrace.Level >= InternalTraceLevel.Error)
				InternalTrace.Log(InternalTraceLevel.Error, message, this.name, ex);
		}

		public void Info(string message)
		{
			this.Log(InternalTraceLevel.Info, message);
		}

		public void Info(string message, params object[] args)
		{
			this.Log(InternalTraceLevel.Info, message, args);
		}

		public void Log(InternalTraceLevel level, string message)
		{
			if (InternalTrace.Level >= level)
				InternalTrace.Log(level, message, this.name);
		}

		private void Log(InternalTraceLevel level, string format, params object[] args)
		{
			if (InternalTrace.Level >= level)
				this.Log(level, string.Format(format, args));
		}

		public void Warning(string message)
		{
			this.Log(InternalTraceLevel.Warning, message);
		}

		public void Warning(string message, params object[] args)
		{
			this.Log(InternalTraceLevel.Warning, message, args);
		}

		#endregion
	}
}