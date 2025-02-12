﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Framework.XmlDialect
{
	/// <summary>
	/// This is a custom XML serializer/deserializer that does not suffer from the rigidities of the .NET Framework supplied ones. This implementation was designed to be fast and flexible for XML driven tools.
	/// </summary>
	public sealed class XmlPersistEngine : IXmlPersistEngine
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the XmlPersistEngine class.
		/// </summary>
		public XmlPersistEngine()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly Dictionary<XmlName, Type> knownXmlObjectTypeRegistrations = new Dictionary<XmlName, Type>();
		private Type knownXmlTextObjectTypeRegistration;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the known XML object type registrations.
		/// </summary>
		private Dictionary<XmlName, Type> KnownXmlObjectTypeRegistrations
		{
			get
			{
				return this.knownXmlObjectTypeRegistrations;
			}
		}

		/// <summary>
		/// Gets or sets the known XML text object type registration.
		/// </summary>
		private Type KnownXmlTextObjectTypeRegistration
		{
			get
			{
				return this.knownXmlTextObjectTypeRegistration;
			}
			set
			{
				this.knownXmlTextObjectTypeRegistration = value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Determines whether an element is a text element.
		/// </summary>
		/// <param name="contextStack"> The effective context stack. </param>
		/// <param name="xmlName"> The XML name to use. </param>
		/// <returns> A value indicating whether the element is a text element. </returns>
		private static bool IsTextElement(Stack<IXmlObject> contextStack, XmlName xmlName)
		{
			IXmlObject parentXmlObject;
			Type parentType;
			PropertyInfo[] parentPropertyInfos;

			XmlAttributeMappingAttribute xmlAttributeMappingAttribute;
			XmlChildElementMappingAttribute xmlChildElementMappingAttribute;

			int attributeCount;

			if ((object)contextStack == null)
				throw new ArgumentNullException(nameof(contextStack));

			if ((object)xmlName == null)
				throw new ArgumentNullException(nameof(xmlName));

			// sanity check
			if (contextStack.Count < 1)
				return false;

			// interogate the parent (last pushed value)
			parentXmlObject = contextStack.Peek();
			parentType = parentXmlObject.GetType();
			parentPropertyInfos = parentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			// examine parent mapping tables for attributes and child elements
			if ((object)parentPropertyInfos != null)
			{
				foreach (PropertyInfo parentPropertyInfo in parentPropertyInfos)
				{
					// get potential attribute mapping metadata
					xmlAttributeMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlAttributeMappingAttribute>(parentPropertyInfo);

					// get the potential child mapping metadata
					xmlChildElementMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlChildElementMappingAttribute>(parentPropertyInfo);

					// count what we found; there can only be one
					attributeCount = 0;
					attributeCount += (object)xmlAttributeMappingAttribute == null ? 0 : 1;
					attributeCount += (object)xmlChildElementMappingAttribute == null ? 0 : 1;

					// sanity check
					if (attributeCount > 1)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// we only care about child elements
					if ((object)xmlChildElementMappingAttribute == null)
						continue;

					// is this mapped as a text element?
					if (xmlChildElementMappingAttribute.ChildElementType == ChildElementType.TextValue &&
						xmlChildElementMappingAttribute.LocalName == xmlName.LocalName &&
						xmlChildElementMappingAttribute.NamespaceUri == xmlName.NamespaceUri)
						return true;
				}
			}

			// nope, we exhausted our search
			return false;
		}

		/// <summary>
		/// Clears all known XML object registrations.
		/// </summary>
		public void ClearAllKnowns()
		{
			this.KnownXmlObjectTypeRegistrations.Clear();
			this.KnownXmlTextObjectTypeRegistration = null;
		}

		/// <summary>
		/// Deserialize an XML object graph from the specified XML file.
		/// </summary>
		/// <param name="fileName"> The XML file to load. </param>
		/// <returns> An XML object graph. </returns>
		public IXmlObject DeserializeFromXml(string fileName)
		{
			IXmlObject document;

			if ((object)fileName == null)
				throw new ArgumentNullException(nameof(fileName));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(fileName))
				throw new ArgumentOutOfRangeException(nameof(fileName));

			using (Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				document = this.DeserializeFromXml(stream);
				return document;
			}
		}

		/// <summary>
		/// Deserialize an XML object graph from the specified stream.
		/// </summary>
		/// <param name="stream"> The stream to load. </param>
		/// <returns> An XML object graph. </returns>
		public IXmlObject DeserializeFromXml(Stream stream)
		{
			XmlTextReader xmlTextReader;
			IXmlObject document;

			if ((object)stream == null)
				throw new ArgumentNullException(nameof(stream));

			// DO NOT USE A USING BLOCK HERE (CALLER OWNS STREAM) !!!
			xmlTextReader = new XmlTextReader(stream);
			document = this.DeserializeFromXml(xmlTextReader);
			return document;
		}

		/// <summary>
		/// Private method that processes XML object deserialization.
		/// </summary>
		/// <param name="contextStack"> The context stack used to manage deserialization. </param>
		/// <param name="previousElementXmlName"> The previously encountered XML name (parent). </param>
		/// <param name="currentElementXmlName"> The current XML name (current). </param>
		/// <param name="attributes"> The current attributes for the current XML object. </param>
		/// <param name="overrideCurrentXmlTextObject"> A special overriding XML text object. </param>
		/// <returns> The created current XML object. </returns>
		private IXmlObject DeserializeFromXml(IXmlLineInfo xmlLineInfo, Stack<IXmlObject> contextStack, XmlName previousElementXmlName, XmlName currentElementXmlName, IDictionary<XmlName, string> attributes, IXmlTextObject overrideCurrentXmlTextObject)
		{
			IXmlObject currentXmlObject;
			Type currentType;
			PropertyInfo[] currentPropertyInfos;
			XmlElementMappingAttribute currentXmlElementMappingAttribute;

			IXmlObject parentXmlObject = null;
			Type parentType = null;
			PropertyInfo[] parentPropertyInfos;
			XmlElementMappingAttribute parentXmlElementMappingAttribute = null;

			Dictionary<PropertyInfo, XmlAttributeMappingAttribute> parentPropertyToAttributeMappings;
			Dictionary<PropertyInfo, XmlChildElementMappingAttribute> parentPropertyToChildElementMappings;

			Dictionary<PropertyInfo, XmlAttributeMappingAttribute> currentPropertyToAttributeMappings;
			Dictionary<PropertyInfo, XmlChildElementMappingAttribute> currentPropertyToChildElementMappings;

			KeyValuePair<PropertyInfo, XmlChildElementMappingAttribute>? parentPropertyToChildElementMapping = null;

			int attributeCount;

			XmlAttributeMappingAttribute xmlAttributeMappingAttribute;
			XmlChildElementMappingAttribute xmlChildElementMappingAttribute;
			XmlElementMappingAttribute parentOfChildXmlElementMappingAttribute;

			Match match;
			string parentName;
			string propertyName;
			const string PROP_REGEX =
				@"( [a-zA-Z_][a-zA-Z_0-9\-]+ ) \. ( [a-zA-Z_][a-zA-Z_0-9\-]+ )";

			if ((object)contextStack == null)
				throw new ArgumentNullException(nameof(contextStack));

			if ((object)currentElementXmlName == null)
				throw new ArgumentNullException(nameof(currentElementXmlName));

			if ((object)attributes == null)
				throw new ArgumentNullException(nameof(attributes));

			if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(currentElementXmlName.LocalName))
				throw new ArgumentOutOfRangeException(nameof(currentElementXmlName));

			if (contextStack.Count > 0) // is this NOT the root node?
			{
				// element on stack is parent
				parentXmlObject = contextStack.Peek();

				// sanity check
				if ((object)parentXmlObject == null)
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

				// interogate parent XML object
				parentType = parentXmlObject.GetType();
				parentPropertyInfos = parentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

				// sanity check
				if ((object)parentPropertyInfos == null)
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

				// get parent mapping metadata
				parentXmlElementMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlElementMappingAttribute>(parentType);

				// sanity check
				if ((object)parentXmlElementMappingAttribute == null)
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

				// create parent mapping tables for attributes and child elements
				parentPropertyToAttributeMappings = new Dictionary<PropertyInfo, XmlAttributeMappingAttribute>();
				parentPropertyToChildElementMappings = new Dictionary<PropertyInfo, XmlChildElementMappingAttribute>();

				foreach (PropertyInfo parentPropertyInfo in parentPropertyInfos)
				{
					// get potential attribute mapping metadata
					xmlAttributeMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlAttributeMappingAttribute>(parentPropertyInfo);

					// get the potential child mapping metadata
					xmlChildElementMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlChildElementMappingAttribute>(parentPropertyInfo);

					// count what we found; there can only be one
					attributeCount = 0;
					attributeCount += (object)xmlAttributeMappingAttribute == null ? 0 : 1;
					attributeCount += (object)xmlChildElementMappingAttribute == null ? 0 : 1;

					// sanity check
					if (attributeCount > 1)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// append to the correct mapping table
					if ((object)xmlAttributeMappingAttribute != null) // is this an attribute mapping?
						parentPropertyToAttributeMappings.Add(parentPropertyInfo, xmlAttributeMappingAttribute);
					else if ((object)xmlChildElementMappingAttribute != null) // or is this a child element mapping?
						parentPropertyToChildElementMappings.Add(parentPropertyInfo, xmlChildElementMappingAttribute);
				}

				// is this a text element node override?
				if ((object)overrideCurrentXmlTextObject != null)
				{
					string svalue;
					object ovalue;

					// resolve the mapping to get text element property
					parentPropertyToChildElementMapping = parentPropertyToChildElementMappings
						.Where(x => x.Value.ChildElementType == ChildElementType.TextValue)
						.Select(x => (KeyValuePair<PropertyInfo, XmlChildElementMappingAttribute>?)x).Where(x => x.Value.Value.LocalName == overrideCurrentXmlTextObject.Name.LocalName &&
																												x.Value.Value.NamespaceUri == overrideCurrentXmlTextObject.Name.NamespaceUri).SingleOrDefault();

					// sanity check
					if ((object)parentPropertyToChildElementMapping == null)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// sanity check
					if (!parentPropertyToChildElementMapping.Value.Key.CanWrite)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// get the raw string value
					svalue = overrideCurrentXmlTextObject.Text;

					// convert to strongly-typed value
					if (!SolderFascadeAccessor.DataTypeFascade.TryParse(parentPropertyToChildElementMapping.Value.Key.PropertyType, svalue, out ovalue))
						ovalue = SolderFascadeAccessor.DataTypeFascade.DefaultValue(parentPropertyToChildElementMapping.Value.Key.PropertyType);

					// attempt to set the value
					if (!SolderFascadeAccessor.ReflectionFascade.SetLogicalPropertyValue(parentXmlObject, parentPropertyToChildElementMapping.Value.Key.Name, ovalue))
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// return null to prevent recursion
					return null;
				}

				// check if this element is a parent DOT property (<Parent.Property />) convention (similar to XAML)
				match = Regex.Match(currentElementXmlName.LocalName, PROP_REGEX, RegexOptions.IgnorePatternWhitespace);

				if ((object)match != null && match.Success)
				{
					// yes, this is a parent DOT property naming
					parentName = match.Groups[1].Value;
					propertyName = match.Groups[2].Value;
				}
				else
				{
					// no, this is normal naming
					parentName = null;
					propertyName = currentElementXmlName.LocalName;
				}

				// sanity check to ensure local names match if parent DOT property convention is used
				if ((object)previousElementXmlName != null &&
					(object)parentName != null &&
					parentName != previousElementXmlName.LocalName)
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

				// resolve the mapping to get child element property
				parentPropertyToChildElementMapping = parentPropertyToChildElementMappings
					.Where(x => x.Value.ChildElementType != ChildElementType.TextValue)
					.Select(x => (KeyValuePair<PropertyInfo, XmlChildElementMappingAttribute>?)x)
					.Where(x => (x.Value.Value.NamespaceUri == currentElementXmlName.NamespaceUri &&
								x.Value.Value.LocalName == propertyName)).SingleOrDefault();

				if ((object)parentPropertyToChildElementMapping != null)
				{
					// sanity check
					if (!parentPropertyToChildElementMapping.Value.Key.CanWrite)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// get parent-of-child mapping metadata
					parentOfChildXmlElementMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlElementMappingAttribute>(parentPropertyToChildElementMapping.Value.Key.PropertyType);

					// sanity check
					if ((object)parentOfChildXmlElementMappingAttribute == null)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// override based on parent-of-child mapping metadata
					currentElementXmlName.LocalName = parentOfChildXmlElementMappingAttribute.LocalName;
					currentElementXmlName.NamespaceUri = parentOfChildXmlElementMappingAttribute.NamespaceUri;
				}
			}

			// factory-up the new XML object based on registered types
			currentXmlObject = this.ResolveXmlObject(currentElementXmlName);

			// sanity check
			if ((object)currentXmlObject == null)
				throw new InvalidOperationException(string.Format("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message. '{0}'", currentElementXmlName));

			// interogate the current type
			currentType = currentXmlObject.GetType();
			currentPropertyInfos = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			// sanity check
			if ((object)currentPropertyInfos == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			// get current mapping metadata
			currentXmlElementMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlElementMappingAttribute>(currentType);

			// sanity check
			if ((object)currentXmlElementMappingAttribute == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			// create current mapping tables for attributes and child elements
			currentPropertyToAttributeMappings = new Dictionary<PropertyInfo, XmlAttributeMappingAttribute>();
			currentPropertyToChildElementMappings = new Dictionary<PropertyInfo, XmlChildElementMappingAttribute>();

			foreach (PropertyInfo currentPropertyInfo in currentPropertyInfos)
			{
				// get potential attribute mapping metadata
				xmlAttributeMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlAttributeMappingAttribute>(currentPropertyInfo);

				// get the potential child mapping metadata
				xmlChildElementMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlChildElementMappingAttribute>(currentPropertyInfo);

				// count what we found; there can only be one
				attributeCount = 0;
				attributeCount += (object)xmlAttributeMappingAttribute == null ? 0 : 1;
				attributeCount += (object)xmlChildElementMappingAttribute == null ? 0 : 1;

				// sanity check
				if (attributeCount > 1)
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

				// append to the correct mapping table
				if ((object)xmlAttributeMappingAttribute != null) // is this an attribute mapping?
					currentPropertyToAttributeMappings.Add(currentPropertyInfo, xmlAttributeMappingAttribute);
				else if ((object)xmlChildElementMappingAttribute != null) // or is this a child element mapping?
					currentPropertyToChildElementMappings.Add(currentPropertyInfo, xmlChildElementMappingAttribute);
			}

			// iterate over attributes of current element
			if ((object)currentPropertyToAttributeMappings != null)
			{
				foreach (KeyValuePair<PropertyInfo, XmlAttributeMappingAttribute> currentPropertyToAttributeMapping in currentPropertyToAttributeMappings.OrderBy(m => m.Value.Order).ThenBy(m => m.Value.LocalName))
				{
					string svalue;
					object ovalue;
					var _currentPropertyToAttributeMapping = currentPropertyToAttributeMapping; // prevent closure bug

					// sanity check
					if (!currentPropertyToAttributeMapping.Key.CanWrite)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// get the raw string value
					svalue = attributes.Where(a => a.Key.LocalName == _currentPropertyToAttributeMapping.Value.LocalName &&
													a.Key.NamespaceUri == _currentPropertyToAttributeMapping.Value.NamespaceUri)
						.Select(a => a.Value).SingleOrDefault();

					// convert to strongly-typed value
					if (!SolderFascadeAccessor.DataTypeFascade.TryParse(currentPropertyToAttributeMapping.Key.PropertyType, svalue, out ovalue))
						ovalue = SolderFascadeAccessor.DataTypeFascade.DefaultValue(currentPropertyToAttributeMapping.Key.PropertyType);

					// attempt to set the values
					if (!SolderFascadeAccessor.ReflectionFascade.SetLogicalPropertyValue(currentXmlObject, currentPropertyToAttributeMapping.Key.Name, ovalue))
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");
				}
			}

			// determine where to store current XML object
			if ((object)parentPropertyToChildElementMapping != null)
			{
				// store this as a child element of parent XML object
				if (!SolderFascadeAccessor.ReflectionFascade.SetLogicalPropertyValue(parentXmlObject, parentPropertyToChildElementMapping.Value.Key.Name, currentXmlObject))
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");
			}
			else if ((object)parentXmlElementMappingAttribute != null)
			{
				// store this as an element of parent XML object

				// sanity check
				if ((object)parentXmlObject == null)
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

				if (parentXmlElementMappingAttribute.ChildElementModel == ChildElementModel.Content)
				{
					// only one content element is allowed, check to see if it is non-null instance
					if ((object)parentXmlObject.Content != null)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// assign to anonymous content property
					parentXmlObject.Content = currentXmlObject;
				}
				else if (parentXmlElementMappingAttribute.ChildElementModel == ChildElementModel.Items)
				{
					// any number of elements are allowed

					// sanity check
					if ((object)parentXmlObject.AllowedChildTypes == null)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// sanity check
					if ((object)parentXmlObject.Items == null)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// new collection type check
					if (parentXmlObject.AllowedChildTypes.Count(t => t.IsAssignableFrom(currentType)) <= 0)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// add to anonymous collection
					parentXmlObject.Items.Add(currentXmlObject);
				}
			}

			return currentXmlObject; // return current XML object
		}

		/// <summary>
		/// Deserialize an XML object graph from the specified XML reader.
		/// </summary>
		/// <param name="xmlReader"> The XML reader to load. </param>
		/// <returns> An XML object graph. </returns>
		public IXmlObject DeserializeFromXml(XmlReader xmlReader)
		{
			XmlName elementXmlName = null, attributeXmlName, previousElementXmlName;

			IXmlObject documentXmlObject = null;
			IXmlObject currentXmlObject;

			Stack<IXmlObject> contextStack;
			bool isEmptyElement, isTextElement;
			Dictionary<XmlName, string> attributes;

			IXmlLineInfo xmlLineInfo;

			if ((object)xmlReader == null)
				throw new ArgumentNullException(nameof(xmlReader));

			// setup contextual data
			attributes = new Dictionary<XmlName, string>();
			contextStack = new Stack<IXmlObject>();
			xmlLineInfo = xmlReader as IXmlLineInfo;

			// walk the XML document
			while (xmlReader.Read())
			{
				// determine node type
				if (xmlReader.NodeType == XmlNodeType.CDATA ||
					xmlReader.NodeType == XmlNodeType.Text) // textual node
				{
					// clear previous attributes
					attributes.Clear();

					// is this a text element?
					isTextElement = IsTextElement(contextStack, elementXmlName ?? new XmlName());

					// get the current XML object as XML text object
					currentXmlObject = this.DeserializeFromXmlText(xmlLineInfo, contextStack, xmlReader.Value, isTextElement ? elementXmlName : null);

					// is this a text element? if so, deserialize into it in a special maner
					if (isTextElement)
						this.DeserializeFromXml(xmlLineInfo, contextStack, null, elementXmlName, attributes, (IXmlTextObject)currentXmlObject);
				}
				else if (xmlReader.NodeType == XmlNodeType.Element) // actual elements
				{
					OnlyWhen._DEBUG_ThenPrint(string.Format("{2} <{0}{1}>", xmlReader.LocalName, xmlReader.IsEmptyElement ? " /" : string.Empty, xmlReader.IsEmptyElement ? "empty" : "begin"));

					// stash away previous element
					//previousElementXmlName = elementXmlName;
					// fixes a bug here
					if (contextStack.Count > 0)
						previousElementXmlName = contextStack.Peek().GetXmlName();
					else
						previousElementXmlName = null;

					// create the element XML name
					elementXmlName = new XmlName()
									{
										LocalName = xmlReader.LocalName,
										NamespaceUri = xmlReader.NamespaceURI
									};

					// is this an empty element?
					isEmptyElement = xmlReader.IsEmptyElement;

					// is this a text element?
					isTextElement = IsTextElement(contextStack, elementXmlName);

					// clear previous attributes
					attributes.Clear();

					// iterate over attributes of current element
					for (int ai = 0; ai < xmlReader.AttributeCount; ai++)
					{
						// traverse to next attribute
						xmlReader.MoveToAttribute(ai);

						// create the attribute XML name
						attributeXmlName = new XmlName()
											{
												LocalName = xmlReader.LocalName,
												NamespaceUri = xmlReader.NamespaceURI
											};

						// append to attribute collection
						attributes.Add(attributeXmlName, xmlReader.Value);
					}

					// clear attribute name
					attributeXmlName = null;

					// is this not a text element?
					if (!isTextElement)
					{
						// deserialize current XML object
						currentXmlObject = this.DeserializeFromXml(xmlLineInfo, contextStack, previousElementXmlName, elementXmlName, attributes, null);
					}
					else
					{
						// use 'dummy' current XML object (the parent so depth counts are correct and IsTextElement() works)
						currentXmlObject = contextStack.Peek();
					}

					// check context stack depth for emptiness
					if (contextStack.Count <= 0)
					{
						// document element is current element when no context present
						documentXmlObject = currentXmlObject;
					}

					// push current XML object as parent XML object if there are children possible (no empty element)
					if (!isEmptyElement)
						contextStack.Push(currentXmlObject);
				}
				else if (xmlReader.NodeType == XmlNodeType.EndElement) // closing element
				{
					OnlyWhen._DEBUG_ThenPrint(string.Format("end <{0}>", xmlReader.LocalName));

					// create the element XML name
					elementXmlName = new XmlName()
									{
										LocalName = xmlReader.LocalName,
										NamespaceUri = xmlReader.NamespaceURI
									};

					// is this a text element?
					isTextElement = IsTextElement(contextStack, elementXmlName);

					// sanity check
					if (contextStack.Count < 1)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// pop element off stack (unwind)
					contextStack.Pop();

					// clear attribute name
					elementXmlName = null;
				}
			}

			// sanity check
			if (contextStack.Count != 0)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			// ...and I'm spent!
			return documentXmlObject;
		}

		/// <summary>
		/// Deserialize an XML object graph from the specified text reader.
		/// </summary>
		/// <param name="textReader"> The text reader to load. </param>
		/// <returns> An XML object graph. </returns>
		public IXmlObject DeserializeFromXml(TextReader textReader)
		{
			XmlTextReader xmlTextReader;
			IXmlObject document;

			if ((object)textReader == null)
				throw new ArgumentNullException(nameof(textReader));

			// DO NOT USE A USING BLOCK HERE (CALLER OWNS TEXTWRITER) !!!
			xmlTextReader = new XmlTextReader(textReader);
			document = this.DeserializeFromXml(xmlTextReader);
			return document;
		}

		/// <summary>
		/// Private method that processes XML text object deserialization.
		/// </summary>
		/// <param name="contextStack"> The context stack used to manage deserialization. </param>
		/// <param name="textValue"> The string value of the text element. </param>
		/// <param name="xmlName"> The in-effect XML name. </param>
		/// <returns> The created current XML text object. </returns>
		private IXmlTextObject DeserializeFromXmlText(IXmlLineInfo xmlLineInfo, Stack<IXmlObject> contextStack, string textValue, XmlName xmlName)
		{
			IXmlTextObject currentXmlTextObject;
			IXmlObject parentXmlObject;
			Type parentType;
			XmlElementMappingAttribute parentXmlElementMappingAttribute;

			if ((object)contextStack == null)
				throw new ArgumentNullException(nameof(contextStack));

			if ((object)textValue == null)
				throw new ArgumentNullException(nameof(textValue));

			// sanity check
			if (contextStack.Count <= 0)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			// resolve the XML text object
			currentXmlTextObject = this.ResolveXmlTextObject(textValue);

			// sanity check
			if ((object)currentXmlTextObject == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			// grab the parent XML object
			parentXmlObject = contextStack.Peek();

			// sanity check
			if ((object)parentXmlObject == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			// interogate the parent
			parentType = parentXmlObject.GetType();

			// get the parent emlement mapping metadata
			parentXmlElementMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlElementMappingAttribute>(parentType);

			// sanity check
			if ((object)parentXmlElementMappingAttribute == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			// is this a named text element?
			if ((object)xmlName != null)
			{
				// named, thus assign name and do not add to anonymous child element collection
				currentXmlTextObject.Name = xmlName;
			}
			else
			{
				// it is OK to have text element and not allow anonymous children
				if (parentXmlElementMappingAttribute.ChildElementModel != ChildElementModel.Items)
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

				// sanity check
				if ((object)parentXmlObject.Items == null)
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

				// anonymous, thus add to anonymous child element collection
				// dpbullington@gmail.com / 2012-10-29 (Issue #32): no longer need to explicitly assign parent
				// currentXmlTextObject.Parent = parentXmlObject;
				parentXmlObject.Items.Add(currentXmlTextObject);
			}

			return currentXmlTextObject;
		}

		/// <summary>
		/// Registers a known XML object by target type and explicit XML name (local name and namespace URI). This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target type to register. </typeparam>
		/// <param name="xmlName"> The XML name (local name and namespace URI). </param>
		public void RegisterKnownXmlObject<TObject>(XmlName xmlName)
			where TObject : IXmlObject
		{
			Type targetType;

			if ((object)xmlName == null)
				throw new ArgumentNullException(nameof(xmlName));

			targetType = typeof(TObject);

			this.RegisterKnownXmlObject(xmlName, targetType);
		}

		/// <summary>
		/// Registers a known XML object by target type and explicit XML name (local name and namespace URI). This is the non-generic overload.
		/// </summary>
		/// <param name="xmlName"> The XML name (local name and namespace URI). </param>
		/// <param name="targetType"> The target type to register. </param>
		public void RegisterKnownXmlObject(XmlName xmlName, Type targetType)
		{
			if ((object)xmlName == null)
				throw new ArgumentNullException(nameof(xmlName));

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			if (this.KnownXmlObjectTypeRegistrations.ContainsKey(xmlName))
				throw new InvalidOperationException(string.Format("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message. | XML for target type '{0}'.", targetType.FullName));

			if (!typeof(IXmlObject).IsAssignableFrom(targetType))
				throw new InvalidOperationException(string.Format("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message. | '{0}'.", targetType.FullName));

			this.KnownXmlObjectTypeRegistrations.Add(xmlName, targetType);
		}

		/// <summary>
		/// Registers a known XML object by target type and implicit attribute-based XML name (local name and namespace URI). This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target type to register. </typeparam>
		public void RegisterKnownXmlObject<TObject>()
			where TObject : IXmlObject
		{
			Type targetType;

			targetType = typeof(TObject);

			this.RegisterKnownXmlObject(targetType);
		}

		/// <summary>
		/// Registers a known XML object by target type. This is the non-generic overload.
		/// </summary>
		/// <param name="targetType"> The target type to register. </param>
		public void RegisterKnownXmlObject(Type targetType)
		{
			XmlName xmlName;
			XmlElementMappingAttribute xmlElementMappingAttribute;

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			xmlElementMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlElementMappingAttribute>(targetType);

			if ((object)xmlElementMappingAttribute == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			xmlName = new XmlName()
					{
						LocalName = xmlElementMappingAttribute.LocalName,
						NamespaceUri = xmlElementMappingAttribute.NamespaceUri
					};

			this.RegisterKnownXmlObject(xmlName, targetType);
		}

		/// <summary>
		/// Registers a known XML text object by target type. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target type to register. </typeparam>
		public void RegisterKnownXmlTextObject<TObject>()
			where TObject : IXmlTextObject
		{
			Type targetType;

			targetType = typeof(TObject);

			this.RegisterKnownXmlTextObject(targetType);
		}

		/// <summary>
		/// Registers a known XML text object by target type and implicit attribute-based XML name (local name and namespace URI). This is the non-generic overload.
		/// </summary>
		/// <param name="targetType"> The target type to register. </param>
		public void RegisterKnownXmlTextObject(Type targetType)
		{
			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			if ((object)this.KnownXmlTextObjectTypeRegistration != null)
				throw new InvalidOperationException(string.Format("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message. | XML for target type '{0}'.", targetType.FullName));

			if (!typeof(IXmlTextObject).IsAssignableFrom(targetType))
				throw new InvalidOperationException(string.Format("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message. | XML for target type '{0}'.", targetType.FullName));

			this.KnownXmlTextObjectTypeRegistration = targetType;
		}

		/// <summary>
		/// Private method to resolve an XML object by XML name.
		/// </summary>
		/// <param name="xmlName"> The XML name to lookup in the known registrations. </param>
		/// <returns> An IXmlObject instance or null if the XML name is not known. </returns>
		private IXmlObject ResolveXmlObject(XmlName xmlName)
		{
			object value;
			IXmlObject xmlObject;
			Type targetType;

			if ((object)xmlName == null)
				throw new ArgumentNullException(nameof(xmlName));

			if (!this.KnownXmlObjectTypeRegistrations.TryGetValue(xmlName, out targetType))
				return null;

			if ((object)targetType == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			value = Activator.CreateInstance(targetType);

			xmlObject = value as IXmlObject;

			if ((object)xmlObject == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			return xmlObject;
		}

		/// <summary>
		/// Private method to resolve an XML text object.
		/// </summary>
		/// <param name="text"> The string value of the XML text object. </param>
		/// <returns> An IXmlTextObject instance or null if it is not known. </returns>
		private IXmlTextObject ResolveXmlTextObject(string text)
		{
			object value;
			IXmlTextObject xmlTextObject;
			Type targetType;

			if ((object)this.KnownXmlTextObjectTypeRegistration == null)
				return null;

			targetType = this.KnownXmlTextObjectTypeRegistration;

			if ((object)targetType == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			value = Activator.CreateInstance(targetType);

			xmlTextObject = value as IXmlTextObject;

			if ((object)xmlTextObject == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			xmlTextObject.Text = text;

			return xmlTextObject;
		}

		/// <summary>
		/// Serializes an XML object graph to the specified XML file.
		/// </summary>
		/// <param name="document"> The document root XML object. </param>
		/// <param name="fileName"> The XML file to save. </param>
		public void SerializeToXml(IXmlObject document, string fileName)
		{
			if ((object)document == null)
				throw new ArgumentNullException(nameof(document));

			if ((object)fileName == null)
				throw new ArgumentNullException(nameof(fileName));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(fileName))
				throw new ArgumentOutOfRangeException(nameof(fileName));

			using (Stream stream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
				this.SerializeToXml(document, stream);
		}

		/// <summary>
		/// Serializes an XML object graph to the specified stream.
		/// </summary>
		/// <param name="document"> The document root XML object. </param>
		/// <param name="stream"> The stream to save. </param>
		public void SerializeToXml(IXmlObject document, Stream stream)
		{
			XmlTextWriter xmlTextWriter;

			if ((object)document == null)
				throw new ArgumentNullException(nameof(document));

			if ((object)stream == null)
				throw new ArgumentNullException(nameof(stream));

			// DO NOT USE A USING BLOCK HERE (CALLER OWNS STREAM) !!!
			xmlTextWriter = new XmlTextWriter(stream, Encoding.UTF8);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.Indentation = 1;
			xmlTextWriter.IndentChar = '\t';

			this.SerializeToXml(document, xmlTextWriter);
			xmlTextWriter.Flush();
		}

		/// <summary>
		/// Serializes an XML object graph to the specified XML writer.
		/// </summary>
		/// <param name="document"> The document root XML object. </param>
		/// <param name="xmlWriter"> The XML writer to save. </param>
		public void SerializeToXml(IXmlObject document, XmlWriter xmlWriter)
		{
			XmlTextWriter xmlTextWriter;

			if ((object)document == null)
				throw new ArgumentNullException(nameof(document));

			if ((object)xmlWriter == null)
				throw new ArgumentNullException(nameof(xmlWriter));

			xmlTextWriter = xmlWriter as XmlTextWriter;

			if ((object)xmlTextWriter != null &&
				xmlTextWriter.Formatting == Formatting.None)
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.Indentation = 1;
				xmlTextWriter.IndentChar = '\t';
			}

			this.SerializeToXml(xmlWriter, document, null);
		}

		private void SerializeToXml(XmlWriter xmlWriter, IXmlObject currentXmlObject, XmlName overrideXmlName)
		{
			IXmlTextObject currentXmlTextObject;
			Type currentType;
			PropertyInfo[] currentPropertyInfos;
			XmlElementMappingAttribute currentXmlElementMappingAttribute;

			Dictionary<PropertyInfo, XmlAttributeMappingAttribute> currentPropertyToAttributeMappings;
			Dictionary<PropertyInfo, XmlChildElementMappingAttribute> currentPropertyToChildElementMappings;

			XmlAttributeMappingAttribute xmlAttributeMappingAttribute;
			XmlChildElementMappingAttribute xmlChildElementMappingAttribute;

			int attributeCount;
			object ovalue;
			string svalue;

			if ((object)xmlWriter == null)
				throw new ArgumentNullException(nameof(xmlWriter));

			if ((object)currentXmlObject == null)
				throw new ArgumentNullException(nameof(currentXmlObject));

			// is this a text element?
			currentXmlTextObject = currentXmlObject as IXmlTextObject;

			if ((object)currentXmlTextObject != null)
			{
				// write as CDATA if name is invalid (expected)
				if ((object)currentXmlTextObject.Name == null ||
					SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(currentXmlTextObject.Name.LocalName))
				{
					xmlWriter.WriteCData(currentXmlTextObject.Text);
					return;
				}
			}

			// interogate the current XML object
			currentType = currentXmlObject.GetType();
			currentPropertyInfos = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			// sanity check
			if ((object)currentPropertyInfos == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			// get the current mapping metadata
			currentXmlElementMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlElementMappingAttribute>(currentType);

			// sanity check
			if ((object)currentXmlElementMappingAttribute == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			// create current mapping tables for attributes and child elements
			currentPropertyToAttributeMappings = new Dictionary<PropertyInfo, XmlAttributeMappingAttribute>();
			currentPropertyToChildElementMappings = new Dictionary<PropertyInfo, XmlChildElementMappingAttribute>();

			foreach (PropertyInfo currentPropertyInfo in currentPropertyInfos)
			{
				// get potential attribute mapping metadata
				xmlAttributeMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlAttributeMappingAttribute>(currentPropertyInfo);

				// get potential child element mapping metadata
				xmlChildElementMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlChildElementMappingAttribute>(currentPropertyInfo);

				// count what we found; there can only be one
				attributeCount = 0;
				attributeCount += (object)xmlAttributeMappingAttribute == null ? 0 : 1;
				attributeCount += (object)xmlChildElementMappingAttribute == null ? 0 : 1;

				// sanity check
				if (attributeCount > 1)
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

				// append to the correct mapping table
				if ((object)xmlAttributeMappingAttribute != null) // is this an attribute mapping?
					currentPropertyToAttributeMappings.Add(currentPropertyInfo, xmlAttributeMappingAttribute);
				else if ((object)xmlChildElementMappingAttribute != null) // or is this a child element mapping?
					currentPropertyToChildElementMappings.Add(currentPropertyInfo, xmlChildElementMappingAttribute);
			}

			// begin current element
			if ((object)overrideXmlName != null &&
				!SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(overrideXmlName.LocalName)) // overriden element is special case for parent DOT property convention
			{
				// write the start of the element
				xmlWriter.WriteStartElement(overrideXmlName.LocalName, overrideXmlName.NamespaceUri);
			}
			else
			{
				// sanity check
				if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(currentXmlElementMappingAttribute.LocalName))
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

				// write the start of the element
				xmlWriter.WriteStartElement(currentXmlElementMappingAttribute.LocalName, currentXmlElementMappingAttribute.NamespaceUri);
			}

			// iterate over attributes of current element
			if ((object)currentPropertyToAttributeMappings != null)
			{
				foreach (KeyValuePair<PropertyInfo, XmlAttributeMappingAttribute> currentPropertyToAttributeMapping in currentPropertyToAttributeMappings.OrderBy(m => m.Value.Order).ThenBy(m => m.Value.LocalName))
				{
					// sanity check
					if (!currentPropertyToAttributeMapping.Key.CanRead)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// get the strongly-typed value
					if (!SolderFascadeAccessor.ReflectionFascade.GetLogicalPropertyValue(currentXmlObject, currentPropertyToAttributeMapping.Key.Name, out ovalue))
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// convert to loosely-typed formatted string
					svalue = ovalue.SafeToString();

					// write the attribute and value
					xmlWriter.WriteStartAttribute(currentPropertyToAttributeMapping.Value.LocalName, currentPropertyToAttributeMapping.Value.NamespaceUri);
					xmlWriter.WriteString(svalue);
					xmlWriter.WriteEndAttribute();
				}
			}

			if ((object)currentPropertyToChildElementMappings != null)
			{
				// write text elements
				foreach (KeyValuePair<PropertyInfo, XmlChildElementMappingAttribute> currentPropertyToChildElementMapping in currentPropertyToChildElementMappings.Where(m => m.Value.ChildElementType == ChildElementType.TextValue).OrderBy(m => m.Value.Order).ThenBy(m => m.Value.LocalName))
				{
					if (!currentPropertyToChildElementMapping.Key.CanRead)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(currentPropertyToChildElementMapping.Value.LocalName))
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					if (!SolderFascadeAccessor.ReflectionFascade.GetLogicalPropertyValue(currentXmlObject, currentPropertyToChildElementMapping.Key.Name, out ovalue))
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					svalue = ovalue.SafeToString();

					xmlWriter.WriteElementString(string.Empty, currentPropertyToChildElementMapping.Value.LocalName, currentPropertyToChildElementMapping.Value.NamespaceUri, svalue);
				}

				// write child elements
				foreach (KeyValuePair<PropertyInfo, XmlChildElementMappingAttribute> currentPropertyToChildElementMapping in currentPropertyToChildElementMappings.Where(m => m.Value.ChildElementType != ChildElementType.TextValue).OrderBy(m => m.Value.Order).ThenBy(m => m.Value.LocalName))
				{
					IXmlObject childElement;
					XmlName xmlName;

					// sanity check
					if (!currentPropertyToChildElementMapping.Key.CanRead)
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

					// sanity check
					if (!typeof(IXmlObject).IsAssignableFrom(currentPropertyToChildElementMapping.Key.PropertyType))
						throw new InvalidOperationException(string.Format("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message. | '{0}'", currentPropertyToChildElementMapping.Key.PropertyType.FullName));

					// get the XML object property value
					object _out;
					if (!SolderFascadeAccessor.ReflectionFascade.GetLogicalPropertyValue(currentXmlObject, currentPropertyToChildElementMapping.Key.Name, out _out))
						throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");
					childElement = (IXmlObject)_out;

					// write the child element if not null
					if ((object)childElement != null)
					{
						// determine the name convention
						if (currentPropertyToChildElementMapping.Value.ChildElementType == ChildElementType.ParentQualified)
						{
							// parent DOT property convention
							xmlName = new XmlName()
									{
										LocalName = string.Format("{0}.{1}",
											currentXmlElementMappingAttribute.LocalName,
											currentPropertyToChildElementMapping.Value.LocalName),
										NamespaceUri = currentXmlElementMappingAttribute.NamespaceUri
									};
						}
						else if (currentPropertyToChildElementMapping.Value.ChildElementType == ChildElementType.Unqualified)
						{
							// normal convention
							xmlName = new XmlName()
									{
										LocalName = string.Format("{0}",
											currentPropertyToChildElementMapping.Value.LocalName),
										NamespaceUri = currentXmlElementMappingAttribute.NamespaceUri
									};
						}
						else
							throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

						// recursively write the child element and so forth
						this.SerializeToXml(xmlWriter, childElement, xmlName);
					}
				}
			}

			// write anonymous child elements (depending on element model)
			if (currentXmlElementMappingAttribute.ChildElementModel == ChildElementModel.Items &&
				(object)currentXmlObject.Items != null)
			{
				// anonymous 0..n child elements
				foreach (IXmlObject childElement in currentXmlObject.Items)
					this.SerializeToXml(xmlWriter, childElement, null);
			}
			else if (currentXmlElementMappingAttribute.ChildElementModel == ChildElementModel.Content &&
					(object)currentXmlObject.Content != null)
			{
				// anonymous 0..1 child element
				this.SerializeToXml(xmlWriter, currentXmlObject.Content, null);
			}

			// write the end of the (current) element
			xmlWriter.WriteEndElement();
		}

		public void SerializeToXml(IXmlObject document, TextWriter textWriter)
		{
			XmlTextWriter xmlTextWriter;

			if ((object)document == null)
				throw new ArgumentNullException(nameof(document));

			if ((object)textWriter == null)
				throw new ArgumentNullException(nameof(textWriter));

			// DO NOT USE A USING BLOCK HERE (CALLER OWNS TEXTWRITER) !!!
			xmlTextWriter = new XmlTextWriter(textWriter);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.Indentation = 1;
			xmlTextWriter.IndentChar = '\t';

			this.SerializeToXml(document, xmlTextWriter);
			xmlTextWriter.Flush();
		}

		/// <summary>
		/// Unregisters a known XML object by target type. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target type to unregister. </typeparam>
		/// <returns> A value indicating if the registration was present. </returns>
		public bool UnregisterKnownXmlObject<TObject>() where TObject : IXmlObject
		{
			Type targetType;

			targetType = typeof(TObject);

			return this.UnregisterKnownXmlObject(targetType);
		}

		/// <summary>
		/// Unregisters a known XML object by target type. This is the generic overload.
		/// </summary>
		/// <param name="targetType"> The target type to unregister. </param>
		/// <returns> A value indicating if the registration was present. </returns>
		public bool UnregisterKnownXmlObject(Type targetType)
		{
			bool retval;
			XmlName xmlName;
			XmlElementMappingAttribute xmlElementMappingAttribute;

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			xmlElementMappingAttribute = SolderFascadeAccessor.ReflectionFascade.GetOneAttribute<XmlElementMappingAttribute>(targetType);

			if ((object)xmlElementMappingAttribute == null)
				throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message.");

			xmlName = new XmlName()
					{
						LocalName = xmlElementMappingAttribute.LocalName,
						NamespaceUri = xmlElementMappingAttribute.NamespaceUri
					};

			if (retval = this.KnownXmlObjectTypeRegistrations.ContainsKey(xmlName))
				this.KnownXmlObjectTypeRegistrations.Remove(xmlName);

			return retval;
		}

		/// <summary>
		/// Unregisters a known XML text object by target type. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target type to unregister. </typeparam>
		/// <returns> A value indicating if the registration was present. </returns>
		public bool UnregisterKnownXmlTextObject<TObject>() where TObject : IXmlTextObject
		{
			Type targetType;

			targetType = typeof(TObject);

			return this.UnregisterKnownXmlTextObject(targetType);
		}

		/// <summary>
		/// Unregisters a known XML text object by target type. This is the generic overload.
		/// </summary>
		/// <param name="targetType"> The target type to unregister. </param>
		/// <returns> A value indicating if the registration was present. </returns>
		public bool UnregisterKnownXmlTextObject(Type targetType)
		{
			bool retval;

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			if (retval = (object)this.KnownXmlTextObjectTypeRegistration != null)
				this.KnownXmlTextObjectTypeRegistration = null;

			return retval;
		}

		#endregion
	}
}