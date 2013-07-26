// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;

namespace NUnit.Core
{
	/// <summary>
	/// 	QueuingEventListener uses an EventQueue to store any
	/// 	events received on its EventListener interface.
	/// </summary>
	public class QueuingEventListener : EventListener
	{
		#region Fields/Constants

		private EventQueue events = new EventQueue();

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	The EvenQueue created and filled by this listener
		/// </summary>
		public EventQueue Events
		{
			get
			{
				return this.events;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Run finished successfully
		/// </summary>
		/// <param name="results"> Array of test results </param>
		public void RunFinished(TestResult result)
		{
			this.events.Enqueue(new RunFinishedEvent(result));
		}

		/// <summary>
		/// 	Run was terminated due to an exception
		/// </summary>
		/// <param name="exception"> Exception that was thrown </param>
		public void RunFinished(Exception exception)
		{
			this.events.Enqueue(new RunFinishedEvent(exception));
		}

		/// <summary>
		/// 	Run is starting
		/// </summary>
		/// <param name="tests"> Array of tests to be run </param>
		public void RunStarted(string name, int testCount)
		{
			this.events.Enqueue(new RunStartedEvent(name, testCount));
		}

		/// <summary>
		/// 	A suite finished
		/// </summary>
		/// <param name="result"> Result of the suite </param>
		public void SuiteFinished(TestResult result)
		{
			this.events.Enqueue(new SuiteFinishedEvent(result));
		}

		/// <summary>
		/// 	A suite is starting
		/// </summary>
		/// <param name="suite"> The suite that is starting </param>
		public void SuiteStarted(TestName testName)
		{
			this.events.Enqueue(new SuiteStartedEvent(testName));
		}

		/// <summary>
		/// 	A test case finished
		/// </summary>
		/// <param name="result"> Result of the test case </param>
		public void TestFinished(TestResult result)
		{
			this.events.Enqueue(new TestFinishedEvent(result));
		}

		/// <summary>
		/// 	A message has been output to the console.
		/// </summary>
		/// <param name="testOutput"> A console message </param>
		public void TestOutput(TestOutput output)
		{
			this.events.Enqueue(new OutputEvent(output));
		}

		/// <summary>
		/// 	A single test case is starting
		/// </summary>
		/// <param name="testCase"> The test case </param>
		public void TestStarted(TestName testName)
		{
			this.events.Enqueue(new TestStartedEvent(testName));
		}

		/// <summary>
		/// 	An unhandled exception occured while running a test,
		/// 	but the test was not terminated.
		/// </summary>
		/// <param name="exception"> </param>
		public void UnhandledException(Exception exception)
		{
			this.events.Enqueue(new UnhandledExceptionEvent(exception));
		}

		#endregion
	}
}