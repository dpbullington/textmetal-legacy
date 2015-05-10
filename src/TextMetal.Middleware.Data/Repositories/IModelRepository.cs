/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;

namespace TextMetal.Middleware.Data.Repositories
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