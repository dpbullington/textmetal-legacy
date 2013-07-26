using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using TextMetal.Common.Data.TypeMap.LowLevel;

namespace TextMetal.Common.Data.TypeMap.HighLevel
{
	public class DefaultSqlCommandProvider : ICommandProvider
	{
		#region Constructors/Destructors

		public DefaultSqlCommandProvider(IExpressionVisitor expressionVisitor)
		{
			if ((object)expressionVisitor == null)
				throw new ArgumentNullException("expressionVisitor");

			this.expressionVisitor = expressionVisitor;
		}

		#endregion

		#region Fields/Constants

		internal const string COLUMN_ALIASED_FORMAT = "{0}.{1}";
		internal const string COLUMN_NAME_FORMAT = "[{0}]";
		internal const string PARAMETER_NAME_FORMAT = "@p{0:000}";
		private const string SCHEMA_TABLE_NAME_FORMAT = "[{0}].[{1}]";
		private const string SCOPE_ID_BATCH_FUNCTION = "SCOPE_IDENTITY()";
		private const string SCOPE_ID_COMMAND_PARAMETER = "@@IDENTITY";
		internal const string TABLE_ALIAS_FORMAT = "t{0:000}";
		private const string TABLE_NAME_FORMAT = "[{0}]";
		private readonly IExpressionVisitor expressionVisitor;

		#endregion

		#region Properties/Indexers/Events

		private static bool UseSingleConnectionBatchIdentityFetchSemantics
		{
			get
			{
				string svalue;
				bool bvalue;

				svalue = ConfigurationManager.AppSettings[string.Format("{0}::UseSingleConnectionBatchIdentityFetchSemantics", typeof(DefaultSqlCommandProvider).FullName)];

				if (bool.TryParse(svalue, out bvalue))
					return bvalue;
				else
					return false; // default to false
			}
		}

		#endregion

		#region Methods/Operators

		private static void AssertMapping(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes)
		{
			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			if ((object)columnMappingAttributes == null)
				throw new ArgumentNullException("columnMappingAttributes");

			if (columnMappingAttributes.Count(x => x.IsPrimaryKey) < 1)
				throw new InvalidOperationException(string.Format("The table mapped type '{0}' does not specify a property marked with the '{1}' attribute with the 'IsPrimaryKey' value set to 'true'.", tableMappingAttribute.TargetType.FullName, typeof(ColumnMappingAttribute).FullName));
		}

