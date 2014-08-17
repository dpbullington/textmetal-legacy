/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Linq.Expressions;

namespace TextMetal.Common.Data.Framework.LinqToSql
{
	public sealed class LinqTableQuery<TTable> : IModelQuery
		where TTable : class, new()
	{
		#region Constructors/Destructors

		public LinqTableQuery(Expression<Func<TTable, bool>> predicate)
		{
			if ((object)predicate == null)
				throw new ArgumentNullException("predicate");

			this.predicate = predicate;
		}

		#endregion

		#region Fields/Constants

		private readonly Expression<Func<TTable, bool>> predicate;

		#endregion

		#region Properties/Indexers/Events

		private Expression<Func<TTable, bool>> Predicate
		{
			get
			{
				return this.predicate;
			}
		}

		#endregion

		#region Methods/Operators

		public object GetUnderlyingQuery()
		{
			return this.Predicate;
		}

		#endregion
	}
}