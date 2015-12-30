#region Using

using System;
using System.IO;
using System.Reflection;

#endregion

namespace NMock.Matchers
{
	/// <summary>
	/// Matcher that checks whether the actual object is a <see cref="MethodInfo" /> and its signature matches the expected signature.
	/// </summary>
	public class MethodMatcher : MethodNameMatcher
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodMatcher" /> class.
		/// </summary>
		/// <param name="methodInfo"> The expected method reference. </param>
		public MethodMatcher(MethodInfo methodInfo)
			: base(methodInfo.Name, methodInfo.DeclaringType)
		{
			if (methodInfo == null)
				throw new ArgumentNullException("methodInfo");

			this._methodInfo = methodInfo;
			this._genericParametersMatcher = new GenericMethodTypeParametersMatcher(this._methodInfo.GetGenericArguments());
			this._expectedParameters = this._methodInfo.GetParameters();
		}

		#endregion

		#region Fields/Constants

		internal readonly MethodInfo _methodInfo;
		private ParameterInfo[] _expectedParameters;
		private GenericMethodTypeParametersMatcher _genericParametersMatcher;

		#endregion

		#region Properties/Indexers/Events

		internal string ReturnType
		{
			get
			{
				return this._methodInfo.ReturnType.FullName;
			}
		}

		#endregion

		#region Methods/Operators

		public override void DescribeTo(TextWriter writer)
		{
			writer.Write(this.Description);
			this._genericParametersMatcher.DescribeTo(writer);
		}

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

			if (methodInfo.IsGenericMethodDefinition && this._methodInfo.IsGenericMethod && methodInfo.GetGenericMethodDefinition() == this._methodInfo.GetGenericMethodDefinition()) //then this is an internal check and not a declaration
				return true;

			if (!this.Matches(methodInfo)) //call to the base to match the name
				return false;

			if (!this._genericParametersMatcher.MatchesTypes(methodInfo))
				return false;

			//check parameters
			var actualParameters = methodInfo.GetParameters();

			if (this._expectedParameters.Length != actualParameters.Length)
				return false;

			for (int i = 0; i < this._expectedParameters.Length; i++)
			{
				if (!this._expectedParameters[i].ParameterType.IsAssignableFrom(actualParameters[i].ParameterType))
					return false;
			}

			return true;
		}

		#endregion
	}
}