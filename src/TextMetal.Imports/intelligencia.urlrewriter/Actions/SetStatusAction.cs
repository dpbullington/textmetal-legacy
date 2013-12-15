// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Net;

namespace Intelligencia.UrlRewriter.Actions
{
	/// <summary>
	/// Sets the StatusCode.
	/// </summary>
	public class SetStatusAction : IRewriteAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="statusCode"> The status code to set. </param>
		public SetStatusAction(HttpStatusCode statusCode)
		{
			this._statusCode = statusCode;
		}

		#endregion

		#region Fields/Constants

		private HttpStatusCode _statusCode;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The status code.
		/// </summary>
		public HttpStatusCode StatusCode
		{
			get
			{
				return this._statusCode;
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
			context.StatusCode = this.StatusCode;
			return ((int)this.StatusCode >= 300)
				? RewriteProcessing.StopProcessing
				: RewriteProcessing.ContinueProcessing;
		}

		#endregion
	}
}