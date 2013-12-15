// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using CP.Windows.Forms;

using NUnit.Core;
using NUnit.UiException.Controls;
using NUnit.Util;

namespace NUnit.UiKit
{
	/// <summary>
	/// Summary description for ErrorDisplay.
	/// </summary>
	public class ErrorDisplay : UserControl, TestObserver
	{
		#region Constructors/Destructors

		public ErrorDisplay()
		{
			// This call is required by the Windows.Forms Form Designer.
			this.InitializeComponent();
		}

		#endregion

		#region Fields/Constants

		private static readonly Font DefaultFixedFont = new Font(FontFamily.GenericMonospace, 8.0F);

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private MenuItem copyDetailMenuItem;

		private ListBox detailList;
		private ContextMenu detailListContextMenu;
		public ErrorBrowser errorBrowser;
		private int hoverIndex = -1;
		private Timer hoverTimer;
		private ISettings settings = null;
		private SourceCodeDisplay sourceCode;
		public StackTraceDisplay stackTraceDisplay;
		public Splitter tabSplitter;
		private TipWindow tipWindow;
		private bool wordWrap = false;

		#endregion

		#region Properties/Indexers/Events

		private bool WordWrap
		{
			get
			{
				return this.wordWrap;
			}
			set
			{
				if (value != this.wordWrap)
				{
					this.wordWrap = value;
					this.RefillDetailList();
				}
			}
		}

		#endregion

		#region Methods/Operators

		public void Clear()
		{
			this.detailList.Items.Clear();
			this.detailList.ContextMenu = null;
			this.errorBrowser.StackTraceSource = "";
		}

