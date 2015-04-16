/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Data.Containers
{
	/// <summary>
	/// Provides a contract for response service model objects.
	/// </summary>
	public interface IResponseDataTransferObject<TResultServiceModelObject> : IResponseDataTransferObject
		where TResultServiceModelObject : class, IResultDataTransferObject
	{
		#region Properties/Indexers/Events

		List<TResultServiceModelObject> Results
		{
			get;
			set;
		}

		#endregion
	}
}