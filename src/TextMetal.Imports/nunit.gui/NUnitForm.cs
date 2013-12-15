// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using CP.Windows.Forms;

using NUnit.Core;
using NUnit.UiKit;
using NUnit.Util;

using StatusBar = NUnit.UiKit.StatusBar;
using TestAction = NUnit.Util.TestAction;

namespace NUnit.Gui
{
	public class NUnitForm : NUnitFormBase
	{
		#region Constructors/Destructors

		public NUnitForm(GuiOptions guiOptions)
			: base("NUnit")
		{
			this.InitializeComponent();

			this.guiOptions = guiOptions;
			this.recentFilesService = Services.RecentFiles;
			this.userSettings = Services.UserSettings;

			this.presenter = new NUnitPresenter(this, this.TestLoader);
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(NUnitForm));
		private TestLoader _testLoader;
		public MenuItem aboutMenuItem;
		private MenuItem addAssemblyMenuItem;
		private MenuItem addVSProjectMenuItem;
		private MenuItem addinInfoMenuItem;
		private MenuItem assemblyDetailsMenuItem;
		private MenuItem closeMenuItem;
		private IContainer components;
		private MenuItem configMenuItem;
		private MenuItem decreaseFixedFontMenuItem;
		private MenuItem decreaseFontMenuItem;
		private MenuItem defaultFontMenuItem;

		// Handlers for our recentFiles and recentProjects

		private string displayFormat = "Full";
		private MenuItem editProjectMenuItem;
		private MenuItem exceptionDetailsMenuItem;
		public MenuItem exitMenuItem;
		public MenuItem fileMenu;
		private MenuItem fileMenuSeparator1;
		private MenuItem fileMenuSeparator2;
		public MenuItem fileMenuSeparator4;

		private Font fixedFont;
		private MenuItem fixedFontMenuItem;
		private MenuItem fontChangeMenuItem;
		private MenuItem fontMenuSeparator;
		private MenuItem fullGuiMenuItem;
		public GroupBox groupBox1;
		private MenuItem guiFontMenuItem;

		// Our current run command line options
		private GuiOptions guiOptions;
		public MenuItem helpItem;
		public MenuItem helpMenuItem;
		public MenuItem helpMenuSeparator1;
		private MenuItem increaseFixedFontMenuItem;
		private MenuItem increaseFontMenuItem;

		// Our 'presenter' - under development

		private Panel leftPanel;
		private LongRunningOperationDisplay longOpDisplay;

		public MainMenu mainMenu;
		private MenuItem menuItem1;
		private MenuItem menuItem2;
		private MenuItem miniGuiMenuItem;

		private MenuItem newMenuItem;
		private MenuItem openLogDirectoryMenuItem;
		private MenuItem openMenuItem;
		private NUnitPresenter presenter;
		public TestProgressBar progressBar;

		private MenuItem projectMenu;
		private MenuItem projectMenuSeparator1;
		private MenuItem projectMenuSeparator2;
		private RecentFiles recentFilesService;
		private MenuItem recentProjectsMenu;
		private RecentFileMenuHandler recentProjectsMenuHandler;
		private MenuItem reloadProjectMenuItem;
		private MenuItem reloadTestsMenuItem;
		private MenuItem restoreFixedFontMenuItem;
		public ResultTabs resultTabs;
		public Panel rightPanel;

		private MenuItem runAllMenuItem;
		public Button runButton;
		private ExpandingLabel runCount;
		private MenuItem runFailedMenuItem;
		private MenuItem runSelectedMenuItem;
		private MenuItem runtimeMenuItem;
		private MenuItem saveAsMenuItem;
		private MenuItem saveMenuItem;
		private MenuItem saveXmlResultsMenuItem;
		private MenuItem settingsMenuItem;
		public StatusBar statusBar;
		private MenuItem statusBarMenuItem;
		private Button stopButton;
		private MenuItem stopRunMenuItem;
		private ExpandingLabel suiteName;
		private MenuItem testMenu;
		private MenuItem testMenuSeparator;
		private TestTree testTree;
		public ToolTip toolTip;
		private MenuItem toolsMenu;
		private MenuItem toolsMenuSeparator1;
		private MenuItem toolsMenuSeparator2;
		public Splitter treeSplitter;
		private ISettings userSettings;
		private MenuItem viewMenu;
		private MenuItem viewMenuSeparator1;
		private MenuItem viewMenuSeparator2;
		private MenuItem viewMenuSeparator3;

		#endregion

		#region Properties/Indexers/Events

		private bool IsProjectLoaded
		{
			get
			{
				return this.TestLoader.IsProjectLoaded;
			}
		}

		private bool IsTestLoaded
		{
			get
			{
				return this.TestLoader.IsTestLoaded;
			}
		}

		private bool IsTestRunning
		{
			get
			{
				return this.TestLoader.Running;
			}
		}

		public NUnitPresenter Presenter
		{
			get
			{
				return this.presenter;
			}
		}

		private TestLoader TestLoader
		{
			get
			{
				if (this._testLoader == null)
					this._testLoader = Services.TestLoader;
				return this._testLoader;
			}
		}

		private NUnitProject TestProject
		{
			get
			{
				return this.TestLoader.TestProject;
			}
		}

		#endregion

		#region Methods/Operators

