/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace TextMetal.Common.Data.Framework.LinqToSql
{
	public interface ILinqToSqlModelRepository<TDataContext> : IModelRepository
		where TDataContext : DataContext
	{
		#region Methods/Operators

		IEnumerable<T> LinqQuery<T>(Func<TDataContext, IQueryable<T>> query);

		IEnumerable<T> LinqQuery<T>(IUnitOfWork unitOfWork,
			Func<TDataContext, IQueryable<T>> query);

		#endregion
	}
}