		private void ClearTimer()
		{
			if (this.hoverTimer != null)
			{
				this.hoverTimer.Stop();
				this.hoverTimer.Dispose();
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
			this.detailList = new ListBox();
			this.tabSplitter = new Splitter();

			this.errorBrowser = new ErrorBrowser();
			this.sourceCode = new SourceCodeDisplay();
			this.stackTraceDisplay = new StackTraceDisplay();
			this.detailListContextMenu = new ContextMenu();
			this.copyDetailMenuItem = new MenuItem();
			this.SuspendLayout();
			// 
			// detailList
			// 
			this.detailList.Dock = DockStyle.Top;
			this.detailList.DrawMode = DrawMode.OwnerDrawVariable;
			this.detailList.Font = DefaultFixedFont;
			this.detailList.HorizontalExtent = 2000;
			this.detailList.HorizontalScrollbar = true;
			this.detailList.ItemHeight = 16;
			this.detailList.Location = new Point(0, 0);
			this.detailList.Name = "detailList";
			this.detailList.ScrollAlwaysVisible = true;
			this.detailList.Size = new Size(496, 128);
			this.detailList.TabIndex = 1;
			this.detailList.Resize += new EventHandler(this.detailList_Resize);
			this.detailList.MouseHover += new EventHandler(this.OnMouseHover);
			this.detailList.MeasureItem += new MeasureItemEventHandler(this.detailList_MeasureItem);
			this.detailList.MouseMove += new MouseEventHandler(this.detailList_MouseMove);
			this.detailList.MouseLeave += new EventHandler(this.detailList_MouseLeave);
			this.detailList.DrawItem += new DrawItemEventHandler(this.detailList_DrawItem);
			this.detailList.SelectedIndexChanged += new EventHandler(this.detailList_SelectedIndexChanged);
			// 
			// tabSplitter
			// 
			this.tabSplitter.Dock = DockStyle.Top;
			this.tabSplitter.Location = new Point(0, 128);
			this.tabSplitter.MinSize = 100;
			this.tabSplitter.Name = "tabSplitter";
			this.tabSplitter.Size = new Size(496, 9);
			this.tabSplitter.TabIndex = 3;
			this.tabSplitter.TabStop = false;
			this.tabSplitter.SplitterMoved += new SplitterEventHandler(this.tabSplitter_SplitterMoved);
			// 
			// errorBrowser
			// 
			this.errorBrowser.Dock = DockStyle.Fill;
			this.errorBrowser.Location = new Point(0, 137);
			this.errorBrowser.Name = "errorBrowser";
			this.errorBrowser.Size = new Size(496, 151);
			this.errorBrowser.StackTraceSource = null;
			this.errorBrowser.TabIndex = 4;
			//
			// configure and register SourceCodeDisplay
			//
			this.sourceCode.AutoSelectFirstItem = true;
			this.sourceCode.ListOrderPolicy = ErrorListOrderPolicy.ReverseOrder;
			this.sourceCode.SplitOrientation = Orientation.Vertical;
			this.sourceCode.SplitterDistance = 0.3f;
			this.stackTraceDisplay.Font = DefaultFixedFont;
			this.errorBrowser.RegisterDisplay(this.sourceCode);
			this.errorBrowser.RegisterDisplay(this.stackTraceDisplay);
			//
			// detailListContextMenu
			// 
			this.detailListContextMenu.MenuItems.AddRange(new MenuItem[]
														{
															this.copyDetailMenuItem
														});
			// 
			// copyDetailMenuItem
			// 
			this.copyDetailMenuItem.Index = 0;
			this.copyDetailMenuItem.Text = "Copy";
			this.copyDetailMenuItem.Click += new EventHandler(this.copyDetailMenuItem_Click);
			// 
			// ErrorDisplay
			// 
			this.Controls.Add(this.errorBrowser);
			this.Controls.Add(this.tabSplitter);
			this.Controls.Add(this.detailList);
			this.Name = "ErrorDisplay";
			this.Size = new Size(496, 288);
			this.ResumeLayout(false);
		}

		private void InsertTestResultItem(TestResult result)
		{
			TestResultItem item = new TestResultItem(result);
			InsertTestResultItem(item);
		}

		private void InsertTestResultItem(TestResultItem item)
		{
			this.detailList.BeginUpdate();
			this.detailList.Items.Insert(this.detailList.Items.Count, item);
			this.detailList.SelectedIndex = 0;
			this.detailList.EndUpdate();
		}

		protected override void OnLoad(EventArgs e)
		{
			// NOTE: DesignMode is not true when display is nested in another
			// user control and the containing form is displayed in the designer.
			// This is a problem with VS.Net.
			//
			// Consequently, we rely on the fact that Services.UserSettings
			// returns a dummy Service, if the ServiceManager has not been
			// initialized.
			if (!this.DesignMode)
			{
				this.settings = Services.UserSettings;
				this.settings.Changed += new SettingsEventHandler(this.UserSettings_Changed);

				int splitPosition = this.settings.GetSetting("Gui.ResultTabs.ErrorsTabSplitterPosition", this.tabSplitter.SplitPosition);
				if (splitPosition >= this.tabSplitter.MinSize && splitPosition < this.ClientSize.Height)
					this.tabSplitter.SplitPosition = splitPosition;

				this.WordWrap = this.settings.GetSetting("Gui.ResultTabs.ErrorsTab.WordWrapEnabled", true);

				this.detailList.Font = this.stackTraceDisplay.Font =
					this.settings.GetSetting("Gui.FixedFont", DefaultFixedFont);

				Orientation splitOrientation = (Orientation)this.settings.GetSetting(
					"Gui.ResultTabs.ErrorBrowser.SplitterOrientation", Orientation.Vertical);
				float splitterDistance = splitOrientation == Orientation.Vertical
					? this.settings.GetSetting("Gui.ResultTabs.ErrorBrowser.VerticalPosition", 0.3f)
					: this.settings.GetSetting("Gui.ResultTabs.ErrorBrowser.HorizontalPosition", 0.3f);

				this.sourceCode.SplitOrientation = splitOrientation;
				this.sourceCode.SplitterDistance = splitterDistance;

				this.sourceCode.SplitOrientationChanged += new EventHandler(this.sourceCode_SplitOrientationChanged);
				this.sourceCode.SplitterDistanceChanged += new EventHandler(this.sourceCode_SplitterDistanceChanged);

				if (this.settings.GetSetting("Gui.ResultTabs.ErrorBrowser.SourceCodeDisplay", false))
					this.errorBrowser.SelectedDisplay = this.sourceCode;
				else
					this.errorBrowser.SelectedDisplay = this.stackTraceDisplay;

				this.errorBrowser.StackTraceDisplayChanged += new EventHandler(this.errorBrowser_StackTraceDisplayChanged);
			}

			base.OnLoad(e);
		}

		private void OnMouseHover(object sender, EventArgs e)
		{
			if (this.tipWindow != null)
				this.tipWindow.Close();

			if (this.settings.GetSetting("Gui.ResultTabs.ErrorsTab.ToolTipsEnabled", false) && this.hoverIndex >= 0 && this.hoverIndex < this.detailList.Items.Count)
			{
				Graphics g = Graphics.FromHwnd(this.detailList.Handle);

				Rectangle itemRect = this.detailList.GetItemRectangle(this.hoverIndex);
				string text = this.detailList.Items[this.hoverIndex].ToString();

				SizeF sizeNeeded = g.MeasureString(text, this.detailList.Font);
				bool expansionNeeded =
					itemRect.Width < (int)sizeNeeded.Width ||
					itemRect.Height < (int)sizeNeeded.Height;

				if (expansionNeeded)
				{
					this.tipWindow = new TipWindow(this.detailList, this.hoverIndex);
					this.tipWindow.ItemBounds = itemRect;
					this.tipWindow.TipText = text;
					this.tipWindow.Expansion = TipWindow.ExpansionStyle.Both;
					this.tipWindow.Overlay = true;
					this.tipWindow.WantClicks = true;
					this.tipWindow.Closed += new EventHandler(this.tipWindow_Closed);
					this.tipWindow.Show();
				}
			}
		}

		private void OnSuiteFinished(object sender, TestEventArgs args)
		{
			TestResult result = args.Result;
			if (result.FailureSite != FailureSite.Child)
			{
				switch (result.ResultState)
				{
					case ResultState.Failure:
					case ResultState.Error:
					case ResultState.Cancelled:
						InsertTestResultItem(result);
						break;
				}
			}
		}

		private void OnTestException(object sender, TestEventArgs args)
		{
			string msg = string.Format("An unhandled {0} was thrown while executing this test : {1}",
				args.Exception.GetType().FullName, args.Exception.Message);
			TestResultItem item = new TestResultItem(args.Name, msg, args.Exception.StackTrace);

			InsertTestResultItem(item);
		}

		private void OnTestFinished(object sender, TestEventArgs args)
		{
			TestResult result = args.Result;
			switch (result.ResultState)
			{
				case ResultState.Failure:
				case ResultState.Error:
				case ResultState.Cancelled:
					if (result.FailureSite != FailureSite.Parent)
						InsertTestResultItem(result);
					break;
				case ResultState.NotRunnable:
					InsertTestResultItem(result);
					break;
			}
		}

		private void RefillDetailList()
		{
			if (this.detailList.Items.Count > 0)
			{
				this.detailList.BeginUpdate();
				ArrayList copiedItems = new ArrayList(this.detailList.Items);
				this.detailList.Items.Clear();
				foreach (object item in copiedItems)
					this.detailList.Items.Add(item);
				this.detailList.EndUpdate();
			}
		}

		public void Subscribe(ITestEvents events)
		{
			events.TestFinished += new TestEventHandler(this.OnTestFinished);
			events.SuiteFinished += new TestEventHandler(this.OnSuiteFinished);
			events.TestException += new TestEventHandler(this.OnTestException);
		}

		private void UserSettings_Changed(object sender, SettingsEventArgs args)
		{
			this.WordWrap = this.settings.GetSetting("Gui.ResultTabs.ErrorsTab.WordWrapEnabled", true);
			Font newFont = this.stackTraceDisplay.Font = this.sourceCode.CodeDisplayFont
				= this.settings.GetSetting("Gui.FixedFont", DefaultFixedFont);
			if (newFont != this.detailList.Font)
			{
				this.detailList.Font = newFont;
				this.RefillDetailList();
			}
		}

		private void copyDetailMenuItem_Click(object sender, EventArgs e)
		{
			if (this.detailList.SelectedItem != null)
				Clipboard.SetDataObject(this.detailList.SelectedItem.ToString());
		}

		private void detailList_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index >= 0)
			{
				e.DrawBackground();
				TestResultItem item = (TestResultItem)this.detailList.Items[e.Index];
				bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? true : false;
				Brush brush = selected ? SystemBrushes.HighlightText : SystemBrushes.WindowText;
				RectangleF layoutRect = e.Bounds;
				if (this.WordWrap && layoutRect.Width > this.detailList.ClientSize.Width)
					layoutRect.Width = this.detailList.ClientSize.Width;
				e.Graphics.DrawString(item.ToString(), this.detailList.Font, brush, layoutRect);
			}
		}

