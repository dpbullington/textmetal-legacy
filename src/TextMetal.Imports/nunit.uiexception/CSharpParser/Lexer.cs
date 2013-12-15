// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.UiException.CodeFormatters
{
	/// <summary>
	/// Splits a text formatted as C# code into a list of identified tokens.
	/// </summary>
	public class Lexer
	{
		#region Constructors/Destructors

		/// <summary>
		/// Builds a new instance of Lexer.
		/// </summary>
		public Lexer()
		{
			this._position = 0;
			this._text = "";

			this._dictionary = new TokenDictionary();
			this._dictionary.Add("/*", LexerTag.CommentC_Open);
			this._dictionary.Add("*/", LexerTag.CommentC_Close);
			this._dictionary.Add("//", LexerTag.CommentCpp);

			// Here: definition of one lengthed sequences
			this._dictionary.Add("\\", LexerTag.Text);
			this._dictionary.Add(" ", LexerTag.Separator);
			this._dictionary.Add("\t", LexerTag.Separator);
			this._dictionary.Add("\r", LexerTag.Separator);
			this._dictionary.Add(".", LexerTag.Separator);
			this._dictionary.Add(";", LexerTag.Separator);
			this._dictionary.Add("[", LexerTag.Separator);
			this._dictionary.Add("]", LexerTag.Separator);
			this._dictionary.Add("(", LexerTag.Separator);
			this._dictionary.Add(")", LexerTag.Separator);
			this._dictionary.Add("#", LexerTag.Separator);
			this._dictionary.Add(":", LexerTag.Separator);
			this._dictionary.Add("<", LexerTag.Separator);
			this._dictionary.Add(">", LexerTag.Separator);
			this._dictionary.Add("=", LexerTag.Separator);
			this._dictionary.Add(",", LexerTag.Separator);
			this._dictionary.Add("\n", LexerTag.EndOfLine);
			this._dictionary.Add("'", LexerTag.SingleQuote);
			this._dictionary.Add("\"", LexerTag.DoubleQuote);

			return;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Holds pre-defined sequences.
		/// </summary>
		private TokenDictionary _dictionary;

		/// <summary>
		/// Reading position in the current text.
		/// </summary>
		private int _position;

		/// <summary>
		/// Text where to fetch tokens.
		/// </summary>
		private string _text;

		/// <summary>
		/// Last identified token.
		/// </summary>
		private InternalToken _token;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the token identifed after a call to Next().
		/// The value may be null if the end of the text has been reached.
		/// </summary>
		public LexToken CurrentToken
		{
			get
			{
				return (this._token);
			}
		}

		public TokenDictionary Dictionary
		{
			get
			{
				return (this._dictionary);
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Clear all previously defined sequences.
		/// </summary>
		protected void Clear()
		{
			this._dictionary = new TokenDictionary();

			return;
		}

		/// <summary>
		/// Checks whether there are none visited tokens.
		/// </summary>
		public bool HasNext()
		{
			return (this._position < this._text.Length);
		}

		/// <summary>
		/// Call this method to visit iteratively all tokens in the source text.
		/// Each time a token has been identifier, the method returns true and the
		/// identified Token is place under the CurrentToken property.
		/// When there is not more token to visit, the method returns false
		/// and null is set in CurrentToken property.
		/// </summary>
		public bool Next()
		{
			char c;
			LexToken token;
			string prediction;
			int pos;
			int count;
			int prediction_length;

			this._token = null;

			if (!this.HasNext())
				return (false);

			pos = this._position;
			this._token = new InternalToken(pos);
			prediction_length = this._dictionary[0].Text.Length;

			while (pos < this._text.Length)
			{
				c = this._text[pos];
				this._token.AppendsChar(c);

				prediction = "";
				if (pos + 1 < this._text.Length)
				{
					count = Math.Min(prediction_length, this._text.Length - pos - 1);
					prediction = this._text.Substring(pos + 1, count);
				}

				token = this._dictionary.TryMatch(this._token.Text, prediction);

				if (token != null)
				{
					this._token.SetText(token.Text);
					this._token.SetIndex(this._position);
					this._token.SetLexerTag(token.Tag);
					this._position += this._token.Text.Length;

					break;
				}

				pos++;
			}

			return (true);
		}

		/// <summary>
		/// Setup the text to be splitted in tokens.
		/// Client code must call Next() first before getting
		/// the first available token (if any).
		/// </summary>
		public void Parse(string codeCSharp)
		{
			UiExceptionHelper.CheckNotNull(codeCSharp, "text");

			this._text = codeCSharp;
			this._position = 0;

			return;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class InternalToken :
			LexToken
		{
			#region Constructors/Destructors

			/// <summary>
			/// Builds a concrete instance of LexToken. By default, created instance
			/// are setup with LexerTag.Text value.
			/// </summary>
			/// <param name="startingPosition"> The starting startingPosition of this token in the text. </param>
			public InternalToken(int index)
			{
				this._tag = LexerTag.Text;
				this._text = "";
				this._start = index;

				return;
			}

			#endregion

			#region Methods/Operators

			/// <summary>
			/// Appends this character to this token.
			/// </summary>
			public void AppendsChar(char c)
			{
				this._text += c;
			}

			/// <summary>
			/// Removes the "count" ending character of this token.
			/// </summary>
			public void PopChars(int count)
			{
				this._text = this._text.Remove(this._text.Length - count);
			}

			public void SetIndex(int index)
			{
				this._start = index;
			}

			/// <summary>
			/// Set a new value to the Tag property.
			/// </summary>
			public void SetLexerTag(LexerTag tag)
			{
				this._tag = tag;
			}

			public void SetText(string text)
			{
				this._text = text;
			}

			#endregion
		}

		#endregion
	}
}