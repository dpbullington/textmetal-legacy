// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Specialized;

namespace Intelligencia.UrlRewriter.Transforms
{
	/// <summary>
	/// Default RewriteMapper, reads its maps from config.
	/// The mapping is CASE-INSENSITIVE.
	/// </summary>
	public sealed class StaticMappingTransform : IRewriteTransform
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="name"> The name of the mapping. </param>
		/// <param name="map"> The mappings. </param>
		public StaticMappingTransform(string name, StringDictionary map)
		{
			this._name = name;
			this._map = map;
		}

		#endregion

		#region Fields/Constants

		private StringDictionary _map;
		private string _name;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The name of the action.
		/// </summary>
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Maps the specified value in the specified map to its replacement value.
		/// </summary>
		/// <param name="input"> The value being mapped. </param>
		/// <returns> The value mapped to, or null if no mapping could be performed. </returns>
		public string ApplyTransform(string input)
		{
			return this._map[input];
		}

		#endregion
	}
}