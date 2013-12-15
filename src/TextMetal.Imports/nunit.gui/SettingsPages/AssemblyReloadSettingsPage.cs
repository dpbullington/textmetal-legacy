// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using NUnit.UiKit;

namespace NUnit.Gui.SettingsPages
{
	public class AssemblyReloadSettingsPage : SettingsPage
	{
		#region Constructors/Destructors

		public AssemblyReloadSettingsPage(string key)
			: base(key)
		{
			// This call is required by the Windows Form Designer.
			this.InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		#endregion

		#region Fields/Constants

		private IContainer components = null;

		private GroupBox groupBox1;
		private HelpProvider helpProvider1;
		private Label label1;
		private CheckBox reloadOnChangeCheckBox;
		private CheckBox reloadOnRunCheckBox;
		private CheckBox rerunOnChangeCheckBox;

		#endregion

		#region Methods/Operators

		public override void ApplySettings()
		{
			this.settings.SaveSetting("Options.TestLoader.ReloadOnChange", this.reloadOnChangeCheckBox.Checked);
			this.settings.SaveSetting("Options.TestLoader.RerunOnChange", this.rerunOnChangeCheckBox.Checked);
			this.settings.SaveSetting("Options.TestLoader.ReloadOnRun", this.reloadOnRunCheckBox.Checked);
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
			this.label1 = new Label();
			this.groupBox1 = new GroupBox();
			this.rerunOnChangeCheckBox = new CheckBox();
			this.reloadOnRunCheckBox = new CheckBox();
			this.reloadOnChangeCheckBox = new CheckBox();
			this.helpProvider1 = new HelpProvider();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new Point(8, 4);
			this.label1.Name = "label1";
			this.label1.Size = new Size(88, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Assembly Reload";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.groupBox1.Location = new Point(181, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(259, 8);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			// 
			// rerunOnChangeCheckBox
			// 
			this.rerunOnChangeCheckBox.AutoSize = true;
			this.rerunOnChangeCheckBox.Enabled = false;
			this.helpProvider1.SetHelpString(this.rerunOnChangeCheckBox, "If checked, the last tests run will be re-run automatically whenever the assembly" +
																		" changes.");
			this.rerunOnChangeCheckBox.Location = new Point(48, 96);
			this.rerunOnChangeCheckBox.Name = "rerunOnChangeCheckBox";
			this.helpProvider1.SetShowHelp(this.rerunOnChangeCheckBox, true);
			this.rerunOnChangeCheckBox.Size = new Size(120, 17);
			this.rerunOnChangeCheckBox.TabIndex = 13;
			this.rerunOnChangeCheckBox.Text = "Re-run last tests run";
			// 
			// reloadOnRunCheckBox
			// 
			this.reloadOnRunCheckBox.AutoSize = true;
			this.helpProvider1.SetHelpString(this.reloadOnRunCheckBox, "If checked, the assembly is reloaded before each run");
			this.reloadOnRunCheckBox.Location = new Point(24, 32);
			this.reloadOnRunCheckBox.Name = "reloadOnRunCheckBox";
			this.helpProvider1.SetShowHelp(this.reloadOnRunCheckBox, true);
			this.reloadOnRunCheckBox.Size = new Size(158, 17);
			this.reloadOnRunCheckBox.TabIndex = 11;
			this.reloadOnRunCheckBox.Text = "Reload before each test run";
			// 
			// reloadOnChangeCheckBox
			// 
			this.reloadOnChangeCheckBox.AutoSize = true;
			this.helpProvider1.SetHelpString(this.reloadOnChangeCheckBox, "If checked, the assembly is reloaded whenever it changes. Changes to this setting" +
																		" do not take effect until the next time an assembly is loaded.");
			this.reloadOnChangeCheckBox.Location = new Point(24, 64);
			this.reloadOnChangeCheckBox.Name = "reloadOnChangeCheckBox";
			this.helpProvider1.SetShowHelp(this.reloadOnChangeCheckBox, true);
			this.reloadOnChangeCheckBox.Size = new Size(199, 17);
			this.reloadOnChangeCheckBox.TabIndex = 12;
			this.reloadOnChangeCheckBox.Text = "Reload when test assembly changes";
			this.reloadOnChangeCheckBox.CheckedChanged += new EventHandler(this.reloadOnChangeCheckBox_CheckedChanged);
			// 
			// AssemblyReloadSettingsPage
			// 
			this.Controls.Add(this.rerunOnChangeCheckBox);
			this.Controls.Add(this.reloadOnRunCheckBox);
			this.Controls.Add(this.reloadOnChangeCheckBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.Name = "AssemblyReloadSettingsPage";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		public override void LoadSettings()
		{
			this.reloadOnChangeCheckBox.Checked = this.settings.GetSetting("Options.TestLoader.ReloadOnChange", true);
			this.rerunOnChangeCheckBox.Checked = this.settings.GetSetting("Options.TestLoader.RerunOnChange", false);
			this.reloadOnRunCheckBox.Checked = this.settings.GetSetting("Options.TestLoader.ReloadOnRun", false);
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			Process.Start("http://nunit.com/?p=optionsDialog&r=2.4.5");
		}

		private void reloadOnChangeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.rerunOnChangeCheckBox.Enabled = this.reloadOnChangeCheckBox.Checked;
		}

		#endregion
	}
}