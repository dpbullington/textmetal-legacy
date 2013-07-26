// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Core;

namespace NUnit.UiKit
{
	public partial class TestPropertiesDialog : Form
	{
		#region Constructors/Destructors

		public TestPropertiesDialog(TestSuiteTreeNode node)
		{
			this.InitializeComponent();

			this.node = node;
		}

		#endregion

		#region Fields/Constants

		private int clientWidth;

		private int maxY;
		private int nextY;
		private TestSuiteTreeNode node;

		private Image pinnedImage;
		private TestResult result;
		private ITest test;
		private Image unpinnedImage;

		#endregion

		#region Properties/Indexers/Events

		[Browsable(false)]
		public bool Pinned
		{
			get
			{
				return this.pinButton.Checked;
			}
			set
			{
				this.pinButton.Checked = value;
			}
		}

		#endregion

		#region Methods/Operators

		private void BeginPanel()
		{
			this.maxY = 20;
			this.nextY = 24;
		}

		private void CreateRow(params Control[] controls)
		{
			this.nextY = this.maxY + 4;

			foreach (Control control in controls)
				this.InsertInRow(control);
		}

		public void DisplayProperties()
		{
			this.DisplayProperties(this.node);
		}

		public void DisplayProperties(TestSuiteTreeNode node)
		{
			this.node = node;
			this.test = node.Test;
			this.result = node.Result;

			this.SetTitleBarText();

			this.testResult.Text = node.StatusText;
			this.testResult.Font = new Font(this.Font, FontStyle.Bold);
			if (node.TestType == "Project" || node.TestType == "Assembly")
				this.testName.Text = Path.GetFileName(this.test.TestName.Name);
			else
				this.testName.Text = this.test.TestName.Name;

			this.testType.Text = node.TestType;
			this.fullName.Text = this.test.TestName.FullName;
			this.description.Text = this.test.Description;

			StringBuilder sb1 = new StringBuilder();
			foreach (string cat in this.test.Categories)
			{
				if (sb1.Length > 0)
				{
					sb1.Append(", ");
					sb1.Append(cat);
				}
			}
			this.categories.Text = sb1.ToString();

			this.testCaseCount.Text = this.test.TestCount.ToString();

			switch (this.test.RunState)
			{
				case RunState.Explicit:
					this.shouldRun.Text = "Explicit";
					break;
				case RunState.Runnable:
					this.shouldRun.Text = "Yes";
					break;
				default:
					this.shouldRun.Text = "No";
					break;
			}
			this.ignoreReason.Text = this.test.IgnoreReason;

			this.FillPropertyList();

			this.elapsedTime.Text = "Execution Time:";
			this.assertCount.Text = "Assert Count:";
			this.message.Text = "";
			this.stackTrace.Text = "";

			if (this.result != null)
			{
				this.elapsedTime.Text = string.Format("Execution Time: {0}", this.result.Time);

				this.assertCount.Text = string.Format("Assert Count: {0}", this.result.AssertCount);
				// message may have a leading blank line
				// TODO: take care of this in label?
				if (this.result.Message != null)
				{
					if (this.result.Message.Length > 64000)
						this.message.Text = this.TrimLeadingBlankLines(this.result.Message.Substring(0, 64000));
					else
						this.message.Text = this.TrimLeadingBlankLines(this.result.Message);
				}

				this.stackTrace.Text = this.result.StackTrace;
			}

			this.BeginPanel();

			this.CreateRow(this.testTypeLabel, this.testType);
			this.CreateRow(this.fullNameLabel, this.fullName);
			this.CreateRow(this.descriptionLabel, this.description);
			this.CreateRow(this.categoriesLabel, this.categories);
			this.CreateRow(this.testCaseCountLabel, this.testCaseCount, this.shouldRunLabel, this.shouldRun);
			this.CreateRow(this.ignoreReasonLabel, this.ignoreReason);
			this.CreateRow(this.propertiesLabel, this.properties);
			this.CreateRow(this.hiddenProperties);

			this.groupBox1.ClientSize = new Size(
				this.groupBox1.ClientSize.Width, this.maxY + 12);

			this.groupBox2.Location = new Point(
				this.groupBox1.Location.X, this.groupBox1.Bottom + 12);

			this.BeginPanel();

			this.CreateRow(this.elapsedTime, this.assertCount);
			this.CreateRow(this.messageLabel, this.message);
			this.CreateRow(this.stackTraceLabel, this.stackTrace);

			this.groupBox2.ClientSize = new Size(
				this.groupBox2.ClientSize.Width, this.maxY + 12);

			this.ClientSize = new Size(
				this.ClientSize.Width, this.groupBox2.Bottom + 12);
		}

