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
	/// Factory for creating the action parsers.
	/// </summary>
	public class ActionParserFactory
	{
		#region Fields/Constants

		private Dictionary<string, IList<IRewriteActionParser>> _parsers = new Dictionary<string, IList<IRewriteActionParser>>();

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Adds a parser.
		/// </summary>
		/// <param name="parserType"> The parser type. </param>
		public void AddParser(string parserType)
		{
			this.AddParser((IRewriteActionParser)TypeHelper.Activate(parserType, null));
		}

		/// <summary>
		/// Adds a parser.
		/// </summary>
		/// <param name="parser"> The parser. </param>
		public void AddParser(IRewriteActionParser parser)
		{
			if (parser == null)
				throw new ArgumentNullException("parser");

			IList<IRewriteActionParser> list;

			if (this._parsers.ContainsKey(parser.Name))
				list = this._parsers[parser.Name];
			else
			{
				list = new List<IRewriteActionParser>();
				this._parsers.Add(parser.Name, list);
			}

			list.Add(parser);
		}

		/// <summary>
		/// Returns a list of parsers for the given verb.
		/// </summary>
		/// <param name="verb"> The verb. </param>
		/// <returns> A list of parsers </returns>
		public IList<IRewriteActionParser> GetParsers(string verb)
		{
			return (this._parsers.ContainsKey(verb))
				? this._parsers[verb]
				: null;
		}

		#endregion
	}
}