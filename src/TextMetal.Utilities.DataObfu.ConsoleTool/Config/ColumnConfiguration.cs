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
	public class ColumnConfiguration : ConfigurationObject
	{
		#region Constructors/Destructors

		public ColumnConfiguration()
		{
			this.SetDefaults();
		}

		#endregion

		#region Fields/Constants

		private string columnName;
		private string dictionaryRef;
		private double? maskFactor;
		private ObfuscationStrategy obfuscationStrategy;

		#endregion

		#region Properties/Indexers/Events

		public string ColumnName
		{
			get
			{
				return this.columnName;
			}
			set
			{
				this.columnName = value;
			}
		}

		public string DictionaryRef
		{
			get
			{
				return this.dictionaryRef;
			}
			set
			{
				this.dictionaryRef = value;
			}
		}

		public double? MaskFactor
		{
			get
			{
				return this.maskFactor;
			}
			set
			{
				this.maskFactor = value;
			}
		}

		public ObfuscationStrategy ObfuscationStrategy
		{
			get
			{
				return this.obfuscationStrategy;
			}
			set
			{
				this.obfuscationStrategy = value;
			}
		}

		[JsonIgnore]
		public new TableConfiguration Parent
		{
			get
			{
				return (TableConfiguration)base.Parent;
			}
			set
			{
				base.Parent = value;
			}
		}

		#endregion

		#region Methods/Operators

		private void SetDefaults()
		{
			this.MaskFactor = (new Random().Next(0, 100)) / 100.0;
		}

		public override IEnumerable<Message> Validate()
		{
			return this.Validate(null);
		}

		public IEnumerable<Message> Validate(int? columnIndex)
		{
			List<Message> messages;

			messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(this.ColumnName))
				messages.Add(NewError(string.Format("Column[{0}] name is required.", columnIndex)));

			if (!Enum.IsDefined(typeof(ObfuscationStrategy), this.ObfuscationStrategy))
				messages.Add(NewError(string.Format("Column[{0}] obfuscation strategy is invalid.", columnIndex)));

			if (this.ObfuscationStrategy != ObfuscationStrategy.None)
			{
				if (this.ObfuscationStrategy == ObfuscationStrategy.Masking)
				{
					if ((object)this.MaskFactor == null)
						messages.Add(NewError(string.Format("Column[{0}] mask factor is required.", columnIndex)));
					else if (!((double)this.MaskFactor >= 0.0 && (double)this.MaskFactor <= 1.0))
						messages.Add(NewError(string.Format("Column[{0}] mask factor must be between 0.0 and 1.0.", columnIndex)));
				}

				if (this.ObfuscationStrategy == ObfuscationStrategy.Substitution)
				{
					if (DataType.IsNullOrWhiteSpace(this.DictionaryRef))
						messages.Add(NewError(string.Format("Column[{0}] dictionary ref is required.", columnIndex)));
					else if (this.DictionaryRef.SafeToString().Trim().ToLower() != "" &&
							this.Parent.Parent.Dictionaries.Count(d => d.DictionaryId.SafeToString().Trim().ToLower() == this.DictionaryRef.SafeToString().Trim().ToLower()) != 1)
						messages.Add(NewError(string.Format("Column[{0}] dictionary ref lookup failed.", columnIndex)));
				}
			}

			return messages;
		}

		#endregion
	}
}