/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;
using TextMetal.Middleware.Textual.Primitives;

namespace TextMetal.Middleware.Textual.Delimited
{
	public class DelimitedTextWriter : RecordTextWriter
	{
		#region Constructors/Destructors

		public DelimitedTextWriter(TextWriter innerTextWriter, IDelimitedTextSpec delimitedTextSpec)
			: base(innerTextWriter)
		{
			if ((object)delimitedTextSpec == null)
				throw new ArgumentNullException("delimitedTextSpec");

			this.delimitedTextSpec = delimitedTextSpec;
		}

		#endregion

		#region Fields/Constants

		private readonly IDelimitedTextSpec delimitedTextSpec;

		#endregion

		#region Properties/Indexers/Events

		public IDelimitedTextSpec DelimitedTextSpec
		{
			get
			{
				return this.delimitedTextSpec;
			}
		}

		public override Encoding Encoding
		{
			get
			{
				return this.InnerTextWriter.Encoding;
			}
		}

		#endregion

		#region Methods/Operators

		private void WriteField(StringBuilder transientStringBuilder, bool first, object fieldValue)
		{
			if ((object)transientStringBuilder == null)
				throw new ArgumentNullException("transientStringBuilder");

			if (!first && !SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrEmpty(this.DelimitedTextSpec.FieldDelimiter))
				transientStringBuilder.Append(this.DelimitedTextSpec.FieldDelimiter);

			if (!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrEmpty(this.DelimitedTextSpec.QuoteValue))
				transientStringBuilder.Append(this.DelimitedTextSpec.QuoteValue);

			transientStringBuilder.Append(fieldValue);

			if (!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrEmpty(this.DelimitedTextSpec.QuoteValue))
				transientStringBuilder.Append(this.DelimitedTextSpec.QuoteValue);
		}

		public override void WriteRecords(IEnumerable<IRecord> records)
		{
			bool first;
			StringBuilder transientStringBuilder;
			string tempStringValue;

			if ((object)records == null)
				throw new ArgumentNullException("records");

			transientStringBuilder = new StringBuilder();

			if ((object)this.DelimitedTextSpec.HeaderSpecs != null &&
				this.DelimitedTextSpec.FirstRecordIsHeader)
			{
				first = true;
				foreach (TextHeaderSpec headerSpec in this.DelimitedTextSpec.HeaderSpecs)
				{
					this.WriteField(transientStringBuilder, first, headerSpec.HeaderName);

					if (first)
						first = false;
				}
			}

			if (!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrEmpty(this.DelimitedTextSpec.RecordDelimiter))
				transientStringBuilder.Append(this.DelimitedTextSpec.RecordDelimiter);

			tempStringValue = transientStringBuilder.ToString();
			transientStringBuilder.Clear();
			this.InnerTextWriter.Write(tempStringValue);

			foreach (IDictionary<string, object> record in records)
			{
				first = true;
				foreach (KeyValuePair<string, object> field in record)
				{
					this.WriteField(transientStringBuilder, first, field.Value);

					if (first)
						first = false;
				}

				if (!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrEmpty(this.DelimitedTextSpec.RecordDelimiter))
					transientStringBuilder.Append(this.DelimitedTextSpec.RecordDelimiter);

				tempStringValue = transientStringBuilder.ToString();
				transientStringBuilder.Clear();
				this.InnerTextWriter.Write(tempStringValue);
			}
		}

		#endregion
	}
}