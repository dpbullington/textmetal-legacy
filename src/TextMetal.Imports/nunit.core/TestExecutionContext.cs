// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.Threading;

namespace NUnit.Core
{
	/// <summary>
	/// 	Helper class used to save and restore certain static or
	/// 	singleton settings in the environment that affect tests 
	/// 	or which might be changed by the user tests.
	/// 
	/// 	An internal class is used to hold settings and a stack
	/// 	of these objects is pushed and popped as Save and Restore
	/// 	are called.
	/// 
	/// 	Static methods for each setting forward to the internal 
	/// 	object on the top of the stack.
	/// </summary>
	public class TestExecutionContext
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="TestExecutionContext" /> class.
		/// </summary>
		public TestExecutionContext()
		{
			this.prior = null;
			this.tracing = false;
			//this.logging = false;
			this.outWriter = Console.Out;
			this.errorWriter = Console.Error;
			this.traceWriter = null;
			this.logCapture = new Log4NetCapture();
			this.testCaseTimeout = 0;

			this.currentDirectory = Environment.CurrentDirectory;
			this.currentCulture = CultureInfo.CurrentCulture;
			this.currentUICulture = CultureInfo.CurrentUICulture;
			this.currentPrincipal = Thread.CurrentPrincipal;

			this.contextDictionary = new ContextDictionary(this);
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="TestExecutionContext" /> class.
		/// </summary>
		/// <param name="other"> An existing instance of TestExecutionContext. </param>
		public TestExecutionContext(TestExecutionContext other)
		{
			this.prior = other;
			this.tracing = other.tracing;
			//this.logging = other.logging;
			this.outWriter = other.outWriter;
			this.errorWriter = other.errorWriter;
			this.traceWriter = other.traceWriter;
			this.logCapture = other.logCapture;
			this.testCaseTimeout = other.testCaseTimeout;

			this.currentTest = other.currentTest;
			this.currentResult = other.currentResult;
			this.testPackage = other.testPackage;

			this.currentDirectory = Environment.CurrentDirectory;
			this.currentCulture = CultureInfo.CurrentCulture;
			this.currentUICulture = CultureInfo.CurrentUICulture;
			this.currentPrincipal = Thread.CurrentPrincipal;

			this.contextDictionary = new ContextDictionary(this);
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// 	The current context, head of the list of saved contexts.
		/// </summary>
		private static TestExecutionContext current = new TestExecutionContext();

		/// <summary>
		/// 	Context dictionary used to provide test access
		/// 	to this TestExecutionContext
		/// </summary>
		private ContextDictionary contextDictionary;

		/// <summary>
		/// 	The current culture
		/// </summary>
		private CultureInfo currentCulture;

		/// <summary>
		/// 	The current working directory
		/// </summary>
		private string currentDirectory;

		/// <summary>
		/// 	The current Principal.
		/// </summary>
		private IPrincipal currentPrincipal;

		/// <summary>
		/// 	The active TestResult for the current test
		/// </summary>
		private TestResult currentResult;

		/// <summary>
		/// 	The currently executing test
		/// </summary>
		private Test currentTest;

		/// <summary>
		/// 	The current UI culture
		/// </summary>
		private CultureInfo currentUICulture;

		/// <summary>
		/// 	Destination for standard error
		/// </summary>
		private TextWriter errorWriter;

		private Log4NetCapture logCapture;

		//private bool logging;
		/// <summary>
		/// 	Indicates whether logging is enabled
		/// </summary>
		/// <summary>
		/// 	Destination for standard output
		/// </summary>
		private TextWriter outWriter;

		/// <summary>
		/// 	Link to a prior saved context
		/// </summary>
		public TestExecutionContext prior;

		/// <summary>
		/// 	Default timeout for test cases
		/// </summary>
		private int testCaseTimeout;

		/// <summary>
		/// 	The TestPackage being executed
		/// </summary>
		private TestPackage testPackage;

		/// <summary>
		/// 	Destination for Trace output
		/// </summary>
		private TextWriter traceWriter;

		/// <summary>
		/// 	Indicates whether trace is enabled
		/// </summary>
		private bool tracing;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Gets the current context.
		/// </summary>
		/// <value> The current context. </value>
		public static TestExecutionContext CurrentContext
		{
			get
			{
				return current;
			}
		}

		/// <summary>
		/// 	Saves or restores the CurrentCulture
		/// </summary>
		public CultureInfo CurrentCulture
		{
			get
			{
				return this.currentCulture;
			}
			set
			{
				this.currentCulture = value;
				Thread.CurrentThread.CurrentCulture = this.currentCulture;
			}
		}

		/// <summary>
		/// 	Saves and restores the CurrentDirectory
		/// </summary>
		public string CurrentDirectory
		{
			get
			{
				return this.currentDirectory;
			}
			set
			{
				this.currentDirectory = value;
				Environment.CurrentDirectory = this.currentDirectory;
			}
		}

		/// <summary>
		/// 	Gets or sets the current <see cref="IPrincipal" /> for the Thread.
		/// </summary>
		public IPrincipal CurrentPrincipal
		{
			get
			{
				return this.currentPrincipal;
			}
			set
			{
				this.currentPrincipal = value;
				Thread.CurrentPrincipal = this.currentPrincipal;
			}
		}

		/// <summary>
		/// 	Gets or sets the current test result
		/// </summary>
		public TestResult CurrentResult
		{
			get
			{
				return this.currentResult;
			}
			set
			{
				this.currentResult = value;
			}
		}

		/// <summary>
		/// 	Gets or sets the current test
		/// </summary>
		public Test CurrentTest
		{
			get
			{
				return this.currentTest;
			}
			set
			{
				this.currentTest = value;
			}
		}

		/// <summary>
		/// 	Saves or restores the CurrentUICulture
		/// </summary>
		public CultureInfo CurrentUICulture
		{
			get
			{
				return this.currentUICulture;
			}
			set
			{
				this.currentUICulture = value;
				Thread.CurrentThread.CurrentUICulture = this.currentUICulture;
			}
		}

		/// <summary>
		/// 	Controls where Console.Error is directed
		/// </summary>
		public TextWriter Error
		{
			get
			{
				return this.errorWriter;
			}
			set
			{
				if (this.errorWriter != value)
				{
					this.errorWriter = value;
					Console.Error.Flush();
					Console.SetError(this.errorWriter);
				}
			}
		}

		public LoggingThreshold LogLevel
		{
			get
			{
				return this.logCapture.Threshold;
			}
			set
			{
				this.logCapture.Threshold = value;
			}
		}

		/// <summary>
		/// 	Gets or sets the Log writer, which is actually held by a log4net 
		/// 	TextWriterAppender. When first set, the appender will be created
		/// 	and will thereafter send any log events to the writer.
		///  
		/// 	In normal operation, LogWriter is set to an EventListenerTextWriter
		/// 	connected to the EventQueue in the test domain. The events are
		/// 	subsequently captured in the Gui an the output displayed in
		/// 	the Log tab. The application under test does not need to define
		/// 	any additional appenders.
		/// </summary>
		public TextWriter LogWriter
		{
			get
			{
				return this.logCapture.Writer;
			}
			set
			{
				this.logCapture.Writer = value;
			}
		}

		/// <summary>
		/// 	Controls where Console.Out is directed
		/// </summary>
		public TextWriter Out
		{
			get
			{
				return this.outWriter;
			}
			set
			{
				if (this.outWriter != value)
				{
					this.outWriter = value;
					Console.Out.Flush();
					Console.SetOut(this.outWriter);
				}
			}
		}

		/// <summary>
		/// 	Gets or sets the test case timeout value
		/// </summary>
		public int TestCaseTimeout
		{
			get
			{
				return this.testCaseTimeout;
			}
			set
			{
				this.testCaseTimeout = value;
			}
		}

		/// <summary>
		/// 	Gets the test package currently being run
		/// </summary>
		public TestPackage TestPackage
		{
			get
			{
				return this.testPackage;
			}
			set
			{
				this.testPackage = value;
			}
		}

		/// <summary>
		/// 	Controls where Trace output is directed
		/// </summary>
		public TextWriter TraceWriter
		{
			get
			{
				return this.traceWriter;
			}
			set
			{
				if (this.traceWriter != value)
				{
					if (this.traceWriter != null && this.tracing)
						this.StopTracing();

					this.traceWriter = value;

					if (this.traceWriter != null && this.tracing)
						this.StartTracing();
				}
			}
		}

		/// <summary>
		/// 	Controls whether trace and debug output are written
		/// 	to the standard output.
		/// </summary>
		public bool Tracing
		{
			get
			{
				return this.tracing;
			}
			set
			{
				if (this.tracing != value)
				{
					if (this.traceWriter != null && this.tracing)
						this.StopTracing();

					this.tracing = value;

					if (this.traceWriter != null && this.tracing)
						this.StartTracing();
				}
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Restores the last saved context and puts
		/// 	any saved settings back into effect.
		/// </summary>
		public static void Restore()
		{
			current.ReverseChanges();
			current = current.prior;
#if CLR_2_0 || CLR_4_0
			CallContext.LogicalSetData("NUnit.Framework.TestContext", current.contextDictionary);
#else
            CallContext.SetData("NUnit.Framework.TestContext", current.contextDictionary);
#endif
		}

		/// <summary>
		/// 	Saves the old context and makes a fresh one 
		/// 	current without changing any settings.
		/// </summary>
		public static void Save()
		{
			current = new TestExecutionContext(current);
#if CLR_2_0 || CLR_4_0
			CallContext.LogicalSetData("NUnit.Framework.TestContext", current.contextDictionary);
#else
            CallContext.SetData("NUnit.Framework.TestContext", current.contextDictionary);
#endif
		}

		/// <summary>
		/// 	Used to restore settings to their prior
		/// 	values before reverting to a prior context.
		/// </summary>
		public void ReverseChanges()
		{
			if (this.prior == null)
				throw new InvalidOperationException("TestContext: too many Restores");

			this.Tracing = this.prior.Tracing;
			this.Out = this.prior.Out;
			this.Error = this.prior.Error;
			this.CurrentDirectory = this.prior.CurrentDirectory;
			this.CurrentCulture = this.prior.CurrentCulture;
			this.CurrentUICulture = this.prior.CurrentUICulture;
			this.TestCaseTimeout = this.prior.TestCaseTimeout;
			this.CurrentPrincipal = this.prior.CurrentPrincipal;
		}

		private void StartTracing()
		{
			Trace.Listeners.Add(new TextWriterTraceListener(this.traceWriter, "NUnit"));
		}

		private void StopTracing()
		{
			this.traceWriter.Close();
			Trace.Listeners.Remove("NUnit");
		}

		/// <summary>
		/// 	Record any changed values in the current context
		/// </summary>
		public void Update()
		{
			this.currentDirectory = Environment.CurrentDirectory;
			this.currentCulture = CultureInfo.CurrentCulture;
			this.currentUICulture = CultureInfo.CurrentUICulture;
			this.currentPrincipal = Thread.CurrentPrincipal;
		}

		#endregion
	}
}