		public virtual Command GetDelete(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes)
		{
			Command command;

			int parameterIndex = 0;

			string tableName, fieldName, columnName, parameterName;

			List<string> predicateNames;
			string predicateList;

			AssertMapping(tableMappingAttribute, columnMappingAttributes);

			columnMappingAttributes = columnMappingAttributes.OrderBy(x => x.Ordinal).ThenBy(x => x.Name).ToArray();

			command = new Command();
			command.Type = CommandType.Text;
			command.Timeout = null;
			command.Prepare = false;

			tableName = !DataType.IsNullOrWhiteSpace(tableMappingAttribute.Schema) ? string.Format(SCHEMA_TABLE_NAME_FORMAT, tableMappingAttribute.Schema, tableMappingAttribute.Name) : string.Format(TABLE_NAME_FORMAT, tableMappingAttribute.Name);

			predicateNames = new List<string>();

			foreach (ColumnMappingAttribute primaryKeyColumnMappingAttribute in columnMappingAttributes.Where(x => x.IsPrimaryKey))
			{
				columnName = string.Format(COLUMN_NAME_FORMAT, primaryKeyColumnMappingAttribute.Name);

				parameterName = string.Format(PARAMETER_NAME_FORMAT, parameterIndex++);

				predicateNames.Add(string.Format("{0} = {1}", columnName, parameterName));

				command.Parameters.Add(new Parameter()
				                       {
				                       	Direction = ParameterDirection.Input,
				                       	Type = primaryKeyColumnMappingAttribute.DbType,
				                       	Size = primaryKeyColumnMappingAttribute.Size,
				                       	Precision = primaryKeyColumnMappingAttribute.Precision,
				                       	Scale = primaryKeyColumnMappingAttribute.Scale,
				                       	Nullable = primaryKeyColumnMappingAttribute.Nullable,
				                       	Name = parameterName,
				                       	Property = (object)primaryKeyColumnMappingAttribute.TargetProperty != null ? primaryKeyColumnMappingAttribute.TargetProperty.Name : ""
				                       });
			}

			foreach (ColumnMappingAttribute concurrencyCheckColumnMappingAttribute in columnMappingAttributes.Where(x => x.UseInConcurrencyCheck))
			{
				columnName = string.Format(COLUMN_NAME_FORMAT, concurrencyCheckColumnMappingAttribute.Name);

				parameterName = string.Format(PARAMETER_NAME_FORMAT, parameterIndex++);

				predicateNames.Add(string.Format("{0} = {1}", columnName, parameterName));

				command.Parameters.Add(new Parameter()
				                       {
				                       	Direction = ParameterDirection.Input,
				                       	Type = concurrencyCheckColumnMappingAttribute.DbType,
				                       	Size = concurrencyCheckColumnMappingAttribute.Size,
				                       	Precision = concurrencyCheckColumnMappingAttribute.Precision,
				                       	Scale = concurrencyCheckColumnMappingAttribute.Scale,
				                       	Nullable = concurrencyCheckColumnMappingAttribute.Nullable,
				                       	Name = parameterName,
				                       	Property = concurrencyCheckColumnMappingAttribute.PreviousVersionPath + ((object)concurrencyCheckColumnMappingAttribute.TargetProperty != null ? concurrencyCheckColumnMappingAttribute.TargetProperty.Name : "")
				                       });
			}

			predicateList = string.Join(" AND ", predicateNames.ToArray());

			command.Text = string.Format("DELETE FROM {0} WHERE (1 = 1) AND {1}", tableName, predicateList);

			return command;
		}

