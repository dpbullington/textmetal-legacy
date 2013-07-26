// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Drawing;

namespace NUnit.UiException.Controls
{
	/// <summary>
	/// 	Encapsulate data to draw a line of text.
	/// </summary>
	public class PaintLineLocation
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Build a new instance of this object given some data.
		/// </summary>
		/// <param name="lineIndex"> Index of the current line. </param>
		/// <param name="text"> String value at this line. </param>
		/// <param name="location"> Client coordinate where beginning the drawing. </param>
		public PaintLineLocation(int lineIndex, string text, PointF location)
		{
			this.SetLine(lineIndex);
			this.SetText(text);
			this.SetLocation(location);

			return;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// 	Index of the current line.
		/// </summary>
		private int _lineIndex;

		/// <summary>
		/// 	A client coordinate from where beginning the drawing.
		/// </summary>
		private PointF _location;

		/// <summary>
		/// 	The string value at this line.
		/// </summary>
		private string _text;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Index of the current line.
		/// </summary>
		public int LineIndex
		{
			get
			{
				return (this._lineIndex);
			}
		}

		/// <summary>
		/// 	Client coordinate where to beginning the drawing.
		/// </summary>
		public PointF Location
		{
			get
			{
				return (this._location);
			}
		}

		/// <summary>
		/// 	String value at this line.
		/// </summary>
		public string Text
		{
			get
			{
				return (this._text);
			}
		}

		#endregion

		#region Methods/Operators

		public override bool Equals(object obj)
		{
			PaintLineLocation line;

			if (obj == null ||
			    !(obj is PaintLineLocation))
				return (false);

			line = obj as PaintLineLocation;

			return (line.LineIndex == this.LineIndex &&
			        line.Text == this.Text &&
			        line.Location == this.Location);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		protected void SetLine(int lineIndex)
		{
			this._lineIndex = lineIndex;

			return;
		}

		protected void SetLocation(PointF location)
		{
			this._location = location;
		}

		protected void SetText(string text)
		{
			UiExceptionHelper.CheckNotNull(text, "text");
			this._text = text;
		}

		public override string ToString()
		{
			return ("PaintLineLocation: {" + this.LineIndex + ":[" + this.Text + "]:(" +
			        this.Location.X + ", " + this.Location.Y + ")}");
		}

		#endregion
	}
}