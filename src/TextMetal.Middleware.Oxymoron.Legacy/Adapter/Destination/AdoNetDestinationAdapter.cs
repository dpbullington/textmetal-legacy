/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

using TextMetal.Middleware.Datazoid.Extensions;
using TextMetal.Middleware.Datazoid.Primitives;
using TextMetal.Middleware.Datazoid.UoW;
using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Destination
{
	public abstract class AdoNetDestinationAdapter : DestinationAdapter<AdoNetAdapterConfiguration>, IAdoNetAdapter
	{
		#region Constructors/Destructors

		protected AdoNetDestinationAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private IUnitOfWork destinationUnitOfWork;

		#endregion

		#region Properties/Indexers/Events

		private IUnitOfWork DestinationUnitOfWork
		{
			get
			{
				return this.destinationUnitOfWork;
			}
			set
			{
				this.destinationUnitOfWork = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
			IEnumerable<IResultset> resultsets;

			this.DestinationUnitOfWork = this.AdapterConfiguration.AdapterSpecificConfiguration.GetUnitOfWork();

			if (!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommandText))
			{
				resultsets = this.DestinationUnitOfWork.ExecuteResultsets(this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommandType ?? CommandType.Text, this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommandText, new DbParameter[] { });

				if ((object)resultsets == null)
					throw new InvalidOperationException(string.Format("Resultsets were invalid."));

				resultsets.ToArray();
			}
		}

		protected abstract void CorePublishImpl(TableConfiguration configuration, IUnitOfWork destinationUnitOfWork, DbDataReader sourceDataReader, out long rowsCopied);

		protected override void CorePushData(TableConfiguration tableConfiguration, IEnumerable<IRecord> sourceDataEnumerable)
		{
			DbDataReader sourceDataReader;
			long rowsCopied;

			if ((object)tableConfiguration == null)
				throw new ArgumentNullException(nameof(tableConfiguration));

			if ((object)sourceDataEnumerable == null)
				throw new ArgumentNullException(nameof(sourceDataEnumerable));

			sourceDataReader = new RecordAdapterDbDataReader(this.UpstreamMetadata, sourceDataEnumerable);

			this.CorePublishImpl(tableConfiguration, this.DestinationUnitOfWork, sourceDataReader, out rowsCopied);
		}

		protected override void CoreTerminate()
		{
			IEnumerable<IResultset> resultsets;

			if (!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommandText))
			{
				resultsets = this.DestinationUnitOfWork.ExecuteResultsets(this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommandType ?? CommandType.Text, this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommandText, new DbParameter[] { });

				if ((object)resultsets == null)
					throw new InvalidOperationException(string.Format("Resultsets were invalid."));

				resultsets.ToArray();
			}

			if ((object)this.DestinationUnitOfWork != null)
				this.DestinationUnitOfWork.Dispose();

			this.DestinationUnitOfWork = null;
		}

		#endregion
	}
}