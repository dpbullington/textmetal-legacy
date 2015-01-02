/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Common.WinForms.Forms
{
	public partial class TmForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.ttMain = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// ttMain
			// 
			this.ttMain.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Error;
			// 
			// TmForm
			// 
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Name = "TmForm";
			this.ResumeLayout(false);

		}

		private System.Windows.Forms.ToolTip ttMain;

		#endregion
	}
}