// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;

namespace NUnit.Core
{
	/// <summary>
	/// 	ParameterizedFixtureSuite serves as a container for the set of test 
	/// 	fixtures created from a given Type using various parameters.
	/// </summary>
	public class ParameterizedFixtureSuite : TestSuite
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="ParameterizedFixtureSuite" /> class.
		/// </summary>
		/// <param name="type"> The type. </param>
		public ParameterizedFixtureSuite(Type type)
			: base(type.Namespace, TypeHelper.GetDisplayName(type))
		{
			this.type = type;
		}

		#endregion

		#region Fields/Constants

		private Type type;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Gets the Type represented by this suite.
		/// </summary>
		/// <value> A Sysetm.Type. </value>
		public Type ParameterizedType
		{
			get
			{
				return this.type;
			}
		}

		/// <summary>
		/// 	Gets the type of the test.
		/// </summary>
		/// <value> The type of the test. </value>
		public override string TestType
		{
			get
			{
#if CLR_2_0 || CLR_4_0
				if (this.type.IsGenericType)
					return "GenericFixture";
#endif

				return "ParameterizedFixture";
			}
		}

		#endregion
	}
}