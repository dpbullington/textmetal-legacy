// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System.Collections;

namespace NUnit.Core
{
	using System;

	/// <summary>
	/// DelegatingTestRUnner is the abstract base for core TestRunner
	/// implementations that operate by controlling a downstream
	/// TestRunner. All calls are simply passed on to the
	/// TestRunner that is provided to the constructor.
	/// Although the class is abstract, it has no abstract
	/// methods specified because each implementation will
	/// need to override different methods. All methods are
	/// specified using interface syntax and the derived class
	/// must explicitly implement TestRunner in order to
	/// redefine the selected methods.
	/// </summary>
	public abstract class ProxyTestRunner : MarshalByRefObject, TestRunner
	{
		#region Constructors/Destructors

		public ProxyTestRunner(TestRunner testRunner)
		{
			this.testRunner = testRunner;
			this.runnerID = testRunner.ID;
		}

		/// <summary>
		/// Protected constructor for runners that delay creation
		/// of their downstream runner.
		/// </summary>
		protected ProxyTestRunner(int runnerID)
		{
			this.runnerID = runnerID;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// The event listener for the currently running test
		/// </summary>
		protected EventListener listener;

		/// <summary>
		/// Our runner ID
		/// </summary>
		protected int runnerID;

		/// <summary>
		/// The downstream TestRunner
		/// </summary>
		private TestRunner testRunner;

		#endregion

		#region Properties/Indexers/Events

		public virtual IList AssemblyInfo
		{
			get
			{
				return this.testRunner == null ? null : this.testRunner.AssemblyInfo;
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
				return this.testRunner != null && this.testRunner.Running;
			}
		}

		public virtual ITest Test
		{
			get
			{
				return this.testRunner == null ? null : this.testRunner.Test;
			}
		}

		public virtual TestResult TestResult
		{
			get
			{
				return this.testRunner == null ? null : this.testRunner.TestResult;
			}
		}

		/// <summary>
		/// Protected property copies any settings to the downstream test runner
		/// when it is set. Derived runners overriding this should call the base
		/// or copy the settings themselves.
		/// </summary>
		protected virtual TestRunner TestRunner
		{
			get
			{
				return this.testRunner;
			}
			set
			{
				this.testRunner = value;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void BeginRun(EventListener listener, ITestFilter filter, bool tracing, LoggingThreshold logLevel)
		{
			// Save active listener for derived classes
			this.listener = listener;
			this.testRunner.BeginRun(listener, filter, tracing, logLevel);
		}

		public virtual void CancelRun()
		{
			this.testRunner.CancelRun();
		}

		public virtual int CountTestCases(ITestFilter filter)
		{
			return this.testRunner.CountTestCases(filter);
		}

		public virtual void Dispose()
		{
			if (this.testRunner != null)
				this.testRunner.Dispose();
		}

		public virtual TestResult EndRun()
		{
			return this.testRunner.EndRun();
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public virtual bool Load(TestPackage package)
		{
			return this.testRunner.Load(package);
		}

		public virtual TestResult Run(EventListener listener, ITestFilter filter, bool tracing, LoggingThreshold logLevel)
		{
			// Save active listener for derived classes
			this.listener = listener;
			return this.testRunner.Run(listener, filter, tracing, logLevel);
		}

		public virtual void Unload()
		{
			if (this.testRunner != null)
				this.testRunner.Unload();
		}

		public virtual void Wait()
		{
			this.testRunner.Wait();
		}

		#endregion
	}
}