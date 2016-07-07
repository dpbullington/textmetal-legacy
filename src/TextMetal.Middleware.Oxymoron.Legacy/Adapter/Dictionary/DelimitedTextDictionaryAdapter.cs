/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

// BACKLOG: add new config props 'KeyFieldName' and 'ValueFieldName'

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;
using TextMetal.Middleware.Textual.Delimited;
using TextMetal.Middleware.Textual.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Dictionary
{
	public class DelimitedTextDictionaryAdapter : DictionaryAdapter<DelimitedTextAdapterConfiguration>, IDelimitedTextAdapter
	{
		#region Constructors/Destructors

		public DelimitedTextDictionaryAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private const int ID_INDEX = 0;
		private const int VALUE_INDEX = 1;

		#endregion

		#region Methods/Operators

		protected override object CoreGetAlternativeValueFromId(DictionaryConfiguration dictionaryConfiguration, IColumn metaColumn, object surrogateId)
		{
			object value;
			int _surrogateId;

			ITextHeaderSpec[] headerSpecs;
			IEnumerable<IRecord> records;

			if ((object)dictionaryConfiguration == null)
				throw new ArgumentNullException(nameof(dictionaryConfiguration));

			if ((object)metaColumn == null)
				throw new ArgumentNullException(nameof(metaColumn));

			if ((object)surrogateId == null)
				throw new ArgumentNullException(nameof(surrogateId));

			if ((object)this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "DelimitedTextSpec"));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextFilePath))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "DelimitedTextFilePath"));

			using (RecordTextReader delimitedTextReader = new DelimitedTextReader(new StreamReader(File.Open(this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextFilePath, FileMode.Open, FileAccess.Read, FileShare.None)), this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec))
			{
				_surrogateId = surrogateId.ChangeType<int>() - 1;

				headerSpecs = delimitedTextReader.ReadHeaderSpecs().ToArray();
				records = delimitedTextReader.ReadRecords();

				var record = records.SingleOrDefault(r =>
													{
														object[] values = r.Values.ToArray();
														long id = values[ID_INDEX].ChangeType<long>();
														object val = values[VALUE_INDEX].ChangeType<string>();

														return id == _surrogateId;
													});

				if ((object)record == null)
					value = null;
				else
					value = record.Values.ToArray()[VALUE_INDEX];
			}

			return value;
		}

		protected override void CoreInitialize()
		{
		}

		protected override void CorePreloadCache(Func<IEnumerable<IRecord>, IEnumerable<IRecord>> recordCallback, DictionaryConfiguration dictionaryConfiguration, IDictionary<string, IDictionary<long, object>> substitutionCacheRoot)
		{
			ITextHeaderSpec[] headerSpecs;
			IEnumerable<IRecord> records;
			IDictionary<long, object> dictionaryCache;

			if ((object)recordCallback == null)
				throw new ArgumentNullException(nameof(recordCallback));

			if ((object)dictionaryConfiguration == null)
				throw new ArgumentNullException(nameof(dictionaryConfiguration));

			if ((object)substitutionCacheRoot == null)
				throw new ArgumentNullException(nameof(substitutionCacheRoot));

			if (!dictionaryConfiguration.PreloadEnabled)
				throw new InvalidOperationException(string.Format("The dictionary adapter '{1}' requires the configuration property '{2}' to be set to '{0}'.", true, typeof(DelimitedTextDictionaryAdapter).FullName, nameof(DictionaryConfiguration.PreloadEnabled)));

			// force preload for this adapter
			{
				if ((object)this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec == null)
					throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "DelimitedTextSpec"));

				if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextFilePath))
					throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "DelimitedTextFilePath"));

				using (RecordTextReader delimitedTextReader = new DelimitedTextReader(new StreamReader(File.Open(this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextFilePath, FileMode.Open, FileAccess.Read, FileShare.None)), this.AdapterConfiguration.AdapterSpecificConfiguration.DelimitedTextSpec))
				{
					headerSpecs = delimitedTextReader.ReadHeaderSpecs().ToArray();
					records = delimitedTextReader.ReadRecords();

					if ((object)records == null)
						throw new InvalidOperationException(string.Format("Records were invalid."));

					records = recordCallback(records);
					dictionaryCache = new Dictionary<long, object>();

					foreach (IDictionary<string, object> record in records)
					{
						object[] values = record.Values.ToArray();
						long id = values[ID_INDEX].ChangeType<long>();
						object value = values[VALUE_INDEX].ChangeType<string>();

						dictionaryCache.Add(id, value);
					}

					substitutionCacheRoot.Add(dictionaryConfiguration.DictionaryId, dictionaryCache);
				}
			}
		}

		protected override void CoreTerminate()
		{
		}

		#endregion
	}
}