/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using TextMetal.Middleware.Datazoid.Models.Tabular;
using TextMetal.Middleware.Datazoid.Repositories.Impl.Expressions;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl
{
	public sealed class DzTableModelQuery : ITableModelQuery
	{
		#region Constructors/Destructors

		public DzTableModelQuery(IExpression filterExpression, IEnumerable<SortOrder> sortOrders)
		{
			if ((object)filterExpression == null)
				throw new ArgumentNullException(nameof(filterExpression));

			if ((object)sortOrders == null)
				throw new ArgumentNullException(nameof(sortOrders));

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