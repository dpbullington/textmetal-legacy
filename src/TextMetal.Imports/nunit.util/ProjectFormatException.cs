// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;

namespace NUnit.Util
{
	/// <summary>
	/// Exception raised when loading a project file with
	/// an invalid format.
	/// </summary>
	public class ProjectFormatException : ApplicationException
	{
		#region Constructors/Destructors

		public ProjectFormatException()
			: base()
		{
		}

		public ProjectFormatException(string message)
			: base(message)
		{
		}

		public ProjectFormatException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public ProjectFormatException(string message, int lineNumber, int linePosition)
			: base(message)
		{
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		#endregion

		#region Fields/Constants

		private int lineNumber;

		private int linePosition;

		#endregion

		#region Properties/Indexers/Events

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		#endregion
	}
}