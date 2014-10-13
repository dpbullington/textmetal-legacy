/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using TextMetal.Common.Core;
using TextMetal.Common.WinForms;
using TextMetal.Common.WinForms.Controls;
using TextMetal.Common.WinForms.Forms;

namespace TextMetal.HostImpl.WindowsTool.Forms
{
	public partial class MainForm : TmForm
	{
		#region Constructors/Destructors

		public MainForm()
		{
			this.InitializeComponent();
			this.StatusText = string.Empty;
		}

		#endregion

		#region Fields/Constants

		private readonly IList<DocumentForm> documentForms = new List<DocumentForm>();

		#endregion

		#region Properties/Indexers/Events

		public IList<DocumentForm> DocumentForms
		{
			get
			{
				return this.documentForms;
			}
		}

		public bool HasAnyDocuments
		{
			get
			{
				return this.DocumentForms.Count > 0;
			}
		}

		public string StatusText
		{
			get
			{
				return this.tsslMain.Text;
			}
			set
			{
				this.tsslMain.Text = value;
			}
		}

		#endregion

		#region Methods/Operators

		private void AboutBox()
		{
			using (AboutForm aboutForm = new AboutForm())
				aboutForm.ShowDialog(this);
		}

		private void CloseAllDocuments()
		{
			bool cancel;

			this.CloseAllDocuments(out cancel, "close all documents");
		}

