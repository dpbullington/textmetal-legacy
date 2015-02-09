/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Windows.Forms;

using TextMetal.Common.Core;

namespace TextMetal.Common.WinForms
{
	public abstract class WindowsApplication<TMainForm, TSplashForm> : WindowsApplication<TMainForm>
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

		protected override sealed void OnRunApplication()
		{
			if (!this.ShowSplashScreen)
				base.OnRunApplication();
			else
				Application.Run(new SplashApplicationContext(new TMainForm(), new TSplashForm()));
		}

		#endregion
	}
}