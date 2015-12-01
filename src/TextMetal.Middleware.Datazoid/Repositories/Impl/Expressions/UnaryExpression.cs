/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Expressions
{
	/// <summary>
	/// Represents an expression with one operand.
	/// </summary>
	[Serializable]
	public sealed class UnaryExpression : IExpression
	{
		#region Constructors/Destructors

		public UnaryExpression(IExpression theExpression, UnaryOperator unaryOperator)
		{
			if ((object)theExpression == null)
				throw new ArgumentNullException("theExpression");

			this.theExpression = theExpression;
			this.unaryOperator = unaryOperator;
		}

		#endregion

		#region Fields/Constants

		private readonly IExpression theExpression;
		private readonly UnaryOperator unaryOperator;

		#endregion

		#region Properties/Indexers/Events

		public IExpression TheExpression
		{
			get
			{
				return this.theExpression;
			}
		}

		public UnaryOperator UnaryOperator
		{
			get
			{
				return this.unaryOperator;
			}
		}

		#endregion
	}
}