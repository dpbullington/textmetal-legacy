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

	public class SysXmlCursor : SysXmlNode, IXmlCursor
	{
		#region Constructors/Destructors

		public SysXmlCursor(IXmlNode parent, IXmlKnownTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
			: base(namespaces, parent)
		{
			if (null == parent)
				throw Error.ArgumentNull("parent");
			if (null == knownTypes)
				throw Error.ArgumentNull("knownTypes");

			this.knownTypes = knownTypes;
			this.flags = flags;
			this.index = -1;

			var source = parent.RequireRealizable<XmlNode>();
			if (source.IsReal)
				this.node = source.Value;
		}

		#endregion

		#region Fields/Constants

		protected static readonly StringComparer
			DefaultComparer = StringComparer.OrdinalIgnoreCase;

		private readonly CursorFlags flags;
		private readonly IXmlKnownTypeMap knownTypes;
		private int index;
		private State state;

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
				return this.state > State.Initial;
			}
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
				return this.HasCurrent ? base.Name : this.GetEffectiveName(this.knownTypes.Default, this.node);
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

		private bool Advance()
		{
			for (;;)
			{
				switch (this.state)
				{
					case State.Initial:
						return this.AdvanceToFirstElement() || this.AdvanceToFirstAttribute() || this.Fail(State.End);
					case State.Element:
						return this.AdvanceToNextElement() || this.AdvanceToFirstAttribute() || this.Fail(State.End);
					case State.Attribute:
						return this.AdvanceToNextAttribute() || this.Fail(State.End);
					case State.ElementPrimed:
						return this.Succeed(State.Element);
					case State.AttributePrimed:
						return this.Succeed(State.Attribute);
					case State.End:
						return false;
					case State.Empty:
						return false;
				}
			}
		}

		private bool AdvanceAttribute(XmlNode parent)
		{
			var attributes = parent.Attributes;

			for (;;)
			{
				this.index++;
				if (this.index >= attributes.Count)
					return false;

				var attribute = attributes[this.index];
				if (!attribute.IsNamespace())
				{
					this.node = attribute;
					return true;
				}
			}
		}

		private bool AdvanceElement(XmlNode next)
		{
			for (;;)
			{
				if (next == null)
					return false;

				if (next.NodeType == XmlNodeType.Element)
				{
					this.node = next;
					return true;
				}

				next = next.NextSibling;
			}
		}

		protected virtual bool AdvanceToFirstAttribute()
		{
			if (!this.flags.IncludesAttributes() || this.node == null)
				return false;
			if (!this.AdvanceAttribute(this.node))
				return false;
			this.state = State.Attribute;
			return true;
		}

		protected virtual bool AdvanceToFirstElement()
		{
			if (!this.flags.IncludesElements() || this.node == null)
				return false;
			if (!this.AdvanceElement(this.node.FirstChild))
				return false;
			this.state = State.Element;
			return true;
		}

		private bool AdvanceToNextAttribute()
		{
			if (this.AdvanceAttribute(((XmlAttribute)this.node).OwnerElement))
				return true;
			this.MoveToParentOfAttribute();
			return false;
		}

		private bool AdvanceToNextElement()
		{
			if (this.AdvanceElement(this.node.NextSibling))
				return true;
			this.MoveToParentOfElement();
			return false;
		}

		public void Coerce(Type clrType)
		{
			this.RequireCoercible();

			var knownType = this.knownTypes.Require(clrType);

			if (this.IsElement)
				this.CoerceElement(knownType);
			else
				this.CoerceAttribute(knownType);

			this.type = knownType.ClrType;
		}

		private void CoerceAttribute(IXmlKnownType knownType)
		{
			this.RequireNoXsiType(knownType);

			var oldNode = (XmlAttribute)this.node;
			var parent = oldNode.OwnerElement;
			var name = this.GetEffectiveName(knownType, parent);

			if (!XmlNameComparer.Default.Equals(this.Name, name))
			{
				var newNode = this.CreateAttributeCore(parent, name);
				var attributes = parent.Attributes;
				attributes.RemoveNamedItem(newNode.LocalName, newNode.NamespaceURI);
				attributes.InsertBefore(newNode, oldNode);
				attributes.Remove(oldNode);
			}
		}

		private void CoerceElement(IXmlKnownType knownType)
		{
			var oldNode = (XmlElement)this.node;
			var parent = oldNode.ParentNode;
			var name = this.GetEffectiveName(knownType, parent);

			if (!XmlNameComparer.Default.Equals(this.Name, name))
			{
				var newNode = this.CreateElementCore(parent, name);
				parent.ReplaceChild(newNode, oldNode);

				if (knownType.XsiType != XmlName.Empty)
					this.SetXsiType(knownType.XsiType);
			}
			else
				this.SetXsiType(knownType.XsiType);
		}

		public void Create(Type type)
		{
			var knownType = this.knownTypes.Require(type);
			var position = this.RequireCreatable();

			if (this.flags.IncludesElements())
				this.CreateElement(knownType, position);
			else
				this.CreateAttribute(knownType, position);

			this.type = knownType.ClrType;
		}

		private void CreateAttribute(IXmlKnownType knownType, XmlNode position)
		{
			this.RequireNoXsiType(knownType);

			var parent = this.node;
			var name = this.GetEffectiveName(knownType, parent);
			var attribute = this.CreateAttributeCore(parent, name);
			parent.Attributes.InsertBefore(attribute, (XmlAttribute)position);
			this.state = State.Attribute;
		}

		private XmlAttribute CreateAttributeCore(XmlNode parent, XmlName name)
		{
			var document = parent.OwnerDocument ?? (XmlDocument)parent;
			var prefix = this.Namespaces.GetAttributePrefix(this, name.NamespaceUri);
			var attribute = document.CreateAttribute(prefix, name.LocalName, name.NamespaceUri);
			this.node = attribute;
			return attribute;
		}

		private void CreateElement(IXmlKnownType knownType, XmlNode position)
		{
			var parent = this.node;
			var name = this.GetEffectiveName(knownType, parent);
			var element = this.CreateElementCore(parent, name);
			parent.InsertBefore(element, position);
			this.state = State.Element;

			if (knownType.XsiType != XmlName.Empty)
				this.SetXsiType(knownType.XsiType);
		}

		private XmlElement CreateElementCore(XmlNode parent, XmlName name)
		{
			var document = parent.OwnerDocument ?? (XmlDocument)parent;
			var prefix = this.Namespaces.GetElementPrefix(this, name.NamespaceUri);
			var element = document.CreateElement(prefix, name.LocalName, name.NamespaceUri);
			this.node = element;
			return element;
		}

		public override object Evaluate(CompiledXPath path)
		{
			return this.HasCurrent ? base.Evaluate(path) : null;
		}

		private bool Fail(State state)
		{
			this.state = state;
			return false;
		}

		private XmlName GetEffectiveName(IXmlKnownType knownType, XmlNode parent)
		{
			var name = knownType.Name;

			return name.NamespaceUri != null
				? name
				: name.WithNamespaceUri
					(
						parent != null
							? parent.NamespaceURI
							: string.Empty
					);
		}

		private bool IsAtEnd()
		{
			var priorNode = this.node;
			var priorType = this.type;
			var priorState = this.state;
			var priorIndex = this.index;

			var hasNext = this.MoveNextCore();

			this.node = priorNode;
			this.type = priorType;
			this.state = priorState;
			this.index = priorIndex;

			return !hasNext;
		}

		private bool IsMatch()
		{
			IXmlKnownType knownType;
			return this.knownTypes.TryGet(this, out knownType)
				? Try.Success(out this.type, knownType.ClrType)
				: Try.Failure(out this.type);
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
			var hadCurrent = this.HasCurrent;
			var hasCurrent = this.MoveNextCore() &&
							(
								this.flags.AllowsMultipleItems() ||
								this.IsAtEnd()
								);

			if (!hasCurrent && !hadCurrent)
				this.state = State.Empty;

			return hasCurrent;
		}

		private bool MoveNextCore()
		{
			while (this.Advance())
			{
				if (this.IsMatch())
					return true;
			}

			return false;
		}

		public void MoveTo(IXmlNode position)
		{
			var source = position.AsRealizable<XmlNode>();
			if (source == null || !source.IsReal)
				throw Error.CursorCannotMoveToGivenNode();

			IXmlKnownType knownType;
			if (!this.knownTypes.TryGet(position, out knownType))
				throw Error.CursorCannotMoveToGivenNode();

			this.node = source.Value;
			this.type = knownType.ClrType;

			if (this.IsElement)
				this.SetMovedToElement();
			else
				this.SetMovedToAttribute();
		}

		public void MoveToEnd()
		{
			switch (this.state)
			{
				case State.Element:
				case State.ElementPrimed:
					this.MoveToParentOfElement();
					this.state = State.End;
					break;

				case State.Attribute:
				case State.AttributePrimed:
					this.MoveToParentOfAttribute();
					this.state = State.End;
					break;

				case State.Initial:
					this.state = this.IsAtEnd() ? State.Empty : State.End;
					break;
			}
		}

		private void MoveToParentOfAttribute()
		{
			this.node = ((XmlAttribute)this.node).OwnerElement;
		}

		private void MoveToParentOfElement()
		{
			this.node = this.node.ParentNode;
		}

		private void MoveToRealizedParent()
		{
			var parent = this.Parent;
			this.node = parent.AsRealizable<XmlNode>().Value;
			parent.IsNil = false;
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
			if (this.state != State.Empty)
				throw Error.CursorNotInRealizableState();
			if (!this.flags.SupportsMutation())
				throw Error.CursorNotMutable();
			this.Create(this.knownTypes.Default.ClrType);
			this.OnRealized();
		}

		public void Remove()
		{
			this.RequireRemovable();

			var removedNode = this.node;
			var wasElement = this.IsElement;
			this.MoveNext();

			switch (this.state)
			{
				case State.Attribute:
					this.state = State.AttributePrimed;
					break;
				case State.Element:
					this.state = State.ElementPrimed;
					break;
			}

			if (wasElement)
				this.RemoveElement(removedNode);
			else
				this.RemoveAttribute(removedNode);
		}

		public void RemoveAllNext()
		{
			while (this.MoveNext())
				this.Remove();
		}

		private void RemoveAttribute(XmlNode node)
		{
			var attribute = (XmlAttribute)node;
			attribute.OwnerElement.Attributes.Remove(attribute);
		}

		private void RemoveElement(XmlNode node)
		{
			node.ParentNode.RemoveChild(node);
		}

		private void RequireCoercible()
		{
			if (this.state <= State.Initial)
				throw Error.CursorNotInCoercibleState();
		}

		private XmlNode RequireCreatable()
		{
			XmlNode position;
			switch (this.state)
			{
				case State.Element:
					position = this.node;
					this.MoveToParentOfElement();
					break;
				case State.Attribute:
					position = this.node;
					this.MoveToParentOfAttribute();
					break;
				case State.Empty:
					position = null;
					this.MoveToRealizedParent();
					break;
				case State.End:
					position = null;
					break;
				default:
					throw Error.CursorNotInCreatableState();
			}
			return position;
		}

		private void RequireNoXsiType(IXmlKnownType knownType)
		{
			if (knownType.XsiType != XmlName.Empty)
				throw Error.CannotSetAttribute(this);
		}

		private void RequireRemovable()
		{
			if (this.state <= State.Initial)
				throw Error.CursorNotInRemovableState();
		}

		public void Reset()
		{
			this.MoveToEnd();
			this.state = State.Initial;
			this.index = -1;
		}

		public override IXmlNode Save()
		{
			return this.HasCurrent ? new SysXmlNode(this.node, this.type, this.Namespaces) : this;
		}

		private void SetMovedToAttribute()
		{
			this.state = State.Attribute;

			var parent = ((XmlAttribute)this.node).OwnerElement;
			var attributes = parent.Attributes;

			for (this.index = 0; this.index < attributes.Count; this.index++)
			{
				if (attributes[this.index] == this.node)
					break;
			}
		}

		private void SetMovedToElement()
		{
			this.state = State.Element;
			this.index = -1;
		}

		private bool Succeed(State state)
		{
			this.state = state;
			return true;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		protected enum State
		{
			Empty = -4, // After last item, no items were selected
			End = -3, // After last item, 1+ items were selected
			AttributePrimed = -2, // MoveNext will select an attribute (happens after remove)
			ElementPrimed = -1, // MoveNext will select an element   (happens after remove)
			Initial = 0, // Before first item
			Element = 1, // An element   is currently selected
			Attribute = 2 // An attribute is currently selected
		}

		#endregion
	}
}

#endif
#endif