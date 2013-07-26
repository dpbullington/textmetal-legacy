// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Windows.Forms;

using NUnit.UiException.Properties;

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// 	Implements IErrorDisplay to show the actual stack trace in a TextBox control.
	/// </summary>
	public class StackTraceDisplay :
		UserControl,
		IErrorDisplay
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Builds a new instance of StackTraceDisplay.
		/// </summary>
		public StackTraceDisplay()
		{
			this._btnPlugin = ErrorToolbar.NewStripButton(true, "Display actual stack trace", Resources.ImageStackTraceDisplay, null);
			this._btnCopy = ErrorToolbar.NewStripButton(false, "Copy stack trace to clipboard", Resources.ImageCopyToClipboard, this.OnClick);

			this._textContent = new TextBox();
			this._textContent.ReadOnly = true;
			this._textContent.Multiline = true;
			this._textContent.ScrollBars = ScrollBars.Both;

			return;
		}

		#endregion

		#region Fields/Constants

		private ToolStripButton _btnCopy;
		private ToolStripButton _btnPlugin;
		private TextBox _textContent;

		#endregion

		#region Properties/Indexers/Events

		public Control Content
		{
			get
			{
				return (this._textContent);
			}
		}

		public ToolStripItem[] OptionItems
		{
			get
			{
				return (new ToolStripItem[] { this._btnCopy });
			}
		}

		public ToolStripButton PluginItem
		{
			get
			{
				return (this._btnPlugin);
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Copies the actual stack trace to the clipboard.
		/// </summary>
		public void CopyToClipBoard()
		{
			if (String.IsNullOrEmpty(this._textContent.Text))
			{
				Clipboard.Clear();
				return;
			}

			Clipboard.SetText(this._textContent.Text);

			return;
		}

		private void OnClick(object sender, EventArgs args)
		{
			this.CopyToClipBoard();
		}

		protected override void OnFontChanged(EventArgs e)
		{
			this._textContent.Font = this.Font;

			base.OnFontChanged(e);
		}

		public void OnStackTraceChanged(string stackTrace)
		{
			this._textContent.Text = stackTrace;
		}

		#endregion
	}
}