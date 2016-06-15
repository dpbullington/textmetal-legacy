/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Datazoid.UoW;

namespace TextMetal.Middleware.Datazoid.Repositories
{
	public interface IModelRepository : IUnitOfWorkFactory
	{
		#region Properties/Indexers/Events

		bool ForceEagerLoading
		{
			get;
			set;
		}

		#endregion
	}
}