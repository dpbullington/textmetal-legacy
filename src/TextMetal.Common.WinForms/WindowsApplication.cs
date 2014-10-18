/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using TextMetal.Common.Core;

using Message = TextMetal.Common.Core.Message;

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

		protected override sealed void DisplayArgumentErrorMessage(IEnumerable<Message> argumentMessages)
		{
			/*if ((object)argumentMessages != null)
			{
				using (MessageForm messageForm = new MessageForm()
												{
													CoreText = this.AssemblyInformation.Product,
													Message = string.Empty,
													Messages = argumentMessages,
													StartPosition = FormStartPosition.CenterScreen
												})
					messageForm.ShowDialog(null);
			}*/
		}

		protected override sealed void DisplayArgumentMapMessage(IDictionary<string, ArgumentSpec> argumentMap)
		{
			string message;

			var requiredArgumentTokens = argumentMap.Select(m => (!m.Value.Required ? "[" : string.Empty) + string.Format("-{0}:value{1}", m.Key, !m.Value.Bounded ? "(s)" : string.Empty) + (!m.Value.Required ? "]" : string.Empty));

			if ((object)requiredArgumentTokens != null)
			{
				message = string.Format("USAGE: {0} ", Assembly.GetEntryAssembly().ManifestModule.Name) + string.Join(" ", requiredArgumentTokens) + Environment.NewLine;
				MessageBox.Show(message, this.AssemblyInformation.Product, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		protected override sealed void DisplayFailureMessage(Exception exception)
		{
			MessageBox.Show("A fatal error occured:" + Environment.NewLine + (object)exception == null ? Reflexion.GetErrors(exception, 0) : "<unknown>" + Environment.NewLine + "The application will now terminate.", this.AssemblyInformation.Product, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		protected override sealed void DisplaySuccessMessage(TimeSpan duration)
		{
			//MessageBox.Show(string.Format("Operation completed successfully; duration: '{0}'.", duration), this.AssemblyInformation.Product, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void OnApplicationThreadException(object sender, ThreadExceptionEventArgs e)
		{
			this.ShowNestedExceptionsAndThrowBrickAtProcess(new ApplicationException("OnApplicationThreadException", e.Exception));
		}

		protected override sealed int OnStartup(string[] args, IDictionary<string, IList<object>> arguments)
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