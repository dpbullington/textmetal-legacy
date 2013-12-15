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

	public class XmlElementBehaviorAccessor : XmlNodeAccessor,
		IConfigurable<XmlElementAttribute>,
		IXmlBehaviorSemantics<XmlElementAttribute>
	{
		#region Constructors/Destructors

		public XmlElementBehaviorAccessor(string name, Type type, IXmlContext context)
			: base(name, type, context)
		{
		}

		#endregion

		#region Fields/Constants

		internal static readonly XmlAccessorFactory<XmlElementBehaviorAccessor>
			Factory = (name, type, context) => new XmlElementBehaviorAccessor(name, type, context);

		private List<XmlElementAttribute> attributes;
		private ItemAccessor itemAccessor;

		#endregion

		#region Methods/Operators

		public void Configure(XmlElementAttribute attribute)
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
					this.attributes = new List<XmlElementAttribute>();
				this.attributes.Add(attribute);
			}
		}

		public Type GetClrType(XmlElementAttribute attribute)
		{
			return attribute.Type;
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return this.itemAccessor ?? (this.itemAccessor = new ItemAccessor(this));
		}

		public string GetLocalName(XmlElementAttribute attribute)
		{
			return attribute.ElementName;
		}

		public string GetNamespaceUri(XmlElementAttribute attribute)
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

		public override IXmlCursor SelectCollectionNode(IXmlNode node, bool mutable)
		{
			return node.SelectSelf(this.ClrType);
		}

		public override IXmlCursor SelectPropertyNode(IXmlNode node, bool mutable)
		{
			return node.SelectChildren(this.KnownTypes, this.Context, CursorFlags.Elements.MutableIf(mutable));
		}

		public override void SetValue(IXmlCursor cursor, IDictionaryAdapter parentObject, XmlReferenceManager references,
			bool hasCurrent, object oldValue, ref object newValue)
		{
			if (newValue == null && this.IsCollection)
				base.RemoveCollectionItems(cursor, references, oldValue);
			else
				base.SetValue(cursor, parentObject, references, hasCurrent, oldValue, ref newValue);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class ItemAccessor : XmlNodeAccessor
		{
			#region Constructors/Destructors

			public ItemAccessor(XmlNodeAccessor parent)
				: base(parent.ClrType.GetCollectionItemType(), parent.Context)
			{
				this.ConfigureLocalName(parent.Name.LocalName);
				this.ConfigureNamespaceUri(parent.Name.NamespaceUri);
				this.ConfigureNillable(parent.IsNillable);
				this.ConfigureReference(parent.IsReference);
				this.ConfigureKnownTypesFromParent(parent);
			}

			#endregion

			#region Methods/Operators

			public override void Prepare()
			{
				// Don't prepare; parent already did it
			}

			public override IXmlCursor SelectCollectionItems(IXmlNode node, bool mutable)
			{
				return node.SelectChildren(this.KnownTypes, this.Context, CursorFlags.Elements.MutableIf(mutable) | CursorFlags.Multiple);
			}

			#endregion
		}

		#endregion
	}
}

#endif