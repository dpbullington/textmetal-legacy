#region Using

using System.IO;

#endregion

namespace NMock.Matchers
{
	/// <summary>
	/// Matcher that combines two matcher with a logically or.
	/// </summary>
	public class OrMatcher : BinaryOperator
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="OrMatcher" /> class.
		/// </summary>
		/// <param name="left"> The left operand. </param>
		/// <param name="right"> The right operand. </param>
		public OrMatcher(Matcher left, Matcher right)
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
			writer.Write("' or '");
			this.Right.DescribeTo(writer);
			writer.Write("'");
		}

		/// <summary>
		/// Matches the specified object to this matcher and returns whether it matches.
		/// </summary>
		/// <param name="o"> The object to match. </param>
		/// <returns> Whether the object matches one of the two combined matchers. </returns>
		public override bool Matches(object o)
		{
			return this.Left.Matches(o) || this.Right.Matches(o);
		}

		#endregion
	}
}