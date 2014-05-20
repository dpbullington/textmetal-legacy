/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using Microsoft.SqlServer.Server;

using TextMetal.Common.Core;

namespace TextMetal.Common.SqlServerClr
{
	public static class ScalarFunctions
	{
		#region Methods/Operators

		public static string GetCipher(string sharedSecret, string value)
		{
			const string INIT_VECTOR = "textmetalWasHere";
			const int KEY_SIZE = 256;

			byte[] initVectorBytes;
			byte[] plainTextBytes;
			ICryptoTransform encryptor;
			byte[] keyBytes;
			byte[] cipherTextBytes;

			if ((object)sharedSecret == null)
				throw new ArgumentNullException("sharedSecret");

			if (DataType.IsNullOrWhiteSpace(value))
				return value;

			initVectorBytes = Encoding.UTF8.GetBytes(INIT_VECTOR);
			plainTextBytes = Encoding.GetEncoding("UCS-2").GetBytes(value);

			using (PasswordDeriveBytes password = new PasswordDeriveBytes(sharedSecret, null))
				keyBytes = password.GetBytes(KEY_SIZE / 8);

			using (RijndaelManaged symmetricKey = new RijndaelManaged())
			{
				symmetricKey.Mode = CipherMode.CBC;
				encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
					{
						cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
						cryptoStream.FlushFinalBlock();
						cipherTextBytes = memoryStream.ToArray();
					}
				}
			}

			return Encoding.GetEncoding("UCS-2").GetString(cipherTextBytes);
		}

		public static object GetDefault(bool isNullable, string sqlType)
		{
			if ((object)sqlType == null)
				throw new ArgumentNullException("sqlType");

			switch (sqlType = sqlType.SafeToString().ToUpper())
			{
				case "BIGINT":
					return (isNullable ? (Int64?)null : (Int64)0);
				case "BINARY":
					return typeof(Byte[]);
				case "BIT":
					return (isNullable ? (Boolean?)null : (Boolean)false);
				case "CHARACTER":
				case "CHAR":
					return (isNullable ? null : string.Empty);
				case "CURSOR":
					return (isNullable ? (Object)null : (Object)new object());
				case "DATE":
					return (isNullable ? (DateTime?)null : (DateTime)DateTime.MinValue);
				case "DATETIME":
					return (isNullable ? (DateTime?)null : (DateTime)DateTime.MinValue);
				case "DATETIME2":
					return (isNullable ? (DateTime?)null : (DateTime)DateTime.MinValue);
				case "DATETIMEOFFSET":
					return (isNullable ? (DateTimeOffset?)null : (DateTimeOffset)DateTimeOffset.MinValue);
				case "DEC":
				case "DECIMAL":
					return (isNullable ? (Decimal?)null : (Decimal)0.0);
				case "DOUBLE PRECISION":
				case "FLOAT":
					// ignore precision
					return (isNullable ? (Single?)null : (Single)0.0);
				case "GEOGRAPHY":
					return (isNullable ? (Object)null : (Object)new object());
				case "GEOMETRY":
					return (isNullable ? (Object)null : (Object)new object());
				case "HIERARCHYID":
					return (isNullable ? (Object)null : (Object)new object());
				case "IMAGE":
					return (isNullable ? (Byte[])null : (Byte[])new byte[0]);
				case "INTEGER":
				case "INT":
					return (isNullable ? (Int32?)null : (Int32)0);
				case "MONEY":
					return (isNullable ? (Decimal?)null : (Decimal)0.0);
				case "NATIONAL CHARACTER":
				case "NATIONAL CHAR":
				case "NCHAR":
					return (isNullable ? null : string.Empty);
				case "NATIONAL TEXT":
				case "NTEXT":
					return (isNullable ? null : string.Empty);
				case "NUMERIC":
					return (isNullable ? (Decimal?)null : (Decimal)0.0);
				case "NATIONAL CHARACTER VARYING":
				case "NATIONAL CHAR VARYING":
				case "NVARCHAR":
					return (isNullable ? null : string.Empty);
				case "REAL":
					return (isNullable ? (Single?)null : (Single)0.0);
				case "TIMESTAMP":
				case "ROWVERSION":
					return typeof(Byte[]);
				case "SMALLDATETIME":
					return (isNullable ? (DateTime?)null : (DateTime)DateTime.MinValue);
				case "SMALLINT":
					return (isNullable ? (Int16?)null : (Int16)0);
				case "SMALLMONEY":
					return (isNullable ? (Decimal?)null : (Decimal)0.0);
				case "SQL_VARIANT":
					return (isNullable ? (Object)null : (Object)new object());
				case "SYSNAME":
					return (isNullable ? null : string.Empty);
				case "TABLE":
					return (isNullable ? (Object)null : (Object)new object());
				case "TEXT":
					return (isNullable ? null : string.Empty);
				case "TIME":
					return (isNullable ? (TimeSpan?)null : (TimeSpan)TimeSpan.Zero);
				case "TINYINT":
					return (isNullable ? (Byte?)null : (Byte)0);
				case "UNIQUEIDENTIFIER":
					return (isNullable ? (Guid?)null : (Guid)Guid.Empty);
				case "BINARY VARYING":
				case "VARBINARY":
					return (isNullable ? (Byte[])null : (Byte[])new byte[0]);
				case "CHARACTER VARYING":
				case "CHAR VARYING":
				case "VARCHAR":
					return (isNullable ? null : string.Empty);
				case "XML":
					return (isNullable ? (XmlDocument)null : (XmlDocument)new XmlDocument());
				default:
					throw new ArgumentOutOfRangeException(string.Format("Unsupported parameter type: '{0}'.", sqlType));
			}
		}

