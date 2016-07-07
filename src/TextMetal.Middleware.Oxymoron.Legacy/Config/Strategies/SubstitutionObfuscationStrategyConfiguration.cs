/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Config.Strategies
{
	public class SubstitutionObfuscationStrategyConfiguration : ObfuscationStrategyConfiguration
	{
		#region Constructors/Destructors

		public SubstitutionObfuscationStrategyConfiguration()
		{
		}

		#endregion

		#region Fields/Constants

		private string dictionaryReference;

		#endregion

		#region Properties/Indexers/Events

		public string DictionaryReference
		{
			get
			{
				return this.dictionaryReference;
			}
			set
			{
				this.dictionaryReference = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override IEnumerable<Message> Validate(int? columnIndex)
		{
			List<Message> messages;

			messages = new List<Message>();

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.DictionaryReference))
				messages.Add(NewError(string.Format("Column[{0}/{1}] dictionary reference is required.", columnIndex, this.Parent.ColumnName)));

			return messages;
		}

		#endregion
	}
}