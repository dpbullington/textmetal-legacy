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

using System.Xml.XPath;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
#if !SL3

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XPathReadOnlyCursor : XPathNode, IXmlCursor
	{
		#region Constructors/Destructors

		public XPathReadOnlyCursor(IXmlNode parent, CompiledXPath path,
			IXmlIncludedTypeMap includedTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
			: base(path, namespaces, parent)
		{
			if (parent == null)
				throw Error.ArgumentNull("parent");
			if (path == null)
				throw Error.ArgumentNull("path");
			if (includedTypes == null)
				throw Error.ArgumentNull("includedTypes");

			this.includedTypes = includedTypes;
			this.flags = flags;

			this.Reset();
		}

		#endregion

		#region Fields/Constants

		private readonly CursorFlags flags;
		private readonly IXmlIncludedTypeMap includedTypes;
		private XPathNodeIterator iterator;

		#endregion

		#region Methods/Operators

		public void Coerce(Type type)
		{
			throw Error.CursorNotMutable();
		}

		public void Create(Type type)
		{
			throw Error.CursorNotMutable();
		}

		public void MakeNext(Type type)
		{
			throw Error.CursorNotMutable();
		}

		public bool MoveNext()
		{
			for (;;)
			{
				var hasNext
					= this.iterator != null
					&& this.iterator.MoveNext()
					&& (this.flags.AllowsMultipleItems() || !this.iterator.MoveNext());

				if (!hasNext)
					return this.SetAtEnd();
				if (this.SetAtNext())
					return true;
			}
		}

		public void MoveTo(IXmlNode position)
		{
			var source = position.AsRealizable<XPathNavigator>();
			if (source == null || !source.IsReal)
				throw Error.CursorCannotMoveToGivenNode();

			var positionNode = source.Value;

			this.Reset();

			if (this.iterator != null)
			{
				while (this.iterator.MoveNext())
				{
					if (this.iterator.Current.IsSamePosition(positionNode))
					{
						this.SetAtNext();
						return;
					}
				}
			}

			throw Error.CursorCannotMoveToGivenNode();
		}

		public void MoveToEnd()
		{
			if (this.iterator != null)
			{
				while (this.iterator.MoveNext())
					;
			}
			this.SetAtEnd();
		}

		public void Remove()
		{
			throw Error.CursorNotMutable();
		}

		public void RemoveAllNext()
		{
			throw Error.CursorNotMutable();
		}

		public void Reset()
		{
			var source = this.Parent.RequireRealizable<XPathNavigator>();
			if (source.IsReal)
				this.iterator = source.Value.Select(this.xpath.Path);
		}

		public override IXmlNode Save()
		{
			return new XPathNode(this.node.Clone(), this.type, this.Namespaces);
		}

		private bool SetAtEnd()
		{
			this.node = null;
			this.type = null;
			return false;
		}

		private bool SetAtNext()
		{
			this.node = this.iterator.Current;

			IXmlIncludedType includedType;
			if (!this.includedTypes.TryGet(this.XsiType, out includedType))
				return false;

			this.type = includedType.ClrType;
			return true;
		}

		#endregion
	}
}

#endif
#endif