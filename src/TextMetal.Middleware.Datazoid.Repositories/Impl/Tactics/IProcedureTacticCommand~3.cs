/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Datazoid.Models.Functional;
using TextMetal.Middleware.Datazoid.Primitives;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Tactics
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