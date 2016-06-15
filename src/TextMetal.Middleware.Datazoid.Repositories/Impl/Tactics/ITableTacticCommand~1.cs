/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Datazoid.Models.Tabular;
using TextMetal.Middleware.Datazoid.Primitives;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Tactics
{
	public interface ITableTacticCommand<TTableModelObject> : ITacticCommand
		where TTableModelObject : class, ITableModelObject
	{
		#region Properties/Indexers/Events

		bool UseBatchScopeIdentificationSemantics
		{
			get;
		}

		Action<TTableModelObject, int, IRecord> RecordToTableModelMappingCallback
		{
			get;
			set;
		}

		#endregion
	}
}