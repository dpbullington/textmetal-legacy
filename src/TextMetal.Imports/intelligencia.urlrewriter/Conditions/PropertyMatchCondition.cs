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
	/// Performs a property match.
	/// </summary>
	public sealed class PropertyMatchCondition : MatchCondition
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="propertyName"> The property name </param>
		/// <param name="pattern"> The pattern </param>
		public PropertyMatchCondition(string propertyName, string pattern)
			: base(pattern)
		{
			if (propertyName == null)
				throw new ArgumentNullException("propertyName");
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			this._propertyName = propertyName;
		}

		#endregion

		#region Fields/Constants

		private string _propertyName = String.Empty;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The property name.
		/// </summary>
		public string PropertyName
		{
			get
			{
				return this._propertyName;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Determines if the condition is matched.
		/// </summary>
		/// <param name="context"> The rewriting context. </param>
		/// <returns> True if the condition is met. </returns>
		public override bool IsMatch(RewriteContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			string property = context.Properties[this.PropertyName];
			if (property != null)
			{
				Match match = this.Pattern.Match(property);
				if (match.Success)
					context.LastMatch = match;
				return match.Success;
			}

			return false;
		}

		#endregion
	}
}