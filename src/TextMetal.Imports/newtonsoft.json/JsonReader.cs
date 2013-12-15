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
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Newtonsoft.Json.Utilities;
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#else
using System.Linq;

#endif

namespace Newtonsoft.Json
{
	/// <summary>
	/// Represents a reader that provides fast, non-cached, forward-only access to serialized Json data.
	/// </summary>
	public abstract class JsonReader : IDisposable
	{
		/// <summary>
		/// Specifies the state of the reader.
		/// </summary>
		protected internal enum State
		{
			/// <summary>
			/// The Read method has not been called.
			/// </summary>
			Start,

			/// <summary>
			/// The end of the file has been reached successfully.
			/// </summary>
			Complete,

			/// <summary>
			/// Reader is at a property.
			/// </summary>
			Property,

			/// <summary>
			/// Reader is at the start of an object.
			/// </summary>
			ObjectStart,

			/// <summary>
			/// Reader is in an object.
			/// </summary>
			Object,

			/// <summary>
			/// Reader is at the start of an array.
			/// </summary>
			ArrayStart,

			/// <summary>
			/// Reader is in an array.
			/// </summary>
			Array,

			/// <summary>
			/// The Close method has been called.
			/// </summary>
			Closed,

			/// <summary>
			/// Reader has just read a value.
			/// </summary>
			PostValue,

			/// <summary>
			/// Reader is at the start of a constructor.
			/// </summary>
			ConstructorStart,

			/// <summary>
			/// Reader in a constructor.
			/// </summary>
			Constructor,

			/// <summary>
			/// An error occurred that prevents the read operation from continuing.
			/// </summary>
			Error,

			/// <summary>
			/// The end of the file has been reached successfully.
			/// </summary>
			Finished
		}

		// current Token data
		private JsonToken _tokenType;
		private object _value;
		internal char _quoteChar;
		internal State _currentState;
		internal ReadType _readType;
		private JsonPosition _currentPosition;
		private CultureInfo _culture;
		private DateTimeZoneHandling _dateTimeZoneHandling;
		private int? _maxDepth;
		private bool _hasExceededMaxDepth;
		internal DateParseHandling _dateParseHandling;
		internal FloatParseHandling _floatParseHandling;
		private readonly List<JsonPosition> _stack;

