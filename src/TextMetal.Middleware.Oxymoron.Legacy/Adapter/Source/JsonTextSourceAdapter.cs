/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Source
{
	public class JsonTextSourceAdapter : SourceAdapter<JsonTextAdapterConfiguration>, IJsonTextAdapter
	{
		#region Constructors/Destructors

		public JsonTextSourceAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private JsonTextReader jsonTextReader;

		#endregion

		#region Properties/Indexers/Events

		private JsonTextReader JsonTextReader
		{
			get
			{
				return this.jsonTextReader;
			}
			set
			{
				this.jsonTextReader = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
			if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(this.AdapterConfiguration.AdapterSpecificConfiguration.JsonTextFilePath))
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "JsonTextFilePath"));

			this.JsonTextReader = new JsonTextReader(new StreamReader(File.Open(this.AdapterConfiguration.AdapterSpecificConfiguration.JsonTextFilePath, FileMode.Open, FileAccess.Read, FileShare.None))) { CloseInput = true };
		}

		protected override IEnumerable<IRecord> CorePullData(TableConfiguration tableConfiguration)
		{
			JsonSerializer serializer;
			//IEnumerable<IRecord> sourceDataEnumerable;

			if ((object)tableConfiguration == null)
				throw new ArgumentNullException(nameof(tableConfiguration));

			serializer = JsonSerializer.Create(new JsonSerializerSettings()
												{
													Formatting = Formatting.Indented,
													TypeNameHandling = TypeNameHandling.None,
													ReferenceLoopHandling = ReferenceLoopHandling.Ignore
												});

			this.JsonTextReader.SupportMultipleContent = true;

			IEnumerable<IRecord> objs = serializer.Deserialize<IEnumerable<Record>>(this.JsonTextReader);

			return objs;

			/*while (this.JsonTextReader.Read())
			{
				if(this.JsonTextReader.TokenType == JsonToken.StartArray)
				//return sourceDataEnumerable;
				yield return serializer.Deserialize<Record>(this.JsonTextReader);
			}*/
		}

		protected override void CoreTerminate()
		{
			if ((object)this.JsonTextReader != null)
				this.JsonTextReader.Close(); // why not simply Dispose()?

			this.JsonTextReader = null;
		}

		#endregion
	}
}