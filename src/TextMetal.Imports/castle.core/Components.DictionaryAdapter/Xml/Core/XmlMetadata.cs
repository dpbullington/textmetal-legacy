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

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XmlMetadata : IXmlKnownType, IXmlKnownTypeMap, IXmlIncludedType, IXmlIncludedTypeMap
	{
		private readonly Type clrType;
		private readonly bool? qualified;
		private readonly bool? isNullable;
		private readonly bool? isReference;
		private readonly string rootLocalName;
		private readonly string rootNamespaceUri;
		private readonly string childNamespaceUri;
		private readonly string typeLocalName;
		private readonly string typeNamespaceUri;

		private readonly HashSet<string> reservedNamespaceUris;
		private List<Type> pendingIncludes;
		private readonly XmlIncludedTypeSet includedTypes;
		private readonly XmlContext context;
		private readonly DictionaryAdapterMeta source;
#if !SL3
		private readonly CompiledXPath path;
#endif

		public XmlMetadata(DictionaryAdapterMeta meta, IEnumerable<string> reservedNamespaceUris)
		{
			if (meta == null)
				throw Error.ArgumentNull("meta");
			if (reservedNamespaceUris == null)
				throw Error.ArgumentNull("reservedNamespaceUris");

			this.source = meta;
			this.clrType = meta.Type;
			this.context = new XmlContext(this);
			this.includedTypes = new XmlIncludedTypeSet();

			this.reservedNamespaceUris
				= reservedNamespaceUris as HashSet<string>
				?? new HashSet<string>(reservedNamespaceUris);

			var xmlRoot = null as XmlRootAttribute;
			var xmlType = null as XmlTypeAttribute;
			var xmlDefaults = null as XmlDefaultsAttribute;
			var xmlInclude = null as XmlIncludeAttribute;
			var xmlNamespace = null as XmlNamespaceAttribute;
			var reference = null as ReferenceAttribute;
#if !SL3
			var xPath = null as XPathAttribute;
			var xPathVariable = null as XPathVariableAttribute;
			var xPathFunction = null as XPathFunctionAttribute;
#endif
			foreach (var behavior in meta.Behaviors)
			{
				if (TryCast(behavior, ref xmlRoot))
				{
				}
				else if (TryCast(behavior, ref xmlType))
				{
				}
				else if (TryCast(behavior, ref xmlDefaults))
				{
				}
				else if (TryCast(behavior, ref xmlInclude))
					this.AddPendingInclude(xmlInclude);
				else if (TryCast(behavior, ref xmlNamespace))
					this.context.AddNamespace(xmlNamespace);
				else if (TryCast(behavior, ref reference))
				{
				}
#if !SL3
				else if (TryCast(behavior, ref xPath))
				{
				}
				else if (TryCast(behavior, ref xPathVariable))
					this.context.AddVariable(xPathVariable);
				else if (TryCast(behavior, ref xPathFunction))
					this.context.AddFunction(xPathFunction);
#endif
			}

			if (xmlDefaults != null)
			{
				this.qualified = xmlDefaults.Qualified;
				this.isNullable = xmlDefaults.IsNullable;
			}

			if (reference != null)
				this.isReference = true;

			this.typeLocalName = XmlConvert.EncodeLocalName
				(
					(!meta.HasXmlType() ? null : meta.GetXmlType().NonEmpty()) ??
					(xmlType == null ? null : xmlType.TypeName.NonEmpty()) ??
					this.GetDefaultTypeLocalName(this.clrType)
				);

			this.rootLocalName = XmlConvert.EncodeLocalName
				(
					(xmlRoot == null ? null : xmlRoot.ElementName.NonEmpty()) ??
					this.typeLocalName
				);

			this.typeNamespaceUri =
				(
					(xmlType == null ? null : xmlType.Namespace)
					);

			this.rootNamespaceUri =
				(
					(xmlRoot == null ? null : xmlRoot.Namespace)
					);

			this.childNamespaceUri =
				(
					this.typeNamespaceUri ??
					this.rootNamespaceUri
					);

#if !SL3
			if (xPath != null)
			{
				this.path = xPath.GetPath;
				this.path.SetContext(this.context);
			}
#endif
		}

		public Type ClrType
		{
			get
			{
				return this.clrType;
			}
		}

		public bool? Qualified
		{
			get
			{
				return this.qualified;
			}
		}

		public bool? IsNullable
		{
			get
			{
				return this.isNullable;
			}
		}

		public bool? IsReference
		{
			get
			{
				return this.isReference;
			}
		}

		public XmlName Name
		{
			get
			{
				return new XmlName(this.rootLocalName, this.rootNamespaceUri);
			}
		}

		public XmlName XsiType
		{
			get
			{
				return new XmlName(this.typeLocalName, this.typeNamespaceUri);
			}
		}

		XmlName IXmlIdentity.XsiType
		{
			get
			{
				return XmlName.Empty;
			}
		}

		public string ChildNamespaceUri
		{
			get
			{
				return this.childNamespaceUri;
			}
		}

		public IEnumerable<string> ReservedNamespaceUris
		{
			get
			{
				return this.reservedNamespaceUris.ToArray();
			}
		}

		public XmlIncludedTypeSet IncludedTypes
		{
			get
			{
				this.ProcessPendingIncludes();
				return this.includedTypes;
			}
		}

		public IXmlContext Context
		{
			get
			{
				return this.context;
			}
		}

#if !SL3
		public CompiledXPath Path
		{
			get
			{
				return this.path;
			}
		}
#endif

		IXmlKnownType IXmlKnownTypeMap.Default
		{
			get
			{
				return this;
			}
		}

		IXmlIncludedType IXmlIncludedTypeMap.Default
		{
			get
			{
				return this;
			}
		}

		public bool IsReservedNamespaceUri(string namespaceUri)
		{
			return this.reservedNamespaceUris.Contains(namespaceUri);
		}

		public IXmlCursor SelectBase(IXmlNode node) // node is root
		{
#if !SL3
			if (this.path != null)
				return node.Select(this.path, this, this.context, RootFlags);
#endif
			return node.SelectChildren(this, this.context, RootFlags);
		}

		private bool IsMatch(IXmlIdentity xmlIdentity)
		{
			var name = xmlIdentity.Name;

			return NameComparer.Equals(this.rootLocalName, name.LocalName)
					&& (this.rootNamespaceUri == null || NameComparer.Equals(this.rootNamespaceUri, name.NamespaceUri));
		}

		private bool IsMatch(Type clrType)
		{
			return clrType == this.clrType;
		}

		public bool TryGet(IXmlIdentity xmlIdentity, out IXmlKnownType knownType)
		{
			return IsMatch(xmlIdentity)
				? Try.Success(out knownType, this)
				: Try.Failure(out knownType);
		}

		public bool TryGet(Type clrType, out IXmlKnownType knownType)
		{
			return this.IsMatch(clrType)
				? Try.Success(out knownType, this)
				: Try.Failure(out knownType);
		}

		public bool TryGet(XmlName xsiType, out IXmlIncludedType includedType)
		{
			return xsiType == XmlName.Empty || xsiType == this.XsiType
				? Try.Success(out includedType, this)
				: Try.Failure(out includedType);
		}

		public bool TryGet(Type clrType, out IXmlIncludedType includedType)
		{
			return clrType == this.clrType
				? Try.Success(out includedType, this)
				: Try.Failure(out includedType);
		}

		private void AddPendingInclude(XmlIncludeAttribute attribute)
		{
			if (this.pendingIncludes == null)
				this.pendingIncludes = new List<Type>();
			this.pendingIncludes.Add(attribute.Type);
		}

		private void ProcessPendingIncludes()
		{
			var clrTypes = this.pendingIncludes;
			this.pendingIncludes = null;
			if (clrTypes == null)
				return;

			foreach (var clrType in clrTypes)
			{
				var xsiType = this.GetDefaultXsiType(clrType);
				var includedType = new XmlIncludedType(xsiType, clrType);
				this.includedTypes.Add(includedType);
			}
		}

		public XmlName GetDefaultXsiType(Type clrType)
		{
			if (clrType == this.clrType)
				return this.XsiType;

			IXmlIncludedType include;
			if (this.includedTypes.TryGet(clrType, out include))
				return include.XsiType;

			var kind = XmlTypeSerializer.For(clrType).Kind;
			switch (kind)
			{
				case XmlTypeKind.Complex:
					if (!clrType.IsInterface)
						goto default;
					return this.GetXmlMetadata(clrType).XsiType;

				case XmlTypeKind.Collection:
					var itemClrType = clrType.GetCollectionItemType();
					var itemXsiType = this.GetDefaultXsiType(itemClrType);
					return new XmlName("ArrayOf" + itemXsiType.LocalName, null);

				default:
					return new XmlName(clrType.Name, null);
			}
		}

		public IEnumerable<IXmlIncludedType> GetIncludedTypes(Type baseType)
		{
			var queue = new Queue<XmlMetadata>();
			var visited = new HashSet<Type>();
			XmlMetadata metadata;

			visited.Add(baseType);
			if (this.TryGetXmlMetadata(baseType, out metadata))
				queue.Enqueue(metadata);
			metadata = this;

			for (;;)
			{
				foreach (var includedType in metadata.IncludedTypes)
				{
					var clrType = includedType.ClrType;
					var relevant
						= baseType != clrType
						&& baseType.IsAssignableFrom(clrType)
						&& visited.Add(clrType);

					if (!relevant)
						continue;

					yield return includedType;

					if (this.TryGetXmlMetadata(clrType, out metadata))
						queue.Enqueue(metadata);
				}

				if (queue.Count == 0)
					yield break;

				metadata = queue.Dequeue();
			}
		}

		private bool TryGetXmlMetadata(Type clrType, out XmlMetadata metadata)
		{
			var kind = XmlTypeSerializer.For(clrType).Kind;

			return kind == XmlTypeKind.Complex && clrType.IsInterface
				? Try.Success(out metadata, this.GetXmlMetadata(clrType))
				: Try.Failure(out metadata);
		}

		private XmlMetadata GetXmlMetadata(Type clrType)
		{
			return this.source
				.GetAdapterMeta(clrType)
				.GetXmlMeta();
		}

		private string GetDefaultTypeLocalName(Type clrType)
		{
			var name = clrType.Name;
			return IsInterfaceName(name)
				? name.Substring(1)
				: name;
		}

		private static bool IsInterfaceName(string name)
		{
			return name.Length > 1
					&& name[0] == 'I'
					&& char.IsUpper(name, 1);
		}

		private static bool TryCast<T>(object obj, ref T result)
			where T : class
		{
			var value = obj as T;
			if (null == value)
				return false;

			result = value;
			return true;
		}

		protected static readonly StringComparer
			NameComparer = StringComparer.OrdinalIgnoreCase;

		private const CursorFlags RootFlags
			= CursorFlags.Elements
			| CursorFlags.Mutable;
	}
}

#endif