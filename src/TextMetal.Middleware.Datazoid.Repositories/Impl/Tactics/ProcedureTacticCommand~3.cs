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

		private Action<TReturnProcedureModelObject, IRecord> outputToReturnProcedureModelMappingCallback;
		private Action<TResultProcedureModelObject, int, IRecord> recordToResultModelMappingCallback;

		#endregion

		#region Properties/Indexers/Events

		public Action<TReturnProcedureModelObject, IRecord> OutputToReturnProcedureModelMappingCallback
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

		public Action<TResultProcedureModelObject, int, IRecord> RecordToResultModelMappingCallback
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