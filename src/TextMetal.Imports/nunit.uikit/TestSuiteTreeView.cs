// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using CP.Windows.Forms;

using NUnit.Core;
using NUnit.Core.Filters;
using NUnit.Util;

namespace NUnit.UiKit
{
	public delegate void SelectedTestChangedHandler(ITest test);

	public delegate void CheckedTestChangedHandler(ITest[] tests);

	/// <summary>
	/// TestSuiteTreeView is a tree view control
	/// specialized for displaying the tests
	/// in an assembly. Clients should always
	/// use TestNode rather than TreeNode when
	/// dealing with this class to be sure of
	/// calling the proper methods.
	/// </summary>
	public class TestSuiteTreeView : TreeView
	{
		#region Constructors/Destructors

		public TestSuiteTreeView()
		{
			this.InitializeComponent();

			this.ContextMenu = new ContextMenu();
			this.ContextMenu.Popup += new EventHandler(this.ContextMenu_Popup);

			this.LoadAlternateImages();

			Services.UserSettings.Changed += new SettingsEventHandler(this.UserSettings_Changed);
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(TestSuiteTreeView));
		private TestFilter categoryFilter = TestFilter.Empty;
		private IContainer components;

		/// <summary>
		/// Whether or not we track progress of tests visibly in the tree
		/// </summary>
		private bool displayProgress = true;

		/// <summary>
		/// The TestNode on which a right click was done
		/// </summary>
		private TestSuiteTreeNode explicitlySelectedNode;

		private bool fixtureLoaded = false;

		/// <summary>
		/// Source of events that the tree responds to and
		/// target for the run command.
		/// </summary>
		private ITestLoader loader;

		/// <summary>
		/// The properties dialog if displayed
		/// </summary>
		private TestPropertiesDialog propertiesDialog;

		/// <summary>
		/// True if the UI should allow a run command to be selected
		/// </summary>
		private bool runCommandEnabled = false;

		/// <summary>
		/// Whether the browser supports running tests,
		/// or just loading and examining them
		/// </summary>
		private bool runCommandSupported = true;

		private ITest[] runningTests;

		private bool showInconclusiveResults = false;
		private bool suppressEvents = false;
		public ImageList treeImages;

		/// <summary>
		/// Hashtable provides direct access to TestNodes
		/// </summary>
		private Hashtable treeMap = new Hashtable();

		#endregion

		#region Properties/Indexers/Events

		public event EventHandler CheckBoxesChanged;
		public event CheckedTestChangedHandler CheckedTestChanged;
		public event SelectedTestChangedHandler SelectedTestChanged;

		public TestSuiteTreeNode this[string uniqueName]
		{
			get
			{
				return this.treeMap[uniqueName] as TestSuiteTreeNode;
			}
		}

		/// <summary>
		/// Test node corresponding to a test
		/// </summary>
		private TestSuiteTreeNode this[ITest test]
		{
			get
			{
				return this.FindNode(test);
			}
		}

		/// <summary>
		/// Test node corresponding to a TestResultInfo
		/// </summary>
		private TestSuiteTreeNode this[TestResult result]
		{
			get
			{
				return this.FindNode(result.Test);
			}
		}

		[Browsable(false)]
		public TestFilter CategoryFilter
		{
			get
			{
				return this.categoryFilter;
			}
			set
			{
				this.categoryFilter = value;

				TestFilterVisitor visitor = new TestFilterVisitor(this.categoryFilter);
				this.Accept(visitor);
			}
		}

		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Indicates whether checkboxes are displayed beside test nodes")]
		public new bool CheckBoxes
		{
			get
			{
				return base.CheckBoxes;
			}
			set
			{
				if (base.CheckBoxes != value)
				{
					VisualState visualState = !value && this.TopNode != null
						? new VisualState(this)
						: null;

					base.CheckBoxes = value;

					if (this.CheckBoxesChanged != null)
						this.CheckBoxesChanged(this, new EventArgs());

					if (visualState != null)
					{
						try
						{
							this.suppressEvents = true;
							visualState.ShowCheckBoxes = this.CheckBoxes;
							//RestoreVisualState( visualState );
							visualState.Restore(this);
						}
						finally
						{
							this.suppressEvents = false;
						}
					}
				}
			}
		}

		[Browsable(false)]
		public ITest[] CheckedTests
		{
			get
			{
				CheckedTestFinder finder = new CheckedTestFinder(this);
				return finder.GetCheckedTests(CheckedTestFinder.SelectionFlags.All);
			}
		}

		/// <summary>
		/// Property determining whether tree should reDraw nodes
		/// as tests are complete in order to show progress.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Indicates whether results should be displayed in the tree as each test completes")]
		public bool DisplayTestProgress
		{
			get
			{
				return this.displayProgress;
			}
			set
			{
				this.displayProgress = value;
			}
		}

