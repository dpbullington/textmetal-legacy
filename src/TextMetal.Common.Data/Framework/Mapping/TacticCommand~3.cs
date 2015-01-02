/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;

namespace TextMetal.Common.Data.Framework.Mapping
{
	public sealed class TacticCommand<TRequestModel, TResultModel, TResponseModel> : ITacticCommand
		where TRequestModel : class, IRequestModelObject
		where TResultModel : class, IResultModelObject
		where TResponseModel : class, IResponseModelObject<TResultModel>
	{
		#region Constructors/Destructors

		public TacticCommand()
		{
		}

		#endregion

		#region Fields/Constants

		private CommandBehavior commandBehavior = CommandBehavior.Default;
		private IEnumerable<IDbDataParameter> commandParameters;
		private bool commandPrepare;
		private string commandText;
		private int? commandTimeout;
		private CommandType commandType;
		private int expectedRecordsAffected;
		private bool isNullipotent;
		private Action<TResponseModel, IDictionary<string, object>> tableToResponseModelMappingCallback;
		private Action<TResultModel, IDictionary<string, object>> tableToResultModelMappingCallback;
		private bool useBatchScopeIdentificationSemantics;

		#endregion

		#region Properties/Indexers/Events

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

		public IEnumerable<IDbDataParameter> CommandParameters
		{
			get
			{
				return this.commandParameters;
			}
			set
			{
				this.commandParameters = value;
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

		public Action<TResponseModel, IDictionary<string, object>> TableToResponseModelMappingCallback
		{
			get
			{
				return this.tableToResponseModelMappingCallback;
			}
			set
			{
				this.tableToResponseModelMappingCallback = value;
			}
		}

		public Action<TResultModel, IDictionary<string, object>> TableToResultModelMappingCallback
		{
			get
			{
				return this.tableToResultModelMappingCallback;
			}
			set
			{
				this.tableToResultModelMappingCallback = value;
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

		public Type[] GetModelTypes()
		{
			return new Type[]
					{
						typeof(TRequestModel),
						typeof(TResultModel),
						typeof(TResponseModel)
					};
		}

		#endregion
	}
}