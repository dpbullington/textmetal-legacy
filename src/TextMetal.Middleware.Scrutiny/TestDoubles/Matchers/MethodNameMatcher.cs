#region Using

using System;
using System.Reflection;

#endregion

namespace NMock.Matchers
{
	/// <summary>
	/// Matcher that checks whether the actual object is a <see cref="MethodInfo" /> and its name is equal to the expected name.
	/// </summary>
	public class MethodNameMatcher : Matcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodNameMatcher" /> class.
		/// </summary>
		/// <param name="methodName"> The expected name of the method. </param>
		public MethodNameMatcher(string methodName)
			: this(methodName, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodNameMatcher" /> class with a method name and declaring type
		/// </summary>
		/// <param name="methodName"> </param>
		/// <param name="declaringType"> </param>
		public MethodNameMatcher(string methodName, Type declaringType)
			: base(methodName)
		{
			this._declaringType = declaringType;
		}

		#endregion

		#region Fields/Constants

		private readonly Type _declaringType;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Matches the specified object to this matcher and returns whether it matches.
		/// </summary>
		/// <param name="o"> The MethodInfo to match. </param>
		/// <returns> Whether the object is a MethodInfo and its name matches the expected one. </returns>
		public override bool Matches(object o)
		{
			MethodInfo methodInfo = o as MethodInfo;

			if (methodInfo == null)
				return false;

			return this.Matches(methodInfo);
		}

		protected bool Matches(MethodInfo methodInfo)
		{
			if (methodInfo != null)
			{
				string methodName = methodInfo.Name;

				if (this.Description.Equals(methodName))
					return true;

				//TODO: when is this used?
				if (this._declaringType != null && this._declaringType == methodInfo.DeclaringType && methodName.EndsWith(this.Description))
					return true;
			}
			return false;
		}

		#endregion
	}
}