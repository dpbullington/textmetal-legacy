// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace NUnit.Core
{
	using System;

#if CLR_2_0 || CLR_4_0

#endif

	/// <summary>
	/// Summary description for TestSuite.
	/// </summary>
	[Serializable]
	public class TestSuite : Test
	{
		#region Fields

		private static Logger log = InternalTrace.GetLogger(typeof(TestSuite));

		/// <summary>
		/// Our collection of child tests
		/// </summary>
		private ArrayList tests = new ArrayList();

		/// <summary>
		/// The fixture setup methods for this suite
		/// </summary>
		protected MethodInfo[] fixtureSetUpMethods;

		/// <summary>
		/// The fixture teardown methods for this suite
		/// </summary>
		protected MethodInfo[] fixtureTearDownMethods;

		/// <summary>
		/// The setup methods for this suite
		/// </summary>
		protected MethodInfo[] setUpMethods;

		/// <summary>
		/// The teardown methods for this suite
		/// </summary>
		protected MethodInfo[] tearDownMethods;

#if CLR_2_0 || CLR_4_0
		/// <summary>
		/// The actions for this suite
		/// </summary>
		protected TestAction[] actions;
#endif

		/// <summary>
		/// Set to true to suppress sorting this suite's contents
		/// </summary>
		protected bool maintainTestOrder;

		/// <summary>
		/// Arguments for use in creating a parameterized fixture
		/// </summary>
		internal object[] arguments;

		/// <summary>
		/// The System.Type of the fixture for this test suite, if there is one
		/// </summary>
		private Type fixtureType;

		/// <summary>
		/// The fixture object, if it has been created
		/// </summary>
		private object fixture;

		#endregion

		#region Constructors

		public TestSuite(string name)
			: base(name)
		{
		}

		public TestSuite(string parentSuiteName, string name)
			: base(parentSuiteName, name)
		{
		}

		public TestSuite(Type fixtureType)
			: this(fixtureType, null)
		{
		}

		public TestSuite(Type fixtureType, object[] arguments)
			: base(fixtureType.FullName)
		{
			string name = TypeHelper.GetDisplayName(fixtureType, arguments);
			this.TestName.Name = name;

			this.TestName.FullName = name;
			string nspace = fixtureType.Namespace;
			if (nspace != null && nspace != "")
				this.TestName.FullName = nspace + "." + name;
			this.fixtureType = fixtureType;
			this.arguments = arguments;
		}

		#endregion

		#region Public Methods

		public void Sort()
		{
			if (!this.maintainTestOrder)
			{
				this.tests.Sort();

				foreach (Test test in this.Tests)
				{
					TestSuite suite = test as TestSuite;
					if (suite != null)
						suite.Sort();
				}
			}
		}

		public void Sort(IComparer comparer)
		{
			this.tests.Sort(comparer);

			foreach (Test test in this.Tests)
			{
				TestSuite suite = test as TestSuite;
				if (suite != null)
					suite.Sort(comparer);
			}
		}

		public void Add(Test test)
		{
//			if( test.RunState == RunState.Runnable )
//			{
//				test.RunState = this.RunState;
//				test.IgnoreReason = this.IgnoreReason;
//			}
			test.Parent = this;
			this.tests.Add(test);
		}

		public void Add(object fixture)
		{
			Test test = TestFixtureBuilder.BuildFrom(fixture);
			if (test != null)
				Add(test);
		}

		#endregion

		#region Properties

		public override IList Tests
		{
			get
			{
				return this.tests;
			}
		}

		public override bool IsSuite
		{
			get
			{
				return true;
			}
		}

		public override int TestCount
		{
			get
			{
				int count = 0;

				foreach (Test test in this.Tests)
					count += test.TestCount;
				return count;
			}
		}

		public override Type FixtureType
		{
			get
			{
				return this.fixtureType;
			}
		}

		public override object Fixture
		{
			get
			{
				return this.fixture;
			}
			set
			{
				this.fixture = value;
			}
		}

		public MethodInfo[] GetSetUpMethods()
		{
			return this.setUpMethods;
		}

		public MethodInfo[] GetTearDownMethods()
		{
			return this.tearDownMethods;
		}

#if CLR_2_0 || CLR_4_0
		internal virtual TestAction[] GetTestActions()
		{
			List<TestAction> allActions = new List<TestAction>();

			if (this.Parent != null && this.Parent is TestSuite)
			{
				TestAction[] parentActions = ((TestSuite)this.Parent).GetTestActions();

				if (parentActions != null)
					allActions.AddRange(parentActions);
			}

			if (this.actions != null)
				allActions.AddRange(this.actions);

			return allActions.ToArray();
		}
#endif

		#endregion

		#region Test Overrides

		public override string TestType
		{
			get
			{
				return "TestSuite";
			}
		}

		public override int CountTestCases(ITestFilter filter)
		{
			int count = 0;

			if (filter.Pass(this))
			{
				foreach (Test test in this.Tests)
					count += test.CountTestCases(filter);
			}
			return count;
		}

		public override TestResult Run(EventListener listener, ITestFilter filter)
		{
			listener.SuiteStarted(this.TestName);
			long startTime = DateTime.Now.Ticks;

			TestResult suiteResult = this.RunState == RunState.Runnable || this.RunState == RunState.Explicit
				? this.RunSuiteInContext(listener, filter)
				: this.SkipSuite(listener, filter);

			long stopTime = DateTime.Now.Ticks;
			double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
			suiteResult.Time = time;

			listener.SuiteFinished(suiteResult);
			return suiteResult;
		}

		private TestResult SkipSuite(EventListener listener, ITestFilter filter)
		{
			TestResult suiteResult = new TestResult(this);

			switch (this.RunState)
			{
				default:
				case RunState.Skipped:
					this.SkipAllTests(suiteResult, listener, filter);
					break;
				case RunState.NotRunnable:
					this.MarkAllTestsInvalid(suiteResult, listener, filter);
					break;
				case RunState.Ignored:
					this.IgnoreAllTests(suiteResult, listener, filter);
					break;
			}

			return suiteResult;
		}

		private TestResult RunSuiteInContext(EventListener listener, ITestFilter filter)
		{
			TestExecutionContext.Save();

			TestExecutionContext.CurrentContext.CurrentTest = this;

			try
			{
				return this.ShouldRunOnOwnThread
					? new TestSuiteThread(this).Run(listener, filter)
					: this.RunSuite(listener, filter);
			}
			finally
			{
				TestExecutionContext.Restore();
			}
		}

		public TestResult RunSuite(EventListener listener, ITestFilter filter)
		{
			TestResult suiteResult = new TestResult(this);
			TestExecutionContext.CurrentContext.CurrentResult = suiteResult;

			this.DoOneTimeSetUp(suiteResult);
#if CLR_2_0 || CLR_4_0
			this.DoOneTimeBeforeTestSuiteActions(suiteResult);
#endif

			if (this.Properties["_SETCULTURE"] != null)
			{
				TestExecutionContext.CurrentContext.CurrentCulture =
					new CultureInfo((string)this.Properties["_SETCULTURE"]);
			}
			else if (this.Properties["SetCulture"] != null) // For NUnitLite
			{
				TestExecutionContext.CurrentContext.CurrentCulture =
					new CultureInfo((string)this.Properties["SetCulture"]);
			}

			if (this.Properties["_SETUICULTURE"] != null)
			{
				TestExecutionContext.CurrentContext.CurrentUICulture =
					new CultureInfo((string)this.Properties["_SETUICULTURE"]);
			}
			else if (this.Properties["SetUICulture"] != null) // For NUnitLite
			{
				TestExecutionContext.CurrentContext.CurrentUICulture =
					new CultureInfo((string)this.Properties["SetUICulture"]);
			}

			switch (suiteResult.ResultState)
			{
				case ResultState.Failure:
				case ResultState.Error:
					this.MarkTestsFailed(this.Tests, suiteResult, listener, filter);
					break;
				case ResultState.NotRunnable:
					this.MarkTestsNotRun(this.Tests, ResultState.NotRunnable, suiteResult.Message, suiteResult, listener, filter);
					break;
				default:
					try
					{
						this.RunAllTests(suiteResult, listener, filter);
					}
					finally
					{
#if CLR_2_0 || CLR_4_0
						this.DoOneTimeAfterTestSuiteActions(suiteResult);
#endif
						this.DoOneTimeTearDown(suiteResult);
					}
					break;
			}

			return suiteResult;
		}

		#endregion

		#region Virtual Methods

		protected virtual void DoOneTimeSetUp(TestResult suiteResult)
		{
			if (this.FixtureType != null)
			{
				try
				{
					// In case TestFixture was created with fixture object
					if (this.Fixture == null && !this.IsStaticClass(this.FixtureType))
						this.CreateUserFixture();

					if (this.fixtureSetUpMethods != null)
					{
						foreach (MethodInfo fixtureSetUp in this.fixtureSetUpMethods)
							Reflect.InvokeMethod(fixtureSetUp, fixtureSetUp.IsStatic ? null : this.Fixture);
					}

					TestExecutionContext.CurrentContext.Update();
				}
				catch (Exception ex)
				{
					if (ex is NUnitException || ex is TargetInvocationException)
						ex = ex.InnerException;

					if (ex is InvalidTestFixtureException)
						suiteResult.Invalid(ex.Message);
					else if (this.IsIgnoreException(ex))
					{
						this.RunState = RunState.Ignored;
						suiteResult.Ignore(ex.Message);
						suiteResult.StackTrace = ex.StackTrace;
						this.IgnoreReason = ex.Message;
					}
					else if (this.IsAssertException(ex))
						suiteResult.Failure(ex.Message, ex.StackTrace, FailureSite.SetUp);
					else
						suiteResult.Error(ex, FailureSite.SetUp);
				}
			}
		}

#if CLR_2_0 || CLR_4_0

		protected virtual void ExecuteActions(ActionPhase phase)
		{
			List<TestAction> targetActions = new List<TestAction>();

			if (this.actions != null)
			{
				foreach (var action in this.actions)
				{
					if (action.DoesTarget(TestAction.TargetsSuite) || action.DoesTarget(TestAction.TargetsDefault))
						targetActions.Add(action);
				}
			}

			ActionsHelper.ExecuteActions(phase, targetActions, this);
		}

		protected virtual void DoOneTimeBeforeTestSuiteActions(TestResult suiteResult)
		{
			try
			{
				this.ExecuteActions(ActionPhase.Before);
				TestExecutionContext.CurrentContext.Update();
			}
			catch (Exception ex)
			{
				if (ex is NUnitException || ex is TargetInvocationException)
					ex = ex.InnerException;

				if (ex is InvalidTestFixtureException)
					suiteResult.Invalid(ex.Message);
				else if (this.IsIgnoreException(ex))
				{
					this.RunState = RunState.Ignored;
					suiteResult.Ignore(ex.Message);
					suiteResult.StackTrace = ex.StackTrace;
					this.IgnoreReason = ex.Message;
				}
				else if (this.IsAssertException(ex))
					suiteResult.Failure(ex.Message, ex.StackTrace, FailureSite.SetUp);
				else
					suiteResult.Error(ex, FailureSite.SetUp);
			}
		}
#endif

		protected virtual void CreateUserFixture()
		{
			if (this.arguments != null && this.arguments.Length > 0)
				this.Fixture = Reflect.Construct(this.FixtureType, this.arguments);
			else
				this.Fixture = Reflect.Construct(this.FixtureType);
		}

		protected virtual void DoOneTimeTearDown(TestResult suiteResult)
		{
			if (this.FixtureType != null)
			{
				try
				{
					if (this.fixtureTearDownMethods != null)
					{
						int index = this.fixtureTearDownMethods.Length;
						while (--index >= 0)
						{
							MethodInfo fixtureTearDown = this.fixtureTearDownMethods[index];
							Reflect.InvokeMethod(fixtureTearDown, fixtureTearDown.IsStatic ? null : this.Fixture);
						}
					}

					IDisposable disposable = this.Fixture as IDisposable;
					if (disposable != null)
						disposable.Dispose();
				}
				catch (Exception ex)
				{
					// Error in TestFixtureTearDown or Dispose causes the
					// suite to be marked as a failure, even if
					// all the contained tests passed.
					NUnitException nex = ex as NUnitException;
					if (nex != null)
						ex = nex.InnerException;

					suiteResult.Failure(ex.Message, ex.StackTrace, FailureSite.TearDown);
				}

				this.Fixture = null;
			}
		}

#if CLR_2_0 || CLR_4_0
		protected virtual void DoOneTimeAfterTestSuiteActions(TestResult suiteResult)
		{
			try
			{
				this.ExecuteActions(ActionPhase.After);
			}
			catch (Exception ex)
			{
				// Error in TestFixtureTearDown or Dispose causes the
				// suite to be marked as a failure, even if
				// all the contained tests passed.
				NUnitException nex = ex as NUnitException;
				if (nex != null)
					ex = nex.InnerException;

				suiteResult.Failure(ex.Message, ex.StackTrace, FailureSite.TearDown);
			}
		}
#endif

		protected virtual bool IsAssertException(Exception ex)
		{
			return ex.GetType().FullName == NUnitFramework.AssertException;
		}

		protected virtual bool IsIgnoreException(Exception ex)
		{
			return ex.GetType().FullName == NUnitFramework.IgnoreException;
		}

		#endregion

		#region Helper Methods

		private bool IsStaticClass(Type type)
		{
			return type.IsAbstract && type.IsSealed;
		}

		private void RunAllTests(TestResult suiteResult, EventListener listener, ITestFilter filter)
		{
			if (this.Properties.Contains("Timeout"))
				TestExecutionContext.CurrentContext.TestCaseTimeout = (int)this.Properties["Timeout"];

			IDictionary settings = TestExecutionContext.CurrentContext.TestPackage.Settings;
			bool stopOnError = settings.Contains("StopOnError") && (bool)settings["StopOnError"];

			foreach (Test test in ArrayList.Synchronized(this.Tests))
			{
				if (filter.Pass(test))
				{
					RunState saveRunState = test.RunState;

					if (test.RunState == RunState.Runnable && this.RunState != RunState.Runnable && this.RunState != RunState.Explicit)
					{
						test.RunState = this.RunState;
						test.IgnoreReason = this.IgnoreReason;
					}

					TestResult result = test.Run(listener, filter);

					log.Debug("Test result = " + result.ResultState);

					suiteResult.AddResult(result);

					log.Debug("Suite result = " + suiteResult.ResultState);

					if (saveRunState != test.RunState)
					{
						test.RunState = saveRunState;
						test.IgnoreReason = null;
					}

					if (result.ResultState == ResultState.Cancelled)
						break;

					if ((result.IsError || result.IsFailure || result.ResultState == ResultState.NotRunnable) && stopOnError)
						break;
				}
			}
		}

		private void SkipAllTests(TestResult suiteResult, EventListener listener, ITestFilter filter)
		{
			suiteResult.Skip(this.IgnoreReason);
			this.MarkTestsNotRun(this.Tests, ResultState.Skipped, this.IgnoreReason, suiteResult, listener, filter);
		}

		private void IgnoreAllTests(TestResult suiteResult, EventListener listener, ITestFilter filter)
		{
			suiteResult.Ignore(this.IgnoreReason);
			this.MarkTestsNotRun(this.Tests, ResultState.Ignored, this.IgnoreReason, suiteResult, listener, filter);
		}

		private void MarkAllTestsInvalid(TestResult suiteResult, EventListener listener, ITestFilter filter)
		{
			suiteResult.Invalid(this.IgnoreReason);
			this.MarkTestsNotRun(this.Tests, ResultState.NotRunnable, this.IgnoreReason, suiteResult, listener, filter);
		}

		private void MarkTestsNotRun(
			IList tests, ResultState resultState, string ignoreReason, TestResult suiteResult, EventListener listener, ITestFilter filter)
		{
			foreach (Test test in ArrayList.Synchronized(tests))
			{
				if (filter.Pass(test))
					this.MarkTestNotRun(test, resultState, ignoreReason, suiteResult, listener, filter);
			}
		}

		private void MarkTestNotRun(
			Test test, ResultState resultState, string ignoreReason, TestResult suiteResult, EventListener listener, ITestFilter filter)
		{
			if (test is TestSuite)
			{
				listener.SuiteStarted(test.TestName);
				TestResult result = new TestResult(new TestInfo(test));
				result.SetResult(resultState, ignoreReason, null);
				this.MarkTestsNotRun(test.Tests, resultState, ignoreReason, result, listener, filter);
				suiteResult.AddResult(result);
				listener.SuiteFinished(result);
			}
			else
			{
				listener.TestStarted(test.TestName);
				TestResult result = new TestResult(new TestInfo(test));
				result.SetResult(resultState, ignoreReason, null);
				suiteResult.AddResult(result);
				listener.TestFinished(result);
			}
		}

		private void MarkTestsFailed(
			IList tests, TestResult suiteResult, EventListener listener, ITestFilter filter)
		{
			foreach (Test test in ArrayList.Synchronized(tests))
			{
				if (filter.Pass(test))
					this.MarkTestFailed(test, suiteResult, listener, filter);
			}
		}

		private void MarkTestFailed(
			Test test, TestResult suiteResult, EventListener listener, ITestFilter filter)
		{
			if (test is TestSuite)
			{
				listener.SuiteStarted(test.TestName);
				TestResult result = new TestResult(new TestInfo(test));
				string msg = this.FixtureType == null
					? "Parent SetUp failed"
					: string.Format("Parent SetUp failed in {0}", this.FixtureType.Name);

				result.Failure(msg, null, FailureSite.Parent);
				this.MarkTestsFailed(test.Tests, result, listener, filter);
				suiteResult.AddResult(result);
				listener.SuiteFinished(result);
			}
			else
			{
				listener.TestStarted(test.TestName);
				TestResult result = new TestResult(new TestInfo(test));
				string msg = this.FixtureType == null
					? "TestFixtureSetUp failed"
					: string.Format("TestFixtureSetUp failed in {0}", this.FixtureType.Name);
				result.Failure(msg, null, FailureSite.Parent);
				suiteResult.AddResult(result);
				listener.TestFinished(result);
			}
		}

		#endregion
	}
}