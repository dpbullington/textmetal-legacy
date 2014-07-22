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
using TextMetal.Utilities.DataObfu.ConsoleTool.Config;

namespace TextMetal.Utilities.DataObfu.ConsoleTool
{
	public sealed class ObfuscationDataReader : IDataReader
	{
		#region Constructors/Destructors

		public ObfuscationDataReader(IDataReader dataReader, TableConfiguration tableConfiguration)
		{
			if ((object)dataReader == null)
				throw new ArgumentNullException("dataReader");

			if ((object)tableConfiguration == null)
				throw new ArgumentNullException("tableConfiguration");

			this.dataReader = dataReader;
			this.tableConfiguration = tableConfiguration;
		}

		#endregion

		#region Fields/Constants

		private readonly IDataReader dataReader;
		private readonly TableConfiguration tableConfiguration;

		#endregion

		#region Properties/Indexers/Events

		object IDataRecord.this[string name]
		{
			get
			{
				return this.DataReader[name];
			}
		}

		object IDataRecord.this[int i]
		{
			get
			{
				return this.DataReader[i];
			}
		}

		private IDataReader DataReader
		{
			get
			{
				return this.dataReader;
			}
		}

		int IDataReader.Depth
		{
			get
			{
				return this.DataReader.Depth;
			}
		}

		int IDataRecord.FieldCount
		{
			get
			{
				return this.DataReader.FieldCount;
			}
		}

		bool IDataReader.IsClosed
		{
			get
			{
				return this.DataReader.IsClosed;
			}
		}

		int IDataReader.RecordsAffected
		{
			get
			{
				return this.DataReader.RecordsAffected;
			}
		}

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

			/*using (IUnitOfWork unitOfWork = UnitOfWork.From(getDictionaryConnectionCallback(), null))
			{
				int ra;
				string sql;
				IDataParameter dataParameterKey;
				IList<IDictionary<string, object>> results;

				if (unitOfWork.Connection is SqlConnection)
					sql = "SELECT TOP 1 z.[{0}] FROM {1} z WHERE z.[{2}] = @key";
				else
					sql = "SELECT TOP 1 z.[{0}] FROM {1} z WHERE z.[{2}] = ?";

				sql = string.Format(sql, dictionaryConfiguration.External.ValueMemberName,
					dictionaryConfiguration.External.Source,
					dictionaryConfiguration.External.KeyMemberName);

				dataParameterKey = unitOfWork.CreateParameter(ParameterDirection.Input, DbType.Object, 0, 0, 0, false, "@key", valueHash);
				results = unitOfWork.ExecuteDictionary(CommandType.Text, sql, new IDataParameter[] { dataParameterKey }, out ra);

				if ((object)results == null || results.Count != 1)
					throw new InvalidOperationException("External dictionary results invalid.");

				value = results[0][dictionaryConfiguration.External.ValueMemberName];
			}*/

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

		void IDataReader.Close()
		{
			this.DataReader.Close();
		}

		void IDisposable.Dispose()
		{
			this.DataReader.Dispose();
		}

		bool IDataRecord.GetBoolean(int i)
		{
			return this.DataReader.GetBoolean(i);
		}

		byte IDataRecord.GetByte(int i)
		{
			return this.DataReader.GetByte(i);
		}

		long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return this.DataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		char IDataRecord.GetChar(int i)
		{
			return this.DataReader.GetChar(i);
		}

		long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return this.DataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		IDataReader IDataRecord.GetData(int i)
		{
			return this.DataReader.GetData(i);
		}

		string IDataRecord.GetDataTypeName(int i)
		{
			return this.DataReader.GetDataTypeName(i);
		}

		DateTime IDataRecord.GetDateTime(int i)
		{
			return this.DataReader.GetDateTime(i);
		}

		decimal IDataRecord.GetDecimal(int i)
		{
			return this.DataReader.GetDecimal(i);
		}

		double IDataRecord.GetDouble(int i)
		{
			return this.DataReader.GetDouble(i);
		}

		Type IDataRecord.GetFieldType(int i)
		{
			return this.DataReader.GetFieldType(i);
		}

		float IDataRecord.GetFloat(int i)
		{
			return this.DataReader.GetFloat(i);
		}

		Guid IDataRecord.GetGuid(int i)
		{
			return this.DataReader.GetGuid(i);
		}

		short IDataRecord.GetInt16(int i)
		{
			return this.DataReader.GetInt16(i);
		}

		int IDataRecord.GetInt32(int i)
		{
			return this.DataReader.GetInt32(i);
		}

		long IDataRecord.GetInt64(int i)
		{
			return this.DataReader.GetInt64(i);
		}

		string IDataRecord.GetName(int i)
		{
			return this.DataReader.GetName(i);
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
				(long)this.TableConfiguration.Parent.ValueHash.Size,
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

		int IDataRecord.GetOrdinal(string name)
		{
			return this.DataReader.GetOrdinal(name);
		}

		DataTable IDataReader.GetSchemaTable()
		{
			return this.DataReader.GetSchemaTable();
		}

		string IDataRecord.GetString(int i)
		{
			return this.DataReader.GetString(i);
		}

		object IDataRecord.GetValue(int i)
		{
			string columnName;
			Type columnType;
			object columnValue, obfusscatedValue;

			columnName = this.DataReader.GetName(i);
			columnType = this.DataReader.GetFieldType(i);
			columnValue = this.DataReader.GetValue(i);

			obfusscatedValue = this.GetObfuscatedValue(columnName, columnType, columnValue);

			return obfusscatedValue;
		}

		int IDataRecord.GetValues(object[] values)
		{
			return this.DataReader.GetValues(values);
		}

		bool IDataRecord.IsDBNull(int i)
		{
			return this.DataReader.IsDBNull(i);
		}

		bool IDataReader.NextResult()
		{
			return this.DataReader.NextResult();
		}

		bool IDataReader.Read()
		{
			return this.DataReader.Read();
		}

		#endregion
	}
}