#region Using

using System.IO;

#endregion

namespace NMock.Matchers
{
	/// <summary>
	/// Matcher that checks whether the actual value contains the expected substring.
	/// </summary>
	public class StringContainsMatcher : Matcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="StringContainsMatcher" /> class.
		/// </summary>
		/// <param name="substring"> The substring that is expected. </param>
		public StringContainsMatcher(string substring)
		{
			this.substring = substring;
		}

		#endregion

		#region Fields/Constants

		private readonly string substring;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Describes this matcher.
		/// </summary>
		/// <param name="writer"> The text writer to which the description is added. </param>
		public override void DescribeTo(TextWriter writer)
		{
			writer.Write("string containing ");
			writer.Write((object)this.substring);
		}

		/// <summary>
		/// Matches the specified object to this matcher and returns whether it matches.
		/// </summary>
		/// <param name="o"> The object to match. </param>
		/// <returns> Whether the object is a string and contains the expected substring. </returns>
		public override bool Matches(object o)
		{
			return o != null
					&& o is string
					&& ((string)o).IndexOf(this.substring) >= 0;
		}

		#endregion
	}
}