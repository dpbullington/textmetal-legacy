// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NUnit.Core;
using NUnit.Core.Filters;
using NUnit.Util;

namespace NUnit.UiKit
{
	public delegate void SelectedTestsChangedEventHandler(object sender, SelectedTestsChangedEventArgs e);

	/// <summary>
	/// Summary description for TestTree.
	/// </summary>
	public class TestTree : UserControl
	{
		// Contains all available categories, whether
		// selected or not. Unselected members of this
		// list are displayed in selectedList

		#region Constructors/Destructors

		public TestTree()
		{
			// This call is required by the Windows.Forms Form Designer.
			this.InitializeComponent();
			this.treeMenu = new MenuItem();
			this.checkBoxesMenuItem = new MenuItem();
			this.treeMenuSeparatorItem1 = new MenuItem();
			this.expandMenuItem = new MenuItem();
			this.collapseMenuItem = new MenuItem();
			this.treeMenuSeparatorItem2 = new MenuItem();
			this.expandAllMenuItem = new MenuItem();
			this.collapseAllMenuItem = new MenuItem();
			this.hideTestsMenuItem = new MenuItem();
			this.treeMenuSeparatorItem3 = new MenuItem();
			this.propertiesMenuItem = new MenuItem();

			// 
			// treeMenu
			// 
			this.treeMenu.MergeType = MenuMerge.Add;
			this.treeMenu.MergeOrder = 1;
			this.treeMenu.MenuItems.AddRange(
				new MenuItem[]
				{
					this.checkBoxesMenuItem,
					this.treeMenuSeparatorItem1,
					this.expandMenuItem,
					this.collapseMenuItem,
					this.treeMenuSeparatorItem2,
					this.expandAllMenuItem,
					this.collapseAllMenuItem,
					this.hideTestsMenuItem,
					this.treeMenuSeparatorItem3,
					this.propertiesMenuItem
				});
			this.treeMenu.Text = "&Tree";
			this.treeMenu.Visible = false;
			this.treeMenu.Popup += new EventHandler(this.treeMenu_Popup);
			// 
			// checkBoxesMenuItem
			// 
			this.checkBoxesMenuItem.Index = 0;
			this.checkBoxesMenuItem.Text = "Show Check&Boxes";
			this.checkBoxesMenuItem.Click += new EventHandler(this.checkBoxesMenuItem_Click);
			// 
			// treeMenuSeparatorItem1
			// 
			this.treeMenuSeparatorItem1.Index = 1;
			this.treeMenuSeparatorItem1.Text = "-";
			// 
			// expandMenuItem
			// 
			this.expandMenuItem.Index = 2;
			this.expandMenuItem.Text = "&Expand";
			this.expandMenuItem.Click += new EventHandler(this.expandMenuItem_Click);
			// 
			// collapseMenuItem
			// 
			this.collapseMenuItem.Index = 3;
			this.collapseMenuItem.Text = "&Collapse";
			this.collapseMenuItem.Click += new EventHandler(this.collapseMenuItem_Click);
			// 
			// treeMenuSeparatorItem2
			// 
			this.treeMenuSeparatorItem2.Index = 4;
			this.treeMenuSeparatorItem2.Text = "-";
			// 
			// expandAllMenuItem
			// 
			this.expandAllMenuItem.Index = 5;
			this.expandAllMenuItem.Text = "Expand All";
			this.expandAllMenuItem.Click += new EventHandler(this.expandAllMenuItem_Click);
			// 
			// collapseAllMenuItem
			// 
			this.collapseAllMenuItem.Index = 6;
			this.collapseAllMenuItem.Text = "Collapse All";
			this.collapseAllMenuItem.Click += new EventHandler(this.collapseAllMenuItem_Click);
			// 
			// hideTestsMenuItem
			// 
			this.hideTestsMenuItem.Index = 7;
			this.hideTestsMenuItem.Text = "Hide Tests";
			this.hideTestsMenuItem.Click += new EventHandler(this.hideTestsMenuItem_Click);
			// 
			// treeMenuSeparatorItem3
			// 
			this.treeMenuSeparatorItem3.Index = 8;
			this.treeMenuSeparatorItem3.Text = "-";
			// 
			// propertiesMenuItem
			// 
			this.propertiesMenuItem.Index = 9;
			this.propertiesMenuItem.Text = "&Properties";
			this.propertiesMenuItem.Click += new EventHandler(this.propertiesMenuItem_Click);

			this.tests.SelectedTestChanged += new SelectedTestChangedHandler(this.tests_SelectedTestChanged);
			this.tests.CheckedTestChanged += new CheckedTestChangedHandler(this.tests_CheckedTestChanged);

			this.excludeCheckbox.Enabled = false;
		}

