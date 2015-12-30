#region Using

using System.Diagnostics;
using System.Linq;

using NMock.Monitoring;

#endregion

namespace NMock.Internal
{
	[DebuggerDisplay("Unordered Expectation List[Count = {Count}, Depth = {Depth}]")]
	internal class UnorderedExpectationList : ExpectationListBase
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UnorderedExpectationList" /> class.
		/// </summary>
		/// <param name="parent"> The parent <see cref="IExpectationList" /> of this instance. </param>
		public UnorderedExpectationList(ExpectationListBase parent)
			: base(parent)
		{
			if (parent != null)
				this.depth = parent.Depth + 1;
			else
				this.depth = 0;

			if (this.depth == 0)
				this.prompt = "MockFactory Expectations:";
			else
				this.prompt = "Unordered {";
		}

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets a value indicating whether this instance is active.
		/// </summary>
		/// <value> <c> true </c> if this instance is active; otherwise, <c> false </c>. </value>
		public override bool IsActive
		{
			get
			{
				return this.Any(e => e.IsActive);
			}
		}

		public override bool IsValid
		{
			get
			{
				return this.All(_ => _.IsValid);
			}
		}

		#endregion

		#region Methods/Operators

		public override bool ContainsOrderedExpectationFor(Invocation invocation)
		{
			foreach (var expectation in this)
			{
				if (expectation as IExpectationList != null)
					return ((IExpectationList)expectation).ContainsOrderedExpectationFor(invocation);

				if (expectation.Matches(invocation))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Checks whether stored expectations matches the specified invocation.
		/// </summary>
		/// <param name="invocation"> The invocation to check. </param>
		/// <returns> Returns whether one of the stored expectations has met the specified invocation. </returns>
		public override bool Matches(Invocation invocation)
		{
			var matchFound = this.Any(expectation => expectation.Matches(invocation));
			return matchFound;
		}

		public override bool MatchesIgnoringIsActive(Invocation invocation)
		{
			return this.Any(e => e.MatchesIgnoringIsActive(invocation));
		}

		/// <summary>
		/// Performs the specified invocation on the corresponding expectation if a match was found.
		/// </summary>
		/// <param name="invocation"> The invocation to match. </param>
		public override bool Perform(Invocation invocation)
		{
			//loop through all active expectations looking for a match
			foreach (var expectation in this)
			{
				if (expectation.Matches(invocation)) //expectation.IsValid && 
					return expectation.Perform(invocation);
			}

			//loop through through all inactive expectations looking for a match
			foreach (var expectation in this)
			{
				if (expectation.MatchesIgnoringIsActive(invocation))
				{
					//var ordered = expectation as OrderedExpectationList;
					//if (ordered != null)
					//{
					//    ordered.PerformAgainstInactive(invocation);
					//    return false;
					//}

					var buildable = expectation as BuildableExpectation;
					if (buildable != null)
					{
						buildable.IncreaseCallCount();
						return false;
					}
				}
			}

			return false;
		}

		#endregion
	}
}