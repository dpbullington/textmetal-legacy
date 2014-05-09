/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using TextMetal.HostImpl.WindowsTool.Shared.Forms;

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

			this.CoreText = string.Format("About {0} Studio", Program.Instance.AssemblyInformation.Product);

			this.lblVersion.Text = Program.Instance.AssemblyInformation.AssemblyVersion;
			this.lblCompany.Text = Program.Instance.AssemblyInformation.Company;
			this.lblConfiguration.Text = Program.Instance.AssemblyInformation.Configuration;
			this.lblCopyright.Text = Program.Instance.AssemblyInformation.Copyright;
			this.lblInformationalVersion.Text = Program.Instance.AssemblyInformation.InformationalVersion;
			this.lblProduct.Text = string.Format("{0} Studio", Program.Instance.AssemblyInformation.Product);
			this.lblTitle.Text = Program.Instance.AssemblyInformation.Title;
			this.lblTrademark.Text = Program.Instance.AssemblyInformation.Trademark;
			this.lblWin32FileVersion.Text = Program.Instance.AssemblyInformation.Win32FileVersion;
			this.txtBxDescription.Text = Program.Instance.AssemblyInformation.Description;

			stream = this.GetType().Assembly.GetManifestResourceStream("TextMetal.HostImpl.WindowsTool.Images.SplashScreen.png");

			if ((object)stream == null)
				throw new InvalidOperationException("");

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