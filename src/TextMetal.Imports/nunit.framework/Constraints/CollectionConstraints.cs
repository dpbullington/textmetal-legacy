// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

#if CLR_2_0 || CLR_4_0

#endif

namespace NUnit.Framework.Constraints
{

	#region CollectionConstraint

	/// <summary>
	/// CollectionConstraint is the abstract base class for
	/// constraints that operate on collections.
	/// </summary>
	public abstract class CollectionConstraint : Constraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct an empty CollectionConstraint
		/// </summary>
		public CollectionConstraint()
		{
		}

		/// <summary>
		/// Construct a CollectionConstraint
		/// </summary>
		/// <param name="arg"> </param>
		public CollectionConstraint(object arg)
			: base(arg)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Determines whether the specified enumerable is empty.
		/// </summary>
		/// <param name="enumerable"> The enumerable. </param>
		/// <returns> <c> true </c> if the specified enumerable is empty; otherwise, <c> false </c> . </returns>
		protected static bool IsEmpty(IEnumerable enumerable)
		{
			ICollection collection = enumerable as ICollection;
			if (collection != null)
				return collection.Count == 0;

			foreach (object o in enumerable)
				return false;

			return true;
		}

		/// <summary>
		/// Test whether the constraint is satisfied by a given value
		/// </summary>
		/// <param name="actual"> The value to be tested </param>
		/// <returns> True for success, false for failure </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			IEnumerable enumerable = actual as IEnumerable;
			if (enumerable == null)
				throw new ArgumentException("The actual value must be an IEnumerable", "actual");

