// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Principal;
using System.Windows.Forms;

using NUnit.UiKit;
using NUnit.Util;

namespace NUnit.Gui.SettingsPages
{
	public class AdvancedLoaderSettingsPage : SettingsPage
	{
		#region Constructors/Destructors

		public AdvancedLoaderSettingsPage(string key)
			: base(key)
		{
			// This call is required by the Windows Form Designer.
			this.InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		#endregion

		#region Fields/Constants

		private IContainer components = null;

		private CheckBox enableShadowCopyCheckBox;
		private GroupBox groupBox1;
		private GroupBox groupBox3;
		private HelpProvider helpProvider1;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label6;
		private Label label7;
		private CheckBox principalPolicyCheckBox;
		private ListBox principalPolicyListBox;
		private TextBox shadowCopyPathTextBox;

		#endregion

		#region Properties/Indexers/Events

		public override bool HasChangesRequiringReload
		{
			get
			{
				bool oldShadowCopyFiles = this.settings.GetSetting("Options.TestLoader.ShadowCopyFiles", true);
				string oldShadowCopyPath = this.settings.GetSetting("Options.TestLoader.ShadowCopyPath", "");
				bool oldSetPrincipalPolicy = this.settings.GetSetting("Options.TestLoader.SetPrincipalPolicy", false);
				PrincipalPolicy oldPrincipalPolicy = (PrincipalPolicy)this.settings.GetSetting("Options.TestLoader.PrincipalPolicy", PrincipalPolicy.UnauthenticatedPrincipal);

				return this.enableShadowCopyCheckBox.Checked != oldShadowCopyFiles
						|| this.shadowCopyPathTextBox.Text != oldShadowCopyPath
						|| this.principalPolicyCheckBox.Checked != oldSetPrincipalPolicy
						|| this.principalPolicyListBox.SelectedIndex != (int)oldPrincipalPolicy;
			}
		}

		#endregion

		#region Methods/Operators

		public override void ApplySettings()
		{
			this.settings.SaveSetting("Options.TestLoader.ShadowCopyFiles", this.enableShadowCopyCheckBox.Checked);

			if (this.shadowCopyPathTextBox.Text != "")
				this.settings.SaveSetting("Options.TestLoader.ShadowCopyPath", this.shadowCopyPathTextBox.Text);
			else
				this.settings.RemoveSetting("Options.TestLoader.ShadowCopyPath");

			this.settings.SaveSetting("Options.TestLoader.SetPrincipalPolicy", this.principalPolicyCheckBox.Checked);

			if (this.principalPolicyCheckBox.Checked)
				this.settings.SaveSetting("Options.TestLoader.PrincipalPolicy", (PrincipalPolicy)this.principalPolicyListBox.SelectedIndex);
			else
				this.settings.RemoveSetting("Options.TestLoader.PrincipalPolicy");
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
					this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			ComponentResourceManager resources = new ComponentResourceManager(typeof(AdvancedLoaderSettingsPage));
			this.label3 = new Label();
			this.groupBox3 = new GroupBox();
			this.enableShadowCopyCheckBox = new CheckBox();
			this.label2 = new Label();
			this.helpProvider1 = new HelpProvider();
			this.shadowCopyPathTextBox = new TextBox();
			this.label4 = new Label();
			this.principalPolicyCheckBox = new CheckBox();
			this.label7 = new Label();
			this.label6 = new Label();
			this.groupBox1 = new GroupBox();
			this.principalPolicyListBox = new ListBox();
			this.label1 = new Label();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new Point(8, 8);
			this.label3.Name = "label3";
			this.label3.Size = new Size(73, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Shadow Copy";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.groupBox3.Location = new Point(139, 8);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new Size(309, 8);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			// 
			// enableShadowCopyCheckBox
			// 
			this.enableShadowCopyCheckBox.AutoSize = true;
			this.helpProvider1.SetHelpString(this.enableShadowCopyCheckBox, resources.GetString("enableShadowCopyCheckBox.HelpString"));
			this.enableShadowCopyCheckBox.Location = new Point(24, 32);
			this.enableShadowCopyCheckBox.Name = "enableShadowCopyCheckBox";
			this.helpProvider1.SetShowHelp(this.enableShadowCopyCheckBox, true);
			this.enableShadowCopyCheckBox.Size = new Size(128, 17);
			this.enableShadowCopyCheckBox.TabIndex = 2;
			this.enableShadowCopyCheckBox.Text = "Enable Shadow Copy";
			this.enableShadowCopyCheckBox.CheckedChanged += new EventHandler(this.enableShadowCopyCheckBox_CheckedChanged);
			// 
			// label2
			// 
			this.label2.Location = new Point(139, 101);
			this.label2.Name = "label2";
			this.label2.Size = new Size(260, 59);
			this.label2.TabIndex = 6;
			this.label2.Text = "Shadow copy should normally be enabled. If it is disabled, the NUnit Gui may not " +
								"function correctly.";
			// 
			// shadowCopyPathTextBox
			// 
			this.helpProvider1.SetHelpString(this.shadowCopyPathTextBox, "Leave this blank to permit NUnit to select a location under your temp directory.");
			this.shadowCopyPathTextBox.Location = new Point(139, 65);
			this.shadowCopyPathTextBox.Name = "shadowCopyPathTextBox";
			this.helpProvider1.SetShowHelp(this.shadowCopyPathTextBox, true);
			this.shadowCopyPathTextBox.Size = new Size(309, 20);
			this.shadowCopyPathTextBox.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new Point(42, 66);
			this.label4.Name = "label4";
			this.label4.Size = new Size(66, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Cache Path:";
			// 
			// principalPolicyCheckBox
			// 
			this.principalPolicyCheckBox.AutoSize = true;
			this.principalPolicyCheckBox.Location = new Point(24, 199);
			this.principalPolicyCheckBox.Name = "principalPolicyCheckBox";
			this.principalPolicyCheckBox.Size = new Size(214, 17);
			this.principalPolicyCheckBox.TabIndex = 9;
			this.principalPolicyCheckBox.Text = "Set Principal Policy for test AppDomains";
			this.principalPolicyCheckBox.UseVisualStyleBackColor = true;
			this.principalPolicyCheckBox.CheckedChanged += new EventHandler(this.principalPolicyCheckBox_CheckedChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new Point(42, 225);
			this.label7.Name = "label7";
			this.label7.Size = new Size(38, 13);
			this.label7.TabIndex = 10;
			this.label7.Text = "Policy:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new Point(8, 163);
			this.label6.Name = "label6";
			this.label6.Size = new Size(78, 13);
			this.label6.TabIndex = 7;
			this.label6.Text = "Principal Policy";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.groupBox1.Location = new Point(139, 163);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(309, 8);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			// 
			// principalPolicyListBox
			// 
			this.principalPolicyListBox.FormattingEnabled = true;
			this.principalPolicyListBox.Items.AddRange(new object[]
														{
															"UnauthenticatedPrincipal",
															"NoPrincipal",
															"WindowsPrincipal"
														});
			this.principalPolicyListBox.Location = new Point(139, 225);
			this.principalPolicyListBox.Name = "principalPolicyListBox";
			this.principalPolicyListBox.Size = new Size(241, 69);
			this.principalPolicyListBox.TabIndex = 11;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new Point(42, 101);
			this.label1.Name = "label1";
			this.label1.Size = new Size(50, 13);
			this.label1.TabIndex = 12;
			this.label1.Text = "Warning:";
			// 
			// AdvancedLoaderSettingsPage
			// 
			this.Controls.Add(this.label1);
			this.Controls.Add(this.principalPolicyListBox);
			this.Controls.Add(this.principalPolicyCheckBox);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.shadowCopyPathTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.enableShadowCopyCheckBox);
			this.Name = "AdvancedLoaderSettingsPage";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		public override void LoadSettings()
		{
			this.settings = Services.UserSettings;

			this.enableShadowCopyCheckBox.Checked = this.settings.GetSetting("Options.TestLoader.ShadowCopyFiles", true);
			this.shadowCopyPathTextBox.Text = this.settings.GetSetting("Options.TestLoader.ShadowCopyPath", "");

			this.principalPolicyCheckBox.Checked = this.principalPolicyListBox.Enabled =
				this.settings.GetSetting("Options.TestLoader.SetPrincipalPolicy", false);
			this.principalPolicyListBox.SelectedIndex = (int)(PrincipalPolicy)this.settings.GetSetting("Options.TestLoader.PrincipalPolicy", PrincipalPolicy.UnauthenticatedPrincipal);
		}

		private void enableShadowCopyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.shadowCopyPathTextBox.Enabled = this.enableShadowCopyCheckBox.Checked;
		}

		private void principalPolicyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.principalPolicyListBox.Enabled = this.principalPolicyCheckBox.Checked;
		}

		#endregion
	}
}