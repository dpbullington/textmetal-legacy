/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;

using CompiledCode = System.String;

namespace TextMetal.Framework.Expression
{
	internal sealed class PowerShellHost : PSHost
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the PowerShellHost class.
		/// </summary>
		public PowerShellHost()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly Guid instanceId = Guid.NewGuid();
		private readonly CultureInfo originalCultureInfo = Thread.CurrentThread.CurrentCulture;
		private readonly CultureInfo originalUICultureInfo = Thread.CurrentThread.CurrentUICulture;
		private readonly IDictionary<object, CompiledCode> scriptCompilations = new Dictionary<object, CompiledCode>();

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Return the culture information to use. This implementation returns a snapshot of the culture information of the thread that created this object.
		/// </summary>
		public override CultureInfo CurrentCulture
		{
			get
			{
				return this.originalCultureInfo;
			}
		}

		/// <summary>
		/// Return the UI culture information to use. This implementation returns a snapshot of the UI culture information of the thread that created this object.
		/// </summary>
		public override CultureInfo CurrentUICulture
		{
			get
			{
				return this.originalUICultureInfo;
			}
		}

		/// <summary>
		/// This implementation always returns the GUID allocated at instantiation time.
		/// </summary>
		public override Guid InstanceId
		{
			get
			{
				return this.instanceId;
			}
		}

		/// <summary>
		/// Return a string that contains the name of the host implementation. Keep in mind that this string may be used by script writers to identify when your host is being used.
		/// </summary>
		public override string Name
		{
			get
			{
				return "TextMetalHost";
			}
		}

		private IDictionary<object, CompiledCode> ScriptCompilations
		{
			get
			{
				return this.scriptCompilations;
			}
		}

		/// <summary>
		/// This sample does not implement a PSHostUserInterface component so this property simply returns null.
		/// </summary>
		public override PSHostUserInterface UI
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Return the version object for this application. Typically this should match the version resource in the application.
		/// </summary>
		public override Version Version
		{
			get
			{
				return new Version(1, 0, 0, 0);
			}
		}

		#endregion

		#region Methods/Operators

		public bool Compile(object scriptHandle, string scriptContent)
		{
			CompiledCode compiledCode;

			if ((object)scriptHandle == null)
				throw new ArgumentNullException("scriptHandle");

			if ((object)scriptContent == null)
				throw new ArgumentNullException("scriptContent");

			compiledCode = scriptContent;

			if (this.ScriptCompilations.ContainsKey(scriptHandle))
				return false;

			this.ScriptCompilations.Add(scriptHandle, compiledCode);

			return true;
		}

		/// <summary>
		/// Not implemented by this example class. The call fails with a NotImplementedException exception.
		/// </summary>
		public override void EnterNestedPrompt()
		{
		}

		public object Execute(object scriptHandle, IDictionary<string, object> scriptVariables)
		{
			CompiledCode compiledCode;
			PowerShellHost powerShellHost;
			Collection<PSObject> psObjects;
			object returnValue;

			if ((object)scriptHandle == null)
				throw new ArgumentNullException("scriptHandle");

			if ((object)scriptVariables == null)
				throw new ArgumentNullException("scriptVariables");

			if (!this.ScriptCompilations.TryGetValue(scriptHandle, out compiledCode))
				throw new InvalidOperationException(string.Format("'{0}'", scriptHandle));

			powerShellHost = new PowerShellHost();

			using (Runspace runspace = RunspaceFactory.CreateRunspace(powerShellHost))
			{
				runspace.Open();

				foreach (KeyValuePair<string, object> scriptVariable in scriptVariables)
					runspace.SessionStateProxy.SetVariable(scriptVariable.Key, scriptVariable.Value);

				using (PowerShell powerShell = PowerShell.Create())
				{
					powerShell.Runspace = runspace;
					powerShell.AddScript(compiledCode);

					psObjects = powerShell.Invoke();

					if ((object)psObjects == null || psObjects.Count != 1)
						return null;

					returnValue = psObjects[0];
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Not implemented by this example class. The call fails with a NotImplementedException exception.
		/// </summary>
		public override void ExitNestedPrompt()
		{
		}

		/// <summary>
		/// This API is called before an external application process is started. Typically it is used to save state so the parent can restore state that has been modified by a child process (after the child exits). In this example, this functionality is not needed so the method returns nothing.
		/// </summary>
		public override void NotifyBeginApplication()
		{
		}

		/// <summary>
		/// This API is called after an external application process finishes. Typically it is used to restore state that a child process may have altered. In this example, this functionality is not needed so the method returns nothing.
		/// </summary>
		public override void NotifyEndApplication()
		{
		}

		/// <summary>
		/// Indicate to the host application that exit has been requested. Pass the exit code that the host application should use when exiting the process.
		/// </summary>
		/// <param name="exitCode"> The exit code to use. </param>
		public override void SetShouldExit(int exitCode)
		{
		}

		#endregion
	}
}