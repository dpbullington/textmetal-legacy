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
using System.Linq;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XmlKnownTypeSet : IXmlKnownTypeMap, IEnumerable<IXmlKnownType>
	{
		#region Constructors/Destructors

		public XmlKnownTypeSet(Type defaultType)
		{
			if (defaultType == null)
				throw Error.ArgumentNull("defaultType");

			this.itemsByXmlIdentity = new Dictionary<IXmlIdentity, IXmlKnownType>(XmlIdentityComparer.Instance);
			this.itemsByClrType = new Dictionary<Type, IXmlKnownType>();
			this.defaultType = defaultType;
		}

		#endregion

		#region Fields/Constants

		private static readonly StringComparer
			NameComparer = StringComparer.OrdinalIgnoreCase;

		private static readonly XmlNameComparer
			XsiTypeComparer = XmlNameComparer.Default;

		private readonly Type defaultType;
		private readonly Dictionary<Type, IXmlKnownType> itemsByClrType;
		private readonly Dictionary<IXmlIdentity, IXmlKnownType> itemsByXmlIdentity;

		#endregion

		#region Properties/Indexers/Events

		public IXmlKnownType Default
		{
			get
			{
				IXmlKnownType knownType;
				if (this.defaultType == null || !TryGet(this.defaultType, out knownType))
					throw Error.NoDefaultKnownType();
				return knownType;
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(IXmlKnownType knownType, bool overwrite)
		{
			// All XmlTypes are present here
			if (overwrite || !this.itemsByXmlIdentity.ContainsKey(knownType))
				this.itemsByXmlIdentity[knownType] = knownType;

			// Only contains the default XmlType for each ClrType
			var clrType = knownType.ClrType;
			if (overwrite || !this.itemsByClrType.ContainsKey(clrType))
				this.itemsByClrType[clrType] = knownType;
		}

		public void AddXsiTypeDefaults()
		{
			// If there is only one xsi:type possible for a known local name and namespace URI,
			// add another XmlType to recognize nodes that don't provide the xsi:type.

			var bits = new Dictionary<IXmlKnownType, bool>(
				this.itemsByXmlIdentity.Count,
				XmlKnownTypeNameComparer.Instance);

			foreach (var knownType in this.itemsByXmlIdentity.Values)
			{
				bool bit;
				bits[knownType] = bits.TryGetValue(knownType, out bit)
					? false // another by same name; can't add a default
					: knownType.XsiType != XmlName.Empty; // first   by this name; can   add a default, if not already in default form
			}

			foreach (var pair in bits)
			{
				if (pair.Value)
				{
					var template = pair.Key;
					var knownType = new XmlKnownType(template.Name, XmlName.Empty, template.ClrType);
					this.Add(knownType, true);
				}
			}
		}

		public IEnumerator<IXmlKnownType> GetEnumerator()
		{
			return this.itemsByXmlIdentity.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.itemsByXmlIdentity.Values.GetEnumerator();
		}

		public IXmlKnownType[] ToArray()
		{
			return this.itemsByXmlIdentity.Values.ToArray();
		}

		public bool TryGet(IXmlIdentity xmlIdentity, out IXmlKnownType knownType)
		{
			return this.itemsByXmlIdentity.TryGetValue(xmlIdentity, out knownType);
		}

		public bool TryGet(Type clrType, out IXmlKnownType knownType)
		{
			return this.itemsByClrType.TryGetValue(clrType, out knownType);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private sealed class XmlIdentityComparer : IEqualityComparer<IXmlIdentity>
		{
			#region Constructors/Destructors

			private XmlIdentityComparer()
			{
			}

			#endregion

			#region Fields/Constants

			public static readonly XmlIdentityComparer
				Instance = new XmlIdentityComparer();

			#endregion

			#region Methods/Operators

			public bool Equals(IXmlIdentity x, IXmlIdentity y)
			{
				var nameX = x.Name;
				var nameY = y.Name;

				if (!NameComparer.Equals(nameX.LocalName, nameY.LocalName))
					return false;

				if (!XsiTypeComparer.Equals(x.XsiType, y.XsiType))
					return false;

				return nameX.NamespaceUri == null
						|| nameY.NamespaceUri == null
						|| NameComparer.Equals(nameX.NamespaceUri, nameY.NamespaceUri);
			}

			public int GetHashCode(IXmlIdentity name)
			{
				var code = NameComparer.GetHashCode(name.Name.LocalName);

				if (name.XsiType != XmlName.Empty)
				{
					code = (code << 7 | code >> 25)
							^ XsiTypeComparer.GetHashCode(name.XsiType);
				}

				// DO NOT include NamespaceUri in hash code.
				// That would break 'null means any' behavior.

				return code;
			}

			#endregion
		}

		private sealed class XmlKnownTypeNameComparer : IEqualityComparer<IXmlKnownType>
		{
			#region Constructors/Destructors

			private XmlKnownTypeNameComparer()
			{
			}

			#endregion

			#region Fields/Constants

			public static readonly XmlKnownTypeNameComparer
				Instance = new XmlKnownTypeNameComparer();

			#endregion

			#region Methods/Operators

			public bool Equals(IXmlKnownType knownTypeA, IXmlKnownType knownTypeB)
			{
				return XmlNameComparer.IgnoreCase.Equals(knownTypeA.Name, knownTypeB.Name);
			}

			public int GetHashCode(IXmlKnownType knownType)
			{
				return XmlNameComparer.IgnoreCase.GetHashCode(knownType.Name);
			}

			#endregion
		}

		#endregion
	}
}

#endif