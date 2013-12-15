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

using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	/// <summary>
	/// Represents a reader that provides fast, non-cached, forward-only access to serialized Json data.
	/// </summary>
	public class JTokenReader : JsonReader, IJsonLineInfo
	{
		private readonly JToken _root;
		private JToken _parent;
		private JToken _current;

		/// <summary>
		/// Initializes a new instance of the <see cref="JTokenReader" /> class.
		/// </summary>
		/// <param name="token"> The token to read from. </param>
		public JTokenReader(JToken token)
		{
			ValidationUtils.ArgumentNotNull(token, "token");

			this._root = token;
			this._current = token;
		}

		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="T:Byte[]" />.
		/// </summary>
		/// <returns>
		/// A <see cref="T:Byte[]" /> or a null reference if the next JSON token is null. This method will return <c> null </c> at the end of an array.
		/// </returns>
		public override byte[] ReadAsBytes()
		{
			return this.ReadAsBytesInternal();
		}

		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="Nullable{Decimal}" />.
		/// </summary>
		/// <returns> A <see cref="Nullable{Decimal}" />. This method will return <c> null </c> at the end of an array. </returns>
		public override decimal? ReadAsDecimal()
		{
			return this.ReadAsDecimalInternal();
		}

		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="Nullable{Int32}" />.
		/// </summary>
		/// <returns> A <see cref="Nullable{Int32}" />. This method will return <c> null </c> at the end of an array. </returns>
		public override int? ReadAsInt32()
		{
			return this.ReadAsInt32Internal();
		}

		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="String" />.
		/// </summary>
		/// <returns> A <see cref="String" />. This method will return <c> null </c> at the end of an array. </returns>
		public override string ReadAsString()
		{
			return this.ReadAsStringInternal();
		}

		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="Nullable{DateTime}" />.
		/// </summary>
		/// <returns> A <see cref="String" />. This method will return <c> null </c> at the end of an array. </returns>
		public override DateTime? ReadAsDateTime()
		{
			return this.ReadAsDateTimeInternal();
		}

#if !NET20
		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="Nullable{DateTimeOffset}" />.
		/// </summary>
		/// <returns> A <see cref="Nullable{DateTimeOffset}" />. This method will return <c> null </c> at the end of an array. </returns>
		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			return this.ReadAsDateTimeOffsetInternal();
		}
#endif

		internal override bool ReadInternal()
		{
			if (this.CurrentState != State.Start)
			{
				JContainer container = this._current as JContainer;
				if (container != null && this._parent != container)
					return this.ReadInto(container);
				else
					return this.ReadOver(this._current);
			}

			this.SetToken(this._current);
			return true;
		}

		/// <summary>
		/// Reads the next JSON token from the stream.
		/// </summary>
		/// <returns>
		/// true if the next token was read successfully; false if there are no more tokens to read.
		/// </returns>
		public override bool Read()
		{
			this._readType = ReadType.Read;

			return this.ReadInternal();
		}

		private bool ReadOver(JToken t)
		{
			if (t == this._root)
				return this.ReadToEnd();

			JToken next = t.Next;
			if ((next == null || next == t) || t == t.Parent.Last)
			{
				if (t.Parent == null)
					return this.ReadToEnd();

				return this.SetEnd(t.Parent);
			}
			else
			{
				this._current = next;
				this.SetToken(this._current);
				return true;
			}
		}

		private bool ReadToEnd()
		{
			this.SetToken(JsonToken.None);
			return false;
		}

		private bool IsEndElement
		{
			get
			{
				return (this._current == this._parent);
			}
		}

		private JsonToken? GetEndToken(JContainer c)
		{
			switch (c.Type)
			{
				case JTokenType.Object:
					return JsonToken.EndObject;
				case JTokenType.Array:
					return JsonToken.EndArray;
				case JTokenType.Constructor:
					return JsonToken.EndConstructor;
				case JTokenType.Property:
					return null;
				default:
					throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", c.Type, "Unexpected JContainer type.");
			}
		}

		private bool ReadInto(JContainer c)
		{
			JToken firstChild = c.First;
			if (firstChild == null)
				return this.SetEnd(c);
			else
			{
				this.SetToken(firstChild);
				this._current = firstChild;
				this._parent = c;
				return true;
			}
		}

		private bool SetEnd(JContainer c)
		{
			JsonToken? endToken = this.GetEndToken(c);
			if (endToken != null)
			{
				SetToken(endToken.Value);
				this._current = c;
				this._parent = c;
				return true;
			}
			else
				return this.ReadOver(c);
		}

		private void SetToken(JToken token)
		{
			switch (token.Type)
			{
				case JTokenType.Object:
					this.SetToken(JsonToken.StartObject);
					break;
				case JTokenType.Array:
					this.SetToken(JsonToken.StartArray);
					break;
				case JTokenType.Constructor:
					this.SetToken(JsonToken.StartConstructor);
					break;
				case JTokenType.Property:
					this.SetToken(JsonToken.PropertyName, ((JProperty)token).Name);
					break;
				case JTokenType.Comment:
					this.SetToken(JsonToken.Comment, ((JValue)token).Value);
					break;
				case JTokenType.Integer:
					this.SetToken(JsonToken.Integer, ((JValue)token).Value);
					break;
				case JTokenType.Float:
					this.SetToken(JsonToken.Float, ((JValue)token).Value);
					break;
				case JTokenType.String:
					this.SetToken(JsonToken.String, ((JValue)token).Value);
					break;
				case JTokenType.Boolean:
					this.SetToken(JsonToken.Boolean, ((JValue)token).Value);
					break;
				case JTokenType.Null:
					this.SetToken(JsonToken.Null, ((JValue)token).Value);
					break;
				case JTokenType.Undefined:
					this.SetToken(JsonToken.Undefined, ((JValue)token).Value);
					break;
				case JTokenType.Date:
					this.SetToken(JsonToken.Date, ((JValue)token).Value);
					break;
				case JTokenType.Raw:
					this.SetToken(JsonToken.Raw, ((JValue)token).Value);
					break;
				case JTokenType.Bytes:
					this.SetToken(JsonToken.Bytes, ((JValue)token).Value);
					break;
				case JTokenType.Guid:
					this.SetToken(JsonToken.String, this.SafeToString(((JValue)token).Value));
					break;
				case JTokenType.Uri:
					this.SetToken(JsonToken.String, this.SafeToString(((JValue)token).Value));
					break;
				case JTokenType.TimeSpan:
					this.SetToken(JsonToken.String, this.SafeToString(((JValue)token).Value));
					break;
				default:
					throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", token.Type, "Unexpected JTokenType.");
			}
		}

		private string SafeToString(object value)
		{
			return (value != null) ? value.ToString() : null;
		}

		bool IJsonLineInfo.HasLineInfo()
		{
			if (this.CurrentState == State.Start)
				return false;

			IJsonLineInfo info = this.IsEndElement ? null : this._current;
			return (info != null && info.HasLineInfo());
		}

		int IJsonLineInfo.LineNumber
		{
			get
			{
				if (this.CurrentState == State.Start)
					return 0;

				IJsonLineInfo info = this.IsEndElement ? null : this._current;
				if (info != null)
					return info.LineNumber;

				return 0;
			}
		}

		int IJsonLineInfo.LinePosition
		{
			get
			{
				if (this.CurrentState == State.Start)
					return 0;

				IJsonLineInfo info = this.IsEndElement ? null : this._current;
				if (info != null)
					return info.LinePosition;

				return 0;
			}
		}
	}
}