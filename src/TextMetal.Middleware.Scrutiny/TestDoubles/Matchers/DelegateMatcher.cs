using System;
using System.Reflection;

namespace NMock.Matchers
{
	/// <summary>
	/// Represents a delegate in an expectation that can be matched
	/// </summary>
	public class DelegateMatcher : Matcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes an instance of this class with a <see cref="MulticastDelegate" /> to match.
		/// </summary>
		/// <param name="delegate"> </param>
		public DelegateMatcher(MulticastDelegate @delegate)
			: base("<" + @delegate.GetMethodInfo().Name + "[" + @delegate + "]>")
		{
			this._delegate = @delegate;
		}

		#endregion

		#region Fields/Constants

		private readonly MulticastDelegate _delegate;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Determines if this delegate matches the specified parameter
		/// </summary>
		/// <param name="o"> The delegate to match </param>
		/// <returns> true if the delegates match, false if the object is null or does not match </returns>
		public override bool Matches(object o)
		{
			return o != null && this._delegate.Equals(o);
		}

		#endregion
	}
}