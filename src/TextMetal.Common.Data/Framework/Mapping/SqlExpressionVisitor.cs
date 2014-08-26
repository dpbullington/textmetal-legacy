/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using TextMetal.Common.Data.Framework.Strategy;
using TextMetal.Common.Syntax.Expressions;
using TextMetal.Common.Syntax.Operators;

namespace TextMetal.Common.Data.Framework.Mapping
{
	public sealed class SqlExpressionVisitor : ExpressionVisitor
	{
		#region Constructors/Destructors

		public SqlExpressionVisitor(ISqlNuance sqlNuance, IUnitOfWork unitOfWork, IDictionary<string, IDataParameter> commandParameters)
		{
			if ((object)sqlNuance == null)
				throw new ArgumentNullException("sqlNuance");

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)commandParameters == null)
				throw new ArgumentNullException("commandParameters");

			this.sqlNuance = sqlNuance;
			this.unitOfWork = unitOfWork;
			this.commandParameters = commandParameters;
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary<string, IDataParameter> commandParameters;
		private readonly ISqlNuance sqlNuance;
		private readonly StringBuilder strings = new StringBuilder();
		private readonly IUnitOfWork unitOfWork;

		#endregion

		#region Properties/Indexers/Events

		private IDictionary<string, IDataParameter> CommandParameters
		{
			get
			{
				return this.commandParameters;
			}
		}

		private ISqlNuance SqlNuance
		{
			get
			{
				return this.sqlNuance;
			}
		}

		private StringBuilder Strings
		{
			get
			{
				return this.strings;
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

		public static string GetFilterText(ISqlNuance sqlNuance, IUnitOfWork unitOfWork, IDictionary<string, IDataParameter> commandParameters, IExpression expression)
		{
			SqlExpressionVisitor expressionVisitor;
			string expressionText;

			if ((object)sqlNuance == null)
				throw new ArgumentNullException("sqlNuance");

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)commandParameters == null)
				throw new ArgumentNullException("commandParameters");

			if ((object)expression == null)
				throw new ArgumentNullException("expression");

			expressionVisitor = new SqlExpressionVisitor(sqlNuance, unitOfWork, commandParameters);
			expressionVisitor.Visit(expression);
			expressionText = expressionVisitor.Strings.ToString();

			return expressionText;
		}

		public static string GetSortText(ISqlNuance sqlNuance, IUnitOfWork unitOfWork, IDictionary<string, IDataParameter> commandParameters, IEnumerable<ISequence> sortSequences)
		{
			string expressionText;
			List<string> sortNames;

			if ((object)sqlNuance == null)
				throw new ArgumentNullException("sqlNuance");

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)commandParameters == null)
				throw new ArgumentNullException("commandParameters");

			if ((object)sortSequences == null)
				throw new ArgumentNullException("sortSequences");

			sortNames = new List<string>();

			foreach (var sortSequence in sortSequences)
			{
				if ((object)sortSequence.SortExpression == null)
					continue;

				sortNames.Add(string.Format("{0} {1}", sqlNuance.GetAliasedColumnName("t0", ((ISurface)sortSequence.SortExpression).Name), (sortSequence.SortDirection ?? true) ? "ASC" : "DESC"));
			}

			if (sortNames.Count <= 0)
				sortNames.Add("1");

			expressionText = string.Join(", ", sortNames.ToArray());

			return expressionText;
		}

		protected override IExpression VisitBinary(IBinaryExpression binaryExpression)
		{
			if ((object)binaryExpression == null)
				throw new ArgumentNullException("binaryExpression");

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
				case BinaryOperator.StrLk:
					this.Strings.Append(" LIKE ");
					break;
				case BinaryOperator.Band:
					this.Strings.Append(" & ");
					break;
				case BinaryOperator.Bor:
					this.Strings.Append(" | ");
					break;
				case BinaryOperator.Bxor:
					this.Strings.Append(" ^ ");
					break;
				default:
					throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported.", binaryExpression.BinaryOperator));
			}

			this.Visit(binaryExpression.RightExpression);

			this.Strings.Append(")");

			return binaryExpression;
		}

		protected override IExpression VisitContainer(IContainer container)
		{
			if ((object)container == null)
				throw new ArgumentNullException("container");

			if ((object)container.Content != null)
				this.Visit(container.Content);

			return container;
		}

		protected override IExpression VisitNullary(INullaryExpression nullaryExpression)
		{
			if ((object)nullaryExpression == null)
				throw new ArgumentNullException("nullaryExpression");

			this.Strings.Append(" (1 = 1) ");

			return nullaryExpression;
		}

		protected override IExpression VisitSurface(ISurface surface)
		{
			string columnName;

			if ((object)surface == null)
				throw new ArgumentNullException("surface");

			columnName = this.SqlNuance.GetAliasedColumnName("t0", surface.Name);

			this.Strings.Append(columnName);

			return surface;
		}

		protected override IExpression VisitUnary(IUnaryExpression unaryExpression)
		{
			if ((object)unaryExpression == null)
				throw new ArgumentNullException("unaryExpression");

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
				case UnaryOperator.Incr:
					this.Strings.Append(" (");
					this.Visit(unaryExpression.TheExpression);
					this.Strings.Append(" + 1) ");
					break;
				case UnaryOperator.Decr:
					this.Strings.Append(" (");
					this.Visit(unaryExpression.TheExpression);
					this.Strings.Append(" - 1) ");
					break;
				case UnaryOperator.BComp:
					this.Strings.Append(" ~ ");
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
				throw new ArgumentNullException("expression");

			throw new NotSupportedException(string.Format("The unknown expression of type '{0}' is not supported.", expression.GetType()));
		}

		protected override IExpression VisitValue(IValue value)
		{
			IDataParameter commandParameter;
			string parameterName;
			Type valueType;

			if ((object)value == null)
				throw new ArgumentNullException("value");

			if ((object)value.__ == null)
				throw new InvalidOperationException("Cannot use the constant value NULL as a value operand; use UnaryExpression(..., UnaryOperator.IsNull) instead.");

			parameterName = this.SqlNuance.GetParameterName(string.Format("expr_{0}", Guid.NewGuid().ToString("N")));
			valueType = value.__.GetType();

			commandParameter = this.UnitOfWork.CreateParameter(ParameterDirection.Input, AdoNetHelper.InferDbTypeForClrType(valueType), 0, 0, 0, true, parameterName, value.__);
			this.SqlNuance.ParameterMagic(this.unitOfWork, commandParameter, "");
			this.CommandParameters.Add(parameterName, commandParameter);

			this.Strings.Append(parameterName);

			return value;
		}

		#endregion
	}
}