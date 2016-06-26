/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using TextMetal.Middleware.Textual.Primitives;

namespace TextMetal.Middleware.Textual.Delimited
{
	public interface IDelimitedTextSpec
	{
		List<TextHeaderSpec> HeaderSpecs
		{
			get;
		}

		string FieldDelimiter
		{
			get;
			set;
		}

		bool FirstRecordIsHeader
		{
			get;
			set;
		}

		string QuoteValue
		{
			get;
			set;
		}

		string RecordDelimiter
		{
			get;
			set;
		}

		void AssertValid();
	}
}