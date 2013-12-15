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

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XmlSelfCursor : IXmlCursor
	{
		private readonly IXmlNode node;
		private readonly Type clrType;
		private int position;

		public XmlSelfCursor(IXmlNode node, Type clrType)
		{
			this.node = node;
			this.clrType = clrType;
			this.Reset();
		}

		public CursorFlags Flags
		{
			get
			{
				return this.node.IsAttribute ? CursorFlags.Attributes : CursorFlags.Elements;
			}
		}

#if !SL3
		public CompiledXPath Path
		{
			get
			{
				return this.node.Path;
			}
		}
#endif

		public XmlName Name
		{
			get
			{
				return this.node.Name;
			}
		}

		public XmlName XsiType
		{
			get
			{
				return this.node.XsiType;
			}
		}

		public Type ClrType
		{
			get
			{
				return this.clrType ?? this.node.ClrType;
			}
		}

		public bool IsReal
		{
			get
			{
				return this.node.IsReal;
			}
		}

		public bool IsElement
		{
			get
			{
				return this.node.IsElement;
			}
		}

		public bool IsAttribute
		{
			get
			{
				return this.node.IsAttribute;
			}
		}

		public bool IsNil
		{
			get
			{
				return this.node.IsNil;
			}
			set
			{
				throw Error.NotSupported();
			}
		}

		public string Value
		{
			get
			{
				return this.node.Value;
			}
			set
			{
				this.node.Value = value;
			}
		}

		public string Xml
		{
			get
			{
				return this.node.Xml;
			}
		}

		public IXmlNode Parent
		{
			get
			{
				return this.node.Parent;
			}
		}

		public IXmlNamespaceSource Namespaces
		{
			get
			{
				return this.node.Namespaces;
			}
		}

		public object UnderlyingObject
		{
			get
			{
				return this.node.UnderlyingObject;
			}
		}

		public bool UnderlyingPositionEquals(IXmlNode node)
		{
			return this.node.UnderlyingPositionEquals(node);
		}

		public IRealizable<T> AsRealizable<T>()
		{
			return this.node.AsRealizable<T>();
		}

		public void Realize()
		{
			this.node.Realize();
		}

		public event EventHandler Realized
		{
			add
			{
				this.node.Realized += value;
			}
			remove
			{
				this.node.Realized -= value;
			}
		}

		public string GetAttribute(XmlName name)
		{
			return this.node.GetAttribute(name);
		}

		public void SetAttribute(XmlName name, string value)
		{
			this.node.SetAttribute(name, value);
		}

		public string LookupPrefix(string namespaceUri)
		{
			return this.node.LookupPrefix(namespaceUri);
		}

		public string LookupNamespaceUri(string prefix)
		{
			return this.node.LookupNamespaceUri(prefix);
		}

		public void DefineNamespace(string prefix, string namespaceUri, bool root)
		{
			this.node.DefineNamespace(prefix, namespaceUri, root);
		}

		public bool MoveNext()
		{
			return 0 == ++this.position;
		}

		public void MoveToEnd()
		{
			this.position = 1;
		}

		public void Reset()
		{
			this.position = -1;
		}

		public void MoveTo(IXmlNode position)
		{
			if (position != this.node)
				throw Error.NotSupported();
		}

		public IXmlNode Save()
		{
			return this.position == 0
				? new XmlSelfCursor(this.node.Save(), this.clrType) { position = 0 }
				: this;
		}

		public IXmlCursor SelectSelf(Type clrType)
		{
			return new XmlSelfCursor(this.node, clrType);
		}

		public IXmlCursor SelectChildren(IXmlKnownTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			return this.node.SelectChildren(knownTypes, namespaces, flags);
		}

		public IXmlIterator SelectSubtree()
		{
			return this.node.SelectSubtree();
		}

#if !SL3
		public IXmlCursor Select(CompiledXPath path, IXmlIncludedTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			return this.node.Select(path, knownTypes, namespaces, flags);
		}

		public object Evaluate(CompiledXPath path)
		{
			return this.node.Evaluate(path);
		}
#endif

		public XmlReader ReadSubtree()
		{
			return this.node.ReadSubtree();
		}

		public XmlWriter WriteAttributes()
		{
			return this.node.WriteAttributes();
		}

		public XmlWriter WriteChildren()
		{
			return this.node.WriteChildren();
		}

		public void MakeNext(Type type)
		{
			if (!this.MoveNext())
				throw Error.NotSupported();
		}

		public void Create(Type type)
		{
			throw Error.NotSupported();
		}

		public void Coerce(Type type)
		{
			// Do nothing
		}

		public void Clear()
		{
			this.node.Clear();
		}

		public void Remove()
		{
			// Do nothing
		}

		public void RemoveAllNext()
		{
			// Do nothing
		}
	}
}

#endif