		private void CancelRun()
		{
			this.EnableStopCommand(false);

			if (this.IsTestRunning)
			{
				DialogResult dialogResult = this.MessageDisplay.Ask(
					"Do you want to cancel the running test?");

				if (dialogResult == DialogResult.No)
					this.EnableStopCommand(true);
				else
					this.TestLoader.CancelTestRun();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
					this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void EnableRunCommand(bool enable)
		{
			this.runButton.Enabled = enable;
			this.runAllMenuItem.Enabled = enable;
			this.runSelectedMenuItem.Enabled = enable;
			this.runFailedMenuItem.Enabled = enable && this.TestLoader.TestResult != null &&
											(this.TestLoader.TestResult.ResultState == ResultState.Failure ||
											this.TestLoader.TestResult.ResultState == ResultState.Error);
		}

		private void EnableStopCommand(bool enable)
		{
			this.stopButton.Enabled = enable;
			this.stopRunMenuItem.Enabled = enable;
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager resources = new ComponentResourceManager(typeof(NUnitForm));
			this.statusBar = new StatusBar();
			this.mainMenu = new MainMenu(this.components);
			this.fileMenu = new MenuItem();
			this.newMenuItem = new MenuItem();
			this.openMenuItem = new MenuItem();
			this.closeMenuItem = new MenuItem();
			this.fileMenuSeparator1 = new MenuItem();
			this.saveMenuItem = new MenuItem();
			this.saveAsMenuItem = new MenuItem();
			this.fileMenuSeparator2 = new MenuItem();
			this.reloadProjectMenuItem = new MenuItem();
			this.reloadTestsMenuItem = new MenuItem();
			this.runtimeMenuItem = new MenuItem();
			this.menuItem2 = new MenuItem();
			this.recentProjectsMenu = new MenuItem();
			this.fileMenuSeparator4 = new MenuItem();
			this.exitMenuItem = new MenuItem();
			this.viewMenu = new MenuItem();
			this.fullGuiMenuItem = new MenuItem();
			this.miniGuiMenuItem = new MenuItem();
			this.viewMenuSeparator1 = new MenuItem();
			this.viewMenuSeparator2 = new MenuItem();
			this.guiFontMenuItem = new MenuItem();
			this.increaseFontMenuItem = new MenuItem();
			this.decreaseFontMenuItem = new MenuItem();
			this.fontMenuSeparator = new MenuItem();
			this.fontChangeMenuItem = new MenuItem();
			this.defaultFontMenuItem = new MenuItem();
			this.fixedFontMenuItem = new MenuItem();
			this.increaseFixedFontMenuItem = new MenuItem();
			this.decreaseFixedFontMenuItem = new MenuItem();
			this.menuItem1 = new MenuItem();
			this.restoreFixedFontMenuItem = new MenuItem();
			this.viewMenuSeparator3 = new MenuItem();
			this.statusBarMenuItem = new MenuItem();
			this.projectMenu = new MenuItem();
			this.configMenuItem = new MenuItem();
			this.projectMenuSeparator1 = new MenuItem();
			this.addAssemblyMenuItem = new MenuItem();
			this.addVSProjectMenuItem = new MenuItem();
			this.projectMenuSeparator2 = new MenuItem();
			this.editProjectMenuItem = new MenuItem();
			this.testMenu = new MenuItem();
			this.runAllMenuItem = new MenuItem();
			this.runSelectedMenuItem = new MenuItem();
			this.runFailedMenuItem = new MenuItem();
			this.testMenuSeparator = new MenuItem();
			this.stopRunMenuItem = new MenuItem();
			this.toolsMenu = new MenuItem();
			this.assemblyDetailsMenuItem = new MenuItem();
			this.saveXmlResultsMenuItem = new MenuItem();
			this.exceptionDetailsMenuItem = new MenuItem();
			this.openLogDirectoryMenuItem = new MenuItem();
			this.toolsMenuSeparator1 = new MenuItem();
			this.settingsMenuItem = new MenuItem();
			this.toolsMenuSeparator2 = new MenuItem();
			this.addinInfoMenuItem = new MenuItem();
			this.helpItem = new MenuItem();
			this.helpMenuItem = new MenuItem();
			this.helpMenuSeparator1 = new MenuItem();
			this.aboutMenuItem = new MenuItem();
			this.treeSplitter = new Splitter();
			this.rightPanel = new Panel();
			this.groupBox1 = new GroupBox();
			this.suiteName = new ExpandingLabel();
			this.runCount = new ExpandingLabel();
			this.stopButton = new Button();
			this.runButton = new Button();
			this.progressBar = new TestProgressBar();
			this.resultTabs = new ResultTabs();
			this.toolTip = new ToolTip(this.components);
			this.testTree = new TestTree();
			this.leftPanel = new Panel();
			this.rightPanel.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.leftPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusBar
			// 
			this.statusBar.DisplayTestProgress = true;
			this.statusBar.Location = new Point(0, 407);
			this.statusBar.Name = "statusBar";
			this.statusBar.ShowPanels = true;
			this.statusBar.Size = new Size(744, 24);
			this.statusBar.TabIndex = 0;
			this.statusBar.Text = "Status";
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new MenuItem[]
											{
												this.fileMenu,
												this.viewMenu,
												this.projectMenu,
												this.testMenu,
												this.toolsMenu,
												this.helpItem
											});
			// 
			// fileMenu
			// 
			this.fileMenu.Index = 0;
			this.fileMenu.MenuItems.AddRange(new MenuItem[]
											{
												this.newMenuItem,
												this.openMenuItem,
												this.closeMenuItem,
												this.fileMenuSeparator1,
												this.saveMenuItem,
												this.saveAsMenuItem,
												this.fileMenuSeparator2,
												this.reloadProjectMenuItem,
												this.reloadTestsMenuItem,
												this.runtimeMenuItem,
												this.menuItem2,
												this.recentProjectsMenu,
												this.fileMenuSeparator4,
												this.exitMenuItem
											});
			this.fileMenu.Text = "&File";
			this.fileMenu.Popup += new EventHandler(this.fileMenu_Popup);
			// 
			// newMenuItem
			// 
			this.newMenuItem.Index = 0;
			this.newMenuItem.Shortcut = Shortcut.CtrlN;
			this.newMenuItem.Text = "&New Project...";
			this.newMenuItem.Click += new EventHandler(this.newMenuItem_Click);
			// 
			// openMenuItem
			// 
			this.openMenuItem.Index = 1;
			this.openMenuItem.Shortcut = Shortcut.CtrlO;
			this.openMenuItem.Text = "&Open Project...";
			this.openMenuItem.Click += new EventHandler(this.openMenuItem_Click);
			// 
			// closeMenuItem
			// 
			this.closeMenuItem.Index = 2;
			this.closeMenuItem.Text = "&Close";
			this.closeMenuItem.Click += new EventHandler(this.closeMenuItem_Click);
			// 
			// fileMenuSeparator1
			// 
			this.fileMenuSeparator1.Index = 3;
			this.fileMenuSeparator1.Text = "-";
			// 
			// saveMenuItem
			// 
			this.saveMenuItem.Index = 4;
			this.saveMenuItem.Shortcut = Shortcut.CtrlS;
			this.saveMenuItem.Text = "&Save";
			this.saveMenuItem.Click += new EventHandler(this.saveMenuItem_Click);
			// 
			// saveAsMenuItem
			// 
			this.saveAsMenuItem.Index = 5;
			this.saveAsMenuItem.Text = "Save &As...";
			this.saveAsMenuItem.Click += new EventHandler(this.saveAsMenuItem_Click);
			// 
			// fileMenuSeparator2
			// 
			this.fileMenuSeparator2.Index = 6;
			this.fileMenuSeparator2.Text = "-";
			// 
			// reloadProjectMenuItem
			// 
			this.reloadProjectMenuItem.Index = 7;
			this.reloadProjectMenuItem.Shortcut = Shortcut.CtrlL;
			this.reloadProjectMenuItem.Text = "Re&load Project";
			this.reloadProjectMenuItem.Click += new EventHandler(this.reloadProjectMenuItem_Click);
			// 
			// reloadTestsMenuItem
			// 
			this.reloadTestsMenuItem.Index = 8;
			this.reloadTestsMenuItem.Shortcut = Shortcut.CtrlR;
			this.reloadTestsMenuItem.Text = "&Reload Tests";
			this.reloadTestsMenuItem.Click += new EventHandler(this.reloadTestsMenuItem_Click);
			// 
			// runtimeMenuItem
			// 
			this.runtimeMenuItem.Index = 9;
			this.runtimeMenuItem.Text = "  Select R&untime";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 10;
			this.menuItem2.Text = "-";
			// 
			// recentProjectsMenu
			// 
			this.recentProjectsMenu.Index = 11;
			this.recentProjectsMenu.Text = "Recent &Projects";
			// 
			// fileMenuSeparator4
			// 
			this.fileMenuSeparator4.Index = 12;
			this.fileMenuSeparator4.Text = "-";
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Index = 13;
			this.exitMenuItem.Text = "E&xit";
			this.exitMenuItem.Click += new EventHandler(this.exitMenuItem_Click);
			// 
			// viewMenu
			// 
			this.viewMenu.Index = 1;
			this.viewMenu.MenuItems.AddRange(new MenuItem[]
											{
												this.fullGuiMenuItem,
												this.miniGuiMenuItem,
												this.viewMenuSeparator1,
												this.viewMenuSeparator2,
												this.guiFontMenuItem,
												this.fixedFontMenuItem,
												this.viewMenuSeparator3,
												this.statusBarMenuItem
											});
			this.viewMenu.Text = "&View";
			this.viewMenu.Popup += new EventHandler(this.viewMenu_Popup);
			// 
			// fullGuiMenuItem
			// 
			this.fullGuiMenuItem.Checked = true;
			this.fullGuiMenuItem.Index = 0;
			this.fullGuiMenuItem.RadioCheck = true;
			this.fullGuiMenuItem.Text = "&Full GUI";
			this.fullGuiMenuItem.Click += new EventHandler(this.fullGuiMenuItem_Click);
			// 
			// miniGuiMenuItem
			// 
			this.miniGuiMenuItem.Index = 1;
			this.miniGuiMenuItem.RadioCheck = true;
			this.miniGuiMenuItem.Text = "&Mini GUI";
			this.miniGuiMenuItem.Click += new EventHandler(this.miniGuiMenuItem_Click);
			// 
			// viewMenuSeparator1
			// 
			this.viewMenuSeparator1.Index = 2;
			this.viewMenuSeparator1.Text = "-";
			// 
			// viewMenuSeparator2
			// 
			this.viewMenuSeparator2.Index = 3;
			this.viewMenuSeparator2.Text = "-";
			// 
			// guiFontMenuItem
			// 
			this.guiFontMenuItem.Index = 4;
			this.guiFontMenuItem.MenuItems.AddRange(new MenuItem[]
													{
														this.increaseFontMenuItem,
														this.decreaseFontMenuItem,
														this.fontMenuSeparator,
														this.fontChangeMenuItem,
														this.defaultFontMenuItem
													});
			this.guiFontMenuItem.Text = "GUI Fo&nt";
			// 
			// increaseFontMenuItem
			// 
			this.increaseFontMenuItem.Index = 0;
			this.increaseFontMenuItem.Text = "&Increase";
			this.increaseFontMenuItem.Click += new EventHandler(this.increaseFontMenuItem_Click);
			// 
			// decreaseFontMenuItem
			// 
			this.decreaseFontMenuItem.Index = 1;
			this.decreaseFontMenuItem.Text = "&Decrease";
			this.decreaseFontMenuItem.Click += new EventHandler(this.decreaseFontMenuItem_Click);
			// 
			// fontMenuSeparator
			// 
			this.fontMenuSeparator.Index = 2;
			this.fontMenuSeparator.Text = "-";
			// 
			// fontChangeMenuItem
			// 
			this.fontChangeMenuItem.Index = 3;
			this.fontChangeMenuItem.Text = "&Change...";
			this.fontChangeMenuItem.Click += new EventHandler(this.fontChangeMenuItem_Click);
			// 
			// defaultFontMenuItem
			// 
			this.defaultFontMenuItem.Index = 4;
			this.defaultFontMenuItem.Text = "&Restore";
			this.defaultFontMenuItem.Click += new EventHandler(this.defaultFontMenuItem_Click);
			// 
			// fixedFontMenuItem
			// 
			this.fixedFontMenuItem.Index = 5;
			this.fixedFontMenuItem.MenuItems.AddRange(new MenuItem[]
													{
														this.increaseFixedFontMenuItem,
														this.decreaseFixedFontMenuItem,
														this.menuItem1,
														this.restoreFixedFontMenuItem
													});
			this.fixedFontMenuItem.Text = "Fi&xed Font";
			// 
			// increaseFixedFontMenuItem
			// 
			this.increaseFixedFontMenuItem.Index = 0;
			this.increaseFixedFontMenuItem.Text = "&Increase";
			this.increaseFixedFontMenuItem.Click += new EventHandler(this.increaseFixedFontMenuItem_Click);
			// 
			// decreaseFixedFontMenuItem
			// 
			this.decreaseFixedFontMenuItem.Index = 1;
			this.decreaseFixedFontMenuItem.Text = "&Decrease";
			this.decreaseFixedFontMenuItem.Click += new EventHandler(this.decreaseFixedFontMenuItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// restoreFixedFontMenuItem
			// 
			this.restoreFixedFontMenuItem.Index = 3;
			this.restoreFixedFontMenuItem.Text = "&Restore";
			this.restoreFixedFontMenuItem.Click += new EventHandler(this.restoreFixedFontMenuItem_Click);
			// 
			// viewMenuSeparator3
			// 
			this.viewMenuSeparator3.Index = 6;
			this.viewMenuSeparator3.Text = "-";
			// 
			// statusBarMenuItem
			// 
			this.statusBarMenuItem.Checked = true;
			this.statusBarMenuItem.Index = 7;
			this.statusBarMenuItem.Text = "&Status Bar";
			this.statusBarMenuItem.Click += new EventHandler(this.statusBarMenuItem_Click);
			// 
			// projectMenu
			// 
			this.projectMenu.Index = 2;
			this.projectMenu.MenuItems.AddRange(new MenuItem[]
												{
													this.configMenuItem,
													this.projectMenuSeparator1,
													this.addAssemblyMenuItem,
													this.addVSProjectMenuItem,
													this.projectMenuSeparator2,
													this.editProjectMenuItem
												});
			this.projectMenu.Text = "&Project";
			this.projectMenu.Visible = false;
			this.projectMenu.Popup += new EventHandler(this.projectMenu_Popup);
			// 
			// configMenuItem
			// 
			this.configMenuItem.Index = 0;
			this.configMenuItem.Text = "&Configurations";
			// 
			// projectMenuSeparator1
			// 
			this.projectMenuSeparator1.Index = 1;
			this.projectMenuSeparator1.Text = "-";
			// 
			// addAssemblyMenuItem
			// 
			this.addAssemblyMenuItem.Index = 2;
			this.addAssemblyMenuItem.Text = "Add Assembly...";
			this.addAssemblyMenuItem.Click += new EventHandler(this.addAssemblyMenuItem_Click);
			// 
			// addVSProjectMenuItem
			// 
			this.addVSProjectMenuItem.Index = 3;
			this.addVSProjectMenuItem.Text = "Add VS Project...";
			this.addVSProjectMenuItem.Click += new EventHandler(this.addVSProjectMenuItem_Click);
			// 
			// projectMenuSeparator2
			// 
			this.projectMenuSeparator2.Index = 4;
			this.projectMenuSeparator2.Text = "-";
			// 
			// editProjectMenuItem
			// 
			this.editProjectMenuItem.Index = 5;
			this.editProjectMenuItem.Text = "Edit...";
			this.editProjectMenuItem.Click += new EventHandler(this.editProjectMenuItem_Click);
			// 
			// testMenu
			// 
			this.testMenu.Index = 3;
			this.testMenu.MenuItems.AddRange(new MenuItem[]
											{
												this.runAllMenuItem,
												this.runSelectedMenuItem,
												this.runFailedMenuItem,
												this.testMenuSeparator,
												this.stopRunMenuItem
											});
			this.testMenu.Text = "&Tests";
			// 
			// runAllMenuItem
			// 
			this.runAllMenuItem.Index = 0;
			this.runAllMenuItem.Shortcut = Shortcut.F5;
			this.runAllMenuItem.Text = "&Run All";
			this.runAllMenuItem.Click += new EventHandler(this.runAllMenuItem_Click);
			// 
			// runSelectedMenuItem
			// 
			this.runSelectedMenuItem.Index = 1;
			this.runSelectedMenuItem.Shortcut = Shortcut.F6;
			this.runSelectedMenuItem.Text = "Run &Selected";
			this.runSelectedMenuItem.Click += new EventHandler(this.runSelectedMenuItem_Click);
			// 
			// runFailedMenuItem
			// 
			this.runFailedMenuItem.Enabled = false;
			this.runFailedMenuItem.Index = 2;
			this.runFailedMenuItem.Shortcut = Shortcut.F7;
			this.runFailedMenuItem.Text = "Run &Failed";
			this.runFailedMenuItem.Click += new EventHandler(this.runFailedMenuItem_Click);
			// 
			// testMenuSeparator
			// 
			this.testMenuSeparator.Index = 3;
			this.testMenuSeparator.Text = "-";
			// 
			// stopRunMenuItem
			// 
			this.stopRunMenuItem.Index = 4;
			this.stopRunMenuItem.Text = "S&top Run";
			this.stopRunMenuItem.Click += new EventHandler(this.stopRunMenuItem_Click);
			// 
			// toolsMenu
			// 
			this.toolsMenu.Index = 4;
			this.toolsMenu.MenuItems.AddRange(new MenuItem[]
											{
												this.assemblyDetailsMenuItem,
												this.saveXmlResultsMenuItem,
												this.exceptionDetailsMenuItem,
												this.openLogDirectoryMenuItem,
												this.toolsMenuSeparator1,
												this.settingsMenuItem,
												this.toolsMenuSeparator2,
												this.addinInfoMenuItem
											});
			this.toolsMenu.Text = "T&ools";
			this.toolsMenu.Popup += new EventHandler(this.toolsMenu_Popup);
			// 
			// assemblyDetailsMenuItem
			// 
			this.assemblyDetailsMenuItem.Index = 0;
			this.assemblyDetailsMenuItem.Text = "&Test Assemblies...";
			this.assemblyDetailsMenuItem.Click += new EventHandler(this.assemblyDetailsMenuItem_Click);
			// 
			// saveXmlResultsMenuItem
			// 
			this.saveXmlResultsMenuItem.Index = 1;
			this.saveXmlResultsMenuItem.Text = "&Save Results as XML...";
			this.saveXmlResultsMenuItem.Click += new EventHandler(this.saveXmlResultsMenuItem_Click);
			// 
			// exceptionDetailsMenuItem
			// 
			this.exceptionDetailsMenuItem.Index = 2;
			this.exceptionDetailsMenuItem.Text = "&Exception Details...";
			this.exceptionDetailsMenuItem.Click += new EventHandler(this.exceptionDetailsMenuItem_Click);
			// 
			// openLogDirectoryMenuItem
			// 
			this.openLogDirectoryMenuItem.Index = 3;
			this.openLogDirectoryMenuItem.Text = "Open &Log Directory...";
			this.openLogDirectoryMenuItem.Click += new EventHandler(this.openLogDirectoryMenuItem_Click);
			// 
			// toolsMenuSeparator1
			// 
			this.toolsMenuSeparator1.Index = 4;
			this.toolsMenuSeparator1.Text = "-";
			// 
			// settingsMenuItem
			// 
			this.settingsMenuItem.Index = 5;
			this.settingsMenuItem.Text = "&Settings...";
			this.settingsMenuItem.Click += new EventHandler(this.settingsMenuItem_Click);
			// 
			// toolsMenuSeparator2
			// 
			this.toolsMenuSeparator2.Index = 6;
			this.toolsMenuSeparator2.Text = "-";
			// 
			// addinInfoMenuItem
			// 
			this.addinInfoMenuItem.Index = 7;
			this.addinInfoMenuItem.Text = "Addins...";
			this.addinInfoMenuItem.Click += new EventHandler(this.addinInfoMenuItem_Click);
			// 
			// helpItem
			// 
			this.helpItem.Index = 5;
			this.helpItem.MenuItems.AddRange(new MenuItem[]
											{
												this.helpMenuItem,
												this.helpMenuSeparator1,
												this.aboutMenuItem
											});
			this.helpItem.Text = "&Help";
			// 
			// helpMenuItem
			// 
			this.helpMenuItem.Index = 0;
			this.helpMenuItem.Shortcut = Shortcut.F1;
			this.helpMenuItem.Text = "NUnit &Help...";
			this.helpMenuItem.Click += new EventHandler(this.helpMenuItem_Click);
			// 
			// helpMenuSeparator1
			// 
			this.helpMenuSeparator1.Index = 1;
			this.helpMenuSeparator1.Text = "-";
			// 
			// aboutMenuItem
			// 
			this.aboutMenuItem.Index = 2;
			this.aboutMenuItem.Text = "&About NUnit...";
			this.aboutMenuItem.Click += new EventHandler(this.aboutMenuItem_Click);
			// 
			// treeSplitter
			// 
			this.treeSplitter.Location = new Point(240, 0);
			this.treeSplitter.MinSize = 240;
			this.treeSplitter.Name = "treeSplitter";
			this.treeSplitter.Size = new Size(6, 407);
			this.treeSplitter.TabIndex = 2;
			this.treeSplitter.TabStop = false;
			// 
			// rightPanel
			// 
			this.rightPanel.BackColor = SystemColors.Control;
			this.rightPanel.Controls.Add(this.groupBox1);
			this.rightPanel.Controls.Add(this.resultTabs);
			this.rightPanel.Dock = DockStyle.Fill;
			this.rightPanel.Location = new Point(246, 0);
			this.rightPanel.Name = "rightPanel";
			this.rightPanel.Size = new Size(498, 407);
			this.rightPanel.TabIndex = 3;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.suiteName);
			this.groupBox1.Controls.Add(this.runCount);
			this.groupBox1.Controls.Add(this.stopButton);
			this.groupBox1.Controls.Add(this.runButton);
			this.groupBox1.Controls.Add(this.progressBar);
			this.groupBox1.Dock = DockStyle.Top;
			this.groupBox1.Location = new Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(498, 120);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// suiteName
			// 
			this.suiteName.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.suiteName.AutoEllipsis = true;
			this.suiteName.Location = new Point(145, 21);
			this.suiteName.Name = "suiteName";
			this.suiteName.Size = new Size(343, 23);
			this.suiteName.TabIndex = 1;
			// 
			// runCount
			// 
			this.runCount.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.runCount.AutoEllipsis = true;
			this.runCount.Location = new Point(8, 89);
			this.runCount.Name = "runCount";
			this.runCount.Size = new Size(480, 21);
			this.runCount.TabIndex = 5;
			// 
			// stopButton
			// 
			this.stopButton.AutoSize = true;
			this.stopButton.Location = new Point(75, 16);
			this.stopButton.Name = "stopButton";
			this.stopButton.Size = new Size(64, 31);
			this.stopButton.TabIndex = 4;
			this.stopButton.Text = "&Stop";
			this.stopButton.Click += new EventHandler(this.stopButton_Click);
			// 
			// runButton
			// 
			this.runButton.Location = new Point(8, 16);
			this.runButton.Name = "runButton";
			this.runButton.Size = new Size(64, 31);
			this.runButton.TabIndex = 3;
			this.runButton.Text = "&Run";
			this.runButton.Click += new EventHandler(this.runButton_Click);
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
														| AnchorStyles.Right)));
			this.progressBar.BackColor = SystemColors.Control;
			this.progressBar.CausesValidation = false;
			this.progressBar.Enabled = false;
			this.progressBar.ForeColor = SystemColors.Highlight;
			this.progressBar.Location = new Point(8, 54);
			this.progressBar.Maximum = 100;
			this.progressBar.Minimum = 0;
			this.progressBar.Name = "progressBar";
			this.progressBar.Segmented = true;
			this.progressBar.Size = new Size(480, 28);
			this.progressBar.Step = 1;
			this.progressBar.TabIndex = 0;
			this.progressBar.Value = 0;
			// 
			// resultTabs
			// 
			this.resultTabs.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
														| AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.resultTabs.Location = new Point(0, 120);
			this.resultTabs.Name = "resultTabs";
			this.resultTabs.Size = new Size(498, 284);
			this.resultTabs.TabIndex = 2;
			// 
			// testTree
			// 
			this.testTree.Dock = DockStyle.Fill;
			this.testTree.Location = new Point(0, 0);
			this.testTree.Name = "testTree";
			this.testTree.ShowCheckBoxes = false;
			this.testTree.Size = new Size(240, 407);
			this.testTree.TabIndex = 0;
			this.testTree.SelectedTestsChanged += new SelectedTestsChangedEventHandler(this.testTree_SelectedTestsChanged);
			// 
			// leftPanel
			// 
			this.leftPanel.Controls.Add(this.testTree);
			this.leftPanel.Dock = DockStyle.Left;
			this.leftPanel.Location = new Point(0, 0);
			this.leftPanel.Name = "leftPanel";
			this.leftPanel.Size = new Size(240, 407);
			this.leftPanel.TabIndex = 4;
			// 
			// NUnitForm
			// 
			this.ClientSize = new Size(744, 431);
			this.Controls.Add(this.rightPanel);
			this.Controls.Add(this.treeSplitter);
			this.Controls.Add(this.leftPanel);
			this.Controls.Add(this.statusBar);
			this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu;
			this.MinimumSize = new Size(160, 32);
			this.Name = "NUnitForm";
			this.StartPosition = FormStartPosition.Manual;
			this.Text = "NUnit";
			this.Load += new EventHandler(this.NUnitForm_Load);
			this.Closing += new CancelEventHandler(this.NUnitForm_Closing);
			this.rightPanel.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.leftPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		private void InitializeControls()
		{
			// ToDo: Migrate more ui elements to handle events on their own.
			this.progressBar.Subscribe(this.TestLoader.Events);
			this.statusBar.Subscribe(this.TestLoader.Events);
		}

