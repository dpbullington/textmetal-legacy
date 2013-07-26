/*
	Copyright ©2002-2010 Daniel Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using TextMetal.Common.Data.TypeMap.LowLevel;

namespace TextMetal.Common.Data.TypeMap.HighLevel
{
	public class DefaultSqlExpressionVisitor : ExpressionVisitor, IExpressionVisitor
	{
		#region Constructors/Destructors

		public DefaultSqlExpressionVisitor()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<Parameter> parameters = new List<Parameter>();
		private ColumnMappingAttribute[] columnMappingAttributes;
		private int parameterIndex;
		private Query query;
		private string sqlPredicate;
		private Parameter[] sqlPredicateParameters;
		private string sqlRestriction;
		private string sqlSort;
		private StringBuilder strings;
		private int tableIndex;
		private TableMappingAttribute tableMappingAttribute;

		#endregion

		#region Properties/Indexers/Events

		public virtual string SqlPredicate
		{
			get
			{
				return this.sqlPredicate;
			}
		}

		public virtual Parameter[] SqlPredicateParameters
		{
			get
			{
				return this.sqlPredicateParameters;
			}
		}

		public virtual string SqlRestriction
		{
			get
			{
				return this.sqlRestriction;
			}
		}

		public virtual string SqlSort
		{
			get
			{
				return this.sqlSort;
			}
		}

		#endregion

		#region Methods/Operators

		protected virtual string GetRestrictionSql()
		{
			if ((object)this.query.Page == null)
				throw new InvalidOperationException("query.Page");

			if (this.query.Page.PageSize < 0 ||
			    this.query.Page.PageNumber < 0)
				throw new InvalidOperationException("TODO: mesg");

			if (this.query.Page.PageSize == 0 ||
			    this.query.Page.PageNumber == 0)
				return "";
			else
				return string.Format(" TOP {0} ", this.query.Page.PageSize); // no inherent Skip for Take
		}

		protected virtual string GetSortSql()
		{
			List<string> sortNames;
			string sortList;

			string tableAlias, columnAliasedName;

			string columnName;
			ColumnMappingAttribute columnMappingAttribute;

			if ((object)this.query.Orders == null)
				throw new InvalidOperationException("query.Orders");

			if (this.query.Orders.Length <= 0)
				return "";

			tableAlias = string.Format(DefaultSqlCommandProvider.TABLE_ALIAS_FORMAT, this.tableIndex);

			sortNames = new List<string>();

			foreach (Order order in this.query.Orders)
			{
				if ((object)order.Facet == null)
					throw new InvalidOperationException("this.query.Orders[*].Facet");

				columnMappingAttribute = this.columnMappingAttributes.SingleOrDefault(c => c.TargetProperty.Name == order.Facet.Name);

				if ((object)columnMappingAttribute == null)
					throw new InvalidOperationException(string.Format("Invalid column mapping ( ORDER {2} ON PROPERTY {1} ) on type '{0}'.", this.tableMappingAttribute.TargetType.FullName, order.Facet.Name, order.Ascending ? "ascending" : "descending"));

				columnName = string.Format(DefaultSqlCommandProvider.COLUMN_NAME_FORMAT, columnMappingAttribute.Name);

				columnAliasedName = string.Format(DefaultSqlCommandProvider.COLUMN_ALIASED_FORMAT, tableAlias, columnName);

				sortNames.Add(string.Format("{0} {1}", columnAliasedName, order.Ascending ? "ASC" : "DESC"));
			}

			sortList = string.Join(", ", sortNames.ToArray());

			return sortList;
		}

		public virtual void GoVisit(Query query, TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes, int tableIndex, int parameterIndex)
		{
			if ((object)query == null)
				throw new ArgumentNullException("query");

			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException("tableMappingAttribute");

			if ((object)columnMappingAttributes == null)
				throw new ArgumentNullException("columnMappingAttributes");

			this.query = query;
			this.tableMappingAttribute = tableMappingAttribute;
			this.columnMappingAttributes = columnMappingAttributes;
			this.tableIndex = tableIndex;
			this.parameterIndex = parameterIndex;

			this.sqlSort = this.GetSortSql();
			this.sqlRestriction = this.GetRestrictionSql();

			if ((object)query.Expression == null)
				throw new InvalidOperationException("query.Expression");

			this.strings = new StringBuilder();
			this.parameters.Clear();
			//this.parameterIndex = ???

			base.Visit(query.Expression);

			this.sqlPredicate = this.strings.ToString();
			this.sqlPredicateParameters = this.parameters.ToArray();
		}

		protected override Expression VisitBinary(BinaryExpression binaryExpression)
		{
			if ((object)binaryExpression == null)
				throw new ArgumentNullException("binaryExpression");

			this.strings.Append("(");

			this.Visit(binaryExpression.LeftExpression);

			switch (binaryExpression.BinaryOperator)
			{
				case BinaryOperator.And:
					this.strings.Append(" AND ");
					break;
				case BinaryOperator.Or:
					this.strings.Append(" OR ");
					break;
				case BinaryOperator.Eq:
					this.strings.Append(" = ");
					break;
				case BinaryOperator.Ne:
					this.strings.Append(" <> ");
					break;
				case BinaryOperator.Gt:
					this.strings.Append(" > ");
					break;
				case BinaryOperator.Ge:
					this.strings.Append(" >= ");
					break;
				case BinaryOperator.Lt:
					this.strings.Append(" < ");
					break;
				case BinaryOperator.Le:
					this.strings.Append(" <= ");
					break;
				case BinaryOperator.Lk:
					this.strings.Append(" LIKE ");
					break;
				default:
					throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported.", binaryExpression.BinaryOperator));
			}

			this.Visit(binaryExpression.RightExpression);

			this.strings.Append(")");

			return binaryExpression;
		}

		protected override Expression VisitFacet(Facet facet)
		{
			string tableAlias, columnAliasedName;

			string columnName;
			ColumnMappingAttribute columnMappingAttribute;

			if ((object)facet == null)
				throw new ArgumentNullException("facet");

			tableAlias = string.Format(DefaultSqlCommandProvider.TABLE_ALIAS_FORMAT, this.tableIndex);

			columnMappingAttribute = this.columnMappingAttributes.SingleOrDefault(c => c.TargetProperty.Name == facet.Name);

			if ((object)columnMappingAttribute == null)
				throw new InvalidOperationException(string.Format("Invalid column mapping ( ON PROPERTY {1} ) on type '{0}'.", this.tableMappingAttribute.TargetType.FullName, facet.Name));

			columnName = string.Format(DefaultSqlCommandProvider.COLUMN_NAME_FORMAT, columnMappingAttribute.Name);

			columnAliasedName = string.Format(DefaultSqlCommandProvider.COLUMN_ALIASED_FORMAT, tableAlias, columnName);

			this.strings.Append(columnAliasedName);

			return facet;
		}

		protected override Expression VisitNullary(NullaryExpression nullaryExpression)
		{
			if ((object)nullaryExpression == null)
				throw new ArgumentNullException("nullaryExpression");

			this.strings.Append(" (1 = 1) ");

			return nullaryExpression;
		}

		protected override Expression VisitUnary(UnaryExpression unaryExpression)
		{
			if ((object)unaryExpression == null)
				throw new ArgumentNullException("unaryExpression");

			switch (unaryExpression.UnaryOperator)
			{
				case UnaryOperator.Not:
					this.strings.Append(" NOT ");
					this.Visit(unaryExpression.TheExpression);
					break;
				case UnaryOperator.IsNull:
					this.Visit(unaryExpression.TheExpression);
					this.strings.Append(" IS NULL ");
					break;
				case UnaryOperator.IsNotNull:
					this.Visit(unaryExpression.TheExpression);
					this.strings.Append(" IS NOT NULL ");
					break;
				default:
					throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported.", unaryExpression.UnaryOperator));
			}

			return unaryExpression;
		}

		protected override Expression VisitValue(Value value)
		{
			string propertyName, parameterName;
			object result;
			Type valueType;

			if ((object)value == null)
				throw new ArgumentNullException("value");

			if ((object)value.__ == null)
				throw new InvalidOperationException("Cannot use the constant value NULL as a value operand; use UnaryExpression(..., UnaryOperator.IsNull) instead.");

			valueType = value.__.GetType();

			propertyName = string.Format(DefaultSqlCommandProvider.PARAMETER_NAME_FORMAT, this.parameterIndex);
			parameterName = string.Format(DefaultSqlCommandProvider.PARAMETER_NAME_FORMAT, this.parameterIndex);

			this.parameterIndex++;

			this.strings.Append(parameterName);

			this.parameters.Add(new Parameter()
			                    {
			                    	Direction = ParameterDirection.Input,
			                    	Type = MappingUtils.InferDbTypeForClrType(valueType),
			                    	Size = 0,
			                    	Precision = 0,
			                    	Scale = 0,
			                    	Nullable = false,
			                    	Name = parameterName,
			                    	Property = propertyName
			                    });

			this.query.Add(propertyName, new object());
			result = Reflexion.GetSetObjectByPath(this.query, propertyName, value.__);

			if (result == Type.Missing)
				throw new InvalidOperationException(string.Format("Failed to obtain a writable, public instance property '{1}' for the type '{0}'.", this.query.GetType().FullName, propertyName));

			return value;
		}

		#endregion
	}
}