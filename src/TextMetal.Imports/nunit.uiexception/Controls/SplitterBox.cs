// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Drawing;
using System.Windows.Forms;

using NUnit.UiException.Properties;

//
// This re-implements SplitContainer. Why re-inventing the wheel?
// Well... I faced some strange behaviors in SplitContainer in particular
// when I started to provide a custom paint method. It seems to me
// that there is a kind of defect that affects how the Invalidate or
// paint event is called. In some situations I faced a SplitContainer
// that didn't redraw itself while having some parts of its window
// dirty. I didn't found out the cause of the problem.
//
// Another feature that is quite annoying is the unability to change
// the mouse cursor while hovering some special areas of the splitter
// bar. Maybe there is a trick or something but the normal way doesn't
// look like to work.
//

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// 	Implements a place holder that can be splitted either horizontally or vertically.
	/// 	The SplitterBox is layouted with two place holders, respectively named Control1
	/// 	and Control2 where clients can put their controls.
	/// 
	/// 	Unlike SplitContainer, the place holders in SplitterBox are the client controls
	/// 	itself. The direct consequence is the layout policy will be to dock the client
	/// 	controls in filling the maximum possible space.
	/// 
	/// 	SplitterBox also add three buttons on the splitter bar that to change the split
	/// 	orientation and collapse either Control1 or Control2. The example below shows
	/// 	how to intialize and set up SplitterBox with two controls.
	/// 	<code>// creates a new SplitterBox, with a vertical split
	/// 		// and position splitter to appear in the middle of the window
	/// 		SplitterBox splitter = new SplitterBox();
	/// 		splitter.Orientation = Orientation.Vertical;
	/// 		splitter.SplitterDistance = 0.5f;
	/// 		splitter.Control1 = oneControl;
	/// 		splitter.Control2 = anotherControl;</code>
	/// </summary>
	public class SplitterBox : Control
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Creates a new SplitterBox.
		/// </summary>
		public SplitterBox()
		{
			this._brush = new SolidBrush(Color.FromArgb(146, 180, 224));
			this._pen = new Pen(Color.FromArgb(103, 136, 190));

			this._rVerticalCollapse1 = new Rectangle(0, 0, 9, 13);
			this._rVerticalDirection = new Rectangle(10, 0, 9, 13);
			this._rVerticalCollapse2 = new Rectangle(20, 0, 9, 13);

			this._rHorizontalCollapse1 = new Rectangle(0, 24, 13, 9);
			this._rHorizontalDirection = new Rectangle(14, 14, 13, 9);
			this._rHorizontalCollapse2 = new Rectangle(0, 14, 13, 9);

			this._emptyControl1 = new Control();
			this._emptyControl2 = new Control();

			this.Width = 150;
			this.Height = 150;

			this._control1 = this._emptyControl1;
			this._control2 = this._emptyControl2;

			this.Controls.Add(this._control1);
			this.Controls.Add(this._control2);

			this._x = this._y = 0.5f;

			this.Orientation = Orientation.Vertical;

			this.DoLayout();

			return;
		}

		#endregion

		#region Fields/Constants

		public static readonly int BUTTON_SIZE = 13;
		public static readonly int SPLITTER_HALFSIZE = SPLITTER_SIZE / 2;
		public static readonly int SPLITTER_SIZE = 9;
		private Brush _brush;

		private Rectangle _collapse1Rectangle;
		private Rectangle _collapse2Rectangle;
		private Control _control1;
		private Control _control2;
		private Rectangle _directionRectangle;
		private Control _emptyControl1;
		private Control _emptyControl2;

		private bool _movingSplitter;
		private Orientation _orientation;

		private Pen _pen;

		private Rectangle _rHorizontalCollapse1;
		private Rectangle _rHorizontalCollapse2;
		private Rectangle _rHorizontalDirection;
		private Rectangle _rVerticalCollapse1;
		private Rectangle _rVerticalCollapse2;
		private Rectangle _rVerticalDirection;
		private Rectangle _splitterRectangle;
		private float _x;
		private float _y;

		#endregion

		#region Properties/Indexers/Events

		public event EventHandler OrientationChanged;
		public event EventHandler SplitterDistanceChanged;

		protected Rectangle Collapse1Rectangle
		{
			get
			{
				return (this._collapse1Rectangle);
			}
		}

		protected Rectangle Collapse2Rectangle
		{
			get
			{
				return (this._collapse2Rectangle);
			}
		}

		/// <summary>
		/// 	Gets or sets the "first" control to be shown. This control will appear
		/// 	either at the top or on the left when the orientation is respectively
		/// 	vertical or horizontal.
		/// 	If the value is not null, the control will automatically be added
		/// 	to the SplitterBox's hierarchy of controls.
		/// 	If the value is null, the former control is removed and replaced
		/// 	by a default and empty area.
		/// </summary>
		public Control Control1
		{
			get
			{
				return (this._control1);
			}
			set
			{
				if (this._control1 == value)
					return;

				this.Controls.Remove(this._control1);
				if (value == null)
					value = this._emptyControl1;
				this._control1 = value;
				this.Controls.Add(value);
				this.DoLayout();

				return;
			}
		}

		/// <summary>
		/// 	Gets or sets the "second" control to be shown. This control will appear
		/// 	either at the bottom or on the right when the orientation is respectively
		/// 	vertical or horizontal.
		/// 	If the value is not null, the control will automatically be added
		/// 	to the SplitterBox's hierarchy of controls.
		/// 	If the value is null, the former control is removed and replaced
		/// 	by a default and empty area.
		/// </summary>
		public Control Control2
		{
			get
			{
				return (this._control2);
			}
			set
			{
				if (this._control2 == value)
					return;

				if (value == null)
					value = this._emptyControl2;

				this.Controls.Remove(this._control2);
				this._control2 = value;
				this.Controls.Add(value);
				this.DoLayout();

				return;
			}
		}

		protected Rectangle DirectionRectangle
		{
			get
			{
				return (this._directionRectangle);
			}
		}

		/// <summary>
		/// 	Gets or sets the orientation of the splitter in the SplitterBox.
		/// </summary>
		public Orientation Orientation
		{
			get
			{
				return (this._orientation);
			}
			set
			{
				this._orientation = value;
				this.DoLayout();
			}
		}

		/// <summary>
		/// 	Gets or sets the splitter distance expressed as a float number in the
		/// 	range [0 - 1]. A value of 0 collapses Control1 and makes Control2 take
		/// 	the whole space in the window. A value of 1 collapses Control2 and makes
		/// 	Control1 take the whole space in the window. A value of 0.5 makes the
		/// 	splitter appear in the middle of the window.
		/// 
		/// 	Values that don't fall in [0 - 1] are automatically clipped to this range.
		/// </summary>
		public float SplitterDistance
		{
			get
			{
				return (this._orientation == Orientation.Vertical ? this._x : this._y);
			}
			set
			{
				value = Math.Max(0, Math.Min(1, value));
				if (this._orientation == Orientation.Vertical)
					this._x = value;
				else
					this._y = value;
				this.DoLayout();
			}
		}

		/// <summary>
		/// 	Gets the rectangle occupied with the splitter.
		/// </summary>
		public Rectangle SplitterRectangle
		{
			get
			{
				return (this._splitterRectangle);
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Collapses Control1.
		/// </summary>
		public void CollapseControl1()
		{
			this.PointToSplit(0, 0);
		}

		/// <summary>
		/// 	Collapses Control2.
		/// </summary>
		public void CollapseControl2()
		{
			this.PointToSplit(this.Width, this.Height);
		}

		private void DoLayout()
		{
			if (this._orientation == Orientation.Vertical)
				this.VerticalLayout();
			else
				this.HorizontalLayout();

			this.Invalidate();

			return;
		}

		private void FireOrientationChanged()
		{
			if (this.OrientationChanged != null)
				this.OrientationChanged(this, EventArgs.Empty);
		}

		private void FireSplitterDistanceChanged()
		{
			if (this.SplitterDistanceChanged != null)
				this.SplitterDistanceChanged(this, EventArgs.Empty);
		}

		private void HorizontalLayout()
		{
			int x = (this.Width - 41) / 2;
			int y;
			int top;

			top = (int)Math.Max(0, this._y * this.Height - SPLITTER_HALFSIZE);
			top = Math.Min(top, this.Height - SPLITTER_SIZE);

			this._splitterRectangle = new Rectangle(0, top, this.Width, SPLITTER_SIZE);

			y = this._splitterRectangle.Top;

			this._collapse1Rectangle = new Rectangle(x, y, BUTTON_SIZE, SPLITTER_SIZE);
			this._directionRectangle = new Rectangle(this._collapse1Rectangle.Right + 2, y, BUTTON_SIZE, SPLITTER_SIZE);
			this._collapse2Rectangle = new Rectangle(this._directionRectangle.Right + 2, y, BUTTON_SIZE, SPLITTER_SIZE);

			this._control1.SetBounds(0, 0, this.Width, this._splitterRectangle.Top);
			this._control2.SetBounds(0, this._splitterRectangle.Bottom, this.Width, this.Height - this._splitterRectangle.Bottom);

			return;
		}

		private bool HoveringButtons(int x, int y)
		{
			if (!this.SplitterRectangle.Contains(x, y))
				return (false);

			return (this._collapse1Rectangle.Contains(x, y) ||
			        this._collapse2Rectangle.Contains(x, y) ||
			        this._directionRectangle.Contains(x, y));
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.UpdateCursor(e.X, e.Y);

			if (this._splitterRectangle.Contains(e.X, e.Y))
			{
				if (this.HoveringButtons(e.X, e.Y))
					return;

				this._movingSplitter = true;
			}

			base.OnMouseDown(e);

			return;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			this.Cursor = Cursors.Default;
			base.OnMouseLeave(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			this.UpdateCursor(e.X, e.Y);

			if (this._movingSplitter == true)
			{
				this.PointToSplit(e.X, e.Y);
				this.Invalidate();
			}

			base.OnMouseMove(e);

			return;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			bool wasMovingSplitter;

			this.UpdateCursor(e.X, e.Y);

			wasMovingSplitter = this._movingSplitter;
			this._movingSplitter = false;

			if (wasMovingSplitter)
				this.FireSplitterDistanceChanged();
			else
			{
				if (this._collapse1Rectangle.Contains(e.X, e.Y))
				{
					this.CollapseControl1();
					this.FireSplitterDistanceChanged();
					return;
				}

				if (this._collapse2Rectangle.Contains(e.X, e.Y))
				{
					this.CollapseControl2();
					this.FireSplitterDistanceChanged();
					return;
				}

				if (this._directionRectangle.Contains(e.X, e.Y))
				{
					this.Orientation = (this._orientation == Orientation.Vertical) ?
						                                                               Orientation.Horizontal :
							                                                                                      Orientation.Vertical;

					this.FireOrientationChanged();

					return;
				}
			}

			base.OnMouseUp(e);

			return;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(this._brush, this._splitterRectangle);

			if (this.Orientation == Orientation.Vertical)
			{
				e.Graphics.DrawLine(this._pen, this._splitterRectangle.Left, 0,
				                    this.SplitterRectangle.Left, this._splitterRectangle.Height);
				e.Graphics.DrawLine(this._pen, this._splitterRectangle.Right - 1, 0,
				                    this.SplitterRectangle.Right - 1, this._splitterRectangle.Height);

				e.Graphics.DrawImage(Resources.ImageSplitterBox,
				                     this._collapse1Rectangle,
				                     this._rVerticalCollapse1,
				                     GraphicsUnit.Pixel);

				e.Graphics.DrawImage(Resources.ImageSplitterBox,
				                     this._directionRectangle,
				                     this._rVerticalDirection,
				                     GraphicsUnit.Pixel);

				e.Graphics.DrawImage(Resources.ImageSplitterBox,
				                     this._collapse2Rectangle,
				                     this._rVerticalCollapse2,
				                     GraphicsUnit.Pixel);
			}
			else
			{
				e.Graphics.DrawLine(this._pen, 0, this._splitterRectangle.Top,
				                    this.Width, this._splitterRectangle.Top);
				e.Graphics.DrawLine(this._pen, 0, this._splitterRectangle.Bottom - 1,
				                    this.Width, this._splitterRectangle.Bottom - 1);

				e.Graphics.DrawImage(Resources.ImageSplitterBox,
				                     this._collapse1Rectangle,
				                     this._rHorizontalCollapse1,
				                     GraphicsUnit.Pixel);

				e.Graphics.DrawImage(Resources.ImageSplitterBox,
				                     this._directionRectangle,
				                     this._rHorizontalDirection,
				                     GraphicsUnit.Pixel);

				e.Graphics.DrawImage(Resources.ImageSplitterBox,
				                     this._collapse2Rectangle,
				                     this._rHorizontalCollapse2,
				                     GraphicsUnit.Pixel);
			}

			base.OnPaint(e);

			return;
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			if (this._control1 != null)
				this.DoLayout();
			base.OnSizeChanged(e);
		}

		/// <summary>
		/// 	Sets a new location for the splitter expressed as client coordinate.
		/// </summary>
		/// <param name="x"> The new location in pixels when orientation is set to Vertical. </param>
		/// <param name="y"> The new location in pixels when orientation is set to Horizontal. </param>
		public void PointToSplit(int x, int y)
		{
			if (this._orientation == Orientation.Vertical)
			{
				x = Math.Max(0, Math.Min(this.Width, x));
				this._x = (float)x / (float)this.Width;
			}
			else
			{
				y = Math.Max(0, Math.Min(this.Height, y));
				this._y = (float)y / (float)this.Height;
			}

			this.DoLayout();

			return;
		}

		private void UpdateCursor(int x, int y)
		{
			if (!this.SplitterRectangle.Contains(x, y) ||
			    this.HoveringButtons(x, y))
			{
				this.Cursor = Cursors.Default;
				return;
			}

			this.Cursor = (this.Orientation == Orientation.Vertical ? Cursors.VSplit : Cursors.HSplit);

			return;
		}

		private void VerticalLayout()
		{
			int y = (this.Height - 41) / 2;
			int left;
			int x;

			left = (int)Math.Max(0, this._x * this.Width - SPLITTER_HALFSIZE);
			left = Math.Min(left, this.Width - SPLITTER_SIZE);

			this._splitterRectangle = new Rectangle(left, 0, SPLITTER_SIZE, this.Height);

			x = this._splitterRectangle.Left;

			this._collapse1Rectangle = new Rectangle(x, y, SPLITTER_SIZE, BUTTON_SIZE);
			this._directionRectangle = new Rectangle(x, this._collapse1Rectangle.Bottom + 2, SPLITTER_SIZE, BUTTON_SIZE);
			this._collapse2Rectangle = new Rectangle(x, this._directionRectangle.Bottom + 2, SPLITTER_SIZE, BUTTON_SIZE);

			this._control1.SetBounds(0, 0, this._splitterRectangle.Left, this.Height);
			this._control2.SetBounds(this._splitterRectangle.Right, 0, this.Width - this._splitterRectangle.Right, this.Height);

			return;
		}

		#endregion
	}
}