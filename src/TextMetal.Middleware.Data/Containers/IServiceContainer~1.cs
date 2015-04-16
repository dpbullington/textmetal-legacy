/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Data.Models;

namespace TextMetal.Middleware.Data.Containers
{
	public interface IServiceContainer<TRepository> : IServiceContainer
		where TRepository : class, IModelRepository
	{
		#region Properties/Indexers/Events

		TRepository Repository
		{
			get;
		}

		#endregion
	}
}