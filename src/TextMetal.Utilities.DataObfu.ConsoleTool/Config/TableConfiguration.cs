/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using TextMetal.Common.Core;

namespace TextMetal.Utilities.DataObfu.ConsoleTool.Config
{
	public class TableConfiguration : ConfigurationObject
	{
		#region Constructors/Destructors

		public TableConfiguration()
		{
			this.columns = new ConfigurationCollection<ColumnConfiguration>(this);

			this.SetDefaults();
		}

		#endregion

		#region Fields/Constants

		private readonly ConfigurationCollection<ColumnConfiguration> columns;
		private string destinationTableName;
		private string sourceCommandText;

		#endregion

		#region Properties/Indexers/Events

		public ConfigurationCollection<ColumnConfiguration> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public string DestinationTableName
		{
			get
			{
				return this.destinationTableName;
			}
			set
			{
				this.destinationTableName = value;
			}
		}

		[JsonIgnore]
		public new LoaderConfiguration Parent
		{
			get
			{
				return (LoaderConfiguration)base.Parent;
			}
			set
			{
				base.Parent = value;
			}
		}

		public string SourceCommandText
		{
			get
			{
				return this.sourceCommandText;
			}
			set
			{
				this.sourceCommandText = value;
			}
		}

		#endregion

		#region Methods/Operators

		private void SetDefaults()
		{
		}

		public override IEnumerable<Message> Validate()
		{
			return this.Validate(null);
		}

		public IEnumerable<Message> Validate(int? tableIndex)
		{
			List<Message> messages;
			int index;

			messages = new List<Message>();

			// check for duplicate columns
			var columnNameSums = this.Columns.GroupBy(c => c.ColumnName)
				.Select(cl => new
							{
								ColumnName = cl.First().ColumnName,
								Count = cl.Count()
							}).Where(cl => cl.Count > 1);

			if (columnNameSums.Any())
				messages.AddRange(columnNameSums.Select(c => NewError(string.Format("Table[{0}] with duplicate column configuration found: '{1}'.", tableIndex, c.ColumnName))).ToArray());

			index = 0;
			foreach (ColumnConfiguration columnConfiguration in this.Columns)
				messages.AddRange(columnConfiguration.Validate(index++));

			return messages;
		}

		#endregion
	}
}