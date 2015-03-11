/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using TextMetal.Common.Data.Framework.Syntax.Operators;

namespace TextMetal.Common.Data.Framework.Syntax.Expressions
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