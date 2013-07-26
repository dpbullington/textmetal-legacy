// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System.Collections;
using System.Text;

namespace NUnit.Core
{
	using System;

	/// <summary>
	/// 	The TestResult class represents
	/// 	the result of a test and is used to
	/// 	communicate results across AppDomains.
	/// </summary>
	[Serializable]
	public class TestResult
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Construct a test result given a TestInfo
		/// </summary>
		/// <param name="test"> The test to be used </param>
		public TestResult(TestInfo test)
		{
			this.test = test;
			this.message = test.IgnoreReason;
		}

		/// <summary>
		/// 	Construct a TestResult given an ITest
		/// </summary>
		/// <param name="test"> </param>
		public TestResult(ITest test)
			: this(new TestInfo(test))
		{
		}

		/// <summary>
		/// 	Construct a TestResult given a TestName
		/// </summary>
		/// <param name="testName"> A TestName </param>
		public TestResult(TestName testName)
			: this(new TestInfo(testName))
		{
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// 	The test that this result pertains to
		/// </summary>
		private readonly TestInfo test;

		/// <summary>
		/// 	Number of asserts executed by this test
		/// </summary>
		private int assertCount = 0;

		/// <summary>
		/// 	Indicates the location of a failure
		/// </summary>
		private FailureSite failureSite;

		/// <summary>
		/// 	Message giving the reason for failure
		/// </summary>
		private string message;

		/// <summary>
		/// 	Indicates the result of the test
		/// </summary>
		private ResultState resultState;

		/// <summary>
		/// 	List of child results
		/// </summary>
		private IList results;

		/// <summary>
		/// 	The stacktrace at the point of failure
		/// </summary>
		private string stackTrace;

		/// <summary>
		/// 	The elapsed time for executing this test
		/// </summary>
		private double time = 0.0;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Gets or sets the count of asserts executed
		/// 	when running the test.
		/// </summary>
		public int AssertCount
		{
			get
			{
				return this.assertCount;
			}
			set
			{
				this.assertCount = value;
			}
		}

		/// <summary>
		/// 	Gets a description associated with the test
		/// </summary>
		public string Description
		{
			get
			{
				return this.test.Description;
			}
		}

		/// <summary>
		/// 	Indicates whether the test executed
		/// </summary>
		public bool Executed
		{
			get
			{
				return this.resultState == ResultState.Success ||
				       this.resultState == ResultState.Failure ||
				       this.resultState == ResultState.Error ||
				       this.resultState == ResultState.Inconclusive;
			}
		}

		/// <summary>
		/// 	Gets the stage of the test in which a failure
		/// 	or error occured.
		/// </summary>
		public FailureSite FailureSite
		{
			get
			{
				return this.failureSite;
			}
		}

		/// <summary>
		/// 	Gets the full name of the test result
		/// </summary>
		public virtual string FullName
		{
			get
			{
				return this.test.TestName.FullName;
			}
		}

		/// <summary>
		/// 	Return true if this result has any child results
		/// </summary>
		public bool HasResults
		{
			get
			{
				return this.results != null && this.results.Count > 0;
			}
		}

		/// <summary>
		/// 	Indicates whether the test had an error (as opposed to a failure)
		/// </summary>
		public virtual bool IsError
		{
			get
			{
				return this.resultState == ResultState.Error;
			}
		}

		/// <summary>
		/// 	Indicates whether the test failed
		/// </summary>
		public virtual bool IsFailure
		{
			get
			{
				return this.resultState == ResultState.Failure;
			}
		}

		/// <summary>
		/// 	Indicates whether the test ran successfully
		/// </summary>
		public virtual bool IsSuccess
		{
			get
			{
				return this.resultState == ResultState.Success;
			}
		}

		/// <summary>
		/// 	Gets the message associated with a test
		/// 	failure or with not running the test
		/// </summary>
		public string Message
		{
			get
			{
				return this.message;
			}
		}

		/// <summary>
		/// 	Gets the name of the test result
		/// </summary>
		public virtual string Name
		{
			get
			{
				return this.test.TestName.Name;
			}
		}

		/// <summary>
		/// 	Gets the ResultState of the test result, which 
		/// 	indicates the success or failure of the test.
		/// </summary>
		public ResultState ResultState
		{
			get
			{
				return this.resultState;
			}
		}

		/// <summary>
		/// 	Gets a list of the child results of this TestResult
		/// </summary>
		public IList Results
		{
			get
			{
				return this.results;
			}
		}

		/// <summary>
		/// 	Gets any stacktrace associated with an
		/// 	error or failure.
		/// </summary>
		public virtual string StackTrace
		{
			get
			{
				return this.stackTrace;
			}
			set
			{
				this.stackTrace = value;
			}
		}

		/// <summary>
		/// 	Gets the test associated with this result
		/// </summary>
		public ITest Test
		{
			get
			{
				return this.test;
			}
		}

		/// <summary>
		/// 	Gets the elapsed time for running the test
		/// </summary>
		public double Time
		{
			get
			{
				return this.time;
			}
			set
			{
				this.time = value;
			}
		}

		#endregion

		#region Methods/Operators

		private static string BuildMessage(Exception exception)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0} : {1}", exception.GetType().ToString(), exception.Message);

