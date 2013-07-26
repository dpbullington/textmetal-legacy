// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org/
// ****************************************************************

using System;

namespace NUnit.Framework.Constraints
{
	/// <summary>
	/// 	Modes in which the tolerance value for a comparison can
	/// 	be interpreted.
	/// </summary>
	public enum ToleranceMode
	{
		/// <summary>
		/// 	The tolerance was created with a value, without specifying 
		/// 	how the value would be used. This is used to prevent setting
		/// 	the mode more than once and is generally changed to Linear
		/// 	upon execution of the test.
		/// </summary>
		None,

		/// <summary>
		/// 	The tolerance is used as a numeric range within which
		/// 	two compared values are considered to be equal.
		/// </summary>
		Linear,

		/// <summary>
		/// 	Interprets the tolerance as the percentage by which
		/// 	the two compared values my deviate from each other.
		/// </summary>
		Percent,

		/// <summary>
		/// 	Compares two values based in their distance in
		/// 	representable numbers.
		/// </summary>
		Ulps
	}

	/// <summary>
	/// 	The Tolerance class generalizes the notion of a tolerance
	/// 	within which an equality test succeeds. Normally, it is
	/// 	used with numeric types, but it can be used with any
	/// 	type that supports taking a difference between two 
	/// 	objects and comparing that difference to a value.
	/// </summary>
	public class Tolerance
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Constructs a linear tolerance of a specdified amount
		/// </summary>
		public Tolerance(object amount)
			: this(amount, ToleranceMode.Linear)
		{
		}

		/// <summary>
		/// 	Constructs a tolerance given an amount and ToleranceMode
		/// </summary>
		public Tolerance(object amount, ToleranceMode mode)
		{
			this.amount = amount;
			this.mode = mode;
		}

		#endregion

		#region Fields/Constants

		private static readonly string ModeMustFollowTolerance =
			"Tolerance amount must be specified before setting mode";

		private static readonly string MultipleToleranceModes =
			"Tried to use multiple tolerance modes at the same time";

		private static readonly string NumericToleranceRequired =
			"A numeric tolerance is required";

		private object amount;
		private ToleranceMode mode;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Returns an empty Tolerance object, equivalent to
		/// 	specifying no tolerance. In most cases, it results
		/// 	in an exact match but for floats and doubles a
		/// 	default tolerance may be used.
		/// </summary>
		public static Tolerance Empty
		{
			get
			{
				return new Tolerance(0, ToleranceMode.None);
			}
		}

		/// <summary>
		/// 	Returns a zero Tolerance object, equivalent to 
		/// 	specifying an exact match.
		/// </summary>
		public static Tolerance Zero
		{
			get
			{
				return new Tolerance(0, ToleranceMode.Linear);
			}
		}

		/// <summary>
		/// 	Returns a new tolerance with a TimeSpan as the amount, using 
		/// 	the current amount as a number of days.
		/// </summary>
		public Tolerance Days
		{
			get
			{
				this.CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromDays(Convert.ToDouble(this.amount)));
			}
		}

		/// <summary>
		/// 	Returns a new tolerance with a TimeSpan as the amount, using 
		/// 	the current amount as a number of hours.
		/// </summary>
		public Tolerance Hours
		{
			get
			{
				this.CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromHours(Convert.ToDouble(this.amount)));
			}
		}

		/// <summary>
		/// 	Returns true if the current tolerance is empty.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.mode == ToleranceMode.None;
			}
		}

		/// <summary>
		/// 	Returns a new tolerance with a TimeSpan as the amount, using 
		/// 	the current amount as a number of milliseconds.
		/// </summary>
		public Tolerance Milliseconds
		{
			get
			{
				this.CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromMilliseconds(Convert.ToDouble(this.amount)));
			}
		}

		/// <summary>
		/// 	Returns a new tolerance with a TimeSpan as the amount, using 
		/// 	the current amount as a number of minutes.
		/// </summary>
		public Tolerance Minutes
		{
			get
			{
				this.CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromMinutes(Convert.ToDouble(this.amount)));
			}
		}

		/// <summary>
		/// 	Gets the ToleranceMode for the current Tolerance
		/// </summary>
		public ToleranceMode Mode
		{
			get
			{
				return this.mode;
			}
		}

		/// <summary>
		/// 	Returns a new tolerance, using the current amount as a percentage.
		/// </summary>
		public Tolerance Percent
		{
			get
			{
				this.CheckLinearAndNumeric();
				return new Tolerance(this.amount, ToleranceMode.Percent);
			}
		}

		/// <summary>
		/// 	Returns a new tolerance with a TimeSpan as the amount, using 
		/// 	the current amount as a number of seconds.
		/// </summary>
		public Tolerance Seconds
		{
			get
			{
				this.CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromSeconds(Convert.ToDouble(this.amount)));
			}
		}

		/// <summary>
		/// 	Returns a new tolerance with a TimeSpan as the amount, using 
		/// 	the current amount as a number of clock ticks.
		/// </summary>
		public Tolerance Ticks
		{
			get
			{
				this.CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromTicks(Convert.ToInt64(this.amount)));
			}
		}

		/// <summary>
		/// 	Returns a new tolerance, using the current amount in Ulps.
		/// </summary>
		public Tolerance Ulps
		{
			get
			{
				this.CheckLinearAndNumeric();
				return new Tolerance(this.amount, ToleranceMode.Ulps);
			}
		}

		/// <summary>
		/// 	Gets the value of the current Tolerance instance.
		/// </summary>
		public object Value
		{
			get
			{
				return this.amount;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Tests that the current Tolerance is linear with a 
		/// 	numeric value, throwing an exception if it is not.
		/// </summary>
		private void CheckLinearAndNumeric()
		{
			if (this.mode != ToleranceMode.Linear)
			{
				throw new InvalidOperationException(this.mode == ToleranceMode.None
					                                    ? ModeMustFollowTolerance
					                                    : MultipleToleranceModes);
			}

			if (!Numerics.IsNumericType(this.amount))
				throw new InvalidOperationException(NumericToleranceRequired);
		}

		#endregion
	}
}