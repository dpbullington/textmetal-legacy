/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Common.Data;
using TextMetal.Common.Data.Framework;
using TextMetal.Common.Data.Framework.LinqToSql;
using TextMetal.Framework.Core;
using TextMetal.HostImpl.AspNetSample.DomainModel;
using TextMetal.HostImpl.AspNetSample.DomainModel.Tables;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Pages
{
	public class Index : ISourceStrategy
	{
		#region Methods/Operators

		public object GetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			Repository repository;
			int ct;

			repository = new Repository();

			repository.TryWriteEventLogEntry(Guid.NewGuid().ToString());

			using (AmbientUnitOfWorkScope scope = new AmbientUnitOfWorkScope(repository))
			{
				IModelQuery modelQuery;

				modelQuery = new LinqTableQuery<IEventLog>(ev => true);

				var list = repository.Find<IEventLog>(modelQuery).ToList();

				ct = list.Count();

				list.ForEach(el => repository.Discard<IEventLog>(el));

				scope.ScopeComplete();
			}

			return new
					{
						Y = "deez nizzles", CT = ct
					};
		}

		#endregion
	}
}