namespace NMock.Matchers
{
	/// <summary>
	/// BinaryOperator is an abstract base class for matchers that combine two matchers into a single matcher.
	/// </summary>
	public abstract class BinaryOperator : Matcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BinaryOperator" /> class.
		/// </summary>
		/// <param name="left"> The left operand. </param>
		/// <param name="right"> The right operand. </param>
		protected BinaryOperator(Matcher left, Matcher right)
		{
			this.Left = left;
			this.Right = right;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// The left hand side of the binary operator.
		/// </summary>
		protected readonly Matcher Left;

		/// <summary>
		/// The right hand side of the binary operator.
		/// </summary>
		protected readonly Matcher Right;

		#endregion
	}
}