/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

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
			const string PROP_TOKEN_FIRST_ROW_CONTAINS_COLUMN_HEADINGS = "FirstRowIsHeader";
			const string PROP_TOKEN_FIELD_DELIMITER = "FieldDelimiter";
			const string PROP_TOKEN_ROW_DELIMITER = "RowDelimiter";
			const string PROP_TOKEN_VALUE_QUALIFIER = "ValueQualifier";

			IList<string> values;
			bool firstRowIsHeader;
			string rowDelimiter, fieldDelimiter;
			string[] headerNames = new string[] { };

			ObjectConstruct objectConstruct00;
			ArrayConstruct arrayConstruct00;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException("sourceFilePath");

			if ((object)properties == null)
				throw new ArgumentNullException("properties");

			if (DataType.Instance.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException("sourceFilePath");

			sourceFilePath = Path.GetFullPath(sourceFilePath);

			objectConstruct00 = new ObjectConstruct();
			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "TextFileLines";
			objectConstruct00.Items.Add(arrayConstruct00);

			firstRowIsHeader = false;
			if (properties.TryGetValue(PROP_TOKEN_FIRST_ROW_CONTAINS_COLUMN_HEADINGS, out values))
			{
				if ((object)values != null && values.Count == 1)
				{
					if (!DataType.Instance.TryParse<bool>(values[0], out firstRowIsHeader))
						firstRowIsHeader = false;
				}
			}

			fieldDelimiter = string.Empty;
			if (properties.TryGetValue(PROP_TOKEN_FIELD_DELIMITER, out values))
			{
				if ((object)values != null && values.Count == 1)
					fieldDelimiter = values[0];
			}

			rowDelimiter = string.Empty;
			if (properties.TryGetValue(PROP_TOKEN_ROW_DELIMITER, out values))
			{
				if ((object)values != null && values.Count == 1)
					rowDelimiter = values[0];
			}

			if (!DataType.Instance.IsNullOrWhiteSpace(fieldDelimiter))
			{
				fieldDelimiter = fieldDelimiter.Replace("\\t", "\t");
				fieldDelimiter = fieldDelimiter.Replace("\\r", "\r");
				fieldDelimiter = fieldDelimiter.Replace("\\n", "\n");
			}

			using (StreamReader streamReader = File.OpenText(sourceFilePath))
			{
				using (StructuredTextReader structuredTextReader = new StructuredTextReader(streamReader, firstRowIsHeader, headerNames, fieldDelimiter, rowDelimiter))
				{
					ObjectConstruct objectConstruct01;
					PropertyConstruct propertyConstruct01;

					var rows = structuredTextReader.ReadRowsUsingDelimiters();

					foreach (var row in rows)
					{
						objectConstruct01 = new ObjectConstruct();
						arrayConstruct00.Items.Add(objectConstruct01);

						foreach (var field in row)
						{
							propertyConstruct01 = new PropertyConstruct();
							propertyConstruct01.Name = DataType.Instance.IsNullOrWhiteSpace(field.Key) ? "TextFileLine" : field.Key;
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