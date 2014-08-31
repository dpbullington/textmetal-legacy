/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;

namespace TextMetal.Common.Data.Framework.Mapping
{
	public interface ITacticCommand
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the command behavior.
		/// </summary>
		CommandBehavior CommandBehavior
		{
			get;
		}

		IEnumerable<IDbDataParameter> CommandParameters
		{
			get;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to prepare the command.
		/// </summary>
		bool CommandPrepare
		{
			get;
		}

		/// <summary>
		/// Gets or sets the command text.
		/// </summary>
		string CommandText
		{
			get;
		}

		/// <summary>
		/// Gets or sets the command timeout.
		/// </summary>
		int? CommandTimeout
		{
			get;
		}

		/// <summary>
		/// Gets or sets the command type.
		/// </summary>
		CommandType CommandType
		{
			get;
		}

		int ExpectedRecordsAffected
		{
			get;
		}

		bool IsNullipotent
		{
			get;
		}

		bool UseBatchScopeIdentificationSemantics
		{
			get;
		}

		#endregion

		#region Methods/Operators

		Type[] GetModelTypes();

		#endregion
	}
}