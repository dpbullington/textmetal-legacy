/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Datazoid.Models.Functional
{
	/// <summary>
	/// Provides a contract for return procedure model objects (procedure, function, packages, etc.).
	/// </summary>
	public interface IReturnProcedureModelObject<TResultsetModelObject, TResultProcedureModelObject> : IReturnProcedureModelObject
		where TResultsetModelObject : class, IResultsetModelObject<TResultProcedureModelObject>
		where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
	{
		#region Properties/Indexers/Events

		IEnumerable<TResultsetModelObject> Resultsets
		{
			get;
			set;
		}

		#endregion
	}
}