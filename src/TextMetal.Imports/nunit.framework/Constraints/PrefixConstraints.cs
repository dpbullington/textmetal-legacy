// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{

	#region PrefixConstraint

	/// <summary>
	/// Abstract base class used for prefixes
	/// </summary>
	public abstract class PrefixConstraint : Constraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct given a base constraint
		/// </summary>
		/// <param name="resolvable"> </param>
		protected PrefixConstraint(IResolveConstraint resolvable)
			: base(resolvable)
		{
			if (resolvable != null)
				this.baseConstraint = resolvable.Resolve();
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// The base constraint
		/// </summary>
		protected Constraint baseConstraint;

		#endregion
	}

	#endregion

	#region NotConstraint

	/// <summary>
	/// NotConstraint negates the effect of some other constraint
	/// </summary>
	public class NotConstraint : PrefixConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NotConstraint" /> class.
		/// </summary>
		/// <param name="baseConstraint"> The base constraint to be negated. </param>
		public NotConstraint(Constraint baseConstraint)
			: base(baseConstraint)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Test whether the constraint is satisfied by a given value
		/// </summary>
		/// <param name="actual"> The value to be tested </param>
		/// <returns> True for if the base constraint fails, false if it succeeds </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;
			return !this.baseConstraint.Matches(actual);
		}

		/// <summary>
		/// Write the actual value for a failing constraint test to a MessageWriter.
		/// </summary>
		/// <param name="writer"> The writer on which the actual value is displayed </param>
		public override void WriteActualValueTo(MessageWriter writer)
		{
			this.baseConstraint.WriteActualValueTo(writer);
		}

		/// <summary>
		/// Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("not");
			this.baseConstraint.WriteDescriptionTo(writer);
		}

		#endregion
	}

	#endregion

	#region AllItemsConstraint

	/// <summary>
	/// AllItemsConstraint applies another constraint to each
	/// item in a collection, succeeding if they all succeed.
	/// </summary>
	public class AllItemsConstraint : PrefixConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct an AllItemsConstraint on top of an existing constraint
		/// </summary>
		/// <param name="itemConstraint"> </param>
		public AllItemsConstraint(Constraint itemConstraint)
			: base(itemConstraint)
		{
			this.DisplayName = "all";
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Apply the item constraint to each item in the collection,
		/// failing if any item fails.
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if (!(actual is IEnumerable))
				throw new ArgumentException("The actual value must be an IEnumerable", "actual");

			foreach (object item in (IEnumerable)actual)
			{
				if (!this.baseConstraint.Matches(item))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Write a description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("all items");
			this.baseConstraint.WriteDescriptionTo(writer);
		}

		#endregion
	}

	#endregion

	#region SomeItemsConstraint

	/// <summary>
	/// SomeItemsConstraint applies another constraint to each
	/// item in a collection, succeeding if any of them succeeds.
	/// </summary>
	public class SomeItemsConstraint : PrefixConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct a SomeItemsConstraint on top of an existing constraint
		/// </summary>
		/// <param name="itemConstraint"> </param>
		public SomeItemsConstraint(Constraint itemConstraint)
			: base(itemConstraint)
		{
			this.DisplayName = "some";
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Apply the item constraint to each item in the collection,
		/// succeeding if any item succeeds.
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if (!(actual is IEnumerable))
				throw new ArgumentException("The actual value must be an IEnumerable", "actual");

			foreach (object item in (IEnumerable)actual)
			{
				if (this.baseConstraint.Matches(item))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Write a description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("some item");
			this.baseConstraint.WriteDescriptionTo(writer);
		}

		#endregion
	}

	#endregion

	#region NoItemConstraint

	/// <summary>
	/// NoItemConstraint applies another constraint to each
	/// item in a collection, failing if any of them succeeds.
	/// </summary>
	public class NoItemConstraint : PrefixConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct a NoItemConstraint on top of an existing constraint
		/// </summary>
		/// <param name="itemConstraint"> </param>
		public NoItemConstraint(Constraint itemConstraint)
			: base(itemConstraint)
		{
			this.DisplayName = "none";
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Apply the item constraint to each item in the collection,
		/// failing if any item fails.
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if (!(actual is IEnumerable))
				throw new ArgumentException("The actual value must be an IEnumerable", "actual");

			foreach (object item in (IEnumerable)actual)
			{
				if (this.baseConstraint.Matches(item))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Write a description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("no item");
			this.baseConstraint.WriteDescriptionTo(writer);
		}

		#endregion
	}

	#endregion

	#region ExactCountConstraint

	/// <summary>
	/// ExactCoutConstraint applies another constraint to each
	/// item in a collection, succeeding only if a specified
	/// number of items succeed.
	/// </summary>
	public class ExactCountConstraint : PrefixConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct an ExactCountConstraint on top of an existing constraint
		/// </summary>
		/// <param name="expectedCount"> </param>
		/// <param name="itemConstraint"> </param>
		public ExactCountConstraint(int expectedCount, Constraint itemConstraint)
			: base(itemConstraint)
		{
			this.DisplayName = "one";
			this.expectedCount = expectedCount;
		}

		#endregion

		#region Fields/Constants

		private int expectedCount;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Apply the item constraint to each item in the collection,
		/// succeeding only if the expected number of items pass.
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if (!(actual is IEnumerable))
				throw new ArgumentException("The actual value must be an IEnumerable", "actual");

			int count = 0;
			foreach (object item in (IEnumerable)actual)
			{
				if (this.baseConstraint.Matches(item))
					count++;
			}

			return count == this.expectedCount;
		}

		/// <summary>
		/// Write a description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			switch (this.expectedCount)
			{
				case 0:
					writer.WritePredicate("no item");
					break;
				case 1:
					writer.WritePredicate("exactly one item");
					break;
				default:
					writer.WritePredicate("exactly " + this.expectedCount.ToString() + " items");
					break;
			}

			this.baseConstraint.WriteDescriptionTo(writer);
		}

		#endregion
	}

	#endregion
}