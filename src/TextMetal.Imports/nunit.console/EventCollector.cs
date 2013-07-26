// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

using NUnit.Core;
using NUnit.Util;

namespace NUnit.ConsoleRunner
{
	/// <summary>
	/// 	Summary description for EventCollector.
	/// </summary>
	public class EventCollector : MarshalByRefObject, EventListener
	{
		#region Constructors/Destructors

		public EventCollector(ConsoleOptions options, TextWriter outWriter, TextWriter errorWriter)
		{
			this.level = 0;
			this.options = options;
			this.outWriter = outWriter;
			this.errorWriter = errorWriter;
			this.currentTestName = string.Empty;
			this.progress = !options.xmlConsole && !options.labels && !options.nodots;

			AppDomain.CurrentDomain.UnhandledException +=
				new UnhandledExceptionEventHandler(this.OnUnhandledException);
		}

		#endregion

		#region Fields/Constants

		private string currentTestName;
		private TextWriter errorWriter;

		private int failureCount;
		private int level;
		private StringCollection messages;

		private ConsoleOptions options;
		private TextWriter outWriter;

		private bool progress = false;
		private int testIgnoreCount;
		private int testRunCount;

		private ArrayList unhandledExceptions = new ArrayList();

		#endregion

		#region Properties/Indexers/Events

		public bool HasExceptions
		{
			get
			{
				return this.unhandledExceptions.Count > 0;
			}
		}

		#endregion

		#region Methods/Operators

		public override object InitializeLifetimeService()
		{
			return null;
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject.GetType() != typeof(ThreadAbortException))
				this.UnhandledException((Exception)e.ExceptionObject);
		}

		public void RunFinished(TestResult result)
		{
		}

		public void RunFinished(Exception exception)
		{
		}

		public void RunStarted(string name, int testCount)
		{
		}

		public void SuiteFinished(TestResult suiteResult)
		{
			if (--this.level == 0)
			{
				Trace.WriteLine("############################################################################");

				if (this.messages.Count == 0)
					Trace.WriteLine("##############                 S U C C E S S               #################");
				else
				{
					Trace.WriteLine("##############                F A I L U R E S              #################");

					foreach (string s in this.messages)
						Trace.WriteLine(s);
				}

				Trace.WriteLine("############################################################################");
				Trace.WriteLine("Executed tests       : " + this.testRunCount);
				Trace.WriteLine("Ignored tests        : " + this.testIgnoreCount);
				Trace.WriteLine("Failed tests         : " + this.failureCount);
				Trace.WriteLine("Unhandled exceptions : " + this.unhandledExceptions.Count);
				Trace.WriteLine("Total time           : " + suiteResult.Time + " seconds");
				Trace.WriteLine("############################################################################");
			}
		}

		public void SuiteStarted(TestName testName)
		{
			if (this.level++ == 0)
			{
				this.messages = new StringCollection();
				this.testRunCount = 0;
				this.testIgnoreCount = 0;
				this.failureCount = 0;
				Trace.WriteLine("################################ UNIT TESTS ################################");
				Trace.WriteLine("Running tests in '" + testName.FullName + "'...");
			}
		}

		public void TestFinished(TestResult testResult)
		{
			switch (testResult.ResultState)
			{
				case ResultState.Error:
				case ResultState.Failure:
				case ResultState.Cancelled:
					this.testRunCount++;
					this.failureCount++;

					if (this.progress)
						Console.Write("F");

					this.messages.Add(string.Format("{0}) {1} :", this.failureCount, testResult.Test.TestName.FullName));
					this.messages.Add(testResult.Message.Trim(Environment.NewLine.ToCharArray()));

					string stackTrace = StackTraceFilter.Filter(testResult.StackTrace);
					if (stackTrace != null && stackTrace != string.Empty)
					{
						string[] trace = stackTrace.Split(Environment.NewLine.ToCharArray());
						foreach (string s in trace)
						{
							if (s != string.Empty)
							{
								string link = Regex.Replace(s.Trim(), @".* in (.*):line (.*)", "$1($2)");
								this.messages.Add(string.Format("at\n{0}", link));
							}
						}
					}
					break;

				case ResultState.Inconclusive:
				case ResultState.Success:
					this.testRunCount++;
					break;

				case ResultState.Ignored:
				case ResultState.Skipped:
				case ResultState.NotRunnable:
					this.testIgnoreCount++;

					if (this.progress)
						Console.Write("N");
					break;
			}

			this.currentTestName = string.Empty;
		}

		public void TestOutput(TestOutput output)
		{
			switch (output.Type)
			{
				case TestOutputType.Out:
					this.outWriter.Write(output.Text);
					break;
				case TestOutputType.Error:
					this.errorWriter.Write(output.Text);
					break;
			}
		}

		public void TestStarted(TestName testName)
		{
			this.currentTestName = testName.FullName;

			if (this.options.labels)
				this.outWriter.WriteLine("***** {0}", this.currentTestName);

			if (this.progress)
				Console.Write(".");
		}

		public void UnhandledException(Exception exception)
		{
			// If we do labels, we already have a newline
			this.unhandledExceptions.Add(this.currentTestName + " : " + exception.ToString());
			//if (!options.labels) outWriter.WriteLine();
			string msg = string.Format("##### Unhandled Exception while running {0}", this.currentTestName);
			//outWriter.WriteLine(msg);
			//outWriter.WriteLine(exception.ToString());

			Trace.WriteLine(msg);
			Trace.WriteLine(exception.ToString());
		}

		public void WriteExceptions()
		{
			Console.WriteLine();
			Console.WriteLine("Unhandled exceptions:");
			int index = 1;
			foreach (string msg in this.unhandledExceptions)
				Console.WriteLine("{0}) {1}", index++, msg);
		}

		#endregion
	}
}