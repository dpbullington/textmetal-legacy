﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;

using TextMetal.Framework.Associative;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.Source.Primative
{
	public class XmlSchemaSourceStrategy : SourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the XmlSchemaSourceStrategy class.
		/// </summary>
		public XmlSchemaSourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		private static void EnumSchema(IAssociativeXmlObject parentAssociativeXmlObject, XmlSchemaObjectCollection currentXmlSchemaObjectCollection)
		{
			XmlSchemaElement xmlSchemaElement;
			XmlSchemaComplexType xmlSchemaComplexType;
			XmlSchemaSequence xmlSchemaSequence;
			XmlSchemaSimpleType xmlSchemaSimpleType;
			ArrayConstruct arrayConstruct00, arrayConstruct01;
			ObjectConstruct objectConstruct00, objectConstruct01;
			PropertyConstruct propertyConstruct00, propertyConstruct01;

			if ((object)parentAssociativeXmlObject == null)
				throw new ArgumentNullException(nameof(parentAssociativeXmlObject));

			if ((object)currentXmlSchemaObjectCollection == null)
				throw new ArgumentNullException(nameof(currentXmlSchemaObjectCollection));

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "XmlSchemaElements";
			parentAssociativeXmlObject.Items.Add(arrayConstruct00);

			foreach (XmlSchemaObject xmlSchemaObject in currentXmlSchemaObjectCollection)
			{
				objectConstruct00 = new ObjectConstruct();
				arrayConstruct00.Items.Add(objectConstruct00);

				xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;

				if ((object)xmlSchemaElement != null)
				{
					if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(xmlSchemaElement.Name) &&
						!SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(xmlSchemaElement.RefName.Name))
					{
						propertyConstruct00 = new PropertyConstruct();
						propertyConstruct00.Name = "XmlSchemaElementIsRef";
						propertyConstruct00.RawValue = true;
						objectConstruct00.Items.Add(propertyConstruct00);

						propertyConstruct00 = new PropertyConstruct();
						propertyConstruct00.Name = "XmlSchemaElementLocalName";
						propertyConstruct00.RawValue = xmlSchemaElement.RefName.Name;
						objectConstruct00.Items.Add(propertyConstruct00);

						propertyConstruct00 = new PropertyConstruct();
						propertyConstruct00.Name = "XmlSchemaElementNamespace";
						propertyConstruct00.RawValue = xmlSchemaElement.RefName.Namespace;
						objectConstruct00.Items.Add(propertyConstruct00);

						continue;
					}
					else
					{
						propertyConstruct00 = new PropertyConstruct();
						propertyConstruct00.Name = "XmlSchemaElementIsRef";
						propertyConstruct00.RawValue = false;
						objectConstruct00.Items.Add(propertyConstruct00);

						propertyConstruct00 = new PropertyConstruct();
						propertyConstruct00.Name = "XmlSchemaElementLocalName";
						propertyConstruct00.RawValue = xmlSchemaElement.QualifiedName.Name;
						objectConstruct00.Items.Add(propertyConstruct00);

						propertyConstruct00 = new PropertyConstruct();
						propertyConstruct00.Name = "XmlSchemaElementNamespace";
						propertyConstruct00.RawValue = xmlSchemaElement.QualifiedName.Namespace;
						objectConstruct00.Items.Add(propertyConstruct00);

						xmlSchemaComplexType = xmlSchemaElement.ElementSchemaType as XmlSchemaComplexType;
						xmlSchemaSimpleType = xmlSchemaElement.ElementSchemaType as XmlSchemaSimpleType;

						if ((object)xmlSchemaSimpleType != null)
						{
							propertyConstruct00 = new PropertyConstruct();
							propertyConstruct00.Name = "XmlSchemaElementSimpleType";
							propertyConstruct00.RawValue = xmlSchemaSimpleType.Datatype.TypeCode;
							objectConstruct00.Items.Add(propertyConstruct00);
						}
						else if ((object)xmlSchemaComplexType != null)
						{
							arrayConstruct01 = new ArrayConstruct();
							arrayConstruct01.Name = "XmlSchemaAttributes";
							objectConstruct00.Items.Add(arrayConstruct01);

							if ((object)xmlSchemaComplexType.Attributes != null)
							{
								foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
								{
									objectConstruct01 = new ObjectConstruct();
									arrayConstruct01.Items.Add(objectConstruct01);

									propertyConstruct01 = new PropertyConstruct();
									propertyConstruct01.Name = "XmlSchemaElementLocalName";
									propertyConstruct01.RawValue = xmlSchemaAttribute.QualifiedName.Name;
									objectConstruct01.Items.Add(propertyConstruct01);

									propertyConstruct01 = new PropertyConstruct();
									propertyConstruct01.Name = "XmlSchemaElementNamespace";
									propertyConstruct01.RawValue = xmlSchemaAttribute.QualifiedName.Namespace;
									objectConstruct01.Items.Add(propertyConstruct01);

									propertyConstruct01 = new PropertyConstruct();
									propertyConstruct01.Name = "XmlSchemaElementNamespace";
									propertyConstruct01.RawValue = xmlSchemaAttribute.AttributeSchemaType.TypeCode;
									objectConstruct01.Items.Add(propertyConstruct01);
								}
							}

							xmlSchemaSequence = xmlSchemaComplexType.ContentTypeParticle as XmlSchemaSequence;

							if ((object)xmlSchemaSequence != null)
								EnumSchema(objectConstruct00, xmlSchemaSequence.Items);
						}
					}
				}
			}
		}

		private static void ValidationCallback(object sender, ValidationEventArgs args)
		{
			Console.WriteLine(args.Message);
		}

		protected override object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			XmlSchemaSet xmlSchemaSet;
			XmlSchema xmlSchema;
			ObjectConstruct objectConstruct00;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException(nameof(sourceFilePath));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException(nameof(sourceFilePath));

			sourceFilePath = Path.GetFullPath(sourceFilePath);

			objectConstruct00 = new ObjectConstruct();

			using (Stream stream = File.Open(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				xmlSchema = XmlSchema.Read(stream, ValidationCallback);

			xmlSchemaSet = new XmlSchemaSet();
			xmlSchemaSet.Add(xmlSchema);
			xmlSchemaSet.Compile();

			xmlSchema = xmlSchemaSet.Schemas().Cast<XmlSchema>().ToList()[0];

			EnumSchema(objectConstruct00, xmlSchema.Items);

			return objectConstruct00;
		}

		#endregion
	}
}