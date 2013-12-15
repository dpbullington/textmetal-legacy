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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Threading;
using System.Xml;
using System.Xml.Serialization;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XmlSubtreeReader : XmlReader
	{
		private readonly string rootLocalName;
		private readonly string rootNamespaceURI;
		private string underlyingNamespaceURI;
		private XmlReader reader;

		public XmlSubtreeReader(IXmlNode node, XmlRootAttribute root)
			: this(node, root.ElementName, root.Namespace)
		{
		}

		public XmlSubtreeReader(IXmlNode node, string rootLocalName, string rootNamespaceUri)
		{
			if (null == node)
				throw Error.ArgumentNull("node");
			if (null == rootLocalName)
				throw Error.ArgumentNull("rootLocalName");

			this.reader = node.ReadSubtree();
			this.rootLocalName = this.reader.NameTable.Add(rootLocalName);
			this.rootNamespaceURI = rootNamespaceUri ?? string.Empty;
		}

		protected override void Dispose(bool managed)
		{
			try
			{
				if (managed)
					this.DisposeReader();
			}
			finally
			{
				base.Dispose(managed);
			}
		}

		private void DisposeReader()
		{
			IDisposable value = Interlocked.Exchange(ref this.reader, null);
			if (null != value)
				value.Dispose();
		}

		public bool IsDisposed
		{
			get
			{
				return null == this.reader;
			}
		}

		private void RequireNotDisposed()
		{
			if (this.IsDisposed)
				throw Error.ObjectDisposed("XmlSubtreeReader");
		}

		protected XmlReader Reader
		{
			get
			{
				this.RequireNotDisposed();
				return this.reader;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return this.IsDisposed ? ReadState.Closed : this.reader.ReadState;
			}
		}

		public override int Depth
		{
			get
			{
				return this.Reader.Depth;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.Reader.NodeType;
			}
		}

		public bool IsAtRootElement
		{
			get
			{
				this.RequireNotDisposed();
				return
					this.reader.ReadState == ReadState.Interactive &&
					this.reader.Depth == 0 &&
					(
						this.reader.NodeType == XmlNodeType.Element ||
						this.reader.NodeType == XmlNodeType.EndElement
						);
			}
		}

		public override bool EOF
		{
			get
			{
				return this.Reader.EOF;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.Reader.Prefix;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.IsAtRootElement ? this.rootLocalName : this.Reader.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.IsAtRootElement ? this.CaptureNamespaceUri() : this.TranslateNamespaceURI();
			}
		}

		private string CaptureNamespaceUri()
		{
			if (this.underlyingNamespaceURI == null)
				this.underlyingNamespaceURI = this.Reader.NamespaceURI;
			return this.rootNamespaceURI;
		}

		private string TranslateNamespaceURI()
		{
			var actualNamespaceURI = this.Reader.NamespaceURI;
			return actualNamespaceURI == this.underlyingNamespaceURI
				? this.rootNamespaceURI
				: actualNamespaceURI;
		}

#if !DOTNET40
	// Virtual in .NET 4.0, abstract in .NET 3.5
	// Use default implementation from .NET 4.0
		public override bool HasValue
		{
			get { return 0UL != (HasValueMask & (1UL << ((int)NodeType & 31))); }
		}
		private const ulong HasValueMask = 0x0002659CU;
#endif

		public override string Value
		{
			get
			{
				return this.Reader.Value;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.Reader.IsEmptyElement;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return this.Reader.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.Reader.BaseURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.Reader.NameTable;
			}
		}

		public override bool Read()
		{
			return this.Reader.Read();
		}

		public override bool MoveToElement()
		{
			return this.Reader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			return this.Reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return this.Reader.MoveToNextAttribute();
		}

		public override bool MoveToAttribute(string name)
		{
			return this.Reader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return this.Reader.MoveToAttribute(name, ns);
		}

		public override bool ReadAttributeValue()
		{
			return this.Reader.ReadAttributeValue();
		}

		public override string GetAttribute(int i)
		{
			return this.Reader.GetAttribute(i);
		}

		public override string GetAttribute(string name)
		{
			return this.Reader.GetAttribute(name);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			return this.Reader.GetAttribute(name, namespaceURI);
		}

		public override string LookupNamespace(string prefix)
		{
			return this.Reader.LookupNamespace(prefix);
		}

		public override void ResolveEntity()
		{
			this.Reader.ResolveEntity();
		}

		public override void Close()
		{
			if (!this.IsDisposed)
				this.reader.Close();
		}
	}
}

#endif