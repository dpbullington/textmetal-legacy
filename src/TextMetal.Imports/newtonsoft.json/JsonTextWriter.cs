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
#if !(NET20 || NET35 || SILVERLIGHT || PORTABLE40 || PORTABLE)
using System.Numerics;
#endif
using System.IO;

using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	/// <summary>
	/// Represents a writer that provides a fast, non-cached, forward-only way of generating Json data.
	/// </summary>
	public class JsonTextWriter : JsonWriter
	{
		private readonly TextWriter _writer;
		private Base64Encoder _base64Encoder;
		private char _indentChar;
		private int _indentation;
		private char _quoteChar;
		private bool _quoteName;
		private bool[] _charEscapeFlags;
		private char[] _writeBuffer;

		private Base64Encoder Base64Encoder
		{
			get
			{
				if (this._base64Encoder == null)
					this._base64Encoder = new Base64Encoder(this._writer);

				return this._base64Encoder;
			}
		}

		/// <summary>
		/// Gets or sets how many IndentChars to write for each level in the hierarchy when <see cref="Formatting" /> is set to <c> Formatting.Indented </c>.
		/// </summary>
		public int Indentation
		{
			get
			{
				return this._indentation;
			}
			set
			{
				if (value < 0)
					throw new ArgumentException("Indentation value must be greater than 0.");

				this._indentation = value;
			}
		}

		/// <summary>
		/// Gets or sets which character to use to quote attribute values.
		/// </summary>
		public char QuoteChar
		{
			get
			{
				return this._quoteChar;
			}
			set
			{
				if (value != '"' && value != '\'')
					throw new ArgumentException(@"Invalid JavaScript string quote character. Valid quote characters are ' and "".");

				this._quoteChar = value;
				this.UpdateCharEscapeFlags();
			}
		}

		/// <summary>
		/// Gets or sets which character to use for indenting when <see cref="Formatting" /> is set to <c> Formatting.Indented </c>.
		/// </summary>
		public char IndentChar
		{
			get
			{
				return this._indentChar;
			}
			set
			{
				this._indentChar = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether object names will be surrounded with quotes.
		/// </summary>
		public bool QuoteName
		{
			get
			{
				return this._quoteName;
			}
			set
			{
				this._quoteName = value;
			}
		}

		/// <summary>
		/// Creates an instance of the <c> JsonWriter </c> class using the specified <see cref="TextWriter" />.
		/// </summary>
		/// <param name="textWriter"> The <c> TextWriter </c> to write to. </param>
		public JsonTextWriter(TextWriter textWriter)
		{
			if (textWriter == null)
				throw new ArgumentNullException("textWriter");

			this._writer = textWriter;
			this._quoteChar = '"';
			this._quoteName = true;
			this._indentChar = ' ';
			this._indentation = 2;

			this.UpdateCharEscapeFlags();
		}

		/// <summary>
		/// Flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.
		/// </summary>
		public override void Flush()
		{
			this._writer.Flush();
		}

		/// <summary>
		/// Closes this stream and the underlying stream.
		/// </summary>
		public override void Close()
		{
			base.Close();

			if (this.CloseOutput && this._writer != null)
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
        _writer.Close();
#else
				this._writer.Dispose();
#endif
		}

		/// <summary>
		/// Writes the beginning of a Json object.
		/// </summary>
		public override void WriteStartObject()
		{
			this.InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);

			this._writer.Write("{");
		}

		/// <summary>
		/// Writes the beginning of a Json array.
		/// </summary>
		public override void WriteStartArray()
		{
			this.InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);

			this._writer.Write("[");
		}

		/// <summary>
		/// Writes the start of a constructor with the given name.
		/// </summary>
		/// <param name="name"> The name of the constructor. </param>
		public override void WriteStartConstructor(string name)
		{
			this.InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);

			this._writer.Write("new ");
			this._writer.Write(name);
			this._writer.Write("(");
		}

		/// <summary>
		/// Writes the specified end token.
		/// </summary>
		/// <param name="token"> The end token to write. </param>
		protected override void WriteEnd(JsonToken token)
		{
			switch (token)
			{
				case JsonToken.EndObject:
					this._writer.Write("}");
					break;
				case JsonToken.EndArray:
					this._writer.Write("]");
					break;
				case JsonToken.EndConstructor:
					this._writer.Write(")");
					break;
				default:
					throw JsonWriterException.Create(this, "Invalid JsonToken: " + token, null);
			}
		}

		/// <summary>
		/// Writes the property name of a name/value pair on a Json object.
		/// </summary>
		/// <param name="name"> The name of the property. </param>
		public override void WritePropertyName(string name)
		{
			this.InternalWritePropertyName(name);

			this.WriteEscapedString(name);

			this._writer.Write(':');
		}

		/// <summary>
		/// Writes the property name of a name/value pair on a JSON object.
		/// </summary>
		/// <param name="name"> The name of the property. </param>
		/// <param name="escape"> A flag to indicate whether the text should be escaped when it is written as a JSON property name. </param>
		public override void WritePropertyName(string name, bool escape)
		{
			this.InternalWritePropertyName(name);

			if (escape)
				this.WriteEscapedString(name);
			else
			{
				if (this._quoteName)
					this._writer.Write(this._quoteChar);

				this._writer.Write(name);

				if (this._quoteName)
					this._writer.Write(this._quoteChar);
			}

			this._writer.Write(':');
		}

		internal override void OnStringEscapeHandlingChanged()
		{
			this.UpdateCharEscapeFlags();
		}

		private void UpdateCharEscapeFlags()
		{
			if (this.StringEscapeHandling == StringEscapeHandling.EscapeHtml)
				this._charEscapeFlags = JavaScriptUtils.HtmlCharEscapeFlags;
			else if (this._quoteChar == '"')
				this._charEscapeFlags = JavaScriptUtils.DoubleQuoteCharEscapeFlags;
			else
				this._charEscapeFlags = JavaScriptUtils.SingleQuoteCharEscapeFlags;
		}

		/// <summary>
		/// Writes indent characters.
		/// </summary>
		protected override void WriteIndent()
		{
			this._writer.Write(Environment.NewLine);

			// levels of indentation multiplied by the indent count
			int currentIndentCount = this.Top * this._indentation;

			while (currentIndentCount > 0)
			{
				// write up to a max of 10 characters at once to avoid creating too many new strings
				int writeCount = Math.Min(currentIndentCount, 10);

				this._writer.Write(new string(this._indentChar, writeCount));

				currentIndentCount -= writeCount;
			}
		}

		/// <summary>
		/// Writes the JSON value delimiter.
		/// </summary>
		protected override void WriteValueDelimiter()
		{
			this._writer.Write(',');
		}

		/// <summary>
		/// Writes an indent space.
		/// </summary>
		protected override void WriteIndentSpace()
		{
			this._writer.Write(' ');
		}

		private void WriteValueInternal(string value, JsonToken token)
		{
			this._writer.Write(value);
		}

		#region WriteValue methods

		/// <summary>
		/// Writes a <see cref="Object" /> value.
		/// An error will raised if the value cannot be written as a single JSON token.
		/// </summary>
		/// <param name="value"> The <see cref="Object" /> value to write. </param>
		public override void WriteValue(object value)
		{
#if !(NET20 || NET35 || SILVERLIGHT || PORTABLE || PORTABLE40)
      if (value is BigInteger)
      {
        InternalWriteValue(JsonToken.Integer);
        WriteValueInternal(((BigInteger)value).ToString(CultureInfo.InvariantCulture), JsonToken.String);
      }
      else
#endif
			{
				base.WriteValue(value);
			}
		}

		/// <summary>
		/// Writes a null value.
		/// </summary>
		public override void WriteNull()
		{
			this.InternalWriteValue(JsonToken.Null);
			this.WriteValueInternal(JsonConvert.Null, JsonToken.Null);
		}

		/// <summary>
		/// Writes an undefined value.
		/// </summary>
		public override void WriteUndefined()
		{
			this.InternalWriteValue(JsonToken.Undefined);
			this.WriteValueInternal(JsonConvert.Undefined, JsonToken.Undefined);
		}

		/// <summary>
		/// Writes raw JSON.
		/// </summary>
		/// <param name="json"> The raw JSON to write. </param>
		public override void WriteRaw(string json)
		{
			this.InternalWriteRaw();

			this._writer.Write(json);
		}

		/// <summary>
		/// Writes a <see cref="String" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="String" /> value to write. </param>
		public override void WriteValue(string value)
		{
			this.InternalWriteValue(JsonToken.String);

			if (value == null)
				this.WriteValueInternal(JsonConvert.Null, JsonToken.Null);
			else
				this.WriteEscapedString(value);
		}

		private void WriteEscapedString(string value)
		{
			this.EnsureWriteBuffer();
			JavaScriptUtils.WriteEscapedJavaScriptString(this._writer, value, this._quoteChar, true, this._charEscapeFlags, this.StringEscapeHandling, ref this._writeBuffer);
		}

		/// <summary>
		/// Writes a <see cref="Int32" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Int32" /> value to write. </param>
		public override void WriteValue(int value)
		{
			this.InternalWriteValue(JsonToken.Integer);
			WriteIntegerValue(value);
		}

		/// <summary>
		/// Writes a <see cref="UInt32" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="UInt32" /> value to write. </param>
		[CLSCompliant(false)]
		public override void WriteValue(uint value)
		{
			this.InternalWriteValue(JsonToken.Integer);
			WriteIntegerValue(value);
		}

		/// <summary>
		/// Writes a <see cref="Int64" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Int64" /> value to write. </param>
		public override void WriteValue(long value)
		{
			this.InternalWriteValue(JsonToken.Integer);
			WriteIntegerValue(value);
		}

		/// <summary>
		/// Writes a <see cref="UInt64" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="UInt64" /> value to write. </param>
		[CLSCompliant(false)]
		public override void WriteValue(ulong value)
		{
			this.InternalWriteValue(JsonToken.Integer);
			WriteIntegerValue(value);
		}

		/// <summary>
		/// Writes a <see cref="Single" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Single" /> value to write. </param>
		public override void WriteValue(float value)
		{
			this.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value, this.FloatFormatHandling, this.QuoteChar, false), JsonToken.Float);
		}

		/// <summary>
		/// Writes a <see cref="Nullable{Single}" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Nullable{Single}" /> value to write. </param>
		public override void WriteValue(float? value)
		{
			if (value == null)
				this.WriteNull();
			else
			{
				this.InternalWriteValue(JsonToken.Float);
				this.WriteValueInternal(JsonConvert.ToString(value.Value, this.FloatFormatHandling, this.QuoteChar, true), JsonToken.Float);
			}
		}

		/// <summary>
		/// Writes a <see cref="Double" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Double" /> value to write. </param>
		public override void WriteValue(double value)
		{
			this.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value, this.FloatFormatHandling, this.QuoteChar, false), JsonToken.Float);
		}

		/// <summary>
		/// Writes a <see cref="Nullable{Double}" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Nullable{Double}" /> value to write. </param>
		public override void WriteValue(double? value)
		{
			if (value == null)
				this.WriteNull();
			else
			{
				this.InternalWriteValue(JsonToken.Float);
				this.WriteValueInternal(JsonConvert.ToString(value.Value, this.FloatFormatHandling, this.QuoteChar, true), JsonToken.Float);
			}
		}

		/// <summary>
		/// Writes a <see cref="Boolean" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Boolean" /> value to write. </param>
		public override void WriteValue(bool value)
		{
			this.InternalWriteValue(JsonToken.Boolean);
			this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.Boolean);
		}

		/// <summary>
		/// Writes a <see cref="Int16" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Int16" /> value to write. </param>
		public override void WriteValue(short value)
		{
			this.InternalWriteValue(JsonToken.Integer);
			WriteIntegerValue(value);
		}

		/// <summary>
		/// Writes a <see cref="UInt16" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="UInt16" /> value to write. </param>
		[CLSCompliant(false)]
		public override void WriteValue(ushort value)
		{
			this.InternalWriteValue(JsonToken.Integer);
			WriteIntegerValue(value);
		}

		/// <summary>
		/// Writes a <see cref="Char" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Char" /> value to write. </param>
		public override void WriteValue(char value)
		{
			this.InternalWriteValue(JsonToken.String);
			this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.String);
		}

		/// <summary>
		/// Writes a <see cref="Byte" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Byte" /> value to write. </param>
		public override void WriteValue(byte value)
		{
			this.InternalWriteValue(JsonToken.Integer);
			WriteIntegerValue(value);
		}

		/// <summary>
		/// Writes a <see cref="SByte" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="SByte" /> value to write. </param>
		[CLSCompliant(false)]
		public override void WriteValue(sbyte value)
		{
			this.InternalWriteValue(JsonToken.Integer);
			WriteIntegerValue(value);
		}

		/// <summary>
		/// Writes a <see cref="Decimal" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Decimal" /> value to write. </param>
		public override void WriteValue(decimal value)
		{
			this.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.Float);
		}

		/// <summary>
		/// Writes a <see cref="DateTime" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="DateTime" /> value to write. </param>
		public override void WriteValue(DateTime value)
		{
			this.InternalWriteValue(JsonToken.Date);
			value = DateTimeUtils.EnsureDateTime(value, this.DateTimeZoneHandling);

			if (string.IsNullOrEmpty(this.DateFormatString))
			{
				this.EnsureWriteBuffer();

				int pos = 0;
				this._writeBuffer[pos++] = this._quoteChar;
				pos = DateTimeUtils.WriteDateTimeString(this._writeBuffer, pos, value, null, value.Kind, this.DateFormatHandling);
				this._writeBuffer[pos++] = this._quoteChar;

				this._writer.Write(this._writeBuffer, 0, pos);
			}
			else
			{
				this._writer.Write(this._quoteChar);
				this._writer.Write(value.ToString(this.DateFormatString, this.Culture));
				this._writer.Write(this._quoteChar);
			}
		}

		/// <summary>
		/// Writes a <see cref="T:Byte[]" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="T:Byte[]" /> value to write. </param>
		public override void WriteValue(byte[] value)
		{
			if (value == null)
				this.WriteNull();
			else
			{
				this.InternalWriteValue(JsonToken.Bytes);
				this._writer.Write(this._quoteChar);
				this.Base64Encoder.Encode(value, 0, value.Length);
				this.Base64Encoder.Flush();
				this._writer.Write(this._quoteChar);
			}
		}

