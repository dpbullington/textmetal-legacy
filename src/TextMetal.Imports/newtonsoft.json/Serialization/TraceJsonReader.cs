using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json.Serialization
{
	internal class TraceJsonReader : JsonReader, IJsonLineInfo
	{
		private readonly JsonReader _innerReader;
		private readonly JsonTextWriter _textWriter;
		private readonly StringWriter _sw;

		public TraceJsonReader(JsonReader innerReader)
		{
			this._innerReader = innerReader;

			this._sw = new StringWriter(CultureInfo.InvariantCulture);
			this._textWriter = new JsonTextWriter(this._sw);
			this._textWriter.Formatting = Formatting.Indented;
		}

		public string GetJson()
		{
			return this._sw.ToString();
		}

		public override bool Read()
		{
			var value = this._innerReader.Read();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return value;
		}

		public override int? ReadAsInt32()
		{
			var value = this._innerReader.ReadAsInt32();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return value;
		}

		public override string ReadAsString()
		{
			var value = this._innerReader.ReadAsString();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return value;
		}

		public override byte[] ReadAsBytes()
		{
			var value = this._innerReader.ReadAsBytes();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return value;
		}

		public override decimal? ReadAsDecimal()
		{
			var value = this._innerReader.ReadAsDecimal();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return value;
		}

		public override DateTime? ReadAsDateTime()
		{
			var value = this._innerReader.ReadAsDateTime();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return value;
		}

#if !NET20
		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			var value = this._innerReader.ReadAsDateTimeOffset();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return value;
		}
#endif

		public override int Depth
		{
			get
			{
				return this._innerReader.Depth;
			}
		}

		public override string Path
		{
			get
			{
				return this._innerReader.Path;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this._innerReader.QuoteChar;
			}
			protected internal set
			{
				this._innerReader.QuoteChar = value;
			}
		}

		public override JsonToken TokenType
		{
			get
			{
				return this._innerReader.TokenType;
			}
		}

		public override object Value
		{
			get
			{
				return this._innerReader.Value;
			}
		}

		public override Type ValueType
		{
			get
			{
				return this._innerReader.ValueType;
			}
		}

		public override void Close()
		{
			this._innerReader.Close();
		}

		bool IJsonLineInfo.HasLineInfo()
		{
			IJsonLineInfo lineInfo = this._innerReader as IJsonLineInfo;
			return lineInfo != null && lineInfo.HasLineInfo();
		}

		int IJsonLineInfo.LineNumber
		{
			get
			{
				IJsonLineInfo lineInfo = this._innerReader as IJsonLineInfo;
				return (lineInfo != null) ? lineInfo.LineNumber : 0;
			}
		}

		int IJsonLineInfo.LinePosition
		{
			get
			{
				IJsonLineInfo lineInfo = this._innerReader as IJsonLineInfo;
				return (lineInfo != null) ? lineInfo.LinePosition : 0;
			}
		}
	}
}