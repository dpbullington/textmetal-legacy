﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Framework.Tokenization
{
	/// <summary>
	/// Provides a dynamic token replacement strategy which executes an on-demand callback method to obtain a replacement value.
	/// </summary>
	public class ContextualDynamicValueTokenReplacementStrategy<TContext> : ITokenReplacementStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ContextualDynamicValueTokenReplacementStrategy class.
		/// </summary>
		/// <param name="method"> The callback method to evaluate during token replacement. </param>
		/// <param name="context"> The context object used during token replacement. </param>
		public ContextualDynamicValueTokenReplacementStrategy(Func<TContext, string[], object> method, TContext context)
		{
			if ((object)method == null)
				throw new ArgumentNullException(nameof(method));

			this.method = method;
			this.context = context;
		}

		#endregion

		#region Fields/Constants

		private readonly TContext context;
		private readonly Func<TContext, string[], object> method;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the context object used during token replacement.
		/// </summary>
		public TContext Context
		{
			get
			{
				return this.context;
			}
		}

		/// <summary>
		/// Gets the callback method to evaluate during token replacement.
		/// </summary>
		public Func<TContext, string[], object> Method
		{
			get
			{
				return this.method;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Evaluate a token using any parameters specified.
		/// </summary>
		/// <param name="parameters"> Should be null for value semantics; or a valid string array for function semantics. </param>
		/// <returns> An approapriate token replacement value. </returns>
		public object Evaluate(string[] parameters)
		{
			return this.Method(this.Context, parameters);
		}

		#endregion
	}
}