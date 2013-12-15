// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Xml;
using System.Xml.XPath;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
#if !SILVERLIGHT

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class SysXmlNode : XmlNodeBase, IXmlNode,
		IRealizable<XmlNode>,
		IRealizable<XPathNavigator>
	{
		#region Constructors/Destructors

		protected SysXmlNode(IXmlNamespaceSource namespaces, IXmlNode parent)
			: base(namespaces, parent)
		{
		}

		public SysXmlNode(XmlNode node, Type type, IXmlNamespaceSource namespaces)
			: base(namespaces, null)
		{
			if (node == null)
				throw Error.ArgumentNull("node");
			if (type == null)
				throw Error.ArgumentNull("type");

			this.node = node;
			this.type = type;
		}

		#endregion

		#region Fields/Constants

		protected XmlNode node;

		#endregion

		#region Properties/Indexers/Events

		public virtual bool IsAttribute
		{
			get
			{
				return this.node.NodeType == XmlNodeType.Attribute;
			}
		}

		public virtual bool IsElement
		{
			get
			{
				return this.node.NodeType == XmlNodeType.Element;
			}
		}

		public virtual bool IsNil
		{
			get
			{
				return this.IsXsiNil();
			}
			set
			{
				this.SetXsiNil(value);
			}
		}

		public virtual XmlName Name
		{
			get
			{
				return new XmlName(this.node.LocalName, this.node.NamespaceURI);
			}
		}

		public object UnderlyingObject
		{
			get
			{
				return this.node;
			}
		}

		XmlNode IRealizable<XmlNode>.Value
		{
			get
			{
				this.Realize();
				return this.node;
			}
		}

		XPathNavigator IRealizable<XPathNavigator>.Value
		{
			get
			{
				this.Realize();
				return this.node.CreateNavigator();
			}
		}

		public virtual string Value
		{
			get
			{
				return this.node.InnerText;
			}
			set
			{
				var nil = (value == null);
				this.IsNil = nil;
				if (!nil)
					this.node.InnerText = value;
			}
		}

		public virtual string Xml
		{
			get
			{
				return this.node.OuterXml;
			}
		}

		public virtual XmlName XsiType
		{
			get
			{
				return this.GetXsiType();
			}
		}

		#endregion

		#region Methods/Operators

		public void Clear()
		{
			if (this.IsElement)
				this.ClearAttributes();
			else if (this.IsAttribute)
			{
				this.Value = string.Empty;
				return;
			}
			this.ClearChildren();
		}

		private void ClearAttribute(XmlName name)
		{
			if (!this.IsReal)
				return;

			var element = this.node as XmlElement;
			if (element == null)
				return;

			element.RemoveAttribute(name.LocalName, name.NamespaceUri);
			return;
		}

		private void ClearAttributes()
		{
			var attributes = this.node.Attributes;
			var count = attributes.Count;
			while (count > 0)
			{
				var attribute = attributes[--count];
				if (!attribute.IsNamespace() && !attribute.IsXsiType())
					attributes.RemoveAt(count);
			}
		}

		private void ClearChildren()
		{
			XmlNode next;
			for (var child = this.node.FirstChild; child != null; child = next)
			{
				next = child.NextSibling;
				this.node.RemoveChild(child);
			}
		}

		public void DefineNamespace(string prefix, string namespaceUri, bool root)
		{
			var target = this.GetNamespaceTargetElement();
			if (target == null)
				throw Error.InvalidOperation();

			if (root)
				target = target.FindRoot();

			target.DefineNamespace(prefix, namespaceUri);
		}

		public virtual object Evaluate(CompiledXPath path)
		{
			return this.node.CreateNavigator().Evaluate(path.Path);
		}

		public string GetAttribute(XmlName name)
		{
			if (!this.IsReal)
				return null;

			var element = this.node as XmlElement;
			if (element == null)
				return null;

			var attribute = element.GetAttributeNode(name.LocalName, name.NamespaceUri);
			if (attribute == null)
				return null;

			var value = attribute.Value;
			if (string.IsNullOrEmpty(value))
				return null;

			return value;
		}

		private XmlElement GetNamespaceTargetElement()
		{
			var element = this.node as XmlElement;
			if (element != null)
				return element;

			var attribute = this.node as XmlAttribute;
			if (attribute != null)
				return attribute.OwnerElement;

			var document = this.node as XmlDocument;
			if (document != null)
				return document.DocumentElement;

			return null;
		}

		public XmlNode GetNode()
		{
			return this.node;
		}

		public string LookupNamespaceUri(string prefix)
		{
			return this.node.GetNamespaceOfPrefix(prefix);
		}

		public string LookupPrefix(string namespaceUri)
		{
			return this.node.GetPrefixOfNamespace(namespaceUri);
		}

		public XmlReader ReadSubtree()
		{
			return this.node.CreateNavigator().ReadSubtree();
		}

		public virtual IXmlNode Save()
		{
			return this;
		}

		public IXmlCursor Select(CompiledXPath path, IXmlIncludedTypeMap includedTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			return flags.SupportsMutation()
				? (IXmlCursor)new XPathMutableCursor(this, path, includedTypes, namespaces, flags)
				: (IXmlCursor)new XPathReadOnlyCursor(this, path, includedTypes, namespaces, flags);
		}

		public IXmlCursor SelectChildren(IXmlKnownTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			return new SysXmlCursor(this, knownTypes, namespaces, flags);
		}

		public IXmlCursor SelectSelf(Type clrType)
		{
			return new XmlSelfCursor(this, clrType);
		}

		public IXmlIterator SelectSubtree()
		{
			return new SysXmlSubtreeIterator(this, this.Namespaces);
		}

		public void SetAttribute(XmlName name, string value)
		{
			if (string.IsNullOrEmpty(value))
				this.ClearAttribute(name);
			else
				this.SetAttributeCore(name, value);
		}

		private void SetAttributeCore(XmlName name, string value)
		{
			if (!this.IsElement)
				throw Error.CannotSetAttribute(this);

			this.Realize();

			var element = this.node as XmlElement;
			if (element == null)
				throw Error.CannotSetAttribute(this);

			var attribute = element.GetAttributeNode(name.LocalName, name.NamespaceUri);
			if (attribute == null)
			{
				var prefix = this.Namespaces.GetAttributePrefix(this, name.NamespaceUri);
				attribute = element.OwnerDocument.CreateAttribute(prefix, name.LocalName, name.NamespaceUri);
				element.SetAttributeNode(attribute);
			}
			attribute.Value = value;
		}

		public bool UnderlyingPositionEquals(IXmlNode node)
		{
			var sysXmlNode = node.AsRealizable<XmlNode>();
			if (sysXmlNode != null)
			{
				return sysXmlNode.IsReal
						&& sysXmlNode.Value == this.node;
			}

			var xPathNode = node.AsRealizable<XPathNavigator>();
			if (xPathNode != null)
			{
				return xPathNode.IsReal
						&& xPathNode.Value.UnderlyingObject == this.node;
			}

			return false;
		}

		public XmlWriter WriteAttributes()
		{
			return this.node.CreateNavigator().CreateAttributes();
		}

		public XmlWriter WriteChildren()
		{
			return this.node.CreateNavigator().AppendChild();
		}

		#endregion
	}
}

#endif
#endif