// ****************************************************************
// Copyright 2012, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

#if CLR_2_0 || CLR_4_0

using System;
using System.Reflection;

namespace NUnit.Framework
{
	/// <summary>
	/// 	Provides details about a test
	/// </summary>
	public class TestDetails
	{
		#region Constructors/Destructors

		///<summary>
		///	Creates an instance of TestDetails
		///</summary>
		///<param name="fixture"> The fixture that the test is a member of, if available. </param>
		///<param name="method"> The method that implements the test, if available. </param>
		///<param name="fullName"> The full name of the test. </param>
		///<param name="type"> A string representing the type of test, e.g. "Test Case". </param>
		///<param name="isSuite"> Indicates if the test represents a suite of tests. </param>
		public TestDetails(object fixture, MethodInfo method, string fullName, string type, bool isSuite)
		{
			this.Fixture = fixture;
			this.Method = method;

			this.FullName = fullName;
			this.Type = type;
			this.IsSuite = isSuite;
		}

		#endregion

		#region Properties/Indexers/Events

		///<summary>
		///	The fixture that the test is a member of, if available.
		///</summary>
		public object Fixture
		{
			get;
			private set;
		}

		/// <summary>
		/// 	The full name of the test.
		/// </summary>
		public string FullName
		{
			get;
			private set;
		}

		/// <summary>
		/// 	Indicates if the test represents a suite of tests.
		/// </summary>
		public bool IsSuite
		{
			get;
			private set;
		}

		/// <summary>
		/// 	The method that implements the test, if available.
		/// </summary>
		public MethodInfo Method
		{
			get;
			private set;
		}

		/// <summary>
		/// 	A string representing the type of test, e.g. "Test Case".
		/// </summary>
		public string Type
		{
			get;
			private set;
		}

		#endregion
	}
}

#endif