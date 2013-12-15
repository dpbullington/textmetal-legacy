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

using System;

namespace Newtonsoft.Json.Utilities
{
	/// <summary>
	/// Builds a string. Unlike StringBuilder this class lets you reuse it's internal buffer.
	/// </summary>
	internal class StringBuffer
	{
		#region Constructors/Destructors

		public StringBuffer()
		{
			this._buffer = EmptyBuffer;
		}

		public StringBuffer(int initalSize)
		{
			this._buffer = new char[initalSize];
		}

		#endregion

		#region Fields/Constants

		private static readonly char[] EmptyBuffer = new char[0];

		private char[] _buffer;
		private int _position;

		#endregion

		#region Properties/Indexers/Events

		public int Position
		{
			get
			{
				return this._position;
			}
			set
			{
				this._position = value;
			}
		}

		#endregion

		#region Methods/Operators

		public void Append(char value)
		{
			// test if the buffer array is large enough to take the value
			if (this._position == this._buffer.Length)
				this.EnsureSize(1);

			// set value and increment poisition
			this._buffer[this._position++] = value;
		}

		public void Append(char[] buffer, int startIndex, int count)
		{
			if (this._position + count >= this._buffer.Length)
				this.EnsureSize(count);

			Array.Copy(buffer, startIndex, this._buffer, this._position, count);

			this._position += count;
		}

		public void Clear()
		{
			this._buffer = EmptyBuffer;
			this._position = 0;
		}

		private void EnsureSize(int appendLength)
		{
			char[] newBuffer = new char[(this._position + appendLength) * 2];

			Array.Copy(this._buffer, newBuffer, this._position);

			this._buffer = newBuffer;
		}

		public char[] GetInternalBuffer()
		{
			return this._buffer;
		}

		public override string ToString()
		{
			return this.ToString(0, this._position);
		}

		public string ToString(int start, int length)
		{
			// TODO: validation
			return new string(this._buffer, start, length);
		}

		#endregion
	}
}