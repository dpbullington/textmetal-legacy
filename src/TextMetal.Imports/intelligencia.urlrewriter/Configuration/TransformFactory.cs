// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Generic;

using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Configuration
{
	/// <summary>
	/// Factory for creating transforms.
	/// </summary>
	public class TransformFactory
	{
		#region Fields/Constants

		private IDictionary<string, IRewriteTransform> _transforms = new Dictionary<string, IRewriteTransform>();

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Adds a transform.
		/// </summary>
		/// <param name="transformType"> The type of the transform. </param>
		public void AddTransform(string transformType)
		{
			this.AddTransform((IRewriteTransform)TypeHelper.Activate(transformType, null));
		}

		/// <summary>
		/// Adds a transform.
		/// </summary>
		/// <param name="transform"> The transform object. </param>
		public void AddTransform(IRewriteTransform transform)
		{
			if (transform == null)
				throw new ArgumentNullException("transform");

			this._transforms.Add(transform.Name, transform);
		}

		/// <summary>
		/// Gets a transform by name.
		/// </summary>
		/// <param name="name"> The transform name. </param>
		/// <returns> The transform object. </returns>
		public IRewriteTransform GetTransform(string name)
		{
			return this._transforms[name];
		}

		#endregion
	}
}