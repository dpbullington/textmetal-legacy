// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Reflection;

namespace NUnit.Framework.Constraints
{
	/// <summary>
	/// 	AttributeExistsConstraint tests for the presence of a
	/// 	specified attribute on  a Type.
	/// </summary>
	public class AttributeExistsConstraint : Constraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Constructs an AttributeExistsConstraint for a specific attribute Type
		/// </summary>
		/// <param name="type"> </param>
		public AttributeExistsConstraint(Type type)
			: base(type)
		{
			this.expectedType = type;

			if (!typeof(Attribute).IsAssignableFrom(this.expectedType))
			{
				throw new ArgumentException(string.Format(
					"Type {0} is not an attribute", this.expectedType), "type");
			}
		}

		#endregion

		#region Fields/Constants

		private Type expectedType;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Tests whether the object provides the expected attribute.
		/// </summary>
		/// <param name="actual"> A Type, MethodInfo, or other ICustomAttributeProvider </param>
		/// <returns> True if the expected attribute is present, otherwise false </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;
			ICustomAttributeProvider attrProvider =
				actual as ICustomAttributeProvider;

			if (attrProvider == null)
				throw new ArgumentException(string.Format("Actual value {0} does not implement ICustomAttributeProvider", actual), "actual");

			return attrProvider.GetCustomAttributes(this.expectedType, true).Length > 0;
		}

		/// <summary>
		/// 	Writes the description of the constraint to the specified writer
		/// </summary>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("type with attribute");
			writer.WriteExpectedValue(this.expectedType);
		}

		#endregion
	}

	/// <summary>
	/// 	AttributeConstraint tests that a specified attribute is present
	/// 	on a Type or other provider and that the value of the attribute
	/// 	satisfies some other constraint.
	/// </summary>
	public class AttributeConstraint : PrefixConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Constructs an AttributeConstraint for a specified attriute
		/// 	Type and base constraint.
		/// </summary>
		/// <param name="type"> </param>
		/// <param name="baseConstraint"> </param>
		public AttributeConstraint(Type type, Constraint baseConstraint)
			: base(baseConstraint)
		{
			this.expectedType = type;

			if (!typeof(Attribute).IsAssignableFrom(this.expectedType))
			{
				throw new ArgumentException(string.Format(
					"Type {0} is not an attribute", this.expectedType), "type");
			}
		}

		#endregion

		#region Fields/Constants

		private Attribute attrFound;
		private Type expectedType;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Returns a string representation of the constraint.
		/// </summary>
		protected override string GetStringRepresentation()
		{
			return string.Format("<attribute {0} {1}>", this.expectedType, this.baseConstraint);
		}

		/// <summary>
		/// 	Determines whether the Type or other provider has the 
		/// 	expected attribute and if its value matches the
		/// 	additional constraint specified.
		/// </summary>
		public override bool Matches(object actual)
		{
			this.actual = actual;
			ICustomAttributeProvider attrProvider =
				actual as ICustomAttributeProvider;

			if (attrProvider == null)
				throw new ArgumentException(string.Format("Actual value {0} does not implement ICustomAttributeProvider", actual), "actual");

			Attribute[] attrs = (Attribute[])attrProvider.GetCustomAttributes(this.expectedType, true);
			if (attrs.Length == 0)
				throw new ArgumentException(string.Format("Attribute {0} was not found", this.expectedType), "actual");

			this.attrFound = attrs[0];
			return this.baseConstraint.Matches(this.attrFound);
		}

		/// <summary>
		/// 	Writes the actual value supplied to the specified writer.
		/// </summary>
		public override void WriteActualValueTo(MessageWriter writer)
		{
			writer.WriteActualValue(this.attrFound);
		}

		/// <summary>
		/// 	Writes a description of the attribute to the specified writer.
		/// </summary>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("attribute " + this.expectedType.FullName);
			if (this.baseConstraint != null)
			{
				if (this.baseConstraint is EqualConstraint)
					writer.WritePredicate("equal to");
				this.baseConstraint.WriteDescriptionTo(writer);
			}
		}

		#endregion
	}
}