/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using TextMetal.Common.Core;
using TextMetal.Common.Data;
using TextMetal.Utilities.DataObfu.ConsoleTool.Config;

namespace TextMetal.Utilities.DataObfu.ConsoleTool
{
	public sealed class ObfuscationDataReader : WrapperDataReader
	{
		#region Constructors/Destructors

		public ObfuscationDataReader(IDataReader dataReader, TableConfiguration tableConfiguration)
			: base(dataReader)
		{
			if ((object)tableConfiguration == null)
				throw new ArgumentNullException("tableConfiguration");

			this.tableConfiguration = tableConfiguration;
		}

		#endregion

		#region Fields/Constants

		private static readonly IDictionary<string, IDictionary<long?, object>> substitutionCacheRoot = new Dictionary<string, IDictionary<long?, object>>();

		private readonly TableConfiguration tableConfiguration;

		#endregion

		#region Properties/Indexers/Events

		private TableConfiguration TableConfiguration
		{
			get
			{
				return this.tableConfiguration;
			}
		}

		#endregion

		#region Methods/Operators

		private static object GetCipher(string sharedSecret, object value)
		{
			const string INIT_VECTOR = "textmetal_was_here";
			const int KEY_SIZE = 256;

			byte[] initVectorBytes;
			byte[] plainTextBytes;
			ICryptoTransform encryptor;
			byte[] keyBytes;
			byte[] cipherTextBytes;
			Type valueType;
			string _value;

			if ((object)sharedSecret == null)
				throw new ArgumentNullException("sharedSecret");

			if ((object)value == null)
				return null;

			valueType = value.GetType();

			if (valueType != typeof(String))
				return null;

			_value = (String)value;

			if (DataType.IsWhiteSpace(_value))
				return _value;

			_value = _value.Trim();

			initVectorBytes = Encoding.UTF8.GetBytes(INIT_VECTOR);
			plainTextBytes = Encoding.UTF8.GetBytes(_value);

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

			return Encoding.UTF8.GetString(cipherTextBytes);
		}

		private static object GetDefault(bool isNullable, Type valueType)
		{
			if ((object)valueType == null)
				throw new ArgumentNullException("valueType");

			//if (isNullable)
			//return null;

			if (valueType == typeof(String))
				return string.Empty;

			return DataType.DefaultValue(valueType);
		}

