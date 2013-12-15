// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

using System.Collections.Generic;
using System.Linq;

#if DOTNET40

namespace Castle.Components.DictionaryAdapter
{
	public class SetProjection<T> : ListProjection<T>, ISet<T>
	{
		#region Constructors/Destructors

		public SetProjection(ICollectionAdapter<T> adapter)
			: base(adapter)
		{
			this.set = new HashSet<T>();
			this.Repopulate();
		}

		#endregion

		#region Fields/Constants

		private readonly HashSet<T> set;

		#endregion

		#region Methods/Operators

		public override bool Add(T item)
		{
			return !this.set.Contains(item)
					&& base.Add(item);
		}

		public override void Clear()
		{
			this.set.Clear();
			base.Clear();
		}

		public override bool Contains(T item)
		{
			return this.set.Contains(item);
		}

		public override void EndNew(int index)
		{
			if (this.IsNew(index) && this.OnInserting(this[index]))
				base.EndNew(index);
			else
				this.CancelNew(index);
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			foreach (var value in other)
				this.Remove(value);
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			var removals = this.set.Except(other).ToArray();

			this.ExceptWith(removals);
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			return this.set.IsProperSubsetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			return this.set.IsProperSupersetOf(other);
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			return this.set.IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			return this.set.IsSupersetOf(other);
		}

		protected override bool OnInserting(T value)
		{
			return this.set.Add(value);
		}

		protected override bool OnReplacing(T oldValue, T newValue)
		{
			if (!this.set.Add(newValue))
				return false;

			this.set.Remove(oldValue);
			return true;
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			return this.set.Overlaps(other);
		}

		public override bool Remove(T item)
		{
			return this.set.Remove(item)
					&& base.Remove(item);
		}

		public override void RemoveAt(int index)
		{
			this.set.Remove(this[index]);
			base.RemoveAt(index);
		}

		private void Repopulate()
		{
			this.SuspendEvents();

			var count = this.Count;
			for (var index = 0; index < count;)
			{
				var value = this[index];

				if (!this.set.Add(value))
				{
					this.RemoveAt(index);
					count--;
				}
				else
					index++;
			}

			this.ResumeEvents();
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			return this.set.SetEquals(other);
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			var removals = this.set.Intersect(other).ToArray();
			var additions = other.Except(removals);

			this.ExceptWith(removals);
			this.UnionWith(additions);
		}

		public void UnionWith(IEnumerable<T> other)
		{
			foreach (var value in other)
				this.Add(value);
		}

		#endregion
	}
}

#endif