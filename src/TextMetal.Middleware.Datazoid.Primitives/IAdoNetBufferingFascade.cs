/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using __Record = System.Collections.Generic.IDictionary<string, object>;

namespace TextMetal.Middleware.Datazoid.Primitives
{
	public interface IAdoNetBufferingFascade
	{
		#region Methods/Operators

		DbParameter CreateParameter(Type connectionType, ParameterDirection parameterDirection, DbType parameterDbType, int parameterSize, byte parameterPrecision, byte parameterScale, bool parameterNullable, string parameterName, object parameterValue);

		IEnumerable<__Record> ExecuteRecords(bool schemaOnly, Type connectionType, string connectionString, bool transactional, IsolationLevel isolationLevel, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> resultsetCallback = null);

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