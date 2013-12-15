// ****************************************************************
// Copyright 2002-2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System.Collections;
using System.IO;

using NUnit.Core;

namespace NUnit.Util
{
	using System;

	/// <summary>
	/// TestLoader handles interactions between a test runner and a
	/// client program - typically the user interface - for the
	/// purpose of loading, unloading and running tests.
	/// It implemements the EventListener interface which is used by
	/// the test runner and repackages those events, along with
	/// others as individual events that clients may subscribe to
	/// in collaboration with a TestEventDispatcher helper object.
	/// TestLoader is quite handy for use with a gui client because
	/// of the large number of events it supports. However, it has
	/// no dependencies on ui components and can be used independently.
	/// </summary>
	public class TestLoader : MarshalByRefObject, EventListener, ITestLoader, IService
	{
		#region Constructors/Destructors

		public TestLoader()
			: this(new TestEventDispatcher())
		{
		}

		public TestLoader(TestEventDispatcher eventDispatcher)
			: this(eventDispatcher, new AssemblyWatcher())
		{
		}

		public TestLoader(IAssemblyWatcher assemblyWatcher)
			: this(new TestEventDispatcher(), assemblyWatcher)
		{
		}

		public TestLoader(TestEventDispatcher eventDispatcher, IAssemblyWatcher assemblyWatcher)
		{
			this.events = eventDispatcher;
			this.watcher = assemblyWatcher;
			this.factory = new DefaultTestRunnerFactory();
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(TestLoader));

		/// <summary>
		/// The runtime framework being used for the currently
		/// loaded tests, or the current framework if no tests
		/// are loaded.
		/// </summary>
		private RuntimeFramework currentFramework = RuntimeFramework.CurrentFramework;

		/// <summary>
		/// The currently set runtime framework
		/// </summary>
		private RuntimeFramework currentRuntime;

		/// <summary>
		/// The currently executing test
		/// </summary>
		private string currentTestName;

		/// <summary>
		/// Our event dispatching helper object
		/// </summary>
		private TestEventDispatcher events;

		/// <summary>
		/// Our TestRunnerFactory
		/// </summary>
		private ITestRunnerFactory factory;

		/// <summary>
		/// The last exception received when trying to load, unload or run a test
		/// </summary>
		private Exception lastException = null;

		/// <summary>
		/// The last filter used for a run - used to
		/// rerun tests when a change occurs
		/// </summary>
		private ITestFilter lastFilter;

		/// <summary>
		/// Last logging level used for a run
		/// </summary>
		private LoggingThreshold lastLogLevel;

		/// <summary>
		/// The last trace setting used for a run
		/// </summary>
		private bool lastTracing;

		//private ITest loadedTest = null;
		/// <summary>
		/// The currently loaded test, returned by the testrunner
		/// </summary>
		/// <summary>
		/// The test name that was specified when loading
		/// </summary>
		private string loadedTestName = null;

		/// <summary>
		/// LoggingThreshold to use for running tests
		/// </summary>
		private LoggingThreshold logLevel;

		/// <summary>
		/// Assembly changed during a test and
		/// needs to be reloaded later
		/// </summary>
		private bool reloadPending = false;

		/// <summary>
		/// Our current test project, if we have one.
		/// </summary>
		private NUnitProject testProject = null;

		/// <summary>
		/// Result of the last test run
		/// </summary>
		private TestResult testResult = null;

		/// <summary>
		/// Loads and executes tests. Non-null when
		/// we have loaded a test.
		/// </summary>
		private TestRunner testRunner = null;

		/// <summary>
		/// Trace setting to use for running tests
		/// </summary>
		private bool tracing;

		/// <summary>
		/// Watcher fires when the assembly changes
		/// </summary>
		private IAssemblyWatcher watcher;

		#endregion

		#region Properties/Indexers/Events

		public IList AssemblyInfo
		{
			get
			{
				return this.testRunner == null ? new TestAssemblyInfo[0] : this.testRunner.AssemblyInfo;
			}
		}

		public RuntimeFramework CurrentFramework
		{
			get
			{
				return this.currentFramework;
			}
		}

		public ITestEvents Events
		{
			get
			{
				return this.events;
			}
		}

		public bool IsProjectLoaded
		{
			get
			{
				return this.testProject != null;
			}
		}

		public bool IsTestLoaded
		{
			get
			{
				return this.testRunner != null && this.testRunner.Test != null;
			}
		}

