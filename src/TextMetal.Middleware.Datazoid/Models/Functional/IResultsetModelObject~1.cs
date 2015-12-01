/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace TextMetal.Middleware.Datazoid.Models.Functional
{
	public interface IResultsetModelObject<TResultProcedureModelObject> : IResultsetModelObject
		where TResultProcedureModelObject : class, IResultProcedureModelObject
	{
		#region Properties/Indexers/Events

		IEnumerable<TResultProcedureModelObject> Records
		{
			get;
			set;
		}

		#endregion
	}
}