		private void FillPropertyList()
		{
			this.properties.Items.Clear();
			foreach (DictionaryEntry entry in this.test.Properties)
			{
				if (this.hiddenProperties.Checked || !entry.Key.ToString().StartsWith("_"))
				{
					if (entry.Value is ICollection)
					{
						ICollection items = (ICollection)entry.Value;
						if (items.Count == 0)
							continue;

						StringBuilder sb = new StringBuilder();
						foreach (object item in items)
						{
							if (sb.Length > 0)
								sb.Append(",");
							sb.Append(item.ToString());
						}

						this.properties.Items.Add(entry.Key.ToString() + "=" + sb.ToString());
					}
					else
						this.properties.Items.Add(entry.Key.ToString() + "=" + entry.Value.ToString());
				}
			}
		}

		private void InsertInRow(Control control)
		{
			Label label = control as Label;
			if (label != null)
				this.SizeToFitText(label);

			control.Location = new Point(control.Location.X, this.nextY);
			this.maxY = Math.Max(this.maxY, control.Bottom);
		}

		private void OnSelectedNodeChanged(object sender, TreeViewEventArgs e)
		{
			if (this.pinButton.Checked)
				this.DisplayProperties((TestSuiteTreeNode)e.Node);
			else
				this.Close();
		}

		protected override bool ProcessKeyPreview(ref Message m)
		{
			const int ESCAPE = 27;
			const int WM_CHAR = 258;

			if (m.Msg == WM_CHAR && m.WParam.ToInt32() == ESCAPE)
			{
				this.Close();
				return true;
			}

			return base.ProcessKeyEventArgs(ref m);
		}

		private void SetTitleBarText()
		{
			string name = this.test.TestName.Name;
			int index = name.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
			if (index >= 0)
				name = name.Substring(index + 1);
			this.Text = string.Format("{0} Properties - {1}", this.node.TestType, name);
		}

		private void SizeToFitText(Label label)
		{
			string text = label.Text;
			if (text == "")
				text = "Ay"; // Include descender to be sure of size

			Graphics g = Graphics.FromHwnd(label.Handle);
			SizeF size = g.MeasureString(text, label.Font, label.Parent.ClientSize.Width - label.Left - 8);
			label.ClientSize = new Size(
				(int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
		}

		private void TestPropertiesDialog_Load(object sender, EventArgs e)
		{
			this.pinnedImage = new Bitmap(typeof(TestPropertiesDialog), "Images.pinned.gif");
			this.unpinnedImage = new Bitmap(typeof(TestPropertiesDialog), "Images.unpinned.gif");
			this.pinButton.Image = this.unpinnedImage;

			if (!this.DesignMode)
				this.DisplayProperties();

			this.node.TreeView.AfterSelect += new TreeViewEventHandler(this.OnSelectedNodeChanged);
		}

		private void TestPropertiesDialog_ResizeEnd(object sender, EventArgs e)
		{
			this.ClientSize = new Size(
				this.ClientSize.Width, this.groupBox2.Bottom + 12);

			this.clientWidth = this.ClientSize.Width;
		}

		private void TestPropertiesDialog_SizeChanged(object sender, EventArgs e)
		{
			if (this.clientWidth != this.ClientSize.Width)
			{
				if (this.node != null)
					this.DisplayProperties();
				this.clientWidth = this.ClientSize.Width;
			}
		}

		private string TrimLeadingBlankLines(string s)
		{
			if (s == null)
				return s;

			int start = 0;
			for (int i = 0; i < s.Length; i++)
			{
				switch (s[i])
				{
					case ' ':
					case '\t':
						break;
					case '\r':
					case '\n':
						start = i + 1;
						break;

					default:
						goto getout;
				}
			}

			getout:
			return start == 0 ? s : s.Substring(start);
		}

		private void hiddenProperties_CheckedChanged(object sender, EventArgs e)
		{
			this.FillPropertyList();
		}

		private void pinButton_Click(object sender, EventArgs e)
		{
			if (this.pinButton.Checked)
				this.pinButton.Image = this.pinnedImage;
			else
				this.pinButton.Image = this.unpinnedImage;
		}

		#endregion
	}
}