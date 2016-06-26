/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Textual.Primitives;

namespace TextMetal.Middleware.Textual.Delimited
{
	public class DelimitedTextSpec : IDelimitedTextSpec
	{
		#region Constructors/Destructors

		public DelimitedTextSpec()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<TextHeaderSpec> headerSpecs = new List<TextHeaderSpec>();
		private string fieldDelimiter;
		private bool firstRecordIsHeader;
		private string quoteValue;
		private string recordDelimiter;

		#endregion

		#region Properties/Indexers/Events

		public List<TextHeaderSpec> HeaderSpecs
		{
			get
			{
				return this.headerSpecs;
			}
		}

		public string FieldDelimiter
		{
			get
			{
				return this.fieldDelimiter;
			}
			set
			{
				this.fieldDelimiter = value;
			}
		}

		public bool FirstRecordIsHeader
		{
			get
			{
				return this.firstRecordIsHeader;
			}
			set
			{
				this.firstRecordIsHeader = value;
			}
		}

		public string QuoteValue
		{
			get
			{
				return this.quoteValue;
			}
			set
			{
				this.quoteValue = value;
			}
		}

		public string RecordDelimiter
		{
			get
			{
				return this.recordDelimiter;
			}
			set
			{
				this.recordDelimiter = value;
			}
		}

		#endregion

		#region Methods/Operators

		public void AssertValid()
		{
			List<string> strings;

			strings = new List<string>();

			if (!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrEmpty(this.RecordDelimiter))
				strings.Add(this.RecordDelimiter);

			if (!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrEmpty(this.FieldDelimiter))
				strings.Add(this.FieldDelimiter);

			if (!SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrEmpty(this.QuoteValue))
				strings.Add(this.QuoteValue);

			if (strings.GroupBy(s => s).Where(gs => gs.Count() > 1).Any())
				throw new InvalidOperationException(string.Format("Duplicate delimiter/value encountered."));
		}

		#endregion
	}
}