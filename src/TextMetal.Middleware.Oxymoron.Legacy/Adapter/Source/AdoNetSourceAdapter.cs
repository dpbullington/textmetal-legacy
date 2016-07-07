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
using TextMetal.Middleware.Datazoid.UoW;
using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Source
{
	public class AdoNetSourceAdapter : SourceAdapter<AdoNetAdapterConfiguration>, IAdoNetAdapter
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
			List<Column> metaColumns;

			this.SourceUnitOfWork = this.AdapterConfiguration.AdapterSpecificConfiguration.GetUnitOfWork();

			if (!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommandText))
			{
				resultsets = this.SourceUnitOfWork.ExecuteSchemaResultsets(this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommandType ?? CommandType.Text, this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommandText, new DbParameter[] { });

				if ((object)resultsets == null)
					throw new InvalidOperationException(string.Format("Resultsets were invalid."));

				resultsets.ToArray();
			}

			resultsets = this.SourceUnitOfWork.ExecuteSchemaResultsets(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandType ?? CommandType.Text, this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandText, new DbParameter[] { });

			if ((object)resultsets == null)
				throw new InvalidOperationException(string.Format("Resultsets were invalid."));

			metaColumns = new List<Column>();

			foreach (IResultset resultset in resultsets)
			{
				records = resultset.Records;

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				int i = 0;
				foreach (IRecord record in records)
				{
					DbColumn dbColumn = (DbColumn)record.TagContext;

					metaColumns.Add(new Column()
									{
										TableIndex = resultset.Index,
										ColumnIndex = i++,
										ColumnName = dbColumn.ColumnName,
										ColumnType = dbColumn.DataType,
										ColumnIsNullable = dbColumn.AllowDBNull,
										TagContext = record
									});
				}
			}

			this.UpstreamMetadata = metaColumns;
		}

		protected override IEnumerable<IRecord> CorePullData(TableConfiguration tableConfiguration)
		{
			AdapterConfiguration<AdoNetAdapterConfiguration> adapterConfiguration;
			IEnumerable<IRecord> sourceDataEnumerable;

			if ((object)tableConfiguration == null)
				throw new ArgumentNullException(nameof(tableConfiguration));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandText))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "ExecuteCommandText"));

			sourceDataEnumerable = this.SourceUnitOfWork.ExecuteRecords(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandType ?? CommandType.Text, this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandText, new DbParameter[] { }, null);

			if ((object)sourceDataEnumerable == null)
				throw new InvalidOperationException(string.Format("Records were invalid."));

			return sourceDataEnumerable;
		}

		protected override void CoreTerminate()
		{
			IEnumerable<IResultset> resultsets;

			if (!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommandText))
			{
				resultsets = this.SourceUnitOfWork.ExecuteSchemaResultsets(this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommandType ?? CommandType.Text, this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommandText, new DbParameter[] { });

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