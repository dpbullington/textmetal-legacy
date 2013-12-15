// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

using NUnit.Util;

namespace NUnit.UiKit
{
	/// <summary>
	/// ConfigurationEditor form is designed for adding, deleting
	/// and renaming configurations from a project.
	/// </summary>
	public class ConfigurationEditor : NUnitFormBase
	{
		#region Constructors/Destructors

		public ConfigurationEditor(NUnitProject project)
		{
			this.InitializeComponent();
			this.project = project;
		}

		#endregion

		#region Fields/Constants

		private Button activeButton;
		private Button addButton;
		private Button closeButton;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private ListBox configListBox;
		private HelpProvider helpProvider1;
		private NUnitProject project;
		private Button removeButton;
		private Button renameButton;
		private int selectedIndex = -1;

		#endregion

		#region Methods/Operators

		private void ConfigurationEditor_Load(object sender, EventArgs e)
		{
			this.FillListBox();
			if (this.configListBox.Items.Count > 0)
				this.configListBox.SelectedIndex = this.selectedIndex = 0;
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

		private void FillListBox()
		{
			this.configListBox.Items.Clear();
			int count = 0;

			foreach (ProjectConfig config in this.project.Configs)
			{
				string name = config.Name;

				if (name == this.project.ActiveConfigName)
					name += " (active)";

				this.configListBox.Items.Add(name);
				count++;
			}

			if (count > 0)
			{
				if (this.selectedIndex >= count)
					this.selectedIndex = count - 1;

				this.configListBox.SelectedIndex = this.selectedIndex;
			}
			else
				this.selectedIndex = -1;
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			ResourceManager resources = new ResourceManager(typeof(ConfigurationEditor));
			this.configListBox = new ListBox();
			this.removeButton = new Button();
			this.renameButton = new Button();
			this.closeButton = new Button();
			this.addButton = new Button();
			this.activeButton = new Button();
			this.helpProvider1 = new HelpProvider();
			this.SuspendLayout();
			// 
			// configListBox
			// 
			this.helpProvider1.SetHelpString(this.configListBox, "Selects the configuration to operate on.");
			this.configListBox.ItemHeight = 16;
			this.configListBox.Location = new Point(8, 8);
			this.configListBox.Name = "configListBox";
			this.helpProvider1.SetShowHelp(this.configListBox, true);
			this.configListBox.Size = new Size(168, 212);
			this.configListBox.TabIndex = 0;
			this.configListBox.SelectedIndexChanged += new EventHandler(this.configListBox_SelectedIndexChanged);
			// 
			// removeButton
			// 
			this.helpProvider1.SetHelpString(this.removeButton, "Removes the selected configuration");
			this.removeButton.Location = new Point(192, 8);
			this.removeButton.Name = "removeButton";
			this.helpProvider1.SetShowHelp(this.removeButton, true);
			this.removeButton.Size = new Size(96, 32);
			this.removeButton.TabIndex = 1;
			this.removeButton.Text = "&Remove";
			this.removeButton.Click += new EventHandler(this.removeButton_Click);
			// 
			// renameButton
			// 
			this.helpProvider1.SetHelpString(this.renameButton, "Allows renaming the selected configuration");
			this.renameButton.Location = new Point(192, 48);
			this.renameButton.Name = "renameButton";
			this.helpProvider1.SetShowHelp(this.renameButton, true);
			this.renameButton.Size = new Size(96, 32);
			this.renameButton.TabIndex = 2;
			this.renameButton.Text = "Re&name";
			this.renameButton.Click += new EventHandler(this.renameButton_Click);
			// 
			// closeButton
			// 
			this.closeButton.DialogResult = DialogResult.Cancel;
			this.helpProvider1.SetHelpString(this.closeButton, "Closes this dialog");
			this.closeButton.Location = new Point(192, 216);
			this.closeButton.Name = "closeButton";
			this.helpProvider1.SetShowHelp(this.closeButton, true);
			this.closeButton.Size = new Size(96, 32);
			this.closeButton.TabIndex = 4;
			this.closeButton.Text = "Close";
			this.closeButton.Click += new EventHandler(this.okButton_Click);
			// 
			// addButton
			// 
			this.helpProvider1.SetHelpString(this.addButton, "Allows adding a new configuration");
			this.addButton.Location = new Point(192, 88);
			this.addButton.Name = "addButton";
			this.helpProvider1.SetShowHelp(this.addButton, true);
			this.addButton.Size = new Size(96, 32);
			this.addButton.TabIndex = 5;
			this.addButton.Text = "&Add...";
			this.addButton.Click += new EventHandler(this.addButton_Click);
			// 
			// activeButton
			// 
			this.helpProvider1.SetHelpString(this.activeButton, "Makes the selected configuration active");
			this.activeButton.Location = new Point(192, 128);
			this.activeButton.Name = "activeButton";
			this.helpProvider1.SetShowHelp(this.activeButton, true);
			this.activeButton.Size = new Size(96, 32);
			this.activeButton.TabIndex = 6;
			this.activeButton.Text = "&Make Active";
			this.activeButton.Click += new EventHandler(this.activeButton_Click);
			// 
			// ConfigurationEditor
			// 
			this.AcceptButton = this.closeButton;
			this.CancelButton = this.closeButton;
			this.ClientSize = new Size(297, 267);
			this.ControlBox = false;
			this.Controls.Add(this.activeButton);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.renameButton);
			this.Controls.Add(this.removeButton);
			this.Controls.Add(this.configListBox);
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigurationEditor";
			this.helpProvider1.SetShowHelp(this, false);
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Configuration Editor";
			this.Load += new EventHandler(this.ConfigurationEditor_Load);
			this.ResumeLayout(false);
		}

		private void RenameConfiguration(string oldName)
		{
			using (RenameConfigurationDialog dlg =
				new RenameConfigurationDialog(this.project, oldName))
			{
				this.Site.Container.Add(dlg);
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					this.project.Configs[oldName].Name = dlg.ConfigurationName;
					this.FillListBox();
				}
			}
		}

		private void activeButton_Click(object sender, EventArgs e)
		{
			this.project.SetActiveConfig(this.selectedIndex);
			this.FillListBox();
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			using (AddConfigurationDialog dlg = new AddConfigurationDialog(this.project))
			{
				this.Site.Container.Add(dlg);
				if (dlg.ShowDialog() == DialogResult.OK)
					this.FillListBox();
			}
		}

		private void configListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.selectedIndex = this.configListBox.SelectedIndex;
			this.activeButton.Enabled = this.selectedIndex >= 0 && this.project.Configs[this.selectedIndex].Name != this.project.ActiveConfigName;
			this.renameButton.Enabled = this.addButton.Enabled = this.selectedIndex >= 0;
			this.removeButton.Enabled = this.selectedIndex >= 0 && this.configListBox.Items.Count > 0;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void removeButton_Click(object sender, EventArgs e)
		{
			if (this.project.Configs.Count == 1)
			{
				string msg = "Removing the last configuration will make the project unloadable until you add another configuration.\r\rAre you sure you want to remove the configuration?";

				if (this.MessageDisplay.Ask(msg) == DialogResult.No)
					return;
			}

			this.project.Configs.RemoveAt(this.selectedIndex);
			this.FillListBox();
		}

		private void renameButton_Click(object sender, EventArgs e)
		{
			this.RenameConfiguration(this.project.Configs[this.selectedIndex].Name);
		}

		#endregion
	}
}