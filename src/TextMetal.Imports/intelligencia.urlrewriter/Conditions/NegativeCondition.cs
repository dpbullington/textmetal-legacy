// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;

namespace Intelligencia.UrlRewriter.Conditions
{
	/// <summary>
	/// Performs a negation of the given conditions.
	/// </summary>
	public sealed class NegativeCondition : IRewriteCondition
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="chainedCondition"> </param>
		public NegativeCondition(IRewriteCondition chainedCondition)
		{
			if (chainedCondition == null)
				throw new ArgumentNullException("chainedCondition");
			this._chainedCondition = chainedCondition;
		}

		#endregion

		#region Fields/Constants

		private IRewriteCondition _chainedCondition;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Determines if the condition is matched.
		/// </summary>
		/// <param name="context"> The rewriting context. </param>
		/// <returns> True if the condition is met. </returns>
		public bool IsMatch(RewriteContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			return !this._chainedCondition.IsMatch(context);
		}

		#endregion
	}
}