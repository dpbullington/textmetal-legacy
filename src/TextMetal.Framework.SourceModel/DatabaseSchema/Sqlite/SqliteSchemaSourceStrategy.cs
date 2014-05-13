/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;

using TextMetal.Common.Data;

namespace TextMetal.Framework.SourceModel.DatabaseSchema.Sqlite
{
	public class SqliteSchemaSourceStrategy : SchemaSourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the SqliteSchemaSourceStrategy class.
		/// </summary>
		public SqliteSchemaSourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		protected override int CoreCalculateColumnSize(string dataSourceTag, Column column)
		{
			throw new NotImplementedException();
		}

		protected override int CoreCalculateParameterSize(string dataSourceTag, Parameter parameter)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetColumnParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetDatabaseParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetDdlTriggerParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetDmlTriggerParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table)
		{
			throw new NotImplementedException();
		}

		protected override bool CoreGetEmitImplicitReturnParameter(string dataSourceTag)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetForeignKeyColumnParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table, ForeignKey foreignKey)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetForeignKeyParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetParameterParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Procedure procedure)
		{
			throw new NotImplementedException();
		}

		protected override string CoreGetParameterPrefix(string dataSourceTag)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetProcedureParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetSchemaParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetServerParameters(IUnitOfWork unitOfWork, string dataSourceTag)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetTableParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetUniqueKeyColumnParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table, UniqueKey uniqueKey)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<IDataParameter> CoreGetUniqueKeyParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table)
		{
			throw new NotImplementedException();
		}

		protected override Type CoreInferClrTypeForSqlType(string dataSourceTag, string sqlType, int sqlPrecision)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}