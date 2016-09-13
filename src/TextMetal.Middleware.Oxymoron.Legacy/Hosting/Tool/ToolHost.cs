/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TextMetal.Middleware.Oxymoron.Legacy.Adapter.Destination;
using TextMetal.Middleware.Oxymoron.Legacy.Adapter.Dictionary;
using TextMetal.Middleware.Oxymoron.Legacy.Adapter.Source;
using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Hosting.Tool
{
	public sealed class ToolHost : OxymoronHost, IToolHost
	{
		#region Constructors/Destructors

		public ToolHost()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary<DictionaryConfiguration, IDictionaryAdapter> dictionaryConfigurationToAdapterMappings = new Dictionary<DictionaryConfiguration, IDictionaryAdapter>();

		#endregion

		#region Properties/Indexers/Events

		private IDictionary<DictionaryConfiguration, IDictionaryAdapter> DictionaryConfigurationToAdapterMappings
		{
			get
			{
				return this.dictionaryConfigurationToAdapterMappings;
			}
		}

		#endregion

		#region Methods/Operators

		private static IEnumerable<IRecord> __WrapRecordCounter(string source, IEnumerable<IRecord> records, Action<string, long, bool, double> recordProcessCallback)
		{
			long recordCount = 0;
			DateTime startUtc;

			startUtc = DateTime.UtcNow;

			if ((object)records == null)
				throw new ArgumentNullException(nameof(records));

			foreach (IRecord record in records)
			{
				recordCount++;

				if ((recordCount % 1000) == 0)
				{
					//Thread.Sleep(250);
					if ((object)recordProcessCallback != null)
						recordProcessCallback(source, recordCount, false, (DateTime.UtcNow - startUtc).TotalSeconds);
				}

				yield return record;
			}

			if ((object)recordProcessCallback != null)
				recordProcessCallback(source, recordCount, true, (DateTime.UtcNow - startUtc).TotalSeconds);
		}

		private static IEnumerable<IRecord> DictionaryWrapRecordCounter(IEnumerable<IRecord> records, Action<string, long, bool, double> recordProcessCallback)
		{
			return __WrapRecordCounter("dictionary", records, recordProcessCallback);
		}

		private static IEnumerable<IRecord> SourceWrapRecordCounter(IEnumerable<IRecord> records, Action<string, long, bool, double> recordProcessCallback)
		{
			return __WrapRecordCounter("source", records, recordProcessCallback);
		}

		protected override object CoreGetValueForIdViaDictionaryResolution(DictionaryConfiguration dictionaryConfiguration, IColumn column, object surrogateId)
		{
			return this.DictionaryConfigurationToAdapterMappings[dictionaryConfiguration].GetAlternativeValueFromId(dictionaryConfiguration, column, surrogateId);
		}

		public void Host(ObfuscationConfiguration obfuscationConfiguration, Action<string, long, bool, double> statusCallback)
		{
			IEnumerable<IRecord> sourceDataEnumerable;
			IEnumerable<Message> messages;

			if ((object)obfuscationConfiguration == null)
				throw new ArgumentNullException(nameof(obfuscationConfiguration));

			messages = obfuscationConfiguration.Validate();

			if (messages.Any())
				throw new InvalidOperationException(string.Format("Obfuscation configuration validation failed:\r\n{0}", string.Join("\r\n", messages.Select(m => m.Description).ToArray())));

			using (IOxymoronEngine oxymoronEngine = new OxymoronEngine(this, obfuscationConfiguration))
			{
				using (DisposableList<IDictionaryAdapter> dictionaryAdapters = new DisposableList<IDictionaryAdapter>())
				{
					foreach (DictionaryConfiguration dictionaryConfiguration in obfuscationConfiguration.DictionaryConfigurations)
					{
						IDictionaryAdapter dictionaryAdapter;

						dictionaryAdapter = dictionaryConfiguration.DictionaryAdapterConfiguration.GetAdapterInstance<IDictionaryAdapter>();
						dictionaryAdapters.Add(dictionaryAdapter);
						dictionaryAdapter.Initialize(dictionaryConfiguration.DictionaryAdapterConfiguration);

						dictionaryAdapter.InitializePreloadCache((dictionaryDataEnumerable) => DictionaryWrapRecordCounter(dictionaryDataEnumerable, statusCallback), dictionaryConfiguration, oxymoronEngine.SubstitutionCacheRoot);

						this.DictionaryConfigurationToAdapterMappings.Add(dictionaryConfiguration, dictionaryAdapter);
					}

					using (ISourceAdapter sourceAdapter = obfuscationConfiguration.SourceAdapterConfiguration.GetAdapterInstance<ISourceAdapter>())
					{
						sourceAdapter.Initialize(obfuscationConfiguration.SourceAdapterConfiguration);

						using (IDestinationAdapter destinationAdapter = obfuscationConfiguration.DestinationAdapterConfiguration.GetAdapterInstance<IDestinationAdapter>())
						{
							destinationAdapter.UpstreamMetadata = sourceAdapter.UpstreamMetadata;
							destinationAdapter.Initialize(obfuscationConfiguration.DestinationAdapterConfiguration);

							sourceDataEnumerable = sourceAdapter.PullData(obfuscationConfiguration.TableConfiguration);
							sourceDataEnumerable = oxymoronEngine.GetObfuscatedValues(sourceDataEnumerable);

							if ((object)statusCallback != null)
								sourceDataEnumerable = SourceWrapRecordCounter(sourceDataEnumerable, statusCallback);

							destinationAdapter.PushData(obfuscationConfiguration.TableConfiguration, sourceDataEnumerable);
						}
					}
				}
			}
		}

		public void Host(string sourceFilePath)
		{
			ObfuscationConfiguration obfuscationConfiguration;

			sourceFilePath = Path.GetFullPath(sourceFilePath);
			obfuscationConfiguration = OxymoronEngine.FromJsonFile<ObfuscationConfiguration>(sourceFilePath);

			this.Host(obfuscationConfiguration, (w, x, y, z) => Console.WriteLine("{0}: {1} {2} {3}", w, x, y, z));
		}

		public bool TryGetUpstreamMetadata(ObfuscationConfiguration obfuscationConfiguration, out IEnumerable<IColumn> columns)
		{
			if ((object)obfuscationConfiguration == null)
				throw new ArgumentNullException(nameof(obfuscationConfiguration));

			columns = null;

			if ((object)obfuscationConfiguration.SourceAdapterConfiguration != null &&
				(object)obfuscationConfiguration.SourceAdapterConfiguration.GetAdapterType() != null)
			{
				using (ISourceAdapter sourceAdapter = obfuscationConfiguration.SourceAdapterConfiguration.GetAdapterInstance<ISourceAdapter>())
				{
					sourceAdapter.Initialize(obfuscationConfiguration.SourceAdapterConfiguration);
					columns = sourceAdapter.UpstreamMetadata;
					return true;
				}
			}

			return false;
		}

		#endregion
	}
}