		[Browsable(false)]
		public ITest[] FailedTests
		{
			get
			{
				FailedTestsFilterVisitor visitor = new FailedTestsFilterVisitor();
				this.Accept(visitor);
				return visitor.Tests;
			}
		}

		/// <summary>
		/// Property determining whether the run command
		/// is supported from the tree context menu and
		/// by double-clicking test cases.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Indicates whether the tree context menu should include a run command")]
		public bool RunCommandSupported
		{
			get
			{
				return this.runCommandSupported;
			}
			set
			{
				this.runCommandSupported = value;
			}
		}

		/// <summary>
		/// The currently selected test.
		/// </summary>
		[Browsable(false)]
		public ITest SelectedTest
		{
			get
			{
				TestSuiteTreeNode node = (TestSuiteTreeNode)this.SelectedNode;
				return node == null ? null : node.Test;
			}
		}

		/// <summary>
		/// The currently selected test result or null
		/// </summary>
		[Browsable(false)]
		public TestResult SelectedTestResult
		{
			get
			{
				TestSuiteTreeNode node = (TestSuiteTreeNode)this.SelectedNode;
				return node == null ? null : node.Result;
			}
		}

		[Browsable(false)]
		public ITest[] SelectedTests
		{
			get
			{
				ITest[] result = null;

				if (this.CheckBoxes)
				{
					CheckedTestFinder finder = new CheckedTestFinder(this);
					result = finder.GetCheckedTests(
						CheckedTestFinder.SelectionFlags.Top | CheckedTestFinder.SelectionFlags.Explicit);
				}

				if (result == null || result.Length == 0)
				{
					if (this.SelectedTest != null)
						result = new ITest[] { this.SelectedTest };
				}

				return result;
			}
		}

		public bool ShowInconclusiveResults
		{
			get
			{
				return this.showInconclusiveResults;
			}
		}

		#endregion

		#region Methods/Operators

		public void Accept(TestSuiteTreeNodeVisitor visitor)
		{
			foreach (TestSuiteTreeNode node in this.Nodes)
				node.Accept(visitor);
		}

		private void AddToMap(TestSuiteTreeNode node)
		{
			string key = node.Test.TestName.UniqueName;

			if (this.treeMap.ContainsKey(key))
				log.Error("Duplicate entry: " + key);
				//				UserMessage.Display( string.Format( 
				//					"The test {0} is duplicated\r\rResults will not be displayed correctly in the tree.", node.Test.FullName ), "Duplicate Test" );
			else
			{
				log.Debug("Added to map: " + node.Test.TestName.UniqueName);
				this.treeMap.Add(key, node);
			}
		}

		/// <summary>
		/// Add nodes to the tree constructed from a test
		/// </summary>
		/// <param name="nodes"> The TreeNodeCollection to which the new node should be added </param>
		/// <param name="rootTest"> The test for which a node is to be built </param>
		/// <param name="highlight"> If true, highlight the text for this node in the tree </param>
		/// <returns> A newly constructed TestNode, possibly with descendant nodes </returns>
		private TestSuiteTreeNode AddTreeNodes(IList nodes, TestNode rootTest, bool highlight)
		{
			TestSuiteTreeNode node = new TestSuiteTreeNode(rootTest);
			//			if ( highlight ) node.ForeColor = Color.Blue;
			this.AddToMap(node);

			nodes.Add(node);

			if (rootTest.IsSuite)
			{
				foreach (TestNode test in rootTest.Tests)
					AddTreeNodes(node.Nodes, test, highlight);
			}

			return node;
		}

		private TestSuiteTreeNode AddTreeNodes(IList nodes, TestResult rootResult, bool highlight)
		{
			TestSuiteTreeNode node = new TestSuiteTreeNode(rootResult);
			this.AddToMap(node);

			nodes.Add(node);

			if (rootResult.HasResults)
			{
				foreach (TestResult result in rootResult.Results)
					AddTreeNodes(node.Nodes, result, highlight);
			}

			node.UpdateImageIndex();

			return node;
		}

		public void CheckFailedNodes()
		{
			this.Accept(new CheckFailedNodesVisitor());
		}

		private void CheckPropertiesDialog()
		{
			if (this.propertiesDialog != null && !this.propertiesDialog.Pinned)
				this.propertiesDialog.Close();
		}

		/// <summary>
		/// Clear all the info in the tree.
		/// </summary>
		public void Clear()
		{
			this.treeMap.Clear();
			this.Nodes.Clear();
		}

		/// <summary>
		/// Clear all the results in the tree.
		/// </summary>
		public void ClearAllResults()
		{
			foreach (TestSuiteTreeNode rootNode in this.Nodes)
				rootNode.ClearResults();
		}

		public void ClearCheckedNodes()
		{
			this.Accept(new ClearCheckedNodesVisitor());
		}

		private void ClosePropertiesDialog()
		{
			if (this.propertiesDialog != null)
				this.propertiesDialog.Close();
		}

