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
using System.Xml.XPath;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
#if !SL3

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	internal class XPathMutableCursor : XPathNode, IXmlCursor
	{
		#region Constructors/Destructors

		public XPathMutableCursor(IXmlNode parent, CompiledXPath path,
			IXmlIncludedTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
			: base(path, namespaces, parent)
		{
			if (null == parent)
				throw Error.ArgumentNull("parent");
			if (null == path)
				throw Error.ArgumentNull("path");
			if (null == knownTypes)
				throw Error.ArgumentNull("knownTypes");
			if (!path.IsCreatable)
				throw Error.XPathNotCreatable(path);

			this.step = path.FirstStep;
			this.knownTypes = knownTypes;
			this.flags = flags;

			var source = parent.RequireRealizable<XPathNavigator>();
			if (source.IsReal)
			{
				this.iterator = new XPathBufferedNodeIterator(
					source.Value.Select(path.FirstStep.Path));
			}
		}

		#endregion

		#region Fields/Constants

		private readonly CursorFlags flags;
		private readonly IXmlIncludedTypeMap knownTypes;
		private int depth;

		private XPathBufferedNodeIterator iterator;
		private CompiledXPathStep step;

		#endregion

		#region Properties/Indexers/Events

		public override event EventHandler Realized;

		public override Type ClrType
		{
			get
			{
				return this.HasCurrent ? base.ClrType : this.knownTypes.Default.ClrType;
			}
		}

		public bool HasCurrent
		{
			get
			{
				return this.depth == this.xpath.Depth;
			}
		}

		public bool HasPartialOrCurrent
		{
			get
			{
				return null != this.node;
			} // (_depth > 0 works also)
		}

		public override bool IsAttribute
		{
			get
			{
				return this.HasCurrent ? base.IsAttribute : !this.flags.IncludesElements();
			}
		}

		public override bool IsElement
		{
			get
			{
				return this.HasCurrent ? base.IsElement : this.flags.IncludesElements();
			}
		}

		public override bool IsNil
		{
			get
			{
				return this.HasCurrent && base.IsNil;
			}
			set
			{
				this.Realize();
				base.IsNil = value;
			}
		}

		public override bool IsReal
		{
			get
			{
				return this.HasCurrent;
			}
		}

		public override XmlName Name
		{
			get
			{
				return this.HasCurrent ? base.Name : XmlName.Empty;
			}
		}

		public override string Value
		{
			get
			{
				return this.HasCurrent ? base.Value : string.Empty;
			}
			set
			{
				base.Value = value;
			} // base sets IsNil, so no need to call Realize() here
		}

		public override string Xml
		{
			get
			{
				return this.HasCurrent ? base.Xml : null;
			}
		}

		public override XmlName XsiType
		{
			get
			{
				return this.HasCurrent ? base.XsiType : this.knownTypes.Default.XsiType;
			}
		}

		#endregion

		#region Methods/Operators

		private void Append()
		{
			this.node = this.Parent.AsRealizable<XPathNavigator>().Value.Clone();
			this.Parent.IsNil = false;
			this.Complete();
		}

		public void Coerce(Type clrType)
		{
			var includedType = this.knownTypes.Require(clrType);
			this.SetXsiType(includedType.XsiType);
			this.type = clrType;
		}

		private void Complete()
		{
			using (var writer = this.CreateWriterForAppend())
				this.WriteNode(this.step, writer);

			var moved = this.step.IsAttribute
				? this.node.MoveToLastAttribute()
				: this.node.MoveToLastChild();
			this.SeekCurrentAfterCreate(moved);
		}

		private bool Consume(XPathNodeIterator iterator, bool multiple)
		{
			var candidate = iterator.Current;
			if (!multiple && iterator.MoveNext())
				return false;

			this.node = candidate;
			this.Descend();
			return true;
		}

		public void Create(Type type)
		{
			if (this.HasCurrent)
				this.Insert();
			else if (this.HasPartialOrCurrent)
				this.Complete();
			else
				this.Append();

			this.Coerce(type);
		}

		private XmlWriter CreateWriterForAppend()
		{
			return this.step.IsAttribute
				? this.node.CreateAttributes()
				: this.node.AppendChild();
		}

		private int Descend()
		{
			this.step = this.step.NextStep;
			return ++this.depth;
		}

		public override object Evaluate(CompiledXPath path)
		{
			return this.HasCurrent ? base.Evaluate(path) : null;
		}

		private void Insert()
		{
			while (--this.depth > 0)
				this.node.MoveToParent();
			this.ResetDepth();

			using (var writer = this.node.InsertBefore())
				this.WriteNode(this.step, writer);

			var moved = this.node.MoveToPrevious();
			this.SeekCurrentAfterCreate(moved);
		}

		public void MakeNext(Type clrType)
		{
			if (this.MoveNext())
				this.Coerce(clrType);
			else
				this.Create(clrType);
		}

		public bool MoveNext()
		{
			this.ResetCurrent();

			for (;;)
			{
				var hasNext
					= this.iterator != null
					&& this.iterator.MoveNext()
					&& this.Consume(this.iterator, this.flags.AllowsMultipleItems());

				if (!hasNext)
					return this.SetAtEnd();
				if (this.SeekCurrent())
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
			while (this.MoveNext())
			{
				if (this.HasCurrent && this.node.IsSamePosition(positionNode))
					return;
			}

			throw Error.CursorCannotMoveToGivenNode();
		}

		public void MoveToEnd()
		{
			this.ResetCurrent();
			this.iterator.MoveToEnd();
		}

		protected virtual void OnRealized()
		{
			if (this.Realized != null)
				this.Realized(this, EventArgs.Empty);
		}

		protected override void Realize()
		{
			if (this.HasCurrent)
				return;
			if (!(this.iterator == null || this.iterator.IsEmpty || this.HasPartialOrCurrent))
				throw Error.CursorNotInRealizableState();
			this.Create(this.knownTypes.Default.ClrType);
			this.OnRealized();
		}

		public void Remove()
		{
			this.RequireRemovable();

			var name = XmlName.Empty;

			if (!this.HasCurrent)
			{
				var namespaceUri = this.LookupNamespaceUri(this.step.Prefix) ?? this.node.NamespaceURI;
				name = new XmlName(this.step.LocalName, namespaceUri);
			}

			do
			{
				if (this.node.MoveToChild(name.LocalName, name.NamespaceUri))
					break;

				name = new XmlName(this.node.LocalName, this.node.NamespaceURI);
				this.node.DeleteSelf();
				this.depth--;
			}
			while (this.depth > 0);

			this.ResetCurrent();
		}

		public void RemoveAllNext()
		{
			while (this.MoveNext())
				this.Remove();
		}

		private void RequireMoved(bool result)
		{
			if (!result)
				throw Error.XPathNavigationFailed(this.step.Path);
		}

		private void RequireRemovable()
		{
			if (!this.HasPartialOrCurrent)
				throw Error.CursorNotInRemovableState();
		}

		public void Reset()
		{
			this.ResetCurrent();
			this.iterator.Reset();
		}

		private void ResetCurrent()
		{
			this.node = null;
			this.type = null;
			this.ResetDepth();
		}

		private void ResetDepth()
		{
			this.step = this.xpath.FirstStep;
			this.depth = 0;
		}

		public override IXmlNode Save()
		{
			return this.HasCurrent ? new XPathNode(this.node.Clone(), this.type, this.Namespaces) : this;
		}

		private bool SeekCurrent()
		{
			while (this.depth < this.xpath.Depth)
			{
				var iterator = this.node.Select(this.step.Path);
				if (!iterator.MoveNext())
					return true; // Sought as far as possible
				if (!this.Consume(iterator, false))
					return false; // Problem: found multiple nodes
			}

			IXmlIncludedType includedType;
			if (!this.knownTypes.TryGet(this.XsiType, out includedType))
				return false; // Problem: unrecognized xsi:type

			this.type = includedType.ClrType;
			return true; // Sought all the way
		}

		private void SeekCurrentAfterCreate(bool moved)
		{
			this.RequireMoved(moved);
			if (this.Descend() == this.xpath.Depth)
				return;

			do
			{
				moved = this.step.IsAttribute
					? this.node.MoveToFirstAttribute()
					: this.node.MoveToFirstChild();
				this.RequireMoved(moved);
			}
			while (this.Descend() < this.xpath.Depth);
		}

		private bool SetAtEnd()
		{
			this.ResetCurrent();
			return false;
		}

		private void WriteAttribute(CompiledXPathNode node, XmlWriter writer)
		{
			writer.WriteStartAttribute(node.Prefix, node.LocalName, null);
			this.WriteValue(node, writer);
			writer.WriteEndAttribute();
		}

		private void WriteComplexElement(CompiledXPathNode node, XmlWriter writer)
		{
			writer.WriteStartElement(node.Prefix, node.LocalName, null);
			this.WriteSubnodes(node, writer, true);
			this.WriteSubnodes(node, writer, false);
			writer.WriteEndElement();
		}

		private void WriteNode(CompiledXPathNode node, XmlWriter writer)
		{
			if (node.IsAttribute)
				this.WriteAttribute(node, writer);
			else if (node.IsSimple)
				this.WriteSimpleElement(node, writer);
			else
				this.WriteComplexElement(node, writer);
		}

		private void WriteSimpleElement(CompiledXPathNode node, XmlWriter writer)
		{
			writer.WriteStartElement(node.Prefix, node.LocalName, null);
			this.WriteValue(node, writer);
			writer.WriteEndElement();
		}

		private void WriteSubnodes(CompiledXPathNode parent, XmlWriter writer, bool attributes)
		{
			var next = parent.NextNode;
			if (next != null && next.IsAttribute == attributes)
				this.WriteNode(next, writer);

			foreach (var node in parent.Dependencies)
			{
				if (node.IsAttribute == attributes)
					this.WriteNode(node, writer);
			}
		}

		private void WriteValue(CompiledXPathNode node, XmlWriter writer)
		{
			if (node.Value == null)
				return;

			var value = this.Parent.AsRealizable<XPathNavigator>().Value.Evaluate(node.Value);
			writer.WriteValue(value);
		}

		#endregion
	}
}

#endif
#endif