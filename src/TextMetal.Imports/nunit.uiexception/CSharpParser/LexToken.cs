// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.UiException.CodeFormatters
{
	/// <summary>
	/// 	This enum defines the list of all tags
	/// 	that can be assigned to a particular string.
	/// </summary>
	public enum LexerTag
	{
		/// <summary>
		/// 	All sequences but the ones below
		/// </summary>
		Text,

		/// <summary>
		/// 	White characters: ' ' \t \n
		/// 	and other separators like:
		/// 	- '[' ']' '(' ')' ';'
		/// </summary>
		Separator,

		/// <summary>
		/// 	Char: \n
		/// </summary>
		EndOfLine,

		/// <summary>
		/// 	string: /*
		/// </summary>
		CommentC_Open,

		/// <summary>
		/// 	string: */
		/// </summary>
		CommentC_Close,

		/// <summary>
		/// 	string: //
		/// </summary>
		CommentCpp,

		/// <summary>
		/// 	Char: '
		/// </summary>
		SingleQuote,

		/// <summary>
		/// 	Char: "
		/// </summary>
		DoubleQuote
	}

	/// <summary>
	/// 	This class is used to make the link between a string and a LexerTag value.
	/// </summary>
	public class LexToken
	{
		#region Constructors/Destructors

		public LexToken()
		{
			this._text = null;
			this._tag = LexerTag.Text;
			this._start = -1;

			return;
		}

		public LexToken(string text, LexerTag tag, int start)
		{
			this._text = text;
			this._tag = tag;
			this._start = start;

			return;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// 	The starting startingPosition.
		/// </summary>
		protected int _start;

		/// <summary>
		/// 	The current tag.
		/// </summary>
		protected LexerTag _tag;

		/// <summary>
		/// 	The string in this token.
		/// </summary>
		protected string _text;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Gets the starting startingPosition of the string.
		/// </summary>
		public int IndexStart
		{
			get
			{
				return (this._start);
			}
		}

		/// <summary>
		/// 	Gets the tag value
		/// </summary>
		public LexerTag Tag
		{
			get
			{
				return (this._tag);
			}
		}

		/// <summary>
		/// 	Gets the string value.
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
			LexToken token;

			if (obj == null || !(obj is LexToken))
				return (false);

			token = (LexToken)obj;

			return (token.Text == this.Text &&
			        token.IndexStart == this.IndexStart &&
			        token.Tag == this.Tag);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return (String.Format("Token=([{0}], Index={1}, Tag={2})",
			                      this.Text, this.IndexStart, this.Tag));
		}

		#endregion
	}
}