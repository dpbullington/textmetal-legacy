// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System.Runtime.Serialization;

namespace NUnit.Core
{
	using System;

	/// <summary>
	/// 	Summary description for NoTestMethodsException.
	/// </summary>
	[Serializable]
	public class InvalidTestFixtureException : ApplicationException
	{
		#region Constructors/Destructors

		public InvalidTestFixtureException()
			: base()
		{
		}

		public InvalidTestFixtureException(string message)
			: base(message)
		{
		}

		public InvalidTestFixtureException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// 	Serialization Constructor
		/// </summary>
		protected InvalidTestFixtureException(SerializationInfo info,
		                                      StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}