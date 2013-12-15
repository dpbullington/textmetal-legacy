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


#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XmlListSerializer : XmlCollectionSerializer
	{
		#region Constructors/Destructors

		protected XmlListSerializer()
		{
		}

		#endregion

		#region Fields/Constants

		public static readonly XmlListSerializer
			Instance = new XmlListSerializer();

		#endregion

		#region Properties/Indexers/Events

		public override Type ListTypeConstructor
		{
			get
			{
				return typeof(XmlNodeList<>);
			}
		}

		#endregion
	}
}

#endif