		public virtual Command GetInsert(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes)
		{
			Command command;

			int parameterIndex = 0;

			string tableName, fieldName, columnName, parameterName, scopeIdentifierColumnName, scopeIdentifierParameterName;

			List<string> columnNames, valueNames;
			string columnList, valueList;

			ColumnMappingAttribute scopeIdentityColumnMappingAttribute = null;

			AssertMapping(tableMappingAttribute, columnMappingAttributes);

			columnMappingAttributes = columnMappingAttributes.OrderBy(x => x.Ordinal).ThenBy(x => x.Name).ToArray();

			command = new Command();
			command.Type = CommandType.Text;
			command.Timeout = null;
			command.Prepare = false;

			tableName = !DataType.IsNullOrWhiteSpace(tableMappingAttribute.Schema) ? string.Format(SCHEMA_TABLE_NAME_FORMAT, tableMappingAttribute.Schema, tableMappingAttribute.Name) : string.Format(TABLE_NAME_FORMAT, tableMappingAttribute.Name);

			columnNames = new List<string>();
			valueNames = new List<string>();

			foreach (ColumnMappingAttribute columnMappingAttribute in columnMappingAttributes)
			{
				if (columnMappingAttribute.IsReadOnly)
					continue;

				columnName = string.Format(COLUMN_NAME_FORMAT, columnMappingAttribute.Name);

				parameterName = string.Format(PARAMETER_NAME_FORMAT, parameterIndex++);

				columnNames.Add(columnName);

				valueNames.Add(parameterName);

				command.Parameters.Add(new Parameter()
				                       {
				                       	Direction = ParameterDirection.Input,
				                       	Type = columnMappingAttribute.DbType,
				                       	Size = columnMappingAttribute.Size,
				                       	Precision = columnMappingAttribute.Precision,
				                       	Scale = columnMappingAttribute.Scale,
				                       	Nullable = columnMappingAttribute.Nullable,
				                       	Name = parameterName,
				                       	Property = (object)columnMappingAttribute.TargetProperty != null ? columnMappingAttribute.TargetProperty.Name : ""
				                       });
			}

			columnList = string.Join(", ", columnNames.ToArray());
			valueList = string.Join(", ", valueNames.ToArray());

			// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

			// only one primary key as identity column can be created per table
			scopeIdentityColumnMappingAttribute = columnMappingAttributes.SingleOrDefault(x => x.IsPrimaryKey && x.IsReadOnly);

			if ((object)scopeIdentityColumnMappingAttribute != null &&
			    !UseSingleConnectionBatchIdentityFetchSemantics)
			{
				scopeIdentifierParameterName = string.Format(PARAMETER_NAME_FORMAT, parameterIndex++);

				command.Parameters.Add(new Parameter()
				                       {
				                       	Direction = ParameterDirection.Output,
				                       	Type = scopeIdentityColumnMappingAttribute.DbType,
				                       	Size = scopeIdentityColumnMappingAttribute.Size,
				                       	Precision = scopeIdentityColumnMappingAttribute.Precision,
				                       	Scale = scopeIdentityColumnMappingAttribute.Scale,
				                       	Nullable = scopeIdentityColumnMappingAttribute.Nullable,
				                       	Name = scopeIdentifierParameterName,
				                       	Property = (object)scopeIdentityColumnMappingAttribute.TargetProperty != null ? scopeIdentityColumnMappingAttribute.TargetProperty.Name : ""
				                       });

				command.Text = string.Format("INSERT INTO {0} ({1}) VALUES ({2})\r\nSET {3} = {4}", tableName, columnList, valueList, scopeIdentifierParameterName, SCOPE_ID_BATCH_FUNCTION);
			}
			else if ((object)scopeIdentityColumnMappingAttribute != null &&
			         UseSingleConnectionBatchIdentityFetchSemantics)
			{
				scopeIdentifierColumnName = string.Format(COLUMN_NAME_FORMAT, scopeIdentityColumnMappingAttribute.Name);

				command.Text = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, columnList, valueList);

				command.Identity = string.Format("SELECT {0} AS {1}", SCOPE_ID_COMMAND_PARAMETER, scopeIdentifierColumnName);
			}
			else
				command.Text = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, columnList, valueList);

