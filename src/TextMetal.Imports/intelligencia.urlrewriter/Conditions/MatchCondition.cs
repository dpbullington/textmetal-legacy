// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Text.RegularExpressions;

namespace Intelligencia.UrlRewriter.Conditions
{
	/// <summary>
	/// Base class for MatchConditions.
	/// </summary>
	public abstract class MatchCondition : IRewriteCondition
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="pattern"> Pattern to match. </param>
		protected MatchCondition(string pattern)
		{
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			this._pattern = new Regex(pattern, RegexOptions.IgnoreCase);
		}

		#endregion

		#region Fields/Constants

		private Regex _pattern;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The pattern to match.
		/// </summary>
		public Regex Pattern
		{
			get
			{
				return this._pattern;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Determines if the condition is matched.
		/// </summary>
		/// <param name="context"> The rewriting context. </param>
		/// <returns> True if the condition is met. </returns>
		public abstract bool IsMatch(RewriteContext context);

		#endregion
	}
}