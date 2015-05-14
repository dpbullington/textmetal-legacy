/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

namespace TextMetal.Middleware.Data.UoW
{
	public interface IUnitOfWorkFactory
	{
		#region Methods/Operators

		IUnitOfWork GetUnitOfWork(bool transactional, IsolationLevel isolationLevel = IsolationLevel.Unspecified);

		#endregion
	}
}