		#endregion

		#region Fields/Constants

		private Button addCategory;

		private IList availableCategories = new List<string>();
		private ListBox availableList;
		private Panel buttonPanel;
		private Panel categoryButtonPanel;

		// Our test loader
		private TabPage categoryPage;
		private Panel categoryPanel;
		private MenuItem checkBoxesMenuItem;
		private Button checkFailedButton;
		private Button clearAllButton;
		private MenuItem collapseAllMenuItem;
		private MenuItem collapseMenuItem;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private CheckBox excludeCheckbox;
		private MenuItem expandAllMenuItem;
		private MenuItem expandMenuItem;

		private GroupBox groupBox1;
		private MenuItem hideTestsMenuItem;
		private TestLoader loader;
		private MenuItem propertiesMenuItem;
		private Button removeCategory;
		private GroupBox selectedCategories;
		private ListBox selectedList;
		private TabControl tabs;
		private TabPage testPage;
		private Panel testPanel;
		private TestSuiteTreeView tests;
		private MenuItem treeMenu;
		private MenuItem treeMenuSeparatorItem1;
		private MenuItem treeMenuSeparatorItem2;
		private MenuItem treeMenuSeparatorItem3;
		private Panel treePanel;

		#endregion

		#region Properties/Indexers/Events

		public event SelectedTestsChangedEventHandler SelectedTestsChanged;

		public string[] SelectedCategories
		{
			get
			{
				int n = this.selectedList.Items.Count;
				string[] categories = new string[n];
				for (int i = 0; i < n; i++)
					categories[i] = this.selectedList.Items[i].ToString();
				return categories;
			}
		}

		[Browsable(false)]
		public bool ShowCheckBoxes
		{
			get
			{
				return this.tests.CheckBoxes;
			}
			set
			{
				this.tests.CheckBoxes = value;
				this.buttonPanel.Visible = value;
				this.clearAllButton.Visible = value;
				this.checkFailedButton.Visible = value;
				this.checkBoxesMenuItem.Checked = value;
			}
		}

		public MenuItem TreeMenu
		{
			get
			{
				return this.treeMenu;
			}
		}

		#endregion

		#region Methods/Operators

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

