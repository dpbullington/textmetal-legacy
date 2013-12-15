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
	/// A special type of textbox which can display a tooltip-like
	/// window to show the full extent of any text which doesn't
	/// fit. The window may be placed directly over the label
	/// or immediately beneath it and will expand to fit in
	/// a horizontal, vertical or both directions as needed.
	/// TODO: This control is virtually identical to ExpandingLabel.
	/// We need to have an extension provider that works like a
	/// ToolTip in order to eliminate the duplication.
	/// </summary>
	public class ExpandingTextBox : TextBox
	{
		#region Fields/Constants

		/// <summary>
		/// Time in milliseconds that the tip window
		/// will remain displayed.
		/// </summary>
		private int autoCloseDelay = 0;

		/// <summary>
		/// True if control should expand automatically on hover.
		/// </summary>
		private bool autoExpand = true;

		/// <summary>
		/// If true, a context menu with Copy is displayed which
		/// allows copying contents to the clipboard.
		/// </summary>
		private bool copySupported = false;

		/// <summary>
		/// Direction of expansion
		/// </summary>
		private TipWindow.ExpansionStyle expansion = TipWindow.ExpansionStyle.Horizontal;

		/// <summary>
		/// Timer used to control display behavior on hover.
		/// </summary>
		private Timer hoverTimer;

		/// <summary>
		/// Time in milliseconds that the mouse must
		/// be stationary over an item before the
		/// tip window will display.
		/// </summary>
		private int mouseHoverDelay = 300;

		/// <summary>
		/// Time in milliseconds that the window stays
		/// open after the mouse leaves the control.
		/// </summary>
		private int mouseLeaveDelay = 300;

		/// <summary>
		/// True if tipWindow may overlay the label
		/// </summary>
		private bool overlay = true;

		/// <summary>
		/// Our window for displaying expanded text
		/// </summary>
		private TipWindow tipWindow;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Time in milliseconds that the tip window
		/// will remain displayed.
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
		[DefaultValue(true)]
		public bool AutoExpand
		{
			get
			{
				return this.autoExpand;
			}
			set
			{
				this.autoExpand = value;
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
		/// Time in milliseconds that the mouse must
		/// be stationary over an item before the
		/// tip window will display.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(300)]
		[Description("Time in milliseconds that mouse must be stationary over an item before the tip is displayed.")]
		public int MouseHoverDelay
		{
			get
			{
				return this.mouseHoverDelay;
			}
			set
			{
				this.mouseHoverDelay = value;
			}
		}

		/// <summary>
		/// Time in milliseconds that the window stays
		/// open after the mouse leaves the control.
		/// Reentering the control resets this.
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
		/// Override Text property to set up copy menu if
		/// the value is non-empty.
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

		private void ClearTimer()
		{
			if (this.hoverTimer != null)
			{
				this.hoverTimer.Stop();
				this.hoverTimer.Dispose();
			}
		}

		/// <summary>
		/// Copy contents to clipboard
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

		private void OnMouseHover(object sender, EventArgs e)
		{
			if (this.autoExpand)
			{
				Graphics g = Graphics.FromHwnd(this.Handle);
				SizeF sizeNeeded = g.MeasureString(this.Text, this.Font);
				bool expansionNeeded =
					this.Width < (int)sizeNeeded.Width ||
					this.Height < (int)sizeNeeded.Height;

				if (expansionNeeded)
					this.Expand();
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			this.ClearTimer();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			this.ClearTimer();

			int textExtent = this.Lines.Length * this.FontHeight;
			if (e.Y <= textExtent)
			{
				this.hoverTimer = new Timer();
				this.hoverTimer.Interval = this.mouseHoverDelay;
				this.hoverTimer.Tick += new EventHandler(this.OnMouseHover);
				this.hoverTimer.Start();
			}

			base.OnMouseMove(e);
		}

		public void Unexpand()
		{
			if (this.Expanded)
				this.tipWindow.Close();
		}

		private void tipWindow_Closed(object sender, EventArgs e)
		{
			this.tipWindow = null;
			this.ClearTimer();
		}

		#endregion
	}

//	public class HoverDetector
//	{
//		private Control control;
//
//		private Timer hoverTimer;
//
//		private int hoverDelay;
//
//		public int HoverDelay
//		{
//			get { return hoverDelay; }
//			set { hoverDelay = value; }
//		}
//
//		public event System.EventHandler Hover;
//
//		public HoverDetector( Control control )
//		{
//			this.control = control;
//			
//			control.MouseLeave += new EventHandler( OnMouseLeave );
//			control.MouseMove += new MouseEventHandler( OnMouseMove );
//		}
//
//		private void OnMouseLeave( object sender, System.EventArgs e )
//		{
//			ClearTimer();
//		}
//
//		private void OnMouseMove( object sender, MouseEventArgs e )
//		{
//			ClearTimer();
//
//			hoverTimer = new System.Windows.Forms.Timer();
//			hoverTimer.Interval = hoverDelay;
//			hoverTimer.Tick += new EventHandler( OnMouseHover );
//			hoverTimer.Start();	
//		}
//
//		private void OnMouseHover( object sender, System.EventArgs e )
//		{
//			if ( Hover != null )
//				Hover( this, e );
//		}
//
//		private void ClearTimer()
//		{
//			if ( hoverTimer != null )
//			{
//				hoverTimer.Stop();
//				hoverTimer.Dispose();
//			}
//		}
//	}
}