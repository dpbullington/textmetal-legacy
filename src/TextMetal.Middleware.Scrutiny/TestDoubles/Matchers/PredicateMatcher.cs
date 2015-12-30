using System;

namespace NMock.Matchers
{
	/// <summary>
	/// A matcher that uses lambda expressions to perform matching
	/// </summary>
	/// <typeparam name="T"> </typeparam>
	public class PredicateMatcher<T> : Matcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PredicateMatcher{T}" /> class with the supplied predicate.
		/// </summary>
		/// <param name="matcher"> A lambda expression that evaluates to true or false </param>
		public PredicateMatcher(Predicate<T> matcher)
		{
			this._matcher = matcher;
			this.Description = string.Format("Predicate<{0}> evaluation failed.", typeof(T).Name);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PredicateMatcher{T}" /> class with the supplied predicate.
		/// </summary>
		/// <param name="matcher"> A lambda expression that evaluates to true or false </param>
		/// <param name="description"> A message to describe the expectation. </param>
		public PredicateMatcher(Predicate<T> matcher, string description)
			: base(description)
		{
			this._matcher = matcher;
		}

		#endregion

		#region Fields/Constants

		private readonly Predicate<T> _matcher;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Matches the specified object to this matcher and returns whether it matches.
		/// </summary>
		/// <param name="o"> The object to match. </param>
		/// <returns> Whether the object matches. </returns>
		public override bool Matches(object o)
		{
			try
			{
				bool result = this._matcher.Invoke((T)o);
				return result;
			}
			catch (InvalidCastException)
			{
				if (o == null)
					this.Description += "  Match parameter was null";
				else
					this.Description += string.Format("  PredicateMatcher is constructed with a Predicate<{1}>, but a {0} was supplied.", o.GetType().Name, typeof(T).Name);
				return false;
			}
		}

		#endregion
	}
}