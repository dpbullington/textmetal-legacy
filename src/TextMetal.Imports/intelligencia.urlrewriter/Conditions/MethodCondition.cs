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
	/// Matches on the current method.
	/// </summary>
	public sealed class MethodCondition : MatchCondition
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="pattern"> </param>
		public MethodCondition(string pattern)
			: base(GetMethodPattern(pattern))
		{
		}

		#endregion

		#region Methods/Operators

		private static string GetMethodPattern(string method)
		{
			// Convert the "GET,POST,*" pattern to a regex, e.g. "^GET|POST|.+$".
			return String.Format("^{0}$", Regex.Replace(method, @"[^a-zA-Z,\*]+", "").Replace(",", "|").Replace("*", ".+"));
		}

		/// <summary>
		/// Determines if the condition is matched.
		/// </summary>
		/// <param name="context"> The rewriting context. </param>
		/// <returns> True if the condition is met. </returns>
		public override bool IsMatch(RewriteContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			return this.Pattern.IsMatch(context.Method);
		}

		#endregion
	}
}