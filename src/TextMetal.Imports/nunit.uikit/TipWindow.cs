// ****************************************************************
// Copyright 2002-2003, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CP.Windows.Forms
{
	public class TipWindow : Form
	{
		#region Constructors/Destructors

		public TipWindow(Control control)
		{
			this.InitializeComponent();
			this.InitControl(control);

			// Note: This causes an error if called on a listbox
			// with no item as yet selected, therefore, it is handled
			// differently in the constructor for a listbox.
			this.tipText = control.Text;
		}

		public TipWindow(ListBox listbox, int index)
		{
			this.InitializeComponent();
			this.InitControl(listbox);

			this.itemBounds = listbox.GetItemRectangle(index);
			this.tipText = listbox.Items[index].ToString();
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Time before automatically closing
		/// </summary>
		private int autoCloseDelay = 0;

		/// <summary>
		/// Timer used for auto-close
		/// </summary>
		private Timer autoCloseTimer;

		/// <summary>
		/// The control for which we are showing expanded text
		/// </summary>
		private Control control;

		/// <summary>
		/// Directions we are allowed to expand
		/// </summary>
		private ExpansionStyle expansion = ExpansionStyle.Horizontal;

		/// <summary>
		/// Rectangle representing bounds to overlay. For a listbox, this
		/// is a single item rectangle. For other controls, it is usually
		/// the entire client area.
		/// </summary>
		private Rectangle itemBounds;

		/// <summary>
		/// Time to wait for after mouse leaves
		/// the window or the label before closing.
		/// </summary>
		private int mouseLeaveDelay = 300;

		/// <summary>
		/// Timer used for mouse leave delay
		/// </summary>
		private Timer mouseLeaveTimer;

		/// <summary>
		/// True if we may overlay control or item
		/// </summary>
		private bool overlay = true;

		/// <summary>
		/// Rectangle used to display text
		/// </summary>
		private Rectangle textRect;

		/// <summary>
		/// Text we are displaying
		/// </summary>
		private string tipText;

		/// <summary>
		/// Indicates whether any clicks should be passed to the underlying control
		/// </summary>
		private bool wantClicks = false;

		#endregion

		#region Properties/Indexers/Events

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

		public ExpansionStyle Expansion
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

		public Rectangle ItemBounds
		{
			get
			{
				return this.itemBounds;
			}
			set
			{
				this.itemBounds = value;
			}
		}

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

		public string TipText
		{
			get
			{
				return this.tipText;
			}
			set
			{
				this.tipText = value;
			}
		}

		public bool WantClicks
		{
			get
			{
				return this.wantClicks;
			}
			set
			{
				this.wantClicks = value;
			}
		}

		#endregion

		#region Methods/Operators

		[DllImport("user32.dll")]
		private static extern uint SendMessage(
			IntPtr hwnd,
			int msg,
			IntPtr wparam,
			IntPtr lparam
			);

		private void InitControl(Control control)
		{
			this.control = control;
			this.Owner = control.FindForm();
			this.itemBounds = control.ClientRectangle;

			this.ControlBox = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.BackColor = Color.LightYellow;
			this.FormBorderStyle = FormBorderStyle.None;
			this.StartPosition = FormStartPosition.Manual;

			this.Font = control.Font;
		}

		private void InitializeComponent()
		{
			// 
			// TipWindow
			// 
			this.BackColor = Color.LightYellow;
			this.ClientSize = new Size(292, 268);
			this.ControlBox = false;
			this.FormBorderStyle = FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TipWindow";
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.Manual;
		}

		private void OnAutoClose(object sender, EventArgs e)
		{
			this.Close();
		}

		protected override void OnLoad(EventArgs e)
		{
			// At this point, further changes to the properties
			// of the label will have no effect on the tip.
			Point origin = this.control.Parent.PointToScreen(this.control.Location);
			origin.Offset(this.itemBounds.Left, this.itemBounds.Top);
			if (!this.overlay)
				origin.Offset(0, this.itemBounds.Height);
			this.Location = origin;

			Graphics g = Graphics.FromHwnd(this.Handle);
			Screen screen = Screen.FromControl(this.control);
			SizeF layoutArea = new SizeF(screen.WorkingArea.Width - 40, screen.WorkingArea.Height - 40);
			if (this.expansion == ExpansionStyle.Vertical)
				layoutArea.Width = this.itemBounds.Width;
			else if (this.expansion == ExpansionStyle.Horizontal)
				layoutArea.Height = this.itemBounds.Height;

			Size sizeNeeded = Size.Ceiling(g.MeasureString(this.tipText, this.Font, layoutArea));

			this.ClientSize = sizeNeeded;
			this.Size = sizeNeeded + new Size(2, 2);
			this.textRect = new Rectangle(1, 1, sizeNeeded.Width, sizeNeeded.Height);

			// Catch mouse leaving the control
			this.control.MouseLeave += new EventHandler(this.control_MouseLeave);

			// Catch the form that holds the control closing
			this.control.FindForm().Closed += new EventHandler(this.control_FormClosed);

			if (this.Right > screen.WorkingArea.Right)
			{
				this.Left = Math.Max(
					screen.WorkingArea.Right - this.Width - 20,
					screen.WorkingArea.Left + 20);
			}

			if (this.Bottom > screen.WorkingArea.Bottom - 20)
			{
				if (this.overlay)
				{
					this.Top = Math.Max(
						screen.WorkingArea.Bottom - this.Height - 20,
						screen.WorkingArea.Top + 20);
				}

				if (this.Bottom > screen.WorkingArea.Bottom - 20)
					this.Height = screen.WorkingArea.Bottom - 20 - this.Top;
			}

			if (this.autoCloseDelay > 0)
			{
				this.autoCloseTimer = new Timer();
				this.autoCloseTimer.Interval = this.autoCloseDelay;
				this.autoCloseTimer.Tick += new EventHandler(this.OnAutoClose);
				this.autoCloseTimer.Start();
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			if (this.mouseLeaveTimer != null)
			{
				this.mouseLeaveTimer.Stop();
				this.mouseLeaveTimer.Dispose();
				Debug.WriteLine("Entered TipWindow - stopped mouseLeaveTimer");
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (this.mouseLeaveDelay > 0)
			{
				this.mouseLeaveTimer = new Timer();
				this.mouseLeaveTimer.Interval = this.mouseLeaveDelay;
				this.mouseLeaveTimer.Tick += new EventHandler(this.OnAutoClose);
				this.mouseLeaveTimer.Start();
				Debug.WriteLine("Left TipWindow - started mouseLeaveTimer");
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics g = e.Graphics;
			Rectangle outlineRect = this.ClientRectangle;
			outlineRect.Inflate(-1, -1);
			g.DrawRectangle(Pens.Black, outlineRect);
			g.DrawString(this.tipText, this.Font, Brushes.Black, this.textRect);
		}

		protected override void WndProc(ref Message m)
		{
			uint WM_LBUTTONDOWN = 0x201;
			uint WM_RBUTTONDOWN = 0x204;
			uint WM_MBUTTONDOWN = 0x207;

			if (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_MBUTTONDOWN)
			{
				if (m.Msg != WM_LBUTTONDOWN)
					this.Close();
				SendMessage(this.control.Handle, m.Msg, m.WParam, m.LParam);
			}
			else
				base.WndProc(ref m);
		}

		/// <summary>
		/// The form our label is on closed, so we should.
		/// </summary>
		private void control_FormClosed(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// The mouse left the label. We ignore if we are
		/// overlaying the label but otherwise start a
		/// delay for closing the window
		/// </summary>
		private void control_MouseLeave(object sender, EventArgs e)
		{
			if (this.mouseLeaveDelay > 0 && !this.overlay)
			{
				this.mouseLeaveTimer = new Timer();
				this.mouseLeaveTimer.Interval = this.mouseLeaveDelay;
				this.mouseLeaveTimer.Tick += new EventHandler(this.OnAutoClose);
				this.mouseLeaveTimer.Start();
				Debug.WriteLine("Left Control - started mouseLeaveTimer");
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// Direction in which to expand
		/// </summary>
		public enum ExpansionStyle
		{
			Horizontal,
			Vertical,
			Both
		}

		#endregion
	}
}