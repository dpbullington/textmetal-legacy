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
			long _rowsCopied = 0;

			if ((object)configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			if ((object)destinationUnitOfWork == null)
				throw new ArgumentNullException(nameof(destinationUnitOfWork));

			if ((object)sourceDataReader == null)
				throw new ArgumentNullException(nameof(sourceDataReader));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandText))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "ExecuteCommandText"));

			// ?
			{
				DbParameter commandParameter;
				IDictionary<string, DbParameter> commandParameters;

				commandParameters = new Dictionary<string, DbParameter>();

				while (sourceDataReader.Read())
				{
					commandParameters.Clear();

					foreach (ColumnConfiguration columnConfiguration in configuration.ColumnConfigurations)
					{
						commandParameter = destinationUnitOfWork.CreateParameter(ParameterDirection.Input, DbType.AnsiString, 0, 0, 0, true, string.Format("@{0}", columnConfiguration.ColumnName), sourceDataReader[columnConfiguration.ColumnName]);
						commandParameters.Add(columnConfiguration.ColumnName, commandParameter);
					}

					resultsets = destinationUnitOfWork.ExecuteResultsets(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandType ?? CommandType.Text, this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandText, commandParameters.Values.ToArray());
					resultsets.ToArray();

					_rowsCopied++;
				}
			}

			rowsCopied = _rowsCopied;

			Console.WriteLine("DESTINATION (update): rowsCopied={0}", rowsCopied);
		}

		#endregion
	}
}