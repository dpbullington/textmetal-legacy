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


#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public static class XRef
	{
		#region Fields/Constants

		public const string
			NamespaceUri = "urn:schemas-castle-org:xml-reference";

		public const string
			Prefix = "x";

		public static readonly XmlName
			Id = new XmlName("id", NamespaceUri);

		internal static readonly XmlNamespaceAttribute
			Namespace = new XmlNamespaceAttribute(NamespaceUri, Prefix) { Root = true };

		public static readonly XmlName
			Ref = new XmlName("ref", NamespaceUri);

		#endregion

		#region Methods/Operators

		public static string GetId(this IXmlNode node)
		{
			return node.GetAttribute(Id);
		}

		public static string GetReference(this IXmlNode node)
		{
			return node.GetAttribute(Ref);
		}

		public static void SetId(this IXmlCursor node, string id)
		{
			node.SetAttribute(Id, id);
		}

		public static void SetReference(this IXmlCursor cursor, string id)
		{
			cursor.SetAttribute(Ref, id);
		}

		#endregion
	}
}

#endif