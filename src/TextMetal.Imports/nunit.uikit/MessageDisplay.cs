// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Text;
using System.Windows.Forms;

namespace NUnit.UiKit
{
	/// <summary>
	/// 	Summary description for MessageDisplay.
	/// </summary>
	public class MessageDisplay : IMessageDisplay
	{
		#region Constructors/Destructors

		public MessageDisplay()
			: this(DEFAULT_CAPTION)
		{
		}

		public MessageDisplay(string caption)
		{
			this.caption = caption;
		}

		#endregion

		#region Fields/Constants

		private static readonly string DEFAULT_CAPTION = "NUnit";

		private readonly string caption;

		#endregion

		#region Methods/Operators

		private static string BuildMessage(Exception exception)
		{
			Exception ex = exception;
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("{0} : {1}", ex.GetType().ToString(), ex.Message);

			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
				sb.AppendFormat("\r----> {0} : {1}", ex.GetType().ToString(), ex.Message);
			}

			return sb.ToString();
		}

		private static string BuildMessage(string message, Exception exception, bool isFatal)
		{
			string msg = message + Environment.NewLine + Environment.NewLine + BuildMessage(exception);

			return isFatal
				       ? msg
				       : msg + Environment.NewLine + Environment.NewLine + "For further information, use the Exception Details menu item.";
		}

		public DialogResult Ask(string message)
		{
			return this.Ask(message, MessageBoxButtons.YesNo);
		}

		public DialogResult Ask(string message, MessageBoxButtons buttons)
		{
			return MessageBox.Show(message, this.caption, buttons, MessageBoxIcon.Question);
		}

		public DialogResult Display(string message)
		{
			return this.Display(message, MessageBoxButtons.OK);
		}

		public DialogResult Display(string message, MessageBoxButtons buttons)
		{
			return MessageBox.Show(message, this.caption, buttons, MessageBoxIcon.None);
		}

		public DialogResult Error(string message)
		{
			return this.Error(message, MessageBoxButtons.OK);
		}

		public DialogResult Error(string message, MessageBoxButtons buttons)
		{
			return MessageBox.Show(message, this.caption, buttons, MessageBoxIcon.Stop);
		}

		public DialogResult Error(string message, Exception exception)
		{
			return this.Error(message, exception, MessageBoxButtons.OK);
		}

		public DialogResult Error(string message, Exception exception, MessageBoxButtons buttons)
		{
			return Error(BuildMessage(message, exception, false), buttons);
		}

		public DialogResult FatalError(string message, Exception exception)
		{
			return this.Error(BuildMessage(message, exception, true), MessageBoxButtons.OK);
		}

		public DialogResult Info(string message)
		{
			return this.Info(message, MessageBoxButtons.OK);
		}

		public DialogResult Info(string message, MessageBoxButtons buttons)
		{
			return MessageBox.Show(message, this.caption, buttons, MessageBoxIcon.Information);
		}

		#endregion
	}
}