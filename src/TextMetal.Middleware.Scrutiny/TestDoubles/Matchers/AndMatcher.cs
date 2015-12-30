#region Using

using System.IO;

#endregion

namespace NMock.Matchers
{
	/// <summary>
	/// Matcher that is the logical and combination of two matchers.
	/// </summary>
	public class AndMatcher : BinaryOperator
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AndMatcher" /> class.
		/// </summary>
		/// <param name="left"> The left operand. </param>
		/// <param name="right"> The right operand. </param>
		public AndMatcher(Matcher left, Matcher right)
			: base(left, right)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Describes this matcher.
		/// </summary>
		/// <param name="writer"> The text writer to which the description is added. </param>
		public override void DescribeTo(TextWriter writer)
		{
			writer.Write("'");
			this.Left.DescribeTo(writer);
			writer.Write("' and '");
			this.Right.DescribeTo(writer);
			writer.Write("'");
		}

		/// <summary>
		/// Matches the specified object to this matcher and returns whether it matches.
		/// </summary>
		/// <param name="o"> The object to match. </param>
		/// <returns> Returns whether the object matches. </returns>
		public override bool Matches(object o)
		{
			return this.Left.Matches(o) && this.Right.Matches(o);
		}

		#endregion
	}
}