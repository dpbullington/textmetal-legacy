// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Reflection;

using NUnit.Core.Builders;
using NUnit.Core.Extensibility;

namespace NUnit.Core
{
	/// <summary>
	/// CoreExtensions is a singleton class that groups together all
	/// the extension points that are supported in the test domain.
	/// It also provides access to the test builders and decorators
	/// by other parts of the NUnit core.
	/// </summary>
	public class CoreExtensions : ExtensionHost, IService
	{
		#region Constructors/Destructors

		public CoreExtensions()
		{
			this.suiteBuilders = new SuiteBuilderCollection(this);
			this.testBuilders = new TestCaseBuilderCollection(this);
			this.testDecorators = new TestDecoratorCollection(this);
			this.listeners = new EventListenerCollection(this);
			this.frameworks = new FrameworkRegistry(this);
			this.testcaseProviders = new TestCaseProviders(this);
			this.dataPointProviders = new DataPointProviders(this);

			this.extensions = new ArrayList();
			this.extensions.Add(this.suiteBuilders);
			this.extensions.Add(this.testBuilders);
			this.extensions.Add(this.testDecorators);
			this.extensions.Add(this.listeners);
			this.extensions.Add(this.frameworks);
			this.extensions.Add(this.testcaseProviders);
			this.extensions.Add(this.dataPointProviders);

			this.supportedTypes = ExtensionType.Core;

			// TODO: This should be somewhere central
//			string logfile = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
//			logfile = Path.Combine( logfile, "NUnit" );
//			logfile = Path.Combine( logfile, "NUnitTest.log" );
//
//			appender = new log4net.Appender.ConsoleAppender();
////			appender.File = logfile;
////			appender.AppendToFile = true;
////			appender.LockingModel = new log4net.Appender.FileAppender.MinimalLock();
//			appender.Layout = new log4net.Layout.PatternLayout(
//				"%date{ABSOLUTE} %-5level [%4thread] %logger{1}: PID=%property{PID} %message%newline" );
//			appender.Threshold = log4net.Core.Level.All;
//			log4net.Config.BasicConfigurator.Configure(appender);
		}

		#endregion

		#region Fields/Constants

		private static CoreExtensions host;

		private static Logger log = InternalTrace.GetLogger("CoreExtensions");

		private IAddinRegistry addinRegistry;
		private DataPointProviders dataPointProviders;
		private FrameworkRegistry frameworks;
		private bool initialized;
		private EventListenerCollection listeners;

		private SuiteBuilderCollection suiteBuilders;
		private TestCaseBuilderCollection testBuilders;
		private TestDecoratorCollection testDecorators;
		private TestCaseProviders testcaseProviders;

		#endregion

		#region Properties/Indexers/Events

		public static CoreExtensions Host
		{
			get
			{
				if (host == null)
					host = new CoreExtensions();

				return host;
			}
		}

		/// <summary>
		/// Our AddinRegistry may be set from outside or passed into the domain
		/// </summary>
		public IAddinRegistry AddinRegistry
		{
			get
			{
				if (this.addinRegistry == null)
					this.addinRegistry = AppDomain.CurrentDomain.GetData("AddinRegistry") as IAddinRegistry;

				return this.addinRegistry;
			}
			set
			{
				this.addinRegistry = value;
			}
		}

		public bool Initialized
		{
			get
			{
				return this.initialized;
			}
		}

		internal EventListener Listeners
		{
			get
			{
				return this.listeners;
			}
		}

		internal ISuiteBuilder SuiteBuilders
		{
			get
			{
				return this.suiteBuilders;
			}
		}

		internal ITestCaseBuilder2 TestBuilders
		{
			get
			{
				return this.testBuilders;
			}
		}

		internal TestCaseProviders TestCaseProviders
		{
			get
			{
				return this.testcaseProviders;
			}
		}

		internal ITestDecorator TestDecorators
		{
			get
			{
				return this.testDecorators;
			}
		}

		internal FrameworkRegistry TestFrameworks
		{
			get
			{
				return this.frameworks;
			}
		}

		#endregion

		#region Methods/Operators

		public void InitializeService()
		{
			this.InstallBuiltins();
			this.InstallAddins();

			this.initialized = true;
		}

		private bool InstallAddin(Type type)
		{
			ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
			object obj = ctor.Invoke(new object[0]);
			IAddin theAddin = (IAddin)obj;

			return theAddin.Install(this);
		}

		public void InstallAddins()
		{
			log.Info("Installing Addins");

			if (this.AddinRegistry != null)
			{
				foreach (Addin addin in this.AddinRegistry.Addins)
				{
					if ((this.ExtensionTypes & addin.ExtensionType) != 0)
					{
						AddinStatus status = AddinStatus.Unknown;
						string message = null;

						try
						{
							Type type = Type.GetType(addin.TypeName);
							if (type == null)
							{
								status = AddinStatus.Error;
								message = string.Format("Unable to locate {0} Type", addin.TypeName);
							}
							else if (!this.InstallAddin(type))
							{
								status = AddinStatus.Error;
								message = "Install method returned false";
							}
							else
								status = AddinStatus.Loaded;
						}
						catch (Exception ex)
						{
							status = AddinStatus.Error;
							message = ex.ToString();
						}

						this.AddinRegistry.SetStatus(addin.Name, status, message);
						if (status != AddinStatus.Loaded)
						{
							log.Error("Failed to load {0}", addin.Name);
							log.Error(message);
						}
					}
				}
			}
		}

		public void InstallAdhocExtensions(Assembly assembly)
		{
			foreach (Type type in assembly.GetExportedTypes())
			{
				if (type.GetCustomAttributes(typeof(NUnitAddinAttribute), false).Length == 1)
					this.InstallAddin(type);
			}
		}

		public void InstallBuiltins()
		{
			log.Info("Installing Builtins");

			// Define NUnit Frameworks
			this.frameworks.Register("NUnit", "nunit.framework");
			this.frameworks.Register("NUnitLite", "NUnitLite");

			// Install builtin SuiteBuilders
			this.suiteBuilders.Install(new NUnitTestFixtureBuilder());
			this.suiteBuilders.Install(new SetUpFixtureBuilder());

			// Install builtin TestCaseBuilder
			this.testBuilders.Install(new NUnitTestCaseBuilder());
			//testBuilders.Install(new TheoryBuilder());

			// Install builtin TestCaseProviders
			this.testcaseProviders.Install(new TestCaseParameterProvider());
			this.testcaseProviders.Install(new TestCaseSourceProvider());
			this.testcaseProviders.Install(new CombinatorialTestCaseProvider());

			// Install builtin DataPointProvider
			this.dataPointProviders.Install(new InlineDataPointProvider());
			this.dataPointProviders.Install(new ValueSourceProvider());
			this.dataPointProviders.Install(new DatapointProvider());
		}

		public void UnloadService()
		{
			// TODO:  Add CoreExtensions.UnloadService implementation
		}

		#endregion
	}
}