		public void Initialize(TestLoader loader)
		{
			this.tests.Initialize(loader, loader.Events);
			this.loader = loader;
			loader.Events.TestLoaded += new TestEventHandler(this.events_TestLoaded);
			loader.Events.TestReloaded += new TestEventHandler(this.events_TestReloaded);
			loader.Events.TestUnloaded += new TestEventHandler(this.events_TestUnloaded);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tabs = new TabControl();
			this.testPage = new TabPage();
			this.testPanel = new Panel();
			this.treePanel = new Panel();
			this.tests = new TestSuiteTreeView();
			this.buttonPanel = new Panel();
			this.checkFailedButton = new Button();
			this.clearAllButton = new Button();
			this.categoryPage = new TabPage();
			this.categoryPanel = new Panel();
			this.categoryButtonPanel = new Panel();
			this.removeCategory = new Button();
			this.addCategory = new Button();
			this.selectedCategories = new GroupBox();
			this.selectedList = new ListBox();
			this.excludeCheckbox = new CheckBox();
			this.groupBox1 = new GroupBox();
			this.availableList = new ListBox();
			this.tabs.SuspendLayout();
			this.testPage.SuspendLayout();
			this.testPanel.SuspendLayout();
			this.treePanel.SuspendLayout();
			this.buttonPanel.SuspendLayout();
			this.categoryPage.SuspendLayout();
			this.categoryPanel.SuspendLayout();
			this.categoryButtonPanel.SuspendLayout();
			this.selectedCategories.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabs
			// 
			this.tabs.Alignment = TabAlignment.Left;
			this.tabs.Controls.Add(this.testPage);
			this.tabs.Controls.Add(this.categoryPage);
			this.tabs.Dock = DockStyle.Fill;
			this.tabs.Location = new Point(0, 0);
			this.tabs.Multiline = true;
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new Size(248, 496);
			this.tabs.TabIndex = 0;
			// 
			// testPage
			// 
			this.testPage.Controls.Add(this.testPanel);
			this.testPage.Location = new Point(25, 4);
			this.testPage.Name = "testPage";
			this.testPage.Size = new Size(219, 488);
			this.testPage.TabIndex = 0;
			this.testPage.Text = "Tests";
			// 
			// testPanel
			// 
			this.testPanel.Controls.Add(this.treePanel);
			this.testPanel.Controls.Add(this.buttonPanel);
			this.testPanel.Dock = DockStyle.Fill;
			this.testPanel.Location = new Point(0, 0);
			this.testPanel.Name = "testPanel";
			this.testPanel.Size = new Size(219, 488);
			this.testPanel.TabIndex = 0;
			// 
			// treePanel
			// 
			this.treePanel.Controls.Add(this.tests);
			this.treePanel.Dock = DockStyle.Fill;
			this.treePanel.Location = new Point(0, 0);
			this.treePanel.Name = "treePanel";
			this.treePanel.Size = new Size(219, 448);
			this.treePanel.TabIndex = 0;
			// 
			// tests
			// 
			this.tests.AllowDrop = true;
			this.tests.Dock = DockStyle.Fill;
			this.tests.HideSelection = false;
			this.tests.Location = new Point(0, 0);
			this.tests.Name = "tests";
			this.tests.Size = new Size(219, 448);
			this.tests.TabIndex = 0;
			this.tests.CheckBoxesChanged += new EventHandler(this.tests_CheckBoxesChanged);
			// 
			// buttonPanel
			// 
			this.buttonPanel.Controls.Add(this.checkFailedButton);
			this.buttonPanel.Controls.Add(this.clearAllButton);
			this.buttonPanel.Dock = DockStyle.Bottom;
			this.buttonPanel.Location = new Point(0, 448);
			this.buttonPanel.Name = "buttonPanel";
			this.buttonPanel.Size = new Size(219, 40);
			this.buttonPanel.TabIndex = 1;
			// 
			// checkFailedButton
			// 
			this.checkFailedButton.Anchor = AnchorStyles.Top;
			this.checkFailedButton.Location = new Point(117, 8);
			this.checkFailedButton.Name = "checkFailedButton";
			this.checkFailedButton.Size = new Size(96, 23);
			this.checkFailedButton.TabIndex = 1;
			this.checkFailedButton.Text = "Check Failed";
			this.checkFailedButton.Click += new EventHandler(this.checkFailedButton_Click);
			// 
			// clearAllButton
			// 
			this.clearAllButton.Anchor = AnchorStyles.Top;
			this.clearAllButton.Location = new Point(13, 8);
			this.clearAllButton.Name = "clearAllButton";
			this.clearAllButton.Size = new Size(96, 23);
			this.clearAllButton.TabIndex = 0;
			this.clearAllButton.Text = "Clear All";
			this.clearAllButton.Click += new EventHandler(this.clearAllButton_Click);
			// 
			// categoryPage
			// 
			this.categoryPage.Controls.Add(this.categoryPanel);
			this.categoryPage.Location = new Point(25, 4);
			this.categoryPage.Name = "categoryPage";
			this.categoryPage.Size = new Size(219, 488);
			this.categoryPage.TabIndex = 1;
			this.categoryPage.Text = "Categories";
			// 
			// categoryPanel
			// 
			this.categoryPanel.Controls.Add(this.categoryButtonPanel);
			this.categoryPanel.Controls.Add(this.selectedCategories);
			this.categoryPanel.Controls.Add(this.groupBox1);
			this.categoryPanel.Dock = DockStyle.Fill;
			this.categoryPanel.Location = new Point(0, 0);
			this.categoryPanel.Name = "categoryPanel";
			this.categoryPanel.Size = new Size(219, 488);
			this.categoryPanel.TabIndex = 0;
			// 
			// categoryButtonPanel
			// 
			this.categoryButtonPanel.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left)
																| AnchorStyles.Right)));
			this.categoryButtonPanel.Controls.Add(this.removeCategory);
			this.categoryButtonPanel.Controls.Add(this.addCategory);
			this.categoryButtonPanel.Location = new Point(8, 280);
			this.categoryButtonPanel.Name = "categoryButtonPanel";
			this.categoryButtonPanel.Size = new Size(203, 40);
			this.categoryButtonPanel.TabIndex = 1;
			// 
			// removeCategory
			// 
			this.removeCategory.Anchor = AnchorStyles.Top;
			this.removeCategory.Location = new Point(109, 8);
			this.removeCategory.Name = "removeCategory";
			this.removeCategory.TabIndex = 1;
			this.removeCategory.Text = "Remove";
			this.removeCategory.Click += new EventHandler(this.removeCategory_Click);
			// 
			// addCategory
			// 
			this.addCategory.Anchor = AnchorStyles.Top;
			this.addCategory.Location = new Point(21, 8);
			this.addCategory.Name = "addCategory";
			this.addCategory.TabIndex = 0;
			this.addCategory.Text = "Add";
			this.addCategory.Click += new EventHandler(this.addCategory_Click);
			// 
			// selectedCategories
			// 
			this.selectedCategories.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left)
															| AnchorStyles.Right)));
			this.selectedCategories.Controls.Add(this.selectedList);
			this.selectedCategories.Controls.Add(this.excludeCheckbox);
			this.selectedCategories.Location = new Point(8, 328);
			this.selectedCategories.Name = "selectedCategories";
			this.selectedCategories.Size = new Size(203, 144);
			this.selectedCategories.TabIndex = 2;
			this.selectedCategories.TabStop = false;
			this.selectedCategories.Text = "Selected Categories";
			// 
			// selectedList
			// 
			this.selectedList.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
														| AnchorStyles.Left)
														| AnchorStyles.Right)));
			this.selectedList.ItemHeight = 16;
			this.selectedList.Location = new Point(8, 16);
			this.selectedList.Name = "selectedList";
			this.selectedList.SelectionMode = SelectionMode.MultiExtended;
			this.selectedList.Size = new Size(187, 84);
			this.selectedList.TabIndex = 0;
			this.selectedList.DoubleClick += new EventHandler(this.removeCategory_Click);
			// 
			// excludeCheckbox
			// 
			this.excludeCheckbox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
															| AnchorStyles.Right)));
			this.excludeCheckbox.Location = new Point(8, 120);
			this.excludeCheckbox.Name = "excludeCheckbox";
			this.excludeCheckbox.Size = new Size(179, 16);
			this.excludeCheckbox.TabIndex = 1;
			this.excludeCheckbox.Text = "Exclude these categories";
			this.excludeCheckbox.CheckedChanged += new EventHandler(this.excludeCheckbox_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
													| AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.availableList);
			this.groupBox1.Location = new Point(8, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(203, 272);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Available Categories";
			// 
			// availableList
			// 
			this.availableList.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
														| AnchorStyles.Left)
														| AnchorStyles.Right)));
			this.availableList.ItemHeight = 16;
			this.availableList.Location = new Point(8, 24);
			this.availableList.Name = "availableList";
			this.availableList.SelectionMode = SelectionMode.MultiExtended;
			this.availableList.Size = new Size(187, 244);
			this.availableList.TabIndex = 0;
			this.availableList.DoubleClick += new EventHandler(this.addCategory_Click);
			// 
			// TestTree
			// 
			this.Controls.Add(this.tabs);
			this.Name = "TestTree";
			this.Size = new Size(248, 496);
			this.tabs.ResumeLayout(false);
			this.testPage.ResumeLayout(false);
			this.testPanel.ResumeLayout(false);
			this.treePanel.ResumeLayout(false);
			this.buttonPanel.ResumeLayout(false);
			this.categoryPage.ResumeLayout(false);
			this.categoryPanel.ResumeLayout(false);
			this.categoryButtonPanel.ResumeLayout(false);
			this.selectedCategories.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!this.DesignMode)
			{
				this.ShowCheckBoxes =
					Services.UserSettings.GetSetting("Options.ShowCheckBoxes", false);
				this.Initialize(Services.TestLoader);
				Services.UserSettings.Changed += new SettingsEventHandler(this.UserSettings_Changed);
			}

			base.OnLoad(e);
		}

		public void RunAllTests()
		{
			this.RunAllTests(true);
		}

		public void RunAllTests(bool ignoreTests)
		{
			this.tests.RunAllTests(ignoreTests);
		}

		public void RunFailedTests()
		{
			this.tests.RunFailedTests();
		}

		public void RunSelectedTests()
		{
			this.tests.RunSelectedTests();
		}

		public void SelectCategories(string[] categories, bool exclude)
		{
			foreach (string category in categories)
			{
				if (this.availableCategories.Contains(category))
				{
					if (!this.selectedList.Items.Contains(category))
						this.selectedList.Items.Add(category);
					this.availableList.Items.Remove(category);

					this.excludeCheckbox.Checked = exclude;
				}
			}

			this.UpdateCategoryFilter();
			if (this.SelectedCategories.Length > 0)
				this.excludeCheckbox.Enabled = true;
		}

		private void UpdateCategoryFilter()
		{
			TestFilter catFilter;

			if (this.SelectedCategories == null || this.SelectedCategories.Length == 0)
				catFilter = TestFilter.Empty;
			else
				catFilter = new CategoryFilter(this.SelectedCategories);

			if (this.excludeCheckbox.Checked)
				catFilter = new NotFilter(catFilter, true);

			this.tests.CategoryFilter = catFilter;
		}

		private void UserSettings_Changed(object sender, SettingsEventArgs args)
		{
			if (args.SettingName == "Options.ShowCheckBoxes")
				this.ShowCheckBoxes = Services.UserSettings.GetSetting(args.SettingName, false);
		}

		private void addCategory_Click(object sender, EventArgs e)
		{
			if (this.availableList.SelectedItems.Count > 0)
			{
				// Create a separate list to avoid exception
				// when using the list box directly.
				List<string> categories = new List<string>();
				foreach (string category in this.availableList.SelectedItems)
					categories.Add(category);

				foreach (string category in categories)
				{
					this.selectedList.Items.Add(category);
					this.availableList.Items.Remove(category);
				}

				this.UpdateCategoryFilter();
				if (this.SelectedCategories.Length > 0)
					this.excludeCheckbox.Enabled = true;
			}
		}

		private void checkBoxesMenuItem_Click(object sender, EventArgs e)
		{
			Services.UserSettings.SaveSetting("Options.ShowCheckBoxes",
				this.ShowCheckBoxes = !this.checkBoxesMenuItem.Checked);

			// Temporary till we can save tree state and restore
			//this.SetInitialExpansion();
		}

		private void checkFailedButton_Click(object sender, EventArgs e)
		{
			this.tests.CheckFailedNodes();
		}

		private void clearAllButton_Click(object sender, EventArgs e)
		{
			this.tests.ClearCheckedNodes();
		}

		private void collapseAllMenuItem_Click(object sender, EventArgs e)
		{
			this.tests.BeginUpdate();
			this.tests.CollapseAll();
			this.tests.EndUpdate();

			// Compensate for a bug in the underlying control
			if (this.tests.Nodes.Count > 0)
				this.tests.SelectedNode = this.tests.Nodes[0];
		}

		private void collapseMenuItem_Click(object sender, EventArgs e)
		{
			this.tests.SelectedNode.Collapse();
		}

		private void events_TestLoaded(object sender, TestEventArgs args)
		{
			this.treeMenu.Visible = true;

			this.availableCategories = this.loader.GetCategories();
			this.availableList.Items.Clear();
			this.selectedList.Items.Clear();

			this.availableList.SuspendLayout();
			foreach (string category in this.availableCategories)
				this.availableList.Items.Add(category);

			// tree may have restored visual state
			if (!this.tests.CategoryFilter.IsEmpty)
			{
				ITestFilter filter = this.tests.CategoryFilter;
				if (filter is NotFilter)
				{
					filter = ((NotFilter)filter).BaseFilter;
					this.excludeCheckbox.Checked = true;
				}

				foreach (string cat in ((CategoryFilter)filter).Categories)
				{
					if (this.availableCategories.Contains(cat))
					{
						this.availableList.Items.Remove(cat);
						this.selectedList.Items.Add(cat);
						this.excludeCheckbox.Enabled = true;
					}
				}

				this.UpdateCategoryFilter();
			}

			this.availableList.ResumeLayout();
		}

		private void events_TestReloaded(object sender, TestEventArgs args)
		{
			// Get new list of available categories
			this.availableCategories = this.loader.GetCategories();

			// Remove any selected items that are no longer available
			int index = this.selectedList.Items.Count;
			this.selectedList.SuspendLayout();
			while (--index >= 0)
			{
				string category = this.selectedList.Items[index].ToString();
				if (!this.availableCategories.Contains(category))
					this.selectedList.Items.RemoveAt(index);
			}
			this.selectedList.ResumeLayout();

			// Clear check box if there are no more selected items.
			if (this.selectedList.Items.Count == 0)
				this.excludeCheckbox.Checked = this.excludeCheckbox.Enabled = false;

			// Put any unselected available items on availableList
			this.availableList.Items.Clear();
			this.availableList.SuspendLayout();
			foreach (string category in this.availableCategories)
			{
				if (this.selectedList.FindStringExact(category) < 0)
					this.availableList.Items.Add(category);
			}
			this.availableList.ResumeLayout();

			// Tell the tree what is selected
			this.UpdateCategoryFilter();
		}

		private void events_TestUnloaded(object sender, TestEventArgs args)
		{
			this.availableCategories.Clear();
			this.availableList.Items.Clear();
			this.selectedList.Items.Clear();
			this.excludeCheckbox.Checked = false;
			this.excludeCheckbox.Enabled = false;
			this.treeMenu.Visible = false;
		}

		private void excludeCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			this.UpdateCategoryFilter();
		}

		private void expandAllMenuItem_Click(object sender, EventArgs e)
		{
			this.tests.BeginUpdate();
			this.tests.ExpandAll();
			this.tests.EndUpdate();
		}

		private void expandMenuItem_Click(object sender, EventArgs e)
		{
			this.tests.SelectedNode.Expand();
		}

		private void hideTestsMenuItem_Click(object sender, EventArgs e)
		{
			this.tests.HideTests();
		}

		private void propertiesMenuItem_Click(object sender, EventArgs e)
		{
			if (this.tests.SelectedTest != null)
				this.tests.ShowPropertiesDialog(this.tests.SelectedTest);
		}

		private void removeCategory_Click(object sender, EventArgs e)
		{
			if (this.selectedList.SelectedItems.Count > 0)
			{
				// Create a separate list to avoid exception
				// when using the list box directly.
				List<string> categories = new List<string>();
				foreach (string category in this.selectedList.SelectedItems)
					categories.Add(category);

				foreach (string category in categories)
				{
					this.selectedList.Items.Remove(category);
					this.availableList.Items.Add(category);
				}

				this.UpdateCategoryFilter();
				if (this.SelectedCategories.Length == 0)
				{
					this.excludeCheckbox.Checked = false;
					this.excludeCheckbox.Enabled = false;
				}
			}
		}

		private void tests_CheckBoxesChanged(object sender, EventArgs e)
		{
			this.ShowCheckBoxes = this.tests.CheckBoxes;
		}

		private void tests_CheckedTestChanged(ITest[] tests)
		{
			if (this.SelectedTestsChanged != null)
			{
				SelectedTestsChangedEventArgs args = new SelectedTestsChangedEventArgs("", tests.Length);
				this.SelectedTestsChanged(tests, args);
			}

			if (tests.Length > 0)
			{
			}
		}

		private void tests_SelectedTestChanged(ITest test)
		{
			if (this.SelectedTestsChanged != null)
			{
				SelectedTestsChangedEventArgs args = new SelectedTestsChangedEventArgs(test.TestName.Name, test.TestCount);
				this.SelectedTestsChanged(this.tests, args);
			}
		}

		private void treeMenu_Popup(object sender, EventArgs e)
		{
			TreeNode selectedNode = this.tests.SelectedNode;
			if (selectedNode != null && selectedNode.Nodes.Count > 0)
			{
				bool isExpanded = selectedNode.IsExpanded;
				this.collapseMenuItem.Enabled = isExpanded;
				this.expandMenuItem.Enabled = !isExpanded;
			}
			else
				this.collapseMenuItem.Enabled = this.expandMenuItem.Enabled = false;
		}

		#endregion
	}

	public class SelectedTestsChangedEventArgs : EventArgs
	{
		#region Constructors/Destructors

		public SelectedTestsChangedEventArgs(string testName, int count)
		{
			this.testName = testName;
			this.count = count;
		}

		#endregion

		#region Fields/Constants

		private int count;
		private string testName;

		#endregion

		#region Properties/Indexers/Events

		public int TestCount
		{
			get
			{
				return this.count;
			}
		}

		public string TestName
		{
			get
			{
				return this.testName;
			}
		}

		#endregion
	}
}