/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Common.Cerealization;
using TextMetal.Common.Core;

namespace TextMetal.Utilities.DataObfu.ConsoleTool.Config
{
	public class LoaderConfiguration : ConfigurationObject
	{
		#region Constructors/Destructors

		public LoaderConfiguration()
		{
			this.tables = new ConfigurationCollection<TableConfiguration>(this);
			this.dictionaries = new ConfigurationCollection<DictionaryConfiguration>(this);

			this.SetDefaults();
		}

		#endregion

		#region Fields/Constants

		private readonly ConfigurationCollection<DictionaryConfiguration> dictionaries;
		private readonly HashConfiguration signHash = new HashConfiguration();
		private readonly ConfigurationCollection<TableConfiguration> tables;
		private readonly HashConfiguration valueHash = new HashConfiguration();
		private string destinationConnectionAqtn;
		private string destinationConnectionString;
		private string dictionaryConnectionAqtn;
		private string dictionaryConnectionString;
		private string sourceConnectionAqtn;
		private string sourceConnectionString;

		#endregion

		#region Properties/Indexers/Events

		public string DestinationConnectionAqtn
		{
			get
			{
				return this.destinationConnectionAqtn;
			}
			set
			{
				this.destinationConnectionAqtn = value;
			}
		}

		public string DestinationConnectionString
		{
			get
			{
				return this.destinationConnectionString;
			}
			set
			{
				this.destinationConnectionString = value;
			}
		}

		public ConfigurationCollection<DictionaryConfiguration> Dictionaries
		{
			get
			{
				return this.dictionaries;
			}
		}

		public string DictionaryConnectionAqtn
		{
			get
			{
				return this.dictionaryConnectionAqtn;
			}
			set
			{
				this.dictionaryConnectionAqtn = value;
			}
		}

		public string DictionaryConnectionString
		{
			get
			{
				return this.dictionaryConnectionString;
			}
			set
			{
				this.dictionaryConnectionString = value;
			}
		}

		public HashConfiguration SignHash
		{
			get
			{
				return this.signHash;
			}
		}

		public string SourceConnectionAqtn
		{
			get
			{
				return this.sourceConnectionAqtn;
			}
			set
			{
				this.sourceConnectionAqtn = value;
			}
		}

		public string SourceConnectionString
		{
			get
			{
				return this.sourceConnectionString;
			}
			set
			{
				this.sourceConnectionString = value;
			}
		}

		public ConfigurationCollection<TableConfiguration> Tables
		{
			get
			{
				return this.tables;
			}
		}

		public HashConfiguration ValueHash
		{
			get
			{
				return this.valueHash;
			}
		}

		#endregion

		#region Methods/Operators

		public static LoaderConfiguration FromJson(string jsonData)
		{
			LoaderConfiguration loaderConfiguration;

			if (DataType.IsNullOrWhiteSpace(jsonData))
				loaderConfiguration = null;
			else
				loaderConfiguration = new JsonSerializationStrategy().GetObjectFromString<LoaderConfiguration>(jsonData);

			return loaderConfiguration;
		}

		public static LoaderConfiguration FromJsonFile(string jsonFile)
		{
			LoaderConfiguration loaderConfiguration;

			loaderConfiguration = new JsonSerializationStrategy().GetObjectFromFile<LoaderConfiguration>(jsonFile);

			return loaderConfiguration;
		}

		public static string ToJson(LoaderConfiguration loaderConfiguration)
		{
			string jsonData;

			if ((object)loaderConfiguration == null)
				jsonData = null;
			else
				jsonData = new JsonSerializationStrategy().SetObjectToString<LoaderConfiguration>(loaderConfiguration);

			return jsonData;
		}

		public static void ToJsonFile(LoaderConfiguration loaderConfiguration, string jsonFile)
		{
			new JsonSerializationStrategy().SetObjectToFile<LoaderConfiguration>(jsonFile, loaderConfiguration);
		}

		public Type GetDestinationConnectionType()
		{
			Type connectionType;

			connectionType = Type.GetType(this.DestinationConnectionAqtn, false);

			return connectionType;
		}

		public Type GetDictionaryConnectionType()
		{
			Type connectionType;

			connectionType = Type.GetType(this.DictionaryConnectionAqtn, false);

			return connectionType;
		}

		public Type GetSourceConnectionType()
		{
			Type connectionType;

			connectionType = Type.GetType(this.SourceConnectionAqtn, false);

			return connectionType;
		}

		private void SetDefaults()
		{
			this.SignHash.Multiplier = 33;
			this.SignHash.Size = 999999;
			this.SignHash.Seed = 5381;

			this.ValueHash.Multiplier = 33;
			this.ValueHash.Size = 0;
			this.ValueHash.Seed = 5381;
		}

		public override IEnumerable<Message> Validate()
		{
			List<Message> messages;
			int index;

			messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(this.SourceConnectionAqtn))
				messages.Add(NewError(string.Format("Source connection AQTN is required.")));

			if (DataType.IsNullOrWhiteSpace(this.SourceConnectionString))
				messages.Add(NewError(string.Format("Source connection string is required.")));

			if (DataType.IsNullOrWhiteSpace(this.DestinationConnectionAqtn))
				messages.Add(NewError(string.Format("Destination connection AQTN is required.")));

			if (DataType.IsNullOrWhiteSpace(this.DestinationConnectionString))
				messages.Add(NewError(string.Format("Destination connection string is required.")));

			if (DataType.IsNullOrWhiteSpace(this.DictionaryConnectionAqtn))
				messages.Add(NewError(string.Format("Dictionary connection AQTN is required.")));

			if (DataType.IsNullOrWhiteSpace(this.DictionaryConnectionString))
				messages.Add(NewError(string.Format("Dictionary connection string is required.")));

			if ((object)this.SignHash == null)
				messages.Add(NewError("Sign hash is required."));
			else
				messages.AddRange(this.SignHash.Validate("Sign hash"));

			if ((object)this.ValueHash == null)
				messages.Add(NewError(string.Format("Value hash is required.")));
			else
				messages.AddRange(this.ValueHash.Validate(string.Format("Value hash")));

			// check for duplicate dictionaries
			var dictionaryIdSums = this.Dictionaries.GroupBy(d => d.DictionaryId)
				.Select(dl => new
							{
								DictionaryId = dl.First().DictionaryId,
								Count = dl.Count()
							}).Where(dl => dl.Count > 1);

			if (dictionaryIdSums.Any())
				messages.AddRange(dictionaryIdSums.Select(d => NewError(string.Format("Duplicate dictionary configuration found: '{0}'.", d.DictionaryId))).ToArray());

			index = 0;
			foreach (DictionaryConfiguration dictionaryConfiguration in this.Dictionaries)
				messages.AddRange(dictionaryConfiguration.Validate(index++));

			// check for duplicate tables
			var tableNameSums = this.Tables.GroupBy(t => t.DestinationTableName)
				.Select(cl => new
							{
								TableName = cl.First().DestinationTableName,
								Count = cl.Count()
							}).Where(cl => cl.Count > 1);

			if (tableNameSums.Any())
				messages.AddRange(tableNameSums.Select(t => NewError(string.Format("Duplicate table configuration found: '{0}'.", t.TableName))).ToArray());

			index = 0;
			foreach (TableConfiguration tableConfiguration in this.Tables)
				messages.AddRange(tableConfiguration.Validate(index++));

			return messages;
		}

		#endregion
	}
}