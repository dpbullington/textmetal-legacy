// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Util
{
	/// <summary>
	/// 	Summary description for RecentFilesCollection.
	/// </summary>
	public class RecentFilesCollection : ReadOnlyCollectionBase
	{
		#region Properties/Indexers/Events

		public RecentFileEntry this[int index]
		{
			get
			{
				return (RecentFileEntry)this.InnerList[index];
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(RecentFileEntry entry)
		{
			this.InnerList.Add(entry);
		}

		public void Clear()
		{
			this.InnerList.Clear();
		}

		public int IndexOf(string fileName)
		{
			for (int index = 0; index < this.InnerList.Count; index++)
			{
				if (this[index].Path == fileName)
					return index;
			}
			return -1;
		}

		public void Insert(int index, RecentFileEntry entry)
		{
			this.InnerList.Insert(index, entry);
		}

		public void Remove(string fileName)
		{
			int index = this.IndexOf(fileName);
			if (index != -1)
				this.RemoveAt(index);
		}

		public void RemoveAt(int index)
		{
			this.InnerList.RemoveAt(index);
		}

		#endregion
	}
}