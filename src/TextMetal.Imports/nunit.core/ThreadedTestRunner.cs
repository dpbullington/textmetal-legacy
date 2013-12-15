// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System.Threading;

namespace NUnit.Core
{
	using System;

	/// <summary>
	/// ThreadedTestRunner overrides the Run and BeginRun methods
	/// so that they are always run on a separate thread. The actual
	/// </summary>
	public class ThreadedTestRunner : ProxyTestRunner
	{
		#region Constructors/Destructors

		public ThreadedTestRunner(TestRunner testRunner)
			: this(testRunner, ApartmentState.Unknown, ThreadPriority.Normal)
		{
		}

		public ThreadedTestRunner(TestRunner testRunner, ApartmentState apartmentState, ThreadPriority priority)
			: base(testRunner)
		{
			this.apartmentState = apartmentState;
			this.priority = priority;
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(ThreadedTestRunner));

		private ApartmentState apartmentState;
		private ThreadPriority priority;
		private TestRunnerThread testRunnerThread;

		#endregion

		#region Methods/Operators

		public override void BeginRun(EventListener listener, ITestFilter filter, bool tracing, LoggingThreshold logLevel)
		{
			log.Info("BeginRun");
			this.testRunnerThread = new TestRunnerThread(this.TestRunner, this.apartmentState, this.priority);
			this.testRunnerThread.StartRun(listener, filter, tracing, logLevel);
		}

		public override void CancelRun()
		{
			if (this.testRunnerThread != null)
				this.testRunnerThread.Cancel();
		}

		public override TestResult EndRun()
		{
			log.Info("EndRun");
			this.Wait();
			return this.TestRunner.TestResult;
		}

		public override TestResult Run(EventListener listener, ITestFilter filter, bool tracing, LoggingThreshold logLevel)
		{
			this.BeginRun(listener, filter, tracing, logLevel);
			return this.EndRun();
		}

		public override void Wait()
		{
			if (this.testRunnerThread != null)
				this.testRunnerThread.Wait();
		}

		#endregion
	}
}