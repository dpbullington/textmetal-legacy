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

			this.ResetParserState();
		}

		#endregion

		#region Fields/Constants

		private readonly DelimitedTextSpec delimitedTextSpec;
		private readonly TextReader innerTextReader;
		private readonly _ParserState parserState = new _ParserState();

		#endregion

		#region Properties/Indexers/Events

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

		private _ParserState ParserState
		{
			get
			{
				return this.parserState;
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

		private bool ParserStateMachine()
		{
			string tempStringValue;
			bool matchedRecordDelimiter, matchedFieldDelimiter;

			// now determine what to do based on parser state
			matchedRecordDelimiter = !this.ParserState.isQuotedValue &&
									!DataType.Instance.IsNullOrEmpty(this.DelimitedTextSpec.RecordDelimiter) &&
									LookBehindFixup(this.ParserState.transientStringBuilder, this.DelimitedTextSpec.RecordDelimiter);

			if (!matchedRecordDelimiter)
			{
				matchedFieldDelimiter = !this.ParserState.isQuotedValue &&
										!DataType.Instance.IsNullOrEmpty(this.DelimitedTextSpec.FieldDelimiter) &&
										LookBehindFixup(this.ParserState.transientStringBuilder, this.DelimitedTextSpec.FieldDelimiter);
			}
			else
				matchedFieldDelimiter = false;

			if (matchedRecordDelimiter || matchedFieldDelimiter || this.ParserState.isEOF)
			{
				// RECORD_DELIMITER | FIELD_DELIMITER | EOF

				// get string and clear for exit
				tempStringValue = this.ParserState.transientStringBuilder.ToString();
				this.ParserState.transientStringBuilder.Clear();

				// common logic to store value of field in record
				if (this.ParserState.isHeaderRecord)
				{
					// stash header if FRIS enabled and zeroth record
					this.ParserState.record.Add(tempStringValue, this.ParserState.fieldIndex.ToString("0000"));
				}
				else
				{
					// check header array and field index validity
					if ((object)this.DelimitedTextSpec.HeaderNames == null ||
						this.ParserState.fieldIndex >= this.DelimitedTextSpec.HeaderNames.Length)
						throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: field index '{0}' exceeded known field indices '{1}' at character index '{2}'.", this.ParserState.fieldIndex, (object)this.DelimitedTextSpec.HeaderNames != null ? (this.DelimitedTextSpec.HeaderNames.Length - 1) : (int?)null, this.ParserState.characterIndex));

					// lookup header name (key) by index and commit value to record
					this.ParserState.record.Add(this.DelimitedTextSpec.HeaderNames[this.ParserState.fieldIndex], tempStringValue);
				}

				// handle blank records (we assume that an records with valid record delimiter is OK)
				if (DataType.Instance.IsNullOrEmpty(tempStringValue) && this.ParserState.record.Keys.Count == 1)
					this.ParserState.record = null;

				// now what to do?
				if (this.ParserState.isEOF)
					return true;
				else if (matchedRecordDelimiter)
				{
					// advance record index
					this.ParserState.recordIndex++;

					// reset field index
					this.ParserState.fieldIndex = 0;

					// reset value index
					this.ParserState.valueIndex = 0;

					return true;
				}
				else if (matchedFieldDelimiter)
				{
					// advance field index
					this.ParserState.fieldIndex++;

					// reset value index
					this.ParserState.valueIndex = 0;
				}
			}
			else if (!this.ParserState.isEOF &&
					!this.ParserState.isQuotedValue &&
					!DataType.Instance.IsNullOrEmpty(this.DelimitedTextSpec.QuoteValue) &&
					LookBehindFixup(this.ParserState.transientStringBuilder, this.DelimitedTextSpec.QuoteValue))
			{
				// BEGIN::QUOTE_VALUE
				this.ParserState.isQuotedValue = true;
			}
			//else if (!this.ParserState.isEOF &&
			//	this.ParserState.isQuotedValue &&
			//	!DataType.Instance.IsNullOrEmpty(this.DelimitedTextSpec.QuoteValue) &&
			//	LookBehindFixup(this.ParserState.transientStringBuilder, this.DelimitedTextSpec.QuoteValue) &&
			//	this.ParserState.peekNextCharacter.ToString() == this.DelimitedTextSpec.QuoteValue)
			//{
			//	// unescape::QUOTE_VALUE
			//	this.ParserState.transientStringBuilder.Append("'");
			//}
			else if (!this.ParserState.isEOF &&
					this.ParserState.isQuotedValue &&
					!DataType.Instance.IsNullOrEmpty(this.DelimitedTextSpec.QuoteValue) &&
					LookBehindFixup(this.ParserState.transientStringBuilder, this.DelimitedTextSpec.QuoteValue))
			{
				// END::QUOTE_VALUE
				this.ParserState.isQuotedValue = false;
			}
			else if (!this.ParserState.isEOF)
			{
				// {field_data}

				// advance content index
				this.ParserState.contentIndex++;

				// advance value index
				this.ParserState.valueIndex++;
			}
			else
			{
				// {unknown_parser_state_error}
				throw new InvalidOperationException(string.Format("Unknown parser state error at character index '{0}'.", this.ParserState.characterIndex));
			}

			return false;
		}

		public IEnumerable<string> ReadHeaderNames()
		{
			if (this.DelimitedTextSpec.FirstRecordIsHeader)
			{
				var y = this.ResumableParserMainLoop(true);

				y.SingleOrDefault(); // force a single enumeration - yield return is a brain fyck
			}

			return this.DelimitedTextSpec.HeaderNames;
		}

		public IEnumerable<IDictionary<string, object>> ReadRecords()
		{
			return this.ResumableParserMainLoop(false);
		}

		private void ResetParserState()
		{
			this.ParserState.record = new Dictionary<string, object>();
			this.ParserState.transientStringBuilder = new StringBuilder();
			this.ParserState.readCurrentCharacter = '\0';
			this.ParserState.peekNextCharacter = '\0';
			this.ParserState.characterIndex = 0;
			this.ParserState.contentIndex = 0;
			this.ParserState.recordIndex = 0;
			this.ParserState.fieldIndex = 0;
			this.ParserState.valueIndex = 0;
			this.ParserState.isQuotedValue = false;
			this.ParserState.isEOF = false;

			this.DelimitedTextSpec.AssertValid();
		}

		private IEnumerable<IDictionary<string, object>> ResumableParserMainLoop(bool once)
		{
			int __value;
			char ch;

			// main loop - character stream
			while (!this.ParserState.isEOF)
			{
				// read the next byte
				__value = this.InnerTextReader.Read();
				ch = (char)__value;

				// check for -1 (EOF)
				if (__value == -1)
				{
					this.ParserState.isEOF = true; // set terminal state

					// sanity check - should never end with an open quote value
					if (this.ParserState.isQuotedValue)
						throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: end of file encountered while reading open quoted value."));
				}
				else
				{
					// append character to temp buffer
					this.ParserState.readCurrentCharacter = ch;
					this.ParserState.transientStringBuilder.Append(ch);

					// advance character index
					this.ParserState.characterIndex++;
				}

				// eval on every loop
				this.ParserState.isHeaderRecord = this.ParserState.recordIndex == 0 && this.DelimitedTextSpec.FirstRecordIsHeader;

				// peek the next byte
				__value = this.InnerTextReader.Peek();
				ch = (char)__value;
				this.ParserState.peekNextCharacter = ch;

				if (this.ParserStateMachine())
				{
					if ((object)this.ParserState.record != null)
					{
						if (!this.ParserState.isHeaderRecord)
							yield return this.ParserState.record; // aint this some shhhhhhhh?
						else
						{
							// stash parsed header names into member
							this.DelimitedTextSpec.HeaderNames = this.ParserState.record.Keys.ToArray();
						}
					}

					this.ParserState.record = new Dictionary<string, object>();

					if (once) // state-based resumption of loop ;)
						break;
				}
			}
		}

		private static bool LookBehindFixup(StringBuilder targetStringBuilder, string targetValue)
		{
			if ((object)targetStringBuilder == null)
				throw new ArgumentNullException("targetStringBuilder");

			if ((object)targetValue == null)
				throw new ArgumentNullException("targetValue");

			if (DataType.Instance.IsNullOrEmpty(targetValue))
				throw new ArgumentOutOfRangeException("targetValue");

			// look-behind CHECK
			if (targetStringBuilder.Length > 0 &&
				targetValue.Length > 0 &&
				targetStringBuilder.Length >= targetValue.Length)
			{
				int sb_length;
				int rd_length;
				int matches;

				sb_length = targetStringBuilder.Length;
				rd_length = targetValue.Length;
				matches = 0;

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

			return false; // not enough buffer space to care
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private sealed class _ParserState
		{
			#region Fields/Constants

			public long characterIndex;
			public long contentIndex;
			public int fieldIndex;
			public bool isEOF;
			public bool isHeaderRecord;
			public bool isQuotedValue;
			public char peekNextCharacter;
			public char readCurrentCharacter;
			public IDictionary<string, object> record;
			public long recordIndex;
			public StringBuilder transientStringBuilder;
			public long valueIndex;

			#endregion
		}

		#endregion
	}
}