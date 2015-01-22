/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using TextMetal.Common.Core;

namespace TextMetal.Common.UnitTests.Core._
{
	[TestFixture]
	public class DelimitedTextReaderTests
	{
		#region Constructors/Destructors

		public DelimitedTextReaderTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldParseComplexExampleTest()
		{
			DelimitedTextReader delimitedTextReader;
			DelimitedTextSpec delimitedTextSpec;

			delimitedTextSpec = new DelimitedTextSpec();
			delimitedTextSpec.FirstRecordIsHeader = true;
			delimitedTextSpec.RecordDelimiter = "\r\n";
			delimitedTextSpec.FieldDelimiter = "\t";
			delimitedTextSpec.QuoteValue = "\"";

			using (FileStream fileStream = File.Open(@"d:\TEST_VACU_DATA.csv", FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (StreamReader streamReader = new StreamReader(fileStream))
				{
					delimitedTextReader = new DelimitedTextReader(streamReader, delimitedTextSpec);

					var records = delimitedTextReader.ReadRecords();

					Console.WriteLine(records.Count());

					Console.WriteLine(string.Join("|", delimitedTextSpec.HeaderNames));
				}
			}
		}

		#endregion
	}
}