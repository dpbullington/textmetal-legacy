/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Common.Data;
using TextMetal.Framework.Core;
using TextMetal.HostImpl.AspNetSample.Objects.Model;

namespace TextMetal.HostImpl.AspNetSample.Pages
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

			using (AmbientUnitOfWorkScope scope = new AmbientUnitOfWorkScope(Repository.DefaultUnitOfWorkFactory.Instance))
			{
				var list = repository.FindEventLogs((q) => q.OrderBy(ev => ev.CreationTimestamp)).ToList();

				ct = list.Count();

				list.ForEach(el => repository.DiscardEventLog(el));

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