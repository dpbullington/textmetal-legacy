/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualBasic.FileIO;

using TextMetal.Framework.Associative;
using TextMetal.Middleware.Solder.Utilities;

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

		#endregion

		#region Methods/Operators

		protected override object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			const string PROP_TOKEN_FIRST_RECORD_CONTAINS_COLUMN_HEADINGS = "FirstRecordIsHeader";
			const string PROP_TOKEN_QUOTED_VALUES = "HasQuotedValues";
			const string PROP_TOKEN_HEADER_NAMES = "HeaderName";
			const string PROP_TOKEN_FIELD_DELIMITER = "FieldDelimiter";

			IList<string> values;
			bool firstRecordIsHeader;
			bool hasQuotedValues;
			string fieldDelimiter;
			string[] headerNames;

			ObjectConstruct objectConstruct00;
			ArrayConstruct arrayConstruct00;
			ObjectConstruct objectConstruct01;
			PropertyConstruct propertyConstruct01;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException("sourceFilePath");

			if ((object)properties == null)
				throw new ArgumentNullException("properties");

			if (DataTypeFascade.Instance.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException("sourceFilePath");

			sourceFilePath = Path.GetFullPath(sourceFilePath);

			objectConstruct00 = new ObjectConstruct();

			firstRecordIsHeader = false;
			if (properties.TryGetValue(PROP_TOKEN_FIRST_RECORD_CONTAINS_COLUMN_HEADINGS, out values))
			{
				if ((object)values != null && values.Count == 1)
				{
					if (!DataTypeFascade.Instance.TryParse<bool>(values[0], out firstRecordIsHeader))
						firstRecordIsHeader = false;
				}
			}

			hasQuotedValues = false;
			if (properties.TryGetValue(PROP_TOKEN_QUOTED_VALUES, out values))
			{
				if ((object)values != null && values.Count == 1)
				{
					if (!DataTypeFascade.Instance.TryParse<bool>(values[0], out hasQuotedValues))
						hasQuotedValues = false;
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

			if (!DataTypeFascade.Instance.IsNullOrWhiteSpace(fieldDelimiter))
			{
				fieldDelimiter = fieldDelimiter.Replace("\\\\t", "\t");
				fieldDelimiter = fieldDelimiter.Replace("\\\"", "\"");
			}

			using (StreamReader streamReader = File.OpenText(sourceFilePath))
			{
				using (TextFieldParser textFieldParser = new TextFieldParser(streamReader))
				{
					string[] fields;

					arrayConstruct00 = new ArrayConstruct();
					arrayConstruct00.Name = "Records";
					objectConstruct00.Items.Add(arrayConstruct00);

					textFieldParser.TextFieldType = FieldType.Delimited;
					textFieldParser.Delimiters = new string[] { fieldDelimiter };
					textFieldParser.HasFieldsEnclosedInQuotes = hasQuotedValues;

					int recordIndex = 0;
					while ((object)(fields = textFieldParser.ReadFields()) != null)
					{
						//System.Diagnostics.Debugger.Launch();
						objectConstruct01 = new ObjectConstruct();
						arrayConstruct00.Items.Add(objectConstruct01);

						if (recordIndex == 0 && firstRecordIsHeader)
							headerNames = fields;
						else
						{
							int fieldIndex = 0;
							foreach (var field in fields)
							{
								propertyConstruct01 = new PropertyConstruct();
								propertyConstruct01.Name = headerNames[fieldIndex++];
								propertyConstruct01.RawValue = field;
								objectConstruct01.Items.Add(propertyConstruct01);
							}
						}

						recordIndex++;
					}
				}
			}

			return objectConstruct00;
		}

		#endregion
	}
}