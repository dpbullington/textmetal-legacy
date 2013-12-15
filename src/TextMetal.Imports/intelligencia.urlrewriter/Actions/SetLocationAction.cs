// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;

namespace Intelligencia.UrlRewriter.Actions
{
	/// <summary>
	/// Sets the Location.
	/// </summary>
	public abstract class SetLocationAction : IRewriteAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="location"> The location (pattern) to set. </param>
		protected SetLocationAction(string location)
		{
			if (location == null)
				throw new ArgumentNullException("location");
			this._location = location;
		}

		#endregion

		#region Fields/Constants

		private string _location;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The location to set.  This can include replacements referencing the matched pattern,
		/// for example $1, $2, ... $n and ${group} as well as ${ServerVariable} and mapping, e.g.,
		/// ${MapName:$1}.
		/// </summary>
		public string Location
		{
			get
			{
				return this._location;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Executes the action.
		/// </summary>
		/// <param name="context"> The rewriting context. </param>
		public virtual RewriteProcessing Execute(RewriteContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			context.Location = context.ResolveLocation(context.Expand(this.Location));
			return RewriteProcessing.StopProcessing;
		}

		#endregion
	}
}