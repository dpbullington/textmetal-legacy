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

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public abstract class XmlAccessor : IXmlPropertyAccessor, IXmlCollectionAccessor
	{
		#region Constructors/Destructors

		protected XmlAccessor(Type clrType, IXmlContext context)
		{
			if (clrType == null)
				throw Error.ArgumentNull("clrType");
			if (context == null)
				throw Error.ArgumentNull("context");

			clrType = clrType.NonNullable();
			this.clrType = clrType;
			this.xsiType = context.GetDefaultXsiType(clrType);
			this.serializer = XmlTypeSerializer.For(clrType);
			this.context = context;
		}

		#endregion

		#region Fields/Constants

		private readonly Type clrType;
		private readonly XmlTypeSerializer serializer;
		private readonly XmlName xsiType;
		private IXmlContext context;
		protected States state;

		#endregion

		#region Properties/Indexers/Events

		public Type ClrType
		{
			get
			{
				return this.clrType;
			}
		}

		public IXmlContext Context
		{
			get
			{
				return this.context;
			}
			protected set
			{
				this.SetContext(value);
			}
		}

		public bool IsCollection
		{
			get
			{
				return this.serializer.Kind == XmlTypeKind.Collection;
			}
		}

		public virtual bool IsIgnored
		{
			get
			{
				return false;
			}
		}

		public bool IsNillable
		{
			get
			{
				return 0 != (this.state & States.Nillable);
			}
		}

		public bool IsReference
		{
			get
			{
				return 0 != (this.state & States.Reference);
			}
		}

		public bool IsVolatile
		{
			get
			{
				return 0 != (this.state & States.Volatile);
			}
		}

		public XmlTypeSerializer Serializer
		{
			get
			{
				return this.serializer;
			}
		}

		public XmlName XsiType
		{
			get
			{
				return this.xsiType;
			}
		}

		#endregion

		#region Methods/Operators

		protected IXmlContext CloneContext()
		{
			if (0 == (this.state & States.ConfiguredContext))
			{
				this.context = this.context.Clone();
				this.state |= States.ConfiguredContext;
			}
			return this.context;
		}

		private void Coerce(IXmlCursor cursor, Type clrType, bool replace)
		{
			if (replace)
			{
				cursor.Remove();
				cursor.MoveNext();
				cursor.Create(this.ClrType);
			}
			else
				cursor.Coerce(clrType);
		}

		public virtual void ConfigureNillable(bool nillable)
		{
			if (nillable)
				this.state |= States.Nillable;
		}

		public virtual void ConfigureReference(bool isReference)
		{
			if (isReference)
				this.state |= States.Reference;
		}

		public void ConfigureVolatile(bool isVolatile)
		{
			if (isVolatile)
				this.state |= States.Volatile;
		}

		public virtual IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return this.GetDefaultCollectionAccessor(itemType);
		}

		public void GetCollectionItems(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, IList values)
		{
			var cursor = this.SelectCollectionItems(parentNode, false);

			while (cursor.MoveNext())
			{
				object value;

				if (this.IsReference)
				{
					IXmlNode node = cursor;
					value = null;
					object token;

					if (references.OnGetStarting(ref node, ref value, out token))
					{
						value = this.serializer.GetValue(node, parentObject, this);
						references.OnGetCompleted(node, value, token);
					}
				}
				else
					value = this.serializer.GetValue(cursor, parentObject, this);
				values.Add(value);
			}
		}

		protected IXmlCollectionAccessor GetDefaultCollectionAccessor(Type itemType)
		{
			var accessor = new XmlDefaultBehaviorAccessor(itemType, this.Context);
			accessor.ConfigureNillable(true);
			accessor.ConfigureReference(this.IsReference);
			return accessor;
		}

		public virtual object GetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, bool orStub)
		{
			if (orStub)
				orStub &= this.serializer.CanGetStub;

			var cursor = this.IsCollection
				? this.SelectCollectionNode(parentNode, orStub)
				: this.SelectPropertyNode(parentNode, orStub);

			return this.GetValue(cursor, parentObject, references, cursor.MoveNext(), orStub);
		}

		public object GetValue(IXmlNode node, IDictionaryAdapter parentObject, XmlReferenceManager references, bool nodeExists, bool orStub)
		{
			object value;

			if ((nodeExists || orStub) && this.IsReference)
			{
				value = null;
				object token;

				if (references.OnGetStarting(ref node, ref value, out token))
				{
					value = this.GetValueCore(node, parentObject, nodeExists, orStub);
					references.OnGetCompleted(node, value, token);
				}
			}
			else
				value = this.GetValueCore(node, parentObject, nodeExists, orStub);
			return value;
		}

		private object GetValueCore(IXmlNode node, IDictionaryAdapter parentObject, bool nodeExists, bool orStub)
		{
			if (nodeExists)
			{
				if (!node.IsNil)
					return this.serializer.GetValue(node, parentObject, this);
				else if (this.IsNillable)
					return null;
			}

			return orStub
				? this.serializer.GetStub(node, parentObject, this)
				: null;
		}

		public virtual bool IsPropertyDefined(IXmlNode parentNode)
		{
			var cursor = this.IsCollection
				? this.SelectCollectionNode(parentNode, false)
				: this.SelectPropertyNode(parentNode, false);

			return cursor.MoveNext();
		}

		public virtual void Prepare()
		{
			// Do nothing
		}

		protected void RemoveCollectionItems(IXmlNode parentNode, XmlReferenceManager references, object value)
		{
			var collection = value as ICollectionProjection;
			if (collection != null)
			{
				collection.Clear();
				return;
			}

			var itemType = this.clrType.GetCollectionItemType();
			var accessor = this.GetCollectionAccessor(itemType);
			var cursor = accessor.SelectCollectionItems(parentNode, true);
			var isReference = this.IsReference;

			var items = value as IEnumerable;
			if (items != null)
			{
				foreach (var item in items)
				{
					if (!cursor.MoveNext())
						break;
					if (isReference)
						references.OnAssigningNull(cursor, item);
				}
			}

			cursor.Reset();
			cursor.RemoveAllNext();
		}

		public virtual IXmlCursor SelectCollectionItems(IXmlNode parentNode, bool mutable)
		{
			throw Error.NotSupported();
		}

		public virtual IXmlCursor SelectCollectionNode(IXmlNode parentNode, bool mutable)
		{
			return this.SelectPropertyNode(parentNode, mutable);
		}

		public virtual IXmlCursor SelectPropertyNode(IXmlNode parentNode, bool mutable)
		{
			throw Error.NotSupported();
		}

		private void SetContext(IXmlContext value)
		{
			if (null == value)
				throw Error.ArgumentNull("value");

			this.context = value;
		}

		public virtual void SetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references,
			object oldValue, ref object value)
		{
			var cursor = this.IsCollection
				? this.SelectCollectionNode(parentNode, true)
				: this.SelectPropertyNode(parentNode, true);

			this.SetValue(cursor, parentObject, references, cursor.MoveNext(), oldValue, ref value);
		}

		public virtual void SetValue(IXmlCursor cursor, IDictionaryAdapter parentObject, XmlReferenceManager references,
			bool hasCurrent, object oldValue, ref object newValue)
		{
			var hasValue = null != newValue;
			var isNillable = this.IsNillable;
			var isReference = this.IsReference;

			var clrType = hasValue
				? newValue.GetComponentType()
				: this.clrType;

			if (hasValue || isNillable)
			{
				if (hasCurrent)
					this.Coerce(cursor, clrType, !hasValue && cursor.IsAttribute); // TODO: Refactor. (NB: && isNillable is emplied)
				else
					cursor.Create(clrType);
			}
			else if (!hasCurrent)
			{
				// No node exists + no value to assign + and not nillable = no work to do
				return;
			}

			object token = null;
			if (isReference)
			{
				if (!references.OnAssigningValue(cursor, oldValue, ref newValue, out token))
					return;
			}

			var givenValue = newValue;

			if (hasValue)
				this.serializer.SetValue(cursor, parentObject, this, oldValue, ref newValue);
			else if (isNillable)
				cursor.IsNil = true;
			else
			{
				cursor.Remove();
				cursor.RemoveAllNext();
			}

			if (isReference)
				references.OnAssignedValue(cursor, givenValue, newValue, token);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		[Flags]
		protected enum States
		{
			Nillable = 0x01, // Set a null value as xsi:nil='true'
			Volatile = 0x02, // Always get value from XML store; don't cache it
			Reference = 0x04, // Participate in reference tracking
			ConfiguredContext = 0x08, // Have created our own IXmlContext instance
			ConfiguredLocalName = 0x10, // The local name    has been configured
			ConfiguredNamespaceUri = 0x20, // The namespace URI has been configured
			ConfiguredKnownTypes = 0x40, // Known types have been configured from attributes
		}

		#endregion
	}
}

#endif