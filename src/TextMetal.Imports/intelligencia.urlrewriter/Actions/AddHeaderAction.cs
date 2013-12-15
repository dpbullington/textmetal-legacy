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
	/// Action that adds a given header.
	/// </summary>
	public class AddHeaderAction : IRewriteAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="header"> The header name. </param>
		/// <param name="value"> The header value. </param>
		public AddHeaderAction(string header, string value)
		{
			if (header == null)
				throw new ArgumentNullException("header");
			if (value == null)
				throw new ArgumentNullException("value");
			this._header = header;
			this._value = value;
		}

		#endregion

		#region Fields/Constants

		private string _header;
		private string _value;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The header name.
		/// </summary>
		public string Header
		{
			get
			{
				return this._header;
			}
		}

		/// <summary>
		/// The header value.
		/// </summary>
		public string Value
		{
			get
			{
				return this._value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Executes the action.
		/// </summary>
		/// <param name="context"> The rewrite context. </param>
		public RewriteProcessing Execute(RewriteContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			context.ResponseHeaders.Add(this.Header, this.Value);
			return RewriteProcessing.ContinueProcessing;
		}

		#endregion
	}
}