		/// <summary>
		/// Build treeview context menu dynamically on popup
		/// </summary>
		private void ContextMenu_Popup(object sender, EventArgs e)
		{
			this.ContextMenu.MenuItems.Clear();

			TestSuiteTreeNode targetNode = this.explicitlySelectedNode != null ? this.explicitlySelectedNode : (TestSuiteTreeNode)this.SelectedNode;
			if (targetNode == null)
				return;

			if (this.RunCommandSupported)
			{
				// TODO: handle in Starting event
				if (this.loader.Running)
					this.runCommandEnabled = false;

				MenuItem runMenuItem = new MenuItem("&Run", new EventHandler(this.runMenuItem_Click));
				runMenuItem.DefaultItem = runMenuItem.Enabled = this.runCommandEnabled && targetNode.Included &&
																(targetNode.Test.RunState == RunState.Runnable || targetNode.Test.RunState == RunState.Explicit);

				this.ContextMenu.MenuItems.Add(runMenuItem);

				this.ContextMenu.MenuItems.Add("-");
			}

			TestSuiteTreeNode theoryNode = targetNode.GetTheoryNode();
			if (theoryNode != null)
			{
				MenuItem failedAssumptionsMenuItem = new MenuItem("Show Failed Assumptions", new EventHandler(this.failedAssumptionsMenuItem_Click));
				failedAssumptionsMenuItem.Checked = theoryNode.ShowFailedAssumptions;
				this.ContextMenu.MenuItems.Add(failedAssumptionsMenuItem);

				this.ContextMenu.MenuItems.Add("-");
			}

			MenuItem showCheckBoxesMenuItem = new MenuItem("Show CheckBoxes", new EventHandler(this.showCheckBoxesMenuItem_Click));
			showCheckBoxesMenuItem.Checked = this.CheckBoxes;
			this.ContextMenu.MenuItems.Add(showCheckBoxesMenuItem);
			this.ContextMenu.MenuItems.Add("-");

			MenuItem loadFixtureMenuItem = new MenuItem("Load Fixture", new EventHandler(this.loadFixtureMenuItem_Click));
			loadFixtureMenuItem.Enabled = targetNode.Test.IsSuite && targetNode != this.Nodes[0];
			this.ContextMenu.MenuItems.Add(loadFixtureMenuItem);

			MenuItem clearFixtureMenuItem = new MenuItem("Clear Fixture", new EventHandler(this.clearFixtureMenuItem_Click));
			clearFixtureMenuItem.Enabled = this.fixtureLoaded;
			this.ContextMenu.MenuItems.Add(clearFixtureMenuItem);
			this.ContextMenu.MenuItems.Add("-");

			MenuItem propertiesMenuItem = new MenuItem(
				"&Properties", new EventHandler(this.propertiesMenuItem_Click));

			this.ContextMenu.MenuItems.Add(propertiesMenuItem);
		}

		private TestSuiteTreeNode FindNode(ITest test)
		{
			TestSuiteTreeNode node = this.treeMap[test.TestName.UniqueName] as TestSuiteTreeNode;

			if (node == null)
				node = this.FindNodeByName(test.TestName.FullName);

			return node;
		}

		private TestSuiteTreeNode FindNodeByName(string fullName)
		{
			foreach (string uname in this.treeMap.Keys)
			{
				int rbrack = uname.IndexOf(']');
				string name = rbrack >= 0 ? uname.Substring(rbrack + 1) : uname;
				if (name == fullName)
					return this.treeMap[uname] as TestSuiteTreeNode;
			}

			return null;
		}

		/// <summary>
		/// Helper used to figure out the display style
		/// to use when the setting is Auto
		/// </summary>
		/// <returns> DisplayStyle to be used </returns>
		private DisplayStyle GetDisplayStyle()
		{
			DisplayStyle initialDisplay = (DisplayStyle)
				Services.UserSettings.GetSetting("Gui.TestTree.InitialTreeDisplay", DisplayStyle.Auto);

			if (initialDisplay != DisplayStyle.Auto)
				return initialDisplay;

			if (this.VisibleCount >= this.GetNodeCount(true))
				return DisplayStyle.Expand;

			return DisplayStyle.HideTests;
		}

		public void HideTests()
		{
			this.BeginUpdate();
			foreach (TestSuiteTreeNode node in this.Nodes)
				this.HideTestsUnderNode(node);
			this.EndUpdate();
		}

		/// <summary>
		/// Helper collapses all fixtures under a node
		/// </summary>
		/// <param name="node"> Node under which to collapse fixtures </param>
		private void HideTestsUnderNode(TestSuiteTreeNode node)
		{
			if (node.Test.IsSuite)
			{
				if (node.Test.TestType == "TestFixture")
					node.Collapse();
				else
				{
					node.Expand();

					foreach (TestSuiteTreeNode child in node.Nodes)
						this.HideTestsUnderNode(child);
				}
			}
		}

