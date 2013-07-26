// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using NUnit.UiException.CodeFormatters;

namespace NUnit.UiException.Controls
{
	public class DefaultCodeRenderer :
		ICodeRenderer
	{
		#region Fields/Constants

		private static readonly float MEASURECHAR_BIG_HEIGHT = 100f;

		/// <summary>
		/// 	These constants below address an issue at measure text time
		/// 	that sometimes can cause big lines of text to be misaligned.
		/// </summary>
		private static readonly float MEASURECHAR_BIG_WIDTH = 5000f;

		#endregion

		#region Methods/Operators

		private int CountVisibleLines(float viewportHeight, float fontHeight)
		{
			return ((int)(viewportHeight / fontHeight) + 1);
		}

		public void DrawToGraphics(FormattedCode code, CodeRenderingContext args, Rectangle viewport)
		{
			UiExceptionHelper.CheckNotNull(code, "code");
			UiExceptionHelper.CheckNotNull(args, "args");

			ClassifiedTokenCollection line;
			PaintLineLocation[] lines;
			ClassifiedToken token;
			float fontHeight;
			string text;
			float tk_width;
			int i;
			float x;

			fontHeight = this.LineIndexToYCoordinate(1, args.Graphics, args.Font);
			lines = this.ViewportLines(code, viewport, fontHeight);

			foreach (PaintLineLocation paintLine in lines)
			{
				// All lines that differ from CurrentLine are displayed
				// in using different styles of Brush to make a distinction
				// between code, keyword, comments.
				if (paintLine.LineIndex != args.CurrentLine)
				{
					line = code[paintLine.LineIndex];
					x = 0;
					text = line.Text;

					for (i = 0; i < line.Count; ++i)
					{
						token = line[i];

						args.Graphics.DrawString(token.Text, args.Font, args.GetBrush(token.Tag),
						                         paintLine.Location.X + x, paintLine.Location.Y);

						tk_width = this.measureStringWidth(args.Graphics, args.Font, text,
						                                   token.IndexStart, token.Text.Length);

						x += tk_width;
					}

					continue;
				}

				// The current line is emphasized by using a 
				// specific couples of Background & Foreground colors

				args.Graphics.FillRectangle(
					args.CurrentLineBackBrush,
					0, paintLine.Location.Y,
					viewport.Width, fontHeight);

				args.Graphics.DrawString(
					paintLine.Text, args.Font,
					args.CurrentLineForeBrush,
					paintLine.Location.X, paintLine.Location.Y);
			}

			return;
		}

		public SizeF GetDocumentSize(FormattedCode code, Graphics g, Font font)
		{
			UiExceptionHelper.CheckNotNull(code, "code");
			UiExceptionHelper.CheckNotNull(g, "g");
			UiExceptionHelper.CheckNotNull(font, "font");

			StringBuilder sample;
			SizeF measure;
			int i;

			sample = new StringBuilder();
			for (i = code.MaxLength; i > 0; --i)
				sample.Append("m");

			measure = g.MeasureString(sample.ToString(), font);

			return (new SizeF(measure.Width, measure.Height * code.LineCount));
		}

		public float LineIndexToYCoordinate(int lineIndex, Graphics g, Font font)
		{
			UiExceptionHelper.CheckNotNull(g, "g");
			UiExceptionHelper.CheckNotNull(font, "font");

			SizeF sz = g.MeasureString("m", font);
			return (lineIndex * sz.Height);
		}

		private float LineIndexToYCoordinate(int index, float fontHeight)
		{
			return (index * fontHeight);
		}

		public PaintLineLocation[] ViewportLines(FormattedCode code, RectangleF viewport, float fontHeight)
		{
			List<PaintLineLocation> list = new List<PaintLineLocation>();
			int visibles = this.CountVisibleLines(viewport.Height, fontHeight);
//            int topIndex = YCoordinateToLineIndex(viewport.Top, fontHeight);
			int lineIndex = this.YCoordinateToLineIndex(viewport.Top, fontHeight);
			int i;

			for (i = 0; i < visibles; ++i, lineIndex++)
			{
				if (lineIndex < 0)
					continue;

				if (lineIndex >= code.LineCount)
					break;

				list.Add(
					new PaintLineLocation(lineIndex, code.GetTextAt(lineIndex),
					                      new PointF(-viewport.Left,
					                                 this.LineIndexToYCoordinate(lineIndex, fontHeight) -
					                                 viewport.Top)));
			}

			return (list.ToArray());
		}

		private int YCoordinateToLineIndex(float y, float fontHeight)
		{
			return (int)(y / fontHeight);
		}

		/// <summary>
		/// 	Utility method that measures a region of text in the given string.
		/// </summary>
		/// <param name="g"> The graphics instance used to render this text. </param>
		/// <param name="font"> The font instance used to render this text. </param>
		/// <param name="text"> The text that contains the region to be rendered. </param>
		/// <param name="indexStart"> Starting startingPosition of this region. </param>
		/// <param name="length"> Length of this region. </param>
		/// <returns> The width of this region of text. </returns>
		private float measureStringWidth(Graphics g, Font font, string text, int indexStart, int length)
		{
			CharacterRange[] ranges;
			StringFormat sf;
			Region[] regions;

			if (length == 0)
				return (0);

			length = Math.Min(length, text.Length);

			ranges = new CharacterRange[] { new CharacterRange(indexStart, length) };
			sf = new StringFormat();

			// the string of text may contains white spaces that need to
			// be measured correctly.

			sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;

			sf.SetMeasurableCharacterRanges(ranges);

			// here : giving a layout too small can cause returned measure
			// to be wrong.

			regions = g.MeasureCharacterRanges(
				text, font, new RectangleF(
					            0, 0, MEASURECHAR_BIG_WIDTH, MEASURECHAR_BIG_HEIGHT), sf);

			return (regions[0].GetBounds(g).Width);
		}

		#endregion
	}
}