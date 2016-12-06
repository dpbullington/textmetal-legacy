/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

using TextMetal.Middleware.Datazoid.Extensions;
using TextMetal.Middleware.Datazoid.UoW;
using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Source
{
	public class AdoNetSourceAdapter<TAdoNetAdapterConfiguration> : SourceAdapter<TAdoNetAdapterConfiguration>, IAdoNetAdapter
		where TAdoNetAdapterConfiguration : AdoNetAdapterConfiguration, new()
	{
		#region Constructors/Destructors

		public AdoNetSourceAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private IUnitOfWork sourceUnitOfWork;

		#endregion

		#region Properties/Indexers/Events

		private IUnitOfWork SourceUnitOfWork
		{
			get
			{
				return this.sourceUnitOfWork;
			}
			set
			{
				this.sourceUnitOfWork = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
			IEnumerable<IResultset> resultsets;
			IEnumerable<IRecord> records;
			List<Column> columns;
			IEnumerable<DbParameter> dbParameters;

			this.SourceUnitOfWork = this.AdapterConfiguration.AdapterSpecificConfiguration.GetUnitOfWork();

			if (this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommand != null &&
				!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommand.CommandText))
			{
				dbParameters = this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommand.GetDbDataParameters(this.SourceUnitOfWork);

				records = this.SourceUnitOfWork.ExecuteRecords(this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommand.CommandType ?? CommandType.Text,
					this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommand.CommandText,
					dbParameters, null);

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				records.ToArray();
			}

			if (this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand != null ||
				!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText))
			{
				dbParameters = this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.GetDbDataParameters(this.SourceUnitOfWork);

				resultsets = this.SourceUnitOfWork.ExecuteSchemaResultsets(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandType ?? CommandType.Text,
					this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText,
					dbParameters);

				if ((object)resultsets == null)
					throw new InvalidOperationException(string.Format("Resultsets were invalid."));

				columns = new List<Column>();

				foreach (IResultset resultset in resultsets)
				{
					records = resultset.Records;

					if ((object)records == null)
						throw new InvalidOperationException(string.Format("Records were invalid."));

					int i = 0;
					foreach (IRecord record in records)
					{
						DbColumn dbColumn = (DbColumn)record.Context;

						columns.Add(new Column()
									{
										TableIndex = resultset.Index,
										ColumnIndex = i++,
										ColumnName = dbColumn.ColumnName,
										ColumnType = dbColumn.DataType,
										ColumnIsNullable = dbColumn.AllowDBNull,
										Context = record
									});
					}
				}

				this.UpstreamMetadata = columns;
			}
		}

		protected override IEnumerable<IRecord> CorePullData(TableConfiguration tableConfiguration)
		{
			AdapterConfiguration<AdoNetAdapterConfiguration> adapterConfiguration;
			IEnumerable<IRecord> sourceDataEnumerable;
			IEnumerable<DbParameter> dbParameters;

			if ((object)tableConfiguration == null)
				throw new ArgumentNullException(nameof(tableConfiguration));

			if ((object)this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand)));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}.{1}'.", nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand), nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText)));

			dbParameters = this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.GetDbDataParameters(this.SourceUnitOfWork);

			sourceDataEnumerable = this.SourceUnitOfWork.ExecuteRecords(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandType ?? CommandType.Text,
				this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText,
				dbParameters, null);

			if ((object)sourceDataEnumerable == null)
				throw new InvalidOperationException(string.Format("Records were invalid."));

			return sourceDataEnumerable;
		}

		protected override void CoreTerminate()
		{
			IEnumerable<IResultset> resultsets;
			IEnumerable<DbParameter> dbParameters;

			if (this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommand != null &&
				!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommand.CommandText))
			{
				dbParameters = this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommand.GetDbDataParameters(this.SourceUnitOfWork);

				resultsets = this.SourceUnitOfWork.ExecuteSchemaResultsets(this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommand.CommandType ?? CommandType.Text,
					this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommand.CommandText,
					dbParameters);

				if ((object)resultsets == null)
					throw new InvalidOperationException(string.Format("Resultsets were invalid."));

				resultsets.ToArray();
			}

			if ((object)this.SourceUnitOfWork != null)
				this.SourceUnitOfWork.Dispose();

			this.SourceUnitOfWork = null;
		}

		#endregion
	}
}