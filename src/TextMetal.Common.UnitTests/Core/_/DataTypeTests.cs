/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using NUnit.Framework;

using TextMetal.Common.Core;

namespace TextMetal.Common.UnitTests.Core._
{
	/// <summary>
	/// Unit tests.
	/// </summary>
	[TestFixture]
	public class DataTypeTests
	{
		#region Constructors/Destructors

		public DataTypeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldGetNonNullOnNonNullValueChangeTypeTest()
		{
			object value;

			value = DataType.Instance.ChangeType((byte)1, typeof(int));

			Assert.IsNotNull(value);
			Assert.IsInstanceOf<int>(value);
			Assert.AreEqual((int)1, value);
		}

		[Test]
		public void ShouldGetNonNullOnNonNullValueNullableChangeTypeTest()
		{
			object value;

			value = DataType.Instance.ChangeType((byte)1, typeof(int?));

			Assert.IsNotNull(value);
			Assert.IsInstanceOf<int?>(value);
			Assert.AreEqual((int?)1, value);
		}

		[Test]
		public void ShouldGetDefaultOnNullValueChangeTypeTest()
		{
			object value;

			value = DataType.Instance.ChangeType(null, typeof(int));

			Assert.AreEqual(default(int), value);
		}

		[Test]
		public void ShouldCheckIsNullOrWhiteSpaceTest()
		{
			Assert.IsTrue(DataType.Instance.IsNullOrWhiteSpace(null));
			Assert.IsTrue(DataType.Instance.IsNullOrWhiteSpace(string.Empty));
			Assert.IsTrue(DataType.Instance.IsNullOrWhiteSpace("   "));
			Assert.IsFalse(DataType.Instance.IsNullOrWhiteSpace("daniel"));
			Assert.IsFalse(DataType.Instance.IsNullOrWhiteSpace("   daniel     "));
		}

		[Test]
		public void ShouldCheckIsWhiteSpaceTest()
		{
			Assert.IsFalse(DataType.Instance.IsWhiteSpace(null));
			Assert.IsTrue(DataType.Instance.IsWhiteSpace("   "));
			Assert.IsFalse(DataType.Instance.IsWhiteSpace("daniel"));
			Assert.IsFalse(DataType.Instance.IsWhiteSpace("   daniel     "));
		}

		//[Test]
		//public void ShouldCheckObjectsCompareValueSemanticsTest()
		//{
		//    int? result;
		//    object objA, objB;

		//    // both null
		//    objA = null;
		//    objB = null;
		//    result = DataType.Instance.ObjectsCompareValueSemantics(objA, objB);
		//    Assert.IsNull(result);

		//    // objA null, objB not null
		//    objA = null;
		//    objB = 10;
		//    result = DataType.Instance.ObjectsCompareValueSemantics(objA, objB);
		//    Assert.Less(result, 0);

		//    // objA not null, objB null
		//    objA = 10;
		//    objB = null;
		//    result = DataType.Instance.ObjectsCompareValueSemantics(objA, objB);
		//    Assert.Greater(result, 0);

		//    // objA == objB
		//    objA = 100;
		//    objB = 100;
		//    result = DataType.Instance.ObjectsCompareValueSemantics(objA, objB);
		//    Assert.AreEqual(0, result);

		//    // objA != objB
		//    objA = 100;
		//    objB = -100;
		//    result = DataType.Instance.ObjectsCompareValueSemantics(objA, objB);
		//    Assert.Greater(result, 0);
		//}

