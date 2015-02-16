/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace TextMetal.Common.Core.InOut
{
	public class DelimitedTextSpec
	{
		#region Constructors/Destructors

		public DelimitedTextSpec()
		{
		}

		#endregion

		#region Fields/Constants

		private string fieldDelimiter;
		private bool firstRecordIsHeader;
		private string quoteValue;
		private string recordDelimiter;
		private readonly List<HeaderSpec> headerSpecs = new List<HeaderSpec>();

		#endregion

		#region Properties/Indexers/Events

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

		public List<HeaderSpec> HeaderSpecs
		{
			get
			{
				return this.headerSpecs;
			}
		}

		#endregion

		#region Methods/Operators

		public void AssertValid()
		{
			List<string> strings;

			strings = new List<string>();

			if (!DataType.Instance.IsNullOrEmpty(this.RecordDelimiter))
				strings.Add(this.RecordDelimiter);

			if (!DataType.Instance.IsNullOrEmpty(this.FieldDelimiter))
				strings.Add(this.FieldDelimiter);

			if (!DataType.Instance.IsNullOrEmpty(this.QuoteValue))
				strings.Add(this.QuoteValue);

			if (strings.GroupBy(s => s).Where(gs => gs.Count() > 1).Any())
				throw new InvalidOperationException(string.Format("Duplicate delimiter/value encountered."));
		}

		#endregion
	}
}