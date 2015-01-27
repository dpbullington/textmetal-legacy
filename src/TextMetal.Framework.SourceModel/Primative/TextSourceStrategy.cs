/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TextMetal.Common.Core;
using TextMetal.Framework.AssociativeModel;

namespace TextMetal.Framework.SourceModel.Primative
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
			DelimitedTextSpec delimitedTextSpec;
			bool firstRecordIsHeader;
			string recordDelimiter;
			string fieldDelimiter;
			string quoteValue;
			string[] headerNames;

			ObjectConstruct objectConstruct00;
			ArrayConstruct arrayConstruct00;
			ObjectConstruct objectConstruct01;
			PropertyConstruct propertyConstruct01;

			ObjectConstruct tempOc;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException("sourceFilePath");

			if ((object)properties == null)
				throw new ArgumentNullException("properties");

			if (DataType.Instance.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException("sourceFilePath");

			sourceFilePath = Path.GetFullPath(sourceFilePath);

			objectConstruct00 = new ObjectConstruct();

			firstRecordIsHeader = false;
			if (properties.TryGetValue(PROP_TOKEN_FIRST_RECORD_CONTAINS_COLUMN_HEADINGS, out values))
			{
				if ((object)values != null && values.Count == 1)
				{
					if (!DataType.Instance.TryParse<bool>(values[0], out firstRecordIsHeader))
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

			if (!DataType.Instance.IsNullOrWhiteSpace(fieldDelimiter))
			{
				fieldDelimiter = fieldDelimiter.Replace("\\\\t", "\t");
				fieldDelimiter = fieldDelimiter.Replace("\\\\r", "\r");
				fieldDelimiter = fieldDelimiter.Replace("\\\\n", "\n");
				fieldDelimiter = fieldDelimiter.Replace("\\\"", "\"");
			}

			if (!DataType.Instance.IsNullOrWhiteSpace(recordDelimiter))
			{
				recordDelimiter = recordDelimiter.Replace("\\\\t", "\t");
				recordDelimiter = recordDelimiter.Replace("\\\\r", "\r");
				recordDelimiter = recordDelimiter.Replace("\\\\n", "\n");
				recordDelimiter = recordDelimiter.Replace("\\\"", "\"");
			}

			if (!DataType.Instance.IsNullOrWhiteSpace(quoteValue))
			{
				quoteValue = quoteValue.Replace("\\\\t", "\t");
				quoteValue = quoteValue.Replace("\\\\r", "\r");
				quoteValue = quoteValue.Replace("\\\\n", "\n");
				quoteValue = quoteValue.Replace("\\\"", "\"");
			}

			delimitedTextSpec = new DelimitedTextSpec();
			delimitedTextSpec.FirstRecordIsHeader = firstRecordIsHeader;
			delimitedTextSpec.RecordDelimiter = recordDelimiter;
			delimitedTextSpec.FieldDelimiter = fieldDelimiter;
			delimitedTextSpec.QuoteValue = quoteValue;

			if ((object)headerNames != null)
			{
				delimitedTextSpec.HeaderSpecs.Clear();
				foreach (string headerName in headerNames)
				{
					delimitedTextSpec.HeaderSpecs.Add(new HeaderSpec()
													{
														HeaderName = headerName,
														FieldType = FieldType.Undefined
													});
				}
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
				using (DelimitedTextReader delimitedTextReader = new DelimitedTextReader(streamReader, delimitedTextSpec))
				{
					var __headerSpecs = delimitedTextReader.ReadHeaderSpecs();

					objectConstruct01 = new ObjectConstruct();
					objectConstruct01.Name = "HeaderNames";
					tempOc.Items.Add(objectConstruct01);

					if ((object)__headerSpecs != null)
					{
						foreach (var headerSpec in __headerSpecs)
						{
							propertyConstruct01 = new PropertyConstruct()
												{
													Name = "HeaderName",
													Value = headerSpec.HeaderName
												};
							objectConstruct01.Items.Add(propertyConstruct01);
						}
					}

					var records = delimitedTextReader.ReadRecords();

					arrayConstruct00 = new ArrayConstruct();
					arrayConstruct00.Name = "Records";
					objectConstruct00.Items.Add(arrayConstruct00);

					foreach (var record in records)
					{
						objectConstruct01 = new ObjectConstruct();
						arrayConstruct00.Items.Add(objectConstruct01);

						foreach (var field in record)
						{
							propertyConstruct01 = new PropertyConstruct();
							propertyConstruct01.Name = field.Key;
							propertyConstruct01.RawValue = field.Value;
							objectConstruct01.Items.Add(propertyConstruct01);
						}
					}
				}
			}

			return objectConstruct00;
		}

		#endregion
	}
}