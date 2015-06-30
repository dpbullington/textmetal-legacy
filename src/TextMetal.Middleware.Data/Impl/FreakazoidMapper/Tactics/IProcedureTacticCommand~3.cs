/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Data.Models.Functional;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Tactics
{
	public interface IProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> : ITacticCommand
		where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
		where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
		where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>
	{
		#region Properties/Indexers/Events

		Action<TReturnProcedureModelObject, IRecord> OutputToReturnProcedureModelMappingCallback
		{
			get;
			set;
		}

		Action<TResultProcedureModelObject, int, IRecord> RecordToResultModelMappingCallback
		{
			get;
			set;
		}

		#endregion
	}
}