		public void Initialize(ITestLoader loader, ITestEvents events)
		{
			this.loader = loader;

			events.TestLoaded += new TestEventHandler(this.OnTestLoaded);
			events.TestReloaded += new TestEventHandler(this.OnTestChanged);
			events.TestUnloaded += new TestEventHandler(this.OnTestUnloaded);

			events.RunStarting += new TestEventHandler(this.OnRunStarting);
			events.RunFinished += new TestEventHandler(this.OnRunFinished);
			events.TestFinished += new TestEventHandler(this.OnTestResult);
			events.SuiteFinished += new TestEventHandler(this.OnTestResult);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager resources = new ComponentResourceManager(typeof(TestSuiteTreeView));
			this.treeImages = new ImageList(this.components);
			this.SuspendLayout();
			// 
			// treeImages
			// 
			this.treeImages.ImageStream = ((ImageListStreamer)(resources.GetObject("treeImages.ImageStream")));
			this.treeImages.TransparentColor = Color.White;
			this.treeImages.Images.SetKeyName(0, "Skipped.png");
			this.treeImages.Images.SetKeyName(1, "Failure.png");
			this.treeImages.Images.SetKeyName(2, "Success.png");
			this.treeImages.Images.SetKeyName(3, "Ignored.png");
			this.treeImages.Images.SetKeyName(4, "Inconclusive.png");
			// 
			// TestSuiteTreeView
			// 
			this.ImageIndex = 0;
			this.ImageList = this.treeImages;
			this.SelectedImageIndex = 0;
			this.DoubleClick += new EventHandler(this.TestSuiteTreeView_DoubleClick);
			this.DragDrop += new DragEventHandler(this.TestSuiteTreeView_DragDrop);
			this.DragEnter += new DragEventHandler(this.TestSuiteTreeView_DragEnter);
			this.ResumeLayout(false);
		}

		/// <summary>
		/// Helper method to determine if an IDataObject is valid
		/// for dropping on the tree view. It must be a the drop
		/// of a single file with a valid assembly file type.
		/// </summary>
		/// <param name="data"> IDataObject to be tested </param>
		/// <returns> True if dropping is allowed </returns>
		private bool IsValidFileDrop(IDataObject data)
		{
			if (!data.GetDataPresent(DataFormats.FileDrop))
				return false;

			string[] fileNames = data.GetData(DataFormats.FileDrop) as string[];

			if (fileNames == null || fileNames.Length == 0)
				return false;

			// We can't open more than one project at a time
			// so handle length of 1 separately.
			if (fileNames.Length == 1)
			{
				string fileName = fileNames[0];
				bool isProject = NUnitProject.IsNUnitProjectFile(fileName);
				if (Services.UserSettings.GetSetting("Options.TestLoader.VisualStudioSupport", false))
					isProject |= Services.ProjectService.CanConvertFrom(fileName);

				return isProject || PathUtils.IsAssemblyFileType(fileName);
			}

			// Multiple assemblies are allowed - we
			// assume they are all in the same directory
			// since they are being dragged together.
			foreach (string fileName in fileNames)
			{
				if (!PathUtils.IsAssemblyFileType(fileName))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Load the tree with a test hierarchy
		/// </summary>
		/// <param name="test"> Test to be loaded </param>
		public void Load(TestNode test)
		{
			using (new WaitCursor())
			{
				this.Clear();
				this.BeginUpdate();

				try
				{
					AddTreeNodes(this.Nodes, test, false);
					this.SetInitialExpansion();
				}
				finally
				{
					this.EndUpdate();
					this.explicitlySelectedNode = null;
					this.Select();
				}

				if (Services.UserSettings.GetSetting("Gui.TestTree.SaveVisualState", true) && this.loader != null)
					this.RestoreVisualState();
			}
		}

		/// <summary>
		/// Load the tree from a test result
		/// </summary>
		/// <param name="result"> </param>
		public void Load(TestResult result)
		{
			using (new WaitCursor())
			{
				this.Clear();
				this.BeginUpdate();

				try
				{
					AddTreeNodes(this.Nodes, result, false);
					this.SetInitialExpansion();
				}
				finally
				{
					this.EndUpdate();
				}
			}
		}

		private void LoadAlternateImage(int index, string name, string imageSet)
		{
			string imageDir = PathUtils.Combine(Assembly.GetExecutingAssembly(), "Images", "Tree", imageSet);

			string[] extensions = { ".png", ".jpg" };

			foreach (string ext in extensions)
			{
				string filePath = Path.Combine(imageDir, name + ext);
				if (File.Exists(filePath))
				{
					this.treeImages.Images[index] = Image.FromFile(filePath);
					break;
				}
			}
		}

		private void LoadAlternateImages()
		{
			string imageSet = Services.UserSettings.GetSetting("Gui.TestTree.AlternateImageSet") as string;

			if (imageSet != null)
			{
				string[] imageNames = { "Skipped", "Failure", "Success", "Ignored", "Inconclusive" };

				for (int index = 0; index < imageNames.Length; index++)
					this.LoadAlternateImage(index, imageNames[index], imageSet);
			}
		}

		private TestFilter MakeFilter(ITest[] tests)
		{
			TestFilter nameFilter = this.MakeNameFilter(tests);

			if (nameFilter == TestFilter.Empty)
				return this.CategoryFilter;

			if (tests.Length == 1)
			{
				TestSuiteTreeNode rootNode = (TestSuiteTreeNode)this.Nodes[0];
				if (tests[0] == rootNode.Test)
					return this.CategoryFilter;
			}

			if (this.CategoryFilter.IsEmpty)
				return nameFilter;

			return new AndFilter(nameFilter, this.CategoryFilter);
		}

		private TestFilter MakeNameFilter(ITest[] tests)
		{
			if (tests == null || tests.Length == 0)
				return TestFilter.Empty;

			NameFilter nameFilter = new NameFilter();
			foreach (ITest test in tests)
				nameFilter.Add(test.TestName);

			return nameFilter;
		}

		protected override void OnAfterCheck(TreeViewEventArgs e)
		{
			if (!this.suppressEvents)
			{
				if (this.CheckedTestChanged != null)
					this.CheckedTestChanged(this.CheckedTests);

				base.OnAfterCheck(e);
			}
		}

		protected override void OnAfterCollapse(TreeViewEventArgs e)
		{
			if (!this.suppressEvents)
				base.OnAfterCollapse(e);
		}

		protected override void OnAfterExpand(TreeViewEventArgs e)
		{
			if (!this.suppressEvents)
				base.OnAfterExpand(e);
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			this.explicitlySelectedNode = null;

			if (!this.suppressEvents)
			{
				if (this.SelectedTestChanged != null)
					this.SelectedTestChanged(this.SelectedTest);

				base.OnAfterSelect(e);
			}
		}

		/// <summary>
		/// Handles right mouse button down by
		/// remembering the proper context item
		/// and implements multiple select with the left button.
		/// </summary>
		/// <param name="e"> MouseEventArgs structure with information about the mouse position and button state </param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				this.CheckPropertiesDialog();
				TreeNode theNode = this.GetNodeAt(e.X, e.Y);
				this.explicitlySelectedNode = theNode as TestSuiteTreeNode;
			}
//			else if (e.Button == MouseButtons.Left )
//			{
//				if ( Control.ModifierKeys == Keys.Control )
//				{
//					TestSuiteTreeNode theNode = GetNodeAt( e.X, e.Y ) as TestSuiteTreeNode;
//					if ( theNode != null )
//						theNode.IsSelected = true;
//				}
//				else
//				{
//					ClearSelected();
//				}
//			}

			base.OnMouseDown(e);
		}

