// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;

namespace NUnit.Util
{
	/// <summary>
	/// ITestEvents interface defines events related to loading
	/// and unloading of test projects and loading, unloading and
	/// running tests.
	/// </summary>
	public interface ITestEvents
	{
		// Events related to the loading and unloading
		// of projects - including wrapper projects
		// created in order to load assemblies. This
		// occurs separately from the loading of tests
		// for the assemblies in the project.

		#region Properties/Indexers/Events

		event TestEventHandler ProjectLoadFailed;
		event TestEventHandler ProjectLoaded;
		event TestEventHandler ProjectLoading;
		event TestEventHandler ProjectUnloadFailed;
		event TestEventHandler ProjectUnloaded;
		event TestEventHandler ProjectUnloading;

		// Events related to loading and unloading tests.
		event TestEventHandler RunFinished;
		event TestEventHandler RunStarting;

		// Events that arise while a test is running
		// These are translated from calls to the runner on the
		// EventListener interface.
		event TestEventHandler SuiteFinished;
		event TestEventHandler SuiteStarting;

		/// <summary>
		/// An unhandled exception was thrown during a test run,
		/// and it cannot be associated with a particular test failure.
		/// </summary>
		event TestEventHandler TestException;

		event TestEventHandler TestFinished;
		event TestEventHandler TestLoadFailed;
		event TestEventHandler TestLoaded;
		event TestEventHandler TestLoading;

		/// <summary>
		/// Console Out/Error
		/// </summary>
		event TestEventHandler TestOutput;

		event TestEventHandler TestReloadFailed;
		event TestEventHandler TestReloaded;
		event TestEventHandler TestReloading;
		event TestEventHandler TestStarting;
		event TestEventHandler TestUnloadFailed;
		event TestEventHandler TestUnloaded;
		event TestEventHandler TestUnloading;

		#endregion
	}
}