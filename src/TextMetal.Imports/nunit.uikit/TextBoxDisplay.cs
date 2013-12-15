// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using NUnit.Core;
using NUnit.Util;

namespace NUnit.UiKit
{
	/// <summary>
	/// TextBoxDisplay is an adapter that allows accessing a
	/// System.Windows.Forms.TextBox using the TextDisplay interface.
	/// </summary>
	public class TextBoxDisplay : RichTextBox, TextDisplay, TestObserver
	{
		#region Constructors/Destructors

		public TextBoxDisplay()
		{
			this.Multiline = true;
			this.ReadOnly = true;
			this.WordWrap = false;

			this.ContextMenu = new ContextMenu();
			this.copyMenuItem = new MenuItem("&Copy", new EventHandler(this.copyMenuItem_Click));
			this.selectAllMenuItem = new MenuItem("Select &All", new EventHandler(this.selectAllMenuItem_Click));
			this.wordWrapMenuItem = new MenuItem("&Word Wrap", new EventHandler(this.wordWrapMenuItem_Click));
			this.fontMenuItem = new MenuItem("Font");
			this.increaseFontMenuItem = new MenuItem("Increase", new EventHandler(this.increaseFontMenuItem_Click));
			this.decreaseFontMenuItem = new MenuItem("Decrease", new EventHandler(this.decreaseFontMenuItem_Click));
			this.restoreFontMenuItem = new MenuItem("Restore", new EventHandler(this.restoreFontMenuItem_Click));
			this.fontMenuItem.MenuItems.AddRange(new MenuItem[] { this.increaseFontMenuItem, this.decreaseFontMenuItem, new MenuItem("-"), this.restoreFontMenuItem });
			this.ContextMenu.MenuItems.AddRange(new MenuItem[] { this.copyMenuItem, this.selectAllMenuItem, this.wordWrapMenuItem, this.fontMenuItem });
			this.ContextMenu.Popup += new EventHandler(this.ContextMenu_Popup);
		}

		#endregion

		#region Fields/Constants

		private TextDisplayContent content;

		private MenuItem copyMenuItem;
		private MenuItem decreaseFontMenuItem;
		private MenuItem fontMenuItem;
		private MenuItem increaseFontMenuItem;
		private string pendingTestCaseLabel = null;
		private MenuItem restoreFontMenuItem;
		private MenuItem selectAllMenuItem;
		private MenuItem wordWrapMenuItem;

		#endregion

		#region Properties/Indexers/Events

		public TextDisplayContent Content
		{
			get
			{
				return this.content;
			}
			set
			{
				this.content = value;
			}
		}

		#endregion

		#region Methods/Operators

		private void ContextMenu_Popup(object sender, EventArgs e)
		{
			this.copyMenuItem.Enabled = this.SelectedText != "";
			this.selectAllMenuItem.Enabled = this.TextLength > 0;
		}

		public string GetText()
		{
			return this.Text;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			// Do nothing - this control uses it's own font
		}

		private void OnTestOutput(object sender, TestEventArgs e)
		{
			if (this.ShouldInclude(e.TestOutput.Type))
			{
				if (this.pendingTestCaseLabel != null)
				{
					this.WriteLine(this.pendingTestCaseLabel);
					this.pendingTestCaseLabel = null;
				}

				Write(e.TestOutput.Text);
			}
		}

		// TODO: We determine whether to include output
		// based solely on the output type. This works
		// well for everything but logging. Because we
		// are unable - at this stage of processing - 
		// to determine the logging level of the output
		// all tabs displaying log output will show
		// output at the most verbose level specified
		// on any of the tabs. Since it's not likely
		// that anyone will display logging on multiple
		// tabs, this is not seen as a serious issue.
		// It may be resolved in a future release by
		// limiting the options available when specifying
		// the content of the output displayed.

		private void OnTestStarting(object sender, TestEventArgs args)
		{
			if (this.content.Labels != TestLabelLevel.Off)
			{
				string label = string.Format("***** {0}", args.TestName.FullName);

				if (this.content.Labels == TestLabelLevel.On)
					this.pendingTestCaseLabel = label;
				else
					this.WriteLine(label);
			}
		}

		private bool ShouldInclude(TestOutputType type)
		{
			switch (type)
			{
				default:
				case TestOutputType.Out:
					return this.content.Out;

				case TestOutputType.Error:
					return this.content.Error;

				case TestOutputType.Log:
					return true; // content.LogLevel != LoggingThreshold.Off;

				case TestOutputType.Trace:
					return this.content.Trace;
			}
		}

		public void Subscribe(ITestEvents events)
		{
			events.TestOutput += new TestEventHandler(this.OnTestOutput);
			events.TestStarting += new TestEventHandler(this.OnTestStarting);
		}

		public void Write(string text)
		{
			this.AppendText(text);
		}

		public void Write(TestOutput output)
		{
			Write(output.Text);
		}

		public void WriteLine(string text)
		{
			this.Write(text + Environment.NewLine);
		}

		private void applyFont(Font font)
		{
			this.Font = font;
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
			Services.UserSettings.SaveSetting("Gui.FixedFont",
				converter.ConvertToString(null, CultureInfo.InvariantCulture, font));
		}

		private void copyMenuItem_Click(object sender, EventArgs e)
		{
			this.Copy();
		}

		private void decreaseFontMenuItem_Click(object sender, EventArgs e)
		{
			this.applyFont(new Font(this.Font.FontFamily, this.Font.SizeInPoints / 1.2f, this.Font.Style));
		}

		private void increaseFontMenuItem_Click(object sender, EventArgs e)
		{
			this.applyFont(new Font(this.Font.FontFamily, this.Font.SizeInPoints * 1.2f, this.Font.Style));
		}

		private void restoreFontMenuItem_Click(object sender, EventArgs e)
		{
			this.applyFont(new Font(FontFamily.GenericMonospace, 8.0f));
		}

		private void selectAllMenuItem_Click(object sender, EventArgs e)
		{
			this.SelectAll();
		}

		private void wordWrapMenuItem_Click(object sender, EventArgs e)
		{
			this.WordWrap = this.wordWrapMenuItem.Checked = !this.wordWrapMenuItem.Checked;
		}

		#endregion
	}
}