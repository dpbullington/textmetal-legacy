/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Microsoft.SqlServer.Server;

using TextMetal.Common.Core;

namespace TextMetal.Common.SqlServerClr
{
	public static class ScalarFunctions
	{
		#region Constructors/Destructors

		static ScalarFunctions()
		{
			string[] wellKnownInMemoryDictionary;

			wellKnownInMemoryDictionary = new string[0];
			WellKnownInMemoryDictionaries[(int)WellKnownDictionary.Null] = wellKnownInMemoryDictionary;

			wellKnownInMemoryDictionary = new string[0];
			WellKnownInMemoryDictionaries[(int)WellKnownDictionary.Sign] = wellKnownInMemoryDictionary;

			wellKnownInMemoryDictionary = new string[121];
			wellKnownInMemoryDictionary[0] = "Transparent";
			wellKnownInMemoryDictionary[1] = "Almond";
			wellKnownInMemoryDictionary[2] = "Antique Brass";
			wellKnownInMemoryDictionary[3] = "Apricot";
			wellKnownInMemoryDictionary[4] = "Aquamarine";
			wellKnownInMemoryDictionary[5] = "Asparagus";
			wellKnownInMemoryDictionary[6] = "Atomic Tangerine";
			wellKnownInMemoryDictionary[7] = "Banana Mania";
			wellKnownInMemoryDictionary[8] = "Beaver";
			wellKnownInMemoryDictionary[9] = "Bittersweet";
			wellKnownInMemoryDictionary[10] = "Black";
			wellKnownInMemoryDictionary[11] = "Blue";
			wellKnownInMemoryDictionary[12] = "Blue Bell";
			wellKnownInMemoryDictionary[13] = "Blue Green";
			wellKnownInMemoryDictionary[14] = "Blue Violet";
			wellKnownInMemoryDictionary[15] = "Blush";
			wellKnownInMemoryDictionary[16] = "Brick Red";
			wellKnownInMemoryDictionary[17] = "Brown";
			wellKnownInMemoryDictionary[18] = "Burnt Orange";
			wellKnownInMemoryDictionary[19] = "Burnt Sienna";
			wellKnownInMemoryDictionary[20] = "Cadet Blue";
			wellKnownInMemoryDictionary[21] = "Canary";
			wellKnownInMemoryDictionary[22] = "Caribbean Green";
			wellKnownInMemoryDictionary[23] = "Carnation Pink";
			wellKnownInMemoryDictionary[24] = "Cerise";
			wellKnownInMemoryDictionary[25] = "Cerulean";
			wellKnownInMemoryDictionary[26] = "Chestnut";
			wellKnownInMemoryDictionary[27] = "Copper";
			wellKnownInMemoryDictionary[28] = "Cornflower";
			wellKnownInMemoryDictionary[29] = "Cotton Candy";
			wellKnownInMemoryDictionary[30] = "Dandelion";
			wellKnownInMemoryDictionary[31] = "Denim";
			wellKnownInMemoryDictionary[32] = "Desert Sand";
			wellKnownInMemoryDictionary[33] = "Eggplant";
			wellKnownInMemoryDictionary[34] = "Electric Lime";
			wellKnownInMemoryDictionary[35] = "Fern";
			wellKnownInMemoryDictionary[36] = "Forest Green";
			wellKnownInMemoryDictionary[37] = "Fuchsia";
			wellKnownInMemoryDictionary[38] = "Fuzzy Wuzzy Brown";
			wellKnownInMemoryDictionary[39] = "Gold";
			wellKnownInMemoryDictionary[40] = "Goldenrod";
			wellKnownInMemoryDictionary[41] = "Granny Smith Apple";
			wellKnownInMemoryDictionary[42] = "Gray";
			wellKnownInMemoryDictionary[43] = "Green";
			wellKnownInMemoryDictionary[44] = "Green Yellow";
			wellKnownInMemoryDictionary[45] = "Hot Magenta";
			wellKnownInMemoryDictionary[46] = "Inch Worm";
			wellKnownInMemoryDictionary[47] = "Indigo";
			wellKnownInMemoryDictionary[48] = "Jazzberry Jam";
			wellKnownInMemoryDictionary[49] = "Jungle Green";
			wellKnownInMemoryDictionary[50] = "Laser Lemon";
			wellKnownInMemoryDictionary[51] = "Lavender";
			wellKnownInMemoryDictionary[52] = "Macaroni and Cheese";
			wellKnownInMemoryDictionary[53] = "Magenta";
			wellKnownInMemoryDictionary[54] = "Mahogany";
			wellKnownInMemoryDictionary[55] = "Manatee";
			wellKnownInMemoryDictionary[56] = "Mango Tango";
			wellKnownInMemoryDictionary[57] = "Maroon";
			wellKnownInMemoryDictionary[58] = "Mauvelous";
			wellKnownInMemoryDictionary[59] = "Melon";
			wellKnownInMemoryDictionary[60] = "Midnight Blue";
			wellKnownInMemoryDictionary[61] = "Mountain Meadow";
			wellKnownInMemoryDictionary[62] = "Navy Blue";
			wellKnownInMemoryDictionary[63] = "Neon Carrot";
			wellKnownInMemoryDictionary[64] = "Olive Green";
			wellKnownInMemoryDictionary[65] = "Orange";
			wellKnownInMemoryDictionary[66] = "Orchid";
			wellKnownInMemoryDictionary[67] = "Outer Space";
			wellKnownInMemoryDictionary[68] = "Outrageous Orange";
			wellKnownInMemoryDictionary[69] = "Pacific Blue";
			wellKnownInMemoryDictionary[70] = "Peach";
			wellKnownInMemoryDictionary[71] = "Periwinkle";
			wellKnownInMemoryDictionary[72] = "Piggy Pink";
			wellKnownInMemoryDictionary[73] = "Pine Green";
			wellKnownInMemoryDictionary[74] = "Pink Flamingo";
			wellKnownInMemoryDictionary[75] = "Pink Sherbet";
			wellKnownInMemoryDictionary[76] = "Plum";
			wellKnownInMemoryDictionary[77] = "Purple Heart";
			wellKnownInMemoryDictionary[78] = "Purple Mountains’ Majesty";
			wellKnownInMemoryDictionary[79] = "Purple Pizzazz";
			wellKnownInMemoryDictionary[80] = "Radical Red";
			wellKnownInMemoryDictionary[81] = "Raw Sienna";
			wellKnownInMemoryDictionary[82] = "Razzle Dazzle Rose";
			wellKnownInMemoryDictionary[83] = "Razzmatazz";
			wellKnownInMemoryDictionary[84] = "Red";
			wellKnownInMemoryDictionary[85] = "Red Orange";
			wellKnownInMemoryDictionary[86] = "Red Violet";
			wellKnownInMemoryDictionary[87] = "Robin Egg Blue";
			wellKnownInMemoryDictionary[88] = "Royal Purple";
			wellKnownInMemoryDictionary[89] = "Salmon";
			wellKnownInMemoryDictionary[90] = "Scarlet";
			wellKnownInMemoryDictionary[91] = "Screamin Green";
			wellKnownInMemoryDictionary[92] = "Sea Green";
			wellKnownInMemoryDictionary[93] = "Sepia";
			wellKnownInMemoryDictionary[94] = "Shadow";
			wellKnownInMemoryDictionary[95] = "Shamrock";
			wellKnownInMemoryDictionary[96] = "Shocking Pink";
			wellKnownInMemoryDictionary[97] = "Silver";
			wellKnownInMemoryDictionary[98] = "Sky Blue";
			wellKnownInMemoryDictionary[99] = "Spring Green";
			wellKnownInMemoryDictionary[100] = "Sunglow";
			wellKnownInMemoryDictionary[101] = "Sunset Orange";
			wellKnownInMemoryDictionary[102] = "Tan";
			wellKnownInMemoryDictionary[103] = "Tickle Me Pink";
			wellKnownInMemoryDictionary[104] = "Timberwolf";
			wellKnownInMemoryDictionary[105] = "Tropical Rain Forest";
			wellKnownInMemoryDictionary[106] = "Tumbleweed";
			wellKnownInMemoryDictionary[107] = "Turquoise Blue";
			wellKnownInMemoryDictionary[108] = "Unmellow Yellow";
			wellKnownInMemoryDictionary[109] = "Violet (Purple";
			wellKnownInMemoryDictionary[110] = "Violet Red";
			wellKnownInMemoryDictionary[111] = "Vivid Tangerine";
			wellKnownInMemoryDictionary[112] = "Vivid Violet";
			wellKnownInMemoryDictionary[113] = "White";
			wellKnownInMemoryDictionary[114] = "Wild Blue Yonder";
			wellKnownInMemoryDictionary[115] = "Wild Strawberry";
			wellKnownInMemoryDictionary[116] = "Wild Watermelon";
			wellKnownInMemoryDictionary[117] = "Wisteria";
			wellKnownInMemoryDictionary[118] = "Yellow";
			wellKnownInMemoryDictionary[119] = "Yellow Green";
			wellKnownInMemoryDictionary[120] = "Yellow Orange";

			WellKnownInMemoryDictionaries[(int)WellKnownDictionary.InMemoryCrayonColor] = wellKnownInMemoryDictionary;
		}

