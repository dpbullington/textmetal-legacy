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

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Dictionary
{
	public class AdoNetDictionaryAdapter<TAdoNetAdapterConfiguration> : DictionaryAdapter<TAdoNetAdapterConfiguration>, IAdoNetAdapter
		where TAdoNetAdapterConfiguration : AdoNetAdapterConfiguration, new()
	{
		#region Constructors/Destructors

		public AdoNetDictionaryAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private IUnitOfWork dictionaryUnitOfWork;

		#endregion

		#region Properties/Indexers/Events

		private IUnitOfWork DictionaryUnitOfWork
		{
			get
			{
				return this.dictionaryUnitOfWork;
			}
			set
			{
				this.dictionaryUnitOfWork = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override object CoreGetAlternativeValueFromId(DictionaryConfiguration dictionaryConfiguration, IColumn metaColumn, object surrogateId)
		{
			AdoNetParameterConfiguration idAdoNetParameterConfiguration;
			object value;
			DbParameter dbDataParameterKey;
			IEnumerable<DbParameter> dbParameters;

			if ((object)dictionaryConfiguration == null)
				throw new ArgumentNullException(nameof(dictionaryConfiguration));

			if ((object)metaColumn == null)
				throw new ArgumentNullException(nameof(metaColumn));

			if ((object)surrogateId == null)
				throw new ArgumentNullException(nameof(surrogateId));
			
			if ((object)this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand)));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}.{1}'.", nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand), nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText)));

			idAdoNetParameterConfiguration = this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.AdoNetParameterConfigurations.SingleOrDefault();

			if((object)idAdoNetParameterConfiguration == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "ExecuteCommand.AdoNetParameterConfigurations.AdoNetParameterConfiguration[0]"));

			dbDataParameterKey = this.DictionaryUnitOfWork.CreateParameter(idAdoNetParameterConfiguration.ParameterDirection, idAdoNetParameterConfiguration.ParameterDbType, idAdoNetParameterConfiguration.ParameterSize, idAdoNetParameterConfiguration.ParameterPrecision, idAdoNetParameterConfiguration.ParameterScale, idAdoNetParameterConfiguration.ParameterNullable, idAdoNetParameterConfiguration.ParameterName, surrogateId);

			//dbParameters = this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.GetDbDataParameters(this.DictionaryUnitOfWork);
			//dbParameters = dbParameters.Append(dbDataParameterKey);
			dbParameters = new DbParameter[] { dbDataParameterKey };

			value = this.DictionaryUnitOfWork.ExecuteScalar<string>(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandType ?? CommandType.Text,
				this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText,
				dbParameters);

			return value;
		}

		protected override void CoreInitialize()
		{
			IEnumerable<IRecord> records;
			IEnumerable<DbParameter> dbParameters;

			this.DictionaryUnitOfWork = this.AdapterConfiguration.AdapterSpecificConfiguration.GetUnitOfWork();

			if (this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommand != null &&
				!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommand.CommandText))
			{
				dbParameters = this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommand.GetDbDataParameters(this.DictionaryUnitOfWork);

				records = this.DictionaryUnitOfWork.ExecuteRecords(this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommand.CommandType ?? CommandType.Text,
					this.AdapterConfiguration.AdapterSpecificConfiguration.PreExecuteCommand.CommandText,
					dbParameters, null);

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				records.ToArray();
			}
		}

		protected override void CorePreloadCache(Func<IEnumerable<IRecord>, IEnumerable<IRecord>> recordCallback, DictionaryConfiguration dictionaryConfiguration, IDictionary<string, IDictionary<long, object>> substitutionCacheRoot)
		{
			IEnumerable<IRecord> records;
			IDictionary<long, object> dictionaryCache;

			if ((object)recordCallback == null)
				throw new ArgumentNullException(nameof(recordCallback));

			if ((object)dictionaryConfiguration == null)
				throw new ArgumentNullException(nameof(dictionaryConfiguration));

			if ((object)substitutionCacheRoot == null)
				throw new ArgumentNullException(nameof(substitutionCacheRoot));

			if (dictionaryConfiguration.PreloadEnabled)
			{
				IEnumerable<DbParameter> dbParameters;

				if ((object)this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand == null)
					throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand)));

				if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText))
					throw new InvalidOperationException(string.Format("Configuration missing: '{0}.{1}'.", nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand), nameof(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText)));

				dbParameters = this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.GetDbDataParameters(this.DictionaryUnitOfWork);

				records = this.DictionaryUnitOfWork.ExecuteRecords(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandType ?? CommandType.Text,
					this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommand.CommandText,
					dbParameters, null);

				if ((object)records == null)
					throw new InvalidOperationException(string.Format("Records were invalid."));

				records = recordCallback(records);
				dictionaryCache = new Dictionary<long, object>();

				foreach (IRecord record in records)
				{
					object[] values = record.Values.ToArray();
					long id = values[0].ChangeType<long>();
					object value = values[1].ChangeType<string>();

					dictionaryCache.Add(id, value);
				}

				substitutionCacheRoot.Add(dictionaryConfiguration.DictionaryId, dictionaryCache);
			}
		}

		protected override void CoreTerminate()
		{
			IEnumerable<IResultset> resultsets;
			IEnumerable<DbParameter> dbParameters;

			if (this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommand != null &&
				!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommand.CommandText))
			{
				dbParameters = this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommand.GetDbDataParameters(this.DictionaryUnitOfWork);

				resultsets = this.DictionaryUnitOfWork.ExecuteSchemaResultsets(this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommand.CommandType ?? CommandType.Text,
					this.AdapterConfiguration.AdapterSpecificConfiguration.PostExecuteCommand.CommandText,
					dbParameters);

				if ((object)resultsets == null)
					throw new InvalidOperationException(string.Format("Resultsets were invalid."));

				resultsets.ToArray();
			}

			if ((object)this.DictionaryUnitOfWork != null)
				this.DictionaryUnitOfWork.Dispose();

			this.DictionaryUnitOfWork = null;
		}

		#endregion
	}
}