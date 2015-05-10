/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using TextMetal.Middleware.Data.Impl.FreakazoidMapper.Expressions;
using TextMetal.Middleware.Data.Models.Tabular;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper
{
	public sealed class FreakazoidTableModelQuery : ITableModelQuery
	{
		#region Constructors/Destructors

		public FreakazoidTableModelQuery(IExpression filterExpression, IEnumerable<SortOrder> sortOrders)
		{
			if ((object)filterExpression == null)
				throw new ArgumentNullException("filterExpression");

			if ((object)sortOrders == null)
				throw new ArgumentNullException("sortOrders");

			this.filterExpression = filterExpression;
			this.sortOrders = sortOrders;
		}

		#endregion

		#region Fields/Constants

		private readonly IExpression filterExpression;
		private readonly IEnumerable<SortOrder> sortOrders;

		#endregion

		#region Properties/Indexers/Events

		public IExpression FilterExpression
		{
			get
			{
				return this.filterExpression;
			}
		}

		public IEnumerable<SortOrder> SortOrders
		{
			get
			{
				return this.sortOrders;
			}
		}

		#endregion

		#region Methods/Operators

		public Expression GetCanonicalExpression()
		{
			return null;
		}

		#endregion
	}
}