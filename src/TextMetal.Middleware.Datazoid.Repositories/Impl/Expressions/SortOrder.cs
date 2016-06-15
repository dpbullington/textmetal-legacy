/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Expressions
{
	/// <summary>
	/// Represents a sequence.
	/// </summary>
	public sealed class SortOrder : IExpression
	{
		#region Constructors/Destructors

		public SortOrder(SymbolName symbolName, SortDirection sortDirection)
		{
			if ((object)symbolName == null)
				throw new ArgumentNullException(nameof(symbolName));

			this.symbolName = symbolName;
			this.sortDirection = sortDirection;
		}

		#endregion

		#region Fields/Constants

		private readonly SortDirection sortDirection;
		private readonly SymbolName symbolName;

		#endregion

		#region Properties/Indexers/Events

		public SortDirection SortDirection
		{
			get
			{
				return this.sortDirection;
			}
		}

		public SymbolName SymbolName
		{
			get
			{
				return this.symbolName;
			}
		}

		#endregion
	}
}