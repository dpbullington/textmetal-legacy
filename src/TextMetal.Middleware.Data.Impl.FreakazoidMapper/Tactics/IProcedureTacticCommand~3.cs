/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Data.Models;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Tactics
{
	public interface IProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> : ITacticCommand
		where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
		where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
		where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>
	{
		#region Properties/Indexers/Events

		Action<TReturnProcedureModelObject, IDictionary<string, object>> OutputToReturnProcedureModelMappingCallback
		{
			get;
			set;
		}

		Action<TResultProcedureModelObject, IDictionary<string, object>> RecordToResultModelMappingCallback
		{
			get;
			set;
		}

		#endregion
	}
}