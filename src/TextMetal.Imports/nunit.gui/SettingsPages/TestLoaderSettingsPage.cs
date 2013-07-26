// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Windows.Forms;

using NUnit.Core;
using NUnit.UiKit;

namespace NUnit.Gui.SettingsPages
{
	public class TestLoaderSettingsPage : SettingsPage
	{
		#region Constructors/Destructors

		public TestLoaderSettingsPage(string key)
			: base(key)
		{
			// This call is required by the Windows Form Designer.
			this.InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		#endregion

		#region Fields/Constants

		private IContainer components = null;
		private GroupBox groupBox2;
		private GroupBox groupBox3;

		private HelpProvider helpProvider1;
		private Label label2;
		private Label label3;
		private CheckBox mergeAssembliesCheckBox;
		private RadioButton multiDomainRadioButton;
		private RadioButton multiProcessRadioButton;
		private RadioButton sameProcessRadioButton;
		private RadioButton separateProcessRadioButton;
		private RadioButton singleDomainRadioButton;

		#endregion

		#region Properties/Indexers/Events

		public override bool HasChangesRequiringReload
		{
			get
			{
				return
					this.GetSavedProcessModel() != this.GetSelectedProcessModel() ||
					this.GetSavedDomainUsage() != this.GetSelectedDomainUsage() ||
					this.settings.GetSetting("Options.TestLoader.MergeAssemblies", false) != this.mergeAssembliesCheckBox.Checked;
			}
		}

		#endregion

		#region Methods/Operators

		public override void ApplySettings()
		{
			if (this.multiProcessRadioButton.Checked)
				this.settings.SaveSetting("Options.TestLoader.ProcessModel", ProcessModel.Multiple);
			else if (this.separateProcessRadioButton.Checked)
				this.settings.SaveSetting("Options.TestLoader.ProcessModel", ProcessModel.Separate);
			else
				this.settings.RemoveSetting("Options.TestLoader.ProcessModel");

			if (this.multiDomainRadioButton.Checked)
				this.settings.SaveSetting("Options.TestLoader.DomainUsage", DomainUsage.Multiple);
			else
				this.settings.RemoveSetting("Options.TestLoader.DomainUsage");

			this.settings.SaveSetting("Options.TestLoader.MergeAssemblies", this.mergeAssembliesCheckBox.Checked);
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

		private DomainUsage GetSavedDomainUsage()
		{
			return (DomainUsage)this.settings.GetSetting("Options.TestLoader.DomainUsage", DomainUsage.Default);
		}

		private ProcessModel GetSavedProcessModel()
		{
			return (ProcessModel)this.settings.GetSetting("Options.TestLoader.ProcessModel", ProcessModel.Default);
		}

		private DomainUsage GetSelectedDomainUsage()
		{
			return this.multiDomainRadioButton.Checked
				       ? DomainUsage.Multiple
				       : DomainUsage.Single;
		}

		private ProcessModel GetSelectedProcessModel()
		{
			return this.separateProcessRadioButton.Checked
				       ? ProcessModel.Separate
				       : this.multiProcessRadioButton.Checked
					         ? ProcessModel.Multiple
					         : ProcessModel.Single;
		}

		/// <summary>
		/// 	Required method for Designer support - do not modify
		/// 	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mergeAssembliesCheckBox = new System.Windows.Forms.CheckBox();
			this.singleDomainRadioButton = new System.Windows.Forms.RadioButton();
			this.multiDomainRadioButton = new System.Windows.Forms.RadioButton();
			this.helpProvider1 = new System.Windows.Forms.HelpProvider();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.multiProcessRadioButton = new System.Windows.Forms.RadioButton();
			this.separateProcessRadioButton = new System.Windows.Forms.RadioButton();
			this.sameProcessRadioButton = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// mergeAssembliesCheckBox
			// 
			this.mergeAssembliesCheckBox.AutoSize = true;
			this.helpProvider1.SetHelpString(this.mergeAssembliesCheckBox, "If checked, tests in each assembly will be merged into a single tree.");
			this.mergeAssembliesCheckBox.Location = new System.Drawing.Point(48, 221);
			this.mergeAssembliesCheckBox.Name = "mergeAssembliesCheckBox";
			this.helpProvider1.SetShowHelp(this.mergeAssembliesCheckBox, true);
			this.mergeAssembliesCheckBox.Size = new System.Drawing.Size(169, 17);
			this.mergeAssembliesCheckBox.TabIndex = 10;
			this.mergeAssembliesCheckBox.Text = "Merge tests across assemblies";
			// 
			// singleDomainRadioButton
			// 
			this.singleDomainRadioButton.AutoCheck = false;
			this.singleDomainRadioButton.AutoSize = true;
			this.singleDomainRadioButton.Checked = true;
			this.helpProvider1.SetHelpString(this.singleDomainRadioButton, "If selected, all test assemblies will be loaded in the same AppDomain.");
			this.singleDomainRadioButton.Location = new System.Drawing.Point(32, 190);
			this.singleDomainRadioButton.Name = "singleDomainRadioButton";
			this.helpProvider1.SetShowHelp(this.singleDomainRadioButton, true);
			this.singleDomainRadioButton.Size = new System.Drawing.Size(194, 17);
			this.singleDomainRadioButton.TabIndex = 9;
			this.singleDomainRadioButton.TabStop = true;
			this.singleDomainRadioButton.Text = "Use a single AppDomain for all tests";
			this.singleDomainRadioButton.Click += new System.EventHandler(this.toggleMultiDomain);
			// 
			// multiDomainRadioButton
			// 
			this.multiDomainRadioButton.AutoCheck = false;
			this.multiDomainRadioButton.AutoSize = true;
			this.helpProvider1.SetHelpString(this.multiDomainRadioButton, "If selected, each test assembly will be loaded in a separate AppDomain.");
			this.multiDomainRadioButton.Location = new System.Drawing.Point(32, 160);
			this.multiDomainRadioButton.Name = "multiDomainRadioButton";
			this.helpProvider1.SetShowHelp(this.multiDomainRadioButton, true);
			this.multiDomainRadioButton.Size = new System.Drawing.Size(220, 17);
			this.multiDomainRadioButton.TabIndex = 8;
			this.multiDomainRadioButton.Text = "Use a separate AppDomain per Assembly";
			this.multiDomainRadioButton.Click += new System.EventHandler(this.toggleMultiDomain);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(114, 13);
			this.label3.TabIndex = 35;
			this.label3.Text = "Default Process Model";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                              | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Location = new System.Drawing.Point(199, 8);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(253, 8);
			this.groupBox3.TabIndex = 34;
			this.groupBox3.TabStop = false;
			// 
			// multiProcessRadioButton
			// 
			this.multiProcessRadioButton.AutoSize = true;
			this.multiProcessRadioButton.Location = new System.Drawing.Point(32, 99);
			this.multiProcessRadioButton.Name = "multiProcessRadioButton";
			this.multiProcessRadioButton.Size = new System.Drawing.Size(239, 17);
			this.multiProcessRadioButton.TabIndex = 36;
			this.multiProcessRadioButton.Text = "Run tests in a separate process per Assembly";
			this.multiProcessRadioButton.CheckedChanged += new System.EventHandler(this.toggleProcessUsage);
			// 
			// separateProcessRadioButton
			// 
			this.separateProcessRadioButton.AutoSize = true;
			this.separateProcessRadioButton.Location = new System.Drawing.Point(32, 66);
			this.separateProcessRadioButton.Name = "separateProcessRadioButton";
			this.separateProcessRadioButton.Size = new System.Drawing.Size(204, 17);
			this.separateProcessRadioButton.TabIndex = 37;
			this.separateProcessRadioButton.Text = "Run tests in a single separate process";
			this.separateProcessRadioButton.CheckedChanged += new System.EventHandler(this.toggleProcessUsage);
			// 
			// sameProcessRadioButton
			// 
			this.sameProcessRadioButton.AutoSize = true;
			this.sameProcessRadioButton.Checked = true;
			this.sameProcessRadioButton.Location = new System.Drawing.Point(32, 33);
			this.sameProcessRadioButton.Name = "sameProcessRadioButton";
			this.sameProcessRadioButton.Size = new System.Drawing.Size(205, 17);
			this.sameProcessRadioButton.TabIndex = 38;
			this.sameProcessRadioButton.TabStop = true;
			this.sameProcessRadioButton.Text = "Run tests directly in the NUnit process";
			this.sameProcessRadioButton.CheckedChanged += new System.EventHandler(this.toggleProcessUsage);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 136);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(114, 13);
			this.label2.TabIndex = 40;
			this.label2.Text = "Default Domain Usage";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                              | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Location = new System.Drawing.Point(199, 136);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(253, 8);
			this.groupBox2.TabIndex = 39;
			this.groupBox2.TabStop = false;
			// 
			// TestLoaderSettingsPage
			// 
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.sameProcessRadioButton);
			this.Controls.Add(this.separateProcessRadioButton);
			this.Controls.Add(this.multiProcessRadioButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.mergeAssembliesCheckBox);
			this.Controls.Add(this.singleDomainRadioButton);
			this.Controls.Add(this.multiDomainRadioButton);
			this.Name = "TestLoaderSettingsPage";
			this.Size = new System.Drawing.Size(456, 341);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		public override void LoadSettings()
		{
			switch (this.GetSavedProcessModel())
			{
				case ProcessModel.Separate:
					this.separateProcessRadioButton.Checked = true;
					this.sameProcessRadioButton.Checked = false;
					this.multiProcessRadioButton.Checked = false;
					break;
				case ProcessModel.Multiple:
					this.multiProcessRadioButton.Checked = true;
					this.sameProcessRadioButton.Checked = false;
					this.separateProcessRadioButton.Checked = false;
					break;
				default:
					this.sameProcessRadioButton.Checked = true;
					this.multiProcessRadioButton.Checked = false;
					this.separateProcessRadioButton.Checked = false;
					break;
			}

			bool multiDomain = this.GetSavedDomainUsage() == DomainUsage.Multiple;
			this.multiDomainRadioButton.Checked = multiDomain;
			this.singleDomainRadioButton.Checked = !multiDomain;
			this.mergeAssembliesCheckBox.Enabled = !multiDomain;

			this.mergeAssembliesCheckBox.Checked = this.settings.GetSetting("Options.TestLoader.MergeAssemblies", false);
		}

		private void toggleMultiDomain(object sender, EventArgs e)
		{
			bool multiDomain = this.multiDomainRadioButton.Checked = ! this.multiDomainRadioButton.Checked;
			this.singleDomainRadioButton.Checked = !multiDomain;
			this.mergeAssembliesCheckBox.Enabled = !multiDomain && !this.multiProcessRadioButton.Checked;
		}

		private void toggleProcessUsage(object sender, EventArgs e)
		{
			bool enable = this.sameProcessRadioButton.Checked || this.separateProcessRadioButton.Checked;
			this.singleDomainRadioButton.Enabled = enable;
			this.multiDomainRadioButton.Enabled = enable;
			this.mergeAssembliesCheckBox.Enabled = enable && this.singleDomainRadioButton.Checked;
		}

		#endregion
	}
}