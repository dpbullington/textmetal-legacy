/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Common.Data.Framework
{
	/// <summary>
	/// Provides a contract for response model objects (procedure, function, etc.).
	/// </summary>
	public interface IResponseModelObject<TResultModel>
		where TResultModel : class, IResultModelObject
	{
		#region Properties/Indexers/Events

		IEnumerable<TResultModel> Results
		{
			get;
			set;
		}

		#endregion
	}
}