// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.Framework.Constraints
{
	/// <summary>
	/// 	BasicConstraint is the abstract base for constraints that
	/// 	perform a simple comparison to a constant value.
	/// </summary>
	public abstract class BasicConstraint : Constraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:BasicConstraint" /> class.
		/// </summary>
		/// <param name="expected"> The expected. </param>
		/// <param name="description"> The description. </param>
		public BasicConstraint(object expected, string description)
		{
			this.expected = expected;
			this.description = description;
		}

		#endregion

		#region Fields/Constants

		private string description;
		private object expected;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Test whether the constraint is satisfied by a given value
		/// </summary>
		/// <param name="actual"> The value to be tested </param>
		/// <returns> True for success, false for failure </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if (actual == null && this.expected == null)
				return true;

			if (actual == null || this.expected == null)
				return false;

			return this.expected.Equals(actual);
		}

		/// <summary>
		/// 	Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write(this.description);
		}

		#endregion
	}

	/// <summary>
	/// 	NullConstraint tests that the actual value is null
	/// </summary>
	public class NullConstraint : BasicConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:NullConstraint" /> class.
		/// </summary>
		public NullConstraint()
			: base(null, "null")
		{
		}

		#endregion
	}

	/// <summary>
	/// 	TrueConstraint tests that the actual value is true
	/// </summary>
	public class TrueConstraint : BasicConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:TrueConstraint" /> class.
		/// </summary>
		public TrueConstraint()
			: base(true, "True")
		{
		}

		#endregion
	}

	/// <summary>
	/// 	FalseConstraint tests that the actual value is false
	/// </summary>
	public class FalseConstraint : BasicConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:FalseConstraint" /> class.
		/// </summary>
		public FalseConstraint()
			: base(false, "False")
		{
		}

		#endregion
	}

	/// <summary>
	/// 	NaNConstraint tests that the actual value is a double or float NaN
	/// </summary>
	public class NaNConstraint : Constraint
	{
		#region Methods/Operators

		/// <summary>
		/// 	Test that the actual value is an NaN
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			return actual is double && double.IsNaN((double)actual)
			       || actual is float && float.IsNaN((float)actual);
		}

		/// <summary>
		/// 	Write the constraint description to a specified writer
		/// </summary>
		/// <param name="writer"> </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("NaN");
		}

		#endregion
	}
}