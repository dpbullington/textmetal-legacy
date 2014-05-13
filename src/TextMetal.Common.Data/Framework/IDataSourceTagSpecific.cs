/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Data;

namespace TextMetal.Common.Data.Framework
{
	public interface IDataSourceTagSpecific
	{
		#region Properties/Indexers/Events

		string DataSourceTag
		{
			get;
		}

		#endregion

		#region Methods/Operators

		void CommandMagic(IUnitOfWork unitOfWork, bool executeAsCud, out int thisOrThatRecordsAffected);

		string GetAliasedColumnName(string tableAlias, string columnName);

		string GetColumnName(string columnName);

		string GetIdentityCommand();

		string GetParameterName(string parameterName);

		string GetTableAlias(string tableAlias);

		string GetTableName(string schemaName, string tableName);

		void ParameterMagic(IUnitOfWork unitOfWork, IDataParameter commandParameter, string generatedFromColumnNativeType);

		#endregion
	}
}