// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Threading;

namespace NUnit.Core
{
	/// <summary>
	/// 	TestRunnerThread encapsulates running a test on a thread.
	/// 	It knows how to create the thread based on configuration
	/// 	settings and can cancel abort the test if necessary.
	/// </summary>
	public class TestRunnerThread
	{
		#region Constructors/Destructors

		public TestRunnerThread(TestRunner runner, ApartmentState apartmentState, ThreadPriority priority)
		{
			this.runner = runner;
			this.thread = new Thread(new ThreadStart(this.TestRunnerThreadProc));
			this.thread.IsBackground = true;
			this.thread.Name = "TestRunnerThread";
			this.thread.Priority = priority;
			if (apartmentState != ApartmentState.Unknown)
#if CLR_2_0 || CLR_4_0
				this.thread.SetApartmentState(apartmentState);
#else
                thread.ApartmentState = apartmentState;
#endif
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// 	The Test filter used in selecting the tests
		//private string[] testNames;
		private ITestFilter filter;

		/// <summary>
		/// 	The EventListener interface to receive test events
		/// </summary>
		private EventListener listener;

		/// <summary>
		/// 	The logging threshold for which output should be captured
		/// </summary>
		private LoggingThreshold logLevel;

		/// <summary>
		/// 	Array of returned results
		/// </summary>
		private TestResult[] results;

		/// <summary>
		/// 	The Test runner to be used in running tests on the thread
		/// </summary>
		private TestRunner runner;

		/// <summary>
		/// 	The System.Threading.Thread created by the object
		/// </summary>
		private Thread thread;

		/// <summary>
		/// 	Indicates whether trace output should be captured
		/// </summary>
		private bool tracing;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	True if the thread is executing
		/// </summary>
		public bool IsAlive
		{
			get
			{
				return this.thread.IsAlive;
			}
		}

		/// <summary>
		/// 	Array of returned results
		/// </summary>
		public TestResult[] Results
		{
			get
			{
				return this.results;
			}
		}

		#endregion

		#region Methods/Operators

		public void Cancel()
		{
			ThreadUtility.Kill(this.thread);
		}

		public void StartRun(EventListener listener, ITestFilter filter, bool tracing, LoggingThreshold logLevel)
		{
			this.listener = listener;
			this.filter = filter;
			this.tracing = tracing;
			this.logLevel = logLevel;

			this.thread.Start();
		}

		/// <summary>
		/// 	The thread proc for our actual test run
		/// </summary>
		private void TestRunnerThreadProc()
		{
			try
			{
				this.results = new TestResult[] { this.runner.Run(this.listener, this.filter, this.tracing, this.logLevel) };
			}
			catch (Exception ex)
			{
				if (!(ex is ThreadAbortException))
					throw new ApplicationException("Exception in TestRunnerThread", ex);
			}
		}

		public void Wait()
		{
			if (this.thread.IsAlive)
				this.thread.Join();
		}

		#endregion
	}
}