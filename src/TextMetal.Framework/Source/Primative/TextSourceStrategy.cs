/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TextMetal.Framework.Associative;
using TextMetal.Middleware.Solder.Extensions;

namespace TextMetal.Framework.Source.Primative
{
	public class TextSourceStrategy : SourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TextSourceStrategy class.
		/// </summary>
		public TextSourceStrategy()
		{
		}

		public void foo(string x)
		{
			throw new ArgumentNullException("x");
		}

		#endregion

		#region Methods/Operators

		protected override object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			const string PROP_TOKEN_FIRST_RECORD_CONTAINS_COLUMN_HEADINGS = "FirstRecordIsHeader";
			const string PROP_TOKEN_HEADER_NAMES = "HeaderName";
			const string PROP_TOKEN_FIELD_DELIMITER = "FieldDelimiter";
			const string PROP_TOKEN_RECORD_DELIMITER = "RecordDelimiter";
			const string PROP_TOKEN_QUOTE_VALUE = "QuoteValue";

			IList<string> values;
			bool firstRecordIsHeader;
			string recordDelimiter;
			string fieldDelimiter;
			string quoteValue;
			string[] headerNames;

			ObjectConstruct objectConstruct00;
			ArrayConstruct arrayConstruct00;
			ObjectConstruct objectConstruct01;
			PropertyConstruct propertyConstruct01;

			string line;
			string[] headers = null;

			ObjectConstruct tempOc;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException(nameof(sourceFilePath));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException(nameof(sourceFilePath));

			sourceFilePath = Path.GetFullPath(sourceFilePath);

			objectConstruct00 = new ObjectConstruct();

			firstRecordIsHeader = false;
			if (properties.TryGetValue(PROP_TOKEN_FIRST_RECORD_CONTAINS_COLUMN_HEADINGS, out values))
			{
				if ((object)values != null && values.Count == 1)
				{
					if (!SolderFascadeAccessor.DataTypeFascade.TryParse<bool>(values[0], out firstRecordIsHeader))
						firstRecordIsHeader = false;
				}
			}

			headerNames = null;
			if (properties.TryGetValue(PROP_TOKEN_HEADER_NAMES, out values))
			{
				if ((object)values != null)
					headerNames = values.ToArray();
			}

			fieldDelimiter = string.Empty;
			if (properties.TryGetValue(PROP_TOKEN_FIELD_DELIMITER, out values))
			{
				if ((object)values != null && values.Count == 1)
					fieldDelimiter = values[0];
			}

			recordDelimiter = string.Empty;
			if (properties.TryGetValue(PROP_TOKEN_RECORD_DELIMITER, out values))
			{
				if ((object)values != null && values.Count == 1)
					recordDelimiter = values[0];
			}

			quoteValue = string.Empty;
			if (properties.TryGetValue(PROP_TOKEN_QUOTE_VALUE, out values))
			{
				if ((object)values != null && values.Count == 1)
					quoteValue = values[0];
			}

			if (!SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(fieldDelimiter))
			{
				fieldDelimiter = fieldDelimiter.Replace("\\\\t", "\t");
				fieldDelimiter = fieldDelimiter.Replace("\\\\r", "\r");
				fieldDelimiter = fieldDelimiter.Replace("\\\\n", "\n");
				fieldDelimiter = fieldDelimiter.Replace("\\\"", "\"");
			}

			if (!SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(recordDelimiter))
			{
				recordDelimiter = recordDelimiter.Replace("\\\\t", "\t");
				recordDelimiter = recordDelimiter.Replace("\\\\r", "\r");
				recordDelimiter = recordDelimiter.Replace("\\\\n", "\n");
				recordDelimiter = recordDelimiter.Replace("\\\"", "\"");
			}

			if (!SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(quoteValue))
			{
				quoteValue = quoteValue.Replace("\\\\t", "\t");
				quoteValue = quoteValue.Replace("\\\\r", "\r");
				quoteValue = quoteValue.Replace("\\\\n", "\n");
				quoteValue = quoteValue.Replace("\\\"", "\"");
			}

			tempOc = objectConstruct01 = new ObjectConstruct();
			objectConstruct01.Name = "DelimitedTextSpec";
			objectConstruct00.Items.Add(objectConstruct01);

			propertyConstruct01 = new PropertyConstruct()
								{
									Name = "FirstRecordIsHeader",
									Value = firstRecordIsHeader.ToString()
								};
			objectConstruct01.Items.Add(propertyConstruct01);

			propertyConstruct01 = new PropertyConstruct()
								{
									Name = "RecordDelimiter",
									Value = recordDelimiter
								};
			objectConstruct01.Items.Add(propertyConstruct01);

			propertyConstruct01 = new PropertyConstruct()
								{
									Name = "FieldDelimiter",
									Value = fieldDelimiter
								};
			objectConstruct01.Items.Add(propertyConstruct01);

			propertyConstruct01 = new PropertyConstruct()
								{
									Name = "QuoteValue",
									Value = quoteValue
								};
			objectConstruct01.Items.Add(propertyConstruct01);

			using (StreamReader streamReader = File.OpenText(sourceFilePath))
			{
				int i = 0;

				arrayConstruct00 = new ArrayConstruct();
				arrayConstruct00.Name = "Records";
				objectConstruct00.Items.Add(arrayConstruct00);

				while ((line = (streamReader.ReadLine() ?? string.Empty)).Trim() != string.Empty)
				{
					string[] fields;

					objectConstruct01 = new ObjectConstruct();
					arrayConstruct00.Items.Add(objectConstruct01);

					fields = line.Split(fieldDelimiter.ToCharArray());

					if (firstRecordIsHeader && i == 0)
					{
						headers = fields;
						arrayConstruct00.Items.Remove(objectConstruct01);
						i++;
						continue;
					}

					if ((object)fields != null)
					{
						int j = 0;

						foreach (string field in fields)
						{
							propertyConstruct01 = new PropertyConstruct();

							if (firstRecordIsHeader && (object)headers != null)
								propertyConstruct01.Name = string.Format("{0}", headers[j++]);
							else
								propertyConstruct01.Name = string.Format("TextFileField_{0:00000000}", j++);

							propertyConstruct01.RawValue = field;
							objectConstruct01.Items.Add(propertyConstruct01);
						}
					}

					i++;
				}
			}

			return objectConstruct00;
		}

		#endregion
	}
}