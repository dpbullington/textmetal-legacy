/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Serialization;

namespace TextMetal.Framework.Source.Primative
{
	public class XmlSerializerSourceStrategy : SourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the XmlSerializerSourceStrategy class.
		/// </summary>
		public XmlSerializerSourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		protected override object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			const string CMDLN_TOKEN_XML_SERIALIZED_AQTN = "XmlSerializedType";
			string xmlSerializedObjectAqtn;
			Type xmlSerializedObjectType = null;
			IList<string> values;
			object retval;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException(nameof(sourceFilePath));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException(nameof(sourceFilePath));

			xmlSerializedObjectAqtn = null;
			if (properties.TryGetValue(CMDLN_TOKEN_XML_SERIALIZED_AQTN, out values))
			{
				if ((object)values != null && values.Count == 1)
				{
					xmlSerializedObjectAqtn = values[0];
					xmlSerializedObjectType = Type.GetType(xmlSerializedObjectAqtn, false);
				}
			}

			if ((object)xmlSerializedObjectType == null)
				throw new InvalidOperationException(string.Format("Failed to load the XML type '{0}' via Type.GetType(..).", xmlSerializedObjectAqtn));

			sourceFilePath = Path.GetFullPath(sourceFilePath);

			retval = XmlSerializationStrategy.Instance.GetObjectFromFile(sourceFilePath, xmlSerializedObjectType);

			return retval;
		}

		#endregion
	}
}