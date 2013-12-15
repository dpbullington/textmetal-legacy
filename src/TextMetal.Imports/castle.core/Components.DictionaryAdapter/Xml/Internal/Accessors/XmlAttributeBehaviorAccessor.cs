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

using System.Xml.Serialization;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XmlAttributeBehaviorAccessor : XmlNodeAccessor,
		IConfigurable<XmlAttributeAttribute>
	{
		#region Constructors/Destructors

		public XmlAttributeBehaviorAccessor(string name, Type type, IXmlContext context)
			: base(name, type, context)
		{
			if (this.Serializer.Kind != XmlTypeKind.Simple)
				throw Error.NotSupported();
		}

		#endregion

		#region Fields/Constants

		internal static readonly XmlAccessorFactory<XmlAttributeBehaviorAccessor>
			Factory = (name, type, context) => new XmlAttributeBehaviorAccessor(name, type, context);

		#endregion

		#region Methods/Operators

		public void Configure(XmlAttributeAttribute attribute)
		{
			this.ConfigureLocalName(attribute.AttributeName);
			this.ConfigureNamespaceUri(attribute.Namespace);
		}

		public override void ConfigureNillable(bool nillable)
		{
			// Attributes are never nillable
		}

		public override void ConfigureReference(bool isReference)
		{
			// Attributes cannot store references
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			throw Error.NotSupported();
		}

		public override IXmlCursor SelectCollectionNode(IXmlNode node, bool mutable)
		{
			throw Error.NotSupported();
		}

		public override IXmlCursor SelectPropertyNode(IXmlNode node, bool mutable)
		{
			return node.SelectChildren(this, this.Context, CursorFlags.Attributes.MutableIf(mutable));
		}

		#endregion
	}
}

#endif