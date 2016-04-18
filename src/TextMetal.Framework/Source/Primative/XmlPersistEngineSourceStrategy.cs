/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using TextMetal.Framework.XmlDialect;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.Source.Primative
{
	public class XmlPersistEngineSourceStrategy : SourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the XmlPersistEngineSourceStrategy class.
		/// </summary>
		public XmlPersistEngineSourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		protected override object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			IXmlPersistEngine xpe;
			object retval;

			xpe = this.GetXmlPersistEngine(properties);

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException(nameof(sourceFilePath));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

			if (ExtensionMethods.DataTypeFascadeLegacyInstance.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException(nameof(sourceFilePath));

			if ((object)xpe == null)
				throw new InvalidOperationException(nameof(xpe));

			sourceFilePath = Path.GetFullPath(sourceFilePath);

			retval = new XpeSerializationStrategy(xpe).GetObjectFromFile<object>(sourceFilePath);

			return retval;
		}

		protected virtual IXmlPersistEngine GetXmlPersistEngine(IDictionary<string, IList<string>> properties)
		{
			const string PROP_TOKEN_KNOWN_XML_OBJECT_AQTN = "KnownXmlObjectType";
			const string PROP_TOKEN_KNOWN_XML_TEXT_OBJECT_AQTN = "KnownXmlTextObjectType";
			IXmlPersistEngine xpe;
			IList<string> values;
			string xmlObjectAqtn;
			Type xmlObjectType = null;

			if (properties == null)
				throw new ArgumentNullException(nameof(properties));

			xpe = new XmlPersistEngine();
			xmlObjectAqtn = null;

			if (properties.TryGetValue(PROP_TOKEN_KNOWN_XML_TEXT_OBJECT_AQTN, out values))
			{
				if ((object)values != null && values.Count == 1)
				{
					xmlObjectAqtn = values[0];
					xmlObjectType = Type.GetType(xmlObjectAqtn, false);
				}

				if ((object)xmlObjectType == null)
					throw new InvalidOperationException(string.Format("Failed to load the XML text object type '{0}' via Type.GetType(..).", xmlObjectAqtn));

				if (!typeof(IXmlTextObject).IsAssignableFrom(xmlObjectType))
					throw new InvalidOperationException(string.Format("The XML text object type is not assignable to type '{0}'.", typeof(IXmlTextObject).FullName));

				xpe.RegisterKnownXmlTextObject(xmlObjectType);
			}

			if (properties.TryGetValue(PROP_TOKEN_KNOWN_XML_OBJECT_AQTN, out values))
			{
				if ((object)values != null)
				{
					foreach (string value in values)
					{
						xmlObjectAqtn = value;
						xmlObjectType = Type.GetType(xmlObjectAqtn, false);

						if ((object)xmlObjectType == null)
							throw new InvalidOperationException(string.Format("Failed to load the XML object type '{0}' via Type.GetType(..).", xmlObjectAqtn));

						if (!typeof(IXmlObject).IsAssignableFrom(xmlObjectType))
							throw new InvalidOperationException(string.Format("The XML object type is not assignable to type '{0}'.", typeof(IXmlObject).FullName));

						xpe.RegisterKnownXmlObject(xmlObjectType);
					}
				}
			}

			return xpe;
		}

		#endregion
	}
}