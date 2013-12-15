// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.Framework.Constraints
{
	/// <summary>
	/// Tests whether a value is less than the value supplied to its constructor
	/// </summary>
	public class LessThanConstraint : ComparisonConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:LessThanConstraint" /> class.
		/// </summary>
		/// <param name="expected"> The expected value. </param>
		public LessThanConstraint(object expected)
			: base(expected)
		{
			this.expected = expected;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// The value against which a comparison is to be made
		/// </summary>
		private object expected;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Test whether the constraint is satisfied by a given value
		/// </summary>
		/// <param name="actual"> The value to be tested </param>
		/// <returns> True for success, false for failure </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if (this.expected == null || actual == null)
				throw new ArgumentException("Cannot compare using a null reference");

			return this.comparer.Compare(actual, this.expected) < 0;
		}

		/// <summary>
		/// Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("less than");
			writer.WriteExpectedValue(this.expected);
		}

		#endregion
	}
}