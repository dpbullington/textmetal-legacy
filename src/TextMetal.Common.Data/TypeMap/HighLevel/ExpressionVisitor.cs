/*
	Copyright ©2002-2010 Daniel Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap.HighLevel
{
	public abstract class ExpressionVisitor
	{
		#region Constructors/Destructors

		protected ExpressionVisitor()
		{
		}

		#endregion

		#region Methods/Operators

		protected virtual Expression Visit(Expression expression)
		{
			if ((object)expression == null)
				throw new ArgumentNullException("expression");

			if (expression is NullaryExpression)
				return this.VisitNullary((NullaryExpression)expression);
			else if (expression is UnaryExpression)
				return this.VisitUnary((UnaryExpression)expression);
			else if (expression is BinaryExpression)
				return this.VisitBinary((BinaryExpression)expression);
			else if (expression is Facet)
				return this.VisitFacet((Facet)expression);
			else if (expression is Value)
				return this.VisitValue((Value)expression);
			else
				return this.VisitUnknown(expression);
		}

		protected virtual Expression VisitBinary(BinaryExpression binaryExpression)
		{
			Expression leftExpression, rightExpression;

			if ((object)binaryExpression == null)
				throw new ArgumentNullException("binaryExpression");

			leftExpression = this.Visit(binaryExpression.LeftExpression);
			rightExpression = this.Visit(binaryExpression.RightExpression);

			return new BinaryExpression(leftExpression, rightExpression, binaryExpression.BinaryOperator);
		}

		protected virtual Expression VisitFacet(Facet facet)
		{
			if ((object)facet == null)
				throw new ArgumentNullException("facet");

			return facet;
		}

		protected virtual Expression VisitNullary(NullaryExpression nullaryExpression)
		{
			if ((object)nullaryExpression == null)
				throw new ArgumentNullException("nullaryExpression");

			return nullaryExpression;
		}

		protected virtual Expression VisitUnary(UnaryExpression unaryExpression)
		{
			Expression theExpression;

			if ((object)unaryExpression == null)
				throw new ArgumentNullException("unaryExpression");

			theExpression = this.Visit(unaryExpression.TheExpression);

			return new UnaryExpression(theExpression, unaryExpression.UnaryOperator);
		}

		protected virtual Expression VisitUnknown(Expression expression)
		{
			if ((object)expression == null)
				throw new ArgumentNullException("expression");

			throw new Exception(string.Format("Unknown expression type encountered: '{0}'.", expression.GetType().FullName));
		}

		protected virtual Expression VisitValue(Value value)
		{
			if ((object)value == null)
				throw new ArgumentNullException("value");

			return value;
		}

		#endregion
	}
}