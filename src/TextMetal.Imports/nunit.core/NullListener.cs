// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;

namespace NUnit.Core
{
	/// <summary>
	/// Summary description for NullListener.
	/// </summary>
	[Serializable]
	public class NullListener : EventListener
	{
		#region Properties/Indexers/Events

		public static EventListener NULL
		{
			get
			{
				return new NullListener();
			}
		}

		#endregion

		#region Methods/Operators

		public void RunFinished(TestResult result)
		{
		}

		public void RunFinished(Exception exception)
		{
		}

		public void RunStarted(string name, int testCount)
		{
		}

		public void SuiteFinished(TestResult result)
		{
		}

		public void SuiteStarted(TestName testName)
		{
		}

		public void TestFinished(TestResult result)
		{
		}

		public void TestOutput(TestOutput testOutput)
		{
		}

		public void TestStarted(TestName testName)
		{
		}

		public void UnhandledException(Exception exception)
		{
		}

		#endregion
	}
}