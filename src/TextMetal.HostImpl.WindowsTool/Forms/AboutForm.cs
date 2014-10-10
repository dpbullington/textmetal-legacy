/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using TextMetal.Common.WinForms;
using TextMetal.Common.WinForms.Forms;

namespace TextMetal.HostImpl.WindowsTool.Forms
{
	internal partial class AboutForm : TmForm
	{
		#region Constructors/Destructors

		public AboutForm()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Methods/Operators

		protected override void CoreSetup()
		{
			Stream stream;
			Image image;

			base.CoreSetup();

			this.CoreText = string.Format("About {0} Studio", ExecutableApplication.Current.AssemblyInformation.Product);

			this.lblVersion.Text = ExecutableApplication.Current.AssemblyInformation.AssemblyVersion;
			this.lblCompany.Text = ExecutableApplication.Current.AssemblyInformation.Company;
			this.lblConfiguration.Text = ExecutableApplication.Current.AssemblyInformation.Configuration;
			this.lblCopyright.Text = ExecutableApplication.Current.AssemblyInformation.Copyright;
			this.lblInformationalVersion.Text = ExecutableApplication.Current.AssemblyInformation.InformationalVersion;
			this.lblProduct.Text = string.Format("{0} Studio", ExecutableApplication.Current.AssemblyInformation.Product);
			this.lblTitle.Text = ExecutableApplication.Current.AssemblyInformation.Title;
			this.lblTrademark.Text = ExecutableApplication.Current.AssemblyInformation.Trademark;
			this.lblWin32FileVersion.Text = ExecutableApplication.Current.AssemblyInformation.Win32FileVersion;
			this.txtBxDescription.Text = ExecutableApplication.Current.AssemblyInformation.Description;

			stream = this.GetType().Assembly.GetManifestResourceStream("TextMetal.HostImpl.WindowsTool.Images.SplashScreen.png");

			if ((object)stream == null)
				throw new InvalidOperationException("TODO");

			image = Image.FromStream(stream);

			this.pbAppLogo.Image = image;
			// DO NOT DISPOSE (owner cleans up)
		}

		private void Okay()
		{
			this.DialogResult = DialogResult.Cancel; // yes, this is correct
			this.Close(); // direct
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.Okay();
		}

		#endregion
	}
}