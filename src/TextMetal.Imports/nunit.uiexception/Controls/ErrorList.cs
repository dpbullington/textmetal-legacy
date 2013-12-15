// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Drawing;
using System.Windows.Forms;

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// Displays a control which implements IStackTraceView.
	/// </summary>
	public class ErrorList :
		UserControl,
		IStackTraceView
	{
		#region Constructors/Destructors

		/// <summary>
		/// Builds a new instance of ErrorList.
		/// </summary>
		public ErrorList()
			:
				this(new DefaultErrorListRenderer())
		{
		}

		protected ErrorList(IErrorListRenderer renderer)
		{
			UiExceptionHelper.CheckNotNull(renderer, "display");

			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.DoubleBuffered = true;

			this._renderer = renderer;
			this._items = new ErrorItemCollection();
			this._stackTrace = null;
			this._selection = null;
			this._workingGraphics = this.CreateGraphics();
			this._hoveredIndex = -1;

			this._autoSelectFirstItem = false;
			this._listOrder = ErrorListOrderPolicy.InitialOrder;

			return;
		}

		#endregion

		#region Fields/Constants

		private bool _autoSelectFirstItem;
		protected int _hoveredIndex;

		private ErrorItemCollection _items;
		private ErrorListOrderPolicy _listOrder;
		private Point _mouse;
		protected IErrorListRenderer _renderer;
		private ErrorItem _selection;
		private string _stackTrace;
		protected Graphics _workingGraphics;

		#endregion

		#region Properties/Indexers/Events

		public event EventHandler SelectedItemChanged;

		public bool AutoSelectFirstItem
		{
			get
			{
				return (this._autoSelectFirstItem);
			}
			set
			{
				this._autoSelectFirstItem = value;
			}
		}

		/// <summary>
		/// Gives access to the item collection.
		/// </summary>
		public ErrorItemCollection Items
		{
			get
			{
				return (this._items);
			}
		}

		public ErrorListOrderPolicy ListOrderPolicy
		{
			get
			{
				return (this._listOrder);
			}
			set
			{
				if (this._listOrder == value)
					return;
				this._listOrder = value;
				this._items.Reverse();
				this.Invalidate();
			}
		}

		public ErrorItem SelectedItem
		{
			get
			{
				return (this._selection);
			}
			set
			{
				bool fireEvent;

				if (value != null &&
					(!this._items.Contains(value) || !value.HasSourceAttachment))
					return;

				fireEvent = (this._selection != value);
				this._selection = value;

				if (fireEvent && this.SelectedItemChanged != null)
					this.SelectedItemChanged(this, new EventArgs());

				this.Invalidate();
			}
		}

		public string StackTrace
		{
			get
			{
				return (this._stackTrace);
			}
			set
			{
				ErrorItem candidate;

				candidate = this.PopulateList(value);

				if (!String.IsNullOrEmpty(value) &&
					this._items.Count == 0)
					this._items.Add(new ErrorItem(null, "Fail to parse stack trace", -1));

				this.AutoScrollMinSize = this._renderer.GetDocumentSize(this._items, this._workingGraphics);

				this._hoveredIndex = -1;
				this.SelectedItem = (this.AutoSelectFirstItem ? candidate : null);
				this.Invalidate();

				return;
			}
		}

		#endregion

		#region Methods/Operators

		protected virtual void ItemEntered(int index)
		{
			this.Cursor = Cursors.Hand;
		}

		protected virtual void ItemLeaved(int index)
		{
			this.Cursor = Cursors.Default;
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			this.OnClick(this._mouse);

			return;
		}

		protected void OnClick(Point point)
		{
			this.SelectedItem = this._renderer.ItemAt(this._items, this._workingGraphics, point);

			return;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			this._renderer.Font = this.Font;

			base.OnFontChanged(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			this._mouse = new Point(e.X, e.Y - this.AutoScrollPosition.Y);
		}

		protected override void OnMouseHover(EventArgs e)
		{
			base.OnMouseHover(e);
			this.Focus();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			ErrorItem item;
			int itemIndex;

			base.OnMouseMove(e);

			item = this._renderer.ItemAt(this._items, this._workingGraphics, new Point(e.X, e.Y - this.AutoScrollPosition.Y));

			itemIndex = -1;
			for (int i = 0; i < this._items.Count; ++i)
			{
				if (ReferenceEquals(this._items[i], item))
				{
					itemIndex = i;
					break;
				}
			}

			if (itemIndex != this._hoveredIndex)
			{
				if (this._hoveredIndex != -1)
					this.ItemLeaved(this._hoveredIndex);

				if (itemIndex != -1 && this._items[itemIndex].HasSourceAttachment)
				{
					this.ItemEntered(itemIndex);
					this._hoveredIndex = itemIndex;
				}
				else
					this._hoveredIndex = -1;
				this.Invalidate();
			}

			return;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle viewport;

			base.OnPaint(e);

			viewport = new Rectangle(-this.AutoScrollPosition.X, -this.AutoScrollPosition.Y,
				this.ClientRectangle.Width, this.ClientRectangle.Height);
			this._renderer.DrawToGraphics(this._items, this._selection, e.Graphics, viewport);

			if (this._hoveredIndex != -1)
			{
				this._renderer.DrawItem(this._items[this._hoveredIndex], this._hoveredIndex, true,
					this._items[this._hoveredIndex] == this._selection, e.Graphics, viewport);
			}

			return;
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			this.Invalidate();
		}

		private ErrorItem PopulateList(string stackTrace)
		{
			StackTraceParser parser = new StackTraceParser();
			ErrorItem candidate;

			this._stackTrace = stackTrace;
			parser.Parse(stackTrace);
			if (this._listOrder == ErrorListOrderPolicy.ReverseOrder)
				parser.Items.Reverse();

			candidate = null;
			this._items.Clear();
			foreach (ErrorItem item in parser.Items)
			{
				if (candidate == null && item.HasSourceAttachment)
					candidate = item;
				this._items.Add(item);
			}

			return (candidate);
		}

		#endregion
	}
}