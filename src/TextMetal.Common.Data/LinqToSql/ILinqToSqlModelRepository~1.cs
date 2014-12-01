/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data.Linq;

using TextMetal.Common.Data.Framework;

namespace TextMetal.Common.Data.LinqToSql
{
	public interface ILinqToSqlModelRepository<TDataContext> : IModelRepository
		where TDataContext : DataContext
	{
		#region Methods/Operators

		TProjection LinqQuery<TProjection>(Func<TDataContext, TProjection> dataContextQueryCallback);

		TProjection LinqQuery<TProjection>(IUnitOfWork unitOfWork,
			Func<TDataContext, TProjection> dataContextQueryCallback);

		#endregion
	}
}