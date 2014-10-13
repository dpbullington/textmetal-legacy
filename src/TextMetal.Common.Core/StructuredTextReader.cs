/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace TextMetal.Common.Core
{
	public sealed class StructuredTextReader : TextReader
	{
		#region Constructors/Destructors

		public StructuredTextReader(TextReader innerTextReader,
			bool firstRowIsHeader, string[] headerNames, string fieldDelimiter, string rowDelimiter)
		{
			if ((object)innerTextReader == null)
				throw new ArgumentNullException("innerTextReader");

			if ((object)headerNames == null)
				throw new ArgumentNullException("headerNames");

			if ((object)fieldDelimiter == null)
				throw new ArgumentNullException("fieldDelimiter");

			if ((object)rowDelimiter == null)
				throw new ArgumentNullException("rowDelimiter");

			this.innerTextReader = innerTextReader;
			this.firstRowIsHeader = firstRowIsHeader;
			this.headerNames = headerNames;
			this.fieldDelimiter = fieldDelimiter;
			this.rowDelimiter = rowDelimiter;
		}

		#endregion

		#region Fields/Constants

		private readonly string fieldDelimiter;
		private readonly bool firstRowIsHeader;
		private readonly string[] headerNames;
		private readonly TextReader innerTextReader;
		private readonly string rowDelimiter;

		#endregion

		#region Properties/Indexers/Events

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

		public string ReadRowUsingDelimiter()
		{
			string line;

			line = this.InnerTextReader.ReadLine(); // TODO: need to support the row delimitor (defaults to \r\n)

			return line;
		}

		public IEnumerable<IDictionary<string, string>> ReadRowsUsingDelimiters()
		{
			int rowIndex = 0;
			string line;
			string[] temp, headers = null;
			IDictionary<string, string> row;

			while (((line = this.ReadRowUsingDelimiter()) ?? string.Empty).Trim() != string.Empty)
			{
				row = new Dictionary<string, string>();

				if (DataType.IsWhiteSpace(this.FieldDelimiter))
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