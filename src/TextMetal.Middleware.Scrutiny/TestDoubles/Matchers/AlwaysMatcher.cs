namespace NMock.Matchers
{
	/// <summary>
	/// A matcher that will always or never match independent of the value matched but depending on how it is initialized.
	/// </summary>
	public class AlwaysMatcher : Matcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AlwaysMatcher" /> class.
		/// </summary>
		/// <param name="matches"> if set to <c> true </c> the matcher will always match, otherwise it will never match. </param>
		/// <param name="description"> The description which will be printed out when calling <see cref="Matcher.DescribeTo" />. </param>
		public AlwaysMatcher(bool matches, string description)
			: base(description)
		{
			this._matches = matches;
		}

		#endregion

		#region Fields/Constants

		private readonly bool _matches;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Matches the specified object to this matcher and returns whether it matches.
		/// </summary>
		/// <param name="o"> The object to match. </param>
		/// <returns> Returns whether the object matches. </returns>
		public override bool Matches(object o)
		{
			return this._matches;
		}

		#endregion
	}
}