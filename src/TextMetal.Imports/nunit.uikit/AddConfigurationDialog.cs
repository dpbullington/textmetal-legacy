// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NUnit.Util;

namespace NUnit.UiKit
{
	/// <summary>
	/// Displays a dialog for creation of a new configuration.
	/// The dialog collects and validates the name and the
	/// name of a configuration to be copied and then adds the
	/// new configuration to the project.
	/// A DialogResult of DialogResult.OK indicates that the
	/// configuration was added successfully.
	/// </summary>
	public class AddConfigurationDialog : NUnitFormBase
	{
		#region Constructors/Destructors

		public AddConfigurationDialog(NUnitProject project)
		{
			this.InitializeComponent();
			this.project = project;
		}

		#endregion

		#region Fields/Constants

		private Button cancelButton;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private ComboBox configurationComboBox;
		private string configurationName;

		private TextBox configurationNameTextBox;
		private string copyConfigurationName;
		private Label label1;
		private Label label2;
		private Button okButton;
		private NUnitProject project;

		#endregion

		#region Properties/Indexers/Events

		public string ConfigurationName
		{
			get
			{
				return this.configurationName;
			}
		}

		public string CopyConfigurationName
		{
			get
			{
				return this.copyConfigurationName;
			}
		}

		#endregion

		#region Methods/Operators

		private void ConfigurationNameDialog_Load(object sender, EventArgs e)
		{
			this.configurationComboBox.Items.Add("<none>");
			this.configurationComboBox.SelectedIndex = 0;

			foreach (ProjectConfig config in this.project.Configs)
			{
				int index = this.configurationComboBox.Items.Add(config.Name);
				if (config.Name == this.project.ActiveConfigName)
					this.configurationComboBox.SelectedIndex = index;
			}
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
			this.configurationNameTextBox = new TextBox();
			this.okButton = new Button();
			this.cancelButton = new Button();
			this.configurationComboBox = new ComboBox();
			this.label1 = new Label();
			this.label2 = new Label();
			this.SuspendLayout();
			// 
			// configurationNameTextBox
			// 
			this.configurationNameTextBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
																	| AnchorStyles.Right)));
			this.configurationNameTextBox.Location = new Point(16, 24);
			this.configurationNameTextBox.Name = "configurationNameTextBox";
			this.configurationNameTextBox.Size = new Size(254, 22);
			this.configurationNameTextBox.TabIndex = 0;
			this.configurationNameTextBox.Text = "";
			// 
			// okButton
			// 
			this.okButton.Anchor = AnchorStyles.Bottom;
			this.okButton.Location = new Point(50, 120);
			this.okButton.Name = "okButton";
			this.okButton.Size = new Size(76, 23);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.okButton.Click += new EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = AnchorStyles.Bottom;
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Location = new Point(155, 120);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			// 
			// configurationComboBox
			// 
			this.configurationComboBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
																| AnchorStyles.Right)));
			this.configurationComboBox.ItemHeight = 16;
			this.configurationComboBox.Location = new Point(16, 80);
			this.configurationComboBox.Name = "configurationComboBox";
			this.configurationComboBox.Size = new Size(256, 24);
			this.configurationComboBox.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new Size(248, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Configuration Name:";
			// 
			// label2
			// 
			this.label2.Location = new Point(16, 63);
			this.label2.Name = "label2";
			this.label2.Size = new Size(240, 17);
			this.label2.TabIndex = 5;
			this.label2.Text = "Copy Settings From:";
			// 
			// AddConfigurationDialog
			// 
			this.AcceptButton = this.okButton;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new Size(280, 149);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.configurationComboBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.configurationNameTextBox);
			this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			this.Name = "AddConfigurationDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "New Configuration";
			this.Load += new EventHandler(this.ConfigurationNameDialog_Load);
			this.ResumeLayout(false);
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			this.configurationName = this.configurationNameTextBox.Text;

			if (this.configurationName == string.Empty)
			{
				this.MessageDisplay.Error("No configuration name provided");
				return;
			}

			if (this.project.Configs.Contains(this.configurationName))
			{
				this.MessageDisplay.Error("A configuration with that name already exists");
				return;
			}

			// ToDo: Move more of this to project
			ProjectConfig newConfig = new ProjectConfig(this.configurationName);

			this.copyConfigurationName = null;
			if (this.configurationComboBox.SelectedIndex > 0)
			{
				this.copyConfigurationName = (string)this.configurationComboBox.SelectedItem;
				ProjectConfig copyConfig = this.project.Configs[this.copyConfigurationName];
				if (copyConfig != null)
				{
					foreach (string assembly in copyConfig.Assemblies)
						newConfig.Assemblies.Add(assembly);
				}
			}

			this.project.Configs.Add(newConfig);
			this.DialogResult = DialogResult.OK;

			this.Close();
		}

		#endregion
	}
}