		private bool IsValidLocation(Point location)
		{
			Rectangle myArea = new Rectangle(location, this.Size);
			bool intersect = false;
			foreach (Screen screen in Screen.AllScreens)
				intersect |= myArea.IntersectsWith(screen.WorkingArea);
			return intersect;
		}

		private void LoadFormSettings()
		{
			this.displayFormat = this.userSettings.GetSetting("Gui.DisplayFormat", "Full");

			switch (this.displayFormat)
			{
				case "Full":
					this.displayFullGui();
					break;
				case "Mini":
					this.displayMiniGui();
					break;
				default:
					throw new ApplicationException("Invalid Setting");
			}

			// Handle changes to form position
			this.Move += new EventHandler(this.NUnitForm_Move);
			this.Resize += new EventHandler(this.NUnitForm_Resize);

			// Set the splitter position
			int splitPosition = this.userSettings.GetSetting("Gui.MainForm.SplitPosition", this.treeSplitter.SplitPosition);
			if (splitPosition >= this.treeSplitter.MinSize && splitPosition < this.ClientSize.Width)
				this.treeSplitter.SplitPosition = splitPosition;

			// Handle changes in splitter positions
			this.treeSplitter.SplitterMoved += new SplitterEventHandler(this.treeSplitter_SplitterMoved);

			// Get the fixed font used by result tabs
			this.fixedFont = this.userSettings.GetSetting("Gui.FixedFont", new Font(FontFamily.GenericMonospace, 8.0f));

			// Handle changes in form settings
			this.userSettings.Changed += new SettingsEventHandler(this.userSettings_Changed);
		}

