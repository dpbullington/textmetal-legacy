/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Expressions
{
	/// <summary>
	/// Represents an expression with two operands.
	/// </summary>
	[Serializable]
	public sealed class BinaryExpression : IExpression
	{
		#region Constructors/Destructors

		public BinaryExpression(IExpression leftExpression, BinaryOperator binaryOperator, IExpression rightExpression)
		{
			if ((object)leftExpression == null)
				throw new ArgumentNullException("leftExpression");

			if ((object)rightExpression == null)
				throw new ArgumentNullException("rightExpression");

			this.leftExpression = leftExpression;
			this.binaryOperator = binaryOperator;
			this.rightExpression = rightExpression;
		}

		#endregion

		#region Fields/Constants

		private readonly BinaryOperator binaryOperator;
		private readonly IExpression leftExpression;
		private readonly IExpression rightExpression;

		#endregion

		#region Properties/Indexers/Events

		public BinaryOperator BinaryOperator
		{
			get
			{
				return this.binaryOperator;
			}
		}

		public IExpression LeftExpression
		{
			get
			{
				return this.rightExpression;
			}
		}

		public IExpression RightExpression
		{
			get
			{
				return this.rightExpression;
			}
		}

		#endregion
	}
}