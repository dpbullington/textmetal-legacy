// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Windows.Forms;

using NUnit.UiKit;

namespace NUnit.Gui.SettingsPages
{
	public class VisualStudioSettingsPage : SettingsPage
	{
		#region Constructors/Destructors

		public VisualStudioSettingsPage(string key)
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
		private Label label2;
		private Label label3;
		private CheckBox useSolutionConfigsCheckBox;
		private CheckBox visualStudioSupportCheckBox;

		#endregion

		#region Methods/Operators

		public override void ApplySettings()
		{
			this.settings.SaveSetting("Options.TestLoader.VisualStudioSupport", this.visualStudioSupportCheckBox.Checked);
			this.settings.SaveSetting("Options.TestLoader.VisualStudio.UseSolutionConfigs", this.useSolutionConfigsCheckBox.Checked);
		}

		/// <summary>
		/// 	Clean up any resources being used.
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
		/// 	Required method for Designer support - do not modify
		/// 	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisualStudioSettingsPage));
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.visualStudioSupportCheckBox = new System.Windows.Forms.CheckBox();
			this.helpProvider1 = new System.Windows.Forms.HelpProvider();
			this.useSolutionConfigsCheckBox = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "Visual Studio";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                              | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Location = new System.Drawing.Point(151, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(293, 8);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			// 
			// visualStudioSupportCheckBox
			// 
			this.visualStudioSupportCheckBox.AutoSize = true;
			this.helpProvider1.SetHelpString(this.visualStudioSupportCheckBox, "If checked, Visual Studio projects and solutions may be opened or added to existi" +
			                                                                   "ng test projects.");
			this.visualStudioSupportCheckBox.Location = new System.Drawing.Point(24, 24);
			this.visualStudioSupportCheckBox.Name = "visualStudioSupportCheckBox";
			this.helpProvider1.SetShowHelp(this.visualStudioSupportCheckBox, true);
			this.visualStudioSupportCheckBox.Size = new System.Drawing.Size(163, 17);
			this.visualStudioSupportCheckBox.TabIndex = 30;
			this.visualStudioSupportCheckBox.Text = "Enable Visual Studio Support";
			this.visualStudioSupportCheckBox.CheckedChanged += new System.EventHandler(this.visualStudioSupportCheckBox_CheckedChanged);
			// 
			// useSolutionConfigsCheckBox
			// 
			this.useSolutionConfigsCheckBox.AutoSize = true;
			this.useSolutionConfigsCheckBox.Location = new System.Drawing.Point(44, 60);
			this.useSolutionConfigsCheckBox.Name = "useSolutionConfigsCheckBox";
			this.useSolutionConfigsCheckBox.Size = new System.Drawing.Size(255, 17);
			this.useSolutionConfigsCheckBox.TabIndex = 31;
			this.useSolutionConfigsCheckBox.Text = "Use solution configs when opening VS solutions.";
			this.useSolutionConfigsCheckBox.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(110, 89);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(322, 124);
			this.label2.TabIndex = 33;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(39, 89);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(33, 13);
			this.label3.TabIndex = 32;
			this.label3.Text = "Note:";
			// 
			// VisualStudioSettingsPage
			// 
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.useSolutionConfigsCheckBox);
			this.Controls.Add(this.visualStudioSupportCheckBox);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Name = "VisualStudioSettingsPage";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		public override void LoadSettings()
		{
			this.visualStudioSupportCheckBox.Checked = this.settings.GetSetting("Options.TestLoader.VisualStudioSupport", false);
			this.useSolutionConfigsCheckBox.Enabled = this.visualStudioSupportCheckBox.Checked;
			this.useSolutionConfigsCheckBox.Checked = this.settings.GetSetting("Options.TestLoader.VisualStudio.UseSolutionConfigs", true);
		}

		private void visualStudioSupportCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.useSolutionConfigsCheckBox.Enabled = this.visualStudioSupportCheckBox.Checked;
		}

		#endregion
	}
}