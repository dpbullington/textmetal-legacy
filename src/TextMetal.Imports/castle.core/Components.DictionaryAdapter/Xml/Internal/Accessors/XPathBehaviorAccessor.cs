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

	public class XPathBehaviorAccessor : XmlAccessor, IXmlIncludedType, IXmlIncludedTypeMap,
		IConfigurable<XPathAttribute>,
		IConfigurable<XPathVariableAttribute>,
		IConfigurable<XPathFunctionAttribute>
	{
		#region Constructors/Destructors

		protected XPathBehaviorAccessor(Type type, IXmlContext context)
			: base(type, context)
		{
			this.includedTypes = new XmlIncludedTypeSet();

			foreach (var includedType in context.GetIncludedTypes(this.ClrType))
				this.includedTypes.Add(includedType);
		}

		#endregion

		#region Fields/Constants

		internal static readonly XmlAccessorFactory<XPathBehaviorAccessor>
			Factory = (name, type, context) => new XPathBehaviorAccessor(type, context);

		private XmlAccessor defaultAccessor;
		private XmlIncludedTypeSet includedTypes;
		private XmlAccessor itemAccessor;
		private CompiledXPath path;

		#endregion

		#region Properties/Indexers/Events

		private bool CreatesAttributes
		{
			get
			{
				var step = this.path.LastStep;
				return step != null && step.IsAttribute;
			}
		}

		IXmlIncludedType IXmlIncludedTypeMap.Default
		{
			get
			{
				return this;
			}
		}

		private bool SelectsNodes
		{
			get
			{
				return this.path.Path.ReturnType == XPathResultType.NodeSet;
			}
		}

		XmlName IXmlIncludedType.XsiType
		{
			get
			{
				return XmlName.Empty;
			}
		}

		#endregion

		#region Methods/Operators

		public void Configure(XPathAttribute attribute)
		{
			if (this.path != null)
				throw Error.AttributeConflict(this.path.Path.Expression);

			this.path = attribute.SetPath;

			if (this.path == attribute.GetPath)
				return;
			else if (this.Serializer.CanGetStub)
				throw Error.SeparateGetterSetterOnComplexType(this.path.Path.Expression);

			this.defaultAccessor = new DefaultAccessor(this, attribute.GetPath);
		}

		public void Configure(XPathVariableAttribute attribute)
		{
			this.CloneContext().AddVariable(attribute);
		}

		public void Configure(XPathFunctionAttribute attribute)
		{
			this.CloneContext().AddFunction(attribute);
		}

		private object Evaluate(IXmlNode node)
		{
			var value = node.Evaluate(this.path);
			return value != null
				? Convert.ChangeType(value, this.ClrType)
				: null;
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return this.itemAccessor ?? (this.itemAccessor = new ItemAccessor(this));
		}

		private object GetDefaultPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, bool orStub)
		{
			return this.defaultAccessor != null
				? this.defaultAccessor.GetPropertyValue(parentNode, parentObject, references, orStub)
				: null;
		}

		public override object GetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, bool orStub)
		{
			return this.GetPropertyValueCore(parentNode, parentObject, references, orStub)
					?? this.GetDefaultPropertyValue(parentNode, parentObject, references, orStub);
		}

		private object GetPropertyValueCore(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, bool orStub)
		{
			return this.SelectsNodes
				? base.GetPropertyValue(parentNode, parentObject, references, orStub)
				: this.Evaluate(parentNode);
		}

		public override bool IsPropertyDefined(IXmlNode parentNode)
		{
			return this.SelectsNodes
					&& base.IsPropertyDefined(parentNode);
		}

		public override void Prepare()
		{
			if (this.CreatesAttributes)
				this.state &= ~States.Nillable;

			this.Context.Enlist(this.path);

			if (this.defaultAccessor != null)
				this.defaultAccessor.Prepare();
		}

		public override IXmlCursor SelectCollectionItems(IXmlNode node, bool create)
		{
			var flags = CursorFlags.AllNodes.MutableIf(create) | CursorFlags.Multiple;
			return node.Select(this.path, this, this.Context, flags);
		}

		public override IXmlCursor SelectCollectionNode(IXmlNode node, bool create)
		{
			return node.SelectSelf(this.ClrType);
		}

		public override IXmlCursor SelectPropertyNode(IXmlNode node, bool create)
		{
			var flags = CursorFlags.AllNodes.MutableIf(create);
			return node.Select(this.path, this, this.Context, flags);
		}

		public override void SetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, object oldValue, ref object value)
		{
			if (this.SelectsNodes)
				base.SetPropertyValue(parentNode, parentObject, references, oldValue, ref value);
			else
				throw Error.XPathNotCreatable(this.path);
		}

		public bool TryGet(XmlName xsiType, out IXmlIncludedType includedType)
		{
			if (xsiType == XmlName.Empty || xsiType == this.XsiType)
				return Try.Success(out includedType, this);

			if (!this.includedTypes.TryGet(xsiType, out includedType))
				return false;

			if (!this.ClrType.IsAssignableFrom(includedType.ClrType))
				return Try.Failure(out includedType);

			return true;
		}

		public bool TryGet(Type clrType, out IXmlIncludedType includedType)
		{
			return clrType == this.ClrType
				? Try.Success(out includedType, this)
				: this.includedTypes.TryGet(clrType, out includedType);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class DefaultAccessor : XPathBehaviorAccessor
		{
			#region Constructors/Destructors

			public DefaultAccessor(XPathBehaviorAccessor parent, CompiledXPath path)
				: base(parent.ClrType, parent.Context)
			{
				this.parent = parent;
				this.path = path;
			}

			#endregion

			#region Fields/Constants

			private readonly XPathBehaviorAccessor parent;

			#endregion

			#region Methods/Operators

			public override void Prepare()
			{
				this.includedTypes = this.parent.includedTypes;
				this.Context = this.parent.Context;

				base.Prepare();
			}

			#endregion
		}

		private class ItemAccessor : XPathBehaviorAccessor
		{
			#region Constructors/Destructors

			public ItemAccessor(XPathBehaviorAccessor parent)
				: base(parent.ClrType.GetCollectionItemType(), parent.Context)
			{
				this.includedTypes = parent.includedTypes;
				this.path = parent.path;

				this.ConfigureNillable(true);
			}

			#endregion

			#region Methods/Operators

			public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
			{
				return this.GetDefaultCollectionAccessor(itemType);
			}

			#endregion
		}

		#endregion
	}
}

#endif
#endif