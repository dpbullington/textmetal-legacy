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

using System.Xml;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
#if !SILVERLIGHT

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class SysXmlSubtreeIterator : SysXmlNode, IXmlIterator
	{
		#region Constructors/Destructors

		public SysXmlSubtreeIterator(IXmlNode parent, IXmlNamespaceSource namespaces)
			: base(namespaces, parent)
		{
			if (null == parent)
				throw Error.ArgumentNull("parent");

			var source = parent.RequireRealizable<XmlNode>();
			if (source.IsReal)
				this.node = source.Value;

			this.type = typeof(object);
		}

		#endregion

		#region Fields/Constants

		private State state;

		#endregion

		#region Methods/Operators

		public bool MoveNext()
		{
			switch (this.state)
			{
				case State.Initial:
					return this.MoveToInitial();
				case State.Current:
					return this.MoveToSubsequent();
				default:
					return false;
			}
		}

		private bool MoveToElement(XmlNode node)
		{
			for (; node != null; node = node.NextSibling)
			{
				if (node.NodeType == XmlNodeType.Element)
					return this.SetNext(node);
			}

			return false;
		}

		private bool MoveToInitial()
		{
			if (this.node == null)
				return false;

			this.state = State.Current;
			return true;
		}

		private bool MoveToSubsequent()
		{
			if (this.MoveToElement(this.node.FirstChild))
				return true;

			for (; this.node != null; this.node = this.node.ParentNode)
			{
				if (this.MoveToElement(this.node.NextSibling))
					return true;
			}

			this.state = State.End;
			return false;
		}

		public override IXmlNode Save()
		{
			return new SysXmlNode(this.node, this.type, this.Namespaces);
		}

		private bool SetNext(XmlNode node)
		{
			this.node = node;
			return true;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private enum State
		{
			Initial,
			Current,
			End
		}

		#endregion
	}
}

#endif
#endif