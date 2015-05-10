/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Data.Models.Functional;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Tactics
{
	public sealed class ProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> : TacticCommand, IProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>
		where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
		where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
		where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>
	{
		#region Constructors/Destructors

		public ProcedureTacticCommand()
		{
		}

		#endregion

		#region Fields/Constants

		private Action<TReturnProcedureModelObject, IDictionary<string, object>> outputToReturnProcedureModelMappingCallback;
		private Action<TResultProcedureModelObject, IDictionary<string, object>> recordToResultModelMappingCallback;

		#endregion

		#region Properties/Indexers/Events

		public Action<TReturnProcedureModelObject, IDictionary<string, object>> OutputToReturnProcedureModelMappingCallback
		{
			get
			{
				return this.outputToReturnProcedureModelMappingCallback;
			}
			set
			{
				this.outputToReturnProcedureModelMappingCallback = value;
			}
		}

		public Action<TResultProcedureModelObject, IDictionary<string, object>> RecordToResultModelMappingCallback
		{
			get
			{
				return this.recordToResultModelMappingCallback;
			}
			set
			{
				this.recordToResultModelMappingCallback = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override Type[] GetModelTypes()
		{
			return new Type[]
					{
						typeof(TCallProcedureModelObject),
						typeof(TResultProcedureModelObject),
						typeof(TReturnProcedureModelObject)
					};
		}

		#endregion
	}
}