		private void OnPropertiesDialogClosed(object sender, EventArgs e)
		{
			this.propertiesDialog = null;
		}

		private void OnRunFinished(object sender, TestEventArgs e)
		{
			if (this.runningTests != null)
			{
				foreach (ITest test in this.runningTests)
					this[test].Expand();
			}

			if (this.propertiesDialog != null)
				this.propertiesDialog.Invoke(new PropertiesDisplayHandler(this.propertiesDialog.DisplayProperties));

			this.runningTests = null;
			this.runCommandEnabled = true;
		}

		private void OnRunStarting(object sender, TestEventArgs e)
		{
			this.CheckPropertiesDialog();
#if ACCUMULATE_RESULTS
			if ( runningTests != null )
				foreach( ITest test in runningTests )
					this[test].ClearResults();
#else
			this.ClearAllResults();
#endif
			this.runCommandEnabled = false;
		}

		private void OnTestChanged(object sender, TestEventArgs e)
		{
			TestNode test = e.Test as TestNode;
			if (test != null)
				this.Invoke(new LoadHandler(this.Reload), new object[] { test });
		}

		private void OnTestLoaded(object sender, TestEventArgs e)
		{
			this.CheckPropertiesDialog();
			TestNode test = e.Test as TestNode;
			if (test != null)
				Load(test);
			this.runCommandEnabled = true;
		}

		private void OnTestResult(object sender, TestEventArgs e)
		{
			this.SetTestResult(e.Result);
		}

		private void OnTestUnloaded(object sender, TestEventArgs e)
		{
			this.ClosePropertiesDialog();

			if (Services.UserSettings.GetSetting("Gui.TestTree.SaveVisualState", true) && this.loader != null)
			{
				try
				{
					new VisualState(this).Save(VisualState.GetVisualStateFileName(this.loader.TestFileName));
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Unable to save visual state.");
					Debug.WriteLine(ex);
				}
			}

			this.Clear();
			this.explicitlySelectedNode = null;
			this.runCommandEnabled = false;
		}

