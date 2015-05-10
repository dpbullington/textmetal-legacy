/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;

namespace TextMetal.Middleware.Common.Fascades.AdoNet
{
	public interface IAdoNetFascade
	{
		#region Methods/Operators

		/// <summary>
		/// Create a new data parameter from the data source.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="parameterDirection"> Specifies the parameter direction. </param>
		/// <param name="parameterDbType"> Specifies the parameter provider-(in)dependent type. </param>
		/// <param name="parameterSize"> Specifies the parameter size. </param>
		/// <param name="parameterPrecision"> Specifies the parameter precision. </param>
		/// <param name="parameterScale"> Specifies the parameter scale. </param>
		/// <param name="parameterNullable"> Specifies the parameter nullable-ness. </param>
		/// <param name="parameterName"> Specifies the parameter name. </param>
		/// <param name="parameterValue"> Specifies the parameter value. </param>
		/// <returns> The data parameter with the specified properties set. </returns>
		IDbDataParameter CreateParameter(IDbConnection dbConnection, IDbTransaction dbTransaction, ParameterDirection parameterDirection, DbType parameterDbType, int parameterSize, byte parameterPrecision, byte parameterScale, bool parameterNullable, string parameterName, object parameterValue);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to a list of dictionaries.
		/// This overload is for backwards compatability; this overload perfoms EAGER LOADING.
		/// DO NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffected"> The output count of records affected. </param>
		/// <returns> A list of dictionary instances, containing key/value pairs of data. </returns>
		IList<IDictionary<string, object>> ExecuteDictionary(IDbConnection dbConnection, IDbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, out int recordsAffected);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of dictionaries.
		/// This overload is for backwards compatability; this overload perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// DO NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of dictionary instances, containing key/value pairs of data. </returns>
		IEnumerable<IDictionary<string, object>> ExecuteDictionary(IDbConnection dbConnection, IDbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, Action<int> recordsAffectedCallback);

		/// <summary>
		/// Executes a command, returning a data reader, against a data source.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="commandBehavior"> The reader behavior. </param>
		/// <param name="commandTimeout"> The command timeout (use null for default). </param>
		/// <param name="commandPrepare"> Whether to prepare the command at the data source. </param>
		/// <returns> The data reader result. </returns>
		IDataReader ExecuteReader(IDbConnection dbConnection, IDbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, CommandBehavior commandBehavior, int? commandTimeout, bool commandPrepare);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to a list of dictionaries.
		/// This overload is for backwards compatability; this overload perfoms EAGER LOADING.
		/// DO NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffected"> The output count of records affected. </param>
		/// <returns> A list of dictionary instances, containing key/value pairs of schema metadata. </returns>
		IList<IDictionary<string, object>> ExecuteSchema(IDbConnection dbConnection, IDbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, out int recordsAffected);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of dictionaries.
		/// This overload is for backwards compatability; this overload perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// DO NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of dictionary instances, containing key/value pairs of schema metadata. </returns>
		IEnumerable<IDictionary<string, object>> ExecuteSchema(IDbConnection dbConnection, IDbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, Action<int> recordsAffectedCallback);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of dictionaries.
		/// This state machine method (yield return) perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// THE DATA READER WILL BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dataReader"> The target data reader. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of dictionary instances, containing key/value pairs of data. </returns>
		IEnumerable<IDictionary<string, object>> GetEnumerableDictionary(IDataReader dataReader, Action<int> recordsAffectedCallback);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of dictionaries.
		/// This state machine method (yield return) perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// THE DATA READER WILL BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dataReader"> The target data reader. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of dictionary instances, containing key/value pairs of schema metadata. </returns>
		IEnumerable<IDictionary<string, object>> GetSchemaEnumerableDictionary(IDataReader dataReader, Action<int> recordsAffectedCallback);

		/// <summary>
		/// Returns a DbType mapping for a Type.
		/// An InvalidOperationException is thrown for unmappable types.
		/// </summary>
		/// <param name="clrType"> The CLR type to map to a DbType. </param>
		/// <returns> The mapped DbType. </returns>
		DbType InferDbTypeForClrType(Type clrType);

		#endregion
	}
}