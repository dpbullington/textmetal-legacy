﻿// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XmlAdapter : DictionaryBehaviorAttribute,
		IDictionaryInitializer,
		IDictionaryPropertyGetter,
		IDictionaryPropertySetter,
		IDictionaryCreateStrategy,
		IDictionaryCopyStrategy,
		IDictionaryReferenceManager,
		IVirtual,
		IXmlNodeSource
	{
		private IXmlNode node;
		private object source;
		private XmlReferenceManager references;
		private XmlMetadata primaryXmlMeta;
		private Dictionary<Type, XmlMetadata> secondaryXmlMetas;
		private readonly bool isRoot;

#if !SILVERLIGHT
		public XmlAdapter()
			: this(new XmlDocument())
		{
		}

		public XmlAdapter(XmlNode node)
		{
			if (node == null)
				throw Error.ArgumentNull("node");

			this.source = node;
			this.isRoot = true;
		}
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlAdapter" /> class
		/// that represents a child object in a larger object graph.
		/// </summary>
		/// <param name="node"> </param>
		/// <param name="references"> </param>
		public XmlAdapter(IXmlNode node, XmlReferenceManager references)
		{
			if (node == null)
				throw Error.ArgumentNull("node");
			if (references == null)
				throw Error.ArgumentNull("references");

			this.node = node;
			this.references = references;
		}

		public bool IsReal
		{
			get
			{
				return this.node != null && this.node.IsReal;
			}
		}

		public IXmlNode Node
		{
			get
			{
				return this.node;
			}
		}

		internal XmlReferenceManager References
		{
			get
			{
				return this.references;
			}
		}

		object IDictionaryCreateStrategy.Create(IDictionaryAdapter parent, Type type, IDictionary dictionary)
		{
#if !SILVERLIGHT
			var adapter = new XmlAdapter(new XmlDocument());
#endif
#if SILVERLIGHT
	// TODO: Create XNode-based XmlAdapter
#endif
			return parent.CreateChildAdapter(type, adapter, dictionary);
		}

		void IDictionaryInitializer.Initialize(IDictionaryAdapter dictionaryAdapter, object[] behaviors)
		{
			var meta = dictionaryAdapter.Meta;

			if (this.primaryXmlMeta == null)
				this.InitializePrimary(meta, dictionaryAdapter);
			else
				this.InitializeSecondary(meta);

			this.InitializeBaseTypes(meta);
			this.InitializeStrategies(dictionaryAdapter);
			this.InitializeReference(dictionaryAdapter);
		}

		private void InitializePrimary(DictionaryAdapterMeta meta, IDictionaryAdapter dictionaryAdapter)
		{
			RequireXmlMeta(meta);
			this.primaryXmlMeta = meta.GetXmlMeta();

			if (this.node == null)
				this.node = this.GetBaseNode();
			if (!this.node.IsReal)
				this.node.Realized += this.HandleNodeRealized;

			if (this.references == null)
				this.references = new XmlReferenceManager(this.node, DefaultXmlReferenceFormat.Instance);
			this.InitializeReference(this);
		}

		private void InitializeSecondary(DictionaryAdapterMeta meta)
		{
			this.AddSecondaryXmlMeta(meta);
		}

		private void InitializeBaseTypes(DictionaryAdapterMeta meta)
		{
			foreach (var type in meta.Type.GetInterfaces())
			{
				var ns = type.Namespace;
				var ignore
					= ns == "Castle.Components.DictionaryAdapter"
					|| ns == "System.ComponentModel";
				if (ignore)
					continue;

				var baseMeta = meta.GetAdapterMeta(type);
				this.AddSecondaryXmlMeta(baseMeta);
			}
		}

		private void InitializeStrategies(IDictionaryAdapter dictionaryAdapter)
		{
			var instance = dictionaryAdapter.This;
			if (instance.CreateStrategy == null)
			{
				instance.CreateStrategy = this;
				instance.AddCopyStrategy(this);
			}
		}

		private void InitializeReference(object value)
		{
			if (this.isRoot)
				// If this is a root XmlAdapter, we must pre-populate the reference manager with
				// this XmlAdapter and its IDictionaryAdapters.  This enables child objects in the
				// graph to reference the root object.
				this.references.Add(this.node, this, value, true);
		}

		private void AddSecondaryXmlMeta(DictionaryAdapterMeta meta)
		{
			if (this.secondaryXmlMetas == null)
				this.secondaryXmlMetas = new Dictionary<Type, XmlMetadata>();
			else if (this.secondaryXmlMetas.ContainsKey(meta.Type))
				return;

			RequireXmlMeta(meta);
			this.secondaryXmlMetas[meta.Type] = meta.GetXmlMeta();
		}

		private static void RequireXmlMeta(DictionaryAdapterMeta meta)
		{
			if (!meta.HasXmlMeta())
				throw Error.XmlMetadataNotAvailable(meta.Type);
		}

		bool IDictionaryCopyStrategy.Copy(IDictionaryAdapter source, IDictionaryAdapter target, ref Func<PropertyDescriptor, bool> selector)
		{
			if (selector == null)
				selector = property => this.HasProperty(property.PropertyName, source);
			return false;
		}

		object IDictionaryPropertyGetter.GetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, object storedValue, PropertyDescriptor property, bool ifExists)
		{
			XmlAccessor accessor;
			if (this.TryGetAccessor(key, property, null != storedValue, out accessor))
			{
				storedValue = accessor.GetPropertyValue(this.node, dictionaryAdapter, this.references, !ifExists);
				if (null != storedValue)
				{
					this.AttachObservers(storedValue, dictionaryAdapter, property);
					dictionaryAdapter.StoreProperty(property, key, storedValue);
				}
			}
			return storedValue;
		}

		bool IDictionaryPropertySetter.SetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, ref object value, PropertyDescriptor property)
		{
			XmlAccessor accessor;
			if (this.TryGetAccessor(key, property, false, out accessor))
			{
				if (value != null && dictionaryAdapter.ShouldClearProperty(property, value))
					value = null;
				var oldValue = dictionaryAdapter.ReadProperty(key);
				accessor.SetPropertyValue(this.node, dictionaryAdapter, this.references, oldValue, ref value);
			}
			return true;
		}

		private static string EnsureKey(string key, PropertyDescriptor property)
		{
			return string.IsNullOrEmpty(key)
				? property.PropertyName
				: key;
		}

		private IXmlNode GetBaseNode()
		{
			var node = this.GetSourceNode();

			if (node.IsElement)
				return node;
			if (node.IsAttribute)
				throw Error.NotSupported();
			// must be root

			var cursor = this.primaryXmlMeta.SelectBase(node);
			return cursor.MoveNext()
				? cursor.Save()
				: cursor;
		}

		private IXmlNode GetSourceNode()
		{
#if !SILVERLIGHT
			var xmlNode = this.source as XmlNode;
			if (xmlNode != null)
				return new SysXmlNode(xmlNode, this.primaryXmlMeta.ClrType, this.primaryXmlMeta.Context);
#endif

			throw Error.NotSupported();
		}

		private bool TryGetAccessor(string key, PropertyDescriptor property, bool requireVolatile, out XmlAccessor accessor)
		{
			accessor = property.HasAccessor()
				? property.GetAccessor()
				: this.CreateAccessor(key, property);

			if (accessor.IsIgnored)
				return Try.Failure(out accessor);
			if (requireVolatile && !accessor.IsVolatile)
				return Try.Failure(out accessor);
			return true;
		}

		private XmlAccessor CreateAccessor(string key, PropertyDescriptor property)
		{
			var accessor = null as XmlAccessor;
			var isVolatile = false;
			var isReference = false;

			if (string.IsNullOrEmpty(key))
				accessor = this.CreateAccessor(key, property, XmlSelfAccessor.Factory);

			foreach (var behavior in property.Annotations)
			{
				if (IsIgnoreBehavior(behavior))
					return XmlIgnoreBehaviorAccessor.Instance;
				else if (IsVolatileBehavior(behavior))
					isVolatile = true;
				else if (IsReferenceBehavior(behavior))
					isReference = true;
				else
					this.TryApplyBehavior(key, property, behavior, ref accessor);
			}

			if (accessor == null)
				accessor = this.CreateAccessor(key, property, XmlDefaultBehaviorAccessor.Factory);

			accessor.ConfigureVolatile(isVolatile);
			accessor.ConfigureReference(isReference);
			accessor.Prepare();
			property.SetAccessor(accessor);
			return accessor;
		}

		private bool TryApplyBehavior(string key, PropertyDescriptor property, object behavior, ref XmlAccessor accessor)
		{
			return
				this.TryApplyBehavior<XmlElementAttribute, XmlElementBehaviorAccessor>
					(key, property, behavior, ref accessor, XmlElementBehaviorAccessor.Factory)
				||
				this.TryApplyBehavior<XmlArrayAttribute, XmlArrayBehaviorAccessor>
					(key, property, behavior, ref accessor, XmlArrayBehaviorAccessor.Factory)
				||
				this.TryApplyBehavior<XmlArrayItemAttribute, XmlArrayBehaviorAccessor>
					(key, property, behavior, ref accessor, XmlArrayBehaviorAccessor.Factory)
				||
				this.TryApplyBehavior<XmlAttributeAttribute, XmlAttributeBehaviorAccessor>
					(key, property, behavior, ref accessor, XmlAttributeBehaviorAccessor.Factory)
#if !SL3
				||
				this.TryApplyBehavior<XPathAttribute, XPathBehaviorAccessor>
					(key, property, behavior, ref accessor, XPathBehaviorAccessor.Factory)
				||
				this.TryApplyBehavior<XPathVariableAttribute, XPathBehaviorAccessor>
					(key, property, behavior, ref accessor, XPathBehaviorAccessor.Factory)
				||
				this.TryApplyBehavior<XPathFunctionAttribute, XPathBehaviorAccessor>
					(key, property, behavior, ref accessor, XPathBehaviorAccessor.Factory)
#endif
				;
		}

		private bool TryApplyBehavior<TBehavior, TAccessor>(string key, PropertyDescriptor property, object behavior,
			ref XmlAccessor accessor, XmlAccessorFactory<TAccessor> factory)
			where TBehavior : class
			where TAccessor : XmlAccessor, IConfigurable<TBehavior>
		{
			var typedBehavior = behavior as TBehavior;
			if (typedBehavior == null)
				return false;

			if (accessor == null)
				accessor = this.CreateAccessor(key, property, factory);

			var typedAccessor = accessor as TAccessor;
			if (typedAccessor == null)
				throw Error.AttributeConflict(key);

			typedAccessor.Configure(typedBehavior);
			return true;
		}

		private TAccessor CreateAccessor<TAccessor>(string key, PropertyDescriptor property, XmlAccessorFactory<TAccessor> factory)
			where TAccessor : XmlAccessor
		{
			var xmlMeta = this.GetXmlMetadata(property.Property.DeclaringType);
			var accessor = factory(key, property.PropertyType, xmlMeta.Context);
			if (xmlMeta.IsNullable.HasValue)
				accessor.ConfigureNillable(xmlMeta.IsNullable.Value);
			if (xmlMeta.IsReference.HasValue)
				accessor.ConfigureReference(xmlMeta.IsReference.Value);
			return accessor;
		}

		private XmlMetadata GetXmlMetadata(Type type)
		{
			if (type == this.primaryXmlMeta.ClrType)
				return this.primaryXmlMeta;

			XmlMetadata xmlMeta;
			if (this.secondaryXmlMetas.TryGetValue(type, out xmlMeta))
				return xmlMeta;

			throw Error.XmlMetadataNotAvailable(type);
		}

		private static bool IsIgnoreBehavior(object behavior)
		{
			return behavior is XmlIgnoreAttribute;
		}

		private static bool IsVolatileBehavior(object behavior)
		{
			return behavior is VolatileAttribute;
		}

		private static bool IsReferenceBehavior(object behavior)
		{
			return behavior is ReferenceAttribute;
		}

		void IVirtual.Realize()
		{
			throw new NotSupportedException("XmlAdapter does not support realization ssvia IVirtual.Realize().");
		}

		public event EventHandler Realized;

		protected virtual void OnRealized()
		{
			if (this.Realized != null)
				this.Realized(this, EventArgs.Empty);
		}

		private void HandleNodeRealized(object sender, EventArgs e)
		{
			this.OnRealized();
		}

		private void AttachObservers(object value, IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
			var bindingList = value as IBindingList;
			if (bindingList != null)
				bindingList.ListChanged += (s, e) => this.HandleListChanged(s, e, dictionaryAdapter, property);
		}

		private void HandleListChanged(object value, ListChangedEventArgs args, IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
			var change = args.ListChangedType;
			var changed
				= change == ListChangedType.ItemAdded
				|| change == ListChangedType.ItemDeleted
				|| change == ListChangedType.ItemMoved
				|| change == ListChangedType.Reset;

			if (changed && dictionaryAdapter.ShouldClearProperty(property, value))
			{
				value = null;
				dictionaryAdapter.SetProperty(property.PropertyName, ref value);
			}
		}

		bool IDictionaryReferenceManager.IsReferenceProperty(IDictionaryAdapter dictionaryAdapter, string propertyName)
		{
			var xmlAdapter = For(dictionaryAdapter, false);
			if (xmlAdapter == null)
				return false;

			var instance = dictionaryAdapter.This;

			PropertyDescriptor property;
			if (!instance.Properties.TryGetValue(propertyName, out property))
				return false;

			var key = property.GetKey(dictionaryAdapter, propertyName, instance.Descriptor);

			XmlAccessor accessor;
			return xmlAdapter.TryGetAccessor(key, property, false, out accessor)
					&& accessor.IsReference;
		}

		bool IDictionaryReferenceManager.TryGetReference(object keyObject, out object inGraphObject)
		{
			return this.references.TryGet(keyObject, out inGraphObject);
		}

		void IDictionaryReferenceManager.AddReference(object keyObject, object relatedObject, bool isInGraph)
		{
			this.references.Add(null, keyObject, relatedObject, isInGraph);
		}

		public override IDictionaryBehavior Copy()
		{
			return null;
		}

		public static XmlAdapter For(object obj)
		{
			return For(obj, true);
		}

		public static XmlAdapter For(object obj, bool required)
		{
			if (obj == null)
			{
				if (!required)
					return null;
				else
					throw Error.ArgumentNull("obj");
			}

			var dictionaryAdapter = obj as IDictionaryAdapter;
			if (dictionaryAdapter == null)
			{
				if (!required)
					return null;
				else
					throw Error.NotDictionaryAdapter("obj");
			}

			var descriptor = dictionaryAdapter.This.Descriptor;
			if (descriptor == null)
			{
				if (!required)
					return null;
				else
					throw Error.NoInstanceDescriptor("obj");
			}

			var getters = descriptor.Getters;
			if (getters == null)
			{
				if (!required)
					return null;
				else
					throw Error.NoXmlAdapter("obj");
			}

			XmlAdapter xmlAdapter;
			foreach (var getter in getters)
			{
				if (null != (xmlAdapter = getter as XmlAdapter))
					return xmlAdapter;
			}

			if (!required)
				return null;
			else
				throw Error.NoXmlAdapter("obj");
		}

		public static bool IsPropertyDefined(string propertyName, IDictionaryAdapter dictionaryAdapter)
		{
			var xmlAdapter = For(dictionaryAdapter, true);
			return xmlAdapter != null
					&& xmlAdapter.HasProperty(propertyName, dictionaryAdapter);
		}

		public bool HasProperty(string propertyName, IDictionaryAdapter dictionaryAdapter)
		{
			var key = dictionaryAdapter.GetKey(propertyName);
			if (key == null)
				return false;

			PropertyDescriptor property;
			XmlAccessor accessor;
			return dictionaryAdapter.This.Properties.TryGetValue(propertyName, out property)
					&& this.TryGetAccessor(key, property, false, out accessor)
					&& accessor.IsPropertyDefined(this.node);
		}
	}
}

#endif