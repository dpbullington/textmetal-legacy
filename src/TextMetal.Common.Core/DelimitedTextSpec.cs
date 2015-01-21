/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Common.Core
{
	public sealed class DelimitedTextSpec
	{
		#region Constructors/Destructors

		public DelimitedTextSpec()
		{
		}

		#endregion

		#region Fields/Constants

		private string fieldDelimiter = ",";
		private bool firstRecordIsHeader = true;
		private string[] headerNames = new string[] { };
		private string quoteValue = "\"";
		private string recordDelimiter = "\r\n";

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
				this.fieldDelimiter = value ?? string.Empty;
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

		public string[] HeaderNames
		{
			get
			{
				return this.headerNames;
			}
			set
			{
				this.headerNames = value ?? new string[] { };
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
				this.quoteValue = value ?? string.Empty;
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
				this.recordDelimiter = value ?? string.Empty;
			}
		}

		#endregion
	}
}