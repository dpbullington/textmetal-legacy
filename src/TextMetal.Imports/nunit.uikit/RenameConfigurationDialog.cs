// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NUnit.Util;

namespace NUnit.UiKit
{
	/// <summary>
	/// Displays a dialog for entry of a new name for an
	/// existing configuration. This dialog collects and
	/// validates the name. The caller is responsible for
	/// actually renaming the cofiguration.
	/// </summary>
	public class RenameConfigurationDialog : NUnitFormBase
	{
		#region Constructors/Destructors

		public RenameConfigurationDialog(NUnitProject project, string configurationName)
		{
			this.InitializeComponent();
			this.project = project;
			this.configurationName = configurationName;
			this.originalName = configurationName;
		}

		#endregion

		#region Fields/Constants

		private Button cancelButton;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		/// <summary>
		/// The new name to give the configuration
		/// </summary>
		private string configurationName;

		private TextBox configurationNameTextBox;
		private Button okButton;

		/// <summary>
		/// The original name of the configuration
		/// </summary>
		private string originalName;

		/// <summary>
		/// The project in which we are renaming a configuration
		/// </summary>
		private NUnitProject project;

		#endregion

		#region Properties/Indexers/Events

		public string ConfigurationName
		{
			get
			{
				return this.configurationName;
			}
			set
			{
				this.configurationName = value;
			}
		}

		#endregion

		#region Methods/Operators

		private void ConfigurationNameDialog_Load(object sender, EventArgs e)
		{
			if (this.configurationName != null)
			{
				this.configurationNameTextBox.Text = this.configurationName;
				this.configurationNameTextBox.SelectAll();
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
			this.SuspendLayout();
			// 
			// configurationNameTextBox
			// 
			this.configurationNameTextBox.Location = new Point(16, 16);
			this.configurationNameTextBox.Name = "configurationNameTextBox";
			this.configurationNameTextBox.Size = new Size(264, 22);
			this.configurationNameTextBox.TabIndex = 0;
			this.configurationNameTextBox.Text = "";
			this.configurationNameTextBox.TextChanged += new EventHandler(this.configurationNameTextBox_TextChanged);
			// 
			// okButton
			// 
			this.okButton.Location = new Point(56, 48);
			this.okButton.Name = "okButton";
			this.okButton.Size = new Size(75, 24);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.okButton.Click += new EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Location = new Point(160, 48);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new Size(75, 24);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			// 
			// RenameConfigurationDialog
			// 
			this.AcceptButton = this.okButton;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new Size(291, 79);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.configurationNameTextBox);
			this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			this.Name = "RenameConfigurationDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Rename Configuration";
			this.Load += new EventHandler(this.ConfigurationNameDialog_Load);
			this.ResumeLayout(false);
		}

		private void configurationNameTextBox_TextChanged(object sender, EventArgs e)
		{
			this.okButton.Enabled =
				this.configurationNameTextBox.TextLength > 0 &&
				this.configurationNameTextBox.Text != this.originalName;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			this.configurationName = this.configurationNameTextBox.Text;
			if (this.project.Configs.Contains(this.configurationName))
				// TODO: Need general error message display
				this.MessageDisplay.Error("A configuration with that name already exists");
			else
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		#endregion
	}
}