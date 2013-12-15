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

using System.Collections.Generic;

using Castle.Core;
using Castle.Core.Internal;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class XmlReferenceManager
	{
		#region Constructors/Destructors

		public XmlReferenceManager(IXmlNode root, IXmlReferenceFormat format)
		{
			this.entriesById = new Dictionary<int, Entry>();
			this.entriesByValue = new WeakKeyDictionary<object, Entry>(ReferenceEqualityComparer<object>.Instance);
			this.format = format;
			this.nextId = 1;

			this.Populate(root);
		}

		#endregion

		#region Fields/Constants

		private static readonly object
			CreateEntryToken = new object();

		private static readonly Type
			StringType = typeof(string);

		private readonly Dictionary<int, Entry> entriesById;
		private readonly WeakKeyDictionary<object, Entry> entriesByValue;
		private readonly IXmlReferenceFormat format;
		private int nextId;

		#endregion

		#region Methods/Operators

		private static Exception IdNotFoundError(int id)
		{
			var message = string.Format
				(
					"The given ID ({0}) was not present in the underlying data.",
					id
				);
			return new KeyNotFoundException(message);
		}

		private static IXmlNode RedirectNode(ref IXmlNode node, Entry entry)
		{
			var cursor = entry.Node.SelectSelf(node.ClrType);
			cursor.MoveNext();
			return node = cursor;
		}

		private static void SetNotInGraph(Entry entry, object value)
		{
			var xmlAdapter = XmlAdapter.For(value, false);

			SetNotInGraphCore(entry, value);

			if (xmlAdapter != null)
				SetNotInGraphCore(entry, xmlAdapter);
		}

		private static void SetNotInGraphCore(Entry entry, object value)
		{
			var values = entry.Values;
			for (int index = 0; index < values.Count; index++)
			{
				var item = values[index];
				var candidate = item.Value.Target;

				if (ReferenceEquals(candidate, value))
				{
					item = new EntryValue(item.Type, item.Value, false);
					values[index] = item;
					return;
				}
			}
		}

		private static bool ShouldExclude(Type type)
		{
			return type.IsValueType
					|| type == StringType;
		}

		public void Add(IXmlNode node, object keyValue, object newValue, bool isInGraph)
		{
			if (keyValue == null)
				throw Error.ArgumentNull("keyValue");
			if (newValue == null)
				throw Error.ArgumentNull("newValue");

			var type = newValue.GetComponentType();
			if (ShouldExclude(type))
				return;
			if (this.entriesByValue.ContainsKey(newValue))
				return;

			Entry entry;
			if (this.entriesByValue.TryGetValue(keyValue, out entry))
			{
				if (newValue == keyValue)
					return;
			}
			else if (node != null)
			{
				bool reference;
				if (!this.TryGetEntry(node, out entry, out reference))
					entry = new Entry(node);
			}
			else
				return;

			this.AddValueCore(entry, type, newValue, isInGraph);
		}

		private void AddReference(IXmlNode node, Entry entry)
		{
			if (!entry.Node.PositionEquals(node))
			{
				if (entry.References == null)
				{
					this.GenerateId(entry);
					this.format.SetIdentity(entry.Node, entry.Id);
				}
				node.Clear();
				entry.AddReference(node);
				this.format.SetReference(node, entry.Id);
			}
		}

		private void AddValue(Entry entry, Type type, object value, XmlAdapter xmlAdapter)
		{
			if (xmlAdapter == null)
				xmlAdapter = XmlAdapter.For(value, false);

			this.AddValueCore(entry, type, value, true);

			if (xmlAdapter != null)
				this.AddValueCore(entry, typeof(XmlAdapter), xmlAdapter, true);
		}

		private void AddValueCore(Entry entry, Type type, object value, bool isInGraph)
		{
			entry.AddValue(type, value, isInGraph);
			this.entriesByValue.Add(value, entry);
		}

		private void ClearReference(Entry entry, IXmlNode node)
		{
			this.format.ClearReference(node);

			if (entry.References == null)
				this.format.ClearIdentity(entry.Node);
		}

		private void GenerateId(Entry entry)
		{
			if (entry.Id == 0)
			{
				entry.Id = this.nextId++;
				this.entriesById.Add(entry.Id, entry);
			}
		}

		public void OnAssignedValue(IXmlNode node, object givenValue, object storedValue, object token)
		{
			var entry = token as Entry;
			if (entry == null)
				return;

			if (ReferenceEquals(givenValue, storedValue))
				return;

			SetNotInGraph(entry, givenValue);

			if (this.entriesByValue.ContainsKey(storedValue))
				return;

			this.AddValue(entry, node.ClrType, storedValue, null);
		}

		public bool OnAssigningNull(IXmlNode node, object oldValue)
		{
			object token, newValue = null;
			return this.OnAssigningValue(node, oldValue, ref newValue, out token);
		}

		public bool OnAssigningValue(IXmlNode node, object oldValue, ref object newValue, out object token)
		{
			if (newValue == oldValue && newValue != null)
			{
				token = null;
				return false;
			}

			var oldEntry = this.OnReplacingValue(node, oldValue);

			if (newValue == null)
				return this.ShouldAssignmentProceed(oldEntry, null, token = null);

			var type = newValue.GetComponentType();
			if (ShouldExclude(type))
				return this.ShouldAssignmentProceed(oldEntry, null, token = null);

			var xmlAdapter = XmlAdapter.For(newValue, false);

			Entry newEntry;
			if (this.entriesByValue.TryGetValue(xmlAdapter ?? newValue, out newEntry))
			{
				// Value already present in graph; add reference
				this.TryGetCompatibleValue(newEntry, type, ref newValue);
				this.AddReference(node, newEntry);
				token = null;
			}
			else
			{
				// Value not present in graph; add as primary
				newEntry = oldEntry ?? new Entry(node);
				this.AddValue(newEntry, type, newValue, xmlAdapter);
				this.format.ClearIdentity(node);
				this.format.ClearReference(node);
				token = newEntry;
			}
			return this.ShouldAssignmentProceed(oldEntry, newEntry, token);
		}

		public void OnGetCompleted(IXmlNode node, object value, object token)
		{
			if (value == null)
				return;

			var type = node.ClrType;
			if (ShouldExclude(type))
				return;

			if (this.entriesByValue.ContainsKey(value))
				return;

			var entry = (token == CreateEntryToken)
				? new Entry(node)
				: token as Entry;
			if (entry == null)
				return;

			this.AddValue(entry, type, value, null);
		}

		public bool OnGetStarting(ref IXmlNode node, ref object value, out object token)
		{
			Entry entry;
			bool isReference;

			var type = node.ClrType;
			if (ShouldExclude(type))
			{
				token = null;
				return true;
			}

			if (!this.TryGetEntry(node, out entry, out isReference))
			{
				token = CreateEntryToken;
				return true;
			}

			if (isReference)
				RedirectNode(ref node, entry);

			var proceed = ! this.TryGetCompatibleValue(entry, node.ClrType, ref value);

			token = proceed ? entry : null;
			return proceed;
		}

		private Entry OnReplacingValue(IXmlNode node, object oldValue)
		{
			Entry entry;
			bool isReference;

			if (oldValue == null)
			{
				if (!this.TryGetEntry(node, out entry, out isReference))
					return null;
			}
			else
			{
				if (!this.entriesByValue.TryGetValue(oldValue, out entry))
					return null;
				isReference = !entry.Node.PositionEquals(node);
			}

			if (isReference)
			{
				// Replacing reference
				entry.RemoveReference(node);
				this.ClearReference(entry, node);
				return null;
			}
			else if (entry.References != null)
			{
				// Replacing primary that has references
				// Relocate content to a referencing node (making it a new primary)
				node = entry.RemoveReference(0);
				this.ClearReference(entry, node);
				entry.Node.CopyTo(node);
				entry.Node.Clear();
				entry.Node = node;
				return null;
			}
			else
			{
				// Replaceing primary with no references; reuse entry
				this.PrepareForReuse(entry);
				return entry;
			}
		}

		private void Populate(IXmlNode node)
		{
			var references = new List<Reference>();
			var iterator = node.SelectSubtree();

			while (iterator.MoveNext())
				this.PopulateFromNode(iterator, references);

			this.PopulateDeferredReferences(references);
		}

		private void PopulateDeferredReferences(ICollection<Reference> references)
		{
			foreach (var reference in references)
			{
				Entry entry;
				if (this.entriesById.TryGetValue(reference.Id, out entry))
					entry.AddReference(reference.Node);
			}
		}

		private void PopulateFromNode(IXmlIterator node, ICollection<Reference> references)
		{
			int id;
			if (this.format.TryGetIdentity(node, out id))
				this.PopulateIdentity(id, node.Save());
			else if (this.format.TryGetReference(node, out id))
				this.PopulateReference(id, node.Save(), references);
		}

		private void PopulateIdentity(int id, IXmlNode node)
		{
			Entry entry;
			if (!this.entriesById.TryGetValue(id, out entry))
				this.entriesById.Add(id, new Entry(id, node));
			if (this.nextId <= id)
				this.nextId = ++id;
		}

		private void PopulateReference(int id, IXmlNode node, ICollection<Reference> references)
		{
			Entry entry;
			if (this.entriesById.TryGetValue(id, out entry))
				entry.AddReference(node);
			else
				references.Add(new Reference(id, node));
		}

		private void PrepareForReuse(Entry entry)
		{
			foreach (var item in entry.Values)
			{
				var value = item.Value.Target;
				if (null != value)
					this.entriesByValue.Remove(value);
			}
			entry.Values.Clear();

			this.format.ClearIdentity(entry.Node);
		}

		private bool ShouldAssignmentProceed(Entry oldEntry, Entry newEntry, object token)
		{
			if (oldEntry != null && oldEntry != newEntry && oldEntry.Id > 0)
				this.entriesById.Remove(oldEntry.Id); // Didn't reuse old entry; delete it

			return token != null // Expecting callback with a token, so proceed with set
					|| newEntry == null; // No reference tracking for this value; don't prevent assignment
		}

		public bool TryGet(object keyObject, out object inGraphObject)
		{
			Entry entry;
			if (this.entriesByValue.TryGetValue(keyObject, out entry))
			{
				inGraphObject = keyObject;
				this.TryGetCompatibleValue(entry, keyObject.GetComponentType(), ref inGraphObject);
				return true;
			}
			else
			{
				inGraphObject = null;
				return false;
			}
		}

		private bool TryGetCompatibleValue(Entry entry, Type type, ref object value)
		{
			var values = entry.Values;
			if (values == null)
				return false;

			var dictionaryAdapter = null as IDictionaryAdapter;

			// Try to find in the graph a directly assignable value
			foreach (var item in values)
			{
				if (!item.IsInGraph)
					continue;

				var candidate = item.Value.Target;
				if (candidate == null)
					continue;

				if (type.IsAssignableFrom(item.Type))
				{
					if (null != candidate)
						return Try.Success(out value, candidate);
				}

				if (dictionaryAdapter == null)
					dictionaryAdapter = candidate as IDictionaryAdapter;
			}

			// Fall back to coercing a DA found in the graph
			if (dictionaryAdapter != null)
			{
				value = dictionaryAdapter.Coerce(type);
				entry.AddValue(type, value, true);
				return true;
			}

			return false;
		}

		private bool TryGetEntry(IXmlNode node, out Entry entry, out bool reference)
		{
			int id;

			if (this.format.TryGetIdentity(node, out id))
				reference = false;
			else if (this.format.TryGetReference(node, out id))
				reference = true;
			else
			{
				reference = false;
				entry = null;
				return false;
			}

			if (!this.entriesById.TryGetValue(id, out entry))
				throw IdNotFoundError(id);
			return true;
		}

		public void UnionWith(XmlReferenceManager other)
		{
			var visited = null as HashSet<Entry>;

			foreach (var otherEntry in other.entriesByValue)
			{
				Entry thisEntry;
				if (this.entriesByValue.TryGetValue(otherEntry.Key, out thisEntry))
				{
					if (visited == null)
						visited = new HashSet<Entry>(ReferenceEqualityComparer<Entry>.Instance);
					else if (visited.Contains(thisEntry))
						continue;
					visited.Add(thisEntry);

					foreach (var otherValue in otherEntry.Value.Values)
					{
						var otherTarget = otherValue.Value.Target;
						if (otherTarget == null ||
							otherTarget == otherEntry.Key ||
							this.entriesByValue.ContainsKey(otherTarget))
							continue;
						this.AddValueCore(thisEntry, otherValue.Type, otherTarget, false);
					}
				}
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class Entry
		{
			#region Constructors/Destructors

			public Entry(IXmlNode node)
			{
				this.Node = node.Save();
			}

			public Entry(int id, IXmlNode node)
				: this(node)
			{
				this.Id = id;
			}

			#endregion

			#region Fields/Constants

			public int Id;
			public IXmlNode Node;
			private List<IXmlNode> references;
			private List<EntryValue> values;

			#endregion

			#region Properties/Indexers/Events

			public List<IXmlNode> References
			{
				get
				{
					return this.references;
				}
			}

			public List<EntryValue> Values
			{
				get
				{
					return this.values;
				}
			}

			#endregion

			#region Methods/Operators

			public void AddReference(IXmlNode node)
			{
				if (this.references == null)
					this.references = new List<IXmlNode>();
				this.references.Add(node);
			}

			public void AddValue(Type type, object value, bool isInGraph)
			{
				if (this.values == null)
					this.values = new List<EntryValue>();
				this.values.Add(new EntryValue(type, value, isInGraph));
			}

			public IXmlNode RemoveReference(IXmlNode node)
			{
				for (var index = 0; index < this.references.Count; index++)
				{
					if (this.references[index].PositionEquals(node))
						return RemoveReference(index);
				}
				return node;
			}

			public IXmlNode RemoveReference(int index)
			{
				var node = this.references[index];
				this.references.RemoveAt(index);
				if (this.references.Count == 0)
					this.references = null;
				return node;
			}

			#endregion
		}

		private struct EntryValue
		{
			#region Constructors/Destructors

			public EntryValue(Type type, object value, bool isInGraph)
				: this(type, new WeakReference(value), isInGraph)
			{
			}

			public EntryValue(Type type, WeakReference value, bool isInGraph)
			{
				this.Type = type;
				this.Value = value;
				this.IsInGraph = isInGraph;
			}

			#endregion

			#region Fields/Constants

			public readonly bool IsInGraph;
			public readonly Type Type;
			public readonly WeakReference Value;

			#endregion
		}

		private struct Reference
		{
			#region Constructors/Destructors

			public Reference(int id, IXmlNode node)
			{
				this.Id = id;
				this.Node = node;
			}

			#endregion

			#region Fields/Constants

			public readonly int Id;
			public readonly IXmlNode Node;

			#endregion
		}

		#endregion
	}
}

#endif