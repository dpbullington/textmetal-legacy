// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NUnit.UiKit
{
	public class ScrollingTextDisplayForm : Form
	{
		#region Constructors/Destructors

		public ScrollingTextDisplayForm()
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

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private Label message;
		private Button okButton;
		private RichTextBox textBox;

		#endregion

		#region Properties/Indexers/Events

		protected Label Message
		{
			get
			{
				return this.message;
			}
		}

		protected RichTextBox TextBox
		{
			get
			{
				return this.textBox;
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

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.okButton = new Button();
			this.textBox = new RichTextBox();
			this.message = new Label();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Anchor = AnchorStyles.Bottom;
			this.okButton.DialogResult = DialogResult.Cancel;
			this.okButton.Location = new Point(284, 441);
			this.okButton.Name = "okButton";
			this.okButton.Size = new Size(90, 27);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "Close";
			// 
			// textBox
			// 
			this.textBox.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
													| AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.textBox.Location = new Point(10, 65);
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.Size = new Size(640, 362);
			this.textBox.TabIndex = 3;
			this.textBox.Text = "";
			// 
			// message
			// 
			this.message.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.message.Location = new Point(19, 9);
			this.message.Name = "message";
			this.message.Size = new Size(631, 46);
			this.message.TabIndex = 2;
			this.message.Text = "";
			// 
			// TestAssemblyInfoForm
			// 
			this.ClientSize = new Size(669, 480);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.message);
			this.Controls.Add(this.okButton);
			this.Name = "ScrollingTextDisplayForm";
			this.Text = "NUnit";
			this.Resize += new EventHandler(this.ScrollingTextDisplayForm_Resize);
			this.ResumeLayout(false);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.SetMessageLabelSize();
		}

		private void ScrollingTextDisplayForm_Resize(object sender, EventArgs e)
		{
			this.SetMessageLabelSize();
		}

		private void SetMessageLabelSize()
		{
			Rectangle rect = this.message.ClientRectangle;
			Graphics g = Graphics.FromHwnd(this.Handle);
			SizeF sizeNeeded = g.MeasureString(this.message.Text, this.message.Font, rect.Width);
			int delta = sizeNeeded.ToSize().Height - rect.Height;

			this.message.Height += delta;
			this.textBox.Top += delta;
			this.textBox.Height -= delta;
		}

		#endregion
	}
}