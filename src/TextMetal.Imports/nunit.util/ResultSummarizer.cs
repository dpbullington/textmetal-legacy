// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using NUnit.Core;

namespace NUnit.Util
{
	using System;

	/// <summary>
	/// 	Summary description for ResultSummarizer.
	/// </summary>
	public class ResultSummarizer
	{
		#region Constructors/Destructors

		public ResultSummarizer()
		{
		}

		public ResultSummarizer(TestResult result)
		{
			this.Summarize(result);
		}

		public ResultSummarizer(TestResult[] results)
		{
			foreach (TestResult result in results)
				this.Summarize(result);
		}

		#endregion

		#region Fields/Constants

		private int errorCount = 0;
		private int failureCount = 0;
		private int ignoreCount = 0;
		private int inconclusiveCount = 0;
		private string name;
		private int notRunnable = 0;
		private int resultCount = 0;
		private int skipCount = 0;
		private int successCount = 0;
		private int testsRun = 0;

		private double time = 0.0d;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Returns the number of test cases that had an error.
		/// </summary>
		public int Errors
		{
			get
			{
				return this.errorCount;
			}
		}

		/// <summary>
		/// 	Returns the number of test cases that failed.
		/// </summary>
		public int Failures
		{
			get
			{
				return this.failureCount;
			}
		}

		public int Ignored
		{
			get
			{
				return this.ignoreCount;
			}
		}

		/// <summary>
		/// 	Returns the number of test cases that failed.
		/// </summary>
		public int Inconclusive
		{
			get
			{
				return this.inconclusiveCount;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// 	Returns the number of test cases that were not runnable
		/// 	due to errors in the signature of the class or method.
		/// 	Such tests are also counted as Errors.
		/// </summary>
		public int NotRunnable
		{
			get
			{
				return this.notRunnable;
			}
		}

		/// <summary>
		/// 	Returns the number of tests that passed
		/// </summary>
		public int Passed
		{
			get
			{
				return this.successCount;
			}
		}

		/// <summary>
		/// 	Returns the number of test cases for which results
		/// 	have been summarized. Any tests excluded by use of
		/// 	Category or Explicit attributes are not counted.
		/// </summary>
		public int ResultCount
		{
			get
			{
				return this.resultCount;
			}
		}

		/// <summary>
		/// 	Returns the number of test cases that were skipped.
		/// </summary>
		public int Skipped
		{
			get
			{
				return this.skipCount;
			}
		}

		public bool Success
		{
			get
			{
				return this.failureCount == 0;
			}
		}

		public int TestsNotRun
		{
			get
			{
				return this.skipCount + this.ignoreCount + this.notRunnable;
			}
		}

		/// <summary>
		/// 	Returns the number of test cases actually run, which
		/// 	is the same as ResultCount, less any Skipped, Ignored
		/// 	or NonRunnable tests.
		/// </summary>
		public int TestsRun
		{
			get
			{
				return this.testsRun;
			}
		}

		public double Time
		{
			get
			{
				return this.time;
			}
		}

		#endregion

		#region Methods/Operators

		public void Summarize(TestResult result)
		{
			if (this.name == null)
			{
				this.name = result.Name;
				this.time = result.Time;
			}

			if (!result.Test.IsSuite)
			{
				this.resultCount++;

				switch (result.ResultState)
				{
					case ResultState.Success:
						this.successCount++;
						this.testsRun++;
						break;
					case ResultState.Failure:
						this.failureCount++;
						this.testsRun++;
						break;
					case ResultState.Error:
					case ResultState.Cancelled:
						this.errorCount++;
						this.testsRun++;
						break;
					case ResultState.Inconclusive:
						this.inconclusiveCount++;
						this.testsRun++;
						break;
					case ResultState.NotRunnable:
						this.notRunnable++;
						//errorCount++;
						break;
					case ResultState.Ignored:
						this.ignoreCount++;
						break;
					case ResultState.Skipped:
					default:
						this.skipCount++;
						break;
				}
			}

			if (result.HasResults)
			{
				foreach (TestResult childResult in result.Results)
					this.Summarize(childResult);
			}
		}

		#endregion
	}
}