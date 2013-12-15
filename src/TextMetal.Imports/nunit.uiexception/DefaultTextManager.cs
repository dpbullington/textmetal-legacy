// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.UiException
{
	/// <summary>
	/// This is a default implementation of ITextManager interface.
	/// </summary>
	public class DefaultTextManager :
		IEnumerable,
		ITextManager
	{
		#region Constructors/Destructors

		/// <summary>
		/// Builds a new instance of TextManager.
		/// </summary>
		public DefaultTextManager()
		{
			this._lines = new List<string>();
			this.Text = "";

			return;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Array of strings where each string is a line in this text.
		/// </summary>
		private List<string> _lines;

		/// <summary>
		/// Stores the character count of the longest line in this text.
		/// </summary>
		private int _maxLength;

		/// <summary>
		/// Hold the text to be managed by this instance.
		/// </summary>
		private string _text;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the number of lines in the text.
		/// </summary>
		public int LineCount
		{
			get
			{
				return (this._lines.Count);
			}
		}

		/// <summary>
		/// Gets the character count of the longest line in this text.
		/// </summary>
		public int MaxLength
		{
			get
			{
				return (this._maxLength);
			}
		}

		/// <summary>
		/// Gets or sets the text to be managed by this object.
		/// </summary>
		public string Text
		{
			get
			{
				return (this._text);
			}
			set
			{
				if (value == null)
					value = "";

				this._text = value;
				this._populateLineCollection(value);
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Gets an IEnumerator that iterate through each line of the
		/// current text.
		/// </summary>
		/// <returns> An IEnumerator that iterate through each line of this text. </returns>
		public IEnumerator GetEnumerator()
		{
			return (this._lines.GetEnumerator());
		}

		/// <summary>
		/// Gets the line of text at the specified startingPosition.
		/// (zero based startingPosition).
		/// </summary>
		/// <param name="lineIndex"> The startingPosition of the line to get. </param>
		/// <returns> A string that represents the content of the specified line without the trailing characters. </returns>
		public string GetTextAt(int lineIndex)
		{
			return (this._lines[lineIndex]);
		}

		/// <summary>
		/// setup member data with the input text.
		/// </summary>
		private void _populateLineCollection(string text)
		{
			string line;
			int textIndex;
			int newIndex;

			textIndex = 0;

			this._lines.Clear();
			this._maxLength = 0;

			while ((newIndex = text.IndexOf("\n", textIndex, StringComparison.Ordinal)) > textIndex)
			{
				line = text.Substring(textIndex, newIndex - textIndex).TrimEnd();
				this._maxLength = Math.Max(this._maxLength, line.Length);

				this._lines.Add(line);
				textIndex = newIndex + 1;
			}

			if (textIndex < text.Length)
			{
				line = text.Substring(textIndex).TrimEnd();
				this._maxLength = Math.Max(this._maxLength, line.Length);
				this._lines.Add(line);
			}

			return;
		}

		#endregion
	}
}