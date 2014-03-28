/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

using Microsoft.SqlServer.Server;

using TextMetal.Common.Core;

namespace TextMetal.Common.SqlServerClr
{
	public static class ScalarFunctions
	{
		#region Fields/Constants

		private const string CONTEXT_CONNECTION_STRING = "context connection=true";

		#endregion

		#region Methods/Operators

		private static void Denormalize(Dictionary<int, char> fidelityMap, ref string value)
		{
			StringBuilder sb;
			char ch;
			int offset = 0;

			if ((object)fidelityMap == null)
				throw new ArgumentNullException("fidelityMap");

			if ((object)value == null)
				value = null;

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

		public static string ExecuteHashShuffle(string connectionString, int dictionaryConfigId, object value)
		{
			string _value;
			long hashMultiplier;
			long hashBucketSize;
			long hashSeed;

			long dictionaryKey;
			string dictionaryValue;

			IDbDataParameter dbDataParameter;

			_value = value.SafeToString(null, null);

			if (DataType.IsNullOrWhiteSpace(_value))
				return _value;

			using (IDbConnection dbConnection = new SqlConnection(connectionString))
			{
				dbConnection.Open();

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT
						t.[HashMultiplier],
						t.[HashBucketSize],
						t.[HashSeed]
						FROM [DataObfuscation].[DictionaryConfig] t
						WHERE t.[DictionaryConfigId] = @DictionaryConfigId;";

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@DictionaryConfigId";
					dbDataParameter.Value = dictionaryConfigId;

					dbCommmand.Parameters.Add(dbDataParameter);

					using (IDataReader dataReader = dbCommmand.ExecuteReader())
					{
						if (!dataReader.Read())
							return null;

						hashMultiplier = dataReader["HashMultiplier"].ChangeType<long>();
						hashBucketSize = dataReader["HashBucketSize"].ChangeType<long>();
						hashSeed = dataReader["HashSeed"].ChangeType<long>();
					}
				}

				var fidelityMap = Normalize(ref _value);
				dictionaryKey = GetHash(hashMultiplier, hashBucketSize, hashSeed, _value);

				dictionaryValue = GetShuffle(dictionaryKey, _value);
				Denormalize(fidelityMap, ref dictionaryValue);
			}

			return dictionaryValue;
		}

		public static string ExecuteHashSubstitution(string connectionString, int dictionaryConfigId, object value)
		{
			long hashMultiplier;
			long hashBucketSize;
			long hashSeed;

			long dictionaryKey;
			string dictionaryValue;

			IDbDataParameter dbDataParameter;

			using (IDbConnection dbConnection = new SqlConnection(connectionString))
			{
				dbConnection.Open();

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT
						t.[HashMultiplier],
						t.[HashBucketSize],
						t.[HashSeed]
						FROM [DataObfuscation].[DictionaryConfig] t
						WHERE t.[DictionaryConfigId] = @DictionaryConfigId;";

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@DictionaryConfigId";
					dbDataParameter.Value = dictionaryConfigId;

					dbCommmand.Parameters.Add(dbDataParameter);

					using (IDataReader dataReader = dbCommmand.ExecuteReader())
					{
						if (!dataReader.Read())
							return null;

						hashMultiplier = dataReader["HashMultiplier"].ChangeType<long>();
						hashBucketSize = dataReader["HashBucketSize"].ChangeType<long>();
						hashSeed = dataReader["HashSeed"].ChangeType<long>();
					}
				}

				dictionaryKey = GetHash(hashMultiplier, hashBucketSize, hashSeed, value);

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT
						t.[Value]
						FROM [DataObfuscation].[MasterDictionary] t
						WHERE t.[DictionaryConfigId] = @DictionaryConfigId AND t.[Key] = @DictionaryKey;";

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@DictionaryConfigId";
					dbDataParameter.Value = dictionaryConfigId;

					dbCommmand.Parameters.Add(dbDataParameter);

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@DictionaryKey";
					dbDataParameter.Value = dictionaryKey;

					dbCommmand.Parameters.Add(dbDataParameter);

					using (IDataReader dataReader = dbCommmand.ExecuteReader())
					{
						if (!dataReader.Read())
							return null;

						dictionaryValue = dataReader["Value"].ChangeType<string>();
					}
				}
			}