		[Test]
		public void ShouldCheckObjectsEqualValueSemanticsTest()
		{
			bool result;
			object objA, objB;

			// both null
			objA = null;
			objB = null;
			result = DataType.Instance.ObjectsEqualValueSemantics(objA, objB);
			Assert.IsTrue(result);

			// objA null, objB not null
			objA = null;
			objB = "not null string";
			result = DataType.Instance.ObjectsEqualValueSemantics(objA, objB);
			Assert.IsFalse(result);

			// objA not null, objB null
			objA = "not null string";
			objB = null;
			result = DataType.Instance.ObjectsEqualValueSemantics(objA, objB);
			Assert.IsFalse(result);

			// objA == objB
			objA = 100;
			objB = 100;
			result = DataType.Instance.ObjectsEqualValueSemantics(objA, objB);
			Assert.IsTrue(result);

			// objA != objB
			objA = 100;
			objB = -100;
			result = DataType.Instance.ObjectsEqualValueSemantics(objA, objB);
			Assert.IsFalse(result);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ShouldFailOnInvalidGenericTypeTryParseTest()
		{
			KeyValuePair<int, int> ovalue;
			bool result;

			result = DataType.Instance.TryParse<KeyValuePair<int, int>>(DBNull.Value.ToString(), out ovalue);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ShouldFailOnInvalidTypeTryParseTest()
		{
			object ovalue;
			bool result;

			result = DataType.Instance.TryParse(typeof(KeyValuePair<int, int>), DBNull.Value.ToString(), out ovalue);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeChangeTypeTest()
		{
			DataType.Instance.ChangeType(1, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeDefaultValueTest()
		{
			DataType.Instance.DefaultValue(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeTryParseTest()
		{
			object ovalue;
			bool result;

			result = DataType.Instance.TryParse(null, string.Empty, out ovalue);
		}

		[Test]
		public void ShouldGetBooleanTest()
		{
			Boolean ovalue;
			bool result;

			result = DataType.Instance.TryParse<Boolean>("true", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(true, ovalue);

			result = DataType.Instance.TryParse<Boolean>("false", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(false, ovalue);
		}

		[Test]
		public void ShouldGetByteTest()
		{
			Byte ovalue;
			bool result;

			result = DataType.Instance.TryParse<Byte>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(0, ovalue);
		}

		[Test]
		public void ShouldGetCharTest()
		{
			Char ovalue;
			bool result;

			result = DataType.Instance.TryParse<Char>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual('0', ovalue);
		}

		[Test]
		public void ShouldGetDateTimeOffsetTest()
		{
			DateTimeOffset ovalue;
			bool result;

			result = DataType.Instance.TryParse<DateTimeOffset>("6/22/2003", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(new DateTimeOffset(new DateTime(2003, 6, 22)), ovalue);
		}

		[Test]
		public void ShouldGetDateTimeTest()
		{
			DateTime ovalue;
			bool result;

			result = DataType.Instance.TryParse<DateTime>("6/22/2003", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(new DateTime(2003, 6, 22), ovalue);
		}

		[Test]
		public void ShouldGetDecimalTest()
		{
			Decimal ovalue;
			bool result;

			result = DataType.Instance.TryParse<Decimal>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(0, ovalue);
		}

		[Test]
		public void ShouldGetDefaultValueTest()
		{
			object defaultValue;

			defaultValue = DataType.Instance.DefaultValue(typeof(int));

			Assert.AreEqual(0, defaultValue);

			defaultValue = DataType.Instance.DefaultValue(typeof(int?));

			Assert.IsNull(defaultValue);
		}

		[Test]
		public void ShouldGetDoubleTest()
		{
			Double ovalue;
			bool result;

			result = DataType.Instance.TryParse<Double>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(0, ovalue);
		}

		[Test]
		public void ShouldGetEnumTest()
		{
			CharSet ovalue;
			bool result;

			result = DataType.Instance.TryParse<CharSet>("Unicode", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(CharSet.Unicode, ovalue);
		}

		[Test]
		public void ShouldGetGuidTest()
		{
			Guid ovalue;
			bool result;

			result = DataType.Instance.TryParse<Guid>("{00000000-0000-0000-0000-000000000000}", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(Guid.Empty, ovalue);
		}

		[Test]
		public void ShouldGetInt16Test()
		{
			Int16 ovalue;
			bool result;

			result = DataType.Instance.TryParse<Int16>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(0, ovalue);
		}

		[Test]
		public void ShouldGetInt32Test()
		{
			Int32 ovalue;
			bool result;

			result = DataType.Instance.TryParse<Int32>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(0, ovalue);
		}

		[Test]
		public void ShouldGetInt64Test()
		{
			Int64 ovalue;
			bool result;

			result = DataType.Instance.TryParse<Int64>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(0, ovalue);
		}

		[Test]
		public void ShouldGetSByteTest()
		{
			SByte ovalue;
			bool result;

			result = DataType.Instance.TryParse<SByte>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(0, ovalue);
		}

		[Test]
		public void ShouldGetSingleTest()
		{
			Single ovalue;
			bool result;

			result = DataType.Instance.TryParse<Single>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(0, ovalue);
		}

		[Test]
		public void ShouldGetStringTest()
		{
			String ovalue;
			bool result;

			result = DataType.Instance.TryParse<String>("0-8-8-8-8-8-8-8-c", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual("0-8-8-8-8-8-8-8-c", ovalue);
		}

		[Test]
		public void ShouldGetTimeSpanTest()
		{
			TimeSpan ovalue;
			bool result;

			result = DataType.Instance.TryParse<TimeSpan>("0:0:0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(TimeSpan.Zero, ovalue);
		}

		[Test]
		public void ShouldGetUInt16Test()
		{
			UInt16 ovalue;
			bool result;

			result = DataType.Instance.TryParse<UInt16>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(0, ovalue);
		}

		[Test]
		public void ShouldGetUInt32Test()
		{
			UInt32 ovalue;
			bool result;

			result = DataType.Instance.TryParse<UInt32>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(0, ovalue);
		}

		[Test]
		public void ShouldGetUInt64Test()
		{
			UInt64 ovalue;
			bool result;

			result = DataType.Instance.TryParse<UInt64>("0", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual(0, ovalue);
		}

		[Test]
		public void ShouldNotGetBooleanTest()
		{
			Boolean ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Boolean>("gibberish", out ovalue));
		}

		[Test]
		public void ShouldNotGetByteTest()
		{
			Byte ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Byte>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<Byte>("1111111111111111111", out ovalue));
		}

		[Test]
		public void ShouldNotGetCharTest()
		{
			Char ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Char>("gibberish", out ovalue));
		}

		[Test]
		public void ShouldNotGetDateTimeOffsetTest()
		{
			DateTimeOffset ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<DateTimeOffset>("gibberish", out ovalue));
		}

		[Test]
		public void ShouldNotGetDateTimeTest()
		{
			DateTime ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<DateTime>("gibberish", out ovalue));
		}

		[Test]
		public void ShouldNotGetDecimalTest()
		{
			Decimal ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Decimal>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<Decimal>("11111111111111111111111111111111111111", out ovalue));
		}

		[Test]
		public void ShouldNotGetDoubleTest()
		{
			Double ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Double>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<Double>("999,769,313,486,232,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000.00", out ovalue));
		}

		[Test]
		public void ShouldNotGetEnumTest()
		{
			CharSet ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<CharSet>("gibberish", out ovalue));
		}

		[Test]
		public void ShouldNotGetGuidTest()
		{
			Guid ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Guid>("gibberish", out ovalue));
		}

		[Test]
		public void ShouldNotGetInt16Test()
		{
			Int16 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Int16>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<Int16>("1111111111111111111", out ovalue));
		}

		[Test]
		public void ShouldNotGetInt32Test()
		{
			Int32 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Int32>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<Int32>("1111111111111111111", out ovalue));
		}

		[Test]
		public void ShouldNotGetInt64Test()
		{
			Int64 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Int64>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<Int64>("9999999999999999999", out ovalue));
		}

		[Test]
		public void ShouldNotGetSByteTest()
		{
			SByte ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<SByte>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<SByte>("1111111111111111111", out ovalue));
		}

		[Test]
		public void ShouldNotGetSingleTest()
		{
			Single ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Single>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<Single>("999,282,300,000,000,000,000,000,000,000,000,000,000.00", out ovalue));
		}

		[Test]
		public void ShouldNotGetTimeSpanTest()
		{
			TimeSpan ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<TimeSpan>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<TimeSpan>("99999999.02:48:05.4775807", out ovalue));
		}

		[Test]
		public void ShouldNotGetUInt16Test()
		{
			UInt16 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<UInt16>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<UInt16>("1111111111111111111", out ovalue));
		}

