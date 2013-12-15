// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Xml;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XmlSimpleSerializer<T> : XmlTypeSerializer
	{
		#region Constructors/Destructors

		public XmlSimpleSerializer(
			Func<T, string> getString,
			Func<string, T> getObject)
		{
			this.getString = getString;
			this.getObject = getObject;
		}

		#endregion

		#region Fields/Constants

		private readonly Func<string, T> getObject;
		private readonly Func<T, string> getString;

		#endregion

		#region Properties/Indexers/Events

		public override XmlTypeKind Kind
		{
			get
			{
				return XmlTypeKind.Simple;
			}
		}

		#endregion

		#region Methods/Operators

		public override object GetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			return this.getObject(node.Value);
		}

		public override void SetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor, object oldValue, ref object value)
		{
			node.Value = this.getString((T)value);
		}

		#endregion
	}

	public static class XmlSimpleSerializer
	{
		#region Fields/Constants

		public static readonly XmlTypeSerializer
			ForBoolean = new XmlSimpleSerializer<Boolean>(XmlConvert.ToString, XmlConvert.ToBoolean);

		public static readonly XmlTypeSerializer
			ForByte = new XmlSimpleSerializer<Byte>(XmlConvert.ToString, XmlConvert.ToByte);

		public static readonly XmlTypeSerializer
			ForByteArray = new XmlSimpleSerializer<Byte[]>(Convert.ToBase64String, Convert.FromBase64String);

		public static readonly XmlTypeSerializer
			ForChar = new XmlSimpleSerializer<Char>(XmlConvert.ToString, XmlConvert.ToChar);

		public static readonly XmlTypeSerializer
			ForDateTime = new XmlSimpleSerializer<DateTime>(XmlConvert_ToString, XmlConvert_ToDateTime),
			ForDateTimeOffset = new XmlSimpleSerializer<DateTimeOffset>(XmlConvert.ToString, XmlConvert.ToDateTimeOffset);

		public static readonly XmlTypeSerializer
			ForDecimal = new XmlSimpleSerializer<Decimal>(XmlConvert.ToString, XmlConvert.ToDecimal);

		public static readonly XmlTypeSerializer
			ForDouble = new XmlSimpleSerializer<Double>(XmlConvert.ToString, XmlConvert.ToDouble);

		public static readonly XmlTypeSerializer
			ForGuid = new XmlSimpleSerializer<Guid>(XmlConvert.ToString, XmlConvert.ToGuid);

		public static readonly XmlTypeSerializer
			ForInt16 = new XmlSimpleSerializer<Int16>(XmlConvert.ToString, XmlConvert.ToInt16),
			ForInt32 = new XmlSimpleSerializer<Int32>(XmlConvert.ToString, XmlConvert.ToInt32),
			ForInt64 = new XmlSimpleSerializer<Int64>(XmlConvert.ToString, XmlConvert.ToInt64);

		public static readonly XmlTypeSerializer
			ForSByte = new XmlSimpleSerializer<SByte>(XmlConvert.ToString, XmlConvert.ToSByte);

		public static readonly XmlTypeSerializer
			ForSingle = new XmlSimpleSerializer<Single>(XmlConvert.ToString, XmlConvert.ToSingle);

		public static readonly XmlTypeSerializer
			ForTimeSpan = new XmlSimpleSerializer<TimeSpan>(XmlConvert.ToString, XmlConvert.ToTimeSpan);

		public static readonly XmlTypeSerializer
			ForUInt16 = new XmlSimpleSerializer<UInt16>(XmlConvert.ToString, XmlConvert.ToUInt16),
			ForUInt32 = new XmlSimpleSerializer<UInt32>(XmlConvert.ToString, XmlConvert.ToUInt32),
			ForUInt64 = new XmlSimpleSerializer<UInt64>(XmlConvert.ToString, XmlConvert.ToUInt64);

		public static readonly XmlTypeSerializer
			ForUri = new XmlSimpleSerializer<Uri>(u => u.ToString(), s => new Uri(s, UriKind.RelativeOrAbsolute));

		#endregion

		#region Methods/Operators

		private static DateTime XmlConvert_ToDateTime(string value)
		{
			return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
		}

		private static string XmlConvert_ToString(DateTime value)
		{
			return XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind);
		}

		#endregion
	}
}

#endif