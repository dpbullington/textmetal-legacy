/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using TextMetal.Common.Core;

namespace TextMetal.Utilities.DataObfu.ConsoleTool.Config
{
	public class DictionaryConfiguration : ConfigurationObject
	{
		#region Constructors/Destructors

		public DictionaryConfiguration()
		{
			this.SetDefaults();
		}

		#endregion

		#region Fields/Constants

		private string dictionaryCommandText;
		private string dictionaryId;
		private int? recordCount;

		#endregion

		#region Properties/Indexers/Events

		public string DictionaryCommandText
		{
			get
			{
				return this.dictionaryCommandText;
			}
			set
			{
				this.dictionaryCommandText = value;
			}
		}

		public string DictionaryId
		{
			get
			{
				return this.dictionaryId;
			}
			set
			{
				this.dictionaryId = value;
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

		public int? RecordCount
		{
			get
			{
				return this.recordCount;
			}
			set
			{
				this.recordCount = value;
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

		public IEnumerable<Message> Validate(int? index)
		{
			List<Message> messages;

			messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(this.DictionaryId))
				messages.Add(NewError(string.Format("Dictionary[{0}] ID is required.", index)));

			if (DataType.IsNullOrWhiteSpace(this.DictionaryCommandText))
				messages.Add(NewError(string.Format("Dictionary[{0}] command text is required.", index)));

			return messages;
		}

		#endregion
	}
}