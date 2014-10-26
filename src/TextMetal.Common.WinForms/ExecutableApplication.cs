/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;

using TextMetal.Common.Core;
using TextMetal.Common.Solder;

namespace TextMetal.Common.WinForms
{
	public abstract class ExecutableApplication : IDisposable /*, ILogicalThreadAffinative*/
	{
		#region Constructors/Destructors

		protected ExecutableApplication()
		{
			Current = this;
		}

		#endregion

		#region Fields/Constants

		private static readonly string EXECUTABLE_APPLICATION_CONTEXT_CURRENT_KEY = typeof(ExecutableApplication).GUID.SafeToString();

		private AssemblyInformation assemblyInformation;
		private bool disposed;

		#endregion

		#region Properties/Indexers/Events

		public static ExecutableApplication Current
		{
			get
			{
				return (ExecutableApplication)ExecutionPathStorage.GetValue(EXECUTABLE_APPLICATION_CONTEXT_CURRENT_KEY);
			}
			set
			{
				ExecutionPathStorage.SetValue(EXECUTABLE_APPLICATION_CONTEXT_CURRENT_KEY, value);
			}
		}

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

		/// <summary>
		/// Gets a value indicating whether the current instance has been disposed.
		/// </summary>
		public bool Disposed
		{
			get
			{
				return this.disposed;
			}
			private set
			{
				this.disposed = value;
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

		protected abstract void DisplayArgumentErrorMessage(IEnumerable<Message> argumentMessages);

		protected abstract void DisplayArgumentMapMessage(IDictionary<string, ArgumentSpec> argumentMap);

		protected abstract void DisplayFailureMessage(Exception exception);

		protected abstract void DisplaySuccessMessage(TimeSpan duration);

		public void Dispose()
		{
			if (this.Disposed)
				return;

			try
			{
				this.OnDispose();
			}
			finally
			{
				Current = null;

				this.Disposed = true;
				GC.SuppressFinalize(this);
			}
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

		protected abstract IDictionary<string, ArgumentSpec> GetArgumentMap();

		protected virtual void OnDispose()
		{
		}

		protected abstract int OnStartup(string[] args, IDictionary<string, IList<object>> arguments);

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			this.ShowNestedExceptionsAndThrowBrickAtProcess(new ApplicationException("OnUnhandledException", e.ExceptionObject as Exception));
		}

		public void ShowNestedExceptionsAndThrowBrickAtProcess(Exception e)
		{
			this.DisplayFailureMessage(e);

			Environment.Exit(-1);
		}

		private int Startup(string[] args)
		{
			int returnCode;
			DateTime start, end;
			TimeSpan duration;
			IDictionary<string, ArgumentSpec> argumentMap;
			IList<Message> argumentValidationMessages;

			IList<string> argumentValues;
			IDictionary<string, IList<string>> arguments;

			IDictionary<string, IList<object>> finalArguments;
			IList<object> finalArgumentValues;
			object finalArgumentValue;

			try
			{
				start = DateTime.UtcNow;

				AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

				if (this.HookUnhandledExceptionEvents)
					AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;

				this.AssemblyInformation = new AssemblyInformation(Assembly.GetEntryAssembly());

				arguments = AppConfig.ParseCommandLineArguments(args);
				argumentMap = this.GetArgumentMap();

				finalArguments = new Dictionary<string, IList<object>>();
				argumentValidationMessages = new List<Message>();

				if ((object)argumentMap != null)
				{
					foreach (string argumentToken in argumentMap.Keys)
					{
						bool argumentExists;
						int argumentValueCount = 0;
						ArgumentSpec argumentSpec;

						if (argumentExists = arguments.TryGetValue(argumentToken, out argumentValues))
							argumentValueCount = argumentValues.Count;

						if (!argumentMap.TryGetValue(argumentToken, out argumentSpec))
							continue;

						if (argumentSpec.Required && !argumentExists)
						{
							argumentValidationMessages.Add(new Message(string.Empty, string.Format("A required argument was not specified: '{0}'.", argumentToken), Severity.Error));
							continue;
						}

						if (argumentSpec.Bounded && argumentValueCount > 1)
						{
							argumentValidationMessages.Add(new Message(string.Empty, string.Format("A bounded argument was specified more than once: '{0}'.", argumentToken), Severity.Error));
							continue;
						}

						if ((object)argumentValues != null)
						{
							finalArgumentValues = new List<object>();

							if ((object)argumentSpec.Type != null)
							{
								foreach (string argumentValue in argumentValues)
								{
									if (!DataType.Instance.TryParse(argumentSpec.Type, argumentValue, out finalArgumentValue))
										argumentValidationMessages.Add(new Message(string.Empty, string.Format("An argument '{0}' value '{1}' was specified that failed to parse to the target type '{2}'.", argumentToken, argumentValue, argumentSpec.Type.FullName), Severity.Error));
									else
										finalArgumentValues.Add(finalArgumentValue);
								}
							}
							else
							{
								foreach (string argumentValue in argumentValues)
									finalArgumentValues.Add(argumentValue);
							}

							finalArguments.Add(argumentToken, finalArgumentValues);
						}
					}
				}

				if (argumentValidationMessages.Any())
				{
					this.DisplayArgumentErrorMessage(argumentValidationMessages);
					this.DisplayArgumentMapMessage(argumentMap);
					returnCode = -1;
				}
				else
					returnCode = this.OnStartup(args, finalArguments);

				end = DateTime.UtcNow;
				duration = end - start;

				this.DisplaySuccessMessage(duration);

				return returnCode;
			}
			finally
			{
				if (this.HookUnhandledExceptionEvents)
					AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;
			}
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

		#region Classes/Structs/Interfaces/Enums/Delegates

		protected class ArgumentSpec
		{
			#region Constructors/Destructors

			public ArgumentSpec(Type type, bool required, bool bounded)
			{
				this.type = type ?? typeof(Object);
				this.required = required;
				this.bounded = bounded;
			}

			#endregion

			#region Fields/Constants

			private readonly bool bounded;
			private readonly bool required;
			private readonly Type type;

			#endregion

			#region Properties/Indexers/Events

			public bool Bounded
			{
				get
				{
					return this.bounded;
				}
			}

			public bool Required
			{
				get
				{
					return this.required;
				}
			}

			public Type Type
			{
				get
				{
					return this.type;
				}
			}

			#endregion
		}

		protected class ArgumentSpec<T> : ArgumentSpec
		{
			#region Constructors/Destructors

			public ArgumentSpec(bool required, bool bounded)
				: base(typeof(T), required, bounded)
			{
			}

			#endregion
		}

		#endregion
	}
}