		public bool IsTracingEnabled
		{
			get
			{
				return this.tracing;
			}
			set
			{
				this.tracing = value;
			}
		}

		public Exception LastException
		{
			get
			{
				return this.lastException;
			}
		}

		public ITest LoadedTest
		{
			get
			{
				return this.testRunner == null ? null : this.testRunner.Test;
			}
		}

		public LoggingThreshold LoggingThreshold
		{
			get
			{
				return this.logLevel;
			}
			set
			{
				this.logLevel = value;
			}
		}

		public bool Running
		{
			get
			{
				return this.testRunner != null && this.testRunner.Running;
			}
		}

		public int TestCount
		{
			get
			{
				return this.LoadedTest == null ? 0 : this.LoadedTest.TestCount;
			}
		}

		public string TestFileName
		{
			get
			{
				return this.testProject.ProjectPath;
			}
		}

		public NUnitProject TestProject
		{
			get
			{
				return this.testProject;
			}
		}

		public TestResult TestResult
		{
			get
			{
				return this.testResult;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Return true if the current project can be reloaded under
		/// the specified CLR version.
		/// </summary>
		public bool CanReloadUnderRuntimeVersion(Version version)
		{
			if (!Services.TestAgency.IsRuntimeVersionSupported(version))
				return false;

			if (this.AssemblyInfo.Count == 0)
				return false;

			foreach (TestAssemblyInfo info in this.AssemblyInfo)
			{
				if (info == null || info.ImageRuntimeVersion > version)
					return false;
			}

			return true;
		}

		/// <summary>
		/// Cancel the currently running test.
		/// Fail silently if there is none to
		/// allow for latency in the UI.
		/// </summary>
		public void CancelTestRun()
		{
			if (this.Running)
				this.testRunner.CancelRun();
		}

		public IList GetCategories()
		{
			CategoryManager categoryManager = new CategoryManager();
			categoryManager.AddAllCategories(this.LoadedTest);
			ArrayList list = new ArrayList(categoryManager.Categories);
			list.Sort();
			return list;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public void InitializeService()
		{
			// TODO:  Add TestLoader.InitializeService implementation
		}

		/// <summary>
		/// Install our watcher object so as to get notifications
		/// about changes to a test.
		/// </summary>
		private void InstallWatcher()
		{
			if (this.watcher != null)
			{
				this.watcher.Stop();
				this.watcher.FreeResources();

				this.watcher.Setup(1000, this.TestProject.ActiveConfig.Assemblies.ToArray());
				this.watcher.AssemblyChanged += new AssemblyChangedHandler(this.OnTestChanged);
				this.watcher.Start();
			}
		}

		/// <summary>
		/// Load a new project, optionally selecting the config and fire events
		/// </summary>
		public void LoadProject(string filePath, string configName)
		{
			try
			{
				log.Info("Loading project {0}, {1} config", filePath, configName == null ? "default" : configName);
				this.events.FireProjectLoading(filePath);

				NUnitProject newProject = Services.ProjectService.LoadProject(filePath);
				if (configName != null)
				{
					newProject.SetActiveConfig(configName);
					newProject.IsDirty = false;
				}

				this.OnProjectLoad(newProject);
			}
			catch (Exception exception)
			{
				log.Error("Project load failed", exception);
				this.lastException = exception;
				this.events.FireProjectLoadFailed(filePath, exception);
			}
		}

		/// <summary>
		/// Load a new project using the default config and fire events
		/// </summary>
		public void LoadProject(string filePath)
		{
			this.LoadProject(filePath, null);
		}

		/// <summary>
		/// Load a project from a list of assemblies and fire events
		/// </summary>
		public void LoadProject(string[] assemblies)
		{
			try
			{
				log.Info("Loading multiple assemblies as new project");
				this.events.FireProjectLoading("New Project");

				NUnitProject newProject = Services.ProjectService.WrapAssemblies(assemblies);

				this.OnProjectLoad(newProject);
			}
			catch (Exception exception)
			{
				log.Error("Project load failed", exception);
				this.lastException = exception;
				this.events.FireProjectLoadFailed("New Project", exception);
			}
		}

		public void LoadTest()
		{
			this.LoadTest(null);
		}

		public void LoadTest(string testName)
		{
			log.Info("Loading tests for " + Path.GetFileName(this.TestFileName));

			long startTime = DateTime.Now.Ticks;

			try
			{
				this.events.FireTestLoading(this.TestFileName);

				TestPackage package = this.MakeTestPackage(testName);
				if (this.testRunner != null)
					this.testRunner.Dispose();
				this.testRunner = this.factory.MakeTestRunner(package);

				bool loaded = this.testRunner.Load(package);

				this.loadedTestName = testName;
				this.testResult = null;
				this.reloadPending = false;

				if (Services.UserSettings.GetSetting("Options.TestLoader.ReloadOnChange", true))
					this.InstallWatcher();

				if (loaded)
				{
					this.currentFramework = package.Settings.Contains("RuntimeFramework")
						? package.Settings["RuntimeFramework"] as RuntimeFramework
						: RuntimeFramework.CurrentFramework;

					this.testProject.HasChangesRequiringReload = false;
					this.events.FireTestLoaded(this.TestFileName, this.LoadedTest);
				}
				else
				{
					this.lastException = new ApplicationException(string.Format("Unable to find test {0} in assembly", testName));
					this.events.FireTestLoadFailed(this.TestFileName, this.lastException);
				}
			}
			catch (FileNotFoundException exception)
			{
				log.Error("File not found", exception);
				this.lastException = exception;

				foreach (string assembly in this.TestProject.ActiveConfig.Assemblies)
				{
					if (Path.GetFileNameWithoutExtension(assembly) == exception.FileName &&
						!PathUtils.SamePathOrUnder(this.testProject.ActiveConfig.BasePath, assembly))
					{
						this.lastException = new ApplicationException(string.Format("Unable to load {0} because it is not located under the AppBase", exception.FileName), exception);
						break;
					}
				}

				this.events.FireTestLoadFailed(this.TestFileName, this.lastException);

				double loadTime = (double)(DateTime.Now.Ticks - startTime) / (double)TimeSpan.TicksPerSecond;
				log.Info("Load completed in {0} seconds", loadTime);
			}
			catch (Exception exception)
			{
				log.Error("Failed to load test", exception);

				this.lastException = exception;
				this.events.FireTestLoadFailed(this.TestFileName, exception);
			}
		}

		private TestPackage MakeTestPackage(string testName)
		{
			TestPackage package = this.TestProject.ActiveConfig.MakeTestPackage();
			package.TestName = testName;

			ISettings userSettings = Services.UserSettings;
			package.Settings["MergeAssemblies"] = userSettings.GetSetting("Options.TestLoader.MergeAssemblies", false);
			package.Settings["AutoNamespaceSuites"] = userSettings.GetSetting("Options.TestLoader.AutoNamespaceSuites", true);
			package.Settings["ShadowCopyFiles"] = userSettings.GetSetting("Options.TestLoader.ShadowCopyFiles", true);

			ProcessModel processModel = (ProcessModel)userSettings.GetSetting("Options.TestLoader.ProcessModel", ProcessModel.Default);
			DomainUsage domainUsage = (DomainUsage)userSettings.GetSetting("Options.TestLoader.DomainUsage", DomainUsage.Default);

			if (processModel != ProcessModel.Default && // Ignore default setting
				!package.Settings.Contains("ProcessModel")) // Ignore global setting if package has a setting
				package.Settings["ProcessModel"] = processModel;

			// NOTE: This code ignores DomainUsage.None because TestLoader
			// is only called from the GUI and the GUI can't support that setting.
			// TODO: Move this logic to the GUI if TestLoader is used more widely
			if (domainUsage != DomainUsage.Default && // Ignore default setting
				domainUsage != DomainUsage.None && // Ignore DomainUsage.None in Gui
				(processModel != ProcessModel.Multiple ||
				domainUsage != DomainUsage.Multiple) && // Both process and domain may not be multiple
				!package.Settings.Contains("DomainUsage")) // Ignore global setting if package has a setting
				package.Settings["DomainUsage"] = domainUsage;

			if (!package.Settings.Contains("WorkDirectory"))
				package.Settings["WorkDirectory"] = Environment.CurrentDirectory;

			//if (NUnitConfiguration.ApartmentState != System.Threading.ApartmentState.Unknown)
			//    package.Settings["ApartmentState"] = NUnitConfiguration.ApartmentState;

			return package;
		}

		/// <summary>
		/// Create a new project with default naming
		/// </summary>
		public void NewProject()
		{
			log.Info("Creating empty project");
			try
			{
				this.events.FireProjectLoading("New Project");

				this.OnProjectLoad(Services.ProjectService.NewProject());
			}
			catch (Exception exception)
			{
				log.Error("Project creation failed", exception);
				this.lastException = exception;
				this.events.FireProjectLoadFailed("New Project", exception);
			}
		}

		/// <summary>
		/// Create a new project using a given path
		/// </summary>
		public void NewProject(string filePath)
		{
			log.Info("Creating project " + filePath);
			try
			{
				this.events.FireProjectLoading(filePath);

				NUnitProject project = new NUnitProject(filePath);

				project.Configs.Add("Debug");
				project.Configs.Add("Release");
				project.IsDirty = false;

				this.OnProjectLoad(project);
			}
			catch (Exception exception)
			{
				log.Error("Project creation failed", exception);
				this.lastException = exception;
				this.events.FireProjectLoadFailed(filePath, exception);
			}
		}

		/// <summary>
		/// Common operations done each time a project is loaded
		/// </summary>
		/// <param name="testProject"> The newly loaded project </param>
		private void OnProjectLoad(NUnitProject testProject)
		{
			if (this.IsProjectLoaded)
				this.UnloadProject();

			this.testProject = testProject;

			this.events.FireProjectLoaded(this.TestFileName);
		}

		/// <summary>
		/// Handle watcher event that signals when the loaded assembly
		/// file has changed. Make sure it's a real change before
		/// firing the SuiteChangedEvent. Since this all happens
		/// asynchronously, we use an event to let ui components
		/// know that the failure happened.
		/// </summary>
		public void OnTestChanged(string testFileName)
		{
			log.Info("Assembly changed: {0}", testFileName);

			if (this.Running)
				this.reloadPending = true;
			else
			{
				this.ReloadTest();

				if (this.lastFilter != null && Services.UserSettings.GetSetting("Options.TestLoader.RerunOnChange", false))
					this.testRunner.BeginRun(this, this.lastFilter, this.lastTracing, this.lastLogLevel);
			}
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			switch (args.ExceptionObject.GetType().FullName)
			{
				case "System.Threading.ThreadAbortException":
					break;
				case "NUnit.Framework.AssertionException":
				default:
					Exception ex = args.ExceptionObject as Exception;
					this.events.FireTestException(this.currentTestName, ex);
					break;
			}
		}

		/// <summary>
		/// Reload the current test on command
		/// </summary>
		public void ReloadTest(RuntimeFramework framework)
		{
			log.Info("Reloading tests for " + Path.GetFileName(this.TestFileName));
			try
			{
				this.events.FireTestReloading(this.TestFileName);

				TestPackage package = this.MakeTestPackage(this.loadedTestName);
				if (framework != null)
					package.Settings["RuntimeFramework"] = framework;

				this.RemoveWatcher();

				this.testRunner.Unload();
				if (!this.factory.CanReuse(this.testRunner, package))
				{
					this.testRunner.Dispose();
					this.testRunner = this.factory.MakeTestRunner(package);
				}

				if (this.testRunner.Load(package))
				{
					this.currentFramework = package.Settings.Contains("RuntimeFramework")
						? package.Settings["RuntimeFramework"] as RuntimeFramework
						: RuntimeFramework.CurrentFramework;
				}

				this.currentRuntime = framework;
				this.reloadPending = false;

				if (Services.UserSettings.GetSetting("Options.TestLoader.ReloadOnChange", true))
					this.InstallWatcher();

				this.testProject.HasChangesRequiringReload = false;
				this.events.FireTestReloaded(this.TestFileName, this.LoadedTest);

				log.Info("Reload complete");
			}
			catch (Exception exception)
			{
				log.Error("Reload failed", exception);
				this.lastException = exception;
				this.events.FireTestReloadFailed(this.TestFileName, exception);
			}
		}

		public void ReloadTest()
		{
			this.ReloadTest(this.currentRuntime);
		}

		/// <summary>
		/// Stop and remove our current watcher object.
		/// </summary>
		private void RemoveWatcher()
		{
			if (this.watcher != null)
			{
				this.watcher.Stop();
				this.watcher.FreeResources();
				this.watcher.AssemblyChanged -= new AssemblyChangedHandler(this.OnTestChanged);
			}
		}

		public void RunFinished(TestResult testResult)
		{
			this.testResult = testResult;

			this.events.FireRunFinished(testResult);
		}

		public void RunFinished(Exception exception)
		{
			this.lastException = exception;
			this.events.FireRunFinished(exception);
		}

		public void RunStarted(string name, int testCount)
		{
			log.Debug("Got RunStarted Event");
			this.events.FireRunStarting(name, testCount);
		}

		/// <summary>
		/// Run selected tests using a filter
		/// </summary>
		/// <param name="filter"> The filter to be used </param>
		public void RunTests(ITestFilter filter)
		{
			if (!this.Running && this.LoadedTest != null)
			{
				if (this.reloadPending || Services.UserSettings.GetSetting("Options.TestLoader.ReloadOnRun", false))
					this.ReloadTest();

				// Save args for automatic rerun
				this.lastFilter = filter;
				this.lastTracing = this.tracing;
				this.lastLogLevel = this.logLevel;

				this.testRunner.BeginRun(this, filter, this.tracing, this.logLevel);
			}
		}

		public void SaveLastResult(string fileName)
		{
			new XmlResultWriter(fileName).SaveTestResult(this.testResult);
		}

		/// <summary>
		/// Trigger event when each suite finishes
		/// </summary>
		/// <param name="result"> Result of the suite that finished </param>
		public void SuiteFinished(TestResult result)
		{
			this.events.FireSuiteFinished(result);
		}

		/// <summary>
		/// Trigger event when each suite starts
		/// </summary>
		/// <param name="suite"> Suite that is starting </param>
		public void SuiteStarted(TestName suiteName)
		{
			this.events.FireSuiteStarting(suiteName);
		}

		/// <summary>
		/// Trigger event when each test finishes
		/// </summary>
		/// <param name="result"> Result of the case that finished </param>
		public void TestFinished(TestResult result)
		{
			this.events.FireTestFinished(result);
		}

		/// <summary>
		/// Trigger event when output occurs during a test
		/// </summary>
		/// <param name="testOutput"> The test output </param>
		public void TestOutput(TestOutput testOutput)
		{
			this.events.FireTestOutput(testOutput);
		}

		/// <summary>
		/// Trigger event when each test starts
		/// </summary>
		/// <param name="testName"> TestName of the Test that is starting </param>
		public void TestStarted(TestName testName)
		{
			this.currentTestName = testName.FullName;
			this.events.FireTestStarting(testName);
		}

		/// <summary>
		/// Trigger event when an unhandled exception (other than ThreadAbordException) occurs during a test
		/// </summary>
		/// <param name="exception"> The unhandled exception </param>
		public void UnhandledException(Exception exception)
		{
			this.events.FireTestException(this.currentTestName, exception);
		}

		/// <summary>
		/// Unload the current project and fire events
		/// </summary>
		public void UnloadProject()
		{
			string testFileName = this.TestFileName;

			log.Info("Unloading project " + testFileName);
			try
			{
				this.events.FireProjectUnloading(testFileName);

				if (this.IsTestLoaded)
					this.UnloadTest();

				this.testProject = null;

				this.events.FireProjectUnloaded(testFileName);
			}
			catch (Exception exception)
			{
				log.Error("Project unload failed", exception);
				this.lastException = exception;
				this.events.FireProjectUnloadFailed(testFileName, exception);
			}
		}

		public void UnloadService()
		{
			// TODO:  Add TestLoader.UnloadService implementation
		}

		/// <summary>
		/// Unload the current test suite and fire the Unloaded event
		/// </summary>
		public void UnloadTest()
		{
			if (this.IsTestLoaded)
			{
				log.Info("Unloading tests for " + Path.GetFileName(this.TestFileName));

				// Hold the name for notifications after unload
				string fileName = this.TestFileName;

				try
				{
					this.events.FireTestUnloading(fileName);

					this.RemoveWatcher();

					this.testRunner.Unload();
					this.testRunner.Dispose();
					this.testRunner = null;

					this.loadedTestName = null;
					this.testResult = null;
					this.reloadPending = false;

					this.events.FireTestUnloaded(fileName);
					log.Info("Unload complete");
				}
				catch (Exception exception)
				{
					log.Error("Failed to unload tests", exception);
					this.lastException = exception;
					this.events.FireTestUnloadFailed(fileName, exception);
				}
			}
		}

		#endregion
	}
}