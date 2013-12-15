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

	public class XmlDefaultSerializer : XmlTypeSerializer
	{
		#region Constructors/Destructors

		public XmlDefaultSerializer(Type type)
		{
			this.serializer = new XmlSerializer(type, Root);
		}

		#endregion

		#region Fields/Constants

		public static readonly XmlRootAttribute
			Root = new XmlRootAttribute
					{
						ElementName = "Root",
						Namespace = string.Empty
					};

		private readonly XmlSerializer serializer;

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
			using (var reader = new XmlSubtreeReader(node, Root))
			{
				return this.serializer.CanDeserialize(reader)
					? this.serializer.Deserialize(reader)
					: null;
			}
		}

		public override void SetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor, object oldValue, ref object value)
		{
			using (var writer = new XmlSubtreeWriter(node))
				this.serializer.Serialize(writer, value);
		}

		#endregion
	}
}

#endif