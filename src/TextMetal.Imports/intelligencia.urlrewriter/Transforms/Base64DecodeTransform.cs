// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Text;

using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Transforms
{
	/// <summary>
	/// Base 64 encodes the input.
	/// </summary>
	public sealed class Base64Transform : IRewriteTransform
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// The name of the action.
		/// </summary>
		public string Name
		{
			get
			{
				return Constants.TransformBase64Decode;
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
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
		}

		#endregion
	}
}