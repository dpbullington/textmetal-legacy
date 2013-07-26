// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.IO;

namespace NUnit.Util
{
	/// <summary>
	/// 	Represents a list of assemblies. It stores paths 
	/// 	that are added and fires an event whenevever it
	/// 	changes. All paths must be added as absolute paths.
	/// </summary>
	public class AssemblyList : CollectionBase
	{
		#region Properties/Indexers/Events

		public event EventHandler Changed;

		public string this[int index]
		{
			get
			{
				return (string)this.List[index];
			}
			set
			{
				if (!Path.IsPathRooted(value))
					throw new ArgumentException("Assembly path must be absolute");
				this.List[index] = value;
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(string assemblyPath)
		{
			if (!Path.IsPathRooted(assemblyPath))
				throw new ArgumentException("Assembly path must be absolute");
			this.List.Add(assemblyPath);
		}

		public bool Contains(string assemblyPath)
		{
			for (int index = 0; index < this.Count; index++)
			{
				if (this[index] == assemblyPath)
					return true;
			}

			return false;
		}

		private void FireChangedEvent()
		{
			if (this.Changed != null)
				this.Changed(this, EventArgs.Empty);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			this.FireChangedEvent();
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			this.FireChangedEvent();
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			this.FireChangedEvent();
		}

		public void Remove(string assemblyPath)
		{
			for (int index = 0; index < this.Count; index++)
			{
				if (this[index] == assemblyPath)
					this.RemoveAt(index);
			}
		}

		public string[] ToArray()
		{
			return (string[])this.InnerList.ToArray(typeof(string));
		}

		#endregion
	}
}