		private void detailList_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			TestResultItem item = (TestResultItem)this.detailList.Items[e.Index];
			//string s = item.ToString();
			SizeF size = this.WordWrap
				? e.Graphics.MeasureString(item.ToString(), this.detailList.Font, this.detailList.ClientSize.Width)
				: e.Graphics.MeasureString(item.ToString(), this.detailList.Font);
			e.ItemHeight = (int)size.Height;
			e.ItemWidth = (int)size.Width;
		}

		private void detailList_MouseLeave(object sender, EventArgs e)
		{
			this.hoverIndex = -1;
			this.ClearTimer();
		}

		private void detailList_MouseMove(object sender, MouseEventArgs e)
		{
			this.ClearTimer();

			this.hoverIndex = this.detailList.IndexFromPoint(e.X, e.Y);

			if (this.hoverIndex >= 0 && this.hoverIndex < this.detailList.Items.Count)
			{
				// Workaround problem of IndexFromPoint returning an
				// index when mouse is over bottom part of list.
				Rectangle r = this.detailList.GetItemRectangle(this.hoverIndex);
				if (e.Y > r.Bottom)
					this.hoverIndex = -1;
				else
				{
					this.hoverTimer = new Timer();
					this.hoverTimer.Interval = 800;
					this.hoverTimer.Tick += new EventHandler(this.OnMouseHover);
					this.hoverTimer.Start();
				}
			}
		}

		private void detailList_Resize(object sender, EventArgs e)
		{
			if (this.WordWrap)
				this.RefillDetailList();
		}

		/// <summary>
		/// When one of the detail failure items is selected, display
		/// the stack trace and set up the tool tip for that item.
		/// </summary>
		private void detailList_SelectedIndexChanged(object sender, EventArgs e)
		{
			TestResultItem resultItem = (TestResultItem)this.detailList.SelectedItem;
			this.errorBrowser.StackTraceSource = resultItem.StackTrace;
			this.detailList.ContextMenu = this.detailListContextMenu;
		}

		private void errorBrowser_StackTraceDisplayChanged(object sender, EventArgs e)
		{
			this.settings.SaveSetting("Gui.ResultTabs.ErrorBrowser.SourceCodeDisplay",
				this.errorBrowser.SelectedDisplay == this.sourceCode);
		}

		private void sourceCode_SplitOrientationChanged(object sender, EventArgs e)
		{
			this.settings.SaveSetting("Gui.ResultTabs.ErrorBrowser.SplitterOrientation", this.sourceCode.SplitOrientation);

			string distanceSetting = this.sourceCode.SplitOrientation == Orientation.Vertical
				? "Gui.ResultTabs.ErrorBrowser.VerticalPosition" : "Gui.ResultTabs.ErrorBrowser.HorizontalPosition";
			this.sourceCode.SplitterDistance = this.settings.GetSetting(distanceSetting, 0.3f);
		}

		private void sourceCode_SplitterDistanceChanged(object sender, EventArgs e)
		{
			string distanceSetting = this.sourceCode.SplitOrientation == Orientation.Vertical
				? "Gui.ResultTabs.ErrorBrowser.VerticalPosition" : "Gui.ResultTabs.ErrorBrowser.HorizontalPosition";
			this.settings.SaveSetting(distanceSetting, this.sourceCode.SplitterDistance);
		}

		private void tabSplitter_SplitterMoved(object sender, SplitterEventArgs e)
		{
			this.settings.SaveSetting("Gui.ResultTabs.ErrorsTabSplitterPosition", this.tabSplitter.SplitPosition);
		}

		private void tipWindow_Closed(object sender, EventArgs e)
		{
			this.tipWindow = null;
			this.hoverIndex = -1;
			this.ClearTimer();
		}

		#endregion
	}
}