		#endregion

		#region Fields/Constants

		private const string CONTEXT_CONNECTION_STRING = "context connection=true";
		private static readonly object[][] wellKnownInMemoryDictionaries = new object[100][];

		#endregion

		#region Properties/Indexers/Events

		private static object[][] WellKnownInMemoryDictionaries
		{
			get
			{
				return wellKnownInMemoryDictionaries;
			}
		}

		#endregion

		#region Methods/Operators

		public static string ExecuteHashCipher(string connectionString, int dictionaryConfigId, string value)
		{
			long hashMultiplier;
			long hashBucketSize;
			long hashSeed;
			bool isWellKnown;

			long dictionaryKey;
			string cipherValue;

			IDbDataParameter dbDataParameter;

			if ((object)value == null)
				return value;

			using (IDbConnection dbConnection = new SqlConnection(connectionString))
			{
				dbConnection.Open();

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT
						t.[HashMultiplier],
						t.[HashBucketSize],
						t.[HashSeed],
						t.[IsWellKnown]
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
							throw new InvalidOperationException(string.Format("DictionaryConfig not found for ID = '{0}'.", dictionaryConfigId));

						hashMultiplier = dataReader["HashMultiplier"].ChangeType<long>();
						hashBucketSize = dataReader["HashBucketSize"].ChangeType<long>();
						hashSeed = dataReader["HashSeed"].ChangeType<long>();
						isWellKnown = dataReader["IsWellKnown"].ChangeType<bool>();
					}
				}

