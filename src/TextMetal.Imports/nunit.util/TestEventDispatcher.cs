// ****************************************************************
// Copyright 2002-2003, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// Helper class used to dispatch test events
	/// </summary>
	public class TestEventDispatcher : ITestEvents
	{
		// Project loading events

		#region Properties/Indexers/Events

		public event TestEventHandler ProjectLoadFailed;
		public event TestEventHandler ProjectLoaded;
		public event TestEventHandler ProjectLoading;
		public event TestEventHandler ProjectUnloadFailed;
		public event TestEventHandler ProjectUnloaded;
		public event TestEventHandler ProjectUnloading;

		// Test loading events
		public event TestEventHandler RunFinished;
		public event TestEventHandler RunStarting;

		public event TestEventHandler SuiteFinished;
		public event TestEventHandler SuiteStarting;

		public event TestEventHandler TestException;
		public event TestEventHandler TestFinished;
		public event TestEventHandler TestLoadFailed;
		public event TestEventHandler TestLoaded;
		public event TestEventHandler TestLoading;
		public event TestEventHandler TestOutput;
		public event TestEventHandler TestReloadFailed;
		public event TestEventHandler TestReloaded;
		public event TestEventHandler TestReloading;
		public event TestEventHandler TestStarting;
		public event TestEventHandler TestUnloadFailed;
		public event TestEventHandler TestUnloaded;
		public event TestEventHandler TestUnloading;

		#endregion

		#region Methods/Operators

		protected virtual void Fire(TestEventHandler handler, TestEventArgs e)
		{
			if (handler != null)
				handler(this, e);
		}

		public void FireProjectLoadFailed(string fileName, Exception exception)
		{
			this.Fire(
				this.ProjectLoadFailed,
				new TestEventArgs(TestAction.ProjectLoadFailed, fileName, exception));
		}

		public void FireProjectLoaded(string fileName)
		{
			this.Fire(
				this.ProjectLoaded,
				new TestEventArgs(TestAction.ProjectLoaded, fileName));
		}

		public void FireProjectLoading(string fileName)
		{
			this.Fire(
				this.ProjectLoading,
				new TestEventArgs(TestAction.ProjectLoading, fileName));
		}

		public void FireProjectUnloadFailed(string fileName, Exception exception)
		{
			this.Fire(
				this.ProjectUnloadFailed,
				new TestEventArgs(TestAction.ProjectUnloadFailed, fileName, exception));
		}

		public void FireProjectUnloaded(string fileName)
		{
			this.Fire(
				this.ProjectUnloaded,
				new TestEventArgs(TestAction.ProjectUnloaded, fileName));
		}

		public void FireProjectUnloading(string fileName)
		{
			this.Fire(
				this.ProjectUnloading,
				new TestEventArgs(TestAction.ProjectUnloading, fileName));
		}

		public void FireRunFinished(TestResult result)
		{
			this.Fire(
				this.RunFinished,
				new TestEventArgs(TestAction.RunFinished, result));
		}

		public void FireRunFinished(Exception exception)
		{
			this.Fire(
				this.RunFinished,
				new TestEventArgs(TestAction.RunFinished, exception));
		}

		public void FireRunStarting(string name, int testCount)
		{
			this.Fire(
				this.RunStarting,
				new TestEventArgs(TestAction.RunStarting, name, testCount));
		}

		public void FireSuiteFinished(TestResult result)
		{
			this.Fire(
				this.SuiteFinished,
				new TestEventArgs(TestAction.SuiteFinished, result));
		}

		public void FireSuiteStarting(TestName testName)
		{
			this.Fire(
				this.SuiteStarting,
				new TestEventArgs(TestAction.SuiteStarting, testName));
		}

		public void FireTestException(string name, Exception exception)
		{
			this.Fire(
				this.TestException,
				new TestEventArgs(TestAction.TestException, name, exception));
		}

		public void FireTestFinished(TestResult result)
		{
			this.Fire(
				this.TestFinished,
				new TestEventArgs(TestAction.TestFinished, result));
		}

		public void FireTestLoadFailed(string fileName, Exception exception)
		{
			this.Fire(
				this.TestLoadFailed,
				new TestEventArgs(TestAction.TestLoadFailed, fileName, exception));
		}

		public void FireTestLoaded(string fileName, ITest test)
		{
			this.Fire(
				this.TestLoaded,
				new TestEventArgs(TestAction.TestLoaded, fileName, test));
		}

		public void FireTestLoading(string fileName)
		{
			this.Fire(
				this.TestLoading,
				new TestEventArgs(TestAction.TestLoading, fileName));
		}

		public void FireTestOutput(TestOutput testOutput)
		{
			this.Fire(
				this.TestOutput,
				new TestEventArgs(TestAction.TestOutput, testOutput));
		}

		public void FireTestReloadFailed(string fileName, Exception exception)
		{
			this.Fire(
				this.TestReloadFailed,
				new TestEventArgs(TestAction.TestReloadFailed, fileName, exception));
		}

		public void FireTestReloaded(string fileName, ITest test)
		{
			this.Fire(
				this.TestReloaded,
				new TestEventArgs(TestAction.TestReloaded, fileName, test));
		}

		public void FireTestReloading(string fileName)
		{
			this.Fire(
				this.TestReloading,
				new TestEventArgs(TestAction.TestReloading, fileName));
		}

		public void FireTestStarting(TestName testName)
		{
			this.Fire(
				this.TestStarting,
				new TestEventArgs(TestAction.TestStarting, testName));
		}

		public void FireTestUnloadFailed(string fileName, Exception exception)
		{
			this.Fire(
				this.TestUnloadFailed,
				new TestEventArgs(TestAction.TestUnloadFailed, fileName, exception));
		}

		public void FireTestUnloaded(string fileName)
		{
			this.Fire(
				this.TestUnloaded,
				new TestEventArgs(TestAction.TestUnloaded, fileName));
		}

		public void FireTestUnloading(string fileName)
		{
			this.Fire(
				this.TestUnloading,
				new TestEventArgs(TestAction.TestUnloading, fileName));
		}

		#endregion
	}
}