// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Text.RegularExpressions;
#if !NETCF

#endif

namespace NUnit.Framework.Constraints
{

	#region StringConstraint

	/// <summary>
	/// 	StringConstraint is the abstract base for constraints
	/// 	that operate on strings. It supports the IgnoreCase
	/// 	modifier for string operations.
	/// </summary>
	public abstract class StringConstraint : Constraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Constructs a StringConstraint given an expected value
		/// </summary>
		/// <param name="expected"> The expected value </param>
		public StringConstraint(string expected)
			: base(expected)
		{
			this.expected = expected;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// 	Indicates whether tests should be case-insensitive
		/// </summary>
		protected bool caseInsensitive;

		/// <summary>
		/// 	The expected value
		/// </summary>
		protected string expected;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Modify the constraint to ignore case in matching.
		/// </summary>
		public StringConstraint IgnoreCase
		{
			get
			{
				this.caseInsensitive = true;
				return this;
			}
		}

		#endregion
	}

	#endregion

	#region EmptyStringConstraint

	/// <summary>
	/// 	EmptyStringConstraint tests whether a string is empty.
	/// </summary>
	public class EmptyStringConstraint : Constraint
	{
		#region Methods/Operators

		/// <summary>
		/// 	Test whether the constraint is satisfied by a given value
		/// </summary>
		/// <param name="actual"> The value to be tested </param>
		/// <returns> True for success, false for failure </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if (!(actual is string))
				return false;

			return (string)actual == string.Empty;
		}

		/// <summary>
		/// 	Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("<empty>");
		}

		#endregion
	}

	#endregion

	#region NullOrEmptyStringConstraint

	/// <summary>
	/// 	NullEmptyStringConstraint tests whether a string is either null or empty.
	/// </summary>
	public class NullOrEmptyStringConstraint : Constraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Constructs a new NullOrEmptyStringConstraint
		/// </summary>
		public NullOrEmptyStringConstraint()
		{
			this.DisplayName = "nullorempty";
		}

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

			if (actual == null)
				return true;

			if (!(actual is string))
				throw new ArgumentException("Actual value must be a string", "actual");

			return (string)actual == string.Empty;
		}

		/// <summary>
		/// 	Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("null or empty string");
		}

		#endregion
	}

	#endregion

	#region Substring Constraint

	/// <summary>
	/// 	SubstringConstraint can test whether a string contains
	/// 	the expected substring.
	/// </summary>
	public class SubstringConstraint : StringConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:SubstringConstraint" /> class.
		/// </summary>
		/// <param name="expected"> The expected. </param>
		public SubstringConstraint(string expected)
			: base(expected)
		{
		}

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

			if (!(actual is string))
				return false;

			if (this.caseInsensitive)
				return ((string)actual).ToLower().IndexOf(this.expected.ToLower()) >= 0;
			else
				return ((string)actual).IndexOf(this.expected) >= 0;
		}

		/// <summary>
		/// 	Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String containing");
			writer.WriteExpectedValue(this.expected);
			if (this.caseInsensitive)
				writer.WriteModifier("ignoring case");
		}

		#endregion
	}

	#endregion

	#region StartsWithConstraint

	/// <summary>
	/// 	StartsWithConstraint can test whether a string starts
	/// 	with an expected substring.
	/// </summary>
	public class StartsWithConstraint : StringConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:StartsWithConstraint" /> class.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		public StartsWithConstraint(string expected)
			: base(expected)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Test whether the constraint is matched by the actual value.
		/// 	This is a template method, which calls the IsMatch method
		/// 	of the derived class.
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if (!(actual is string))
				return false;

			if (this.caseInsensitive)
				return ((string)actual).ToLower().StartsWith(this.expected.ToLower());
			else
				return ((string)actual).StartsWith(this.expected);
		}

		/// <summary>
		/// 	Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String starting with");
			writer.WriteExpectedValue(MsgUtils.ClipString(this.expected, writer.MaxLineLength - 40, 0));
			if (this.caseInsensitive)
				writer.WriteModifier("ignoring case");
		}

		#endregion
	}

	#endregion

	#region EndsWithConstraint

	/// <summary>
	/// 	EndsWithConstraint can test whether a string ends
	/// 	with an expected substring.
	/// </summary>
	public class EndsWithConstraint : StringConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:EndsWithConstraint" /> class.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		public EndsWithConstraint(string expected)
			: base(expected)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Test whether the constraint is matched by the actual value.
		/// 	This is a template method, which calls the IsMatch method
		/// 	of the derived class.
		/// </summary>
		/// <param name="actual"> </param>
		/// <returns> </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if (!(actual is string))
				return false;

			if (this.caseInsensitive)
				return ((string)actual).ToLower().EndsWith(this.expected.ToLower());
			else
				return ((string)actual).EndsWith(this.expected);
		}

		/// <summary>
		/// 	Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String ending with");
			writer.WriteExpectedValue(this.expected);
			if (this.caseInsensitive)
				writer.WriteModifier("ignoring case");
		}

		#endregion
	}

	#endregion

	#region RegexConstraint

#if !NETCF
	/// <summary>
	/// 	RegexConstraint can test whether a string matches
	/// 	the pattern provided.
	/// </summary>
	public class RegexConstraint : StringConstraint
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="T:RegexConstraint" /> class.
		/// </summary>
		/// <param name="pattern"> The pattern. </param>
		public RegexConstraint(string pattern)
			: base(pattern)
		{
		}

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

			return actual is string &&
			       Regex.IsMatch(
				       (string)actual,
				       this.expected,
				       this.caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
		}

		/// <summary>
		/// 	Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String matching");
			writer.WriteExpectedValue(this.expected);
			if (this.caseInsensitive)
				writer.WriteModifier("ignoring case");
		}

		#endregion
	}
#endif

	#endregion
}