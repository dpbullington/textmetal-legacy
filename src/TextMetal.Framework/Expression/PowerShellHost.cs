/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Threading;

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

		/// <summary>
		/// Not implemented by this example class. The call fails with a NotImplementedException exception.
		/// </summary>
		public override void EnterNestedPrompt()
		{
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