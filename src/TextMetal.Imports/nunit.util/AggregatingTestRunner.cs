// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System.Collections;
using System.IO;

using NUnit.Core;

namespace NUnit.Util
{
	using System;

	#region AggregatingTestRunner

	/// <summary>
	/// 	AggregatingTestRunner allows running multiple TestRunners
	/// 	and combining the results.
	/// </summary>
	public abstract class AggregatingTestRunner : MarshalByRefObject, TestRunner, EventListener
	{
		#region Constructors/Destructors

		public AggregatingTestRunner()
			: this(0)
		{
		}

		public AggregatingTestRunner(int runnerID)
		{
			this.runnerID = runnerID;
			this.testName = new TestName();
			this.testName.TestID = new TestID(AggregateTestID);
			this.testName.RunnerID = this.runnerID;
			this.testName.FullName = this.testName.Name = "Not Loaded";
		}

		#endregion

		#region Fields/Constants

		private static int AggregateTestID = 1000;

		/// <summary>
		/// 	The loaded test suite
		/// </summary>
		protected TestNode aggregateTest;

		/// <summary>
		/// 	The event listener for the currently running test
		/// </summary>
		protected EventListener listener;

		private Logger log;

		/// <summary>
		/// 	Indicates whether we should run test assemblies in parallel
		/// </summary>
		private bool runInParallel;

		/// <summary>
		/// 	Our runner ID
		/// </summary>
		protected int runnerID;

		/// <summary>
		/// 	The downstream TestRunners
		/// </summary>
		protected ArrayList runners;

		protected TestName testName;

		/// <summary>
		/// 	The result of the last run
		/// </summary>
		private TestResult testResult;

		#endregion

		#region Properties/Indexers/Events

		public virtual IList AssemblyInfo
		{
			get
			{
				ArrayList info = new ArrayList();
				foreach (TestRunner runner in this.runners)
					info.AddRange(runner.AssemblyInfo);
				return info;
			}
		}

		public virtual int ID
		{
			get
			{
				return this.runnerID;
			}
		}

		private Logger Log
		{
			get
			{
				if (this.log == null)
					this.log = InternalTrace.GetLogger(this.GetType());

				return this.log;
			}
		}

		public virtual bool Running
		{
			get
			{
				foreach (TestRunner runner in this.runners)
				{
					if (runner.Running)
						return true;
				}

				return false;
			}
		}

		public virtual ITest Test
		{
			get
			{
				if (this.aggregateTest == null && this.runners != null)
				{
					// Count non-null tests, in case we specified a fixture
					int count = 0;
					foreach (TestRunner runner in this.runners)
					{
						if (runner.Test != null)
							++count;
					}

					// Copy non-null tests to an array
					int index = 0;
					ITest[] tests = new ITest[count];
					foreach (TestRunner runner in this.runners)
					{
						if (runner.Test != null)
							tests[index++] = runner.Test;
					}

					// Return master node containing all the tests
					this.aggregateTest = new TestNode(this.testName, tests);
				}

				return this.aggregateTest;
			}
		}

