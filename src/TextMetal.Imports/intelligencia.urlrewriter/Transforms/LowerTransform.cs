// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Threading;

using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Transforms
{
	/// <summary>
	/// Transforms the input to lower case.
	/// </summary>
	public sealed class LowerTransform : IRewriteTransform
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// The name of the action.
		/// </summary>
		public string Name
		{
			get
			{
				return Constants.TransformLower;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Applies a transformation to the input string.
		/// </summary>
		/// <param name="input"> The input string. </param>
		/// <returns> The transformed string. </returns>
		public string ApplyTransform(string input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			return input.ToLower(Thread.CurrentThread.CurrentCulture);
		}

		#endregion
	}
}