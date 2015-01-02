/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using TextMetal.Common.WinForms.Controls;

namespace TextMetal.Common.WinForms.Forms
{
	public partial class PropertyForm
	{
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
			this.pgShape = new TextMetal.Common.WinForms.Controls.TmPropertyGrid();
			this.SuspendLayout();
			// 
			// pgShape
			// 
			this.pgShape.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgShape.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.pgShape.Location = new System.Drawing.Point(0, 0);
			this.pgShape.Name = "pgShape";
			this.pgShape.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.pgShape.Size = new System.Drawing.Size(200, 328);
			this.pgShape.TabIndex = 0;
			this.pgShape.ToolbarVisible = false;
			this.pgShape.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgShape_PropertyValueChanged);
			// 
			// PropertyForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(200, 328);
			this.Controls.Add(this.pgShape);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "PropertyForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.ResumeLayout(false);

		}


		private Container components = null;
		private TmPropertyGrid pgShape;
	}
}