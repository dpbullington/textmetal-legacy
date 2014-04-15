/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Microsoft.SqlServer.Server;

namespace TextMetal.Common.SqlServerClr
{
	public static class TableValueFunctions
	{
		#region Fields/Constants

		private const string CONTEXT_CONNECTION_STRING = "context connection=true";

		#endregion

		#region Methods/Operators

		public static void FillRow(object obj, out SqlInt32 rowNumber)
		{
			rowNumber = (int)obj;
		}

		[SqlFunction(
			DataAccess = DataAccessKind.Read,
			SystemDataAccess = SystemDataAccessKind.Read,
			TableDefinition = "RowNumber int",
			FillRowMethodName = "FillRow"
			)]
		public static IEnumerable fn_TableValueTest(SqlInt32 databaseID)
		{
			IDbDataParameter dbDataParameter;
			using (IDbConnection dbConnection = new SqlConnection())
			{
				dbConnection.ConnectionString = CONTEXT_CONNECTION_STRING;
				dbConnection.Open();

				using (IDbCommand dbCommmand = dbConnection.CreateCommand())
				{
					dbCommmand.CommandText = @"SELECT TOP (100) ROWNUMBER FROM SSP1 WHERE DatabaseID = @DatabaseID;";

					dbDataParameter = dbCommmand.CreateParameter();
					dbDataParameter.Direction = ParameterDirection.Input;
					dbDataParameter.ParameterName = "@DatabaseID";
					dbDataParameter.Value = databaseID;

					dbCommmand.Parameters.Add(dbDataParameter);

					using (IDataReader dataReader = dbCommmand.ExecuteReader())
					{
						while (dataReader.Read())
							yield return dataReader.GetInt32(0);
					}
				}
			}
		}

		#endregion
	}
}