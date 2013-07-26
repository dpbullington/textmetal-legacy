// ****************************************************************
// Copyright 2012, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Windows.Forms;

using NUnit.UiKit;

namespace NUnit.Gui.SettingsPages
{
	public partial class ProjectEditorSettingsPage : SettingsPage
	{
		#region Constructors/Destructors

		public ProjectEditorSettingsPage(string key)
			: base(key)
		{
			this.InitializeComponent();
		}

		#endregion

		#region Fields/Constants

		private static readonly string EDITOR_PATH_SETTING = "Options.ProjectEditor.EditorPath";

		#endregion

		#region Methods/Operators

		public override void ApplySettings()
		{
			if (this.useNUnitEditorRadioButton.Checked)
				this.settings.RemoveSetting(EDITOR_PATH_SETTING);
			else
				this.settings.SaveSetting(EDITOR_PATH_SETTING, this.editorPathTextBox.Text);
		}

		public override void LoadSettings()
		{
			string editorPath = (string)this.settings.GetSetting(EDITOR_PATH_SETTING);

			if (editorPath != null)
			{
				this.useOtherEditorRadioButton.Checked = true;
				this.editorPathTextBox.Text = editorPath;
			}
			else
			{
				this.useNUnitEditorRadioButton.Checked = true;
				this.editorPathTextBox.Text = "";
			}
		}

		private void editorPathBrowseButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			if (this.Site != null)
				dlg.Site = this.Site;
			dlg.Title = "Select Project Editor";

			dlg.Filter = "Executable Files (*.exe)|*.exe";
			dlg.FilterIndex = 1;
			dlg.FileName = "";

			if (dlg.ShowDialog(this) == DialogResult.OK)
				this.editorPathTextBox.Text = dlg.FileName;
		}

		private void editorPathTextBox_TextChanged(object sender, EventArgs e)
		{
			if (this.editorPathTextBox.TextLength == 0)
				this.useNUnitEditorRadioButton.Checked = true;
			else
				this.useOtherEditorRadioButton.Checked = true;
		}

		#endregion
	}
}