/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Data.Models.Tabular;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Tactics
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