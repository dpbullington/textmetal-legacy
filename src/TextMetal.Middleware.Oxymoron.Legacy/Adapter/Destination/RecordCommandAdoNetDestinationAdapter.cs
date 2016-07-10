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

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Destination
{
	public class RecordCommandAdoNetDestinationAdapter : AdoNetDestinationAdapter
	{
		#region Constructors/Destructors

		public RecordCommandAdoNetDestinationAdapter()
		{
		}

		#endregion

		#region Methods/Operators

		protected override void CorePublishImpl(TableConfiguration configuration, IUnitOfWork destinationUnitOfWork, DbDataReader sourceDataReader, out long rowsCopied)
		{
			IEnumerable<IResultset> resultsets;
			IEnumerable<DbParameter> dbParameters;
			long _rowsCopied = 0;

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

			while (sourceDataReader.Read())
			{
				dbParameters = this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.GetDbDataParameters(destinationUnitOfWork);

				dbParameters = dbParameters.Select(p =>
				{
					// prevent modified closure bug
					var _sourceDataReader = sourceDataReader;
					// lazy load
					p.Value = _sourceDataReader[p.SourceColumn];
					return p;
				});

				resultsets = destinationUnitOfWork.ExecuteResultsets(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandType ?? CommandType.Text,
					this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText,
					dbParameters);

				resultsets.ToArray();

				_rowsCopied++;
			}

			rowsCopied = _rowsCopied;

			Console.WriteLine("DESTINATION (update): rowsCopied={0}", rowsCopied);
		}

		#endregion
	}
}