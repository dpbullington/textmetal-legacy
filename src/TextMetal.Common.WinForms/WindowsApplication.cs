/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

using TextMetal.Common.Core;

namespace TextMetal.Common.WinForms
{
	public abstract class WindowsApplication<TMainForm, TSplashForm> : ExecutableApplication
		where TMainForm : Form, new()
		where TSplashForm : Form, new()
	{
		#region Constructors/Destructors

		protected WindowsApplication()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		public bool ShowSplashScreen
		{
			get
			{
				return AppConfig.GetAppSetting<bool>(string.Format("{0}::ShowSplashScreen", this.GetType().Namespace));
			}
		}

		#endregion

		#region Methods/Operators

		protected override void DisplayExceptionMessage(string exceptionMessage)
		{
			MessageBox.Show("A fatal error occured:\r\n" + exceptionMessage + "\r\nThe application will now terminate.", this.AssemblyInformation.Product, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void OnApplicationThreadException(object sender, ThreadExceptionEventArgs e)
		{
			this.ShowNestedExceptionsAndThrowBrickAtProcess(new ApplicationException("OnApplicationThreadException", e.Exception));
		}

		protected override int OnStartup(string[] args, IDictionary<string, IList<string>> arguments)
		{
			if (this.HookUnhandledExceptionEvents)
				Application.ThreadException += this.OnApplicationThreadException;

			Control.CheckForIllegalCrossThreadCalls = true;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (this.ShowSplashScreen)
				Application.Run(new SplashApplicationContext(new TMainForm(), new TSplashForm()));
			else
				Application.Run(new TMainForm());

			return 0;
		}

		#endregion
	}
}