		[Test]
		public void ShouldNotGetUInt32Test()
		{
			UInt32 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<UInt32>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<UInt32>("1111111111111111111", out ovalue));
		}

		[Test]
		public void ShouldNotGetUInt64Test()
		{
			UInt64 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<UInt64>("gibberish", out ovalue));
			Assert.IsFalse(result = DataType.Instance.TryParse<UInt64>("99999999999999999999", out ovalue));
		}

		[Test]
		public void ShouldPreventNullCoaleseTest()
		{
			bool result;
			object objA, objB;

			// both null
			objA = null;
			objB = (int)0;
			result = DataType.Instance.ObjectsEqualValueSemantics(objA, objB);
			Assert.IsFalse(result);
		}

		[Test]
		public void ShouldSafeToStringTest()
		{
			Assert.AreEqual("123.456", DataType.Instance.SafeToString(123.456));
			Assert.AreEqual("123.46", DataType.Instance.SafeToString(123.456, "n"));
			Assert.AreEqual("urn:foo", DataType.Instance.SafeToString(new Uri("urn:foo"), "n"));

			Assert.AreEqual(string.Empty, DataType.Instance.SafeToString((object)null, null));
			Assert.AreEqual(null, DataType.Instance.SafeToString((object)null, null, null));
			Assert.AreEqual("1", DataType.Instance.SafeToString((object)string.Empty, null, "1", true));
		}

		[Test]
		public void ShouldSpecialGetValueOnNonNullNullableGenericTryParseTest()
		{
			int? ovalue;
			bool result;

			result = DataType.Instance.TryParse<int?>("100", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual((int?)100, ovalue);
		}

		[Test]
		public void ShouldSpecialGetValueOnNonNullNullableTryParseTest()
		{
			object ovalue;
			bool result;

			result = DataType.Instance.TryParse(typeof(int?), "100", out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual((int?)100, ovalue);
		}

		[Test]
		public void ShouldSpecialGetValueOnNullNullableGenericTryParseTest()
		{
			int? ovalue;
			bool result;

			result = DataType.Instance.TryParse<int?>(null, out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual((int?)null, ovalue);
		}

		[Test]
		public void ShouldSpecialGetValueOnNullNullableTryParseTest()
		{
			object ovalue;
			bool result;

			result = DataType.Instance.TryParse(typeof(int?), null, out ovalue);
			Assert.IsTrue(result);
			Assert.AreEqual((int?)null, ovalue);
		}

		[Test]
		public void ShouldWithNullNotGetBooleanTest()
		{
			Boolean ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Boolean>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetByteTest()
		{
			Byte ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Byte>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetCharTest()
		{
			Char ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Char>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetDateTimeOffsetTest()
		{
			DateTimeOffset ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<DateTimeOffset>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetDateTimeTest()
		{
			DateTime ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<DateTime>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetDecimalTest()
		{
			Decimal ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Decimal>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetDoubleTest()
		{
			Double ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Double>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetEnumTest()
		{
			CharSet ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<CharSet>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetGuidTest()
		{
			Guid ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Guid>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetInt16Test()
		{
			Int16 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Int16>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetInt32Test()
		{
			Int32 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Int32>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetInt64Test()
		{
			Int64 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Int64>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetSByteTest()
		{
			SByte ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<SByte>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetSingleTest()
		{
			Single ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<Single>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetTimeSpanTest()
		{
			TimeSpan ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<TimeSpan>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetUInt16Test()
		{
			UInt16 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<UInt16>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetUInt32Test()
		{
			UInt32 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<UInt32>(null, out ovalue));
		}

		[Test]
		public void ShouldWithNullNotGetUInt64Test()
		{
			UInt64 ovalue;
			bool result;

			Assert.IsFalse(result = DataType.Instance.TryParse<UInt64>(null, out ovalue));
		}

		#endregion
	}
}