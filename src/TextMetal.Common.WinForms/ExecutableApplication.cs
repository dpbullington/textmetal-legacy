/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

using TextMetal.Common.Core;

namespace TextMetal.Common.WinForms
{
	public abstract class ExecutableApplication : IDisposable
	{
		#region Constructors/Destructors

		protected ExecutableApplication()
		{
		}

		#endregion

		#region Fields/Constants

		private AssemblyInformation assemblyInformation;

		#endregion

		#region Properties/Indexers/Events

		public AssemblyInformation AssemblyInformation
		{
			get
			{
				return this.assemblyInformation;
			}
			private set
			{
				this.assemblyInformation = value;
			}
		}

		public bool HookUnhandledExceptionEvents
		{
			get
			{
				return !Debugger.IsAttached &&
						AppConfig.GetAppSetting<bool>(string.Format("{0}::HookUnhandledExceptionEvents", this.GetType().Namespace));
			}
		}

		#endregion

		#region Methods/Operators

		protected abstract void DisplayExceptionMessage(string exceptionMessage);

		public virtual void Dispose()
		{
		}

		/// <summary>
		/// The indirect entry point method for this application. Code is wrapped in this method to leverage the 'TryStartup'/'Startup' pattern. This method, if used, wraps the Startup() method in an exception handler. The handler will catch all exceptions and report a full detailed stack trace to the Console.Error stream; -1 is then returned as the exit code. Otherwise, if no exception is thrown, the exit code returned is that which is returned by Startup().
		/// </summary>
		/// <param name="args"> The command line arguments passed from the executing environment. </param>
		/// <returns> The resulting exit code. </returns>
		protected int EntryPoint(string[] args)
		{
			if (this.HookUnhandledExceptionEvents)
				return this.TryStartup(args);
			else
				return this.Startup(args);
		}

		//protected abstract IEnumerable<string> GetRequiredArguments();

		protected abstract int OnStartup(string[] args, IDictionary<string, IList<string>> arguments);

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			this.ShowNestedExceptionsAndThrowBrickAtProcess(new ApplicationException("OnUnhandledException", e.ExceptionObject as Exception));
		}

		public void ShowNestedExceptionsAndThrowBrickAtProcess(Exception e)
		{
			string exceptionMessage;

			exceptionMessage = Reflexion.GetErrors(e, 0);

			this.DisplayExceptionMessage(exceptionMessage);

			Environment.Exit(-1);
		}

		private int Startup(string[] args)
		{
			int returnCode;
			DateTime start, end;
			TimeSpan duration;

			start = DateTime.UtcNow;
			IDictionary<string, IList<string>> arguments;

			AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

			if (this.HookUnhandledExceptionEvents)
				AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;

			this.AssemblyInformation = new AssemblyInformation(Assembly.GetEntryAssembly());
			arguments = AppConfig.ParseCommandLineArguments(args);

			returnCode = this.OnStartup(args, arguments);

			end = DateTime.UtcNow;
			duration = end - start;
			//Console.WriteLine("Operation duration: {0}", duration);

			return returnCode;
		}

		private int TryStartup(string[] args)
		{
			try
			{
				return this.Startup(args);
			}
			catch (Exception ex)
			{
				this.ShowNestedExceptionsAndThrowBrickAtProcess(new ApplicationException("Main", ex));
			}

			return -1;
		}

		#endregion
	}
}