		public virtual TestResult TestResult
		{
			get
			{
				return this.testResult;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void BeginRun(EventListener listener, ITestFilter filter, bool tracing, LoggingThreshold logLevel)
		{
			// Save active listener for derived classes
			this.listener = listener;

			this.Log.Info("BeginRun");

			// ThreadedTestRunner will call our Run method on a separate thread
			ThreadedTestRunner threadedRunner = new ThreadedTestRunner(this);
			threadedRunner.BeginRun(listener, filter, tracing, logLevel);
		}

		public virtual void CancelRun()
		{
			foreach (TestRunner runner in this.runners)
				runner.CancelRun();
		}

		public virtual int CountTestCases(ITestFilter filter)
		{
			int count = 0;
			foreach (TestRunner runner in this.runners)
				count += runner.CountTestCases(filter);
			return count;
		}

		protected abstract TestRunner CreateRunner(int runnerID);

		public void Dispose()
		{
			foreach (TestRunner runner in this.runners)
			{
				if (runner != null)
					runner.Dispose();
			}
		}

		public virtual TestResult EndRun()
		{
			this.Log.Info("EndRun");
			TestResult suiteResult = new TestResult(this.Test as TestInfo);
			foreach (TestRunner runner in this.runners)
				suiteResult.Results.Add(runner.EndRun());

			return suiteResult;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public bool Load(TestPackage package)
		{
			this.Log.Info("Loading " + package.Name);

			this.testName.FullName = this.testName.Name = package.FullName;
			this.runners = new ArrayList();

			int nfound = 0;
			int index = 0;

			string targetAssemblyName = null;
			if (package.TestName != null && package.Assemblies.Contains(package.TestName))
			{
				targetAssemblyName = package.TestName;
				package.TestName = null;
			}

			// NOTE: This is experimental. A normally created test package
			// will never have this setting.
			if (package.Settings.Contains("RunInParallel"))
			{
				this.runInParallel = true;
				package.Settings.Remove("RunInParallel");
			}

			//string basePath = package.BasePath;
			//if (basePath == null)
			//    basePath = Path.GetDirectoryName(package.FullName);

			//string configFile = package.ConfigurationFile;
			//if (configFile == null && package.Name != null && !package.IsSingleAssembly)
			//    configFile = Path.ChangeExtension(package.Name, ".config");

			foreach (string assembly in package.Assemblies)
			{
				if (targetAssemblyName == null || targetAssemblyName == assembly)
				{
					TestRunner runner = this.CreateRunner(this.runnerID * 100 + index + 1);

					TestPackage p = new TestPackage(assembly);
					p.AutoBinPath = package.AutoBinPath;
					p.ConfigurationFile = package.ConfigurationFile;
					p.BasePath = package.BasePath;
					p.PrivateBinPath = package.PrivateBinPath;
					p.TestName = package.TestName;
					foreach (object key in package.Settings.Keys)
						p.Settings[key] = package.Settings[key];

					if (package.TestName == null)
					{
						this.runners.Add(runner);
						if (runner.Load(p))
							nfound++;
					}
					else if (runner.Load(p))
					{
						this.runners.Add(runner);
						nfound++;
					}
				}
			}

			this.Log.Info("Load complete");

			if (package.TestName == null && targetAssemblyName == null)
				return nfound == package.Assemblies.Count;
			else
				return nfound > 0;
		}

		public virtual TestResult Run(EventListener listener, ITestFilter filter, bool tracing, LoggingThreshold logLevel)
		{
			this.Log.Info("Run - EventListener={0}", listener.GetType().Name);

			// Save active listener for derived classes
			this.listener = listener;

			ITest[] tests = new ITest[this.runners.Count];
			for (int index = 0; index < this.runners.Count; index++)
				tests[index] = ((TestRunner)this.runners[index]).Test;

			string name = this.testName.Name;
			int count = this.CountTestCases(filter);
			this.Log.Info("Signalling RunStarted({0},{1})", name, count);
			this.listener.RunStarted(name, count);

			long startTime = DateTime.Now.Ticks;

			TestResult result = new TestResult(new TestInfo(this.testName, tests));

			if (this.runInParallel)
			{
				foreach (TestRunner runner in this.runners)
				{
					if (filter.Pass(runner.Test))
						runner.BeginRun(this, filter, tracing, logLevel);
				}

				result = this.EndRun();
			}
			else
			{
				foreach (TestRunner runner in this.runners)
				{
					if (filter.Pass(runner.Test))
						result.AddResult(runner.Run(this, filter, tracing, logLevel));
				}
			}

			long stopTime = DateTime.Now.Ticks;
			double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
			result.Time = time;

			this.listener.RunFinished(result);

			this.testResult = result;

			return result;
		}

		public void RunFinished(Exception exception)
		{
			// Ignore - we provide our own
		}

		void EventListener.RunFinished(TestResult result)
		{
			if (this.runInParallel)
			{
				foreach (TestRunner runner in this.runners)
				{
					if (runner.Running)
						return;
				}

				this.testResult = new TestResult(this.aggregateTest);
				foreach (TestRunner runner in this.runners)
					this.testResult.AddResult(runner.TestResult);

				this.listener.RunFinished(this.TestResult);
			}
		}

		public void RunStarted(string name, int testCount)
		{
			// TODO: We may want to count how many runs are started
			// Ignore - we provide our own
		}

		public void SuiteFinished(TestResult result)
		{
			this.listener.SuiteFinished(result);
		}

		public void SuiteStarted(TestName suiteName)
		{
			this.listener.SuiteStarted(suiteName);
		}

		public void TestFinished(TestResult result)
		{
			this.listener.TestFinished(result);
		}

		public void TestOutput(TestOutput testOutput)
		{
			this.listener.TestOutput(testOutput);
		}

		public void TestStarted(TestName testName)
		{
			this.listener.TestStarted(testName);
		}

		public void UnhandledException(Exception exception)
		{
			this.listener.UnhandledException(exception);
		}

		public virtual void Unload()
		{
			if (this.aggregateTest != null)
				this.Log.Info("Unloading " + Path.GetFileName(this.aggregateTest.TestName.Name));

			if (this.runners != null)
			{
				foreach (TestRunner runner in this.runners)
					runner.Unload();
			}

			this.aggregateTest = null;
			this.Log.Info("Unload complete");
		}

		public virtual void Wait()
		{
			foreach (TestRunner runner in this.runners)
				runner.Wait();
		}

		#endregion
	}

	#endregion

	#region MultipleTestDomainRunner

	/// <summary>
	/// 	Summary description for MultipleTestDomainRunner.
	/// </summary>
	public class MultipleTestDomainRunner : AggregatingTestRunner
	{
		#region Constructors/Destructors

		public MultipleTestDomainRunner()
			: base(0)
		{
		}

		public MultipleTestDomainRunner(int runnerID)
			: base(runnerID)
		{
		}

		#endregion

		#region Methods/Operators

		protected override TestRunner CreateRunner(int runnerID)
		{
			return new TestDomain(runnerID);
		}

		#endregion
	}

	#endregion

	#region MultipleTestProcessRunner

#if CLR_2_0 || CLR_4_0
	public class MultipleTestProcessRunner : AggregatingTestRunner
	{
		#region Constructors/Destructors

		public MultipleTestProcessRunner()
			: base(0)
		{
		}

		public MultipleTestProcessRunner(int runnerID)
			: base(runnerID)
		{
		}

		#endregion

		#region Methods/Operators

		protected override TestRunner CreateRunner(int runnerID)
		{
			return new ProcessRunner(runnerID);
		}

		#endregion
	}
#endif

	#endregion
}