#if !NET20
		/// <summary>
		/// Writes a <see cref="DateTimeOffset" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="DateTimeOffset" /> value to write. </param>
		public override void WriteValue(DateTimeOffset value)
		{
			this.InternalWriteValue(JsonToken.Date);

			if (string.IsNullOrEmpty(this.DateFormatString))
			{
				this.EnsureWriteBuffer();

				int pos = 0;
				this._writeBuffer[pos++] = this._quoteChar;
				pos = DateTimeUtils.WriteDateTimeString(this._writeBuffer, pos, (this.DateFormatHandling == DateFormatHandling.IsoDateFormat) ? value.DateTime : value.UtcDateTime, value.Offset, DateTimeKind.Local, this.DateFormatHandling);
				this._writeBuffer[pos++] = this._quoteChar;

				this._writer.Write(this._writeBuffer, 0, pos);
			}
			else
			{
				this._writer.Write(this._quoteChar);
				this._writer.Write(value.ToString(this.DateFormatString, this.Culture));
				this._writer.Write(this._quoteChar);
			}
		}
#endif

		/// <summary>
		/// Writes a <see cref="Guid" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Guid" /> value to write. </param>
		public override void WriteValue(Guid value)
		{
			this.InternalWriteValue(JsonToken.String);
			this.WriteValueInternal(JsonConvert.ToString(value, this._quoteChar), JsonToken.String);
		}

		/// <summary>
		/// Writes a <see cref="TimeSpan" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="TimeSpan" /> value to write. </param>
		public override void WriteValue(TimeSpan value)
		{
			this.InternalWriteValue(JsonToken.String);
			this.WriteValueInternal(JsonConvert.ToString(value, this._quoteChar), JsonToken.String);
		}

		/// <summary>
		/// Writes a <see cref="Uri" /> value.
		/// </summary>
		/// <param name="value"> The <see cref="Uri" /> value to write. </param>
		public override void WriteValue(Uri value)
		{
			if (value == null)
				this.WriteNull();
			else
			{
				this.InternalWriteValue(JsonToken.String);
				this.WriteValueInternal(JsonConvert.ToString(value, this._quoteChar), JsonToken.String);
			}
		}

		#endregion

		/// <summary>
		/// Writes out a comment <code>/*...*/</code> containing the specified text.
		/// </summary>
		/// <param name="text"> Text to place inside the comment. </param>
		public override void WriteComment(string text)
		{
			this.InternalWriteComment();

			this._writer.Write("/*");
			this._writer.Write(text);
			this._writer.Write("*/");
		}

		/// <summary>
		/// Writes out the given white space.
		/// </summary>
		/// <param name="ws"> The string of white space characters. </param>
		public override void WriteWhitespace(string ws)
		{
			this.InternalWriteWhitespace(ws);

			this._writer.Write(ws);
		}

		private void EnsureWriteBuffer()
		{
			if (this._writeBuffer == null)
				this._writeBuffer = new char[64];
		}

		private void WriteIntegerValue(long value)
		{
			this.EnsureWriteBuffer();

			if (value >= 0 && value <= 9)
				this._writer.Write((char)('0' + value));
			else
			{
				ulong uvalue = (value < 0) ? (ulong)-value : (ulong)value;

				if (value < 0)
					this._writer.Write('-');

				WriteIntegerValue(uvalue);
			}
		}

		private void WriteIntegerValue(ulong uvalue)
		{
			this.EnsureWriteBuffer();

			if (uvalue <= 9)
				this._writer.Write((char)('0' + uvalue));
			else
			{
				int totalLength = MathUtils.IntLength(uvalue);
				int length = 0;

				do
				{
					this._writeBuffer[totalLength - ++length] = (char)('0' + (uvalue % 10));
					uvalue /= 10;
				}
				while (uvalue != 0);

				this._writer.Write(this._writeBuffer, 0, length);
			}
		}
	}
}