		private void LoadOrReloadTestAsNeeded()
		{
			if (this.TestProject.HasChangesRequiringReload)
			{
				if (this.TestProject.IsLoadable)
				{
					if (this.IsTestLoaded)
						this.TestLoader.ReloadTest();
					else
						this.TestLoader.LoadTest();
				}
				else
					this.TestLoader.UnloadTest();
			}
		}

		/// <summary>
		/// Form is about to close, first see if we
		/// have a test run going on and if so whether
		/// we should cancel it. Then unload the
		/// test and save the latest form position.
		/// </summary>
		private void NUnitForm_Closing(object sender, CancelEventArgs e)
		{
			if (this.IsTestRunning)
			{
				DialogResult dialogResult = this.MessageDisplay.Ask(
					"A test is running, do you want to stop the test and exit?");

				if (dialogResult == DialogResult.No)
					e.Cancel = true;
				else
					this.TestLoader.CancelTestRun();
			}

			if (!e.Cancel && this.IsProjectLoaded &&
				this.presenter.CloseProject() == DialogResult.Cancel)
				e.Cancel = true;
		}

		/// <summary>
		/// Get saved options when form loads
		/// </summary>
		private void NUnitForm_Load(object sender, EventArgs e)
		{
			if (!this.DesignMode)
			{
				// TODO: Can these controls add their menus themselves?
				this.viewMenu.MenuItems.Add(3, this.resultTabs.TabsMenu);
				this.viewMenu.MenuItems.Add(4, this.testTree.TreeMenu);

				this.EnableRunCommand(false);
				this.EnableStopCommand(false);

				this.recentProjectsMenuHandler = new RecentFileMenuHandler(this.recentProjectsMenu, this.recentFilesService);
				this.recentProjectsMenuHandler.CheckFilesExist = this.userSettings.GetSetting("Gui.RecentProjects.CheckFilesExist", true);

				this.LoadFormSettings();
				this.SubscribeToTestEvents();
				this.InitializeControls();

				// Force display  so that any "Loading..." or error 
				// message overlays the main form.
				this.Show();
				this.Invalidate();
				this.Update();

				// Set Capture options for the TestLoader
				this.TestLoader.IsTracingEnabled = this.resultTabs.IsTracingEnabled;
				this.TestLoader.LoggingThreshold = this.resultTabs.MaximumLogLevel;

				// Load test specified on command line or
				// the most recent one if options call for it
				if (this.guiOptions.ParameterCount != 0)
					this.presenter.OpenProject((string)this.guiOptions.Parameters[0], this.guiOptions.config, this.guiOptions.fixture);
				else if (this.userSettings.GetSetting("Options.LoadLastProject", true) && !this.guiOptions.noload)
				{
					foreach (RecentFileEntry entry in this.recentFilesService.Entries)
					{
						if (entry != null && entry.Exists && entry.IsCompatibleCLRVersion)
						{
							this.presenter.OpenProject(entry.Path, this.guiOptions.config, this.guiOptions.fixture);
							break;
						}
					}
				}

				if (this.guiOptions.include != null)
				{
					string[] categories = this.guiOptions.include.Split(',');
					if (categories.Length > 0)
						this.testTree.SelectCategories(categories, false);
				}
				else if (this.guiOptions.exclude != null)
				{
					string[] categories = this.guiOptions.exclude.Split(',');
					if (categories.Length > 0)
						this.testTree.SelectCategories(categories, true);
				}

				// Run loaded test automatically if called for
				if (this.TestLoader.IsTestLoaded)
				{
					if (this.guiOptions.run || this.guiOptions.runselected)
					{
						// TODO: Temporary fix to avoid problem when /run is used 
						// with ReloadOnRun turned on. Refactor TestLoader so
						// we can just do a run without reload.
						bool reload = Services.UserSettings.GetSetting("Options.TestLoader.ReloadOnRun", false);

						try
						{
							Services.UserSettings.SaveSetting("Options.TestLoader.ReloadOnRun", false);
							if (this.guiOptions.runselected)
								this.testTree.RunSelectedTests();
							else
								this.testTree.RunAllTests(false);
						}
						finally
						{
							Services.UserSettings.SaveSetting("Options.TestLoader.ReloadOnRun", reload);
						}
					}
				}
			}
		}

