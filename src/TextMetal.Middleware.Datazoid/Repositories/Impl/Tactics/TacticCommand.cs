/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using TextMetal.Middleware.Datazoid.UoW;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Tactics
{
	public abstract class TacticCommand : ITacticCommand
	{
		#region Constructors/Destructors

		protected TacticCommand()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly Guid instanceId = Guid.NewGuid();
		private CommandBehavior commandBehavior = CommandBehavior.Default;
		private bool commandPrepare;
		private string commandText;
		private int? commandTimeout;
		private CommandType commandType;
		private int expectedRecordsAffected;
		private bool isEnumerating;
		private bool isNullipotent;
		private IEnumerable<ITacticParameter> tacticParameters;

		#endregion

		#region Properties/Indexers/Events

		public Guid InstanceId
		{
			get
			{
				return this.instanceId;
			}
		}

		/// <summary>
		/// Gets or sets the command behavior.
		/// </summary>
		public CommandBehavior CommandBehavior
		{
			get
			{
				return this.commandBehavior;
			}
			set
			{
				this.commandBehavior = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to prepare the command.
		/// </summary>
		public bool CommandPrepare
		{
			get
			{
				return this.commandPrepare;
			}
			set
			{
				this.commandPrepare = value;
			}
		}

		/// <summary>
		/// Gets or sets the command text.
		/// </summary>
		public string CommandText
		{
			get
			{
				return this.commandText;
			}
			set
			{
				this.commandText = value;
			}
		}

		/// <summary>
		/// Gets or sets the command timeout.
		/// </summary>
		public int? CommandTimeout
		{
			get
			{
				return this.commandTimeout;
			}
			set
			{
				this.commandTimeout = value;
			}
		}

		/// <summary>
		/// Gets or sets the command type.
		/// </summary>
		public CommandType CommandType
		{
			get
			{
				return this.commandType;
			}
			set
			{
				this.commandType = value;
			}
		}

		public int ExpectedRecordsAffected
		{
			get
			{
				return this.expectedRecordsAffected;
			}
			set
			{
				this.expectedRecordsAffected = value;
			}
		}

		public bool IsEnumerating
		{
			get
			{
				return this.isEnumerating;
			}
			private set
			{
				this.isEnumerating = value;
			}
		}

		public bool IsNullipotent
		{
			get
			{
				return this.isNullipotent;
			}
			set
			{
				this.isNullipotent = value;
			}
		}

		public IEnumerable<ITacticParameter> TacticParameters
		{
			get
			{
				return this.tacticParameters;
			}
			set
			{
				this.tacticParameters = value;
			}
		}

		#endregion

		#region Methods/Operators

		public void EnterEnumeration(bool @override)
		{
			if (!@override && this.IsEnumerating)
				throw new InvalidOperationException(string.Format("Deferred execution enumeration is not re-entrant by design. This behavior can be disabled by setting the '<repository-clr-namespace>::DisableEnumerationReentrantCheck' appSetting to 'true'. This is not advised; however, and can lead to unexpected application behavior."));

			this.IsEnumerating = true;
		}

		public IEnumerable<DbParameter> GetDbParameters(IUnitOfWork unitOfWork)
		{
			List<DbParameter> dbParameters;
			DbParameter dbParameter;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			dbParameters = new List<DbParameter>();

			foreach (ITacticParameter tacticParameter in this.TacticParameters)
			{
				dbParameter = unitOfWork.CreateParameter(tacticParameter.ParameterDirection, tacticParameter.ParameterDbType, tacticParameter.ParameterSize, tacticParameter.ParameterPrecision, tacticParameter.ParameterScale, tacticParameter.ParameterNullable, tacticParameter.ParameterName, tacticParameter.ParameterValue);
				dbParameters.Add(dbParameter);
			}

			return dbParameters.ToArray();
		}

		public abstract Type[] GetModelTypes();

		public void LeaveEnumeration(bool @override)
		{
			if (!@override && !this.IsEnumerating)
				throw new InvalidOperationException(string.Format("Deferred execution enumeration is not re-entrant by design. This behavior can be disabled by setting the '<repository-clr-namespace>::DisableEnumerationReentrantCheck' appSetting to 'true'. This is not advised; however, and can lead to unexpected application behavior."));

			this.IsEnumerating = false;
		}

		#endregion
	}
}