			return dictionaryValue;
		}

		public static object ExecuteHashVariance(string connectionString, int dictionaryConfigId, object value)
		{
			const long SIGN_WELL_KNOWN_DICTIONARY_CONFIG = 0;
			long hashMultiplier;
			long hashBucketSize;
			long hashSeed;

			long dictionaryKey;
			object dictionaryValue;

			long signFactor;
			double varianceFactor;

			IDbDataParameter dbDataParameter;

			if (DataType.IsNullOrWhiteSpace(value.SafeToString(null, null)))
				return value;

			using (IDbConnection dbConnection = new SqlConnection(connectionString))
			{
				dbConnection.Open();

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT
						t.[HashMultiplier],
						t.[HashBucketSize],
						t.[HashSeed]
						FROM [DataObfuscation].[DictionaryConfig] t
						WHERE t.[DictionaryConfigId] = @DictionaryConfigId;";

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@DictionaryConfigId";
					dbDataParameter.Value = SIGN_WELL_KNOWN_DICTIONARY_CONFIG;

					dbCommmand.Parameters.Add(dbDataParameter);

					using (IDataReader dataReader = dbCommmand.ExecuteReader())
					{
						if (!dataReader.Read())
							return null;

						hashMultiplier = dataReader["HashMultiplier"].ChangeType<long>();
						hashBucketSize = dataReader["HashBucketSize"].ChangeType<long>();
						hashSeed = dataReader["HashSeed"].ChangeType<long>();
					}
				}

				dictionaryKey = GetHash(hashMultiplier, hashBucketSize, hashSeed, value);
				signFactor = (dictionaryKey % 2) == 0 ? 1 : -1;

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT
						t.[HashMultiplier],
						t.[HashBucketSize],
						t.[HashSeed]
						FROM [DataObfuscation].[DictionaryConfig] t
						WHERE t.[DictionaryConfigId] = @DictionaryConfigId;";

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@DictionaryConfigId";
					dbDataParameter.Value = dictionaryConfigId;

					dbCommmand.Parameters.Add(dbDataParameter);

					using (IDataReader dataReader = dbCommmand.ExecuteReader())
					{
						if (!dataReader.Read())
							return null;

						hashMultiplier = dataReader["HashMultiplier"].ChangeType<long>();
						hashBucketSize = dataReader["HashBucketSize"].ChangeType<long>();
						hashSeed = dataReader["HashSeed"].ChangeType<long>();
					}
				}

				dictionaryKey = GetHash(hashMultiplier, hashBucketSize, hashSeed, value);
				varianceFactor = (dictionaryKey * signFactor) / 100.0;

				dictionaryValue = GetVariance(varianceFactor * signFactor, value, true);
			}

			return dictionaryValue;
		}

		public static object ExecuteStrategy(string connectionString, int strategyId, int dictionaryConfigId, object value)
		{
			Strategy strategy;

			strategy = (Strategy)(uint)strategyId;

			switch (strategy)
			{
				case Strategy.Unknown:
				case Strategy.None:
					return value;
				case Strategy.Substitution:
					return ExecuteHashSubstitution(connectionString, dictionaryConfigId, value);
				case Strategy.Shuffling:
					return ExecuteHashShuffle(connectionString, dictionaryConfigId, value);
				case Strategy.Variance:
					return ExecuteHashVariance(connectionString, dictionaryConfigId, value);
				case Strategy.Cipher:
					throw new NotImplementedException();
				case Strategy.Nulling:
					return GetNull(false, value.SafeToString(null, null));
				case Strategy.Masking:
					return GetMask(0.25, value.SafeToString(null, ""));
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static int GetHash(long hashMultiplier, long hashBucketSize, long hashSeed, object value)
		{
			long hashCode;
			byte[] buffer;

			if ((object)value == null)
				return -1;

			buffer = Encoding.GetEncoding("UCS-2").GetBytes(value.SafeToString());

			hashCode = hashSeed;
			for (int index = 0; index < buffer.Length; index++)
				hashCode = (hashMultiplier * hashCode + buffer[index]) % uint.MaxValue;

			//Console.WriteLine(hashCode);
			if (hashCode > int.MaxValue)
				hashCode = hashCode - uint.MaxValue;

			//Console.WriteLine(hashCode);
			if (hashCode < 0)
				hashCode = hashCode + int.MaxValue;

			//Console.WriteLine(hashCode);
			hashCode = hashCode % hashBucketSize;

			//Console.WriteLine(hashCode);

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

			if ((object)value == null)
				return null;

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

		public static string GetNull(bool blank, string value)
		{
			if ((object)value == null)
				return null;

			return blank ? string.Empty : null;
		}

		public static object GetObfuscatedTableViewColumnData(string connectionString, string catalogName, string schemaName, string tableName, string columnName, object value)
		{
			int strategyId;
			int dictionaryConfigId;

			IDbDataParameter dbDataParameter;

			if (DataType.IsNullOrWhiteSpace(value.SafeToString(null, null)))
				return value;

			using (IDbConnection dbConnection = new SqlConnection(connectionString))
			{
				dbConnection.Open();

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT t.[StrategyId], t.[DictionaryConfigId]
						FROM [DataObfuscation].[TableViewColumnConfig] t
						WHERE t.[CatalogName] = @CatalogName AND t.[SchemaName] = @SchemaName AND
						t.[TableName] = @TableName AND t.[ColumnName] = @ColumnName;";

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@CatalogName";
					dbDataParameter.Value = catalogName;

					dbCommmand.Parameters.Add(dbDataParameter);

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@SchemaName";
					dbDataParameter.Value = schemaName;

					dbCommmand.Parameters.Add(dbDataParameter);

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@TableName";
					dbDataParameter.Value = tableName;

					dbCommmand.Parameters.Add(dbDataParameter);

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@ColumnName";
					dbDataParameter.Value = columnName;

					dbCommmand.Parameters.Add(dbDataParameter);

					using (IDataReader dataReader = dbCommmand.ExecuteReader())
					{
						if (!dataReader.Read())
							return value;

						strategyId = dataReader["StrategyId"].ChangeType<int?>() ?? int.MaxValue;
						dictionaryConfigId = dataReader["DictionaryConfigId"].ChangeType<int>();
					}
				}
			}

			return ExecuteStrategy(connectionString, strategyId, dictionaryConfigId, value);
		}

		public static string GetShuffle(long seed, string value)
		{
			Random random;

			if ((object)value == null)
				return null;

			random = new Random((int)seed);
			value = new string(value.ToCharArray().OrderBy(s => random.Next(int.MaxValue)).ToArray());

			return value;
		}

		public static object GetVariance(double varianceFactor, object value, bool throwOnBadType)
		{
			if ((object)value == null)
				return null;

			// TODO: lift SQL specific mapping up
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
				value = SqlDateTime.Add(((SqlDateTime)value), TimeSpan.FromDays((Double)(varianceFactor * 100)));
				//else if (value is SqlDateTimeOffset)
				//value = ((DateTimeOffset)value).AddDays(((Double)varianceFactor * (Double)signFactor));
				//else if (value is SqlTimeSpan)
				//value = ((TimeSpan)value).Add(TimeSpan.FromMinutes((Double)varianceFactor * (Double)signFactor));
			else if (throwOnBadType)
				throw new InvalidOperationException(string.Format("Unsupported parameter type: '{0}'.", value.GetType().FullName));
			else
				/* do nothing */
				;

			return value;
		}

		private static Dictionary<int, char> Normalize(ref string value)
		{
			StringBuilder sb;
			Dictionary<int, char> fidelityMap;

			fidelityMap = new Dictionary<int, char>();

			if ((object)value == null)
			{
				value = null;
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

		[SqlFunction(DataAccess = DataAccessKind.Read)]
		public static string fn_ExecuteHashShuffle(int dictionaryConfigId, object value)
		{
			return ExecuteHashShuffle(CONTEXT_CONNECTION_STRING, dictionaryConfigId, value.ChangeType<object>());
		}

		[SqlFunction(DataAccess = DataAccessKind.Read)]
		public static string fn_ExecuteHashSubstitution(int dictionaryConfigId, object value)
		{
			return ExecuteHashSubstitution(CONTEXT_CONNECTION_STRING, dictionaryConfigId, value.ChangeType<object>());
		}

		[SqlFunction(DataAccess = DataAccessKind.Read)]
		public static object fn_ExecuteHashVariance(int dictionaryConfigId, object value)
		{
			return ExecuteHashVariance(CONTEXT_CONNECTION_STRING, dictionaryConfigId, value.ChangeType<object>());
		}

		[SqlFunction(DataAccess = DataAccessKind.Read)]
		public static object fn_ExecuteStrategy(int strategyId, int dictionaryConfigId, object value)
		{
			return ExecuteStrategy(CONTEXT_CONNECTION_STRING, strategyId, dictionaryConfigId, value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static int fn_GetHash(long hashMultiplier, long hashBucketSize, long hashSeed, object value)
		{
			return GetHash(hashMultiplier, hashBucketSize, hashSeed, value.ChangeType<object>());
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static string fn_GetMask(double maskFactor, string value)
		{
			return GetMask(maskFactor, value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static string fn_GetNull(bool blank, string value)
		{
			return GetNull(blank, value);
		}

		[SqlFunction(DataAccess = DataAccessKind.Read)]
		public static object fn_GetObfuscatedTableViewColumnData(string catalogName, string schemaName, string tableName, string columnName, object value)
		{
			return GetObfuscatedTableViewColumnData(CONTEXT_CONNECTION_STRING, catalogName, schemaName, tableName, columnName, value.ChangeType<object>());
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static string fn_GetShuffle(long seed, string value)
		{
			return GetShuffle(seed, value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static object fn_GetVariance(double varianceFactor, object value)
		{
			return GetVariance(varianceFactor, value, true);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		public enum Strategy : uint
		{
			Unknown = 0xFFFFFFFF,

			/// <summary>
			/// Performs no obfuscation.
			/// </summary>
			None = 0,

			/// <summary>
			/// Returns an alternate value for the real data using a hash appoach.
			/// </summary>
			Substitution = 1,

			/// <summary>
			/// Returns an alternate value for the real data using a shuffle appoach.
			/// </summary>
			Shuffling = 2,

			/// <summary>
			/// Returns a value within +/- (x PERCENT | x DAYS) of the real data.
			/// </summary>
			Variance = 3,

			/// <summary>
			/// Returns an encrypted value for all real data.
			/// </summary>
			Cipher = 4,

			/// <summary>
			/// Return a null value instead of the real data.
			/// </summary>
			Nulling = 5,

			/// <summary>
			/// Returns a mask value for most but not all of the real data.
			/// </summary>
			Masking = 6,
		}

		#endregion
	}
}