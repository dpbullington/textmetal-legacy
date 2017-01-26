/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;
using TextMetal.Middleware.Textual.Delimited;

namespace TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters
{
	public class DelimitedTextAdapterConfiguration : AdapterSpecificConfiguration
	{
		#region Constructors/Destructors

		public DelimitedTextAdapterConfiguration()
		{
		}

		#endregion

		#region Fields/Constants

		private string delimitedTextFilePath;
		private DelimitedTextSpec delimitedTextSpec;

		#endregion

		#region Properties/Indexers/Events

		public string DelimitedTextFilePath
		{
			get
			{
				return this.delimitedTextFilePath;
			}
			set
			{
				this.delimitedTextFilePath = value;
			}
		}

		public DelimitedTextSpec DelimitedTextSpec
		{
			get
			{
				return this.delimitedTextSpec;
			}
			set
			{
				this.delimitedTextSpec = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override IEnumerable<Message> Validate(string adapterContext)
		{
			List<Message> messages;

			messages = new List<Message>();

			if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(this.DelimitedTextFilePath))
				messages.Add(NewError(string.Format("{0} adapter delimited text file path is required.", adapterContext)));

			if ((object)this.DelimitedTextSpec == null)
				messages.Add(NewError(string.Format("{0} adapter delimited text specification is required.", adapterContext)));
			else
			{
				//if (SolderFascadeAccessor.DataTypeFascade.IsNullOrEmpty(this.DelimitedTextSpec.QuoteValue))
				//	messages.Add(NewError(string.Format("{0} adapter delimited text quote value is required.", adapterContext)));

				if (SolderFascadeAccessor.DataTypeFascade.IsNullOrEmpty(this.DelimitedTextSpec.RecordDelimiter))
					messages.Add(NewError(string.Format("{0} adapter delimited text record delimiter is required.", adapterContext)));

				if (SolderFascadeAccessor.DataTypeFascade.IsNullOrEmpty(this.DelimitedTextSpec.FieldDelimiter))
					messages.Add(NewError(string.Format("{0} adapter delimited text field delimiter is required.", adapterContext)));
			}

			return messages;
		}

		#endregion
	}
}