		private static long? GetHash(long hashMultiplier, long hashBucketSize, long hashSeed, object value)
		{
			const long INVALID_HASH = -1;
			long hashCode;
			byte[] buffer;
			Type valueType;
			string _value;

			if (hashBucketSize == 0)
				return null; // DIV0

			if ((object)value == null)
				return null;

			valueType = value.GetType();

			if (valueType != typeof(String))
				return null;

			_value = (String)value;

			if (DataType.IsWhiteSpace(_value))
				return INVALID_HASH;

			_value = _value.Trim();

			buffer = Encoding.UTF8.GetBytes(_value);

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

		private static object GetMask(double maskFactor, object value)
		{
			StringBuilder sb;
			Type valueType;
			string _value;

			if ((int)(maskFactor * 100) > 100)
				throw new ArgumentOutOfRangeException("maskFactor");

			if ((int)(maskFactor * 100) == 000)
				throw new ArgumentOutOfRangeException("maskFactor");

			if ((int)(maskFactor * 100) < -100)
				throw new ArgumentOutOfRangeException("maskFactor");

			if ((object)value == null)
				return null;

			valueType = value.GetType();

			if (valueType != typeof(String))
				return null;

			_value = (String)value;

			if (DataType.IsWhiteSpace(_value))
				return _value;

			_value = _value.Trim();

			sb = new StringBuilder(_value);

			if (Math.Sign(maskFactor) == 1)
			{
				for (int index = 0; index < (int)Math.Round((double)_value.Length * maskFactor); index++)
					sb[index] = '*';
			}
			else if (Math.Sign(maskFactor) == -1)
			{
				for (int index = _value.Length - (int)Math.Round((double)_value.Length * Math.Abs(maskFactor)); index < _value.Length; index++)
					sb[index] = '*';
			}
			else
				throw new InvalidOperationException("maskFactor");

			return sb.ToString();
		}

		private static object GetShuffle(long? randomSeed, object value)
		{
			Random random;
			Type valueType;
			string _value;

			if ((object)value == null)
				return null;

			valueType = value.GetType();

			if (valueType != typeof(String))
				return null;

			_value = (String)value;

			if (DataType.IsWhiteSpace(_value))
				return _value;

			_value = _value.Trim();

			random = new Random((int)(long)randomSeed);
			var fidelityMap = ImplNormalize(ref _value);
			_value = new string(_value.ToCharArray().OrderBy(s => random.Next(int.MaxValue)).ToArray());
			ImplDenormalize(fidelityMap, ref _value);

			return _value;
		}

		private static object GetSubstitution(DictionaryConfiguration dictionaryConfiguration, long? valueHash, object value)
		{
			Type valueType;
			string _value;

			IDictionary<long?, object> dictionaryCache = null;

			if ((object)dictionaryConfiguration == null)
				throw new ArgumentNullException("dictionaryConfiguration");

			if ((object)value == null)
				return null;

			valueType = value.GetType();

			if (valueType != typeof(String))
				return null;

			_value = (String)value;

			if (DataType.IsWhiteSpace(_value))
				return _value;

			_value = _value.Trim();

			//Console.WriteLine("substitutionCacheRoot-count: {0}", substitutionCacheRoot.Count);

			if (dictionaryConfiguration.RecordCount < 100000)
			{
				if (substitutionCacheRoot.TryGetValue(dictionaryConfiguration.DictionaryId, out dictionaryCache))
				{
					//Console.WriteLine("cache-count: {0}", dictionaryCache.Count);

					if (dictionaryCache.TryGetValue(valueHash, out value))
						return value;

					//Console.WriteLine("Cache miss.");
				}
				else
				{
					//Console.WriteLine("Dictionary miss.");

					dictionaryCache = new Dictionary<long?, object>();
					substitutionCacheRoot.Add(dictionaryConfiguration.DictionaryId, dictionaryCache);
				}
			}

			using (IUnitOfWork dictionaryUnitOfWork = UnitOfWork.Create(dictionaryConfiguration.Parent.GetDictionaryConnectionType(), dictionaryConfiguration.Parent.DictionaryConnectionString, false))
			{
				IDbDataParameter dbDataParameterKey;

				dbDataParameterKey = dictionaryUnitOfWork.CreateParameter(ParameterDirection.Input, DbType.Object, 0, 0, 0, false, "@ID", valueHash);

				value = dictionaryUnitOfWork.FetchScalar<string>(CommandType.Text, dictionaryConfiguration.DictionaryCommandText, new IDbDataParameter[] { dbDataParameterKey });
			}

			if ((object)dictionaryCache != null)
				dictionaryCache.Add(valueHash, value);

			return value;
		}

		private static object GetVariance(double varianceFactor, object value)
		{
			Type valueType;
			Type openNullableType;
			object originalValue;

			if ((object)value == null)
				return null;

			originalValue = value;
			valueType = value.GetType();
			openNullableType = typeof(Nullable<>);

			if (valueType.IsGenericType &&
				!valueType.IsGenericTypeDefinition &&
				valueType.GetGenericTypeDefinition().Equals(openNullableType))
				valueType = valueType.GetGenericArguments()[0];

			if (valueType == typeof(Boolean))
				value = (Math.Sign(varianceFactor) >= 0) ? true : false;
			else if (valueType == typeof(SByte))
				value = (SByte)value + (SByte)(varianceFactor * (Double)(SByte)value);
			else if (valueType == typeof(Int16))
				value = (Int16)value + (Int16)(varianceFactor * (Double)(Int16)value);
			else if (valueType == typeof(Int32))
				value = (Int32)value + (Int32)(varianceFactor * (Double)(Int32)value);
			else if (valueType == typeof(Int64))
				value = (Int64)value + (Int64)(varianceFactor * (Double)(Int64)value);
			else if (valueType == typeof(Byte))
				value = (Byte)value + (Byte)(varianceFactor * (Double)(Byte)value);
			else if (valueType == typeof(UInt16))
				value = (UInt16)value + (UInt16)(varianceFactor * (Double)(UInt16)value);
			else if (valueType == typeof(Int32))
				value = (UInt32)value + (UInt32)(varianceFactor * (Double)(UInt32)value);
			else if (valueType == typeof(UInt64))
				value = (UInt64)value + (UInt64)(varianceFactor * (Double)(UInt64)value);
			else if (valueType == typeof(Decimal))
				value = (Decimal)value + ((Decimal)varianceFactor * (Decimal)value);
			else if (valueType == typeof(Single))
				value = (Single)value + (Single)(varianceFactor * (Double)(Single)value);
			else if (valueType == typeof(Double))
				value = (Double)value + (Double)(varianceFactor * (Double)value);
			else if (valueType == typeof(Char))
				value = (Char)value + (Char)(varianceFactor * (Char)value);
			else if (valueType == typeof(DateTime))
				value = ((DateTime)value).AddDays((Double)(varianceFactor * 365.25));
			else if (valueType == typeof(DateTimeOffset))
				value = ((DateTimeOffset)value).AddDays((Double)(varianceFactor * 365.25));
			else if (valueType == typeof(TimeSpan))
				value = ((TimeSpan)value).Add(TimeSpan.FromDays((Double)(varianceFactor * 365.25)));
			else // unsupported type
				value = null;

			// roll a recursive doubler call until a new value is achieved
			if (DataType.ObjectsEqualValueSemantics(originalValue, value))
				return GetVariance(varianceFactor * 2.0, value);

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

		private object GetObfuscatedValue(string columnName, Type columnType, object columnValue)
		{
			IEnumerable<Message> messages;
			ColumnConfiguration columnConfiguration;
			DictionaryConfiguration dictionaryConfiguration;
			long? valueHash, signHash;

			if ((object)columnName == null)
				throw new ArgumentNullException("columnName");

			if ((object)columnType == null)
				throw new ArgumentNullException("columnType");

			messages = this.TableConfiguration.Validate();

			if (messages.Any())
				throw new ConfigurationException(string.Format("Obfuscation configuration validation failed:\r\n{0}", string.Join("\r\n", messages.Select(m => m.Description).ToArray())));

			columnConfiguration = this.TableConfiguration.Columns.SingleOrDefault(c => c.ColumnName.SafeToString().Trim().ToLower() == columnName.SafeToString().Trim().ToLower());

			if ((object)columnConfiguration == null)
				return columnValue; // do nothing when no matching column configuration

			if (columnConfiguration.DictionaryRef.SafeToString().Trim().ToLower() == "")
				dictionaryConfiguration = new DictionaryConfiguration();
			else
				dictionaryConfiguration = this.TableConfiguration.Parent.Dictionaries.SingleOrDefault(d => d.DictionaryId.SafeToString().Trim().ToLower() == columnConfiguration.DictionaryRef.SafeToString().Trim().ToLower());

			if ((object)dictionaryConfiguration == null)
				throw new ConfigurationException(string.Format("Unknown dictionary reference '{0}' specified for column '{1}'.", columnConfiguration.DictionaryRef, columnName));

			signHash = GetHash((long)this.TableConfiguration.Parent.SignHash.Multiplier,
				(long)this.TableConfiguration.Parent.SignHash.Size,
				(long)this.TableConfiguration.Parent.SignHash.Seed, columnValue.SafeToString()) ?? 0;

			valueHash = GetHash((long)this.TableConfiguration.Parent.ValueHash.Multiplier,
				columnConfiguration.ObfuscationStrategy == ObfuscationStrategy.Substitution ?
					(long)dictionaryConfiguration.RecordCount : (long)this.TableConfiguration.Parent.ValueHash.Size,
				(long)this.TableConfiguration.Parent.ValueHash.Seed, columnValue.SafeToString()) ?? 0;

			switch (columnConfiguration.ObfuscationStrategy)
			{
				case ObfuscationStrategy.Substitution:
					return GetSubstitution(dictionaryConfiguration, valueHash, columnValue);
				case ObfuscationStrategy.Shuffling:
					return GetShuffle(valueHash, columnValue);
				case ObfuscationStrategy.Variance:
					var varianceFactor =
						((((double)((long)valueHash <= 0 ? 1 : (long)valueHash)) *
						((double)((long)signHash % 2 == 0 ? 1 : -1)))
						/ (double)100.0);
					return GetVariance(varianceFactor, columnValue);
				case ObfuscationStrategy.Ciphering:
					var sharedSecret =
						((((long)valueHash <= 0 ? 1 : (long)valueHash) *
						((long)signHash % 2 == 0 ? -1 : 1))).ToString("X");
					return GetCipher(sharedSecret, columnValue);
				case ObfuscationStrategy.Defaulting:
					return GetDefault(false, columnType);
				case ObfuscationStrategy.Masking:
					return GetMask((double)columnConfiguration.MaskFactor, columnValue);
				case ObfuscationStrategy.None:
					return columnValue;
				default:
					throw new ConfigurationException(string.Format("Unknown obfuscation strategy '{0}' specified for column '{1}'.", columnConfiguration.ObfuscationStrategy, columnName));
			}
		}

		public override object GetValue(int i)
		{
			string columnName;
			Type columnType;
			object columnValue, obfusscatedValue;

			columnName = base.GetName(i);
			columnType = base.GetFieldType(i);
			columnValue = base.GetValue(i);

			obfusscatedValue = this.GetObfuscatedValue(columnName, columnType, columnValue);

			return obfusscatedValue;
		}

		#endregion
	}
}