/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using TextMetal.Middleware.Datazoid.Repositories.Impl.Expressions;
using TextMetal.Middleware.Datazoid.Repositories.Impl.Mappings;
using TextMetal.Middleware.Datazoid.Repositories.Impl.Tactics;
using TextMetal.Middleware.Datazoid.UoW;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Strategies
{
	public sealed class SqlExpressionVisitor : ExpressionVisitor
	{
		#region Constructors/Destructors

		public SqlExpressionVisitor(TableMappingAttribute tableMappingAttribute, ISqlNuance sqlNuance, IUnitOfWork unitOfWork, IDictionary<string, ITacticParameter> tacticParameters)
		{
			if ((object)tableMappingAttribute == null)
				throw new ArgumentNullException(nameof(tableMappingAttribute));

			if ((object)sqlNuance == null)
				throw new ArgumentNullException(nameof(sqlNuance));

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)tacticParameters == null)
				throw new ArgumentNullException(nameof(tacticParameters));

			this.tableMappingAttribute = tableMappingAttribute;
			this.sqlNuance = sqlNuance;
			this.unitOfWork = unitOfWork;
			this.tacticParameters = tacticParameters;
		}

		#endregion

		#region Fields/Constants

		private readonly ISqlNuance sqlNuance;
		private readonly StringBuilder strings = new StringBuilder();
		private readonly TableMappingAttribute tableMappingAttribute;
		private readonly IDictionary<string, ITacticParameter> tacticParameters;
		private readonly IUnitOfWork unitOfWork;

		#endregion

		#region Properties/Indexers/Events

		private TableMappingAttribute MappingAttribute
		{
			get
			{
				return this.tableMappingAttribute;
			}
		}

		private ISqlNuance SqlNuance
		{
			get
			{
				return this.sqlNuance;
			}
		}

		internal StringBuilder Strings
		{
			get
			{
				return this.strings;
			}
		}

		private IDictionary<string, ITacticParameter> TacticParameters
		{
			get
			{
				return this.tacticParameters;
			}
		}

		private IUnitOfWork UnitOfWork
		{
			get
			{
				return this.unitOfWork;
			}
		}

		#endregion

		#region Methods/Operators

		protected override IExpression VisitBinary(BinaryExpression binaryExpression)
		{
			if ((object)binaryExpression == null)
				throw new ArgumentNullException(nameof(binaryExpression));

			this.Strings.Append("(");

			this.Visit(binaryExpression.LeftExpression);

			switch (binaryExpression.BinaryOperator)
			{
				case BinaryOperator.Add:
					this.Strings.Append(" + ");
					break;
				case BinaryOperator.Sub:
					this.Strings.Append(" - ");
					break;
				case BinaryOperator.Div:
					this.Strings.Append(" / ");
					break;
				case BinaryOperator.Mul:
					this.Strings.Append(" * ");
					break;
				case BinaryOperator.Mod:
					this.Strings.Append(" % ");
					break;
				case BinaryOperator.And:
					this.Strings.Append(" AND ");
					break;
				case BinaryOperator.Or:
					this.Strings.Append(" OR ");
					break;
				case BinaryOperator.Eq:
					this.Strings.Append(" = ");
					break;
				case BinaryOperator.Ne:
					this.Strings.Append(" <> ");
					break;
				case BinaryOperator.Gt:
					this.Strings.Append(" > ");
					break;
				case BinaryOperator.Ge:
					this.Strings.Append(" >= ");
					break;
				case BinaryOperator.Lt:
					this.Strings.Append(" < ");
					break;
				case BinaryOperator.Le:
					this.Strings.Append(" <= ");
					break;
				case BinaryOperator.Like:
					this.Strings.Append(" LIKE ");
					break;
				default:
					throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported.", binaryExpression.BinaryOperator));
			}

			this.Visit(binaryExpression.RightExpression);

			this.Strings.Append(")");

			return binaryExpression;
		}

		protected override IExpression VisitLiteralValue(LiteralValue literalValue)
		{
			ITacticParameter tacticParameter;
			string parameterName;
			Type valueType;

			if ((object)literalValue == null)
				throw new ArgumentNullException(nameof(literalValue));

			if ((object)literalValue.__ == null)
				throw new InvalidOperationException("Cannot use the constant literal value NULL as a literalValue operand; use UnaryExpression(..., UnaryOperator.IsNull) instead.");

			parameterName = this.SqlNuance.GetParameterName(string.Format("expr_{0}", Guid.NewGuid().ToString("N")));
			valueType = literalValue.__.GetType();

			/*
				BACKLOG(dpbullington@gmail.com / 2015 - 12 - 18):
				Find the column mapping attribute and lookup this junk...
			*/
			tacticParameter = new TacticParameter()
							{
								ParameterDirection = ParameterDirection.Input,
								ParameterDbType = AdoNetLiteFascade.Instance.InferDbTypeForClrType(valueType),
								ParameterSize = default(int),
								ParameterPrecision = default(byte),
								ParameterScale = default(byte),
								ParameterNullable = true,
								ParameterName = parameterName,
								ParameterValue = literalValue.__
							};

			this.TacticParameters.Add(parameterName, tacticParameter);

			this.Strings.Append(parameterName);

			return literalValue;
		}

		protected override IExpression VisitNullary(NullaryExpression nullaryExpression)
		{
			if ((object)nullaryExpression == null)
				throw new ArgumentNullException(nameof(nullaryExpression));

			switch (nullaryExpression.NullaryOperator)
			{
				case NullaryOperator.Nop:
					this.Strings.Append(" (-1 <> 1) ");
					break;
				default:
					throw new NotSupportedException(string.Format("The nullary operator '{0}' is not supported.", nullaryExpression.NullaryOperator));
			}

			return nullaryExpression;
		}

		protected override IExpression VisitSymbolName(SymbolName symbolName)
		{
			string columnName;

			if ((object)symbolName == null)
				throw new ArgumentNullException(nameof(symbolName));

			columnName = this.SqlNuance.GetAliasedColumnName("t0", symbolName.Name);

			this.Strings.Append(columnName);

			return symbolName;
		}

		protected override IExpression VisitUnary(UnaryExpression unaryExpression)
		{
			if ((object)unaryExpression == null)
				throw new ArgumentNullException(nameof(unaryExpression));

			switch (unaryExpression.UnaryOperator)
			{
				case UnaryOperator.Not:
					this.Strings.Append(" NOT ");
					this.Visit(unaryExpression.TheExpression);
					break;
				case UnaryOperator.IsNull:
					this.Visit(unaryExpression.TheExpression);
					this.Strings.Append(" IS NULL ");
					break;
				case UnaryOperator.IsNotNull:
					this.Visit(unaryExpression.TheExpression);
					this.Strings.Append(" IS NOT NULL ");
					break;
				case UnaryOperator.Neg:
					this.Strings.Append(" - ");
					this.Visit(unaryExpression.TheExpression);
					break;
				case UnaryOperator.Pos:
					this.Strings.Append(" + ");
					this.Visit(unaryExpression.TheExpression);
					break;
				default:
					throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported.", unaryExpression.UnaryOperator));
			}

			return unaryExpression;
		}

		protected override IExpression VisitUnknown(IExpression expression)
		{
			if ((object)expression == null)
				throw new ArgumentNullException(nameof(expression));

			if ((object)expression == VoidExpression.Instance)
			{
				this.Strings.Append(string.Empty);
				return expression;
			}

			throw new NotSupportedException(string.Format("The unknown expression of type '{0}' is not supported.", expression.GetType()));
		}

		#endregion
	}
}