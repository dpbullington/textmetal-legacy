// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections.Generic;

namespace NUnit.UiException.CodeFormatters
{
	/// <summary>
	/// 	(Formerly named CSCode)
	/// 
	/// 	Implements ITextManager and adds new behaviors to provide support for basic
	/// 	syntax coloring.
	/// </summary>
	public class FormattedCode :
		ITextManager
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Builds a new instance of FormattedCode.
		/// </summary>
		public FormattedCode()
		{
			this._codeInfo = NewCodeInfo();
			this._maxLength = 0;

			return;
		}

		public FormattedCode(string csharpText, int[] strIndexes, byte[] tagValues, int[] lineIndexes)
		{
			UiExceptionHelper.CheckNotNull(csharpText, "csharpText");
			UiExceptionHelper.CheckNotNull(strIndexes, "strIndexes");
			UiExceptionHelper.CheckNotNull(tagValues, "tagValues");
			UiExceptionHelper.CheckNotNull(lineIndexes, "lineIndexes");

			this._codeInfo = new CodeInfo();

			this._codeInfo.Text = csharpText;

			this._codeInfo.IndexArray = new List<int>();
			foreach (int index in strIndexes)
				this._codeInfo.IndexArray.Add(index);

			this._codeInfo.TagArray = new List<byte>();
			foreach (byte tag in tagValues)
				this._codeInfo.TagArray.Add(tag);

			this._codeInfo.LineArray = new List<int>();
			foreach (int line in lineIndexes)
				this._codeInfo.LineArray.Add(line);

			return;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// 	Keeps tracks of the text and the data used by the syntax coloring feature.
		/// </summary>
		protected CodeInfo _codeInfo;

		/// <summary>
		/// 	Stores the character count of the longest line in this text.
		/// </summary>
		private int _maxLength;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Gives access to the collection of ClassifiedToken at the specified lineIndex.
		/// </summary>
		/// <param name="lineIndex"> A zero based startingPosition. </param>
		/// <returns> The ClassifiedTokenCollection instance at this startingPosition. </returns>
		public ClassifiedTokenCollection this[int lineIndex]
		{
			get
			{
				return (new InternalCSTokenCollection(this._codeInfo, lineIndex));
			}
		}

		public static FormattedCode Empty
		{
			get
			{
				return (new FormattedCode());
			}
		}

		/// <summary>
		/// 	Gets the line count in the text currently
		/// 	managed by this object.
		/// </summary>
		public int LineCount
		{
			get
			{
				return (this._codeInfo.LineArray.Count);
			}
		}

		/// <summary>
		/// 	Gets the character count of the longest line
		/// 	in this text.
		/// </summary>
		public int MaxLength
		{
			get
			{
				int i;

				if (this._maxLength == 0)
				{
					for (i = 0; i < this.LineCount; ++i)
						this._maxLength = Math.Max(this._maxLength, this[i].Text.TrimEnd().Length);
				}

				return (this._maxLength);
			}
		}

		/// <summary>
		/// 	Gets the text currently managed by this object.
		/// </summary>
		public string Text
		{
			get
			{
				return (this._codeInfo.Text);
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	An utility method that check data consistency. The operation
		/// 	raises an exception if an error is found.
		/// </summary>
		public static void CheckData(FormattedCode data)
		{
			List<int> lines;
			int line;
			int bound;
			int i;

			UiExceptionHelper.CheckNotNull(data, "data");

			UiExceptionHelper.CheckTrue(
				data._codeInfo.IndexArray.Count == data._codeInfo.TagArray.Count,
				"IndexArray.Count and TagArray.Count must match.",
				"data");

			bound = data._codeInfo.IndexArray.Count;
			lines = data._codeInfo.LineArray;
			for (i = 0; i < lines.Count; ++i)
			{
				line = lines[i];

				UiExceptionHelper.CheckTrue(
					line >= 0 && line < bound,
					"Bad LineArray value at index " + i + ", value was: " + line + ", expected to be in: [0-" + bound + "[.",
					"data");

				if (i == 0)
					continue;

				UiExceptionHelper.CheckTrue(
					lines[i] > lines[i - 1],
					"Bad LineArray[" + i + "], value was: " + line + ", expected to be > than LineArray[" + (i - 1) + "]=" + lines[i - 1] + ".",
					"data");
			}

			return;
		}

		/// <summary>
		/// 	Builds a new instance of CodeInfo.
		/// </summary>
		public static CodeInfo NewCodeInfo()
		{
			CodeInfo res;

			res = new CodeInfo();
			res.Text = "";
			res.IndexArray = new List<int>();
			res.LineArray = new List<int>();
			res.TagArray = new List<byte>();

			return (res);
		}

		public CodeInfo CopyInfo()
		{
			FormattedCode copy;

			copy = new FormattedCode(this._codeInfo.Text,
			                         this._codeInfo.IndexArray.ToArray(),
			                         this._codeInfo.TagArray.ToArray(),
			                         this._codeInfo.LineArray.ToArray());

			return (copy._codeInfo);
		}

		public override bool Equals(object obj)
		{
			FormattedCode arg;
			int i;

			if (obj == null ||
			    !(obj is FormattedCode))
				return (false);

			arg = obj as FormattedCode;

			if (arg._codeInfo.Text != this._codeInfo.Text ||
			    arg._codeInfo.IndexArray.Count != this._codeInfo.IndexArray.Count ||
			    arg._codeInfo.TagArray.Count != this._codeInfo.TagArray.Count ||
			    arg._codeInfo.LineArray.Count != this._codeInfo.LineArray.Count)
				return (false);

			for (i = 0; i < arg._codeInfo.IndexArray.Count; ++i)
			{
				if (arg._codeInfo.IndexArray[i] != this._codeInfo.IndexArray[i])
					return (false);
			}

			for (i = 0; i < arg._codeInfo.TagArray.Count; ++i)
			{
				if (arg._codeInfo.TagArray[i] != this._codeInfo.TagArray[i])
					return (false);
			}

			for (i = 0; i < arg._codeInfo.LineArray.Count; ++i)
			{
				if (arg._codeInfo.LineArray[i] != this._codeInfo.LineArray[i])
					return (false);
			}

			return (true);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// 	Gets the text at the specified line.
		/// </summary>
		/// <param name="lineIndex"> A zero based startingPosition. </param>
		/// <returns> The text at the specified line startingPosition. </returns>
		public string GetTextAt(int lineIndex)
		{
			return (this[lineIndex].Text);
		}

		public override string ToString()
		{
			string res;
			string index_array;
			string tag_array;
			string line_array;
			int i;

			index_array = "";
			for (i = 0; i < this._codeInfo.IndexArray.Count; ++i)
			{
				if (i > 0)
					index_array += ", ";
				index_array += "" + this._codeInfo.IndexArray[i];
			}

			tag_array = "";
			for (i = 0; i < this._codeInfo.TagArray.Count; ++i)
			{
				if (i > 0)
					tag_array += ", ";
				tag_array += "" + this._codeInfo.TagArray[i];
			}

			line_array = "";
			for (i = 0; i < this._codeInfo.LineArray.Count; ++i)
			{
				if (i > 0)
					line_array += ", ";
				line_array += "" + this._codeInfo.LineArray[i];
			}

			res = String.Format(
				"FormattedCode: [(text=[{0}], len={1}), (startingPosition=[{2}]), (tag=[{3}]), (line=[{4}])]",
				this._codeInfo.Text, this._codeInfo.Text.Length,
				index_array, tag_array, line_array);

			return (res);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// 	A naive attempt to modelize a structure of data that manages the text to be
		/// 	displayed and extra data to make basic syntax coloring at rendering time,
		/// 	while keeping a low memory footprint.
		/// 
		/// 	At rendering time, CodeInfo is used to make a link between the text to be drawn
		/// 	and the brush color to be used during the process. So it is possible to distinguishes
		/// 	code, comments, keywords and strings.
		/// 	Currently, CodeInfo is used to split the text into a collection of couple of data,
		/// 	where each couple is composed of:
		/// 	- a string of characters
		/// 	- a value (called tag) that classifies this string from 0 to 3.
		/// 	Where 0 corresponds to 'Code', 1 to 'Keyword' and so on.
		/// 	These couples are named 'ClassifiedToken'. At rendering time, the process can link each
		/// 	of these values to a particular System.Drawing.Brush instance and display text
		/// 	differently.
		///  
		/// 	However, keeping track of all these couples at any time could have a significative
		/// 	impact on memory especially for big files. Therefore, instead of storing all theses couples,
		/// 	CodeInfo stores just primitive information that allow to build ClassifiedToken instances on the fly.
		/// </summary>
		public class CodeInfo
		{
			#region Fields/Constants

			/// <summary>
			/// 	Array of character indexes that refers to
			/// 	the field "Text". Each value in this array
			/// 	must be interpreted as the starting index position
			/// 	in the string into Text.
			/// </summary>
			public List<int> IndexArray;

			/// <summary>
			/// 	This index_array is used to easily locate the start of each
			/// 	line of text, for instance: startingPosition[0] refers to line startingPosition 0,
			/// 	startingPosition[1] refers to line startingPosition 1 and so on...
			/// 	However, there is a small indirection in that this index_array
			/// 	doesn't directly hold string indexes but refers to the startingPosition
			/// 	of the item to be used in 'IndexArray'.
			/// 	Therefore, LineArray[0] gives access to the startingPosition of the
			/// 	IndexArray's item to be used to get the corresponding character
			/// 	position. Hence, line 0 starts at: IndexArray[LineArray[0]]
			/// 	line 1: IndexArray[LineArray[1]] and so on...
			/// </summary>
			public List<int> LineArray;

			/// <summary>
			/// 	Array of ClassificationTag values held as a
			/// 	byte array. There is a one-to-one correspondance
			/// 	with 'IndexArray'. i.e.: TagArray[0] refers to the
			/// 	ClassificationTag value for string sequence at
			/// 	IndexArray[0]. TagArray[1] refers value to IndexArray[1] and
			/// 	so on... Hence, the following condition:
			/// 	- IndexArray.Count == TagArray.Count must be verified.
			/// </summary>
			public List<byte> TagArray;

			/// <summary>
			/// 	Holds the entire text as a simple string.
			/// </summary>
			public string Text;

			#endregion
		}

		private class InternalCSTokenCollection :
			ClassifiedTokenCollection
		{
			#region Constructors/Destructors

			public InternalCSTokenCollection(CodeInfo info, int lineIndex)
			{
				this._info = info;
				this._lineIndex = lineIndex;

				return;
			}

			#endregion
		}

		#endregion
	}
}