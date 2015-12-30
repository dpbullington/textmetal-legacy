#region Using

using System;
using System.IO;

#endregion

namespace NMock.Matchers
{
	/// <summary>
	/// Matcher that checks a value against upper and lower bounds.
	/// </summary>
	public class ComparisonMatcher : Matcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ComparisonMatcher" /> class.
		/// </summary>
		/// <param name="value"> The value to compare. </param>
		/// <param name="comparisonResult1"> The first allowed comparison result (result of value.CompareTo(other)). </param>
		/// <param name="comparisonResult2"> The second allowed comparison result (result of value.CompareTo(other)). </param>
		/// <exception cref="ArgumentException"> Thrown when one value is -1 and the other is 1. </exception>
		public ComparisonMatcher(IComparable value, int comparisonResult1, int comparisonResult2)
		{
			this.value = value;
			this.minComparisonResult = Math.Min(comparisonResult1, comparisonResult2);
			this.maxComparisonResult = Math.Max(comparisonResult1, comparisonResult2);

			if (this.minComparisonResult == -1 && this.maxComparisonResult == 1)
				throw new ArgumentException("comparison result range too large");
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Stores the maximum comparison result for a successful match.
		/// </summary>
		private readonly int maxComparisonResult;

		/// <summary>
		/// Stores the minimum comparison result for a successful match.
		/// </summary>
		private readonly int minComparisonResult;

		/// <summary>
		/// Stores the value to be compared.
		/// </summary>
		private readonly IComparable value;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Describes this matcher.
		/// </summary>
		/// <param name="writer"> The text writer to which the description is added. </param>
		public override void DescribeTo(TextWriter writer)
		{
			writer.Write("? ");
			if (this.minComparisonResult == -1)
				writer.Write("<");

			if (this.maxComparisonResult == 1)
				writer.Write(">");

			if (this.minComparisonResult == 0 || this.maxComparisonResult == 0)
				writer.Write("=");

			writer.Write(" ");
			writer.Write(this.value);
		}

		/// <summary>
		/// Matches the specified object to this matcher and returns whether it matches.
		/// </summary>
		/// <param name="o"> The object to match. </param>
		/// <returns> Whether the object compared to the value resulted in either of both specified comparison results. </returns>
		public override bool Matches(object o)
		{
			if (o.GetType() == this.value.GetType())
			{
				int comparisonResult = -this.value.CompareTo(o);
				return comparisonResult >= this.minComparisonResult
						&& comparisonResult <= this.maxComparisonResult;
			}
			else
				return false;
		}

		#endregion
	}
}