			return command;
		}

		public virtual Command GetSelectAll(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes)
		{
			Command command;

			int parameterIndex = 0;

			string tableName, fieldName, columnName, parameterName;

			string tableAlias, columnAliasedName;

			List<string> projectionNames, sortNames;
			string projectionList, sortList;

			AssertMapping(tableMappingAttribute, columnMappingAttributes);

			columnMappingAttributes = columnMappingAttributes.OrderBy(x => x.Ordinal).ThenBy(x => x.Name).ToArray();

			command = new Command();
			command.Type = CommandType.Text;
			command.Timeout = null;
			command.Prepare = false;

			tableName = !DataType.IsNullOrWhiteSpace(tableMappingAttribute.Schema) ? string.Format(SCHEMA_TABLE_NAME_FORMAT, tableMappingAttribute.Schema, tableMappingAttribute.Name) : string.Format(TABLE_NAME_FORMAT, tableMappingAttribute.Name);
			tableAlias = string.Format(TABLE_ALIAS_FORMAT, 0);

			projectionNames = new List<string>();
			sortNames = new List<string>();

			foreach (ColumnMappingAttribute primaryKeyColumnMappingAttribute in columnMappingAttributes.Where(x => x.IsPrimaryKey))
			{
				fieldName = primaryKeyColumnMappingAttribute.Name;

				columnName = string.Format(COLUMN_NAME_FORMAT, primaryKeyColumnMappingAttribute.Name);

				columnAliasedName = string.Format(COLUMN_ALIASED_FORMAT, tableAlias, columnName);

				projectionNames.Add(columnAliasedName);

				sortNames.Add(columnAliasedName);

				command.Fields.Add(new Field()
				                   {
				                   	Name = fieldName,
				                   	Property = (object)primaryKeyColumnMappingAttribute.TargetProperty != null ? primaryKeyColumnMappingAttribute.TargetProperty.Name : ""
				                   });
			}

			foreach (ColumnMappingAttribute nonKeyColumnMappingAttribute in columnMappingAttributes.Where(x => !x.IsPrimaryKey))
			{
				fieldName = nonKeyColumnMappingAttribute.Name;

				columnName = string.Format(COLUMN_NAME_FORMAT, nonKeyColumnMappingAttribute.Name);

				columnAliasedName = string.Format(COLUMN_ALIASED_FORMAT, tableAlias, columnName);

				projectionNames.Add(columnAliasedName);

				command.Fields.Add(new Field()
				                   {
				                   	Name = fieldName,
				                   	Property = (object)nonKeyColumnMappingAttribute.TargetProperty != null ? nonKeyColumnMappingAttribute.TargetProperty.Name : ""
				                   });
			}

			projectionList = string.Join(", ", projectionNames.ToArray());
			sortList = string.Join(", ", sortNames.ToArray());

			command.Text = string.Format("SELECT {0} FROM {1} {2} ORDER BY {3}", projectionList, tableName, tableAlias, sortList);

			return command;
		}

		public virtual For[] GetSelectFors(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes, Query query)
		{
			DefaultSqlExpressionVisitor visitor;
			Command command;

			int parameterIndex = 0;
			const int tableIndex = 0;

			string tableName, fieldName, columnName, parameterName;

			string tableAlias, columnAliasedName;

			List<string> projectionNames, sortNames;
			string projectionList, sortList;

			string sqlPredicate, sqlSort, sqlRestriction;

			if ((object)query == null)
				return new For[] { };

			AssertMapping(tableMappingAttribute, columnMappingAttributes);

			columnMappingAttributes = columnMappingAttributes.OrderBy(x => x.Ordinal).ThenBy(x => x.Name).ToArray();

			command = new Command();
			command.Type = CommandType.Text;
			command.Timeout = null;
			command.Prepare = false;

			tableName = !DataType.IsNullOrWhiteSpace(tableMappingAttribute.Schema) ? string.Format(SCHEMA_TABLE_NAME_FORMAT, tableMappingAttribute.Schema, tableMappingAttribute.Name) : string.Format(TABLE_NAME_FORMAT, tableMappingAttribute.Name);
			tableAlias = string.Format(TABLE_ALIAS_FORMAT, 0);

			projectionNames = new List<string>();
			sortNames = new List<string>();

			foreach (ColumnMappingAttribute primaryKeyColumnMappingAttribute in columnMappingAttributes.Where(x => x.IsPrimaryKey))
			{
				fieldName = primaryKeyColumnMappingAttribute.Name;

				columnName = string.Format(COLUMN_NAME_FORMAT, primaryKeyColumnMappingAttribute.Name);

				columnAliasedName = string.Format(COLUMN_ALIASED_FORMAT, tableAlias, columnName);

				projectionNames.Add(columnAliasedName);

				sortNames.Add(columnAliasedName);

				command.Fields.Add(new Field()
				                   {
				                   	Name = fieldName,
				                   	Property = (object)primaryKeyColumnMappingAttribute.TargetProperty != null ? primaryKeyColumnMappingAttribute.TargetProperty.Name : ""
				                   });
			}

			foreach (ColumnMappingAttribute nonKeyColumnMappingAttribute in columnMappingAttributes.Where(x => !x.IsPrimaryKey))
			{
				fieldName = nonKeyColumnMappingAttribute.Name;

				columnName = string.Format(COLUMN_NAME_FORMAT, nonKeyColumnMappingAttribute.Name);

				columnAliasedName = string.Format(COLUMN_ALIASED_FORMAT, tableAlias, columnName);

				projectionNames.Add(columnAliasedName);

				command.Fields.Add(new Field()
				                   {
				                   	Name = fieldName,
				                   	Property = (object)nonKeyColumnMappingAttribute.TargetProperty != null ? nonKeyColumnMappingAttribute.TargetProperty.Name : ""
				                   });
			}

			projectionList = string.Join(", ", projectionNames.ToArray());
			sortList = string.Join(", ", sortNames.ToArray());

			// after this call, Sql* props are valid is valid;
			// after this call, the Query object will be altered...
			this.expressionVisitor.GoVisit(query, tableMappingAttribute, columnMappingAttributes, tableIndex, parameterIndex);

			sqlRestriction = this.expressionVisitor.SqlRestriction;
			sqlSort = this.expressionVisitor.SqlSort;
			sqlPredicate = this.expressionVisitor.SqlPredicate;

			if ((object)this.expressionVisitor.SqlPredicateParameters != null)
				command.Parameters.AddRange(this.expressionVisitor.SqlPredicateParameters);

			command.Text = string.Format("SELECT {0} {1} FROM {2} {3} WHERE (1 = 1) AND {4} ORDER BY {5}", sqlRestriction, projectionList, tableName, tableAlias, sqlPredicate, (DataType.IsNullOrWhiteSpace(sqlSort) ? sortList : sqlSort));

			return new For[] { new For() { Id = "", Command = command } };
		}

		public virtual Command GetSelectNot(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes)
		{
			Command command;

			int parameterIndex = 0;

			string tableName, fieldName, columnName, parameterName;

			string tableAlias, columnAliasedName;

			List<string> projectionNames, predicateNames;
			string projectionList, predicateList;

			AssertMapping(tableMappingAttribute, columnMappingAttributes);

			columnMappingAttributes = columnMappingAttributes.OrderBy(x => x.Ordinal).ThenBy(x => x.Name).ToArray();

			command = new Command();
			command.Type = CommandType.Text;
			command.Timeout = null;
			command.Prepare = false;

			tableName = !DataType.IsNullOrWhiteSpace(tableMappingAttribute.Schema) ? string.Format(SCHEMA_TABLE_NAME_FORMAT, tableMappingAttribute.Schema, tableMappingAttribute.Name) : string.Format(TABLE_NAME_FORMAT, tableMappingAttribute.Name);
			tableAlias = string.Format(TABLE_ALIAS_FORMAT, 0);

			projectionNames = new List<string>();

			foreach (ColumnMappingAttribute columnMappingAttribute in columnMappingAttributes)
			{
				fieldName = columnMappingAttribute.Name;

				columnName = string.Format(COLUMN_NAME_FORMAT, columnMappingAttribute.Name);

				columnAliasedName = string.Format(COLUMN_ALIASED_FORMAT, tableAlias, columnName);

				projectionNames.Add(columnAliasedName);

				command.Fields.Add(new Field()
				{
					Name = fieldName,
					Property = (object)columnMappingAttribute.TargetProperty != null ? columnMappingAttribute.TargetProperty.Name : ""
				});
			}
			
			projectionList = string.Join(", ", projectionNames.ToArray());
			
			command.Text = string.Format("SELECT {0} FROM {1} {2} WHERE (-1 = 1)", projectionList, tableName, tableAlias);

			return command;
		}

		public virtual Command GetSelectOne(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes)
		{
			Command command;

			int parameterIndex = 0;

			string tableName, fieldName, columnName, parameterName;

			string tableAlias, columnAliasedName;

			List<string> projectionNames, predicateNames;
			string projectionList, predicateList;

			AssertMapping(tableMappingAttribute, columnMappingAttributes);

			columnMappingAttributes = columnMappingAttributes.OrderBy(x => x.Ordinal).ThenBy(x => x.Name).ToArray();

			command = new Command();
			command.Type = CommandType.Text;
			command.Timeout = null;
			command.Prepare = false;

			tableName = !DataType.IsNullOrWhiteSpace(tableMappingAttribute.Schema) ? string.Format(SCHEMA_TABLE_NAME_FORMAT, tableMappingAttribute.Schema, tableMappingAttribute.Name) : string.Format(TABLE_NAME_FORMAT, tableMappingAttribute.Name);
			tableAlias = string.Format(TABLE_ALIAS_FORMAT, 0);

			projectionNames = new List<string>();
			predicateNames = new List<string>();

			foreach (ColumnMappingAttribute primaryKeyColumnMappingAttribute in columnMappingAttributes.Where(x => x.IsPrimaryKey))
			{
				fieldName = primaryKeyColumnMappingAttribute.Name;

				columnName = string.Format(COLUMN_NAME_FORMAT, primaryKeyColumnMappingAttribute.Name);

				columnAliasedName = string.Format(COLUMN_ALIASED_FORMAT, tableAlias, columnName);

				parameterName = string.Format(PARAMETER_NAME_FORMAT, parameterIndex++);

				projectionNames.Add(columnAliasedName);

				predicateNames.Add(string.Format("{0} = {1}", columnAliasedName, parameterName));

				command.Parameters.Add(new Parameter()
				                       {
				                       	Direction = ParameterDirection.Input,
				                       	Type = primaryKeyColumnMappingAttribute.DbType,
				                       	Size = primaryKeyColumnMappingAttribute.Size,
				                       	Precision = primaryKeyColumnMappingAttribute.Precision,
				                       	Scale = primaryKeyColumnMappingAttribute.Scale,
				                       	Nullable = primaryKeyColumnMappingAttribute.Nullable,
				                       	Name = parameterName,
				                       	Property = (object)primaryKeyColumnMappingAttribute.TargetProperty != null ? primaryKeyColumnMappingAttribute.TargetProperty.Name : ""
				                       });

				command.Fields.Add(new Field()
				                   {
				                   	Name = fieldName,
				                   	Property = (object)primaryKeyColumnMappingAttribute.TargetProperty != null ? primaryKeyColumnMappingAttribute.TargetProperty.Name : ""
				                   });
			}

			foreach (ColumnMappingAttribute nonKeyColumnMappingAttribute in columnMappingAttributes.Where(x => !x.IsPrimaryKey))
			{
				fieldName = nonKeyColumnMappingAttribute.Name;

				columnName = string.Format(COLUMN_NAME_FORMAT, nonKeyColumnMappingAttribute.Name);

				columnAliasedName = string.Format(COLUMN_ALIASED_FORMAT, tableAlias, columnName);

				projectionNames.Add(columnAliasedName);

				command.Fields.Add(new Field()
				                   {
				                   	Name = fieldName,
				                   	Property = (object)nonKeyColumnMappingAttribute.TargetProperty != null ? nonKeyColumnMappingAttribute.TargetProperty.Name : ""
				                   });
			}

			projectionList = string.Join(", ", projectionNames.ToArray());
			predicateList = string.Join(" AND ", predicateNames.ToArray());

			command.Text = string.Format("SELECT {0} FROM {1} {2} WHERE (1 = 1) AND {3}", projectionList, tableName, tableAlias, predicateList);

			return command;
		}

		public virtual Command GetUpdate(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes)
		{
			Command command;

			int parameterIndex = 0;

			string tableName, fieldName, columnName, parameterName;

			List<string> setNames, predicateNames;
			string setList, predicateList;

			AssertMapping(tableMappingAttribute, columnMappingAttributes);

			columnMappingAttributes = columnMappingAttributes.OrderBy(x => x.Ordinal).ThenBy(x => x.Name).ToArray();

			command = new Command();
			command.Type = CommandType.Text;
			command.Timeout = null;
			command.Prepare = false;

			tableName = !DataType.IsNullOrWhiteSpace(tableMappingAttribute.Schema) ? string.Format(SCHEMA_TABLE_NAME_FORMAT, tableMappingAttribute.Schema, tableMappingAttribute.Name) : string.Format(TABLE_NAME_FORMAT, tableMappingAttribute.Name);

			setNames = new List<string>();
			predicateNames = new List<string>();

			foreach (ColumnMappingAttribute columnMappingAttribute in columnMappingAttributes)
			{
				if (columnMappingAttribute.IsReadOnly)
					continue;

				columnName = string.Format(COLUMN_NAME_FORMAT, columnMappingAttribute.Name);

				parameterName = string.Format(PARAMETER_NAME_FORMAT, parameterIndex++);

				setNames.Add(string.Format("{0} = {1}", columnName, parameterName));

				command.Parameters.Add(new Parameter()
				                       {
				                       	Direction = ParameterDirection.Input,
				                       	Type = columnMappingAttribute.DbType,
				                       	Size = columnMappingAttribute.Size,
				                       	Precision = columnMappingAttribute.Precision,
				                       	Scale = columnMappingAttribute.Scale,
				                       	Nullable = columnMappingAttribute.Nullable,
				                       	Name = parameterName,
				                       	Property = (object)columnMappingAttribute.TargetProperty != null ? columnMappingAttribute.TargetProperty.Name : ""
				                       });
			}

			foreach (ColumnMappingAttribute primaryKeyColumnMappingAttribute in columnMappingAttributes.Where(x => x.IsPrimaryKey))
			{
				columnName = string.Format(COLUMN_NAME_FORMAT, primaryKeyColumnMappingAttribute.Name);

				parameterName = string.Format(PARAMETER_NAME_FORMAT, parameterIndex++);

				predicateNames.Add(string.Format("{0} = {1}", columnName, parameterName));

				command.Parameters.Add(new Parameter()
				                       {
				                       	Direction = ParameterDirection.Input,
				                       	Type = primaryKeyColumnMappingAttribute.DbType,
				                       	Size = primaryKeyColumnMappingAttribute.Size,
				                       	Precision = primaryKeyColumnMappingAttribute.Precision,
				                       	Scale = primaryKeyColumnMappingAttribute.Scale,
				                       	Nullable = primaryKeyColumnMappingAttribute.Nullable,
				                       	Name = parameterName,
				                       	Property = (object)primaryKeyColumnMappingAttribute.TargetProperty != null ? primaryKeyColumnMappingAttribute.TargetProperty.Name : ""
				                       });
			}

			foreach (ColumnMappingAttribute concurrencyCheckColumnMappingAttribute in columnMappingAttributes.Where(x => x.UseInConcurrencyCheck))
			{
				columnName = string.Format(COLUMN_NAME_FORMAT, concurrencyCheckColumnMappingAttribute.Name);

				parameterName = string.Format(PARAMETER_NAME_FORMAT, parameterIndex++);

				predicateNames.Add(string.Format("{0} = {1}", columnName, parameterName));

				command.Parameters.Add(new Parameter()
				                       {
				                       	Direction = ParameterDirection.Input,
				                       	Type = concurrencyCheckColumnMappingAttribute.DbType,
				                       	Size = concurrencyCheckColumnMappingAttribute.Size,
				                       	Precision = concurrencyCheckColumnMappingAttribute.Precision,
				                       	Scale = concurrencyCheckColumnMappingAttribute.Scale,
				                       	Nullable = concurrencyCheckColumnMappingAttribute.Nullable,
				                       	Name = parameterName,
				                       	Property = concurrencyCheckColumnMappingAttribute.PreviousVersionPath + ((object)concurrencyCheckColumnMappingAttribute.TargetProperty != null ? concurrencyCheckColumnMappingAttribute.TargetProperty.Name : "")
				                       });
			}

			setList = string.Join(", ", setNames.ToArray());
			predicateList = string.Join(" AND ", predicateNames.ToArray());

			command.Text = string.Format("UPDATE {0} SET {1} WHERE (1 = 1) AND {2}", tableName, setList, predicateList);

			return command;
		}

		#endregion
	}
}