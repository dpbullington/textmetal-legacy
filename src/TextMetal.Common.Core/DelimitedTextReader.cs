/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextMetal.Common.Core
{
	public sealed class DelimitedTextReader : TextReader
	{
		#region Constructors/Destructors

		public DelimitedTextReader(TextReader innerTextReader,
			bool firstRowIsHeader = true, string[] headerNames = null,
			string fieldDelimiter = ",", string rowDelimiter = "\r\n",
			string quoteValue = "\"", string commentValue = "#")
		{
			if ((object)innerTextReader == null)
				throw new ArgumentNullException("innerTextReader");

			if ((object)fieldDelimiter == null)
				throw new ArgumentNullException("fieldDelimiter");

			if ((object)rowDelimiter == null)
				throw new ArgumentNullException("rowDelimiter");

			if ((object)quoteValue == null)
				throw new ArgumentNullException("quoteValue");

			if ((object)commentValue == null)
				throw new ArgumentNullException("commentValue");

			this.innerTextReader = innerTextReader;
			this.firstRowIsHeader = firstRowIsHeader;
			this.headerNames = headerNames ?? new string[] { };
			this.fieldDelimiter = fieldDelimiter;
			this.rowDelimiter = rowDelimiter;
			this.quoteValue = quoteValue;
			this.commentValue = commentValue;
		}

		#endregion

		#region Fields/Constants

		private readonly string commentValue;
		private readonly string fieldDelimiter;
		private readonly bool firstRowIsHeader;
		private readonly string[] headerNames;
		private readonly TextReader innerTextReader;
		private readonly string quoteValue;
		private readonly string rowDelimiter;

		#endregion

		#region Properties/Indexers/Events

		public string CommentValue
		{
			get
			{
				return this.commentValue;
			}
		}

		public string FieldDelimiter
		{
			get
			{
				return this.fieldDelimiter;
			}
		}

		public bool FirstRowIsHeader
		{
			get
			{
				return this.firstRowIsHeader;
			}
		}

		public string[] HeaderNames
		{
			get
			{
				return this.headerNames;
			}
		}

		public TextReader InnerTextReader
		{
			get
			{
				return this.innerTextReader;
			}
		}

		public string QuoteValue
		{
			get
			{
				return this.quoteValue;
			}
		}

		public string RowDelimiter
		{
			get
			{
				return this.rowDelimiter;
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

		private StringBuilder ReadRowAsStringBuilderUsingDelimiter()
		{
			int count = 0, value;
			StringBuilder lineStringBuilder;

			lineStringBuilder = new StringBuilder();

			while (true)
			{
				value = this.InnerTextReader.Read();

				if (value == -1)
					break;
				else
					lineStringBuilder.Append((char)value);

				count++;

				// look-behind CHECK
				if (lineStringBuilder.Length > 0 &&
					this.RowDelimiter.Length > 0 &&
					lineStringBuilder.Length >= this.RowDelimiter.Length)
				{
					for (int i = lineStringBuilder.Length - this.RowDelimiter.Length; i < lineStringBuilder.Length; i++)
					{
						for (int j = 0; j < this.RowDelimiter.Length; j++)
						{
							if (lineStringBuilder[i] != this.RowDelimiter[j])
								continue; // look-behind NO MATCH...
						}

						break; // look-behind MATCHED: stop
					}
				}
			}

			return lineStringBuilder;
		}

		public IEnumerable<IDictionary<string, string>> ReadRowsUsingDelimiters()
		{
			int rowIndex = 0;
			StringBuilder lineStringBuilder;
			string line;
			string[] temp, headers = null;
			IDictionary<string, string> row;

			while ((object)(lineStringBuilder = this.ReadRowAsStringBuilderUsingDelimiter()) != null &&
				lineStringBuilder.Length > 0)
			{
				row = new Dictionary<string, string>();
				line = lineStringBuilder.ToString();

				if (DataType.Instance.IsWhiteSpace(this.FieldDelimiter))
					row.Add(string.Empty, line);
				else
				{
					temp = line.Split(this.FieldDelimiter.ToCharArray());

					if ((object)temp == null)
						continue;

					if (rowIndex == 0)
					{
						if (this.FirstRowIsHeader)
						{
							headers = temp;
							rowIndex++;
							continue;
						}

						headers = this.HeaderNames;
					}

					int fieldIndex = 0;
					string key;

					foreach (string value in temp)
					{
						if (this.FirstRowIsHeader && (object)headers != null && headers.Length > 0)
							key = string.Format("{0}", Name.GetConstantCase(headers[fieldIndex++]));
						else
							key = string.Format("Field{0:00000000}", fieldIndex++);

						row.Add(key, value);
					}
				}

				rowIndex++;

				yield return row;
			}
		}

		#endregion
	}
}