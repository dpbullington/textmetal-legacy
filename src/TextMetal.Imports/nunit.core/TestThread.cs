// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace NUnit.Core
{
	/// <summary>
	/// 	Represents a thread of test execution and runs a test
	/// 	on a thread, implementing timeout and setting the 
	/// 	apartment state appropriately.
	/// </summary>
	public abstract class TestThread
	{
		#region Constructors/Destructors

		protected TestThread(Test test)
		{
			this.test = test;

			this.thread = new Thread(new ThreadStart(this.RunTestProc));
			this.thread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
			this.thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;

			// Setting to Unknown causes an error under the Mono 1.0 profile
			if (test.ApartmentState != ApartmentState.Unknown)
				this.ApartmentState = test.ApartmentState;
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(TestThread));
		protected ContextDictionary contextDictionary;
		protected ITestFilter filter;
		protected EventListener listener;

		private Test test;

		/// <summary>
		/// 	The Thread object used to run tests
		/// </summary>
		protected Thread thread;

		/// <summary>
		/// 	The result of running the test, which must be kept
		/// 	separate from the returned TestResult while the thread
		/// 	is running to avoid race conditions.
		/// </summary>
		protected TestResult threadResult;

		/// <summary>
		/// 	Unexpected exception thrown by test thread
		/// </summary>
		protected Exception thrownException;

		#endregion

		#region Properties/Indexers/Events

		public ApartmentState ApartmentState
		{
#if CLR_2_0 || CLR_4_0
			get
			{
				return this.thread.GetApartmentState();
			}
			set
			{
				this.thread.SetApartmentState(value);
			}
#else
            get { return thread.ApartmentState; }
            set { thread.ApartmentState = value; }
#endif
		}

		protected abstract int Timeout
		{
			get;
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Run the test, honoring any timeout value provided. If the
		/// 	timeout is exceeded, set the testresult as a failure. As
		/// 	currently implemented, the thread proc calls test.doRun,
		/// 	which handles all exceptions itself. However, for safety,
		/// 	any exception thrown is rethrown upwards.
		/// 
		/// 	TODO: It would be cleaner to call test.Run, since that's
		/// 	part of the pubic interface, but it would require some
		/// 	restructuring of the Test hierarchy.
		/// </summary>
		public TestResult Run(EventListener listener, ITestFilter filter)
		{
			TestResult testResult = new TestResult(this.test);

			this.thrownException = null;
			this.listener = listener;
			this.filter = filter;

#if CLR_2_0 || CLR_4_0
			this.contextDictionary = (ContextDictionary)CallContext.LogicalGetData("NUnit.Framework.TestContext");
#else
			this.contextDictionary = (ContextDictionary)CallContext.GetData("NUnit.Framework.TestContext");
#endif

			log.Debug("Starting test in separate thread");
			this.thread.Start();
			this.thread.Join(this.Timeout);

			// Timeout?
			if (this.thread.IsAlive)
			{
				log.Debug("Test timed out - aborting thread");
				ThreadUtility.Kill(this.thread);

				// NOTE: Without the use of Join, there is a race condition here.
				// The thread sets the result to Cancelled and our code below sets
				// it to Failure. In order for the result to be shown as a failure,
				// we need to ensure that the following code executes after the
				// thread has terminated. There is a risk here: the test code might
				// refuse to terminate. However, it's more important to deal with
				// the normal rather than a pathological case.
				this.thread.Join();
				testResult.Failure(string.Format("Test exceeded Timeout value of {0}ms", this.Timeout), null);
			}
			else if (this.thrownException != null)
			{
				log.Debug("Test threw " + this.thrownException.GetType().Name);
				throw this.thrownException;
			}
			else
			{
				log.Debug("Test completed normally");
				testResult = this.threadResult;
			}

			return testResult;
		}

		protected abstract void RunTest();

		/// <summary>
		/// 	This is the engine of this class; the actual call to test.doRun!
		/// 	Note that any thrown exception is saved for later use!
		/// </summary>
		private void RunTestProc()
		{
#if CLR_2_0 || CLR_4_0
			CallContext.LogicalSetData("NUnit.Framework.TestContext", this.contextDictionary);
#else
			CallContext.SetData("NUnit.Framework.TestContext", contextDictionary);
#endif

			try
			{
				this.RunTest();
			}
			catch (Exception e)
			{
				this.thrownException = e;
			}
			finally
			{
				CallContext.FreeNamedDataSlot("NUnit.Framework.TestContext");
			}
		}

		#endregion
	}

	public class TestMethodThread : TestThread
	{
		#region Constructors/Destructors

		public TestMethodThread(TestMethod testMethod)
			: base(testMethod)
		{
			this.testMethod = testMethod;
		}

		#endregion

		#region Fields/Constants

		private TestMethod testMethod;

		#endregion

		#region Properties/Indexers/Events

		protected override int Timeout
		{
			get
			{
				return this.testMethod.Timeout == 0 //|| System.Diagnostics.Debugger.IsAttached
					       ? System.Threading.Timeout.Infinite
					       : this.testMethod.Timeout;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void RunTest()
		{
			this.threadResult = this.testMethod.RunTest();
		}

		#endregion
	}

	public class TestSuiteThread : TestThread
	{
		#region Constructors/Destructors

		public TestSuiteThread(TestSuite suite)
			: base(suite)
		{
			this.suite = suite;
		}

		#endregion

		#region Fields/Constants

		private TestSuite suite;

		#endregion

		#region Properties/Indexers/Events

		protected override int Timeout
		{
			get
			{
				return System.Threading.Timeout.Infinite;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void RunTest()
		{
			this.threadResult = this.suite.RunSuite(this.listener, this.filter);
		}

		#endregion
	}
}