		/// <summary>
		/// Reload the tree with a changed test hierarchy
		/// while maintaining as much gui state as possible.
		/// </summary>
		/// <param name="test"> Test suite to be loaded </param>
		public void Reload(TestNode test)
		{
			TestResult result = ((TestSuiteTreeNode)this.Nodes[0]).Result;
			VisualState visualState = new VisualState(this);

			Load(test);

			visualState.Restore(this);

			if (result != null && !Services.UserSettings.GetSetting("Options.TestLoader.ClearResultsOnReload", false))
				this.RestoreResults(result);
		}

		private void RemoveFromMap(TestSuiteTreeNode node)
		{
			foreach (TestSuiteTreeNode child in node.Nodes)
				this.RemoveFromMap(child);
			this.treeMap.Remove(node.Test.TestName.UniqueName);
		}

		/// <summary>
		/// Remove a node from the tree itself and the hashtable
		/// </summary>
		/// <param name="node"> Node to remove </param>
		private void RemoveNode(TestSuiteTreeNode node)
		{
			if (this.explicitlySelectedNode == node)
				this.explicitlySelectedNode = null;
			this.RemoveFromMap(node);
			node.Remove();
		}

		private void RestoreResults(TestResult result)
		{
			if (result.HasResults)
			{
				foreach (TestResult childResult in result.Results)
					this.RestoreResults(childResult);
			}

			this.SetTestResult(result);
		}

		private void RestoreVisualState()
		{
			if (this.loader != null)
			{
				string fileName = VisualState.GetVisualStateFileName(this.loader.TestFileName);
				if (File.Exists(fileName))
					VisualState.LoadFrom(fileName).Restore(this);
			}
		}

		public void RunAllTests()
		{
			this.RunAllTests(true);
		}

		public void RunAllTests(bool ignoreCategories)
		{
			if (this.Nodes.Count > 0)
			{
				this.runCommandEnabled = false;
				this.RunTests(new ITest[] { ((TestSuiteTreeNode)this.Nodes[0]).Test }, ignoreCategories);
			}
		}

		public void RunFailedTests()
		{
			this.runCommandEnabled = false;
			this.RunTests(this.FailedTests, true);
		}

		public void RunSelectedTests()
		{
			this.runCommandEnabled = false;
			this.RunTests(this.SelectedTests, false);
		}

		private void RunTests(ITest[] tests, bool ignoreCategories)
		{
			if (tests != null && tests.Length > 0)
			{
				this.runningTests = tests;

				ITestFilter filter = ignoreCategories
					? this.MakeNameFilter(tests)
					: this.MakeFilter(tests);

				this.loader.RunTests(filter);
			}
		}

		public void SetInitialExpansion()
		{
			this.CollapseAll();

			switch (this.GetDisplayStyle())
			{
				case DisplayStyle.Expand:
					this.ExpandAll();
					break;
				case DisplayStyle.HideTests:
					this.HideTests();
					break;
				case DisplayStyle.Collapse:
				default:
					break;
			}

			this.SelectedNode = this.Nodes[0];
			this.SelectedNode.EnsureVisible();
		}

		/// <summary>
		/// Add the result of a test to the tree
		/// </summary>
		/// <param name="result"> The result of the test </param>
		public void SetTestResult(TestResult result)
		{
			TestSuiteTreeNode node = this[result];
			if (node == null)
				Debug.WriteLine("Test not found in tree: " + result.Test.TestName.UniqueName);
			else
			{
				node.Result = result;

				if (result.Test.TestType == "Theory")
					node.RepopulateTheoryNode();

				if (this.DisplayTestProgress && node.IsVisible)
				{
					Invalidate(node.Bounds);
					this.Update();
				}
			}
		}

		public void ShowPropertiesDialog(ITest test)
		{
			ShowPropertiesDialog(this[test]);
		}

		private void ShowPropertiesDialog(TestSuiteTreeNode node)
		{
			if (this.propertiesDialog == null)
			{
				Form owner = this.FindForm();
				this.propertiesDialog = new TestPropertiesDialog(node);
				this.propertiesDialog.Owner = owner;
				this.propertiesDialog.Font = owner.Font;
				this.propertiesDialog.StartPosition = FormStartPosition.Manual;
				this.propertiesDialog.Left = Math.Max(0, owner.Left + (owner.Width - this.propertiesDialog.Width) / 2);
				this.propertiesDialog.Top = Math.Max(0, owner.Top + (owner.Height - this.propertiesDialog.Height) / 2);
				this.propertiesDialog.Show();
				this.propertiesDialog.Closed += new EventHandler(this.OnPropertiesDialogClosed);
			}
			else
				this.propertiesDialog.DisplayProperties(node);
		}

		private void TestSuiteTreeView_DoubleClick(object sender, EventArgs e)
		{
			TestSuiteTreeNode node = this.SelectedNode as TestSuiteTreeNode;
			if (this.runCommandSupported && this.runCommandEnabled && node.Nodes.Count == 0 && node.Included)
			{
				this.runCommandEnabled = false;

				// TODO: Since this is a terminal node, don't use a category filter
				this.RunTests(new ITest[] { this.SelectedTest }, true);
			}
		}

