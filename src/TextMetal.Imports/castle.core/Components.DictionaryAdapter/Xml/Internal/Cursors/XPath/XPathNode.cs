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
#if !SL3

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XPathNode : XmlNodeBase, IXmlNode, IRealizable<XPathNavigator>
#if !SILVERLIGHT
		, IRealizable<XmlNode>
#endif
	{
		protected XPathNavigator node;
		protected readonly CompiledXPath xpath;

		protected XPathNode(CompiledXPath path, IXmlNamespaceSource namespaces, IXmlNode parent)
			: base(namespaces, parent)
		{
			this.xpath = path;
		}

		public XPathNode(XPathNavigator node, Type type, IXmlNamespaceSource namespaces)
			: this(null, namespaces, null)
		{
			if (node == null)
				throw Error.ArgumentNull("node");
			if (type == null)
				throw Error.ArgumentNull("type");

			this.node = node;
			this.type = type;
		}

		public object UnderlyingObject
		{
			get
			{
				return this.node;
			}
		}

		XPathNavigator IRealizable<XPathNavigator>.Value
		{
			get
			{
				this.Realize();
				return this.node;
			}
		}

#if !SILVERLIGHT
		XmlNode IRealizable<XmlNode>.Value
		{
			get
			{
				this.Realize();
				return (XmlNode)this.node.UnderlyingObject;
			}
		}
#endif

		public override CompiledXPath Path
		{
			get
			{
				return this.xpath;
			}
		}

		public virtual XmlName Name
		{
			get
			{
				return new XmlName(this.node.LocalName, this.node.NamespaceURI);
			}
		}

		public virtual XmlName XsiType
		{
			get
			{
				return this.GetXsiType();
			}
		}

		public virtual bool IsElement
		{
			get
			{
				return this.node.NodeType == XPathNodeType.Element;
			}
		}

		public virtual bool IsAttribute
		{
			get
			{
				return this.node.NodeType == XPathNodeType.Attribute;
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

		public virtual string Value
		{
			get
			{
				return this.node.Value;
			}
			set
			{
				var nil = (value == null);
				this.IsNil = nil;
				if (!nil)
					this.node.SetValue(value);
			}
		}

		public virtual string Xml
		{
			get
			{
				return this.node.OuterXml;
			}
		}

		public string GetAttribute(XmlName name)
		{
			if (!this.IsReal || !this.node.MoveToAttribute(name.LocalName, name.NamespaceUri))
				return null;

			var value = this.node.Value;
			this.node.MoveToParent();
			return string.IsNullOrEmpty(value) ? null : value;
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

			if (this.node.MoveToAttribute(name.LocalName, name.NamespaceUri))
			{
				this.node.SetValue(value);
				this.node.MoveToParent();
			}
			else
			{
				var prefix = this.Namespaces.GetAttributePrefix(this, name.NamespaceUri);
				this.node.CreateAttribute(prefix, name.LocalName, name.NamespaceUri, value);
			}
		}

		private void ClearAttribute(XmlName name)
		{
			if (this.IsReal && this.node.MoveToAttribute(name.LocalName, name.NamespaceUri))
				this.node.DeleteSelf();
		}

		public string LookupPrefix(string namespaceUri)
		{
			return this.node.LookupPrefix(namespaceUri);
		}

		public string LookupNamespaceUri(string prefix)
		{
			return this.node.LookupNamespace(prefix);
		}

		public void DefineNamespace(string prefix, string namespaceUri, bool root)
		{
			var target
				= root ? this.node.GetRootElement()
					: this.IsElement ? this.node
						: this.IsAttribute ? this.node.GetParent()
							: this.node.GetRootElement();

			target.CreateAttribute(Xmlns.Prefix, prefix, Xmlns.NamespaceUri, namespaceUri);
		}

		public bool UnderlyingPositionEquals(IXmlNode node)
		{
#if !SILVERLIGHT
			var sysXmlNode = node.AsRealizable<XmlNode>();
			if (sysXmlNode != null)
			{
				return sysXmlNode.IsReal
						&& sysXmlNode.Value == this.node.UnderlyingObject;
			}
#endif
#if SILVERLIGHT
	// TODO: XNode-based
#endif
			var xPathNode = node.AsRealizable<XPathNavigator>();
			if (xPathNode != null)
			{
				return xPathNode.IsReal
						&& xPathNode.Value.IsSamePosition(this.node);
			}

			return false;
		}

		public virtual IXmlNode Save()
		{
			return this;
		}

		public IXmlCursor SelectSelf(Type clrType)
		{
			return new XmlSelfCursor(this, clrType);
		}

		public IXmlCursor SelectChildren(IXmlKnownTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
#if !SILVERLIGHT
			return new SysXmlCursor(this, knownTypes, namespaces, flags);
#else
	// TODO: XNode-based
#endif
		}

		public IXmlIterator SelectSubtree()
		{
#if !SILVERLIGHT
			return new SysXmlSubtreeIterator(this, this.Namespaces);
#else
	// TODO: XNode-based
#endif
		}

		public IXmlCursor Select(CompiledXPath path, IXmlIncludedTypeMap includedTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			return flags.SupportsMutation()
				? (IXmlCursor)new XPathMutableCursor(this, path, includedTypes, namespaces, flags)
				: (IXmlCursor)new XPathReadOnlyCursor(this, path, includedTypes, namespaces, flags);
		}

		public virtual object Evaluate(CompiledXPath path)
		{
			return this.node.Evaluate(path.Path);
		}

		public virtual XmlReader ReadSubtree()
		{
			return this.node.ReadSubtree();
		}

		public virtual XmlWriter WriteAttributes()
		{
			return this.node.CreateAttributes();
		}

		public virtual XmlWriter WriteChildren()
		{
			return this.node.AppendChild();
		}

		public virtual void Clear()
		{
			this.node.DeleteChildren();
		}
	}
}

#endif
#endif