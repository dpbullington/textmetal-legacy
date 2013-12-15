// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// Summary description for TestResultItem.
	/// </summary>
	public class TestResultItem
	{
		#region Constructors/Destructors

		public TestResultItem(TestResult result)
		{
			this.testName = result.Test.TestName.FullName;
			this.message = result.Message;
			this.stackTrace = result.StackTrace;

			if (result.Test.IsSuite && result.FailureSite == FailureSite.SetUp)
				this.testName += " (TestFixtureSetUp)";
		}

		public TestResultItem(string testName, string message, string stackTrace)
		{
			this.testName = testName;
			this.message = message;
			this.stackTrace = stackTrace;
		}

		#endregion

		#region Fields/Constants

		private string message;
		private string stackTrace;
		private string testName;

		#endregion

		#region Properties/Indexers/Events

		public string StackTrace
		{
			get
			{
				return this.stackTrace == null ? null : StackTraceFilter.Filter(this.stackTrace);

				//string trace = "No stack trace is available";
				//if(stackTrace != null)
				//    trace = StackTraceFilter.Filter(stackTrace);

				//return trace;
			}
		}

		#endregion

		#region Methods/Operators

		public string GetMessage()
		{
			return String.Format("{0}:{1}{2}", this.testName, Environment.NewLine, this.message);
		}

		public string GetToolTipMessage() //NRG 05/28/03 - Substitute spaces for tab characters
		{
			return (this.ReplaceTabs(this.GetMessage(), 8)); // Change each tab to 8 space characters
		}

		public string ReplaceTabs(string strOriginal, int nSpaces) //NRG 05/28/03
		{
			string strSpaces = string.Empty;
			strSpaces = strSpaces.PadRight(nSpaces, ' ');
			return (strOriginal.Replace("\t", strSpaces));
		}

		public override string ToString()
		{
			if (this.message.Length > 64000)
				return string.Format("{0}:{1}{2}", this.testName, Environment.NewLine, this.message.Substring(0, 64000));

			return this.GetMessage();
		}

		#endregion
	}
}