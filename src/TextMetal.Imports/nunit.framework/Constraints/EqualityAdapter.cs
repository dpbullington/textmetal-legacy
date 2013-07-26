// ****************************************************************
// Copyright 2009, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
#if CLR_2_0 || CLR_4_0

#endif

namespace NUnit.Framework.Constraints
{
	/// <summary>
	/// 	EqualityAdapter class handles all equality comparisons
	/// 	that use an IEqualityComparer, IEqualityComparer&lt;T&gt;
	/// 	or a ComparisonAdapter.
	/// </summary>
	public abstract class EqualityAdapter
	{
		/// <summary>
		/// 	Compares two objects, returning true if they are equal
		/// </summary>
		public abstract bool AreEqual(object x, object y);

		/// <summary>
		/// 	Returns true if the two objects can be compared by this adapter.
		/// 	The base adapter cannot handle IEnumerables except for strings.
		/// </summary>
		public virtual bool CanCompare(object x, object y)
		{
			if (x is string && y is string)
				return true;

			if (x is IEnumerable || y is IEnumerable)
				return false;

			return true;
		}

		#region Nested IComparer Adapter

		/// <summary>
		/// 	Returns an EqualityAdapter that wraps an IComparer.
		/// </summary>
		public static EqualityAdapter For(IComparer comparer)
		{
			return new ComparerAdapter(comparer);
		}

		/// <summary>
		/// 	EqualityAdapter that wraps an IComparer.
		/// </summary>
		private class ComparerAdapter : EqualityAdapter
		{
			#region Constructors/Destructors

			public ComparerAdapter(IComparer comparer)
			{
				this.comparer = comparer;
			}

			#endregion

			#region Fields/Constants

			private IComparer comparer;

			#endregion

			#region Methods/Operators

			public override bool AreEqual(object x, object y)
			{
				return this.comparer.Compare(x, y) == 0;
			}

			#endregion
		}

		#endregion

#if CLR_2_0 || CLR_4_0

		#region Nested IEqualityComparer Adapter

		/// <summary>
		/// 	Returns an EqualityAdapter that wraps an IEqualityComparer.
		/// </summary>
		public static EqualityAdapter For(IEqualityComparer comparer)
		{
			return new EqualityComparerAdapter(comparer);
		}

		private class EqualityComparerAdapter : EqualityAdapter
		{
			#region Constructors/Destructors

			public EqualityComparerAdapter(IEqualityComparer comparer)
			{
				this.comparer = comparer;
			}

			#endregion

			#region Fields/Constants

			private IEqualityComparer comparer;

			#endregion

			#region Methods/Operators

			public override bool AreEqual(object x, object y)
			{
				return this.comparer.Equals(x, y);
			}

			#endregion
		}

		#endregion

		#region Nested GenericEqualityAdapter<T>

		private abstract class GenericEqualityAdapter<T> : EqualityAdapter
		{
			#region Methods/Operators

			/// <summary>
			/// 	Returns true if the two objects can be compared by this adapter.
			/// 	Generic adapter requires objects of the specified type.
			/// </summary>
			public override bool CanCompare(object x, object y)
			{
				return typeof(T).IsAssignableFrom(x.GetType())
				       && typeof(T).IsAssignableFrom(y.GetType());
			}

			protected void ThrowIfNotCompatible(object x, object y)
			{
				if (!typeof(T).IsAssignableFrom(x.GetType()))
					throw new ArgumentException("Cannot compare " + x.ToString());

				if (!typeof(T).IsAssignableFrom(y.GetType()))
					throw new ArgumentException("Cannot compare " + y.ToString());
			}

			#endregion
		}

		#endregion

		#region Nested IEqualityComparer<T> Adapter

		/// <summary>
		/// 	Returns an EqualityAdapter that wraps an IEqualityComparer&lt;T&gt;.
		/// </summary>
		public static EqualityAdapter For<T>(IEqualityComparer<T> comparer)
		{
			return new EqualityComparerAdapter<T>(comparer);
		}

		private class EqualityComparerAdapter<T> : GenericEqualityAdapter<T>
		{
			#region Constructors/Destructors

			public EqualityComparerAdapter(IEqualityComparer<T> comparer)
			{
				this.comparer = comparer;
			}

			#endregion

			#region Fields/Constants

			private IEqualityComparer<T> comparer;

			#endregion

			#region Methods/Operators

			public override bool AreEqual(object x, object y)
			{
				this.ThrowIfNotCompatible(x, y);
				return this.comparer.Equals((T)x, (T)y);
			}

			#endregion
		}

		#endregion

		#region Nested IComparer<T> Adapter

		/// <summary>
		/// 	Returns an EqualityAdapter that wraps an IComparer&lt;T&gt;.
		/// </summary>
		public static EqualityAdapter For<T>(IComparer<T> comparer)
		{
			return new ComparerAdapter<T>(comparer);
		}

		/// <summary>
		/// 	EqualityAdapter that wraps an IComparer.
		/// </summary>
		private class ComparerAdapter<T> : GenericEqualityAdapter<T>
		{
			#region Constructors/Destructors

			public ComparerAdapter(IComparer<T> comparer)
			{
				this.comparer = comparer;
			}

			#endregion

			#region Fields/Constants

			private IComparer<T> comparer;

			#endregion

			#region Methods/Operators

			public override bool AreEqual(object x, object y)
			{
				this.ThrowIfNotCompatible(x, y);
				return this.comparer.Compare((T)x, (T)y) == 0;
			}

			#endregion
		}

		#endregion

		#region Nested Comparison<T> Adapter

		/// <summary>
		/// 	Returns an EqualityAdapter that wraps a Comparison&lt;T&gt;.
		/// </summary>
		public static EqualityAdapter For<T>(Comparison<T> comparer)
		{
			return new ComparisonAdapter<T>(comparer);
		}

		private class ComparisonAdapter<T> : GenericEqualityAdapter<T>
		{
			#region Constructors/Destructors

			public ComparisonAdapter(Comparison<T> comparer)
			{
				this.comparer = comparer;
			}

			#endregion

			#region Fields/Constants

			private Comparison<T> comparer;

			#endregion

			#region Methods/Operators

			public override bool AreEqual(object x, object y)
			{
				this.ThrowIfNotCompatible(x, y);
				return this.comparer.Invoke((T)x, (T)y) == 0;
			}

			#endregion
		}

		#endregion

#endif
	}

#if CLR_2_0x || CLR_4_0x
	/// <summary>
	/// EqualityAdapter class handles all equality comparisons
	/// that use an IEqualityComparer, IEqualityComparer&lt;T&gt;
	/// or a ComparisonAdapter.
	/// </summary>
    public abstract class EqualityAdapter<T> : EqualityAdapter, INUnitEqualityComparer<T>
    {
        /// <summary>
        /// Compares two objects, returning true if they are equal
        /// </summary>
        public abstract bool AreEqual(T x, T y, ref Tolerance tolerance);

    }
#endif
}