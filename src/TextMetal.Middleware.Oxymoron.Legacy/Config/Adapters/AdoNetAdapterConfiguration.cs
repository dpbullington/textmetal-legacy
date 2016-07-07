/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;

using TextMetal.Middleware.Datazoid.UoW;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters
{
	public class AdoNetAdapterConfiguration : AdapterSpecificConfiguration
	{
		#region Constructors/Destructors

		public AdoNetAdapterConfiguration()
		{
		}

		#endregion

		#region Fields/Constants

		private string connectionAqtn;
		private string connectionString;
		private string executeCommandText;
		private CommandType? executeCommandType;
		private string postExecuteCommandText;
		private CommandType? postExecuteCommandType;
		private string preExecuteCommandText;
		private CommandType? preExecuteCommandType;

		#endregion

		#region Properties/Indexers/Events

		public string ConnectionAqtn
		{
			get
			{
				return this.connectionAqtn;
			}
			set
			{
				this.connectionAqtn = value;
			}
		}

		public string ConnectionString
		{
			get
			{
				return this.connectionString;
			}
			set
			{
				this.connectionString = value;
			}
		}

		public string ExecuteCommandText
		{
			get
			{
				return this.executeCommandText;
			}
			set
			{
				this.executeCommandText = value;
			}
		}

		public CommandType? ExecuteCommandType
		{
			get
			{
				return this.executeCommandType;
			}
			set
			{
				this.executeCommandType = value;
			}
		}

		public string PostExecuteCommandText
		{
			get
			{
				return this.postExecuteCommandText;
			}
			set
			{
				this.postExecuteCommandText = value;
			}
		}

		public CommandType? PostExecuteCommandType
		{
			get
			{
				return this.postExecuteCommandType;
			}
			set
			{
				this.postExecuteCommandType = value;
			}
		}

		public string PreExecuteCommandText
		{
			get
			{
				return this.preExecuteCommandText;
			}
			set
			{
				this.preExecuteCommandText = value;
			}
		}

		public CommandType? PreExecuteCommandType
		{
			get
			{
				return this.preExecuteCommandType;
			}
			set
			{
				this.preExecuteCommandType = value;
			}
		}

		#endregion

		#region Methods/Operators

		public Type GetConnectionType()
		{
			Type connectionType;

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.ConnectionAqtn))
				return null;

			connectionType = Type.GetType(this.ConnectionAqtn, false);

			return connectionType;
		}

		public virtual IUnitOfWork GetUnitOfWork(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
		{
			Type dictionaryConnectionType;

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.ConnectionAqtn))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "AdoNetAdapterConfiguration.ConnectionAqtn"));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.ConnectionString))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "AdoNetAdapterConfiguration.ConnectionString"));

			dictionaryConnectionType = this.GetConnectionType();

			if ((object)dictionaryConnectionType == null)
				throw new InvalidOperationException(string.Format("Connection type failed to load: '{0}'.", this.ConnectionAqtn));

			return UnitOfWork.Create(dictionaryConnectionType, this.ConnectionString, false, isolationLevel);
		}

		public override IEnumerable<Message> Validate(string adapterContext)
		{
			List<Message> messages;

			messages = new List<Message>();

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.ConnectionAqtn))
				messages.Add(NewError(string.Format("{0} adapter ADO.NET connection AQTN is required.", adapterContext)));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.ConnectionString))
				messages.Add(NewError(string.Format("{0} adapter ADO.NET connection string is required.", adapterContext)));

			if ((object)this.ExecuteCommandType == null)
				messages.Add(NewError(string.Format("{0} adapter ADO.NET execute command type is required.", adapterContext)));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.ExecuteCommandText))
				messages.Add(NewError(string.Format("{0} adapter ADO.NET execute command text is required.", adapterContext)));

			return messages;
		}

		#endregion
	}
}