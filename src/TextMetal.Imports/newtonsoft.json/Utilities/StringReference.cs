#region License

// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

namespace Newtonsoft.Json.Utilities
{
	internal struct StringReference
	{
		#region Constructors/Destructors

		public StringReference(char[] chars, int startIndex, int length)
		{
			this._chars = chars;
			this._startIndex = startIndex;
			this._length = length;
		}

		#endregion

		#region Fields/Constants

		private readonly char[] _chars;
		private readonly int _length;
		private readonly int _startIndex;

		#endregion

		#region Properties/Indexers/Events

		public char[] Chars
		{
			get
			{
				return this._chars;
			}
		}

		public int Length
		{
			get
			{
				return this._length;
			}
		}

		public int StartIndex
		{
			get
			{
				return this._startIndex;
			}
		}

		#endregion

		#region Methods/Operators

		public override string ToString()
		{
			return new string(this._chars, this._startIndex, this._length);
		}

		#endregion
	}
}