		private void NUnitForm_Move(object sender, EventArgs e)
		{
			switch (this.displayFormat)
			{
				case "Full":
				default:
					if (this.WindowState == FormWindowState.Normal)
					{
						this.userSettings.SaveSetting("Gui.MainForm.Left", this.Location.X);
						this.userSettings.SaveSetting("Gui.MainForm.Top", this.Location.Y);
						this.userSettings.SaveSetting("Gui.MainForm.Maximized", false);

						this.statusBar.SizingGrip = true;
					}
					else if (this.WindowState == FormWindowState.Maximized)
					{
						this.userSettings.SaveSetting("Gui.MainForm.Maximized", true);

						this.statusBar.SizingGrip = false;
					}
					break;
				case "Mini":
					if (this.WindowState == FormWindowState.Normal)
					{
						this.userSettings.SaveSetting("Gui.MiniForm.Left", this.Location.X);
						this.userSettings.SaveSetting("Gui.MiniForm.Top", this.Location.Y);
						this.userSettings.SaveSetting("Gui.MiniForm.Maximized", false);

						this.statusBar.SizingGrip = true;
					}
					else if (this.WindowState == FormWindowState.Maximized)
					{
						this.userSettings.SaveSetting("Gui.MiniForm.Maximized", true);

						this.statusBar.SizingGrip = false;
					}
					break;
			}
		}

