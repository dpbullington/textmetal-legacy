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

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	public class XmlMetadataBehavior : DictionaryBehaviorAttribute, IDictionaryMetaInitializer
	{
		#region Fields/Constants

		public static readonly XmlMetadataBehavior
			Default = new XmlMetadataBehavior();

		private readonly HashSet<string> reservedNamespaceUris = new HashSet<string>
																{
																	Xmlns.NamespaceUri,
																	Xsi.NamespaceUri,
																	XRef.NamespaceUri
																};

		#endregion

		#region Properties/Indexers/Events

		public IEnumerable<string> ReservedNamespaceUris
		{
			get
			{
				return this.reservedNamespaceUris.ToArray();
			}
		}

		#endregion

		#region Methods/Operators

		public XmlMetadataBehavior AddReservedNamespaceUri(string uri)
		{
			this.reservedNamespaceUris.Add(uri);
			return this;
		}

		void IDictionaryMetaInitializer.Initialize(IDictionaryAdapterFactory factory, DictionaryAdapterMeta meta)
		{
			meta.SetXmlMeta(new XmlMetadata(meta, this.reservedNamespaceUris));
		}

		bool IDictionaryMetaInitializer.ShouldHaveBehavior(object behavior)
		{
			return behavior is XmlDefaultsAttribute
					|| behavior is XmlNamespaceAttribute
					|| behavior is XPathVariableAttribute
					|| behavior is XPathFunctionAttribute;
		}

		#endregion
	}
}

#endif