// ****************************************************************
// Copyright 2010, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	/// <summary>
	/// 	CollectionTally counts (tallies) the number of
	/// 	occurences of each object in one or more enumerations.
	/// </summary>
	public class CollectionTally
	{
		// Internal list used to track occurences

		#region Constructors/Destructors

		/// <summary>
		/// 	Construct a CollectionTally object from a comparer and a collection
		/// </summary>
		public CollectionTally(NUnitEqualityComparer comparer, IEnumerable c)
		{
			this.comparer = comparer;

			foreach (object o in c)
				this.list.Add(o);
		}

		#endregion

		#region Fields/Constants

		private NUnitEqualityComparer comparer;
		private ArrayList list = new ArrayList();

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	The number of objects remaining in the tally
		/// </summary>
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		#endregion

		#region Methods/Operators

		private bool ItemsEqual(object expected, object actual)
		{
			Tolerance tolerance = Tolerance.Zero;
			return this.comparer.AreEqual(expected, actual, ref tolerance);
		}

		/// <summary>
		/// 	Try to remove an object from the tally
		/// </summary>
		/// <param name="o"> The object to remove </param>
		/// <returns> True if successful, false if the object was not found </returns>
		public bool TryRemove(object o)
		{
			for (int index = 0; index < this.list.Count; index++)
			{
				if (this.ItemsEqual(this.list[index], o))
				{
					this.list.RemoveAt(index);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 	Try to remove a set of objects from the tally
		/// </summary>
		/// <param name="c"> The objects to remove </param>
		/// <returns> True if successful, false if any object was not found </returns>
		public bool TryRemove(IEnumerable c)
		{
			foreach (object o in c)
			{
				if (!TryRemove(o))
					return false;
			}

			return true;
		}

		#endregion
	}
}