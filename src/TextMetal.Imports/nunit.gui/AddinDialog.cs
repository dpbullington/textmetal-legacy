// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using NUnit.Core.Extensibility;
using NUnit.Util;

namespace NUnit.Gui
{
	/// <summary>
	/// 	Summary description for AddinDialog.
	/// </summary>
	public class AddinDialog : Form
	{
		#region Constructors/Destructors

		public AddinDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			this.InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		#endregion

		#region Fields/Constants

		private ListView addinListView;
		private ColumnHeader addinNameColumn;
		private ColumnHeader addinStatusColumn;
		private IList addins;
		private Button button1;

		/// <summary>
		/// 	Required designer variable.
		/// </summary>
		private Container components = null;

		private TextBox descriptionTextBox;
		private Label label1;

		private Label label2;
		private TextBox messageTextBox;

		#endregion

		#region Methods/Operators

		private void AddinDialog_Load(object sender, EventArgs e)
		{
			this.addins = Services.AddinRegistry.Addins;

			foreach (Addin addin in this.addins)
			{
				ListViewItem item = new ListViewItem(
					new string[] { addin.Name, addin.Status.ToString() });
				this.addinListView.Items.Add(item);
			}

			if (this.addinListView.Items.Count > 0)
				this.addinListView.Items[0].Selected = true;

			this.AutoSizeFirstColumnOfListView();
		}

		private void AutoSizeFirstColumnOfListView()
		{
			int width = this.addinListView.ClientSize.Width;
			for (int i = 1; i < this.addinListView.Columns.Count; i++)
				width -= this.addinListView.Columns[i].Width;
			this.addinListView.Columns[0].Width = width;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AddinDialog));
			this.addinListView = new System.Windows.Forms.ListView();
			this.addinNameColumn = new System.Windows.Forms.ColumnHeader();
			this.addinStatusColumn = new System.Windows.Forms.ColumnHeader();
			this.descriptionTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.messageTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// addinListView
			// 
			this.addinListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			                                                                   | System.Windows.Forms.AnchorStyles.Left)
			                                                                  | System.Windows.Forms.AnchorStyles.Right)));
			this.addinListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
			                                    {
				                                    this.addinNameColumn,
				                                    this.addinStatusColumn
			                                    });
			this.addinListView.FullRowSelect = true;
			this.addinListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.addinListView.Location = new System.Drawing.Point(8, 8);
			this.addinListView.MultiSelect = false;
			this.addinListView.Name = "addinListView";
			this.addinListView.Size = new System.Drawing.Size(448, 136);
			this.addinListView.TabIndex = 0;
			this.addinListView.View = System.Windows.Forms.View.Details;
			this.addinListView.Resize += new System.EventHandler(this.addinListView_Resize);
			this.addinListView.SelectedIndexChanged += new System.EventHandler(this.addinListView_SelectedIndexChanged);
			// 
			// addinNameColumn
			// 
			this.addinNameColumn.Text = "Addin";
			this.addinNameColumn.Width = 352;
			// 
			// addinStatusColumn
			// 
			this.addinStatusColumn.Text = "Status";
			this.addinStatusColumn.Width = 89;
			// 
			// descriptionTextBox
			// 
			this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			                                                                       | System.Windows.Forms.AnchorStyles.Right)));
			this.descriptionTextBox.Location = new System.Drawing.Point(8, 184);
			this.descriptionTextBox.Multiline = true;
			this.descriptionTextBox.Name = "descriptionTextBox";
			this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.descriptionTextBox.Size = new System.Drawing.Size(448, 56);
			this.descriptionTextBox.TabIndex = 1;
			this.descriptionTextBox.Text = "";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(8, 160);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(304, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Description:";
			// 
			// button1
			// 
			this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.button1.Location = new System.Drawing.Point(192, 344);
			this.button1.Name = "button1";
			this.button1.TabIndex = 3;
			this.button1.Text = "OK";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(8, 256);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(304, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = " Message:";
			// 
			// messageTextBox
			// 
			this.messageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			                                                                   | System.Windows.Forms.AnchorStyles.Right)));
			this.messageTextBox.Location = new System.Drawing.Point(8, 280);
			this.messageTextBox.Multiline = true;
			this.messageTextBox.Name = "messageTextBox";
			this.messageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.messageTextBox.Size = new System.Drawing.Size(448, 56);
			this.messageTextBox.TabIndex = 4;
			this.messageTextBox.Text = "";
			// 
			// AddinDialog
			// 
			this.ClientSize = new System.Drawing.Size(464, 376);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.messageTextBox);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.descriptionTextBox);
			this.Controls.Add(this.addinListView);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AddinDialog";
			this.ShowInTaskbar = false;
			this.Text = "Registered Addins";
			this.Load += new System.EventHandler(this.AddinDialog_Load);
			this.ResumeLayout(false);
		}

		private void addinListView_Resize(object sender, EventArgs e)
		{
			this.AutoSizeFirstColumnOfListView();
		}

		private void addinListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.addinListView.SelectedIndices.Count > 0)
			{
				int index = this.addinListView.SelectedIndices[0];
				Addin addin = (Addin)this.addins[index];
				this.descriptionTextBox.Text = addin.Description;
				this.messageTextBox.Text = addin.Message;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		#endregion
	}
}