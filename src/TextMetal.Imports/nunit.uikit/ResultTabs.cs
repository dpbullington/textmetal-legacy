// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NUnit.Core;
using NUnit.Util;

namespace NUnit.UiKit
{
	/// <summary>
	/// 	Summary description for ResultTabs.
	/// </summary>
	public class ResultTabs : UserControl, TestObserver
	{
		#region Constructors/Destructors

		public ResultTabs()
		{
			// This call is required by the Windows.Forms Form Designer.
			this.InitializeComponent();

			this.tabsMenu = new MenuItem();
			this.errorsTabMenuItem = new MenuItem();
			this.notRunTabMenuItem = new MenuItem();
			this.menuSeparator = new MenuItem();
			this.textOutputMenuItem = new MenuItem();

			this.tabsMenu.MergeType = MenuMerge.Add;
			this.tabsMenu.MergeOrder = 1;
			this.tabsMenu.MenuItems.AddRange(
				new MenuItem[]
				{
					this.errorsTabMenuItem,
					this.notRunTabMenuItem,
					this.menuSeparator,
					this.textOutputMenuItem,
				});
			this.tabsMenu.Text = "&Result Tabs";
			this.tabsMenu.Visible = true;

			this.errorsTabMenuItem.Index = 0;
			this.errorsTabMenuItem.Text = "&Errors && Failures";
			this.errorsTabMenuItem.Click += new EventHandler(this.errorsTabMenuItem_Click);

			this.notRunTabMenuItem.Index = 1;
			this.notRunTabMenuItem.Text = "Tests &Not Run";
			this.notRunTabMenuItem.Click += new EventHandler(this.notRunTabMenuItem_Click);

			this.menuSeparator.Index = 2;
			this.menuSeparator.Text = "-";

			this.textOutputMenuItem.Index = 3;
			this.textOutputMenuItem.Text = "Text &Output...";
			this.textOutputMenuItem.Click += new EventHandler(this.textOutputMenuItem_Click);

			this.displayController = new TextDisplayController(this.tabControl);
//			displayController.CreatePages();
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(ResultTabs));

		/// <summary>
		/// 	Required designer variable.
		/// </summary>
		private Container components = null;

		private MenuItem copyDetailMenuItem;

		private TextDisplayController displayController;
		private ErrorDisplay errorDisplay;
		private TabPage errorTab;

		private MenuItem errorsTabMenuItem;
		private MenuItem menuSeparator;
		private TabPage notRunTab;
		private MenuItem notRunTabMenuItem;
		private NotRunTree notRunTree;
		private ISettings settings;
		private TabControl tabControl;
		private MenuItem tabsMenu;
		private MenuItem textOutputMenuItem;
		private bool updating = false;

		#endregion

		#region Properties/Indexers/Events

		public bool IsTracingEnabled
		{
			get
			{
				return this.displayController.IsTracingEnabled;
			}
		}

		public LoggingThreshold MaximumLogLevel
		{
			get
			{
				return this.displayController.MaximumLogLevel;
			}
		}

		public MenuItem TabsMenu
		{
			get
			{
				return this.tabsMenu;
			}
		}

		#endregion

		#region Methods/Operators

		public void Clear()
		{
			this.errorDisplay.Clear();
			this.notRunTree.Nodes.Clear();
			this.displayController.Clear();
		}