		// Save settings that change when window is resized
		private void NUnitForm_Resize(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Normal)
			{
				if (this.displayFormat == "Full")
				{
					this.userSettings.SaveSetting("Gui.MainForm.Width", this.Size.Width);
					this.userSettings.SaveSetting("Gui.MainForm.Height", this.Size.Height);
				}
				else
				{
					this.userSettings.SaveSetting("Gui.MiniForm.Width", this.Size.Width);
					this.userSettings.SaveSetting("Gui.MiniForm.Height", this.Size.Height);
				}
			}
		}

		private void OnProjectLoadFailure(object sender, TestEventArgs e)
		{
			this.MessageDisplay.Error("Project Not Loaded", e.Exception);

			this.recentFilesService.Remove(e.Name);

			this.EnableRunCommand(this.IsProjectLoaded);
		}

		private void OnReloadStarting(object sender, TestEventArgs e)
		{
			this.EnableRunCommand(false);
			this.longOpDisplay = new LongRunningOperationDisplay(this, "Reloading...");
		}

		private void OnRunFinished(object sender, TestEventArgs e)
		{
			this.EnableStopCommand(false);
			this.EnableRunCommand(false);

			if (e.Exception != null)
			{
				if (! (e.Exception is ThreadAbortException))
					this.MessageDisplay.Error("NUnit Test Run Failed", e.Exception);
			}

			ResultSummarizer summary = new ResultSummarizer(e.Result);
			this.runCount.Text = string.Format(
				"Passed: {0}   Failed: {1}   Errors: {2}   Inconclusive: {3}   Invalid: {4}   Ignored: {5}   Skipped: {6}   Time: {7}",
				summary.Passed, summary.Failures, summary.Errors, summary.Inconclusive, summary.NotRunnable, summary.Ignored, summary.Skipped, summary.Time);

			string resultPath = Path.Combine(this.TestProject.BasePath, "TestResult.xml");
			try
			{
				this.TestLoader.SaveLastResult(resultPath);
				log.Debug("Saved result to {0}", resultPath);
			}
			catch (Exception ex)
			{
				log.Warning("Unable to save result to {0}\n{1}", resultPath, ex.ToString());
			}

			this.EnableRunCommand(true);

			if (e.Result.ResultState == ResultState.Failure ||
				e.Result.ResultState == ResultState.Error ||
				e.Result.ResultState == ResultState.Cancelled)
				this.Activate();
		}

		private void OnRunStarting(object sender, TestEventArgs e)
		{
			this.suiteName.Text = e.Name;
			this.EnableRunCommand(false);
			this.EnableStopCommand(true);
			this.runCount.Text = "";
		}

		/// <summary>
		/// The current test suite has changed in some way,
		/// so update the info in the UI and clear the
		/// test results, since they are no longer valid.
		/// </summary>
		private void OnTestChanged(object sender, TestEventArgs e)
		{
			this.SetTitleBar(this.TestProject.Name);

			if (this.longOpDisplay != null)
			{
				this.longOpDisplay.Dispose();
				this.longOpDisplay = null;
			}

			if (this.userSettings.GetSetting("Options.TestLoader.ClearResultsOnReload", false))
				this.runCount.Text = null;

			this.EnableRunCommand(true);
		}

		/// <summary>
		/// Event handler for assembly load failures. We do this via
		/// an event since some errors may occur asynchronously.
		/// </summary>
		private void OnTestLoadFailure(object sender, TestEventArgs e)
		{
			if (this.longOpDisplay != null)
			{
				this.longOpDisplay.Dispose();
				this.longOpDisplay = null;
			}

			string message = e.Action == TestAction.TestReloadFailed
				? "Test reload failed!"
				: "Test load failed!";
			string NL = Environment.NewLine;
			if (e.Exception is BadImageFormatException)
			{
				message += string.Format(NL + NL +
										"The assembly could not be loaded by NUnit. PossibleProblems include:" + NL + NL +
										"1. The assembly may not be a valid .NET assembly." + NL + NL +
										"2. You may be attempting to load an assembly built with a later version of the CLR than the version under which NUnit is currently running ({0})." + NL + NL +
										"3. You may be attempting to load a 64-bit assembly into a 32-bit process.",
					Environment.Version.ToString(3));
			}

			this.MessageDisplay.Error(message, e.Exception);

			if (!this.IsTestLoaded)
				this.OnTestUnloaded(sender, e);
			else
				this.EnableRunCommand(true);
		}

		private void OnTestLoadStarting(object sender, TestEventArgs e)
		{
			this.EnableRunCommand(false);
			this.longOpDisplay = new LongRunningOperationDisplay(this, "Loading...");
		}

		/// <summary>
		/// A test suite has been loaded, so update
		/// recent assemblies and display the tests in the UI
		/// </summary>
		private void OnTestLoaded(object sender, TestEventArgs e)
		{
			if (this.longOpDisplay != null)
			{
				this.longOpDisplay.Dispose();
				this.longOpDisplay = null;
			}
			this.EnableRunCommand(true);

			if (this.TestLoader.TestCount == 0)
			{
				foreach (TestAssemblyInfo info in this.TestLoader.AssemblyInfo)
				{
					if (info.TestFrameworks.Count > 0)
						return;
				}

				this.MessageDisplay.Error("This assembly was not built with any known testing framework.");
			}
		}

		private void OnTestProjectLoaded(object sender, TestEventArgs e)
		{
			string projectPath = e.Name;

			this.SetTitleBar(projectPath);
			this.projectMenu.Visible = true;
			this.runCount.Text = "";

			// If this is an NUnit project, set up watcher
			if (NUnitProject.IsNUnitProjectFile(projectPath) && File.Exists(projectPath))
				this.presenter.WatchProject(projectPath);
		}

		private void OnTestProjectUnloaded(object sender, TestEventArgs e)
		{
			this.SetTitleBar(null);
			this.projectMenu.Visible = false;
			this.runCount.Text = "";
		}

		private void OnTestProjectUnloading(object sender, TestEventArgs e)
		{
			// Remove any watcher
			if (e.Name != null && File.Exists(e.Name))
			{
				this.presenter.RemoveWatcher();

				Version version = Environment.Version;
				foreach (TestAssemblyInfo info in this.TestLoader.AssemblyInfo)
				{
					if (info.ImageRuntimeVersion < version)
						version = info.ImageRuntimeVersion;
				}

				this.recentFilesService.SetMostRecent(new RecentFileEntry(e.Name, version));
			}
		}

		private void OnTestUnloadStarting(object sender, TestEventArgs e)
		{
			this.EnableRunCommand(false);
		}

		/// <summary>
		/// A test suite has been unloaded, so clear the UI
		/// and remove any references to the suite.
		/// </summary>
		private void OnTestUnloaded(object sender, TestEventArgs e)
		{
			this.suiteName.Text = null;
			this.runCount.Text = null;
			this.EnableRunCommand(false);
			this.Refresh();
		}

		/// <summary>
		/// Set the title bar based on the loaded file or project
		/// </summary>
		/// <param name="fileName"> </param>
		private void SetTitleBar(string fileName)
		{
			this.Text = fileName == null
				? "NUnit"
				: string.Format("{0} - NUnit", Path.GetFileName(fileName));
		}

		private void SubscribeToTestEvents()
		{
			ITestEvents events = this.TestLoader.Events;

			events.RunStarting += new TestEventHandler(this.OnRunStarting);
			events.RunFinished += new TestEventHandler(this.OnRunFinished);

			events.ProjectLoaded += new TestEventHandler(this.OnTestProjectLoaded);
			events.ProjectLoadFailed += new TestEventHandler(this.OnProjectLoadFailure);
			events.ProjectUnloading += new TestEventHandler(this.OnTestProjectUnloading);
			events.ProjectUnloaded += new TestEventHandler(this.OnTestProjectUnloaded);

			events.TestLoading += new TestEventHandler(this.OnTestLoadStarting);
			events.TestLoaded += new TestEventHandler(this.OnTestLoaded);
			events.TestLoadFailed += new TestEventHandler(this.OnTestLoadFailure);
			events.TestUnloading += new TestEventHandler(this.OnTestUnloadStarting);
			events.TestUnloaded += new TestEventHandler(this.OnTestUnloaded);
			events.TestReloading += new TestEventHandler(this.OnReloadStarting);
			events.TestReloaded += new TestEventHandler(this.OnTestChanged);
			events.TestReloadFailed += new TestEventHandler(this.OnTestLoadFailure);
		}

		/// <summary>
		/// Display the about box when menu item is selected
		/// </summary>
		private void aboutMenuItem_Click(object sender, EventArgs e)
		{
			using (AboutBox aboutBox = new AboutBox())
			{
				this.Site.Container.Add(aboutBox);
				aboutBox.ShowDialog();
			}
		}

		private void addAssemblyMenuItem_Click(object sender, EventArgs e)
		{
			this.presenter.AddAssembly();
			this.LoadOrReloadTestAsNeeded();
		}

		private void addConfigurationMenuItem_Click(object sender, EventArgs e)
		{
			using (AddConfigurationDialog dlg = new AddConfigurationDialog(this.TestProject))
			{
				this.Site.Container.Add(dlg);
				dlg.ShowDialog();
			}

			this.LoadOrReloadTestAsNeeded();
		}

		private void addVSProjectMenuItem_Click(object sender, EventArgs e)
		{
			this.presenter.AddVSProject();
			this.LoadOrReloadTestAsNeeded();
		}

		private void addinInfoMenuItem_Click(object sender, EventArgs e)
		{
			AddinDialog dlg = new AddinDialog();
			dlg.ShowDialog();
		}

		private void applyFixedFont(Font font)
		{
			this.fixedFont = font;
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
			this.userSettings.SaveSetting("Gui.FixedFont", converter.ConvertToString(null, CultureInfo.InvariantCulture, font));
		}

		private void applyFont(Font font)
		{
			this.Font = font;
			this.runCount.Font = font.FontFamily.IsStyleAvailable(FontStyle.Bold)
				? new Font(font, FontStyle.Bold)
				: font;
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
			this.userSettings.SaveSetting(
				this.displayFormat == "Mini" ? "Gui.MiniForm.Font" : "Gui.MainForm.Font",
				converter.ConvertToString(null, CultureInfo.InvariantCulture, font));
		}

		private void assemblyDetailsMenuItem_Click(object sender, EventArgs e)
		{
			new TestAssemblyInfoForm().ShowDialog();
		}

		private void closeMenuItem_Click(object sender, EventArgs e)
		{
			this.presenter.CloseProject();
		}

		private void configMenuItem_Click(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			if (!item.Checked)
			{
				this.TestProject.SetActiveConfig(this.TestProject.Configs[item.Index].Name);
				this.LoadOrReloadTestAsNeeded();
			}
		}

		private void decreaseFixedFontMenuItem_Click(object sender, EventArgs e)
		{
			this.applyFixedFont(new Font(this.fixedFont.FontFamily, this.fixedFont.SizeInPoints / 1.2f, this.fixedFont.Style));
		}

		private void decreaseFontMenuItem_Click(object sender, EventArgs e)
		{
			this.applyFont(new Font(this.Font.FontFamily, this.Font.SizeInPoints / 1.2f, this.Font.Style));
		}

		private void defaultFontMenuItem_Click(object sender, EventArgs e)
		{
			this.applyFont(DefaultFont);
		}

		private void displayFullGui()
		{
			this.fullGuiMenuItem.Checked = true;
			this.miniGuiMenuItem.Checked = false;

			this.displayFormat = "Full";
			this.userSettings.SaveSetting("Gui.DisplayFormat", "Full");

			this.leftPanel.Visible = true;
			this.leftPanel.Dock = DockStyle.Left;
			this.treeSplitter.Visible = true;
			this.rightPanel.Visible = true;
			this.statusBar.Visible = true;

			this.resultTabs.TabsMenu.Visible = true;

			int x = this.userSettings.GetSetting("Gui.MainForm.Left", 10);
			int y = this.userSettings.GetSetting("Gui.MainForm.Top", 10);
			Point location = new Point(x, y);

			if (!this.IsValidLocation(location))
				location = new Point(10, 10);
			this.Location = location;

			int width = this.userSettings.GetSetting("Gui.MainForm.Width", this.Size.Width);
			int height = this.userSettings.GetSetting("Gui.MainForm.Height", this.Size.Height);
			if (width < 160)
				width = 160;
			if (height < 32)
				height = 32;
			this.Size = new Size(width, height);

			// Set to maximized if required
			if (this.userSettings.GetSetting("Gui.MainForm.Maximized", false))
				this.WindowState = FormWindowState.Maximized;

			// Set the font to use
			this.applyFont(this.userSettings.GetSetting("Gui.MainForm.Font", DefaultFont));
		}

		private void displayMiniGui()
		{
			this.miniGuiMenuItem.Checked = true;
			this.fullGuiMenuItem.Checked = false;

			this.displayFormat = "Mini";
			this.userSettings.SaveSetting("Gui.DisplayFormat", "Mini");

			this.leftPanel.Visible = true;
			this.leftPanel.Dock = DockStyle.Fill;
			this.treeSplitter.Visible = false;
			this.rightPanel.Visible = false;
			this.statusBar.Visible = false;

			this.resultTabs.TabsMenu.Visible = false;

			int x = this.userSettings.GetSetting("Gui.MiniForm.Left", 10);
			int y = this.userSettings.GetSetting("Gui.MiniForm.Top", 10);
			Point location = new Point(x, y);

			if (!this.IsValidLocation(location))
				location = new Point(10, 10);
			this.Location = location;

			int width = this.userSettings.GetSetting("Gui.MiniForm.Width", 300);
			int height = this.userSettings.GetSetting("Gui.MiniForm.Height", this.Size.Height);
			if (width < 160)
				width = 160;
			if (height < 32)
				height = 32;
			this.Size = new Size(width, height);

			// Set the font to use
			this.applyFont(this.userSettings.GetSetting("Gui.MiniForm.Font", DefaultFont));
		}

		private void editConfigurationsMenuItem_Click(object sender, EventArgs e)
		{
			using (ConfigurationEditor editor = new ConfigurationEditor(this.TestProject))
			{
				this.Site.Container.Add(editor);
				editor.ShowDialog();
			}

			this.LoadOrReloadTestAsNeeded();
		}

		private void editProjectMenuItem_Click(object sender, EventArgs e)
		{
			this.presenter.EditProject();
		}

		private void exceptionDetailsMenuItem_Click(object sender, EventArgs e)
		{
			using (ExceptionDetailsForm details = new ExceptionDetailsForm(this.TestLoader.LastException))
			{
				this.Site.Container.Add(details);
				details.ShowDialog();
			}
		}

		private void exitMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void fileMenu_Popup(object sender, EventArgs e)
		{
			this.newMenuItem.Enabled = !this.IsTestRunning;
			this.openMenuItem.Enabled = !this.IsTestRunning;
			this.closeMenuItem.Enabled = this.IsProjectLoaded && !this.IsTestRunning;

			this.saveMenuItem.Enabled = this.IsProjectLoaded;
			this.saveAsMenuItem.Enabled = this.IsProjectLoaded;

			this.reloadTestsMenuItem.Enabled = this.IsTestLoaded && !this.IsTestRunning;
			this.reloadProjectMenuItem.Enabled = this.runtimeMenuItem.Enabled =
				this.IsProjectLoaded &&
				File.Exists(this.TestProject.ProjectPath) &&
				!this.IsTestRunning;

			RuntimeFramework current = this.TestLoader.CurrentFramework;
			RuntimeFramework[] frameworks = RuntimeFramework.AvailableFrameworks;

			this.runtimeMenuItem.Visible = frameworks.Length > 1;

			if (this.runtimeMenuItem.Visible && this.runtimeMenuItem.Enabled)
			{
				this.runtimeMenuItem.MenuItems.Clear();

				foreach (RuntimeFramework framework in frameworks)
				{
					MenuItem item = new MenuItem(framework.DisplayName);
					item.Checked = current.Supports(framework);
					item.Tag = framework;
					item.Click += new EventHandler(this.runtimeFrameworkMenuItem_Click);
					try
					{
						item.Enabled = this.TestLoader.CanReloadUnderRuntimeVersion(framework.ClrVersion);
					}
					catch
					{
						item.Enabled = false;
					}
					this.runtimeMenuItem.MenuItems.Add(item);
				}
			}

			this.recentProjectsMenu.Enabled = !this.IsTestRunning;

			if (!this.IsTestRunning)
				this.recentProjectsMenuHandler.Load();
		}

		private void fontChangeMenuItem_Click(object sender, EventArgs e)
		{
			FontDialog fontDialog = new FontDialog();
			fontDialog.FontMustExist = true;
			fontDialog.Font = this.Font;
			fontDialog.MinSize = 6;
			fontDialog.MaxSize = 12;
			fontDialog.AllowVectorFonts = false;
			fontDialog.ScriptsOnly = true;
			fontDialog.ShowEffects = false;
			fontDialog.ShowApply = true;
			fontDialog.Apply += new EventHandler(this.fontDialog_Apply);
			if (fontDialog.ShowDialog() == DialogResult.OK)
				this.applyFont(fontDialog.Font);
		}

		private void fontDialog_Apply(object sender, EventArgs e)
		{
			this.applyFont(((FontDialog)sender).Font);
		}

		private void fullGuiMenuItem_Click(object sender, EventArgs e)
		{
			if (!this.fullGuiMenuItem.Checked)
				this.displayFullGui();
		}

		private void helpMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start(NUnitConfiguration.HelpUrl);
		}

		private void increaseFixedFontMenuItem_Click(object sender, EventArgs e)
		{
			this.applyFixedFont(new Font(this.fixedFont.FontFamily, this.fixedFont.SizeInPoints * 1.2f, this.fixedFont.Style));
		}

		private void increaseFontMenuItem_Click(object sender, EventArgs e)
		{
			this.applyFont(new Font(this.Font.FontFamily, this.Font.SizeInPoints * 1.2f, this.Font.Style));
		}

		private void miniGuiMenuItem_Click(object sender, EventArgs e)
		{
			if (!this.miniGuiMenuItem.Checked)
				this.displayMiniGui();
		}

		private void newMenuItem_Click(object sender, EventArgs e)
		{
			this.presenter.NewProject();
		}

		private void openLogDirectoryMenuItem_Click(object sender, EventArgs e)
		{
			if (!Directory.Exists(NUnitConfiguration.LogDirectory))
				Directory.CreateDirectory(NUnitConfiguration.LogDirectory);

			Process.Start(NUnitConfiguration.LogDirectory);
		}

		private void openMenuItem_Click(object sender, EventArgs e)
		{
			this.presenter.OpenProject();
		}

		/// <summary>
		/// When the project menu pops up, we populate the
		/// submenu for configurations dynamically.
		/// </summary>
		private void projectMenu_Popup(object sender, EventArgs e)
		{
			int index = 0;
			this.configMenuItem.MenuItems.Clear();

			foreach (ProjectConfig config in this.TestProject.Configs)
			{
				string text = string.Format("&{0} {1}", index + 1, config.Name);
				MenuItem item = new MenuItem(
					text, new EventHandler(this.configMenuItem_Click));
				if (config.Name == this.TestProject.ActiveConfigName)
					item.Checked = true;
				this.configMenuItem.MenuItems.Add(index++, item);
			}

			this.configMenuItem.MenuItems.Add("-");

			this.configMenuItem.MenuItems.Add("&Add...",
				new EventHandler(this.addConfigurationMenuItem_Click));

			this.configMenuItem.MenuItems.Add("&Edit...",
				new EventHandler(this.editConfigurationsMenuItem_Click));

			this.addVSProjectMenuItem.Visible = this.userSettings.GetSetting("Options.TestLoader.VisualStudioSupport", false);
			this.addAssemblyMenuItem.Enabled = this.TestProject.Configs.Count > 0;
		}

		private void reloadProjectMenuItem_Click(object sender, EventArgs e)
		{
			this.presenter.ReloadProject();
		}

		private void reloadTestsMenuItem_Click(object sender, EventArgs e)
		{
			this.TestLoader.ReloadTest();
		}

		private void restoreFixedFontMenuItem_Click(object sender, EventArgs e)
		{
			this.applyFixedFont(new Font(FontFamily.GenericMonospace, 8.0f));
		}

		private void runAllMenuItem_Click(object sender, EventArgs e)
		{
			this.testTree.RunAllTests();
		}

		/// <summary>
		/// When the Run Button is clicked, run the selected test.
		/// </summary>
		private void runButton_Click(object sender, EventArgs e)
		{
			this.testTree.RunSelectedTests();
		}

		private void runFailedMenuItem_Click(object sender, EventArgs e)
		{
			this.testTree.RunFailedTests();
		}

		private void runSelectedMenuItem_Click(object sender, EventArgs e)
		{
			this.testTree.RunSelectedTests();
		}

		private void runtimeFrameworkMenuItem_Click(object sender, EventArgs e)
		{
			this.TestLoader.ReloadTest(((MenuItem)sender).Tag as RuntimeFramework);
		}

		private void saveAsMenuItem_Click(object sender, EventArgs e)
		{
			this.presenter.SaveProjectAs();
			this.SetTitleBar(this.TestProject.Name);
		}

		private void saveMenuItem_Click(object sender, EventArgs e)
		{
			this.presenter.SaveProject();
		}

		private void saveXmlResultsMenuItem_Click(object sender, EventArgs e)
		{
			this.presenter.SaveLastResult();
		}

		private void settingsMenuItem_Click(object sender, EventArgs e)
		{
			OptionsDialog.Display(this);
		}

		private void statusBarMenuItem_Click(object sender, EventArgs e)
		{
			this.statusBarMenuItem.Checked = !this.statusBarMenuItem.Checked;
			this.statusBar.Visible = this.statusBarMenuItem.Checked;
		}

		/// <summary>
		/// When the Stop Button is clicked, cancel running test
		/// </summary>
		private void stopButton_Click(object sender, EventArgs e)
		{
			this.CancelRun();
		}

		private void stopRunMenuItem_Click(object sender, EventArgs e)
		{
			this.CancelRun();
		}

		private void testTree_SelectedTestsChanged(object sender, SelectedTestsChangedEventArgs e)
		{
			if (!this.IsTestRunning)
			{
				this.suiteName.Text = e.TestName;
				this.statusBar.Initialize(e.TestCount, e.TestName);
			}
		}

		private void toolsMenu_Popup(object sender, EventArgs e)
		{
			this.assemblyDetailsMenuItem.Enabled = this.IsTestLoaded;
			this.saveXmlResultsMenuItem.Enabled = this.IsTestLoaded && this.TestLoader.TestResult != null;
			this.exceptionDetailsMenuItem.Enabled = this.TestLoader.LastException != null;
		}

		private void treeSplitter_SplitterMoved(object sender, SplitterEventArgs e)
		{
			this.userSettings.SaveSetting("Gui.MainForm.SplitPosition", this.treeSplitter.SplitPosition);
		}

		private void userSettings_Changed(object sender, SettingsEventArgs args)
		{
			if (args.SettingName == "Gui.DisplayFormat")
			{
				string newFormat = this.userSettings.GetSetting("Gui.DisplayFormat", this.displayFormat);
				if (newFormat != this.displayFormat)
				{
					if (newFormat == "Full")
						this.displayFullGui();
					else
						this.displayMiniGui();
				}
			}
			else if (args.SettingName.StartsWith("Gui.TextOutput.") && args.SettingName.EndsWith(".Content"))
			{
				this.TestLoader.IsTracingEnabled = this.resultTabs.IsTracingEnabled;
				this.TestLoader.LoggingThreshold = this.resultTabs.MaximumLogLevel;
			}
		}

		private void viewMenu_Popup(object sender, EventArgs e)
		{
			this.assemblyDetailsMenuItem.Enabled = this.TestLoader.IsTestLoaded;
		}

		#endregion
	}
}