			Exception inner = exception.InnerException;
			while (inner != null)
			{
				sb.Append(Environment.NewLine);
				sb.AppendFormat("  ----> {0} : {1}", inner.GetType().ToString(), inner.Message);
				inner = inner.InnerException;
			}

			return sb.ToString();
		}

		private static string BuildStackTrace(Exception exception)
		{
			StringBuilder sb = new StringBuilder(GetStackTrace(exception));

			Exception inner = exception.InnerException;
			while (inner != null)
			{
				sb.Append(Environment.NewLine);
				sb.Append("--");
				sb.Append(inner.GetType().Name);
				sb.Append(Environment.NewLine);
				sb.Append(GetStackTrace(inner));

				inner = inner.InnerException;
			}

			return sb.ToString();
		}

		private static string GetStackTrace(Exception exception)
		{
			try
			{
				return exception.StackTrace;
			}
			catch (Exception)
			{
				return "No stack trace available";
			}
		}

		/// <summary>
		/// 	Add a child result
		/// </summary>
		/// <param name="result"> The child result to be added </param>
		public void AddResult(TestResult result)
		{
			if (this.results == null)
				this.results = new ArrayList();

			this.results.Add(result);

			switch (result.ResultState)
			{
				case ResultState.Failure:
				case ResultState.Error:
				case ResultState.NotRunnable:
					if (!this.IsFailure && !this.IsError && this.ResultState != ResultState.NotRunnable)
						this.Failure("One or more child tests had errors", null, FailureSite.Child);
					break;
				case ResultState.Success:
					if (this.ResultState == ResultState.Inconclusive)
						this.Success();
					break;
					// Removed this case due to bug #928018
					//case ResultState.Ignored:
					//    if (this.ResultState == ResultState.Inconclusive || ResultState == ResultState.Success)
					//        this.SetResult(ResultState.Ignored, "One or more child tests were ignored", null, FailureSite.Child);
					//    break;
				case ResultState.Cancelled:
					this.SetResult(ResultState.Cancelled, result.Message, null, FailureSite.Child);
					break;
			}
		}

		/// <summary>
		/// 	Marks the result as an error due to an exception thrown
		/// 	by the test.
		/// </summary>
		/// <param name="exception"> The exception that was caught </param>
		public void Error(Exception exception)
		{
			this.Error(exception, FailureSite.Test);
		}

		/// <summary>
		/// 	Marks the result as an error due to an exception thrown
		/// 	from the indicated FailureSite.
		/// </summary>
		/// <param name="exception"> The exception that was caught </param>
		/// <param name="failureSite"> The site from which it was thrown </param>
		public void Error(Exception exception, FailureSite failureSite)
		{
			this.SetResult(ResultState.Error, exception, failureSite);
			//string message = BuildMessage(exception);
			//string stackTrace = BuildStackTrace(exception);

			//if (failureSite == FailureSite.TearDown)
			//{
			//    message = "TearDown : " + message;
			//    stackTrace = "--TearDown" + Environment.NewLine + stackTrace;

			//    if (this.message != null)
			//        message = this.message + Environment.NewLine + message;
			//    if (this.stackTrace != null)
			//        stackTrace = this.stackTrace + Environment.NewLine + stackTrace;
			//}

			//SetResult( ResultState.Error, message, stackTrace );
			//this.failureSite = failureSite;
		}

		/// <summary>
		/// 	Mark the test as a failure due to an
		/// 	assertion having failed.
		/// </summary>
		/// <param name="message"> Message to display </param>
		/// <param name="stackTrace"> Stack trace giving the location of the failure </param>
		public void Failure(string message, string stackTrace)
		{
			this.Failure(message, stackTrace, FailureSite.Test);
		}

		/// <summary>
		/// 	Mark the test as a failure due to an
		/// 	assertion having failed.
		/// </summary>
		/// <param name="message"> Message to display </param>
		/// <param name="stackTrace"> Stack trace giving the location of the failure </param>
		/// <param name="failureSite"> The site of the failure </param>
		public void Failure(string message, string stackTrace, FailureSite failureSite)
		{
			this.SetResult(ResultState.Failure, message, stackTrace);
			this.failureSite = failureSite;
		}

		/// <summary>
		/// 	Mark the test as ignored.
		/// </summary>
		/// <param name="reason"> The reason the test was not run </param>
		public void Ignore(string reason)
		{
			this.Ignore(reason, null);
		}

		/// <summary>
		/// 	Mark the test as ignored.
		/// </summary>
		/// <param name="ex"> The ignore exception that was thrown </param>
		public void Ignore(Exception ex)
		{
			this.Ignore(ex.Message, BuildStackTrace(ex));
		}

		/// <summary>
		/// 	Mark the test as ignored.
		/// </summary>
		/// <param name="reason"> The reason the test was not run </param>
		/// <param name="stackTrace"> Stack trace giving the location of the command </param>
		public void Ignore(string reason, string stackTrace)
		{
			this.SetResult(ResultState.Ignored, reason, stackTrace);
		}

		/// <summary>
		/// 	Mark the test a not runnable with a reason
		/// </summary>
		/// <param name="reason"> The reason the test is invalid </param>
		public void Invalid(string reason)
		{
			this.SetResult(ResultState.NotRunnable, reason, null);
		}

		/// <summary>
		/// 	Mark the test as not runnable due to a builder exception
		/// </summary>
		/// <param name="ex"> The exception thrown by the builder or an addin </param>
		public void Invalid(Exception ex)
		{
			this.SetResult(ResultState.NotRunnable, BuildMessage(ex), BuildStackTrace(ex));
		}

		/// <summary>
		/// 	Set the result of the test
		/// </summary>
		/// <param name="resultState"> The ResultState to use in the result </param>
		/// <param name="reason"> The reason the test was not run </param>
		/// <param name="stackTrace"> Stack trace giving the location of the command </param>
		/// <param name="failureSite"> The location of the failure, if any </param>
		public void SetResult(ResultState resultState, string reason, string stackTrace, FailureSite failureSite)
		{
			if (failureSite == FailureSite.SetUp)
				reason = "SetUp : " + reason;
			else if (failureSite == FailureSite.TearDown)
			{
				reason = "TearDown : " + reason;
				stackTrace = "--TearDown" + Environment.NewLine + stackTrace;

				if (this.message != null)
					reason = this.message + Environment.NewLine + reason;
				if (this.stackTrace != null)
					stackTrace = this.stackTrace + Environment.NewLine + stackTrace;
			}

			this.resultState = resultState;
			this.message = reason;
			this.stackTrace = stackTrace;
			this.failureSite = failureSite;
		}

		/// <summary>
		/// 	Set the result of the test
		/// </summary>
		/// <param name="resultState"> The ResultState to use in the result </param>
		/// <param name="reason"> The reason the test was not run </param>
		/// <param name="stackTrace"> Stack trace giving the location of the command </param>
		public void SetResult(ResultState resultState, string reason, string stackTrace)
		{
			this.SetResult(resultState, reason, stackTrace, FailureSite.Test);
		}

		/// <summary>
		/// 	Set the result of the test.
		/// </summary>
		/// <param name="resultState"> The ResultState to use in the result </param>
		/// <param name="ex"> The exception that caused this result </param>
		/// <param name="failureSite"> The site at which an error or failure occured </param>
		public void SetResult(ResultState resultState, Exception ex, FailureSite failureSite)
		{
			if (resultState == ResultState.Cancelled)
				this.SetResult(resultState, "Test cancelled by user", BuildStackTrace(ex));
			else if (resultState == ResultState.Error)
				this.SetResult(resultState, BuildMessage(ex), BuildStackTrace(ex), failureSite);
			else
				this.SetResult(resultState, ex.Message, ex.StackTrace, failureSite);
		}

		/// <summary>
		/// 	Mark the test as skipped.
		/// </summary>
		/// <param name="reason"> The reason the test was not run </param>
		public void Skip(string reason)
		{
			this.SetResult(ResultState.Skipped, reason, null);
		}

		/// <summary>
		/// 	Mark the test as succeeding
		/// </summary>
		public void Success()
		{
			this.SetResult(ResultState.Success, null, null);
		}

		/// <summary>
		/// 	Mark the test as succeeding and set a message
		/// </summary>
		public void Success(string message)
		{
			this.SetResult(ResultState.Success, message, null);
		}

		#endregion
	}
}