		/// <summary>
		/// Gets the current reader state.
		/// </summary>
		/// <value> The current reader state. </value>
		protected State CurrentState
		{
			get
			{
				return this._currentState;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the underlying stream or
		/// <see cref="TextReader" /> should be closed when the reader is closed.
		/// </summary>
		/// <value>
		/// true to close the underlying stream or <see cref="TextReader" /> when
		/// the reader is closed; otherwise false. The default is true.
		/// </value>
		public bool CloseInput
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the quotation mark character used to enclose the value of a string.
		/// </summary>
		public virtual char QuoteChar
		{
			get
			{
				return this._quoteChar;
			}
			protected internal set
			{
				this._quoteChar = value;
			}
		}

		/// <summary>
		/// Get or set how <see cref="DateTime" /> time zones are handling when reading JSON.
		/// </summary>
		public DateTimeZoneHandling DateTimeZoneHandling
		{
			get
			{
				return this._dateTimeZoneHandling;
			}
			set
			{
				this._dateTimeZoneHandling = value;
			}
		}

		/// <summary>
		/// Get or set how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed when reading JSON.
		/// </summary>
		public DateParseHandling DateParseHandling
		{
			get
			{
				return this._dateParseHandling;
			}
			set
			{
				this._dateParseHandling = value;
			}
		}

		/// <summary>
		/// Get or set how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
		/// </summary>
		public FloatParseHandling FloatParseHandling
		{
			get
			{
				return this._floatParseHandling;
			}
			set
			{
				this._floatParseHandling = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a <see cref="JsonReaderException" />.
		/// </summary>
		public int? MaxDepth
		{
			get
			{
				return this._maxDepth;
			}
			set
			{
				if (value <= 0)
					throw new ArgumentException("Value must be positive.", "value");

				this._maxDepth = value;
			}
		}

		/// <summary>
		/// Gets the type of the current JSON token.
		/// </summary>
		public virtual JsonToken TokenType
		{
			get
			{
				return this._tokenType;
			}
		}

		/// <summary>
		/// Gets the text value of the current JSON token.
		/// </summary>
		public virtual object Value
		{
			get
			{
				return this._value;
			}
		}

		/// <summary>
		/// Gets The Common Language Runtime (CLR) type for the current JSON token.
		/// </summary>
		public virtual Type ValueType
		{
			get
			{
				return (this._value != null) ? this._value.GetType() : null;
			}
		}

		/// <summary>
		/// Gets the depth of the current token in the JSON document.
		/// </summary>
		/// <value> The depth of the current token in the JSON document. </value>
		public virtual int Depth
		{
			get
			{
				int depth = this._stack.Count;
				if (IsStartToken(this.TokenType) || this._currentPosition.Type == JsonContainerType.None)
					return depth;
				else
					return depth + 1;
			}
		}

		/// <summary>
		/// Gets the path of the current JSON token.
		/// </summary>
		public virtual string Path
		{
			get
			{
				if (this._currentPosition.Type == JsonContainerType.None)
					return string.Empty;

				bool insideContainer = (this._currentState != State.ArrayStart
										&& this._currentState != State.ConstructorStart
										&& this._currentState != State.ObjectStart);

				IEnumerable<JsonPosition> positions = (!insideContainer)
					? this._stack
					: this._stack.Concat(new[] { this._currentPosition });

				return JsonPosition.BuildPath(positions);
			}
		}

		/// <summary>
		/// Gets or sets the culture used when reading JSON. Defaults to <see cref="CultureInfo.InvariantCulture" />.
		/// </summary>
		public CultureInfo Culture
		{
			get
			{
				return this._culture ?? CultureInfo.InvariantCulture;
			}
			set
			{
				this._culture = value;
			}
		}

		internal JsonPosition GetPosition(int depth)
		{
			if (depth < this._stack.Count)
				return this._stack[depth];

			return this._currentPosition;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonReader" /> class with the specified <see cref="TextReader" />.
		/// </summary>
		protected JsonReader()
		{
			this._currentState = State.Start;
			this._stack = new List<JsonPosition>(4);
			this._dateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
			this._dateParseHandling = DateParseHandling.DateTime;
			this._floatParseHandling = FloatParseHandling.Double;

			this.CloseInput = true;
		}

		private void Push(JsonContainerType value)
		{
			this.UpdateScopeWithFinishedValue();

			if (this._currentPosition.Type == JsonContainerType.None)
				this._currentPosition = new JsonPosition(value);
			else
			{
				this._stack.Add(this._currentPosition);
				this._currentPosition = new JsonPosition(value);

				// this is a little hacky because Depth increases when first property/value is written but only testing here is faster/simpler
				if (this._maxDepth != null && this.Depth + 1 > this._maxDepth && !this._hasExceededMaxDepth)
				{
					this._hasExceededMaxDepth = true;
					throw JsonReaderException.Create(this, "The reader's MaxDepth of {0} has been exceeded.".FormatWith(CultureInfo.InvariantCulture, this._maxDepth));
				}
			}
		}

		private JsonContainerType Pop()
		{
			JsonPosition oldPosition;
			if (this._stack.Count > 0)
			{
				oldPosition = this._currentPosition;
				this._currentPosition = this._stack[this._stack.Count - 1];
				this._stack.RemoveAt(this._stack.Count - 1);
			}
			else
			{
				oldPosition = this._currentPosition;
				this._currentPosition = new JsonPosition();
			}

			if (this._maxDepth != null && this.Depth <= this._maxDepth)
				this._hasExceededMaxDepth = false;

			return oldPosition.Type;
		}

		private JsonContainerType Peek()
		{
			return this._currentPosition.Type;
		}

		/// <summary>
		/// Reads the next JSON token from the stream.
		/// </summary>
		/// <returns> true if the next token was read successfully; false if there are no more tokens to read. </returns>
		public abstract bool Read();

		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="Nullable{Int32}" />.
		/// </summary>
		/// <returns> A <see cref="Nullable{Int32}" />. This method will return <c> null </c> at the end of an array. </returns>
		public abstract int? ReadAsInt32();

		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="String" />.
		/// </summary>
		/// <returns> A <see cref="String" />. This method will return <c> null </c> at the end of an array. </returns>
		public abstract string ReadAsString();

		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="T:Byte[]" />.
		/// </summary>
		/// <returns> A <see cref="T:Byte[]" /> or a null reference if the next JSON token is null. This method will return <c> null </c> at the end of an array. </returns>
		public abstract byte[] ReadAsBytes();

		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="Nullable{Decimal}" />.
		/// </summary>
		/// <returns> A <see cref="Nullable{Decimal}" />. This method will return <c> null </c> at the end of an array. </returns>
		public abstract decimal? ReadAsDecimal();

		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="Nullable{DateTime}" />.
		/// </summary>
		/// <returns> A <see cref="String" />. This method will return <c> null </c> at the end of an array. </returns>
		public abstract DateTime? ReadAsDateTime();

#if !NET20
		/// <summary>
		/// Reads the next JSON token from the stream as a <see cref="Nullable{DateTimeOffset}" />.
		/// </summary>
		/// <returns> A <see cref="Nullable{DateTimeOffset}" />. This method will return <c> null </c> at the end of an array. </returns>
		public abstract DateTimeOffset? ReadAsDateTimeOffset();
#endif

		internal virtual bool ReadInternal()
		{
			throw new NotImplementedException();
		}

#if !NET20
		internal DateTimeOffset? ReadAsDateTimeOffsetInternal()
		{
			this._readType = ReadType.ReadAsDateTimeOffset;

			JsonToken t;

			do
			{
				if (!this.ReadInternal())
				{
					this.SetToken(JsonToken.None);
					return null;
				}
				else
					t = this.TokenType;
			}
			while (t == JsonToken.Comment);

			if (t == JsonToken.Date)
			{
				if (this.Value is DateTime)
					this.SetToken(JsonToken.Date, new DateTimeOffset((DateTime)this.Value));

				return (DateTimeOffset)this.Value;
			}

			if (t == JsonToken.Null)
				return null;

			DateTimeOffset dt;
			if (t == JsonToken.String)
			{
				string s = (string)this.Value;
				if (string.IsNullOrEmpty(s))
				{
					this.SetToken(JsonToken.Null);
					return null;
				}

				if (DateTimeOffset.TryParse(s, this.Culture, DateTimeStyles.RoundtripKind, out dt))
				{
					this.SetToken(JsonToken.Date, dt);
					return dt;
				}
				else
					throw JsonReaderException.Create(this, "Could not convert string to DateTimeOffset: {0}.".FormatWith(CultureInfo.InvariantCulture, this.Value));
			}

			if (t == JsonToken.EndArray)
				return null;

			throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, t));
		}
#endif

		internal byte[] ReadAsBytesInternal()
		{
			this._readType = ReadType.ReadAsBytes;

			JsonToken t;

			do
			{
				if (!this.ReadInternal())
				{
					this.SetToken(JsonToken.None);
					return null;
				}
				else
					t = this.TokenType;
			}
			while (t == JsonToken.Comment);

			if (this.IsWrappedInTypeObject())
			{
				byte[] data = this.ReadAsBytes();
				this.ReadInternal();
				this.SetToken(JsonToken.Bytes, data);
				return data;
			}

			// attempt to convert possible base 64 string to bytes
			if (t == JsonToken.String)
			{
				string s = (string)this.Value;
				byte[] data = (s.Length == 0) ? new byte[0] : Convert.FromBase64String(s);
				this.SetToken(JsonToken.Bytes, data);
				return data;
			}

			if (t == JsonToken.Null)
				return null;

			if (t == JsonToken.Bytes)
				return (byte[])this.Value;

			if (t == JsonToken.StartArray)
			{
				List<byte> data = new List<byte>();

				while (this.ReadInternal())
				{
					t = this.TokenType;
					switch (t)
					{
						case JsonToken.Integer:
							data.Add(Convert.ToByte(this.Value, CultureInfo.InvariantCulture));
							break;
						case JsonToken.EndArray:
							byte[] d = data.ToArray();
							this.SetToken(JsonToken.Bytes, d);
							return d;
						case JsonToken.Comment:
							// skip
							break;
						default:
							throw JsonReaderException.Create(this, "Unexpected token when reading bytes: {0}.".FormatWith(CultureInfo.InvariantCulture, t));
					}
				}

				throw JsonReaderException.Create(this, "Unexpected end when reading bytes.");
			}

			if (t == JsonToken.EndArray)
				return null;

			throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, t));
		}

		internal decimal? ReadAsDecimalInternal()
		{
			this._readType = ReadType.ReadAsDecimal;

			JsonToken t;

			do
			{
				if (!this.ReadInternal())
				{
					this.SetToken(JsonToken.None);
					return null;
				}
				else
					t = this.TokenType;
			}
			while (t == JsonToken.Comment);

			if (t == JsonToken.Integer || t == JsonToken.Float)
			{
				if (!(this.Value is decimal))
					this.SetToken(JsonToken.Float, Convert.ToDecimal(this.Value, CultureInfo.InvariantCulture));

				return (decimal)this.Value;
			}

			if (t == JsonToken.Null)
				return null;

			decimal d;
			if (t == JsonToken.String)
			{
				string s = (string)this.Value;
				if (string.IsNullOrEmpty(s))
				{
					this.SetToken(JsonToken.Null);
					return null;
				}

				if (decimal.TryParse(s, NumberStyles.Number, this.Culture, out d))
				{
					this.SetToken(JsonToken.Float, d);
					return d;
				}
				else
					throw JsonReaderException.Create(this, "Could not convert string to decimal: {0}.".FormatWith(CultureInfo.InvariantCulture, this.Value));
			}

			if (t == JsonToken.EndArray)
				return null;

			throw JsonReaderException.Create(this, "Error reading decimal. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, t));
		}

		internal int? ReadAsInt32Internal()
		{
			this._readType = ReadType.ReadAsInt32;

			JsonToken t;

			do
			{
				if (!this.ReadInternal())
				{
					this.SetToken(JsonToken.None);
					return null;
				}
				else
					t = this.TokenType;
			}
			while (t == JsonToken.Comment);

			if (t == JsonToken.Integer || t == JsonToken.Float)
			{
				if (!(this.Value is int))
					this.SetToken(JsonToken.Integer, Convert.ToInt32(this.Value, CultureInfo.InvariantCulture));

				return (int)this.Value;
			}

			if (t == JsonToken.Null)
				return null;

			int i;
			if (t == JsonToken.String)
			{
				string s = (string)this.Value;
				if (string.IsNullOrEmpty(s))
				{
					this.SetToken(JsonToken.Null);
					return null;
				}

				if (int.TryParse(s, NumberStyles.Integer, this.Culture, out i))
				{
					this.SetToken(JsonToken.Integer, i);
					return i;
				}
				else
					throw JsonReaderException.Create(this, "Could not convert string to integer: {0}.".FormatWith(CultureInfo.InvariantCulture, this.Value));
			}

			if (t == JsonToken.EndArray)
				return null;

			throw JsonReaderException.Create(this, "Error reading integer. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
		}

		internal string ReadAsStringInternal()
		{
			this._readType = ReadType.ReadAsString;

			JsonToken t;

			do
			{
				if (!this.ReadInternal())
				{
					this.SetToken(JsonToken.None);
					return null;
				}
				else
					t = this.TokenType;
			}
			while (t == JsonToken.Comment);

			if (t == JsonToken.String)
				return (string)this.Value;

			if (t == JsonToken.Null)
				return null;

			if (IsPrimitiveToken(t))
			{
				if (this.Value != null)
				{
					string s;
					if (this.Value is IFormattable)
						s = ((IFormattable)this.Value).ToString(null, this.Culture);
					else
						s = this.Value.ToString();

					this.SetToken(JsonToken.String, s);
					return s;
				}
			}

			if (t == JsonToken.EndArray)
				return null;

			throw JsonReaderException.Create(this, "Error reading string. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, t));
		}

		internal DateTime? ReadAsDateTimeInternal()
		{
			this._readType = ReadType.ReadAsDateTime;

			do
			{
				if (!this.ReadInternal())
				{
					this.SetToken(JsonToken.None);
					return null;
				}
			}
			while (this.TokenType == JsonToken.Comment);

			if (this.TokenType == JsonToken.Date)
				return (DateTime)this.Value;

			if (this.TokenType == JsonToken.Null)
				return null;

			DateTime dt;
			if (this.TokenType == JsonToken.String)
			{
				string s = (string)this.Value;
				if (string.IsNullOrEmpty(s))
				{
					this.SetToken(JsonToken.Null);
					return null;
				}

				if (DateTime.TryParse(s, this.Culture, DateTimeStyles.RoundtripKind, out dt))
				{
					dt = DateTimeUtils.EnsureDateTime(dt, this.DateTimeZoneHandling);
					this.SetToken(JsonToken.Date, dt);
					return dt;
				}
				else
					throw JsonReaderException.Create(this, "Could not convert string to DateTime: {0}.".FormatWith(CultureInfo.InvariantCulture, this.Value));
			}

			if (this.TokenType == JsonToken.EndArray)
				return null;

			throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
		}

		private bool IsWrappedInTypeObject()
		{
			this._readType = ReadType.Read;

			if (this.TokenType == JsonToken.StartObject)
			{
				if (!this.ReadInternal())
					throw JsonReaderException.Create(this, "Unexpected end when reading bytes.");

				if (this.Value.ToString() == "$type")
				{
					this.ReadInternal();
					if (this.Value != null && this.Value.ToString().StartsWith("System.Byte[]"))
					{
						this.ReadInternal();
						if (this.Value.ToString() == "$value")
							return true;
					}
				}

				throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, JsonToken.StartObject));
			}

			return false;
		}

		/// <summary>
		/// Skips the children of the current token.
		/// </summary>
		public void Skip()
		{
			if (this.TokenType == JsonToken.PropertyName)
				this.Read();

			if (IsStartToken(this.TokenType))
			{
				int depth = this.Depth;

				while (this.Read() && (depth < this.Depth))
				{
				}
			}
		}

		/// <summary>
		/// Sets the current token.
		/// </summary>
		/// <param name="newToken"> The new token. </param>
		protected void SetToken(JsonToken newToken)
		{
			this.SetToken(newToken, null);
		}

		/// <summary>
		/// Sets the current token and value.
		/// </summary>
		/// <param name="newToken"> The new token. </param>
		/// <param name="value"> The value. </param>
		protected void SetToken(JsonToken newToken, object value)
		{
			this._tokenType = newToken;
			this._value = value;

			switch (newToken)
			{
				case JsonToken.StartObject:
					this._currentState = State.ObjectStart;
					this.Push(JsonContainerType.Object);
					break;
				case JsonToken.StartArray:
					this._currentState = State.ArrayStart;
					this.Push(JsonContainerType.Array);
					break;
				case JsonToken.StartConstructor:
					this._currentState = State.ConstructorStart;
					this.Push(JsonContainerType.Constructor);
					break;
				case JsonToken.EndObject:
					this.ValidateEnd(JsonToken.EndObject);
					break;
				case JsonToken.EndArray:
					this.ValidateEnd(JsonToken.EndArray);
					break;
				case JsonToken.EndConstructor:
					this.ValidateEnd(JsonToken.EndConstructor);
					break;
				case JsonToken.PropertyName:
					this._currentState = State.Property;

					this._currentPosition.PropertyName = (string)value;
					break;
				case JsonToken.Undefined:
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.Boolean:
				case JsonToken.Null:
				case JsonToken.Date:
				case JsonToken.String:
				case JsonToken.Raw:
				case JsonToken.Bytes:
					this._currentState = (this.Peek() != JsonContainerType.None) ? State.PostValue : State.Finished;

					this.UpdateScopeWithFinishedValue();
					break;
			}
		}

		private void UpdateScopeWithFinishedValue()
		{
			if (this._currentPosition.HasIndex)
				this._currentPosition.Position++;
		}

		private void ValidateEnd(JsonToken endToken)
		{
			JsonContainerType currentObject = this.Pop();

			if (this.GetTypeForCloseToken(endToken) != currentObject)
				throw JsonReaderException.Create(this, "JsonToken {0} is not valid for closing JsonType {1}.".FormatWith(CultureInfo.InvariantCulture, endToken, currentObject));

			this._currentState = (this.Peek() != JsonContainerType.None) ? State.PostValue : State.Finished;
		}

		/// <summary>
		/// Sets the state based on current token type.
		/// </summary>
		protected void SetStateBasedOnCurrent()
		{
			JsonContainerType currentObject = this.Peek();

			switch (currentObject)
			{
				case JsonContainerType.Object:
					this._currentState = State.Object;
					break;
				case JsonContainerType.Array:
					this._currentState = State.Array;
					break;
				case JsonContainerType.Constructor:
					this._currentState = State.Constructor;
					break;
				case JsonContainerType.None:
					this._currentState = State.Finished;
					break;
				default:
					throw JsonReaderException.Create(this, "While setting the reader state back to current object an unexpected JsonType was encountered: {0}".FormatWith(CultureInfo.InvariantCulture, currentObject));
			}
		}

		internal static bool IsPrimitiveToken(JsonToken token)
		{
			switch (token)
			{
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.String:
				case JsonToken.Boolean:
				case JsonToken.Undefined:
				case JsonToken.Null:
				case JsonToken.Date:
				case JsonToken.Bytes:
					return true;
				default:
					return false;
			}
		}

		internal static bool IsStartToken(JsonToken token)
		{
			switch (token)
			{
				case JsonToken.StartObject:
				case JsonToken.StartArray:
				case JsonToken.StartConstructor:
					return true;
				default:
					return false;
			}
		}

		private JsonContainerType GetTypeForCloseToken(JsonToken token)
		{
			switch (token)
			{
				case JsonToken.EndObject:
					return JsonContainerType.Object;
				case JsonToken.EndArray:
					return JsonContainerType.Array;
				case JsonToken.EndConstructor:
					return JsonContainerType.Constructor;
				default:
					throw JsonReaderException.Create(this, "Not a valid close JsonToken: {0}".FormatWith(CultureInfo.InvariantCulture, token));
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"> <c> true </c> to release both managed and unmanaged resources; <c> false </c> to release only unmanaged resources. </param>
		protected virtual void Dispose(bool disposing)
		{
			if (this._currentState != State.Closed && disposing)
				this.Close();
		}

		/// <summary>
		/// Changes the <see cref="State" /> to Closed.
		/// </summary>
		public virtual void Close()
		{
			this._currentState = State.Closed;
			this._tokenType = JsonToken.None;
			this._value = null;
		}
	}
}