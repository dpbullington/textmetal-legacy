// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Drawing;

using NUnit.UiException.CodeFormatters;

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// 	Encapsulates basic colors settings to format a text according a language.
	/// </summary>
	public class CodeRenderingContext
	{
		#region Constructors/Destructors

		public CodeRenderingContext()
		{
			this._colors = new ColorMaterial[]
			               {
				               new ColorMaterial(Color.Black), // code color
				               new ColorMaterial(Color.Blue), // keyword color
				               new ColorMaterial(Color.Green), // comment color
				               new ColorMaterial(Color.Red), // string color

				               new ColorMaterial(Color.White), // background
				               new ColorMaterial(Color.Red), // current line back color
				               new ColorMaterial(Color.White), // current line fore color                
			               };

			return;
		}

		#endregion

		#region Fields/Constants

		private static readonly int INDEX_BACKGROUND = 4;

		private static readonly int INDEX_CODE = 0;
		private static readonly int INDEX_COMMENT = 2;
		private static readonly int INDEX_CURRBACK = 5;
		private static readonly int INDEX_CURRFORE = 6;
		private static readonly int INDEX_KEYWORD = 1;
		private static readonly int INDEX_STRING = 3;

		private ColorMaterial[] _colors;
		private int _currentLine;
		private Font _font;
		private Graphics _graphics;

		#endregion

		#region Properties/Indexers/Events

		public Color this[ClassificationTag tag]
		{
			get
			{
				int idx = (int)tag;
				return (this._colors[idx].Color);
			}
		}

		public Brush BackgroundBrush
		{
			get
			{
				return (this._colors[INDEX_BACKGROUND].Brush);
			}
		}

		public Color BackgroundColor
		{
			get
			{
				return (this._colors[INDEX_BACKGROUND].Color);
			}
			set
			{
				this._colors[INDEX_BACKGROUND].Dispose();
				this._colors[INDEX_BACKGROUND] = new ColorMaterial(value);
			}
		}

		public Pen BackgroundPen
		{
			get
			{
				return (this._colors[INDEX_BACKGROUND].Pen);
			}
		}

		public Brush CodeBrush
		{
			get
			{
				return (this._colors[INDEX_CODE].Brush);
			}
		}

		public Color CodeColor
		{
			get
			{
				return (this._colors[INDEX_CODE].Color);
			}
			set
			{
				this._colors[INDEX_CODE].Dispose();
				this._colors[INDEX_CODE] = new ColorMaterial(value);
			}
		}

		public Pen CodePen
		{
			get
			{
				return (this._colors[INDEX_CODE].Pen);
			}
		}

		public Brush CommentBrush
		{
			get
			{
				return (this._colors[INDEX_COMMENT].Brush);
			}
		}

		public Color CommentColor
		{
			get
			{
				return (this._colors[INDEX_COMMENT].Color);
			}
			set
			{
				this._colors[INDEX_COMMENT].Dispose();
				this._colors[INDEX_COMMENT] = new ColorMaterial(value);
			}
		}

		public Pen CommentPen
		{
			get
			{
				return (this._colors[INDEX_COMMENT].Pen);
			}
		}

		public int CurrentLine
		{
			get
			{
				return (this._currentLine);
			}
			set
			{
				this._currentLine = value;
			}
		}

		public Brush CurrentLineBackBrush
		{
			get
			{
				return (this._colors[INDEX_CURRBACK].Brush);
			}
		}

		public Color CurrentLineBackColor
		{
			get
			{
				return (this._colors[INDEX_CURRBACK].Color);
			}
			set
			{
				this._colors[INDEX_CURRBACK].Dispose();
				this._colors[INDEX_CURRBACK] = new ColorMaterial(value);
			}
		}

		public Pen CurrentLineBackPen
		{
			get
			{
				return (this._colors[INDEX_CURRBACK].Pen);
			}
		}

		public Brush CurrentLineForeBrush
		{
			get
			{
				return (this._colors[INDEX_CURRFORE].Brush);
			}
		}

		public Color CurrentLineForeColor
		{
			get
			{
				return (this._colors[INDEX_CURRFORE].Color);
			}
			set
			{
				this._colors[INDEX_CURRFORE].Dispose();
				this._colors[INDEX_CURRFORE] = new ColorMaterial(value);
			}
		}

		public Pen CurrentLineForePen
		{
			get
			{
				return (this._colors[INDEX_CURRFORE].Pen);
			}
		}

		public Font Font
		{
			get
			{
				return (this._font);
			}
			set
			{
				this._font = value;
			}
		}

		public Graphics Graphics
		{
			get
			{
				return (this._graphics);
			}
			set
			{
				this._graphics = value;
			}
		}

		public Brush KeywordBrush
		{
			get
			{
				return (this._colors[INDEX_KEYWORD].Brush);
			}
		}

		public Color KeywordColor
		{
			get
			{
				return (this._colors[INDEX_KEYWORD].Color);
			}
			set
			{
				this._colors[INDEX_KEYWORD].Dispose();
				this._colors[INDEX_KEYWORD] = new ColorMaterial(value);
			}
		}

		public Pen KeywordPen
		{
			get
			{
				return (this._colors[INDEX_KEYWORD].Pen);
			}
		}

		public Brush StringBrush
		{
			get
			{
				return (this._colors[INDEX_STRING].Brush);
			}
		}

		public Color StringColor
		{
			get
			{
				return (this._colors[INDEX_STRING].Color);
			}
			set
			{
				this._colors[INDEX_STRING].Dispose();
				this._colors[INDEX_STRING] = new ColorMaterial(value);
			}
		}

		public Pen StringPen
		{
			get
			{
				return (this._colors[INDEX_STRING].Pen);
			}
		}

		#endregion

		#region Methods/Operators

		public Brush GetBrush(ClassificationTag tag)
		{
			return (this._colors[(int)tag].Brush);
		}

		public Pen GetPen(ClassificationTag tag)
		{
			return (this._colors[(int)tag].Pen);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class ColorMaterial
		{
			#region Constructors/Destructors

			public ColorMaterial(Color color)
			{
				this.Color = color;
				this.Brush = new SolidBrush(color);
				this.Pen = new Pen(color);

				return;
			}

			#endregion

			#region Fields/Constants

			public Brush Brush;
			public Color Color;
			public Pen Pen;

			#endregion

			#region Methods/Operators

			public void Dispose()
			{
				this.Brush.Dispose();
				this.Pen.Dispose();
			}

			#endregion
		}

		#endregion
	}
}