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

using System.Xml;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
#if !SILVERLIGHT

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XmlXmlNodeSerializer : XmlTypeSerializer
	{
		#region Constructors/Destructors

		private XmlXmlNodeSerializer()
		{
		}

		#endregion

		#region Fields/Constants

		public static readonly XmlXmlNodeSerializer
			Instance = new XmlXmlNodeSerializer();

		#endregion

		#region Properties/Indexers/Events

		public override XmlTypeKind Kind
		{
			get
			{
				return XmlTypeKind.Complex;
			}
		}

		#endregion

		#region Methods/Operators

		public override object GetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			var source = node.AsRealizable<XmlNode>();

			return (source != null && source.IsReal)
				? source.Value
				: null;
		}

		public override void SetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor, object oldValue, ref object value)
		{
			var newNode = (XmlNode)value;

			using (var writer = new XmlSubtreeWriter(node))
				newNode.WriteTo(writer);
		}

		#endregion
	}
}

#endif
#endif