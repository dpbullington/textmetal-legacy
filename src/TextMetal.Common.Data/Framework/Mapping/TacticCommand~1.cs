/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;

namespace TextMetal.Common.Data.Framework.Mapping
{
	public sealed class TacticCommand<TModel>
		where TModel : class, IModelObject
	{
		#region Constructors/Destructors

		public TacticCommand()
		{
		}

		#endregion

		#region Fields/Constants

		private CommandBehavior commandBehavior = CommandBehavior.Default;
		private IEnumerable<IDataParameter> commandParameters;
		private bool commandPrepare;
		private string commandText;
		private int? commandTimeout;
		private CommandType commandType;
		private int expectedRecordsAffected;
		private bool isNullipotent;
		private Action<TModel, IDictionary<string, object>> tableToModelMappingCallback;

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

		public IEnumerable<IDataParameter> CommandParameters
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

		public Action<TModel, IDictionary<string, object>> TableToModelMappingCallback
		{
			get
			{
				return this.tableToModelMappingCallback;
			}
			set
			{
				this.tableToModelMappingCallback = value;
			}
		}

		#endregion

		#region Methods/Operators

		public Type GetModelType()
		{
			return typeof(TModel);
		}

		#endregion
	}
}