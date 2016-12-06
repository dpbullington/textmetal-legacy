/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using TextMetal.Middleware.Datazoid.UoW;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Tactics
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

		Guid InstanceId
		{
			get;
		}

		bool IsNullipotent
		{
			get;
		}

		IEnumerable<ITacticParameter> TacticParameters
		{
			get;
		}

		#endregion

		#region Methods/Operators

		void EnterEnumeration(bool @override);

		IEnumerable<DbParameter> GetDbParameters(IUnitOfWork unitOfWork);

		Type[] GetModelTypes();

		void LeaveEnumeration(bool @override);

		#endregion
	}
}