		private void CloseAllDocuments(out bool cancel, string verb)
		{
			DialogResult dialogResult;
			List<DocumentForm> temp;
			int dirtyCount;
			cancel = false;

			temp = new List<DocumentForm>(this.DocumentForms); // required
			dirtyCount = temp.Count(e => e.CoreIsDirty);

			if (dirtyCount > 0)
			{
				dialogResult = MessageBox.Show(this, string.Format("Do you want {1} without saving the {0} modified document(s)?", dirtyCount, verb), ExecutableApplication.Current.AssemblyInformation.Product, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				
				if (dialogResult == DialogResult.Cancel)
				{
					cancel = true;
				}
				else if (dialogResult == DialogResult.No)
				{
					cancel = false;

					foreach (DocumentForm documentForm in temp)
					{
						if (documentForm.CoreIsDirty)
							documentForm.SaveDocument(false);

						documentForm.Close(); // direct
					}
				}
				else // YES
				{
					cancel = false;
				}
			}
		}

		protected override void CoreQuit(out bool cancel)
		{
			base.CoreQuit(out cancel);

			if (cancel)
				return;

			this.CloseAllDocuments(out cancel, "quit");
		}

		protected override void CoreSetup()
		{
			const string RESOURCE_NAME = "TextMetal.HostImpl.WindowsTool.Images.SplashScreen.png";
			Stream stream;
			Image image;

			base.CoreSetup();

			this.CoreText = string.Format("{0} Studio", ExecutableApplication.Current.AssemblyInformation.Product);

			stream = this.GetType().Assembly.GetManifestResourceStream("TextMetal.HostImpl.WindowsTool.Images.SplashScreen.png");

			if ((object)stream == null)
				throw new InvalidOperationException(string.Format("Manifest resource name '{0}' was not found in assembly '{1}'.", RESOURCE_NAME, this.GetType().Assembly));

			image = Image.FromStream(stream);

			this.BackgroundImage = image;

			// DO NOT DISPOSE (owner cleans up)
		}

		protected override void CoreShown()
		{
			base.CoreShown();
			
			this.RefreshControlState();
		}

		private void HelpTopics()
		{
			MessageBox.Show(this, "Help is not available in this release.", ExecutableApplication.Current.AssemblyInformation.Product, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		private void NewDocument()
		{
			this.ShowDocument(null);
		}

		private void OpenDocument()
		{
			string filePath;

			if (!this.TryGetFilePath(out filePath))
				return;

			this.ShowDocument(filePath);
		}

		private void RefreshControlState()
		{
			this.tsmiNewDocument.Enabled = true;
			this.tsmiOpenDocument.Enabled = true;
			this.tsmiCloseAllDocuments.Enabled = this.HasAnyDocuments;
			this.tsmiDocumentWindows.Enabled = this.HasAnyDocuments;
		}

		private void ShowDocument(string documentFilePath)
		{
			DocumentForm documentForm;

			documentForm = new DocumentForm();
			documentForm.DocumentFilePath = documentFilePath;

			documentForm.Load += this.documentForm_Load;
			documentForm.TextChanged += this.documentForm_TextChanged;
			documentForm.Closed += this.documentForm_Closed;
			documentForm.Show();
		}

		private bool TryGetFilePath(out string filePath)
		{
			DialogResult dialogResult;

			this.ofdMain.FileName = filePath = null;
			dialogResult = this.ofdMain.ShowDialog(this);

			if (dialogResult != DialogResult.OK ||
				DataType.IsNullOrWhiteSpace(this.ofdMain.FileName))
				return false;

			filePath = Path.GetFullPath(this.ofdMain.FileName);
			return true;
		}

		private void documentForm_Closed(object sender, EventArgs e)
		{
			DocumentForm documentForm;
			ToolStripMenuItem tsmiWindow;

			documentForm = (DocumentForm)sender;
			documentForm.Closed -= this.documentForm_Closed;
			documentForm.Load -= this.documentForm_Load;
			documentForm.TextChanged -= this.documentForm_TextChanged;

			tsmiWindow = this.tsmiDocumentWindows.DropDownItems.Cast<ToolStripMenuItem>().SingleOrDefault(tsmi => (object)tsmi.Tag == documentForm);

			if ((object)tsmiWindow == null)
				throw new InvalidOperationException(string.Format("ToolStripMenuItem was null."));

			tsmiWindow.Tag = null;
			tsmiWindow.Click -= this.tsmiWindow_Click;
			this.tsmiDocumentWindows.DropDownItems.Remove(tsmiWindow);
			tsmiWindow.Dispose();

			this.DocumentForms.Remove(documentForm);
			documentForm.Dispose();

			this.RefreshControlState();
			this.Activate();
		}

		private void documentForm_Load(object sender, EventArgs e)
		{
			DocumentForm documentForm;
			ToolStripMenuItem tsmiWindow;

			documentForm = (DocumentForm)sender;

			this.DocumentForms.Add(documentForm);

			tsmiWindow = new ToolStripMenuItem();
			tsmiWindow.Text = documentForm.Text;
			tsmiWindow.Tag = documentForm;
			tsmiWindow.Click += this.tsmiWindow_Click;
			this.tsmiDocumentWindows.DropDownItems.Add(tsmiWindow);

			this.RefreshControlState();
		}

		private void documentForm_TextChanged(object sender, EventArgs e)
		{
			DocumentForm documentForm;
			ToolStripMenuItem tsmiWindow;

			documentForm = (DocumentForm)sender;

			tsmiWindow = this.tsmiDocumentWindows.DropDownItems.Cast<ToolStripMenuItem>().SingleOrDefault(tsmi => (object)tsmi.Tag == documentForm);

			if ((object)tsmiWindow == null)
				throw new InvalidOperationException();

			tsmiWindow.Text = documentForm.Text;
		}

		private void tsmiAbout_Click(object sender, EventArgs e)
		{
			this.AboutBox();
		}

		private void tsmiCloseAllDocuments_Click(object sender, EventArgs e)
		{
			this.CloseAllDocuments();
		}

		private void tsmiExit_Click(object sender, EventArgs e)
		{
			this.Close(); // direct
		}

		private void tsmiNewDocument_Click(object sender, EventArgs e)
		{
			this.NewDocument();
		}

		private void tsmiOpenDocument_Click(object sender, EventArgs e)
		{
			this.OpenDocument();
		}

		private void tsmiTopics_Click(object sender, EventArgs e)
		{
			this.HelpTopics();
		}

		private void tsmiWindow_Click(object sender, EventArgs e)
		{
			DocumentForm documentForm;
			ToolStripMenuItem tsmiWindow;

			tsmiWindow = (ToolStripMenuItem)sender;
			documentForm = (DocumentForm)tsmiWindow.Tag;

			documentForm.BringToFront();
		}

		#endregion
	}
}