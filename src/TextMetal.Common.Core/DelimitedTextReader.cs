/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextMetal.Common.Core
{
	public sealed class DelimitedTextReader : TextReader
	{
		#region Constructors/Destructors

		public DelimitedTextReader(TextReader innerTextReader, DelimitedTextSpec delimitedTextSpec)
		{
			if ((object)innerTextReader == null)
				throw new ArgumentNullException("innerTextReader");

			if ((object)delimitedTextSpec == null)
				throw new ArgumentNullException("delimitedTextSpec");

			this.innerTextReader = innerTextReader;
			this.delimitedTextSpec = delimitedTextSpec;
		}

		#endregion

		#region Fields/Constants

		private string[] parsedHeaders;
		private readonly DelimitedTextSpec delimitedTextSpec;
		private readonly TextReader innerTextReader;

		#endregion

		#region Properties/Indexers/Events

		public string[] ParsedHeaders
		{
			get
			{
				return this.parsedHeaders;
			}
			private set
			{
				this.parsedHeaders = value;
			}
		}

		public DelimitedTextSpec DelimitedTextSpec
		{
			get
			{
				return this.delimitedTextSpec;
			}
		}

		public TextReader InnerTextReader
		{
			get
			{
				return this.innerTextReader;
			}
		}

		#endregion

		#region Methods/Operators

		public override void Close()
		{
			if ((object)this.InnerTextReader != null)
				this.InnerTextReader.Close();

			base.Close();
		}

		public IEnumerable<IDictionary<string, string>> ReadRecords()
		{
			IDictionary<string, string> record;
			string[] headers;

			int __value;
			char ch;

			long characterIndex;
			long recordIndex;
			int fieldIndex;

			bool isQuotedValue;
			bool isEOF;

			StringBuilder tempStringBuilder;
			string tempStringValue;

			tempStringBuilder = new StringBuilder();

			// init on BOF
			characterIndex = 0;
			recordIndex = 0;
			fieldIndex = 0;
			isQuotedValue = false;
			isEOF = false;

			record = new Dictionary<string, string>();
			headers = !this.DelimitedTextSpec.FirstRecordIsHeader ? this.DelimitedTextSpec.HeaderNames : null;

			while (!isEOF)
			{
				// read the next byte
				__value = this.InnerTextReader.Read();
				ch = (char)__value;

				// check for -1 (EOF)
				if (__value == -1)
				{
					isEOF = true; // set terminal state

					// sanity check - should never end with an open quote value
					if (isQuotedValue)
						throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: end of file encountered while reading open quoted value."));
				}
				else
				{
					// append character to temp buffer
					tempStringBuilder.Append(ch);

					// advance character index
					characterIndex++;
				}

				// now determine what to do based on parser state
				if (isEOF ||
					(!isQuotedValue && LookBehindFixup(tempStringBuilder, this.DelimitedTextSpec.RecordDelimiter)))
				{
					// RECORD_DELIMITER | EOF

					// get string and clear for exit
					tempStringValue = tempStringBuilder.ToString();
					tempStringBuilder.Clear();

					if (recordIndex == 0 &&
						this.DelimitedTextSpec.FirstRecordIsHeader)
					{
						// append last header
						record.Add(tempStringValue, fieldIndex.ToString("0000"));

						// finalize headers
						headers = record.Keys.ToArray();

						// stash header names into member
						this.ParsedHeaders = headers.ToArray();

						// prevent acidental usage of record
						record = null;
					}
					else
					{
						if ((this.DelimitedTextSpec.FieldDelimiter ?? string.Empty).Length == 0)
						{
							// we would never match a field delimiter, so we assume entire record is the anonymous value
							record.Add("TextFileLine" /*string.Empty*/, tempStringValue);
						}
						else
						{
							// check header array and field index validity
							if ((object)headers == null ||
								fieldIndex >= headers.Length)
								throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: field index '{0}' exceeded known field indices '{1}' at character index '{2}'.", fieldIndex, (object)headers != null ? (headers.Length - 1) : (int?)null, characterIndex));

							// lookup header name (key) by index and commit value to record
							record.Add(headers[fieldIndex], tempStringValue);
						}

						// commit record to collection
						yield return record;
					}

					if (!isEOF)
					{
						// create new record
						record = new Dictionary<string, string>();

						// advance record index
						recordIndex++;

						// reset field index
						fieldIndex = 0;
					}
				}
				else if (!isEOF && !isQuotedValue && LookBehindFixup(tempStringBuilder, this.DelimitedTextSpec.FieldDelimiter))
				{
					// FIELD_DELIMITER

					// get string and clear for next field
					tempStringValue = tempStringBuilder.ToString();
					tempStringBuilder.Clear();

					if (recordIndex == 0 &&
						this.DelimitedTextSpec.FirstRecordIsHeader)
					{
						// stash header if FRIS enabled and zeroth record
						record.Add(tempStringValue, fieldIndex.ToString("0000"));
					}
					else
					{
						// check header array and field index validity
						if ((object)headers == null ||
							fieldIndex >= headers.Length)
							throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: field index '{0}' exceeded known field indices '{1}' at character index '{2}'.", fieldIndex, (object)headers != null ? (headers.Length - 1) : (int?)null, characterIndex));

						// lookup header name (key) by index and commit value to record
						record.Add(headers[fieldIndex], tempStringValue);
					}

					// advance field index
					fieldIndex++;
				}
				else if (!isEOF && isQuotedValue && LookBehindFixup(tempStringBuilder, (this.DelimitedTextSpec.QuoteValue + this.DelimitedTextSpec.QuoteValue)))
				{
					// QUOTE_VALUE -> ESCAPED::QUOTE_VALUE

					// fixup escaped quote value embeded inside quoted field value
					tempStringBuilder.Append(this.DelimitedTextSpec.QuoteValue);
				}
				else if (!isEOF && !isQuotedValue && LookBehindFixup(tempStringBuilder, this.DelimitedTextSpec.QuoteValue))
				{
					// BEGIN::QUOTE_VALUE
					isQuotedValue = true;
				}
				else if (!isEOF && isQuotedValue && LookBehindFixup(tempStringBuilder, this.DelimitedTextSpec.QuoteValue))
				{
					// END::QUOTE_VALUE
					isQuotedValue = false;
				}
				else if (!isEOF)
				{
					// {field_data}
				}
				else
				{
					// {unknown_parser_state_error}
					throw new InvalidOperationException(string.Format("Unknown parser state error at character index '{0}'.", characterIndex));
				}
			}

			// term on EOF
			tempStringBuilder = null;
		}

		private static bool LookBehindFixup(StringBuilder targetStringBuilder, string targetValue)
		{
			if ((object)targetStringBuilder == null)
				throw new ArgumentNullException("targetStringBuilder");

			if ((object)targetValue == null)
				throw new ArgumentNullException("targetValue");

			// look-behind CHECK
			if (targetStringBuilder.Length > 0 &&
				targetValue.Length > 0 &&
				targetStringBuilder.Length >= targetValue.Length)
			{
				int sb_length = targetStringBuilder.Length;
				int rd_length = targetValue.Length;

				int matches = 0;

				for (int i = 0; i < rd_length; i++)
				{
					if (targetStringBuilder[sb_length - rd_length + i] != targetValue[i])
						return false; // look-behind NO MATCH...

					matches++;
				}

				if (matches != rd_length)
					throw new InvalidOperationException(string.Format("Something went sideways."));

				targetStringBuilder.Remove(sb_length - rd_length, rd_length);

				// look-behind MATCHED: stop
				return true;
			}

			return false; // not enought buffer space to care
		}

		#endregion
	}
}