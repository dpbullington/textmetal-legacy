/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.XmlDialect
{
	public static class XoExtensions
	{
		#region Methods/Operators

		public static XmlName GetXmlName(this IXmlObject xmlObject)
		{
			Type xmlObjectType;
			XmlName xmlName;
			XmlElementMappingAttribute xmlElementMappingAttribute;

			if ((object)xmlObject == null)
				throw new ArgumentNullException(nameof(xmlObject));

			xmlObjectType = xmlObject.GetType();
			xmlElementMappingAttribute = ExtensionMethods.ReflectionFascadeLegacyInstance.GetOneAttribute<XmlElementMappingAttribute>(xmlObjectType);

			if ((object)xmlElementMappingAttribute == null)
				xmlName = null;
			else
			{
				xmlName = new XmlName()
						{
							LocalName = xmlElementMappingAttribute.LocalName,
							NamespaceUri = xmlElementMappingAttribute.NamespaceUri
						};
			}

			return xmlName;
		}

		#endregion
	}
}