		/// <summary>
		/// 	Clean up any resources being used.
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
		/// 	Required method for Designer support - do not modify 
		/// 	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tabControl = new System.Windows.Forms.TabControl();
			this.errorTab = new System.Windows.Forms.TabPage();
			this.errorDisplay = new NUnit.UiKit.ErrorDisplay();
			this.notRunTab = new System.Windows.Forms.TabPage();
			this.notRunTree = new NUnit.UiKit.NotRunTree();
			this.copyDetailMenuItem = new System.Windows.Forms.MenuItem();
			this.tabControl.SuspendLayout();
			this.errorTab.SuspendLayout();
			this.notRunTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.tabControl.Controls.Add(this.errorTab);
			this.tabControl.Controls.Add(this.notRunTab);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(488, 280);
			this.tabControl.TabIndex = 3;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// errorTab
			// 
			this.errorTab.Controls.Add(this.errorDisplay);
			this.errorTab.ForeColor = System.Drawing.SystemColors.ControlText;
			this.errorTab.Location = new System.Drawing.Point(4, 4);
			this.errorTab.Name = "errorTab";
			this.errorTab.Size = new System.Drawing.Size(480, 254);
			this.errorTab.TabIndex = 0;
			this.errorTab.Text = "Errors and Failures";
			// 
			// errorDisplay
			// 
			this.errorDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.errorDisplay.Location = new System.Drawing.Point(0, 0);
			this.errorDisplay.Name = "errorDisplay";
			this.errorDisplay.Size = new System.Drawing.Size(480, 254);
			this.errorDisplay.TabIndex = 0;
			// 
			// notRunTab
			// 
			this.notRunTab.Controls.Add(this.notRunTree);
			this.notRunTab.ForeColor = System.Drawing.SystemColors.ControlText;
			this.notRunTab.Location = new System.Drawing.Point(4, 4);
			this.notRunTab.Name = "notRunTab";
			this.notRunTab.Size = new System.Drawing.Size(480, 254);
			this.notRunTab.TabIndex = 1;
			this.notRunTab.Text = "Tests Not Run";
			this.notRunTab.Visible = false;
			// 
			// notRunTree
			// 
			this.notRunTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.notRunTree.ImageIndex = -1;
			this.notRunTree.Indent = 19;
			this.notRunTree.Location = new System.Drawing.Point(0, 0);
			this.notRunTree.Name = "notRunTree";
			this.notRunTree.SelectedImageIndex = -1;
			this.notRunTree.Size = new System.Drawing.Size(480, 254);
			this.notRunTree.TabIndex = 0;
			// 
			// copyDetailMenuItem
			// 
			this.copyDetailMenuItem.Index = -1;
			this.copyDetailMenuItem.Text = "Copy";
			// 
			// ResultTabs
			// 
			this.Controls.Add(this.tabControl);
			this.Name = "ResultTabs";
			this.Size = new System.Drawing.Size(488, 280);
			this.tabControl.ResumeLayout(false);
			this.errorTab.ResumeLayout(false);
			this.notRunTab.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);

