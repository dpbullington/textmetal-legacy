using System;

namespace TextMetal.Framework.Expression
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
	public sealed class OperatorTextAttribute : Attribute
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the OperatorTextAttribute class.
		/// </summary>
		public OperatorTextAttribute(string value)
		{
		}

		#endregion
	}
}