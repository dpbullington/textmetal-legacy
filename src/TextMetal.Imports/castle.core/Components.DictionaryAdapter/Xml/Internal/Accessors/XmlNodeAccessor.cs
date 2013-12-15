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

using System.Collections;
using System.Collections.Generic;
using System.Xml;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public abstract class XmlNodeAccessor : XmlAccessor, IXmlKnownType, IXmlKnownTypeMap
	{
		#region Constructors/Destructors

		protected XmlNodeAccessor(Type type, IXmlContext context)
			: this(context.GetDefaultXsiType(type).LocalName, type, context)
		{
		}

		protected XmlNodeAccessor(string name, Type type, IXmlContext context)
			: base(type, context)
		{
			if (name == null)
				throw Error.ArgumentNull("name");
			if (name == string.Empty)
				throw Error.InvalidLocalName();

			this.localName = XmlConvert.EncodeLocalName(name);
			this.namespaceUri = context.ChildNamespaceUri;
		}

		#endregion

		#region Fields/Constants

		protected static readonly StringComparer
			NameComparer = StringComparer.OrdinalIgnoreCase;

		private XmlKnownTypeSet knownTypes;
		private string localName;
		private string namespaceUri;

		#endregion

		#region Properties/Indexers/Events

		IXmlKnownType IXmlKnownTypeMap.Default
		{
			get
			{
				return this;
			}
		}

		protected IXmlKnownTypeMap KnownTypes
		{
			get
			{
				if (this.knownTypes != null)
					return this.knownTypes;
				return this;
			}
		}

		public XmlName Name
		{
			get
			{
				return new XmlName(this.localName, this.namespaceUri);
			}
		}

		XmlName IXmlIdentity.XsiType
		{
			get
			{
				return XmlName.Empty;
			}
		}

		#endregion

		#region Methods/Operators

		private void AddKnownType(XmlName name, XmlName xsiType, Type clrType, bool overwrite)
		{
			if (this.knownTypes == null)
			{
				this.knownTypes = new XmlKnownTypeSet(this.ClrType);
				this.AddSelfAsKnownType();
			}
			this.knownTypes.Add(new XmlKnownType(name, xsiType, clrType), overwrite);
		}

		private void AddSelfAsKnownType()
		{
			var mask
				= States.ConfiguredLocalName
				| States.ConfiguredNamespaceUri
				| States.ConfiguredKnownTypes;

			var selfIsKnownType
				= (this.state & mask) != States.ConfiguredKnownTypes;

			if (selfIsKnownType)
			{
				this.knownTypes.Add(new XmlKnownType(this.Name, this.XsiType, this.ClrType), true);
				this.knownTypes.Add(new XmlKnownType(this.Name, XmlName.Empty, this.ClrType), true);
			}
		}

		private void ConfigureDefaultAndIncludedTypes()
		{
			var configuredKnownTypes = this.knownTypes.ToArray();

			this.knownTypes.AddXsiTypeDefaults();

			foreach (var knownType in configuredKnownTypes)
				this.ConfigureIncludedTypes(knownType);
		}

		private void ConfigureField(ref string field, string value, States mask)
		{
			if (string.IsNullOrEmpty(value))
				return;
			if (0 != (this.state & mask))
				throw Error.AttributeConflict(this.localName);
			field = value;
			this.state |= mask;
		}

		private void ConfigureIncludedTypes(IXmlKnownType knownType)
		{
			var includedTypes = this.Context.GetIncludedTypes(knownType.ClrType);

			foreach (var include in includedTypes)
				this.AddKnownType(knownType.Name, include.XsiType, include.ClrType, false);
		}

		protected void ConfigureKnownTypesFromAttributes<T>(IEnumerable<T> attributes, IXmlBehaviorSemantics<T> semantics)
		{
			foreach (var attribute in attributes)
			{
				var clrType = semantics.GetClrType(attribute);
				if (clrType != null)
				{
					var xsiType = this.Context.GetDefaultXsiType(clrType);

					var name = new XmlName(
						semantics.GetLocalName(attribute).NonEmpty() ?? xsiType.LocalName,
						semantics.GetNamespaceUri(attribute) ?? this.namespaceUri);

					this.AddKnownType(name, xsiType, clrType, true);
				}
			}
		}

		protected void ConfigureKnownTypesFromParent(XmlNodeAccessor accessor)
		{
			if (this.knownTypes != null)
				throw Error.AttributeConflict(this.localName);

			this.knownTypes = accessor.knownTypes;
		}

		protected void ConfigureLocalName(string localName)
		{
			this.ConfigureField(ref this.localName, localName, States.ConfiguredLocalName);
		}

		protected void ConfigureNamespaceUri(string namespaceUri)
		{
			this.ConfigureField(ref this.namespaceUri, namespaceUri, States.ConfiguredNamespaceUri);
		}

		protected virtual bool IsMatch(IXmlIdentity xmlIdentity)
		{
			return NameComparer.Equals(this.localName, xmlIdentity.Name.LocalName)
					&& this.IsMatchOnNamespaceUri(xmlIdentity)
					&& this.IsMatchOnXsiType(xmlIdentity);
		}

		protected virtual bool IsMatch(Type clrType)
		{
			return clrType == this.ClrType
					|| (this.Serializer.Kind == XmlTypeKind.Collection
						&& typeof(IEnumerable).IsAssignableFrom(clrType));
		}

		private bool IsMatchOnNamespaceUri(IXmlIdentity xmlIdentity)
		{
			var otherNamespaceUri = xmlIdentity.Name.NamespaceUri;
			if (this.Context.IsReservedNamespaceUri(otherNamespaceUri))
				return NameComparer.Equals(this.namespaceUri, otherNamespaceUri);
			return this.namespaceUri == null
					|| this.ShouldIgnoreAttributeNamespaceUri(xmlIdentity)
					|| NameComparer.Equals(this.namespaceUri, otherNamespaceUri);
		}

		private bool IsMatchOnXsiType(IXmlIdentity xmlIdentity)
		{
			var otherXsiType = xmlIdentity.XsiType;
			return otherXsiType == XmlName.Empty
					|| otherXsiType == this.XsiType;
		}

		public override void Prepare()
		{
			if (this.knownTypes == null)
				this.ConfigureIncludedTypes(this);
			else
				this.ConfigureDefaultAndIncludedTypes();
		}

		private bool ShouldIgnoreAttributeNamespaceUri(IXmlIdentity xmlName)
		{
			var xmlNode = xmlName as IXmlNode;
			return xmlNode != null
					&& xmlNode.IsAttribute
					&& 0 == (this.state & States.ConfiguredNamespaceUri);
		}

		public bool TryGet(IXmlIdentity xmlName, out IXmlKnownType knownType)
		{
			return IsMatch(xmlName)
				? Try.Success(out knownType, this)
				: Try.Failure(out knownType);
		}

		public bool TryGet(Type clrType, out IXmlKnownType knownType)
		{
			return this.IsMatch(clrType)
				? Try.Success(out knownType, this)
				: Try.Failure(out knownType);
		}

		#endregion
	}
}

#endif