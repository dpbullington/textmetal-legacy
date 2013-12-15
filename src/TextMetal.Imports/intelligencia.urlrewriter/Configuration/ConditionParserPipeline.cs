// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections;

using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Configuration
{
	/// <summary>
	/// Pipeline for creating the Condition parsers.
	/// </summary>
	public class ConditionParserPipeline : CollectionBase
	{
		#region Methods/Operators

		/// <summary>
		/// Adds a parser.
		/// </summary>
		/// <param name="parserType"> The parser type. </param>
		public void AddParser(string parserType)
		{
			this.AddParser((IRewriteConditionParser)TypeHelper.Activate(parserType, null));
		}

		/// <summary>
		/// Adds a parser.
		/// </summary>
		/// <param name="parser"> The parser. </param>
		public void AddParser(IRewriteConditionParser parser)
		{
			this.InnerList.Add(parser);
		}

		#endregion
	}
}