			this.tabControl.ItemSize = new Size(this.tabControl.ItemSize.Width, this.Font.Height + 7);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!this.DesignMode)
			{
				this.settings = Services.UserSettings;
				TextDisplayTabSettings tabSettings = new TextDisplayTabSettings();
				tabSettings.LoadSettings(this.settings);

				this.UpdateTabPages();

				this.Subscribe(Services.TestLoader.Events);
				Services.UserSettings.Changed += new SettingsEventHandler(this.UserSettings_Changed);

				ITestEvents events = Services.TestLoader.Events;
				this.errorDisplay.Subscribe(events);
				this.notRunTree.Subscribe(events);
			}

			base.OnLoad(e);
		}

		private void OnRunStarting(object sender, TestEventArgs args)
		{
			this.Clear();
		}

		private void OnTestLoaded(object sender, TestEventArgs args)
		{
			this.Clear();
		}

		private void OnTestReloaded(object sender, TestEventArgs args)
		{
			if (this.settings.GetSetting("Options.TestLoader.ClearResultsOnReload", false))
				this.Clear();
		}

		private void OnTestUnloaded(object sender, TestEventArgs args)
		{
			this.Clear();
		}

		public void Subscribe(ITestEvents events)
		{
			events.TestLoaded += new TestEventHandler(this.OnTestLoaded);
			events.TestUnloaded += new TestEventHandler(this.OnTestUnloaded);
			events.TestReloaded += new TestEventHandler(this.OnTestReloaded);
			events.RunStarting += new TestEventHandler(this.OnRunStarting);
		}

		private void UpdateTabPages()
		{
			this.errorsTabMenuItem.Checked = this.settings.GetSetting("Gui.ResultTabs.DisplayErrorsTab", true);
			this.notRunTabMenuItem.Checked = this.settings.GetSetting("Gui.ResultTabs.DisplayNotRunTab", true);

			log.Debug("Updating tab pages");
			this.updating = true;

			this.tabControl.TabPages.Clear();

			if (this.errorsTabMenuItem.Checked)
				this.tabControl.TabPages.Add(this.errorTab);
			if (this.notRunTabMenuItem.Checked)
				this.tabControl.TabPages.Add(this.notRunTab);

			this.displayController.UpdatePages();

			this.tabControl.SelectedIndex = this.settings.GetSetting("Gui.ResultTabs.SelectedTab", 0);

			this.updating = false;
		}

		private void UserSettings_Changed(object sender, SettingsEventArgs e)
		{
			if (e.SettingName.StartsWith("Gui.ResultTabs.Display") ||
			    e.SettingName == "Gui.TextOutput.TabList" ||
			    e.SettingName.StartsWith("Gui.TextOut") && e.SettingName.EndsWith("Enabled"))
				this.UpdateTabPages();
		}

		private void errorsTabMenuItem_Click(object sender, EventArgs e)
		{
			this.settings.SaveSetting("Gui.ResultTabs.DisplayErrorsTab", this.errorsTabMenuItem.Checked = !this.errorsTabMenuItem.Checked);
		}

		private void notRunTabMenuItem_Click(object sender, EventArgs e)
		{
			this.settings.SaveSetting("Gui.ResultTabs.DisplayNotRunTab", this.notRunTabMenuItem.Checked = !this.notRunTabMenuItem.Checked);
		}

		private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
		{
			bool selected = e.Index == this.tabControl.SelectedIndex;

			Font font = selected ? new Font(e.Font, FontStyle.Bold) : e.Font;
			Brush backBrush = new SolidBrush(selected ? SystemColors.Control : SystemColors.Window);
			Brush foreBrush = new SolidBrush(SystemColors.ControlText);

			e.Graphics.FillRectangle(backBrush, e.Bounds);
			Rectangle r = e.Bounds;
			r.Y += 3;
			r.Height -= 3;
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			e.Graphics.DrawString(this.tabControl.TabPages[e.Index].Text, font, foreBrush, r, sf);

			foreBrush.Dispose();
			backBrush.Dispose();
			if (selected)
				font.Dispose();
		}

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.updating)
			{
				int index = this.tabControl.SelectedIndex;
				if (index >= 0 && index < this.tabControl.TabCount)
					this.settings.SaveSetting("Gui.ResultTabs.SelectedTab", index);
			}
		}

		private void textOutputMenuItem_Click(object sender, EventArgs e)
		{
			SimpleSettingsDialog.Display(this.FindForm(), new TextOutputSettingsPage("Text Output"));
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class TextDisplayController
		{
			#region Constructors/Destructors

			public TextDisplayController(TabControl tabControl)
			{
				this.tabControl = tabControl;
				Services.UserSettings.Changed += new SettingsEventHandler(this.UserSettings_Changed);
			}

			#endregion

			#region Fields/Constants

			private TabControl tabControl;
			private List<TextDisplayTabPage> tabPages = new List<TextDisplayTabPage>();

			#endregion

			#region Properties/Indexers/Events

			public bool IsTracingEnabled
			{
				get
				{
					foreach (TextDisplayTabPage page in this.tabPages)
					{
						if (page.Display.Content.Trace)
							return true;
					}

					return false;
				}
			}

			public LoggingThreshold MaximumLogLevel
			{
				get
				{
					LoggingThreshold logLevel = LoggingThreshold.Off;

					foreach (TextDisplayTabPage page in this.tabPages)
					{
						LoggingThreshold level = page.Display.Content.LogLevel;
						if (level > logLevel)
							logLevel = level;
					}

					return logLevel;
				}
			}

			#endregion

			#region Methods/Operators

			private static Font GetFixedFont()
			{
				ISettings settings = Services.UserSettings;

				return settings == null
					       ? new Font(FontFamily.GenericMonospace, 8.0f)
					       : settings.GetSetting("Gui.FixedFont", new Font(FontFamily.GenericMonospace, 8.0f));
			}

			public void Clear()
			{
				foreach (TextDisplayTabPage page in this.tabPages)
					page.Display.Clear();
			}

			public void UpdatePages()
			{
				TextDisplayTabSettings tabSettings = new TextDisplayTabSettings();
				tabSettings.LoadSettings();
				List<TextDisplayTabPage> oldPages = this.tabPages;
				this.tabPages = new List<TextDisplayTabPage>();
				Font displayFont = GetFixedFont();

				foreach (TextDisplayTabSettings.TabInfo tabInfo in tabSettings.Tabs)
				{
					if (tabInfo.Enabled)
					{
						TextDisplayTabPage thePage = null;
						foreach (TextDisplayTabPage page in oldPages)
						{
							if (page.Name == tabInfo.Name)
							{
								thePage = page;
								break;
							}
						}

						if (thePage == null)
						{
							thePage = new TextDisplayTabPage(tabInfo);
							thePage.Display.Subscribe(Services.TestLoader.Events);
						}

						thePage.DisplayFont = displayFont;

						this.tabPages.Add(thePage);
						this.tabControl.TabPages.Add(thePage);
					}
				}
			}

			private void UserSettings_Changed(object sender, SettingsEventArgs args)
			{
				string settingName = args.SettingName;
				string prefix = "Gui.TextOutput.";

				if (settingName == "Gui.FixedFont")
				{
					Font displayFont = GetFixedFont();
					foreach (TextDisplayTabPage page in this.tabPages)
						page.DisplayFont = displayFont;
				}
				else if (settingName.StartsWith(prefix))
				{
					string fieldName = settingName.Substring(prefix.Length);
					int dot = fieldName.IndexOf('.');
					if (dot > 0)
					{
						string tabName = fieldName.Substring(0, dot);
						string propName = fieldName.Substring(dot + 1);
						foreach (TextDisplayTabPage page in this.tabPages)
						{
							if (page.Name == tabName)
							{
								switch (propName)
								{
									case "Title":
										page.Text = (string)Services.UserSettings.GetSetting(settingName);
										break;
									case "Content":
										page.Display.Content.LoadSettings(tabName);
										break;
								}
							}
						}
					}
				}
			}

			#endregion
		}

		#endregion
	}
}