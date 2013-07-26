// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Drawing;

using NUnit.UiException.Properties;

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// 	Implements IErrorListRenderer.
	/// </summary>
	public class DefaultErrorListRenderer :
		IErrorListRenderer
	{
		//private static readonly int ITEM_HEIGHT = 54;

		#region Constructors/Destructors

		public DefaultErrorListRenderer()
		{
			this.Font = new Font(FontFamily.GenericSansSerif, 8.25f);
			//_fontUnderlined = new Font(_font, FontStyle.Underline);
			//_itemHeight = _font.Height * 4 + 6;

			this._brushBlue = new SolidBrush(Color.FromArgb(0, 43, 114));
			this._brushGray = new SolidBrush(Color.FromArgb(64, 64, 64));

			this._rectListShadow = new Rectangle(0, 0, 48, 9);
			this._rectListBackground = new Rectangle(0, 10, 48, 48);
			this._rectItemGray = new Rectangle(71, 0, 9, 54);
			this._rectItemWhite = new Rectangle(60, 0, 9, 54);
			this._rectSelectionMiddle = new Rectangle(49, 0, 9, 54);
			this._rectIconDll = new Rectangle(1, 59, 16, 15);
			this._rectIconCSharp = new Rectangle(18, 59, 14, 15);
			this._rectIconArrow = new Rectangle(35, 60, 9, 5);
//            _rectShadow = new Rectangle(49, 60, 4, 8);

			this._paintData = new PaintData();

			return;
		}

		#endregion

		#region Fields/Constants

		private static readonly int TEXT_MARGIN_X = 16;
		private Brush _brushBlue;
		private Brush _brushGray;

		private Font _font;
		private Font _fontUnderlined;
		private int _itemHeight;
		private float _offsetLine;
		private PaintData _paintData;
		private Rectangle _rectIconArrow;
		private Rectangle _rectIconCSharp;
		private Rectangle _rectIconDll;

		private Rectangle _rectItemGray;
		private Rectangle _rectItemWhite;
		private Rectangle _rectListBackground;
		private Rectangle _rectListShadow;
		private Rectangle _rectSelectionMiddle;

		#endregion

		#region Properties/Indexers/Events

		public Font Font
		{
			get
			{
				return (this._font);
			}
			set
			{
				this._fontUnderlined = this._font = value;
				if (this._font.FontFamily.IsStyleAvailable(FontStyle.Underline))
					this._fontUnderlined = new Font(this._font, FontStyle.Underline);
				this._itemHeight = this._font.Height * 4 + 6;
			}
		}

		#endregion

		#region Methods/Operators

		private static void PaintBackground(Image img, Graphics g, Rectangle bkg, Rectangle viewport)
		{
			Rectangle destTile;
			int x;
			int y;
			int startY;
			int startX;

			startY = -viewport.Y % viewport.Height;
			startX = -viewport.X % viewport.Width;

			for (y = startY; y < viewport.Height; y += bkg.Height)
			{
				for (x = startX; x < viewport.Width; x += bkg.Width)
				{
					destTile = new Rectangle(x, y, bkg.Width, bkg.Height);
					g.DrawImage(img, destTile, bkg, GraphicsUnit.Pixel);
				}
			}

			return;
		}

		private static void PaintTile(Image tile, Graphics g, Rectangle src, Rectangle dst)
		{
			Rectangle destTile;
			int x;
			int y;

			for (y = dst.Top; y < dst.Bottom; y += src.Height)
			{
				for (x = dst.Left; x < dst.Right; x += src.Width)
				{
					destTile = new Rectangle(x, y, src.Width, src.Height);
					g.DrawImage(tile, destTile, src, GraphicsUnit.Pixel);
				}
			}

			return;
		}

		public void DrawItem(ErrorItem item, int index, bool hovered, bool selected, Graphics g, Rectangle viewport)
		{
			this.DrawItem(item, index, selected, false, hovered, g, viewport);
		}

		private void DrawItem(ErrorItem item, int index, bool selected, bool last, bool hover, Graphics g, Rectangle viewport)
		{
			Rectangle src;
			Font font;

			int x = -viewport.X;
			int y = this._itemHeight * index - viewport.Y;

			src = (index % 2 == 0) ? this._rectItemWhite : this._rectItemGray;
			font = (hover == true) ? this._fontUnderlined : this._font;

			g.DrawImage(Resources.ImageErrorList,
			            new Rectangle(0, y, viewport.Width, this._itemHeight), src,
			            GraphicsUnit.Pixel);

			if (selected)
			{
				g.DrawImage(Resources.ImageErrorList,
				            new Rectangle(0, y + 1, viewport.Width, this._itemHeight),
				            this._rectSelectionMiddle, GraphicsUnit.Pixel);
			}

			if (item.HasSourceAttachment)
			{
				g.DrawImage(Resources.ImageErrorList, new Rectangle(x + 1, y + 2 + font.Height, 14, 15),
				            this._rectIconCSharp, GraphicsUnit.Pixel);
				g.DrawImage(Resources.ImageErrorList,
				            new Rectangle(TEXT_MARGIN_X - 3 + x, y + 5 + 2 * font.Height, 9, 5),
				            this._rectIconArrow, GraphicsUnit.Pixel);

				g.DrawString(String.Format("Line {0}", item.LineNumber),
				             font, this._brushGray, this._offsetLine, y + 2);
				g.DrawString(item.ClassName, font, this._brushBlue, x + TEXT_MARGIN_X, y + 2 + font.Height);
				g.DrawString(item.BaseMethodName + "()", font, this._brushBlue,
				             x + TEXT_MARGIN_X + 5, y + 2 + 2 * font.Height);
				g.DrawString(item.FileName, font, this._brushGray,
				             x + TEXT_MARGIN_X, y + 2 + 3 * this._font.Height);
			}
			else
			{
				g.DrawImage(Resources.ImageErrorList, new Rectangle(x + 1, y + 2 + font.Height, 16, 15),
				            this._rectIconDll, GraphicsUnit.Pixel);

				g.DrawString("N/A", font, this._brushGray, this._offsetLine, y + 2);
				g.DrawString(item.ClassName, font, this._brushGray,
				             x + TEXT_MARGIN_X, y + 2 + font.Height);
				g.DrawString(item.BaseMethodName + "()", font, this._brushGray,
				             x + TEXT_MARGIN_X, y + 2 + 2 * font.Height);
			}

			if (!last)
				return;

			PaintTile(Resources.ImageErrorList, g, this._rectListShadow,
			          new Rectangle(0, y + this._itemHeight, viewport.Width, 9));

			return;
		}

		public void DrawToGraphics(ErrorItemCollection items,
		                           ErrorItem selected, Graphics g, Rectangle viewport)
		{
			SizeF sizeLineSource;
			int last;
			int i;

			UiExceptionHelper.CheckNotNull(items, "items");
			UiExceptionHelper.CheckNotNull(g, "g");

			if (!this._paintData.Equals(items, selected, viewport))
			{
				this._paintData.Dispose();
				this._paintData = new PaintData(items, selected, viewport, g);

				PaintBackground(Resources.ImageErrorList, this._paintData.WorkingGraphics,
				                this._rectListBackground, viewport);

				sizeLineSource = g.MeasureString("Line 9999", this._font);
				this._offsetLine = viewport.Width - sizeLineSource.Width;

				last = this.LastIndexVisible(items.Count, viewport);
				for (i = this.FirstIndexVisible(items.Count, viewport); i <= last; ++i)
				{
					this.DrawItem(items[i], i, selected == items[i], i == items.Count - 1, false,
					              this._paintData.WorkingGraphics, viewport);
				}

				//_paintData.WorkingGraphics.DrawImage(Resources.ErrorList,
				//new Rectangle(0, 0, viewport.Width, _rectShadow.Height),
				//_rectShadow, GraphicsUnit.Pixel);
			}

			this._paintData.PaintTo(g);

			return;
		}

		private int FirstIndexVisible(int count, Rectangle viewport)
		{
			return (Math.Max(0, viewport.Y / this._itemHeight));
		}

		public Size GetDocumentSize(ErrorItemCollection items, Graphics g)
		{
			SizeF current;
			float w;

			this._paintData = new PaintData();

			if (items.Count == 0)
				return (new Size());

			w = 0;
			foreach (ErrorItem item in items)
			{
				current = this.MeasureItem(g, item);
				w = Math.Max(w, current.Width);
			}

			return (new Size((int)w, items.Count * this._itemHeight));
		}

		protected bool IsDirty(ErrorItemCollection items, ErrorItem selection, Rectangle viewport)
		{
			return (!this._paintData.Equals(items, selection, viewport));
		}

		public ErrorItem ItemAt(ErrorItemCollection items, Graphics g, Point point)
		{
			int idx = point.Y / this._itemHeight;

			if (items == null || point.Y < 0 || idx >= items.Count)
				return (null);

			return (items[idx]);
		}

		private int LastIndexVisible(int count, Rectangle viewport)
		{
			return (Math.Min(count - 1,
			                 this.FirstIndexVisible(count, viewport) + 1 + viewport.Height / this._itemHeight));
		}

		protected SizeF MeasureItem(Graphics g, ErrorItem item)
		{
			SizeF sizeMethod;
			SizeF sizeClass;
			SizeF sizeFile;

			UiExceptionHelper.CheckNotNull(g, "g");
			UiExceptionHelper.CheckNotNull(item, "item");

			sizeClass = g.MeasureString(item.ClassName, this._font);
			sizeMethod = g.MeasureString(item.MethodName, this._font);
			sizeFile = g.MeasureString(item.FileName, this._font);

			return (new SizeF(
				Math.Max(sizeClass.Width, Math.Max(sizeMethod.Width, sizeFile.Width)) + TEXT_MARGIN_X,
				this._itemHeight));
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class PaintData
		{
			#region Constructors/Destructors

			public PaintData()
			{
			}

			public PaintData(ErrorItemCollection items, ErrorItem item, Rectangle rectangle, Graphics g)
			{
				if (item == null)
					item = new ErrorItem();
				this.selection = item;

				this._firstItem = ((items.Count > 0) ? items[0] : null);

				this.viewport = rectangle;

				this._workingImage = new Bitmap(rectangle.Width, rectangle.Height, g);
				this.WorkingGraphics = Graphics.FromImage(this._workingImage);

				return;
			}

			#endregion

			#region Fields/Constants

			public Graphics WorkingGraphics;

			private ErrorItem _firstItem;
			private Image _workingImage;
			private ErrorItem selection;
			private Rectangle viewport;

			#endregion

			#region Methods/Operators

			public void Dispose()
			{
				if (this._workingImage != null)
				{
					this._workingImage.Dispose();
					this.WorkingGraphics.Dispose();
				}

				return;
			}

			public bool Equals(ErrorItemCollection items, ErrorItem item, Rectangle rectangle)
			{
				ErrorItem first = ((items.Count > 0) ? items[0] : null);

				return (this.viewport.Equals(rectangle) &&
				        ReferenceEquals(item, this.selection) &&
				        ReferenceEquals(first, this._firstItem));
			}

			public void PaintTo(Graphics g)
			{
				g.DrawImage(this._workingImage, 0, 0);
			}

			#endregion
		}

		#endregion
	}
}