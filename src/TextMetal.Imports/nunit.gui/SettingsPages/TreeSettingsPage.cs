// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using NUnit.UiKit;
using NUnit.Util;

namespace NUnit.Gui.SettingsPages
{
	public class TreeSettingsPage : SettingsPage
	{
		#region Constructors/Destructors

		public TreeSettingsPage(string key)
			: base(key)
		{
			// This call is required by the Windows Form Designer.
			this.InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		#endregion

		#region Fields/Constants

		private static string treeImageDir = PathUtils.Combine(Assembly.GetExecutingAssembly(), "Images", "Tree");
		private RadioButton autoNamespaceSuites;

		private CheckBox clearResultsCheckBox;
		private IContainer components = null;
		private PictureBox failureImage;
		private RadioButton flatTestList;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private HelpProvider helpProvider1;
		private PictureBox ignoredImage;
		private ListBox imageSetListBox;
		private PictureBox inconclusiveImage;
		private ComboBox initialDisplayComboBox;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label6;
		private CheckBox saveVisualStateCheckBox;
		private CheckBox showCheckBoxesCheckBox;
		private PictureBox skippedImage;
		private PictureBox successImage;

		#endregion

		#region Properties/Indexers/Events

		public override bool HasChangesRequiringReload
		{
			get
			{
				return this.settings.GetSetting("Options.TestLoader.AutoNamespaceSuites", true) != this.autoNamespaceSuites.Checked;
			}
		}

		#endregion

		#region Methods/Operators

		public override void ApplySettings()
		{
			this.settings.SaveSetting("Gui.TestTree.InitialTreeDisplay", (TestSuiteTreeView.DisplayStyle)this.initialDisplayComboBox.SelectedIndex);
			this.settings.SaveSetting("Options.TestLoader.ClearResultsOnReload", this.clearResultsCheckBox.Checked);
			this.settings.SaveSetting("Gui.TestTree.SaveVisualState", this.saveVisualStateCheckBox.Checked);
			this.settings.SaveSetting("Options.ShowCheckBoxes", this.showCheckBoxesCheckBox.Checked);

			if (this.imageSetListBox.SelectedIndex >= 0)
				this.settings.SaveSetting("Gui.TestTree.AlternateImageSet", this.imageSetListBox.SelectedItem);

			this.settings.SaveSetting("Options.TestLoader.AutoNamespaceSuites", this.autoNamespaceSuites.Checked);
		}

		private void DisplayImage(string imageDir, string filename, PictureBox box)
		{
			string[] extensions = { ".png", ".jpg" };

			foreach (string ext in extensions)
			{
				string filePath = Path.Combine(imageDir, filename + ext);
				if (File.Exists(filePath))
				{
					box.Load(filePath);
					break;
				}
			}
		}

		private void DisplayImageSet(string imageSet)
		{
			string imageSetDir = Path.Combine(treeImageDir, imageSet);

			this.DisplayImage(imageSetDir, "Success", this.successImage);
			this.DisplayImage(imageSetDir, "Failure", this.failureImage);
			this.DisplayImage(imageSetDir, "Ignored", this.ignoredImage);
			this.DisplayImage(imageSetDir, "Inconclusive", this.inconclusiveImage);
			this.DisplayImage(imageSetDir, "Skipped", this.skippedImage);
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(TreeSettingsPage));
			this.groupBox1 = new GroupBox();
			this.label1 = new Label();
			this.label2 = new Label();
			this.initialDisplayComboBox = new ComboBox();
			this.clearResultsCheckBox = new CheckBox();
			this.saveVisualStateCheckBox = new CheckBox();
			this.showCheckBoxesCheckBox = new CheckBox();
			this.helpProvider1 = new HelpProvider();
			this.flatTestList = new RadioButton();
			this.autoNamespaceSuites = new RadioButton();
			this.label3 = new Label();
			this.groupBox2 = new GroupBox();
			this.label6 = new Label();
			this.successImage = new PictureBox();
			this.failureImage = new PictureBox();
			this.ignoredImage = new PictureBox();
			this.inconclusiveImage = new PictureBox();
			this.skippedImage = new PictureBox();
			this.label4 = new Label();
			this.imageSetListBox = new ListBox();
			((ISupportInitialize)(this.successImage)).BeginInit();
			((ISupportInitialize)(this.failureImage)).BeginInit();
			((ISupportInitialize)(this.ignoredImage)).BeginInit();
			((ISupportInitialize)(this.inconclusiveImage)).BeginInit();
			((ISupportInitialize)(this.skippedImage)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.groupBox1.Location = new Point(144, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(304, 8);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new Point(8, 0);
			this.label1.Name = "label1";
			this.label1.Size = new Size(55, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "Tree View";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new Point(32, 24);
			this.label2.Name = "label2";
			this.label2.Size = new Size(104, 13);
			this.label2.TabIndex = 32;
			this.label2.Text = "Initial display on load";
			// 
			// initialDisplayComboBox
			// 
			this.initialDisplayComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.helpProvider1.SetHelpString(this.initialDisplayComboBox, "Selects the initial display style of the tree when an assembly is loaded");
			this.initialDisplayComboBox.ItemHeight = 13;
			this.initialDisplayComboBox.Items.AddRange(new object[]
														{
															"Auto",
															"Expand",
															"Collapse",
															"HideTests"
														});
			this.initialDisplayComboBox.Location = new Point(236, 24);
			this.initialDisplayComboBox.Name = "initialDisplayComboBox";
			this.helpProvider1.SetShowHelp(this.initialDisplayComboBox, true);
			this.initialDisplayComboBox.Size = new Size(168, 21);
			this.initialDisplayComboBox.TabIndex = 33;
			// 
			// clearResultsCheckBox
			// 
			this.clearResultsCheckBox.AutoSize = true;
			this.helpProvider1.SetHelpString(this.clearResultsCheckBox, "If checked, any prior results are cleared when reloading");
			this.clearResultsCheckBox.Location = new Point(32, 129);
			this.clearResultsCheckBox.Name = "clearResultsCheckBox";
			this.helpProvider1.SetShowHelp(this.clearResultsCheckBox, true);
			this.clearResultsCheckBox.Size = new Size(161, 17);
			this.clearResultsCheckBox.TabIndex = 34;
			this.clearResultsCheckBox.Text = "Clear results when reloading.";
			// 
			// saveVisualStateCheckBox
			// 
			this.saveVisualStateCheckBox.AutoSize = true;
			this.helpProvider1.SetHelpString(this.saveVisualStateCheckBox, "If checked, the visual state of the project is saved on exit. This includes selec" +
																			"ted tests, categories and the state of the tree itself.");
			this.saveVisualStateCheckBox.Location = new Point(32, 155);
			this.saveVisualStateCheckBox.Name = "saveVisualStateCheckBox";
			this.helpProvider1.SetShowHelp(this.saveVisualStateCheckBox, true);
			this.saveVisualStateCheckBox.Size = new Size(184, 17);
			this.saveVisualStateCheckBox.TabIndex = 35;
			this.saveVisualStateCheckBox.Text = "Save Visual State of each project";
			// 
			// showCheckBoxesCheckBox
			// 
			this.showCheckBoxesCheckBox.AutoSize = true;
			this.helpProvider1.SetHelpString(this.showCheckBoxesCheckBox, "If selected, the tree displays checkboxes for use in selecting multiple tests.");
			this.showCheckBoxesCheckBox.Location = new Point(32, 181);
			this.showCheckBoxesCheckBox.Name = "showCheckBoxesCheckBox";
			this.helpProvider1.SetShowHelp(this.showCheckBoxesCheckBox, true);
			this.showCheckBoxesCheckBox.Size = new Size(116, 17);
			this.showCheckBoxesCheckBox.TabIndex = 36;
			this.showCheckBoxesCheckBox.Text = "Show CheckBoxes";
			// 
			// flatTestList
			// 
			this.flatTestList.AutoCheck = false;
			this.flatTestList.AutoSize = true;
			this.helpProvider1.SetHelpString(this.flatTestList, "If selected, the tree will consist of a flat list of fixtures, without any higher" +
																"-level structure beyond the assemblies.");
			this.flatTestList.Location = new Point(32, 269);
			this.flatTestList.Name = "flatTestList";
			this.helpProvider1.SetShowHelp(this.flatTestList, true);
			this.flatTestList.Size = new Size(129, 17);
			this.flatTestList.TabIndex = 40;
			this.flatTestList.Text = "Flat list of TestFixtures";
			this.flatTestList.Click += new EventHandler(this.toggleTestStructure);
			// 
			// autoNamespaceSuites
			// 
			this.autoNamespaceSuites.AutoCheck = false;
			this.autoNamespaceSuites.AutoSize = true;
			this.autoNamespaceSuites.Checked = true;
			this.helpProvider1.SetHelpString(this.autoNamespaceSuites, "If selected, the tree will follow the namespace structure of the tests, with suit" +
																		"es automatically created at each level.");
			this.autoNamespaceSuites.Location = new Point(32, 243);
			this.autoNamespaceSuites.Name = "autoNamespaceSuites";
			this.helpProvider1.SetShowHelp(this.autoNamespaceSuites, true);
			this.autoNamespaceSuites.Size = new Size(162, 17);
			this.autoNamespaceSuites.TabIndex = 39;
			this.autoNamespaceSuites.TabStop = true;
			this.autoNamespaceSuites.Text = "Automatic Namespace suites";
			this.autoNamespaceSuites.Click += new EventHandler(this.toggleTestStructure);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new Point(8, 218);
			this.label3.Name = "label3";
			this.label3.Size = new Size(74, 13);
			this.label3.TabIndex = 38;
			this.label3.Text = "Test Structure";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.groupBox2.Location = new Point(144, 218);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(304, 8);
			this.groupBox2.TabIndex = 37;
			this.groupBox2.TabStop = false;
			// 
			// label6
			// 
			this.label6.BackColor = SystemColors.Window;
			this.label6.BorderStyle = BorderStyle.FixedSingle;
			this.label6.Location = new Point(66, 81);
			this.label6.Name = "label6";
			this.label6.Size = new Size(137, 36);
			this.label6.TabIndex = 47;
			// 
			// successImage
			// 
			this.successImage.Image = ((Image)(resources.GetObject("successImage.Image")));
			this.successImage.Location = new Point(78, 92);
			this.successImage.Name = "successImage";
			this.successImage.Size = new Size(16, 16);
			this.successImage.SizeMode = PictureBoxSizeMode.AutoSize;
			this.successImage.TabIndex = 48;
			this.successImage.TabStop = false;
			// 
			// failureImage
			// 
			this.failureImage.Image = ((Image)(resources.GetObject("failureImage.Image")));
			this.failureImage.Location = new Point(103, 92);
			this.failureImage.Name = "failureImage";
			this.failureImage.Size = new Size(16, 16);
			this.failureImage.SizeMode = PictureBoxSizeMode.AutoSize;
			this.failureImage.TabIndex = 49;
			this.failureImage.TabStop = false;
			// 
			// ignoredImage
			// 
			this.ignoredImage.Image = ((Image)(resources.GetObject("ignoredImage.Image")));
			this.ignoredImage.Location = new Point(128, 92);
			this.ignoredImage.Name = "ignoredImage";
			this.ignoredImage.Size = new Size(16, 16);
			this.ignoredImage.SizeMode = PictureBoxSizeMode.AutoSize;
			this.ignoredImage.TabIndex = 50;
			this.ignoredImage.TabStop = false;
			// 
			// inconclusiveImage
			// 
			this.inconclusiveImage.Image = ((Image)(resources.GetObject("inconclusiveImage.Image")));
			this.inconclusiveImage.Location = new Point(152, 92);
			this.inconclusiveImage.Name = "inconclusiveImage";
			this.inconclusiveImage.Size = new Size(16, 16);
			this.inconclusiveImage.SizeMode = PictureBoxSizeMode.AutoSize;
			this.inconclusiveImage.TabIndex = 51;
			this.inconclusiveImage.TabStop = false;
			// 
			// skippedImage
			// 
			this.skippedImage.Image = ((Image)(resources.GetObject("skippedImage.Image")));
			this.skippedImage.Location = new Point(177, 92);
			this.skippedImage.Name = "skippedImage";
			this.skippedImage.Size = new Size(16, 16);
			this.skippedImage.SizeMode = PictureBoxSizeMode.AutoSize;
			this.skippedImage.TabIndex = 52;
			this.skippedImage.TabStop = false;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new Point(32, 60);
			this.label4.Name = "label4";
			this.label4.Size = new Size(66, 13);
			this.label4.TabIndex = 53;
			this.label4.Text = "Tree Images";
			// 
			// imageSetListBox
			// 
			this.imageSetListBox.FormattingEnabled = true;
			this.imageSetListBox.Location = new Point(236, 61);
			this.imageSetListBox.Name = "imageSetListBox";
			this.imageSetListBox.Size = new Size(168, 56);
			this.imageSetListBox.TabIndex = 54;
			this.imageSetListBox.SelectedIndexChanged += new EventHandler(this.imageSetListBox_SelectedIndexChanged);
			// 
			// TreeSettingsPage
			// 
			this.Controls.Add(this.imageSetListBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.skippedImage);
			this.Controls.Add(this.inconclusiveImage);
			this.Controls.Add(this.ignoredImage);
			this.Controls.Add(this.failureImage);
			this.Controls.Add(this.successImage);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.flatTestList);
			this.Controls.Add(this.autoNamespaceSuites);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.showCheckBoxesCheckBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.initialDisplayComboBox);
			this.Controls.Add(this.clearResultsCheckBox);
			this.Controls.Add(this.saveVisualStateCheckBox);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Name = "TreeSettingsPage";
			((ISupportInitialize)(this.successImage)).EndInit();
			((ISupportInitialize)(this.failureImage)).EndInit();
			((ISupportInitialize)(this.ignoredImage)).EndInit();
			((ISupportInitialize)(this.inconclusiveImage)).EndInit();
			((ISupportInitialize)(this.skippedImage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		public override void LoadSettings()
		{
			this.initialDisplayComboBox.SelectedIndex = (int)(TestSuiteTreeView.DisplayStyle)this.settings.GetSetting("Gui.TestTree.InitialTreeDisplay", TestSuiteTreeView.DisplayStyle.Auto);
			this.clearResultsCheckBox.Checked = this.settings.GetSetting("Options.TestLoader.ClearResultsOnReload", true);
			this.saveVisualStateCheckBox.Checked = this.settings.GetSetting("Gui.TestTree.SaveVisualState", true);
			this.showCheckBoxesCheckBox.Checked = this.settings.GetSetting("Options.ShowCheckBoxes", false);

			string[] altDirs = Directory.Exists(treeImageDir)
				? Directory.GetDirectories(treeImageDir)
				: new string[0];

			foreach (string altDir in altDirs)
				this.imageSetListBox.Items.Add(Path.GetFileName(altDir));
			string imageSet = this.settings.GetSetting("Gui.TestTree.AlternateImageSet", "Default");
			if (this.imageSetListBox.Items.Contains(imageSet))
				this.imageSetListBox.SelectedItem = imageSet;

			this.autoNamespaceSuites.Checked = this.settings.GetSetting("Options.TestLoader.AutoNamespaceSuites", true);
			this.flatTestList.Checked = !this.autoNamespaceSuites.Checked;
		}

		private void imageSetListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			string imageSet = this.imageSetListBox.SelectedItem as string;

			if (imageSet != null)
				this.DisplayImageSet(imageSet);
		}

		private void toggleTestStructure(object sender, EventArgs e)
		{
			bool auto = this.autoNamespaceSuites.Checked = !this.autoNamespaceSuites.Checked;
			this.flatTestList.Checked = !auto;
		}

		#endregion
	}
}