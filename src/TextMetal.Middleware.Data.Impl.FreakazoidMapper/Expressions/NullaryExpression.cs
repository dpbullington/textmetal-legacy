/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Expressions
{
	/// <summary>
	/// Represents an expression with zero operands.
	/// </summary>
	[Serializable]
	public sealed class NullaryExpression : IExpression
	{
		#region Constructors/Destructors

		public NullaryExpression(NullaryOperator nullaryOperator)
		{
			this.nullaryOperator = nullaryOperator;
		}

		#endregion

		#region Fields/Constants

		private readonly NullaryOperator nullaryOperator;

		#endregion

		#region Properties/Indexers/Events

		public NullaryOperator NullaryOperator
		{
			get
			{
				return this.nullaryOperator;
			}
		}

		#endregion
	}
}