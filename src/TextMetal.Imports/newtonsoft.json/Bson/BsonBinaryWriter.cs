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
using System.Globalization;
using System.IO;
using System.Text;

using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Bson
{
	internal class BsonBinaryWriter
	{
		#region Constructors/Destructors

		public BsonBinaryWriter(BinaryWriter writer)
		{
			this.DateTimeKindHandling = DateTimeKind.Utc;
			this._writer = writer;
		}

		#endregion

		#region Fields/Constants

		private static readonly Encoding Encoding = new UTF8Encoding(false);

		private readonly BinaryWriter _writer;

		private byte[] _largeByteBuffer;

		#endregion

		#region Properties/Indexers/Events

		public DateTimeKind DateTimeKindHandling
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		private int CalculateSize(int stringByteCount)
		{
			return stringByteCount + 1;
		}

		private int CalculateSize(BsonToken t)
		{
			switch (t.Type)
			{
				case BsonType.Object:
				{
					BsonObject value = (BsonObject)t;

					int bases = 4;
					foreach (BsonProperty p in value)
					{
						int size = 1;
						size += CalculateSize(p.Name);
						size += CalculateSize(p.Value);

						bases += size;
					}
					bases += 1;
					value.CalculatedSize = bases;
					return bases;
				}
				case BsonType.Array:
				{
					BsonArray value = (BsonArray)t;

					int size = 4;
					ulong index = 0;
					foreach (BsonToken c in value)
					{
						size += 1;
						size += this.CalculateSize(MathUtils.IntLength(index));
						size += CalculateSize(c);
						index++;
					}
					size += 1;
					value.CalculatedSize = size;

					return value.CalculatedSize;
				}
				case BsonType.Integer:
					return 4;
				case BsonType.Long:
					return 8;
				case BsonType.Number:
					return 8;
				case BsonType.String:
				{
					BsonString value = (BsonString)t;
					string s = (string)value.Value;
					value.ByteCount = (s != null) ? Encoding.GetByteCount(s) : 0;
					value.CalculatedSize = this.CalculateSizeWithLength(value.ByteCount, value.IncludeLength);

					return value.CalculatedSize;
				}
				case BsonType.Boolean:
					return 1;
				case BsonType.Null:
				case BsonType.Undefined:
					return 0;
				case BsonType.Date:
					return 8;
				case BsonType.Binary:
				{
					BsonBinary value = (BsonBinary)t;

					byte[] data = (byte[])value.Value;
					value.CalculatedSize = 4 + 1 + data.Length;

					return value.CalculatedSize;
				}
				case BsonType.Oid:
					return 12;
				case BsonType.Regex:
				{
					BsonRegex value = (BsonRegex)t;
					int size = 0;
					size += CalculateSize(value.Pattern);
					size += CalculateSize(value.Options);
					value.CalculatedSize = size;

					return value.CalculatedSize;
				}
				default:
					throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(CultureInfo.InvariantCulture, t.Type));
			}
		}

		private int CalculateSizeWithLength(int stringByteCount, bool includeSize)
		{
			int baseSize = (includeSize)
				? 5 // size bytes + terminator
				: 1; // terminator

			return baseSize + stringByteCount;
		}

		public void Close()
		{
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
      _writer.Close();
#else
			this._writer.Dispose();
#endif
		}

		public void Flush()
		{
			this._writer.Flush();
		}

		private void WriteString(string s, int byteCount, int? calculatedlengthPrefix)
		{
			if (calculatedlengthPrefix != null)
				this._writer.Write(calculatedlengthPrefix.Value);

			this.WriteUtf8Bytes(s, byteCount);

			this._writer.Write((byte)0);
		}

		public void WriteToken(BsonToken t)
		{
			CalculateSize(t);
			this.WriteTokenInternal(t);
		}

		private void WriteTokenInternal(BsonToken t)
		{
			switch (t.Type)
			{
				case BsonType.Object:
				{
					BsonObject value = (BsonObject)t;
					this._writer.Write(value.CalculatedSize);
					foreach (BsonProperty property in value)
					{
						this._writer.Write((sbyte)property.Value.Type);
						this.WriteString((string)property.Name.Value, property.Name.ByteCount, null);
						this.WriteTokenInternal(property.Value);
					}
					this._writer.Write((byte)0);
				}
					break;
				case BsonType.Array:
				{
					BsonArray value = (BsonArray)t;
					this._writer.Write(value.CalculatedSize);
					ulong index = 0;
					foreach (BsonToken c in value)
					{
						this._writer.Write((sbyte)c.Type);
						this.WriteString(index.ToString(CultureInfo.InvariantCulture), MathUtils.IntLength(index), null);
						this.WriteTokenInternal(c);
						index++;
					}
					this._writer.Write((byte)0);
				}
					break;
				case BsonType.Integer:
				{
					BsonValue value = (BsonValue)t;
					this._writer.Write(Convert.ToInt32(value.Value, CultureInfo.InvariantCulture));
				}
					break;
				case BsonType.Long:
				{
					BsonValue value = (BsonValue)t;
					this._writer.Write(Convert.ToInt64(value.Value, CultureInfo.InvariantCulture));
				}
					break;
				case BsonType.Number:
				{
					BsonValue value = (BsonValue)t;
					this._writer.Write(Convert.ToDouble(value.Value, CultureInfo.InvariantCulture));
				}
					break;
				case BsonType.String:
				{
					BsonString value = (BsonString)t;
					this.WriteString((string)value.Value, value.ByteCount, value.CalculatedSize - 4);
				}
					break;
				case BsonType.Boolean:
				{
					BsonValue value = (BsonValue)t;
					this._writer.Write((bool)value.Value);
				}
					break;
				case BsonType.Null:
				case BsonType.Undefined:
					break;
				case BsonType.Date:
				{
					BsonValue value = (BsonValue)t;

					long ticks = 0;

					if (value.Value is DateTime)
					{
						DateTime dateTime = (DateTime)value.Value;
						if (this.DateTimeKindHandling == DateTimeKind.Utc)
							dateTime = dateTime.ToUniversalTime();
						else if (this.DateTimeKindHandling == DateTimeKind.Local)
							dateTime = dateTime.ToLocalTime();

						ticks = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(dateTime, false);
					}
#if !NET20
					else
					{
						DateTimeOffset dateTimeOffset = (DateTimeOffset)value.Value;
						ticks = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(dateTimeOffset.UtcDateTime, dateTimeOffset.Offset);
					}
#endif

					this._writer.Write(ticks);
				}
					break;
				case BsonType.Binary:
				{
					BsonBinary value = (BsonBinary)t;

					byte[] data = (byte[])value.Value;
					this._writer.Write(data.Length);
					this._writer.Write((byte)value.BinaryType);
					this._writer.Write(data);
				}
					break;
				case BsonType.Oid:
				{
					BsonValue value = (BsonValue)t;

					byte[] data = (byte[])value.Value;
					this._writer.Write(data);
				}
					break;
				case BsonType.Regex:
				{
					BsonRegex value = (BsonRegex)t;

					this.WriteString((string)value.Pattern.Value, value.Pattern.ByteCount, null);
					this.WriteString((string)value.Options.Value, value.Options.ByteCount, null);
				}
					break;
				default:
					throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(CultureInfo.InvariantCulture, t.Type));
			}
		}

		public void WriteUtf8Bytes(string s, int byteCount)
		{
			if (s != null)
			{
				if (this._largeByteBuffer == null)
					this._largeByteBuffer = new byte[256];
				if (byteCount <= 256)
				{
					Encoding.GetBytes(s, 0, s.Length, this._largeByteBuffer, 0);
					this._writer.Write(this._largeByteBuffer, 0, byteCount);
				}
				else
				{
					byte[] bytes = Encoding.GetBytes(s);
					this._writer.Write(bytes);
				}
			}
		}

		#endregion
	}
}