		public static long? GetHash(long hashMultiplier, long hashBucketSize, long hashSeed, string value)
		{
			long hashCode;
			byte[] buffer;

			if (DataType.IsNullOrWhiteSpace(value))
				return null;

			buffer = Encoding.GetEncoding("UCS-2").GetBytes(value);

			hashCode = hashSeed;
			for (int index = 0; index < buffer.Length; index++)
				hashCode = (hashMultiplier * hashCode + buffer[index]) % uint.MaxValue;

			if (hashCode > int.MaxValue)
				hashCode = hashCode - uint.MaxValue;

			if (hashCode < 0)
				hashCode = hashCode + int.MaxValue;

			hashCode = hashCode % hashBucketSize;

			return (int)hashCode;
		}

		public static string GetMask(double maskFactor, string value)
		{
			StringBuilder sb;

			if ((int)(maskFactor * 100) > 100)
				throw new ArgumentOutOfRangeException("maskFactor");

			if ((int)(maskFactor * 100) == 000)
				throw new ArgumentOutOfRangeException("maskFactor");

			if ((int)(maskFactor * 100) < -100)
				throw new ArgumentOutOfRangeException("maskFactor");

			if (DataType.IsNullOrWhiteSpace(value))
				return value;

			sb = new StringBuilder(value);

			if (Math.Sign(maskFactor) == 1)
			{
				for (int index = 0; index < (int)Math.Round((double)value.Length * maskFactor); index++)
					sb[index] = '*';
			}
			else if (Math.Sign(maskFactor) == -1)
			{
				for (int index = value.Length - (int)Math.Round((double)value.Length * Math.Abs(maskFactor)); index < value.Length; index++)
					sb[index] = '*';
			}
			else
				throw new InvalidOperationException("maskFactor");

			return sb.ToString();
		}

		public static string GetShuffle(long randomSeed, string value)
		{
			Random random;

			if (DataType.IsNullOrWhiteSpace(value))
				return value;

			random = new Random((int)randomSeed);
			var fidelityMap = ImplNormalize(ref value);
			value = new string(value.ToCharArray().OrderBy(s => random.Next(int.MaxValue)).ToArray());
			ImplDenormalize(fidelityMap, ref value);

			return value;
		}

		public static object GetVariance(double varianceFactor, object value)
		{
			if ((object)value == null)
				return null;

