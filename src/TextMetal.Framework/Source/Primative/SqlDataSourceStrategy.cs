/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

using TextMetal.Framework.Associative;
using TextMetal.Framework.Tokenization;
using TextMetal.Middleware.Data;
using TextMetal.Middleware.Data.UoW;
using TextMetal.Middleware.Solder.Serialization;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.Source.Primative
{
	public class SqlDataSourceStrategy : SourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the SqlDataSourceStrategy class.
		/// </summary>
		public SqlDataSourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		private static void WriteSqlQuery(IEnumerable<SqlQuery> sqlQueries, IAssociativeXmlObject parentAssociativeXmlObject, Type connectionType, string connectionString, bool getSchemaOnly)
		{
			ArrayConstruct arrayConstruct;
			ObjectConstruct objectConstruct;
			PropertyConstruct propertyConstructA, propertyConstructB, propertyConstructC;
			Tokenizer tokenizer;

			IEnumerable<IRecord> records;
			string commandText;
			int count = 0;

			if ((object)sqlQueries == null)
				throw new ArgumentNullException("sqlQueries");

			if ((object)parentAssociativeXmlObject == null)
				throw new ArgumentNullException("parentAssociativeXmlObject");

			if ((object)connectionType == null)
				throw new ArgumentNullException("connectionType");

			if ((object)connectionString == null)
				throw new ArgumentNullException("connectionString");

			if (DataTypeFascade.Instance.IsWhiteSpace(connectionString))
				throw new ArgumentOutOfRangeException("connectionString");

			tokenizer = new Tokenizer(true);

			foreach (SqlQuery sqlQuery in sqlQueries.OrderBy(c => c.Key).ThenBy(c => c.Order))
			{
				arrayConstruct = new ArrayConstruct();
				arrayConstruct.Name = sqlQuery.Key;
				parentAssociativeXmlObject.Items.Add(arrayConstruct);

				commandText = tokenizer.ExpandTokens(sqlQuery.Text, new DynamicWildcardTokenReplacementStrategy(new object[] { parentAssociativeXmlObject }));

				using (IUnitOfWork unitOfWork = UnitOfWork.Create(connectionType, connectionString, false))
				{
					if (!getSchemaOnly)
						records = unitOfWork.ExecuteRecords(sqlQuery.Type, commandText, null, null);
					else
						records = unitOfWork.ExecuteSchemaRecords(sqlQuery.Type, commandText, null, null);
					
					propertyConstructB = new PropertyConstruct();
					propertyConstructB.Name = "RowCount";
					arrayConstruct.Items.Add(propertyConstructB);

					if ((object)records != null)
					{
						foreach (IRecord record in records)
						{
							objectConstruct = new ObjectConstruct();
							arrayConstruct.Items.Add(objectConstruct);

							if ((object)record != null)
							{
								foreach (KeyValuePair<string, object> keyValuePair in record)
								{
									propertyConstructC = new PropertyConstruct();
									propertyConstructC.Name = keyValuePair.Key;
									propertyConstructC.RawValue = keyValuePair.Value;

									objectConstruct.Items.Add(propertyConstructC);
								}
							}

							// correlated
							WriteSqlQuery(sqlQuery.SubQueries, objectConstruct, connectionType, connectionString, getSchemaOnly);

							count++;
						}

						propertyConstructB.RawValue = count;
					}
				}
			}
		}

		protected override object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			const string PROP_TOKEN_CONNECTION_AQTN = "ConnectionType";
			const string PROP_TOKEN_CONNECTION_STRING = "ConnectionString";
			const string PROP_TOKEN_GET_SCHEMA_ONLY = "GetSchemaOnly";
			string connectionAqtn;
			Type connectionType = null;
			string connectionString = null;
			bool getSchemaOnly = false;
			IList<string> values;

			ObjectConstruct objectConstruct00;
			SqlQuery sqlQuery;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException("sourceFilePath");

			if ((object)properties == null)
				throw new ArgumentNullException("properties");

			if (DataTypeFascade.Instance.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException("sourceFilePath");

			connectionAqtn = null;
			if (properties.TryGetValue(PROP_TOKEN_CONNECTION_AQTN, out values))
			{
				if ((object)values != null && values.Count == 1)
				{
					connectionAqtn = values[0];
					connectionType = Type.GetType(connectionAqtn, false);
				}
			}

			if ((object)connectionType == null)
				throw new InvalidOperationException(string.Format("Failed to load the connection type '{0}' via Type.GetType(..).", connectionAqtn));

			if (!typeof(IDbConnection).IsAssignableFrom(connectionType))
				throw new InvalidOperationException(string.Format("The connection type is not assignable to type '{0}'.", typeof(IDbConnection).FullName));

			if (properties.TryGetValue(PROP_TOKEN_CONNECTION_STRING, out values))
			{
				if ((object)values != null && values.Count == 1)
					connectionString = values[0];
			}

			if (DataTypeFascade.Instance.IsWhiteSpace(connectionString))
				throw new InvalidOperationException(string.Format("The connection string cannot be null or whitespace."));

			if (properties.TryGetValue(PROP_TOKEN_GET_SCHEMA_ONLY, out values))
			{
				if ((object)values != null && values.Count == 1)
					DataTypeFascade.Instance.TryParse<bool>(values[0], out getSchemaOnly);
			}

			sourceFilePath = Path.GetFullPath(sourceFilePath);

			sqlQuery = XmlSerializationStrategy.Instance.GetObjectFromFile<SqlQuery>(sourceFilePath);

			objectConstruct00 = new ObjectConstruct();

			WriteSqlQuery(new SqlQuery[] { sqlQuery }, objectConstruct00, connectionType, connectionString, getSchemaOnly);

			return objectConstruct00;
		}

		#endregion
	}
}