// ****************************************************************
// Copyright 2002-2003, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CP.Windows.Forms
{
	/// <summary>
	/// 	A special type of label which can display a tooltip-like
	/// 	window to show the full extent of any text which doesn't 
	/// 	fit. The window may be placed directly over the label
	/// 	or immediately beneath it and will expand to fit in
	/// 	a horizontal, vertical or both directions as needed.
	/// </summary>
	public class ExpandingLabel : Label
	{
		#region Fields/Constants

		/// <summary>
		/// 	Time in milliseconds that the tip window
		/// 	will remain displayed.
		/// </summary>
		private int autoCloseDelay = 0;

		/// <summary>
		/// 	If true, a context menu with Copy is displayed which
		/// 	allows copying contents to the clipboard.
		/// </summary>
		private bool copySupported = false;

		/// <summary>
		/// 	Direction of expansion
		/// </summary>
		private TipWindow.ExpansionStyle expansion = TipWindow.ExpansionStyle.Horizontal;

		/// <summary>
		/// 	Time in milliseconds that the window stays
		/// 	open after the mouse leaves the control.
		/// </summary>
		private int mouseLeaveDelay = 300;

		/// <summary>
		/// 	True if tipWindow may overlay the label
		/// </summary>
		private bool overlay = true;

		/// <summary>
		/// 	Our window for displaying expanded text
		/// </summary>
		private TipWindow tipWindow;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Time in milliseconds that the tip window
		/// 	will remain displayed.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(0)]
		[Description("Time in milliseconds that the tip is displayed. Zero indicates no automatic timeout.")]
		public int AutoCloseDelay
		{
			get
			{
				return this.autoCloseDelay;
			}
			set
			{
				this.autoCloseDelay = value;
			}
		}

		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("If true, displays a context menu with Copy")]
		public bool CopySupported
		{
			get
			{
				return this.copySupported;
			}
			set
			{
				this.copySupported = value;
				if (this.copySupported)
					base.ContextMenu = null;
			}
		}

		[Browsable(false)]
		public bool Expanded
		{
			get
			{
				return this.tipWindow != null && this.tipWindow.Visible;
			}
		}

		[Category("Behavior")]
		[DefaultValue(TipWindow.ExpansionStyle.Horizontal)]
		public TipWindow.ExpansionStyle Expansion
		{
			get
			{
				return this.expansion;
			}
			set
			{
				this.expansion = value;
			}
		}

		/// <summary>
		/// 	Time in milliseconds that the window stays
		/// 	open after the mouse leaves the control.
		/// 	Reentering the control resets this.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(300)]
		[Description("Time in milliseconds that the tip is displayed after the mouse levaes the control")]
		public int MouseLeaveDelay
		{
			get
			{
				return this.mouseLeaveDelay;
			}
			set
			{
				this.mouseLeaveDelay = value;
			}
		}

		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Indicates whether the tip window should overlay the label")]
		public bool Overlay
		{
			get
			{
				return this.overlay;
			}
			set
			{
				this.overlay = value;
			}
		}

		/// <summary>
		/// 	Override Text property to set up copy menu if
		/// 	the value is non-empty.
		/// </summary>
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;

				if (this.copySupported)
				{
					if (value == null || value == string.Empty)
					{
						if (this.ContextMenu != null)
						{
							this.ContextMenu.Dispose();
							this.ContextMenu = null;
						}
					}
					else
					{
						this.ContextMenu = new ContextMenu();
						MenuItem copyMenuItem = new MenuItem("Copy", new EventHandler(this.CopyToClipboard));
						this.ContextMenu.MenuItems.Add(copyMenuItem);
					}
				}
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Copy contents to clipboard
		/// </summary>
		private void CopyToClipboard(object sender, EventArgs e)
		{
			Clipboard.SetDataObject(this.Text);
		}

		public void Expand()
		{
			if (!this.Expanded)
			{
				this.tipWindow = new TipWindow(this);
				this.tipWindow.Closed += new EventHandler(this.tipWindow_Closed);
				this.tipWindow.Expansion = this.Expansion;
				this.tipWindow.Overlay = this.Overlay;
				this.tipWindow.AutoCloseDelay = this.AutoCloseDelay;
				this.tipWindow.MouseLeaveDelay = this.MouseLeaveDelay;
				this.tipWindow.WantClicks = this.CopySupported;
				this.tipWindow.Show();
			}
		}

		protected override void OnMouseHover(EventArgs e)
		{
			Graphics g = Graphics.FromHwnd(this.Handle);
			SizeF sizeNeeded = g.MeasureString(this.Text, this.Font);
			bool expansionNeeded =
				this.Width < (int)sizeNeeded.Width ||
				this.Height < (int)sizeNeeded.Height;

			if (expansionNeeded)
				this.Expand();
		}

		public void Unexpand()
		{
			if (this.Expanded)
				this.tipWindow.Close();
		}

		private void tipWindow_Closed(object sender, EventArgs e)
		{
			this.tipWindow = null;
		}

		#endregion
	}
}