			if (value is SqlBoolean)
				value = (Math.Sign(varianceFactor) >= 0) ? SqlBoolean.True : SqlBoolean.False;
			else if (value is SqlByte)
				value = (SqlByte)value + (SqlByte)(varianceFactor * (double)(SqlByte)value);
			else if (value is SqlInt16)
				value = (SqlInt16)value + (SqlInt16)(varianceFactor * (double)(SqlInt16)value);
			else if (value is SqlInt32)
				value = (SqlInt32)value + (SqlInt32)(varianceFactor * (double)(SqlInt32)value);
			else if (value is SqlInt64)
				value = (SqlInt64)value + (SqlInt64)(varianceFactor * (double)(SqlInt64)value);
			else if (value is SqlDecimal)
				value = (SqlDecimal)value + (SqlDecimal)((decimal)varianceFactor * (decimal)(SqlDecimal)value);
			else if (value is SqlSingle)
				value = (SqlSingle)value + (SqlSingle)(varianceFactor * (double)(SqlSingle)value);
			else if (value is SqlDouble)
				value = (SqlDouble)value + (SqlDouble)(varianceFactor * (double)(SqlDouble)value);
			else if (value is SqlDateTime)
				value = SqlDateTime.Add(((SqlDateTime)value), TimeSpan.FromDays((Double)(varianceFactor * 365.25)));
			else
				throw new InvalidOperationException(string.Format("Unsupported parameter type: '{0}'.", value.GetType().FullName));

			return value;
		}

		private static void ImplDenormalize(Dictionary<int, char> fidelityMap, ref string value)
		{
			StringBuilder sb;
			char ch;
			int offset = 0;

			if ((object)fidelityMap == null)
				throw new ArgumentNullException("fidelityMap");

			if (DataType.IsNullOrWhiteSpace(value))
				return;

			sb = new StringBuilder(value);

			for (int index = 0; index < value.Length; index++)
			{
				if (fidelityMap.TryGetValue(index, out ch))
				{
					sb.Insert(index, ch);
					offset++;
				}
			}

			value = sb.ToString();
		}

		private static Dictionary<int, char> ImplNormalize(ref string value)
		{
			StringBuilder sb;
			Dictionary<int, char> fidelityMap;

			fidelityMap = new Dictionary<int, char>();

			if (DataType.IsNullOrWhiteSpace(value))
			{
				value = value;
				return fidelityMap;
			}

			sb = new StringBuilder();

			// 212-555-1234 => 2125551212 => 1945687302 => 194-568-7302
			for (int index = 0; index < value.Length; index++)
			{
				if (char.IsLetterOrDigit(value[index]))
					sb.Append(value[index]);
				else
					fidelityMap.Add(index, value[index]);
			}

			value = sb.ToString();
			return fidelityMap;
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static object fn_GetCipher(string sharedSecret, object value)
		{
			if (!(value is string || value is SqlString))
				return null;

			return GetCipher(sharedSecret, value.SafeToString(null, null));
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static object fn_GetDefault(bool isNullable, string sqlType)
		{
			return GetDefault(isNullable, sqlType);
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static long? fn_GetHash(long hashMultiplier, long hashBucketSize, long hashSeed, string value)
		{
			if ((object)value == null)
				return -1;

			if (DataType.IsWhiteSpace(value))
				return -1;

			if (!(value is string || value is SqlString))
				return null;

			return GetHash(hashMultiplier, hashBucketSize, hashSeed, value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static object fn_GetMask(double maskFactor, object value)
		{
			if (!(value is string || value is SqlString))
				return null;

			return GetMask(maskFactor, value.SafeToString(null, null));
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static object fn_GetShuffle(long randomSeed, object value)
		{
			if (!(value is string || value is SqlString))
				return null;

			return GetShuffle(randomSeed, value.SafeToString(null, null));
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static object fn_GetVariance(double varianceFactor, object value)
		{
			if ((value is string || value is SqlString))
				return null;

			return GetVariance(varianceFactor, value.ChangeType<object>());
		}

		#endregion
	}
}