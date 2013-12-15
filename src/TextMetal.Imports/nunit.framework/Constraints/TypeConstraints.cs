// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.Framework.Constraints
{
	/// <summary>
	/// TypeConstraint is the abstract base for constraints
	/// that take a Type as their expected value.
	/// </summary>
	public abstract class TypeConstraint : Constraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct a TypeConstraint for a given Type
		/// </summary>
		/// <param name="type"> </param>
		public TypeConstraint(Type type)
			: base(type)
		{
			this.expectedType = type;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// The expected Type used by the constraint
		/// </summary>
		protected Type expectedType;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Write the actual value for a failing constraint test to a
		/// MessageWriter. TypeConstraints override this method to write
		/// the name of the type.
		/// </summary>
		/// <param name="writer"> The writer on which the actual value is displayed </param>
		public override void WriteActualValueTo(MessageWriter writer)
		{
			writer.WriteActualValue(this.actual == null ? null : this.actual.GetType());
		}

		#endregion
	}

	/// <summary>
	/// ExactTypeConstraint is used to test that an object
	/// is of the exact type provided in the constructor
	/// </summary>
	public class ExactTypeConstraint : TypeConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct an ExactTypeConstraint for a given Type
		/// </summary>
		/// <param name="type"> The expected Type. </param>
		public ExactTypeConstraint(Type type)
			: base(type)
		{
			this.DisplayName = "typeof";
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Test that an object is of the exact type specified
		/// </summary>
		/// <param name="actual"> The actual value. </param>
		/// <returns> True if the tested object is of the exact type provided, otherwise false. </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;
			return actual != null && actual.GetType() == this.expectedType;
		}

		/// <summary>
		/// Write the description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> The MessageWriter to use </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WriteExpectedValue(this.expectedType);
		}

		#endregion
	}

	/// <summary>
	/// ExceptionTypeConstraint is a special version of ExactTypeConstraint
	/// used to provided detailed info about the exception thrown in
	/// an error message.
	/// </summary>
	public class ExceptionTypeConstraint : ExactTypeConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Constructs an ExceptionTypeConstraint
		/// </summary>
		public ExceptionTypeConstraint(Type type)
			: base(type)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Write the actual value for a failing constraint test to a
		/// MessageWriter. Overriden to write additional information
		/// in the case of an Exception.
		/// </summary>
		/// <param name="writer"> The MessageWriter to use </param>
		public override void WriteActualValueTo(MessageWriter writer)
		{
			Exception ex = this.actual as Exception;
			base.WriteActualValueTo(writer);

			if (ex != null)
			{
				writer.WriteLine(" ({0})", ex.Message);
				writer.Write(ex.StackTrace);
			}
		}

		#endregion
	}

	/// <summary>
	/// InstanceOfTypeConstraint is used to test that an object
	/// is of the same type provided or derived from it.
	/// </summary>
	public class InstanceOfTypeConstraint : TypeConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct an InstanceOfTypeConstraint for the type provided
		/// </summary>
		/// <param name="type"> The expected Type </param>
		public InstanceOfTypeConstraint(Type type)
			: base(type)
		{
			this.DisplayName = "instanceof";
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Test whether an object is of the specified type or a derived type
		/// </summary>
		/// <param name="actual"> The object to be tested </param>
		/// <returns> True if the object is of the provided type or derives from it, otherwise false. </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;
			return actual != null && this.expectedType.IsInstanceOfType(actual);
		}

		/// <summary>
		/// Write a description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> The MessageWriter to use </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("instance of");
			writer.WriteExpectedValue(this.expectedType);
		}

		#endregion
	}

	/// <summary>
	/// AssignableFromConstraint is used to test that an object
	/// can be assigned from a given Type.
	/// </summary>
	public class AssignableFromConstraint : TypeConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct an AssignableFromConstraint for the type provided
		/// </summary>
		/// <param name="type"> </param>
		public AssignableFromConstraint(Type type)
			: base(type)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Test whether an object can be assigned from the specified type
		/// </summary>
		/// <param name="actual"> The object to be tested </param>
		/// <returns> True if the object can be assigned a value of the expected Type, otherwise false. </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;
			return actual != null && actual.GetType().IsAssignableFrom(this.expectedType);
		}

		/// <summary>
		/// Write a description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> The MessageWriter to use </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("assignable from");
			writer.WriteExpectedValue(this.expectedType);
		}

		#endregion
	}

	/// <summary>
	/// AssignableToConstraint is used to test that an object
	/// can be assigned to a given Type.
	/// </summary>
	public class AssignableToConstraint : TypeConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct an AssignableToConstraint for the type provided
		/// </summary>
		/// <param name="type"> </param>
		public AssignableToConstraint(Type type)
			: base(type)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Test whether an object can be assigned to the specified type
		/// </summary>
		/// <param name="actual"> The object to be tested </param>
		/// <returns> True if the object can be assigned a value of the expected Type, otherwise false. </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;
			return actual != null && this.expectedType.IsAssignableFrom(actual.GetType());
		}

		/// <summary>
		/// Write a description of this constraint to a MessageWriter
		/// </summary>
		/// <param name="writer"> The MessageWriter to use </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("assignable to");
			writer.WriteExpectedValue(this.expectedType);
		}

		#endregion
	}
}