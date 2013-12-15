// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Threading;

namespace NUnit.Core
{
	/// <summary>
	/// SimpleTestRunner is the simplest direct-running TestRunner. It
	/// passes the event listener interface that is provided on to the tests
	/// to use directly and does nothing to redirect text output. Both
	/// Run and BeginRun are actually synchronous, although the client
	/// can usually ignore this. BeginRun + EndRun operates as expected.
	/// </summary>
	public class SimpleTestRunner : MarshalByRefObject, TestRunner
	{
		#region Constructors/Destructors

		public SimpleTestRunner()
			: this(0)
		{
		}

		public SimpleTestRunner(int runnerID)
		{
			this.runnerID = runnerID;
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(SimpleTestRunner));

		/// <summary>
		/// The builder we use to load tests, created for each load
		/// </summary>
		private TestSuiteBuilder builder;

		/// <summary>
		/// The thread on which Run was called. Set to the
		/// current thread while a run is in process.
		/// </summary>
		private Thread runThread;

		/// <summary>
		/// Identifier for this runner. Must be unique among all
		/// active runners in order to locate tests. Default
		/// value of 0 is adequate in applications with a single
		/// runner or a non-branching chain of runners.
		/// </summary>
		private int runnerID = 0;

		/// <summary>
		/// The loaded test suite
		/// </summary>
		private Test test;

		/// <summary>
		/// Results from the last test run
		/// </summary>
		private TestResult testResult;

		#endregion

		#region Properties/Indexers/Events

		public IList AssemblyInfo
		{
			get
			{
				return this.builder.AssemblyInfo;
			}
		}

		public virtual int ID
		{
			get
			{
				return this.runnerID;
			}
		}

		public virtual bool Running
		{
			get
			{
				return this.runThread != null && this.runThread.IsAlive;
			}
		}

		public ITest Test
		{
			get
			{
				return this.test == null ? null : new TestNode(this.test);
			}
		}

		/// <summary>
		/// Results from the last test run
		/// </summary>
		public TestResult TestResult
		{
			get
			{
				return this.testResult;
			}
		}

		#endregion

		#region Methods/Operators

		public void BeginRun(EventListener listener, ITestFilter filter, bool tracing, LoggingThreshold logLevel)
		{
			this.testResult = this.Run(listener, filter, tracing, logLevel);
		}

		public virtual void CancelRun()
		{
			if (this.runThread != null)
			{
				// Cancel Synchronous run only if on another thread
				if (this.runThread == Thread.CurrentThread)
					throw new InvalidOperationException("May not CancelRun on same thread that is running the test");

				ThreadUtility.Kill(this.runThread);
			}
		}

		public int CountTestCases(ITestFilter filter)
		{
			return this.test.CountTestCases(filter);
		}

		public void Dispose()
		{
			this.Unload();
		}

		public virtual TestResult EndRun()
		{
			return this.TestResult;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		/// <summary>
		/// Load a TestPackage
		/// </summary>
		/// <param name="package"> The package to be loaded </param>
		/// <returns> True on success, false on failure </returns>
		public bool Load(TestPackage package)
		{
			log.Debug("Loading package " + package.Name);

			this.builder = new TestSuiteBuilder();

			this.test = this.builder.Build(package);
			if (this.test == null)
				return false;

			this.test.SetRunnerID(this.runnerID, true);
			TestExecutionContext.CurrentContext.TestPackage = package;
			return true;
		}

		public virtual TestResult Run(EventListener listener, ITestFilter filter, bool tracing, LoggingThreshold logLevel)
		{
			try
			{
				log.Debug("Starting test run");

				// Take note of the fact that we are running
				this.runThread = Thread.CurrentThread;

				listener.RunStarted(this.Test.TestName.FullName, this.test.CountTestCases(filter));

				this.testResult = this.test.Run(listener, filter);

				// Signal that we are done
				listener.RunFinished(this.testResult);
				log.Debug("Test run complete");

				// Return result array
				return this.testResult;
			}
			catch (Exception exception)
			{
				// Signal that we finished with an exception
				listener.RunFinished(exception);
				// Rethrow - should we do this?
				throw;
			}
			finally
			{
				this.runThread = null;
			}
		}

		/// <summary>
		/// Unload all tests previously loaded
		/// </summary>
		public void Unload()
		{
			log.Debug("Unloading");
			this.test = null; // All for now
		}

		/// <summary>
		/// Wait is a NOP for SimpleTestRunner
		/// </summary>
		public virtual void Wait()
		{
		}

		#endregion
	}
}