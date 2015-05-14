/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;

using TextMetal.Middleware.Data.UoW;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Tactics
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
				throw new InvalidOperationException(string.Format("Enumeration is not re-entrant by design."));

			this.IsEnumerating = true;
		}

		public IEnumerable<IDbDataParameter> GetDbDataParameters(IUnitOfWork unitOfWork)
		{
			List<IDbDataParameter> dbDataParameters;
			IDbDataParameter dbDataParameter;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			dbDataParameters = new List<IDbDataParameter>();

			foreach (ITacticParameter tacticParameter in this.TacticParameters)
			{
				dbDataParameter = unitOfWork.CreateParameter(tacticParameter.ParameterDirection, tacticParameter.ParameterDbType, tacticParameter.ParameterSize, tacticParameter.ParameterPrecision, tacticParameter.ParameterScale, tacticParameter.ParameterNullable, tacticParameter.ParameterName, tacticParameter.ParameterValue);
				dbDataParameters.Add(dbDataParameter);
			}

			return dbDataParameters.ToArray();
		}

		public abstract Type[] GetModelTypes();

		public void LeaveEnumeration(bool @override)
		{
			if (!@override && !this.IsEnumerating)
				throw new InvalidOperationException(string.Format("Enumeration is not re-entrant by design."));

			this.IsEnumerating = false;
		}

		#endregion
	}
}