				dictionaryKey = GetHash(hashMultiplier, hashBucketSize, hashSeed, value) ?? 0;

				cipherValue = GetCipher(dictionaryKey.ToString("X"), value);
			}

			return cipherValue;
		}

		public static string ExecuteHashShuffle(string connectionString, int dictionaryConfigId, string value)
		{
			long hashMultiplier;
			long hashBucketSize;
			long hashSeed;
			bool isWellKnown;

			long randomSeed;
			string shuffleValue;

			IDbDataParameter dbDataParameter;

			if (DataType.IsNullOrWhiteSpace(value))
				return value;

			using (IDbConnection dbConnection = new SqlConnection(connectionString))
			{
				dbConnection.Open();

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT
						t.[HashMultiplier],
						t.[HashBucketSize],
						t.[HashSeed],
						t.[IsWellKnown]
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
							throw new InvalidOperationException(string.Format("DictionaryConfig not found for ID = '{0}'.", dictionaryConfigId));

						hashMultiplier = dataReader["HashMultiplier"].ChangeType<long>();
						hashBucketSize = dataReader["HashBucketSize"].ChangeType<long>();
						hashSeed = dataReader["HashSeed"].ChangeType<long>();
						isWellKnown = dataReader["IsWellKnown"].ChangeType<bool>();
					}
				}

				var fidelityMap = ImplNormalize(ref value);
				randomSeed = GetHash(hashMultiplier, hashBucketSize, hashSeed, value) ?? 0;
				shuffleValue = GetShuffle(randomSeed, value);
				ImplDenormalize(fidelityMap, ref shuffleValue);
			}

			return shuffleValue;
		}

		public static string ExecuteHashSubstitution(string connectionString, int dictionaryConfigId, string value)
		{
			long hashMultiplier;
			long hashBucketSize;
			long hashSeed;
			bool isWellKnown;

			long dictionaryKey;
			string substitutionValue;

			IDbDataParameter dbDataParameter;

			if (DataType.IsNullOrWhiteSpace(value))
				return value;

			using (IDbConnection dbConnection = new SqlConnection(connectionString))
			{
				dbConnection.Open();

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT
						t.[HashMultiplier],
						t.[HashBucketSize],
						t.[HashSeed],
						t.[IsWellKnown]
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
							throw new InvalidOperationException(string.Format("DictionaryConfig not found for ID = '{0}'.", dictionaryConfigId));

						hashMultiplier = dataReader["HashMultiplier"].ChangeType<long>();
						hashBucketSize = dataReader["HashBucketSize"].ChangeType<long>();
						hashSeed = dataReader["HashSeed"].ChangeType<long>();
						isWellKnown = dataReader["IsWellKnown"].ChangeType<bool>();
					}
				}

				dictionaryKey = GetHash(hashMultiplier, hashBucketSize, hashSeed, value) ?? 0;

				if (isWellKnown)
				{
					string[] wellKnownInMemoryDictionary;
					bool inMemoryMatch = false;

					substitutionValue = null;

					if ((int)(WellKnownDictionary)dictionaryConfigId < WellKnownInMemoryDictionaries.Length)
					{
						wellKnownInMemoryDictionary = (string[])WellKnownInMemoryDictionaries[(int)(WellKnownDictionary)dictionaryConfigId];

						if ((object)wellKnownInMemoryDictionary != null &&
							dictionaryKey < wellKnownInMemoryDictionary.Length)
						{
							substitutionValue = wellKnownInMemoryDictionary[dictionaryKey];
							inMemoryMatch = true; // all good
						}
					}

					if (!inMemoryMatch)
						throw new InvalidOperationException(string.Format("Well-known, in-memory map did not return a value for DictionaryConfigId = '{0}' and DictionaryKey = '{1}'.", dictionaryConfigId, dictionaryKey));
				}
				else
				{
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
								throw new InvalidOperationException(string.Format("MasterDictionary view did not return a value for DictionaryConfigId = '{0}' and DictionaryKey = '{1}'.", dictionaryConfigId, dictionaryKey));

							substitutionValue = dataReader["Value"].ChangeType<string>();
						}
					}
				}
			}

			return substitutionValue;
		}

		public static object ExecuteHashVariance(string connectionString, int dictionaryConfigId, object value)
		{
			long hashMultiplier;
			long hashBucketSize;
			long hashSeed;
			bool isWellKnown;

			long dictionaryKey;
			object varianceValue;

			long signFactor;
			double varianceFactor;

			IDbDataParameter dbDataParameter;

			if ((object)value == null)
				return value;

			using (IDbConnection dbConnection = new SqlConnection(connectionString))
			{
				dbConnection.Open();

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT
						t.[HashMultiplier],
						t.[HashBucketSize],
						t.[HashSeed],
						t.[IsWellKnown]
						FROM [DataObfuscation].[DictionaryConfig] t
						WHERE t.[DictionaryConfigId] = @DictionaryConfigId AND t.[IsWellKnown] = 1;";

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@DictionaryConfigId";
					dbDataParameter.Value = (int)WellKnownDictionary.Sign;

					dbCommmand.Parameters.Add(dbDataParameter);

					using (IDataReader dataReader = dbCommmand.ExecuteReader())
					{
						if (!dataReader.Read())
							throw new InvalidOperationException(string.Format("DictionaryConfig not found for ID = '{0}' and well-known.", dictionaryConfigId));

						hashMultiplier = dataReader["HashMultiplier"].ChangeType<long>();
						hashBucketSize = dataReader["HashBucketSize"].ChangeType<long>();
						hashSeed = dataReader["HashSeed"].ChangeType<long>();
						isWellKnown = dataReader["IsWellKnown"].ChangeType<bool>();
					}
				}

				dictionaryKey = GetHash(hashMultiplier, hashBucketSize, hashSeed, value.SafeToString()) ?? 0;
				signFactor = (dictionaryKey % 2) == 0 ? 1 : -1;

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT
						t.[HashMultiplier],
						t.[HashBucketSize],
						t.[HashSeed],
						t.[IsWellKnown]
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
							throw new InvalidOperationException(string.Format("DictionaryConfig not found for ID = '{0}'.", dictionaryConfigId));

						hashMultiplier = dataReader["HashMultiplier"].ChangeType<long>();
						hashBucketSize = dataReader["HashBucketSize"].ChangeType<long>();
						hashSeed = dataReader["HashSeed"].ChangeType<long>();
						isWellKnown = dataReader["IsWellKnown"].ChangeType<bool>();
					}
				}

				dictionaryKey = GetHash(hashMultiplier, hashBucketSize, hashSeed, value.SafeToString()) ?? 0;
				varianceFactor = (dictionaryKey * signFactor) / 100.0;

				varianceValue = GetVariance(varianceFactor * signFactor, value);
			}

			return varianceValue;
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

					if (!(value is string || value is SqlString))
						return GetNull(value);

					return ExecuteHashSubstitution(connectionString, dictionaryConfigId, value.SafeToString(null, null));
				case Strategy.Shuffling:

					if (!(value is string || value is SqlString))
						return GetNull(value);

					return ExecuteHashShuffle(connectionString, dictionaryConfigId, value.SafeToString(null, null));
				case Strategy.Variance:

					if ((value is string || value is SqlString))
						return GetNull(value);

					return ExecuteHashVariance(connectionString, dictionaryConfigId, value);
				case Strategy.Cipher:

					if (!(value is string || value is SqlString))
						return GetNull(value);

					return ExecuteHashCipher(connectionString, dictionaryConfigId, value.SafeToString(null, null));
				case Strategy.Nulling:
					return GetNull(value);
				case Strategy.Masking:

					if (!(value is string || value is SqlString))
						return GetNull(value);

					return GetMask(0.25, value.SafeToString(null, null));
				default:
					throw new ArgumentOutOfRangeException(string.Format("Unknown strategy '{0}'.", strategy));
			}
		}

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

		public static int? GetHash(long hashMultiplier, long hashBucketSize, long hashSeed, string value)
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

		public static string GetNull(object value)
		{
			return null;
		}

		public static string GetShuffle(long randomSeed, string value)
		{
			Random random;

			if (DataType.IsNullOrWhiteSpace(value))
				return value;

			random = new Random((int)randomSeed);
			value = new string(value.ToCharArray().OrderBy(s => random.Next(int.MaxValue)).ToArray());

			return value;
		}

		public static object GetVariance(double varianceFactor, object value)
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

		public static object ObfuscateTableViewColumnValue(string connectionString, string catalogName, string schemaName, string tableName, string columnName, object value)
		{
			int strategyId;
			int dictionaryConfigId;

			IDbDataParameter dbDataParameter;

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

		[SqlFunction(DataAccess = DataAccessKind.Read)]
		public static string fn_ExecuteHashCipher(int dictionaryConfigId, string value)
		{
			return ExecuteHashCipher(CONTEXT_CONNECTION_STRING, dictionaryConfigId, value.ChangeType<string>());
		}

		[SqlFunction(DataAccess = DataAccessKind.Read)]
		public static string fn_ExecuteHashShuffle(int dictionaryConfigId, string value)
		{
			return ExecuteHashShuffle(CONTEXT_CONNECTION_STRING, dictionaryConfigId, value.ChangeType<string>());
		}

		[SqlFunction(DataAccess = DataAccessKind.Read)]
		public static string fn_ExecuteHashSubstitution(int dictionaryConfigId, string value)
		{
			return ExecuteHashSubstitution(CONTEXT_CONNECTION_STRING, dictionaryConfigId, value.ChangeType<string>());
		}

		[SqlFunction(DataAccess = DataAccessKind.Read)]
		public static object fn_ExecuteHashVariance(int dictionaryConfigId, object value)
		{
			return ExecuteHashVariance(CONTEXT_CONNECTION_STRING, dictionaryConfigId, value.ChangeType<object>());
		}

		[SqlFunction(DataAccess = DataAccessKind.Read)]
		public static object fn_ExecuteStrategy(int strategyId, int dictionaryConfigId, object value)
		{
			return ExecuteStrategy(CONTEXT_CONNECTION_STRING, strategyId, dictionaryConfigId, value.ChangeType<object>());
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static string fn_GetCipher(string sharedSecret, string value)
		{
			return GetCipher(sharedSecret, value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static int? fn_GetHash(long hashMultiplier, long hashBucketSize, long hashSeed, string value)
		{
			return GetHash(hashMultiplier, hashBucketSize, hashSeed, value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static string fn_GetMask(double maskFactor, string value)
		{
			return GetMask(maskFactor, value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static object fn_GetNull(object value)
		{
			return GetNull(value.ChangeType<object>());
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static string fn_GetShuffle(long randomSeed, string value)
		{
			return GetShuffle(randomSeed, value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None)]
		public static object fn_GetVariance(double varianceFactor, object value)
		{
			return GetVariance(varianceFactor, value.ChangeType<object>());
		}

		[SqlFunction(DataAccess = DataAccessKind.Read)]
		public static object fn_ObfuscateTableViewColumnValue(string catalogName, string schemaName, string tableName, string columnName, object value)
		{
			return ObfuscateTableViewColumnValue(CONTEXT_CONNECTION_STRING, catalogName, schemaName, tableName, columnName, value.ChangeType<object>());
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

		public enum WellKnownDictionary : uint
		{
			Null = 0,

			Sign = 1,

			InMemoryCrayonColor = 2
		}

		#endregion
	}
}