/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

using TextMetal.Common.Core;

namespace TextMetal.Common.WinForms
{
	public abstract class WindowsApplication<TMainForm, TSplashForm> : IDisposable
		where TMainForm : Form, new()
		where TSplashForm : Form, new()
	{
		#region Constructors/Destructors

		protected WindowsApplication()
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

		private bool HookUnhandledExceptionEvents
		{
			get
			{
				return !Debugger.IsAttached &&
						AppConfig.GetAppSetting<bool>(string.Format("{0}::HookUnhandledExceptionEvents", this.GetType().Namespace));
			}
		}

		private bool ShowSplashScreen
		{
			get
			{
				return AppConfig.GetAppSetting<bool>(string.Format("{0}::ShowSplashScreen", this.GetType().Namespace));
			}
		}

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		public int EntryPoint(string[] args)
		{
			if (this.HookUnhandledExceptionEvents)
				return this.TryStartup(args);
			else
				return this.Startup(args);
		}

		private void OnApplicationThreadException(object sender, ThreadExceptionEventArgs e)
		{
			this.ShowNestedExceptionsAndThrowBrickAtProcess(new ApplicationException("OnApplicationThreadException", e.Exception));
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			this.ShowNestedExceptionsAndThrowBrickAtProcess(new ApplicationException("OnUnhandledException", e.ExceptionObject as Exception));
		}

		public void ShowNestedExceptionsAndThrowBrickAtProcess(Exception e)
		{
			string message;

			message = Reflexion.GetErrors(e, 0);

			MessageBox.Show("A fatal error occured:\r\n" + message + "\r\nThe application will now terminate.", this.AssemblyInformation.Product, MessageBoxButtons.OK, MessageBoxIcon.Error);

			Environment.Exit(-1);
		}

		private int Startup(string[] args)
		{
			this.AssemblyInformation = new AssemblyInformation(Assembly.GetEntryAssembly());

			AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

			if (this.HookUnhandledExceptionEvents)
			{
				AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;
				Application.ThreadException += this.OnApplicationThreadException;
			}

			Control.CheckForIllegalCrossThreadCalls = true;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (this.ShowSplashScreen)
				Application.Run(new SplashApplicationContext(new TMainForm(), new TSplashForm()));
			else
				Application.Run(new TMainForm());

			return 0;
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