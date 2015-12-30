using System.Collections;
using System.IO;
using System.Linq;

namespace NMock.Matchers
{
	/// <summary>
	/// </summary>
	/// <typeparam name="T"> </typeparam>
	public class CountMatcher<T> : Matcher
		where T : IEnumerable
	{
		#region Constructors/Destructors

		/// <summary>
		/// </summary>
		/// <param name="expectedCount"> </param>
		public CountMatcher(int expectedCount)
		{
			this._expectedCount = expectedCount;
		}

		#endregion

		#region Fields/Constants

		private readonly int _expectedCount;
		private int _actualCount;

		#endregion

		#region Methods/Operators

		public override void DescribeTo(TextWriter writer)
		{
			writer.Write(string.Format("CountMatcher: Actual count of {0} did not match expected count of {1}", this._actualCount, this._expectedCount));
		}

		public override bool Matches(object o)
		{
			var input = (T)o;
			if (input == null)
				return false;
			this._actualCount = input.Cast<object>().Count();
			return this._actualCount == this._expectedCount;
		}

		#endregion
	}
}