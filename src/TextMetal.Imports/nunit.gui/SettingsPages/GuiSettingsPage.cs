// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NUnit.UiKit;
using NUnit.Util;

namespace NUnit.Gui.SettingsPages
{
	public class GuiSettingsPage : SettingsPage
	{
		#region Constructors/Destructors

		public GuiSettingsPage(string key)
			: base(key)
		{
			// This call is required by the Windows Form Designer.
			this.InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		#endregion

		#region Fields/Constants

		private CheckBox checkFilesExistCheckBox;
		private IContainer components = null;
		private RadioButton fullGuiRadioButton;

		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private HelpProvider helpProvider1;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;
		private CheckBox loadLastProjectCheckBox;
		private RadioButton miniGuiRadioButton;
		private TextBox recentFilesCountTextBox;

		#endregion

		#region Methods/Operators

		public override void ApplySettings()
		{
			string fmt = this.fullGuiRadioButton.Checked ? "Full" : "Mini";
			this.settings.SaveSetting("Gui.DisplayFormat", fmt);
			this.settings.SaveSetting("Gui.RecentProjects.CheckFilesExist", this.checkFilesExistCheckBox.Checked);
			this.settings.SaveSetting("Options.LoadLastProject", this.loadLastProjectCheckBox.Checked);
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
			this.label2 = new Label();
			this.groupBox2 = new GroupBox();
			this.label3 = new Label();
			this.recentFilesCountTextBox = new TextBox();
			this.label4 = new Label();
			this.loadLastProjectCheckBox = new CheckBox();
			this.fullGuiRadioButton = new RadioButton();
			this.miniGuiRadioButton = new RadioButton();
			this.helpProvider1 = new HelpProvider();
			this.checkFilesExistCheckBox = new CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new Point(8, 0);
			this.label1.Name = "label1";
			this.label1.Size = new Size(60, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Gui Display";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.groupBox1.Location = new Point(135, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(313, 8);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new Point(8, 96);
			this.label2.Name = "label2";
			this.label2.Size = new Size(66, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "Recent Files";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
			this.groupBox2.Location = new Point(135, 96);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(313, 8);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new Point(152, 120);
			this.label3.Name = "label3";
			this.label3.Size = new Size(65, 13);
			this.label3.TabIndex = 30;
			this.label3.Text = "files in menu";
			// 
			// recentFilesCountTextBox
			// 
			this.helpProvider1.SetHelpString(this.recentFilesCountTextBox, "The maximum number of files to display in the Recent Files list.");
			this.recentFilesCountTextBox.Location = new Point(96, 120);
			this.recentFilesCountTextBox.Name = "recentFilesCountTextBox";
			this.helpProvider1.SetShowHelp(this.recentFilesCountTextBox, true);
			this.recentFilesCountTextBox.Size = new Size(40, 20);
			this.recentFilesCountTextBox.TabIndex = 29;
			this.recentFilesCountTextBox.Validated += new EventHandler(this.recentFilesCountTextBox_Validated);
			this.recentFilesCountTextBox.Validating += new CancelEventHandler(this.recentFilesCountTextBox_Validating);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new Point(32, 120);
			this.label4.Name = "label4";
			this.label4.Size = new Size(23, 13);
			this.label4.TabIndex = 28;
			this.label4.Text = "List";
			// 
			// loadLastProjectCheckBox
			// 
			this.loadLastProjectCheckBox.AutoSize = true;
			this.helpProvider1.SetHelpString(this.loadLastProjectCheckBox, "If checked, most recent project is loaded at startup.");
			this.loadLastProjectCheckBox.Location = new Point(32, 198);
			this.loadLastProjectCheckBox.Name = "loadLastProjectCheckBox";
			this.helpProvider1.SetShowHelp(this.loadLastProjectCheckBox, true);
			this.loadLastProjectCheckBox.Size = new Size(193, 17);
			this.loadLastProjectCheckBox.TabIndex = 31;
			this.loadLastProjectCheckBox.Text = "Load most recent project at startup.";
			// 
			// fullGuiRadioButton
			// 
			this.fullGuiRadioButton.AutoSize = true;
			this.helpProvider1.SetHelpString(this.fullGuiRadioButton, "If selected, the full Gui is displayed, including the progress bar and output tab" +
																	"s.");
			this.fullGuiRadioButton.Location = new Point(32, 24);
			this.fullGuiRadioButton.Name = "fullGuiRadioButton";
			this.helpProvider1.SetShowHelp(this.fullGuiRadioButton, true);
			this.fullGuiRadioButton.Size = new Size(215, 17);
			this.fullGuiRadioButton.TabIndex = 32;
			this.fullGuiRadioButton.Text = "Full Gui with progress bar and result tabs";
			// 
			// miniGuiRadioButton
			// 
			this.miniGuiRadioButton.AutoSize = true;
			this.helpProvider1.SetHelpString(this.miniGuiRadioButton, "If selected, the mini-Gui, consisting of only the tree of tests, is displayed.");
			this.miniGuiRadioButton.Location = new Point(32, 56);
			this.miniGuiRadioButton.Name = "miniGuiRadioButton";
			this.helpProvider1.SetShowHelp(this.miniGuiRadioButton, true);
			this.miniGuiRadioButton.Size = new Size(148, 17);
			this.miniGuiRadioButton.TabIndex = 33;
			this.miniGuiRadioButton.Text = "Mini Gui showing tree only";
			// 
			// checkFilesExistCheckBox
			// 
			this.checkFilesExistCheckBox.AutoSize = true;
			this.checkFilesExistCheckBox.Location = new Point(32, 159);
			this.checkFilesExistCheckBox.Name = "checkFilesExistCheckBox";
			this.checkFilesExistCheckBox.Size = new Size(185, 17);
			this.checkFilesExistCheckBox.TabIndex = 34;
			this.checkFilesExistCheckBox.Text = "Check that files exist before listing";
			this.checkFilesExistCheckBox.UseVisualStyleBackColor = true;
			// 
			// GuiSettingsPage
			// 
			this.Controls.Add(this.checkFilesExistCheckBox);
			this.Controls.Add(this.miniGuiRadioButton);
			this.Controls.Add(this.fullGuiRadioButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.recentFilesCountTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.loadLastProjectCheckBox);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Name = "GuiSettingsPage";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		public override void LoadSettings()
		{
			string displayFormat = this.settings.GetSetting("Gui.DisplayFormat", "Full");
			switch (displayFormat)
			{
				case "Full":
					this.fullGuiRadioButton.Checked = true;
					break;
				case "Mini":
					this.miniGuiRadioButton.Checked = true;
					break;
			}

			this.recentFilesCountTextBox.Text = Services.RecentFiles.MaxFiles.ToString();
			this.checkFilesExistCheckBox.Checked = this.settings.GetSetting("Gui.RecentProjects.CheckFilesExist", true);
			this.loadLastProjectCheckBox.Checked = this.settings.GetSetting("Options.LoadLastProject", true);
		}

		private void recentFilesCountTextBox_Validated(object sender, EventArgs e)
		{
			int count = int.Parse(this.recentFilesCountTextBox.Text);
			Services.RecentFiles.MaxFiles = count;
			if (count == 0)
				this.loadLastProjectCheckBox.Checked = false;
		}

		private void recentFilesCountTextBox_Validating(object sender, CancelEventArgs e)
		{
			if (this.recentFilesCountTextBox.Text.Length == 0)
			{
				this.recentFilesCountTextBox.Text = Services.RecentFiles.MaxFiles.ToString();
				this.recentFilesCountTextBox.SelectAll();
				e.Cancel = true;
			}
			else
			{
				string errmsg = null;

				try
				{
					int count = int.Parse(this.recentFilesCountTextBox.Text);

					if (count < RecentFilesService.MinSize ||
						count > RecentFilesService.MaxSize)
					{
						errmsg = string.Format("Number of files must be from {0} to {1}",
							RecentFilesService.MinSize, RecentFilesService.MaxSize);
					}
				}
				catch
				{
					errmsg = "Number of files must be numeric";
				}

				if (errmsg != null)
				{
					this.recentFilesCountTextBox.SelectAll();
					this.MessageDisplay.Error(errmsg);
					e.Cancel = true;
				}
			}
		}

		#endregion
	}
}