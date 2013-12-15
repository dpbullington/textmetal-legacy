// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Generic;
using System.Net;

namespace Intelligencia.UrlRewriter.Actions
{
	/// <summary>
	/// Redirect using 302 temporary redirect.
	/// </summary>
	public sealed class RedirectAction : SetLocationAction, IRewriteCondition
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="location"> The location to set. </param>
		/// <param name="permanent"> Whether the redirection is permanent. </param>
		public RedirectAction(string location, bool permanent)
			: base(location)
		{
			if (location == null)
				throw new ArgumentNullException("location");
			this._permanent = permanent;
		}

		#endregion

		#region Fields/Constants

		private IList<IRewriteCondition> _conditions = new List<IRewriteCondition>();
		private bool _permanent;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Conditions that must hold for the rule to fire.
		/// </summary>
		public IList<IRewriteCondition> Conditions
		{
			get
			{
				return this._conditions;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Executes the action.
		/// </summary>
		/// <param name="context"> The rewriting context. </param>
		public override RewriteProcessing Execute(RewriteContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			base.Execute(context);

			context.StatusCode = (this._permanent)
				? HttpStatusCode.Moved
				: HttpStatusCode.Found;

			return RewriteProcessing.StopProcessing;
		}

		/// <summary>
		/// Determines if the rewrite rule matches.
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		public bool IsMatch(RewriteContext context)
		{
			// Ensure the conditions are met.
			foreach (IRewriteCondition condition in this.Conditions)
			{
				if (!condition.IsMatch(context))
					return false;
			}

			return true;
		}

		#endregion
	}
}