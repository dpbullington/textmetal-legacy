/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using TextMetal.Common.Syntax.Operators;

namespace TextMetal.Common.Syntax.Expressions
{
	/// <summary>
	/// Represents an expression with two operands.
	/// </summary>
	public interface IBinaryExpression : IExpression
	{
		#region Properties/Indexers/Events

		BinaryOperator BinaryOperator
		{
			get;
		}

		IExpression LeftExpression
		{
			get;
		}

		IExpression RightExpression
		{
			get;
		}

		#endregion
	}
}