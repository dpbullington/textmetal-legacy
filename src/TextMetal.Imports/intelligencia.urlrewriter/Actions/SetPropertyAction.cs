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
	/// Action that sets properties in the context.
	/// </summary>
	public class SetPropertyAction : IRewriteAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="name"> The name of the variable. </param>
		/// <param name="value"> The name of the value. </param>
		public SetPropertyAction(string name, string value)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (value == null)
				throw new ArgumentNullException("value");

			this._name = name;
			this._value = value;
		}

		#endregion

		#region Fields/Constants

		private string _name;
		private string _value;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The name of the variable.
		/// </summary>
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		/// <summary>
		/// The value of the variable.
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
			context.Properties.Set(this.Name, context.Expand(this.Value));
			return RewriteProcessing.ContinueProcessing;
		}

		#endregion
	}
}