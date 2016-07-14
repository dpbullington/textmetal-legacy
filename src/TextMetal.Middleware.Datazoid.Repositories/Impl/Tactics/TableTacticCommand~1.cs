/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Datazoid.Models.Tabular;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Tactics
{
	public sealed class TableTacticCommand<TTableModelObject> : TacticCommand, ITableTacticCommand<TTableModelObject>
		where TTableModelObject : class, ITableModelObject
	{
		#region Constructors/Destructors

		public TableTacticCommand()
		{
		}

		#endregion

		#region Fields/Constants

		private Action<TTableModelObject, int, IRecord> recordToTableModelMappingCallback;
		private bool useBatchScopeIdentificationSemantics;

		#endregion

		#region Properties/Indexers/Events

		public Action<TTableModelObject, int, IRecord> RecordToTableModelMappingCallback
		{
			get
			{
				return this.recordToTableModelMappingCallback;
			}
			set
			{
				this.recordToTableModelMappingCallback = value;
			}
		}

		public bool UseBatchScopeIdentificationSemantics
		{
			get
			{
				return this.useBatchScopeIdentificationSemantics;
			}
			set
			{
				this.useBatchScopeIdentificationSemantics = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override Type[] GetModelTypes()
		{
			return new Type[] { typeof(TTableModelObject) };
		}

		#endregion
	}
}