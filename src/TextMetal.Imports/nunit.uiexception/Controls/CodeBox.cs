// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Drawing;
using System.Windows.Forms;

using NUnit.UiException.CodeFormatters;

/// This control could have been replaced by a standard RichTextBox control, but
/// it turned out that RichTextBox:
///     - was hard to configure
///     - was hard to set the viewport
///     - doesn't use double buffer optimization
///     - scrolls text one line at a time without be configurable.
/// 
/// CodeBox has been written to address these specific issues in order to display
/// C# source code where exceptions occured.

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// 	A control that implements ICodeView.
	/// </summary>
	public class CodeBox : UserControl, ICodeView
	{
		#region Constructors/Destructors

		public CodeBox()
			:
				this(new GeneralCodeFormatter(), new DefaultCodeRenderer())
		{
		}

		protected CodeBox(IFormatterCatalog formatter, ICodeRenderer renderer)
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.DoubleBuffered = true;

			this._formatter = formatter;
			this._formattedCode = FormattedCode.Empty;

			this._renderer = renderer;

			this._currentLine = -1;
			this._showCurrentLine = false;

			this._language = "";

			this.Font = new Font(FontFamily.GenericMonospace, 8);
			this.BackColor = Color.White;

			this.createGraphics();
			this.AutoScroll = true;

			return;
		}

		#endregion

		#region Fields/Constants

		private int _currentLine;

		protected FormattedCode _formattedCode;

		private IFormatterCatalog _formatter;
		private string _language;
		private ICodeRenderer _renderer;

		private bool _showCurrentLine;
		protected CodeRenderingContext _workingContext;

		#endregion

		#region Properties/Indexers/Events

		public int CurrentLine
		{
			get
			{
				return (this._currentLine);
			}
			set
			{
				float y = this._renderer.LineIndexToYCoordinate(value,
				                                                this._workingContext.Graphics, this._workingContext.Font);

				y -= this.Height / 2;

				this._currentLine = value;
				this.AutoScrollPosition = new Point(0, (int)y);

				this.Invalidate();
			}
		}

		/// <summary>
		/// 	If ShowCurrentLine is set, this set the current line's background color.
		/// </summary>
		public Color CurrentLineBackColor
		{
			get
			{
				return (this._workingContext.CurrentLineBackColor);
			}
			set
			{
				this._workingContext.CurrentLineBackColor = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// 	If ShowCurrentLine is set, this set current line's foreground color.
		/// </summary>
		public Color CurrentLineForeColor
		{
			get
			{
				return (this._workingContext.CurrentLineForeColor);
			}
			set
			{
				this._workingContext.CurrentLineForeColor = value;
				this.Invalidate();
			}
		}

		public IFormatterCatalog Formatter
		{
			get
			{
				return (this._formatter);
			}
		}

		public string Language
		{
			get
			{
				return (this._language);
			}
			set
			{
				if (value == null)
					value = "";
				if (this._language == value)
					return;

				this._language = value;
				this.Text = this.Text;
			}
		}

		/// <summary>
		/// 	Gets or sets a value telling whether or not displaying a special
		/// 	feature for the current line at drawing time.
		/// </summary>
		public bool ShowCurrentLine
		{
			get
			{
				return (this._showCurrentLine);
			}
			set
			{
				this._showCurrentLine = value;
			}
		}

		public override string Text
		{
			get
			{
				return (this._formattedCode.Text);
			}
			set
			{
				if (value == null)
					value = "";

				SizeF docSize;

				this._formattedCode = this._formatter.Format(value, this._language);
				docSize = this._renderer.GetDocumentSize(
					this._formattedCode, this._workingContext.Graphics, this._workingContext.Font);
				this.AutoScrollMinSize = new Size((int)docSize.Width, (int)docSize.Height);

				this.Invalidate();

				return;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);

			if (this._workingContext != null)
			{
				this._workingContext.Font = this.Font;
				this.Text = this.Text;
			}

			return;
		}

		protected override void OnMouseHover(EventArgs e)
		{
			base.OnMouseHover(e);
			this.Focus();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics backup;

			base.OnPaint(e);

			backup = this._workingContext.Graphics;
			this._workingContext.Graphics = e.Graphics;
			this._workingContext.CurrentLine = (this._showCurrentLine ? this._currentLine : -1);

			this._renderer.DrawToGraphics(this._formattedCode, this._workingContext,
			                              new Rectangle(-this.AutoScrollPosition.X, -this.AutoScrollPosition.Y, this.Width, this.Height));

			this._workingContext.Graphics = backup;

			return;
		}

		private void createGraphics()
		{
			Graphics gCurrent = this.CreateGraphics();
			Image img = new Bitmap(10, 10, gCurrent);
			Graphics gImg = Graphics.FromImage(img);

			gCurrent.Dispose();

			this._workingContext = new CodeRenderingContext();
			this._workingContext.Graphics = gImg;
			this._workingContext.Font = this.Font;

			this._workingContext.CurrentLine = -1;
			this._workingContext.BackgroundColor = Color.White;
			this._workingContext.CurrentLineBackColor = Color.Red;
			this._workingContext.CurrentLineForeColor = Color.White;
			this._workingContext.CodeColor = Color.Black;
			this._workingContext.CommentColor = Color.Green;
			this._workingContext.KeywordColor = Color.Blue;
			this._workingContext.StringColor = Color.Red;

			return;
		}

		#endregion
	}
}