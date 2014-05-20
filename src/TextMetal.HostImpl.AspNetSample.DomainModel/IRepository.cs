/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

using TextMetal.Common.Data;
using TextMetal.HostImpl.AspNetSample.Common;

namespace TextMetal.HostImpl.AspNetSample.DomainModel
{
	public partial interface IRepository
	{
		#region Methods/Operators

		IEnumerable<TResultEntity> Find<TDataContext, TResultEntity>(TDataContext dummy, Func<TDataContext, IQueryable<TResultEntity>> callback)
			where TDataContext : DataContext;

		IEnumerable<TResultEntity> Find<TDataContext, TResultEntity>(TDataContext dummy, IUnitOfWork unitOfWork, Func<TDataContext, IQueryable<TResultEntity>> callback)
			where TDataContext : DataContext;

		IEnumerable<IListItem<int?>> GetSecurityRoles();

		bool TrySendEmailTemplate(string templateResourceName, object modelObject);

		bool TryWriteEventLogEntry(string eventText);

		#endregion
	}
}