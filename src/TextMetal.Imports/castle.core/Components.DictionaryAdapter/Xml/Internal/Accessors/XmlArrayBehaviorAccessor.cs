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

using System.Collections.Generic;
using System.Xml.Serialization;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XmlArrayBehaviorAccessor : XmlNodeAccessor,
		IConfigurable<XmlArrayAttribute>,
		IConfigurable<XmlArrayItemAttribute>
	{
		#region Constructors/Destructors

		public XmlArrayBehaviorAccessor(string name, Type type, IXmlContext context)
			: base(name, type, context)
		{
			if (this.Serializer.Kind != XmlTypeKind.Collection)
				throw Error.AttributeConflict(name);

			this.itemAccessor = new ItemAccessor(this.ClrType.GetCollectionItemType(), this);
		}

		#endregion

		#region Fields/Constants

		private const CursorFlags
			CollectionItemFlags = CursorFlags.Elements | CursorFlags.Multiple;

		private const CursorFlags
			PropertyFlags = CursorFlags.Elements;

		internal static readonly XmlAccessorFactory<XmlArrayBehaviorAccessor>
			Factory = (name, type, context) => new XmlArrayBehaviorAccessor(name, type, context);

		private readonly ItemAccessor itemAccessor;

		#endregion

		#region Methods/Operators

		public void Configure(XmlArrayAttribute attribute)
		{
			this.ConfigureLocalName(attribute.ElementName);
			this.ConfigureNamespaceUri(attribute.Namespace);
			this.ConfigureNillable(attribute.IsNullable);
		}

		public void Configure(XmlArrayItemAttribute attribute)
		{
			this.itemAccessor.Configure(attribute);
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return this.itemAccessor;
		}

		public override void Prepare()
		{
			base.Prepare();
			this.itemAccessor.Prepare();
		}

		public override IXmlCursor SelectPropertyNode(IXmlNode node, bool mutable)
		{
			return node.SelectChildren(this, this.Context, PropertyFlags.MutableIf(mutable));
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class ItemAccessor : XmlNodeAccessor,
			IConfigurable<XmlArrayItemAttribute>,
			IXmlBehaviorSemantics<XmlArrayItemAttribute>
		{
			#region Constructors/Destructors

			public ItemAccessor(Type itemClrType, XmlNodeAccessor accessor)
				: base(itemClrType, accessor.Context)
			{
				this.ConfigureNillable(true);
				this.ConfigureReference(accessor.IsReference);
			}

			#endregion

			#region Fields/Constants

			private List<XmlArrayItemAttribute> attributes;

			#endregion

			#region Methods/Operators

			public void Configure(XmlArrayItemAttribute attribute)
			{
				if (attribute.Type == null)
				{
					this.ConfigureLocalName(attribute.ElementName);
					this.ConfigureNamespaceUri(attribute.Namespace);
					this.ConfigureNillable(attribute.IsNullable);
				}
				else
				{
					if (this.attributes == null)
						this.attributes = new List<XmlArrayItemAttribute>();
					this.attributes.Add(attribute);
				}
			}

			public Type GetClrType(XmlArrayItemAttribute attribute)
			{
				return attribute.Type;
			}

			public string GetLocalName(XmlArrayItemAttribute attribute)
			{
				return attribute.ElementName;
			}

			public string GetNamespaceUri(XmlArrayItemAttribute attribute)
			{
				return attribute.Namespace;
			}

			public override void Prepare()
			{
				if (this.attributes != null)
				{
					this.ConfigureKnownTypesFromAttributes(this.attributes, this);
					this.attributes = null;
				}
				base.Prepare();
			}

			public override IXmlCursor SelectCollectionItems(IXmlNode node, bool mutable)
			{
				return node.SelectChildren(this.KnownTypes, this.Context, CollectionItemFlags.MutableIf(mutable));
			}

			#endregion
		}

		#endregion
	}
}

#endif