/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Expressions
{
	public abstract class ExpressionVisitor
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ExpressionVisitor class.
		/// </summary>
		protected ExpressionVisitor()
		{
		}

		#endregion

		#region Methods/Operators

		internal IExpression Visit(IExpression expression)
		{
			if ((object)expression == null)
				throw new ArgumentNullException("expression");

			if (expression is NullaryExpression)
				return this.VisitNullary((NullaryExpression)expression);
			else if (expression is UnaryExpression)
				return this.VisitUnary((UnaryExpression)expression);
			else if (expression is BinaryExpression)
				return this.VisitBinary((BinaryExpression)expression);
			else if (expression is SymbolName)
				return this.VisitSymbolName((SymbolName)expression);
			else if (expression is LiteralValue)
				return this.VisitLiteralValue((LiteralValue)expression);
			else
				return this.VisitUnknown(expression);
		}

		protected abstract IExpression VisitBinary(BinaryExpression binaryExpression);

		protected abstract IExpression VisitLiteralValue(LiteralValue literalValue);

		protected abstract IExpression VisitNullary(NullaryExpression nullaryExpression);

		protected abstract IExpression VisitSymbolName(SymbolName symbolName);

		protected abstract IExpression VisitUnary(UnaryExpression unaryExpression);

		protected abstract IExpression VisitUnknown(IExpression expression);

		#endregion
	}
}