		private void TestSuiteTreeView_DragDrop(object sender, DragEventArgs e)
		{
			if (this.IsValidFileDrop(e.Data))
			{
				string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (fileNames.Length == 1)
					this.loader.LoadProject(fileNames[0]);
				else
					this.loader.LoadProject(fileNames);

				if (this.loader.IsProjectLoaded && this.loader.TestProject.IsLoadable)
					this.loader.LoadTest();
			}
		}

		private void TestSuiteTreeView_DragEnter(object sender, DragEventArgs e)
		{
			if (this.IsValidFileDrop(e.Data))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void UserSettings_Changed(object sender, SettingsEventArgs args)
		{
			if (args.SettingName == "Gui.TestTree.AlternateImageSet")
			{
				this.LoadAlternateImages();
				this.Invalidate();
			}
		}

		private void clearFixtureMenuItem_Click(object sender, EventArgs e)
		{
			this.loader.LoadTest();
			this.fixtureLoaded = false;
		}

		private void collapseAllMenuItem_Click(object sender, EventArgs e)
		{
			this.BeginUpdate();
			this.CollapseAll();
			this.EndUpdate();

			// Compensate for a bug in the underlying control
			if (this.Nodes.Count > 0)
				this.SelectedNode = this.Nodes[0];
		}

		/// <summary>
		/// When Collapse context menu item is clicked, collapse the node
		/// </summary>
		private void collapseMenuItem_Click(object sender, EventArgs e)
		{
			TestSuiteTreeNode targetNode = this.explicitlySelectedNode != null ? this.explicitlySelectedNode : (TestSuiteTreeNode)this.SelectedNode;
			if (targetNode != null)
				targetNode.Collapse();
		}

		private void expandAllMenuItem_Click(object sender, EventArgs e)
		{
			this.BeginUpdate();
			this.ExpandAll();
			this.EndUpdate();
		}

		/// <summary>
		/// When Expand context menu item is clicked, expand the node
		/// </summary>
		private void expandMenuItem_Click(object sender, EventArgs e)
		{
			TestSuiteTreeNode targetNode = this.explicitlySelectedNode != null ? this.explicitlySelectedNode : (TestSuiteTreeNode)this.SelectedNode;
			if (targetNode != null)
				targetNode.Expand();
		}

		private void failedAssumptionsMenuItem_Click(object sender, EventArgs e)
		{
			TestSuiteTreeNode targetNode = this.explicitlySelectedNode != null ? this.explicitlySelectedNode : (TestSuiteTreeNode)this.SelectedNode;
			TestSuiteTreeNode theoryNode = targetNode != null ? targetNode.GetTheoryNode() : null;
			if (theoryNode != null)
			{
				MenuItem item = (MenuItem)sender;

				this.BeginUpdate();
				item.Checked = !item.Checked;
				theoryNode.ShowFailedAssumptions = item.Checked;
				this.EndUpdate();
			}
		}

		private void loadFixtureMenuItem_Click(object sender, EventArgs e)
		{
			if (this.explicitlySelectedNode != null)
			{
				this.loader.LoadTest(this.explicitlySelectedNode.Test.TestName.FullName);
				this.fixtureLoaded = true;
			}
		}

		private void propertiesMenuItem_Click(object sender, EventArgs e)
		{
			TestSuiteTreeNode targetNode = this.explicitlySelectedNode != null ? this.explicitlySelectedNode : (TestSuiteTreeNode)this.SelectedNode;
			if (targetNode != null)
				ShowPropertiesDialog(targetNode);
		}

		private void runAllMenuItem_Click(object sender, EventArgs e)
		{
			if (this.runCommandEnabled)
			{
				this.runCommandEnabled = false;
				this.RunAllTests();
			}
		}

		private void runFailedMenuItem_Click(object sender, EventArgs e)
		{
			if (this.runCommandEnabled)
			{
				this.runCommandEnabled = false;
				this.RunFailedTests();
			}
		}

		/// <summary>
		/// When Run context menu item is clicked, run the test that
		/// was selected when the right click was done.
		/// </summary>
		private void runMenuItem_Click(object sender, EventArgs e)
		{
			//TODO: some sort of lock on these booleans?
			if (this.runCommandEnabled)
			{
				this.runCommandEnabled = false;

				if (this.explicitlySelectedNode != null)
					this.RunTests(new ITest[] { this.explicitlySelectedNode.Test }, false);
				else
					this.RunSelectedTests();
			}
		}

		private void showCheckBoxesMenuItem_Click(object sender, EventArgs e)
		{
			this.CheckBoxes = !this.CheckBoxes;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// Indicates how a tree should be displayed
		/// </summary>
		public enum DisplayStyle
		{
			Auto, // Select based on space available
			Expand, // Expand fully
			Collapse, // Collpase fully
			HideTests // Expand all but the fixtures, leaving
			// leaf nodes hidden
		}

		/// <summary>
		/// Delegate for use in invoking the tree loader
		/// from the watcher thread.
		/// </summary>
		private delegate void LoadHandler(TestNode test);

		private delegate void PropertiesDisplayHandler();

		private class TreeStructureChangedException : Exception
		{
			#region Constructors/Destructors

			public TreeStructureChangedException(string message)
				: base(message)
			{
			}

			#endregion
		}

		#endregion
	}

	#region Helper Classes

	#region ClearCheckedNodesVisitor

	internal class ClearCheckedNodesVisitor : TestSuiteTreeNodeVisitor
	{
		#region Methods/Operators

		public override void Visit(TestSuiteTreeNode node)
		{
			node.Checked = false;
		}

		#endregion
	}

	#endregion

	#region CheckFailedNodesVisitor

	internal class CheckFailedNodesVisitor : TestSuiteTreeNodeVisitor
	{
		#region Methods/Operators

		public override void Visit(TestSuiteTreeNode node)
		{
			if (!node.Test.IsSuite && node.HasResult &&
				(node.Result.ResultState == ResultState.Failure ||
				node.Result.ResultState == ResultState.Error))
			{
				node.Checked = true;
				node.EnsureVisible();
			}
			else
				node.Checked = false;
		}

		#endregion
	}

	#endregion

	#region FailedTestsFilterVisitor

	internal class FailedTestsFilterVisitor : TestSuiteTreeNodeVisitor
	{
		#region Fields/Constants

		private List<ITest> tests = new List<ITest>();

		#endregion

		#region Properties/Indexers/Events

		public ITest[] Tests
		{
			get
			{
				return this.tests.ToArray();
			}
		}

		#endregion

		#region Methods/Operators

		public override void Visit(TestSuiteTreeNode node)
		{
			if (!node.Test.IsSuite && node.HasResult &&
				(node.Result.ResultState == ResultState.Failure ||
				node.Result.ResultState == ResultState.Error))
				this.tests.Add(node.Test);
		}

		#endregion
	}

	#endregion

	#region TestFilterVisitor

	public class TestFilterVisitor : TestSuiteTreeNodeVisitor
	{
		#region Constructors/Destructors

		public TestFilterVisitor(ITestFilter filter)
		{
			this.filter = filter;
		}

		#endregion

		#region Fields/Constants

		private ITestFilter filter;

		#endregion

		#region Methods/Operators

		public override void Visit(TestSuiteTreeNode node)
		{
			node.Included = this.filter.Pass(node.Test);
		}

		#endregion
	}

	#endregion

	#region CheckedTestFinder

	internal class CheckedTestFinder
	{
		#region Constructors/Destructors

		public CheckedTestFinder(TestSuiteTreeView treeView)
		{
			FindCheckedNodes(treeView.Nodes, true);
		}

		#endregion

		#region Fields/Constants

		private List<CheckedTestInfo> checkedTests = new List<CheckedTestInfo>();

		#endregion

		#region Methods/Operators

		private void FindCheckedNodes(TestSuiteTreeNode node, bool topLevel)
		{
			if (node.Checked)
			{
				this.checkedTests.Add(new CheckedTestInfo(node.Test, topLevel));
				topLevel = false;
			}

			FindCheckedNodes(node.Nodes, topLevel);
		}

		private void FindCheckedNodes(TreeNodeCollection nodes, bool topLevel)
		{
			foreach (TestSuiteTreeNode node in nodes)
				FindCheckedNodes(node, topLevel);
		}

		public ITest[] GetCheckedTests(SelectionFlags flags)
		{
			int count = 0;
			foreach (CheckedTestInfo info in this.checkedTests)
			{
				if (this.isSelected(info, flags))
					count++;
			}

			ITest[] result = new ITest[count];

			int index = 0;
			foreach (CheckedTestInfo info in this.checkedTests)
			{
				if (this.isSelected(info, flags))
					result[index++] = info.Test;
			}

			return result;
		}

		private bool isSelected(CheckedTestInfo info, SelectionFlags flags)
		{
			if (info.TopLevel && (flags & SelectionFlags.Top) != 0)
				return true;
			else if (!info.TopLevel && (flags & SelectionFlags.Sub) != 0)
				return true;
			else if (info.Test.RunState == RunState.Explicit && (flags & SelectionFlags.Explicit) != 0)
				return true;
			else
				return false;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private struct CheckedTestInfo
		{
			#region Constructors/Destructors

			public CheckedTestInfo(ITest test, bool topLevel)
			{
				this.Test = test;
				this.TopLevel = topLevel;
			}

			#endregion

			#region Fields/Constants

			public ITest Test;
			public bool TopLevel;

			#endregion
		}

		[Flags]
		public enum SelectionFlags
		{
			Top = 1,
			Sub = 2,
			Explicit = 4,
			All = Top + Sub
		}

		#endregion
	}

	#endregion

	#endregion
}