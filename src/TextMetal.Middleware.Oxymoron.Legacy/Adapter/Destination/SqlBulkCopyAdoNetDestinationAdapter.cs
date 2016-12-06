/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data.Common;
using System.Data.SqlClient;

using TextMetal.Middleware.Datazoid.UoW;
using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Solder.Extensions;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Destination
{
	public class SqlBulkCopyAdoNetDestinationAdapter : AdoNetDestinationAdapter
	{
		#region Constructors/Destructors

		public SqlBulkCopyAdoNetDestinationAdapter()
		{
		}

		#endregion

		#region Methods/Operators

		protected override void CorePublishImpl(TableConfiguration configuration, IUnitOfWork destinationUnitOfWork, DbDataReader sourceDataReader, out long rowsCopied)
		{
			long _rowsCopied = 0;
			//SqlRowsCopiedEventHandler callback;

			if ((object)configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			if ((object)destinationUnitOfWork == null)
				throw new ArgumentNullException(nameof(destinationUnitOfWork));

			if ((object)sourceDataReader == null)
				throw new ArgumentNullException(nameof(sourceDataReader));

			if ((object)this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand)));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}.{1}'.", nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand), nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText)));

			using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy((SqlConnection)destinationUnitOfWork.Connection, SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls, (SqlTransaction)destinationUnitOfWork.Transaction))
			{
				//callback = (sender, e) => Console.WriteLine(_rowsCopied = e.RowsCopied);

				foreach (ColumnConfiguration columnConfiguration in configuration.ColumnConfigurations)
					sqlBulkCopy.ColumnMappings.Add(columnConfiguration.ColumnName, columnConfiguration.ColumnName);

				sqlBulkCopy.EnableStreaming = true;
				sqlBulkCopy.BatchSize = 2500;
				sqlBulkCopy.NotifyAfter = 2500;
				//sqlBulkCopy.SqlRowsCopied += callback;
				sqlBulkCopy.DestinationTableName = this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText;

				sqlBulkCopy.WriteToServer(sourceDataReader);

				//sqlBulkCopy.SqlRowsCopied -= callback;
			}

			rowsCopied = _rowsCopied;
		}

		#endregion
	}
}