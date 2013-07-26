// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// 	The ITestLoader interface supports the loading and running
	/// 	of tests in a remote domain.
	/// </summary>
	public interface ITestLoader
	{
		// See if a project is loaded

		#region Properties/Indexers/Events

		bool IsProjectLoaded
		{
			get;
		}

		// See if a test has been loaded from the project
		bool IsTestLoaded
		{
			get;
		}

		// See if a test is running
		bool Running
		{
			get;
		}

		// The loaded test project

		string TestFileName
		{
			get;
		}

		NUnitProject TestProject
		{
			get;
		}

		// Our last test results
		TestResult TestResult
		{
			get;
		}

		#endregion

		#region Methods/Operators

		void CancelTestRun();

		// Create a new empty project using a default name

		// Load a project given a filename
		void LoadProject(string filename);

		// Load a project given a filename and config
		void LoadProject(string filename, string configname);

		// Load a project given an array of assemblies
		void LoadProject(string[] assemblies);

		// Unload current project

		// Load tests for current project and config
		void LoadTest();

		// Load a specific test for current project and config
		void LoadTest(string testName);

		void NewProject();

		// Create a new project given a filename
		void NewProject(string filename);

		// Unload current test

		// Reload current test
		void ReloadTest();

		// Run the tests
		void RunTests(ITestFilter filter);

		void UnloadProject();

		void UnloadTest();

		#endregion

		// Cancel the running test
	}
}