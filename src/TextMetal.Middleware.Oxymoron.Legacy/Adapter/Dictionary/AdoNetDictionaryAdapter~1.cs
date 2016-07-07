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
			object value;
			DbParameter dbDataParameterKey;

			if ((object)dictionaryConfiguration == null)
				throw new ArgumentNullException(nameof(dictionaryConfiguration));

			if ((object)metaColumn == null)
				throw new ArgumentNullException(nameof(metaColumn));

			if ((object)surrogateId == null)
				throw new ArgumentNullException(nameof(surrogateId));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandText))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "ExecuteCommandText"));

			dbDataParameterKey = this.DictionaryUnitOfWork.CreateParameter(ParameterDirection.Input, DbType.Object, 0, 0, 0, false, "@ID", surrogateId);

			value = this.DictionaryUnitOfWork.ExecuteScalar<string>(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandType ?? CommandType.Text, this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandText, new DbParameter[] { dbDataParameterKey });

			return value;
		}

		protected override void CoreInitialize()
		{
			this.DictionaryUnitOfWork = this.AdapterConfiguration.AdapterSpecificConfiguration.GetUnitOfWork();
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
				if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandText))
					throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "ExecuteCommandText"));

				records = this.DictionaryUnitOfWork.ExecuteRecords(this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandType ?? CommandType.Text, this.AdapterConfiguration.AdapterSpecificConfiguration.ExecuteCommandText, new DbParameter[] { }, null);

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
			if ((object)this.DictionaryUnitOfWork != null)
				this.DictionaryUnitOfWork.Dispose();

			this.DictionaryUnitOfWork = null;
		}

		#endregion
	}
}