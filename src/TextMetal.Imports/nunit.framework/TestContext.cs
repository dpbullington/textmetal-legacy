// ****************************************************************
// Copyright 2010, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;

namespace NUnit.Framework
{
	/// <summary>
	/// 	Provide the context information of the current test
	/// </summary>
	public class TestContext
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Constructs a TestContext using the provided context dictionary
		/// </summary>
		/// <param name="context"> A context dictionary </param>
		public TestContext(IDictionary context)
		{
			this._context = context;
		}

		#endregion

		#region Fields/Constants

		private const string contextKey = "NUnit.Framework.TestContext";
		private const string stateKey = "Result.State";

		private IDictionary _context;

		private ResultAdapter _result;
		private TestAdapter _test;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Get the current test context. This is created
		/// 	as needed. The user may save the context for
		/// 	use within a test, but it should not be used
		/// 	outside the test for which it is created.
		/// </summary>
		public static TestContext CurrentContext
		{
			get
			{
				return new TestContext((IDictionary)CallContext.GetData(contextKey));
			}
		}

		/// <summary>
		/// 	Gets a ResultAdapter representing the current result for the test 
		/// 	executing in this context.
		/// </summary>
		public ResultAdapter Result
		{
			get
			{
				if (this._result == null)
					this._result = new ResultAdapter(this._context);

				return this._result;
			}
		}

		/// <summary>
		/// 	Gets a TestAdapter representing the currently executing test in this context.
		/// </summary>
		public TestAdapter Test
		{
			get
			{
				if (this._test == null)
					this._test = new TestAdapter(this._context);

				return this._test;
			}
		}

		/// <summary>
		/// 	Gets the directory containing the current test assembly.
		/// </summary>
		public string TestDirectory
		{
			get
			{
				return (string)this._context["TestDirectory"];
			}
		}

		/// <summary>
		/// 	Gets the directory to be used for outputing files created
		/// 	by this test run.
		/// </summary>
		public string WorkDirectory
		{
			get
			{
				return (string)this._context["WorkDirectory"];
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// 	ResultAdapter adapts a TestResult for consumption by
		/// 	the user test code.
		/// </summary>
		public class ResultAdapter
		{
			#region Constructors/Destructors

			/// <summary>
			/// 	Construct a ResultAdapter for a context
			/// </summary>
			/// <param name="context"> The context holding the result </param>
			public ResultAdapter(IDictionary context)
			{
				this._context = context;
			}

			#endregion

			#region Fields/Constants

			private IDictionary _context;

			#endregion

			#region Properties/Indexers/Events

			/// <summary>
			/// 	The TestState of current test. This maps to the ResultState
			/// 	used in nunit.core and is subject to change in the future.
			/// </summary>
			public TestState State
			{
				get
				{
					return (TestState)this._context["Result.State"];
				}
			}

			/// <summary>
			/// 	The TestStatus of current test. This enum will be used
			/// 	in future versions of NUnit and so is to be preferred
			/// 	to the TestState value.
			/// </summary>
			public TestStatus Status
			{
				get
				{
					switch (this.State)
					{
						default:
						case TestState.Inconclusive:
							return TestStatus.Inconclusive;
						case TestState.Skipped:
						case TestState.Ignored:
						case TestState.NotRunnable:
							return TestStatus.Skipped;
						case TestState.Success:
							return TestStatus.Passed;
						case TestState.Failure:
						case TestState.Error:
						case TestState.Cancelled:
							return TestStatus.Failed;
					}
				}
			}

			#endregion
		}

		/// <summary>
		/// 	TestAdapter adapts a Test for consumption by
		/// 	the user test code.
		/// </summary>
		public class TestAdapter
		{
			#region Constructors/Destructors

			/// <summary>
			/// 	Constructs a TestAdapter for this context
			/// </summary>
			/// <param name="context"> The context dictionary </param>
			public TestAdapter(IDictionary context)
			{
				this._context = context;
			}

			#endregion

			#region Fields/Constants

			private IDictionary _context;

			#endregion

			#region Properties/Indexers/Events

			/// <summary>
			/// 	The FullName of the test
			/// </summary>
			public string FullName
			{
				get
				{
					return this._context["Test.FullName"] as string;
				}
			}

			/// <summary>
			/// 	The name of the test.
			/// </summary>
			public string Name
			{
				get
				{
					return this._context["Test.Name"] as string;
				}
			}

			/// <summary>
			/// 	The properties of the test.
			/// </summary>
			public IDictionary Properties
			{
				get
				{
					return this._context["Test.Properties"] as IDictionary;
				}
			}

			#endregion
		}

		#endregion
	}
}