			return this.doMatch(enumerable);
		}

		/// <summary>
		/// Protected method to be implemented by derived classes
		/// </summary>
		/// <param name="collection"> </param>
		/// <returns> </returns>
		protected abstract bool doMatch(IEnumerable collection);

		#endregion
	}

	#endregion

	#region CollectionItemsEqualConstraint

	/// <summary>
	/// CollectionItemsEqualConstraint is the abstract base class for all
	/// collection constraints that apply some notion of item equality
	/// as a part of their operation.
	/// </summary>
	public abstract class CollectionItemsEqualConstraint : CollectionConstraint
	{
		// This is internal so that ContainsConstraint can set it
		// TODO: Figure out a way to avoid this indirection
		internal NUnitEqualityComparer comparer = NUnitEqualityComparer.Default;

		/// <summary>
		/// Construct an empty CollectionConstraint
		/// </summary>
		public CollectionItemsEqualConstraint()
		{
		}

		/// <summary>
		/// Construct a CollectionConstraint
		/// </summary>
		/// <param name="arg"> </param>
		public CollectionItemsEqualConstraint(object arg)
			: base(arg)
		{
		}

		#region Modifiers

		/// <summary>
		/// Flag the constraint to ignore case and return self.
		/// </summary>
		public CollectionItemsEqualConstraint IgnoreCase
		{
			get
			{
				this.comparer.IgnoreCase = true;
				return this;
			}
		}

		/// <summary>
		/// Flag the constraint to use the supplied IComparer object.
		/// </summary>
		/// <param name="comparer"> The IComparer object to use. </param>
		/// <returns> Self. </returns>
		public CollectionItemsEqualConstraint Using(IComparer comparer)
		{
			this.comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

#if CLR_2_0 || CLR_4_0
		/// <summary>
		/// Flag the constraint to use the supplied IComparer object.
		/// </summary>
		/// <param name="comparer"> The IComparer object to use. </param>
		/// <returns> Self. </returns>
		public CollectionItemsEqualConstraint Using<T>(IComparer<T> comparer)
		{
			this.comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		/// <summary>
		/// Flag the constraint to use the supplied Comparison object.
		/// </summary>
		/// <param name="comparer"> The IComparer object to use. </param>
		/// <returns> Self. </returns>
		public CollectionItemsEqualConstraint Using<T>(Comparison<T> comparer)
		{
			this.comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		/// <summary>
		/// Flag the constraint to use the supplied IEqualityComparer object.
		/// </summary>
		/// <param name="comparer"> The IComparer object to use. </param>
		/// <returns> Self. </returns>
		public CollectionItemsEqualConstraint Using(IEqualityComparer comparer)
		{
			this.comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		/// <summary>
		/// Flag the constraint to use the supplied IEqualityComparer object.
		/// </summary>
		/// <param name="comparer"> The IComparer object to use. </param>
		/// <returns> Self. </returns>
		public CollectionItemsEqualConstraint Using<T>(IEqualityComparer<T> comparer)
		{
			this.comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}
#endif

		#endregion

		/// <summary>
		/// Compares two collection members for equality
		/// </summary>
		protected bool ItemsEqual(object x, object y)
		{
			Tolerance tolerance = Tolerance.Zero;
			return this.comparer.AreEqual(x, y, ref tolerance);
		}

		/// <summary>
		/// Return a new CollectionTally for use in making tests
		/// </summary>
		/// <param name="c"> The collection to be included in the tally </param>
		protected CollectionTally Tally(IEnumerable c)
		{
			return new CollectionTally(this.comparer, c);
		}
	}

	#endregion

	#region EmptyCollectionConstraint

	/// <summary>
	/// EmptyCollectionConstraint tests whether a collection is empty.
	/// </summary>
	public class EmptyCollectionConstraint : CollectionConstraint
	{
		#region Methods/Operators

		/// <summary>
		/// Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("<empty>");
		}

		/// <summary>
		/// Check that the collection is empty
		/// </summary>
		/// <param name="collection"> </param>
		/// <returns> </returns>
		protected override bool doMatch(IEnumerable collection)
		{
			return IsEmpty(collection);
		}

		#endregion
	}

	#endregion

	#region UniqueItemsConstraint

	/// <summary>
	/// UniqueItemsConstraint tests whether all the items in a
	/// collection are unique.
	/// </summary>
	public class UniqueItemsConstraint : CollectionItemsEqualConstraint
	{
		#region Methods/Operators

		/// <summary>
		/// Write a description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("all items unique");
		}

		/// <summary>
		/// Check that all items are unique.
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		protected override bool doMatch(IEnumerable actual)
		{
			ArrayList list = new ArrayList();

			foreach (object o1 in actual)
			{
				foreach (object o2 in list)
				{
					if (this.ItemsEqual(o1, o2))
						return false;
				}
				list.Add(o1);
			}

			return true;
		}

		#endregion
	}

	#endregion

	#region CollectionContainsConstraint

	/// <summary>
	/// CollectionContainsConstraint is used to test whether a collection
	/// contains an expected object as a member.
	/// </summary>
	public class CollectionContainsConstraint : CollectionItemsEqualConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct a CollectionContainsConstraint
		/// </summary>
		/// <param name="expected"> </param>
		public CollectionContainsConstraint(object expected)
			: base(expected)
		{
			this.expected = expected;
			this.DisplayName = "contains";
		}

		#endregion

		#region Fields/Constants

		private object expected;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Write a descripton of the constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("collection containing");
			writer.WriteExpectedValue(this.expected);
		}

		/// <summary>
		/// Test whether the expected item is contained in the collection
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		protected override bool doMatch(IEnumerable actual)
		{
			foreach (object obj in actual)
			{
				if (this.ItemsEqual(obj, this.expected))
					return true;
			}

			return false;
		}

		#endregion
	}

	#endregion

	#region CollectionEquivalentConstraint

	/// <summary>
	/// CollectionEquivalentCOnstraint is used to determine whether two
	/// collections are equivalent.
	/// </summary>
	public class CollectionEquivalentConstraint : CollectionItemsEqualConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct a CollectionEquivalentConstraint
		/// </summary>
		/// <param name="expected"> </param>
		public CollectionEquivalentConstraint(IEnumerable expected)
			: base(expected)
		{
			this.expected = expected;
			this.DisplayName = "equivalent";
		}

		#endregion

		#region Fields/Constants

		private IEnumerable expected;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Write a description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("equivalent to");
			writer.WriteExpectedValue(this.expected);
		}

		/// <summary>
		/// Test whether two collections are equivalent
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		protected override bool doMatch(IEnumerable actual)
		{
			// This is just an optimization
			if (this.expected is ICollection && actual is ICollection)
			{
				if (((ICollection)actual).Count != ((ICollection)this.expected).Count)
					return false;
			}

			CollectionTally tally = this.Tally(this.expected);
			return tally.TryRemove(actual) && tally.Count == 0;
		}

		#endregion
	}

	#endregion

	#region CollectionSubsetConstraint

	/// <summary>
	/// CollectionSubsetConstraint is used to determine whether
	/// one collection is a subset of another
	/// </summary>
	public class CollectionSubsetConstraint : CollectionItemsEqualConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct a CollectionSubsetConstraint
		/// </summary>
		/// <param name="expected"> The collection that the actual value is expected to be a subset of </param>
		public CollectionSubsetConstraint(IEnumerable expected)
			: base(expected)
		{
			this.expected = expected;
			this.DisplayName = "subsetof";
		}

		#endregion

		#region Fields/Constants

		private IEnumerable expected;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Write a description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("subset of");
			writer.WriteExpectedValue(this.expected);
		}

		/// <summary>
		/// Test whether the actual collection is a subset of
		/// the expected collection provided.
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		protected override bool doMatch(IEnumerable actual)
		{
			return this.Tally(this.expected).TryRemove(actual);
		}

		#endregion
	}

	#endregion

	#region CollectionOrderedConstraint

	/// <summary>
	/// CollectionOrderedConstraint is used to test whether a collection is ordered.
	/// </summary>
	public class CollectionOrderedConstraint : CollectionConstraint
	{
		private ComparisonAdapter comparer = ComparisonAdapter.Default;
		private string comparerName;
		private string propertyName;
		private bool descending;

		/// <summary>
		/// Construct a CollectionOrderedConstraint
		/// </summary>
		public CollectionOrderedConstraint()
		{
			this.DisplayName = "ordered";
		}

		/// <summary>
		/// If used performs a reverse comparison
		/// </summary>
		public CollectionOrderedConstraint Descending
		{
			get
			{
				this.@descending = true;
				return this;
			}
		}

		/// <summary>
		/// Modifies the constraint to use an IComparer and returns self.
		/// </summary>
		public CollectionOrderedConstraint Using(IComparer comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			this.comparerName = comparer.GetType().FullName;
			return this;
		}

#if CLR_2_0 || CLR_4_0
		/// <summary>
		/// Modifies the constraint to use an IComparer&lt;T&gt; and returns self.
		/// </summary>
		public CollectionOrderedConstraint Using<T>(IComparer<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			this.comparerName = comparer.GetType().FullName;
			return this;
		}

		/// <summary>
		/// Modifies the constraint to use a Comparison&lt;T&gt; and returns self.
		/// </summary>
		public CollectionOrderedConstraint Using<T>(Comparison<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			this.comparerName = comparer.GetType().FullName;
			return this;
		}
#endif

		/// <summary>
		/// Modifies the constraint to test ordering by the value of
		/// a specified property and returns self.
		/// </summary>
		public CollectionOrderedConstraint By(string propertyName)
		{
			this.propertyName = propertyName;
			return this;
		}

		/// <summary>
		/// Test whether the collection is ordered
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		protected override bool doMatch(IEnumerable actual)
		{
			object previous = null;
			int index = 0;
			foreach (object obj in actual)
			{
				object objToCompare = obj;
				if (obj == null)
					throw new ArgumentNullException("actual", "Null value at index " + index.ToString());

				if (this.propertyName != null)
				{
					PropertyInfo prop = obj.GetType().GetProperty(this.propertyName);
					objToCompare = prop.GetValue(obj, null);
					if (objToCompare == null)
						throw new ArgumentNullException("actual", "Null property value at index " + index.ToString());
				}

				if (previous != null)
				{
					//int comparisonResult = comparer.Compare(al[i], al[i + 1]);
					int comparisonResult = this.comparer.Compare(previous, objToCompare);

					if (this.@descending && comparisonResult < 0)
						return false;
					if (!this.@descending && comparisonResult > 0)
						return false;
				}

				previous = objToCompare;
				index++;
			}

			return true;
		}

		/// <summary>
		/// Write a description of the constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			if (this.propertyName == null)
				writer.Write("collection ordered");
			else
			{
				writer.WritePredicate("collection ordered by");
				writer.WriteExpectedValue(this.propertyName);
			}

			if (this.@descending)
				writer.WriteModifier("descending");
		}

		/// <summary>
		/// Returns the string representation of the constraint.
		/// </summary>
		/// <returns> </returns>
		protected override string GetStringRepresentation()
		{
			StringBuilder sb = new StringBuilder("<ordered");

			if (this.propertyName != null)
				sb.Append("by " + this.propertyName);
			if (this.@descending)
				sb.Append(" descending");
			if (this.comparerName != null)
				sb.Append(" " + this.comparerName);

			sb.Append(">");

			return sb.ToString();
		}
	}

	#endregion
}