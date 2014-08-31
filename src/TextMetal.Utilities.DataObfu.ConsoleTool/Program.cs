/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.Data.SqlClient;

using TextMetal.Common.Data;
using TextMetal.Utilities.DataObfu.ConsoleTool.Config;

namespace TextMetal.Utilities.DataObfu.ConsoleTool
{
	internal class Program
	{
		#region Methods/Operators

		private static void ExecuteObfuscation(string[] args)
		{
			LoaderConfiguration loaderConfiguration;

			IDataReader dataReader;
			int recordsAffected;
			long rowsCopied = 0;
			SqlRowsCopiedEventHandler callback;

			loaderConfiguration = LoaderConfiguration.FromJsonFile("example.json");
			
			using (IUnitOfWork sourceUnitOfWork = UnitOfWork.Create(loaderConfiguration.GetSourceConnectionType(), loaderConfiguration.SourceConnectionString, false))
			{
				using (IUnitOfWork destinationUnitOfWork = UnitOfWork.Create(loaderConfiguration.GetDestinationConnectionType(), loaderConfiguration.DestinationConnectionString, false))
				{
					foreach (TableConfiguration tableConfiguration in loaderConfiguration.Tables)
					{
						recordsAffected = -1;
						rowsCopied = 0;

						sourceUnitOfWork.ExecuteDictionary(CommandType.Text, tableConfiguration.DestinationCommandText, new IDbDataParameter[] { }, out recordsAffected);
						Console.WriteLine("DESTINATION: recordsAffected={0}", recordsAffected);

						using (dataReader = new ObfuscationDataReader(AdoNetHelper.ExecuteReader(sourceUnitOfWork.Connection, sourceUnitOfWork.Transaction, CommandType.Text, tableConfiguration.SourceCommandText, new IDbDataParameter[] { }, CommandBehavior.Default, null, false), tableConfiguration))
						{
							using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy((SqlConnection)destinationUnitOfWork.Connection, SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls, (SqlTransaction)destinationUnitOfWork.Transaction))
							{
								callback = (sender, e) => Console.WriteLine(rowsCopied = e.RowsCopied);

								foreach (ColumnConfiguration columnConfiguration in tableConfiguration.Columns)
									sqlBulkCopy.ColumnMappings.Add(columnConfiguration.ColumnName, columnConfiguration.ColumnName);

								sqlBulkCopy.EnableStreaming = true;
								sqlBulkCopy.BatchSize = 2500;
								sqlBulkCopy.NotifyAfter = 2500;
								sqlBulkCopy.SqlRowsCopied += callback;
								sqlBulkCopy.DestinationTableName = tableConfiguration.DestinationTableName;

								sqlBulkCopy.WriteToServer(dataReader);

								sqlBulkCopy.SqlRowsCopied -= callback;
							}

							recordsAffected = dataReader.RecordsAffected;
						}

						Console.WriteLine("SOURCE: recordsAffected={0}; rowsCopied={1}", recordsAffected, rowsCopied);
					}
				}
			}
		}

		private static int Main(string[] args)
		{
			DateTime start, end;
			TimeSpan duration;

			start = DateTime.UtcNow;

			try
			{
				ExecuteObfuscation(args);
			}
				/*catch (Exception ex)
			{
				Console.WriteLine("******************** Unhandled Exception ********************");
				Console.WriteLine(Reflexion.GetErrors(ex, 0));
				Console.WriteLine("******************** Unhandled Exception ********************");

				return -1;
			}*/
			finally
			{
				end = DateTime.UtcNow;
				duration = end - start;
				Console.WriteLine("Operation duration: {0}", duration);
			}

			return 0;
		}

		#endregion
	}
}