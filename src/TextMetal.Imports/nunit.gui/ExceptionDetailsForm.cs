// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NUnit.Gui
{
	/// <summary>
	/// Summary description for ExceptionDetailsForm.
	/// </summary>
	public class ExceptionDetailsForm : Form
	{
		#region Constructors/Destructors

		public ExceptionDetailsForm(Exception exception)
		{
			//
			// Required for Windows Form Designer support
			//
			this.InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.exception = exception;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private Exception exception;
		private Label message;
		private Button okButton;
		private RichTextBox stackTrace;

		#endregion

		#region Methods/Operators

		private void AppendStackTrace(StringBuilder sb, Exception ex)
		{
			if (ex.InnerException != null)
				this.AppendStackTrace(sb, ex.InnerException);

			sb.Append(ex.GetType().ToString());
			sb.Append("...\r\n");
			sb.Append(ex.StackTrace);
			sb.Append("\r\n\r\n");
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

		private void ExceptionDetailsForm_Load(object sender, EventArgs e)
		{
			//TODO: Put some of this in the constructor for easier testing?
			this.message.Text = this.FormatMessage(this.exception);
			this.SetMessageLabelSize();

			this.stackTrace.Text = this.FormatStackTrace(this.exception);
		}

		private void ExceptionDetailsForm_Resize(object sender, EventArgs e)
		{
			this.SetMessageLabelSize();
		}

		private string FormatMessage(Exception exception)
		{
			StringBuilder sb = new StringBuilder();

			for (Exception ex = exception; ex != null; ex = ex.InnerException)
			{
				if (ex != exception)
					sb.Append("\r\n----> ");
				sb.Append(ex.GetType().ToString());
				sb.Append(": ");
				sb.Append(ex.Message);
			}

			return sb.ToString();
		}

		private string FormatStackTrace(Exception exception)
		{
			StringBuilder sb = new StringBuilder();
			this.AppendStackTrace(sb, exception);

			return sb.ToString();
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.okButton = new Button();
			this.stackTrace = new RichTextBox();
			this.message = new Label();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Anchor = AnchorStyles.Bottom;
			this.okButton.DialogResult = DialogResult.Cancel;
			this.okButton.Location = new Point(434, 512);
			this.okButton.Name = "okButton";
			this.okButton.Size = new Size(90, 27);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			// 
			// stackTrace
			// 
			this.stackTrace.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
														| AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.stackTrace.Location = new Point(10, 65);
			this.stackTrace.Name = "stackTrace";
			this.stackTrace.ReadOnly = true;
			this.stackTrace.Size = new Size(940, 433);
			this.stackTrace.TabIndex = 3;
			this.stackTrace.Text = "";
			// 
			// message
			// 
			this.message.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
													| AnchorStyles.Right)));
			this.message.Location = new Point(19, 9);
			this.message.Name = "message";
			this.message.Size = new Size(931, 46);
			this.message.TabIndex = 2;
			// 
			// ExceptionDetailsForm
			// 
			this.ClientSize = new Size(969, 551);
			this.Controls.Add(this.stackTrace);
			this.Controls.Add(this.message);
			this.Controls.Add(this.okButton);
			this.Name = "ExceptionDetailsForm";
			this.Text = "Exception Details";
			this.Resize += new EventHandler(this.ExceptionDetailsForm_Resize);
			this.Load += new EventHandler(this.ExceptionDetailsForm_Load);
			this.ResumeLayout(false);
		}

		private void SetMessageLabelSize()
		{
			Rectangle rect = this.message.ClientRectangle;
			Graphics g = Graphics.FromHwnd(this.Handle);
			SizeF sizeNeeded = g.MeasureString(this.message.Text, this.message.Font, rect.Width);
			int delta = sizeNeeded.ToSize().Height - rect.Height;

			this.message.Height += delta;
			this.stackTrace.Top += delta;
			this.stackTrace.Height -= delta;
		}

		#endregion
	}
}