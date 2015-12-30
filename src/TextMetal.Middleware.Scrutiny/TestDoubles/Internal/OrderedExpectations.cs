#region Using

using System.Diagnostics;
using System.Linq;

using NMock.Monitoring;

#endregion

namespace NMock.Internal
{
	[DebuggerDisplay("Ordered Expectation List[Count = {Count}, Depth = {Depth}]")]
	internal class OrderedExpectationList : ExpectationListBase
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="OrderedExpectationList" /> class.
		/// </summary>
		/// <param name="parent"> The parent <see cref="IExpectationList" /> of this <see cref="OrderedExpectationList" /> </param>
		public OrderedExpectationList(ExpectationListBase parent)
			: base(parent)
		{
			if (parent != null)
				this.depth = parent.Depth + 1;

			this.prompt = "Ordered {";
		}

		#endregion

		#region Fields/Constants

		private int current;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the current expectation.
		/// </summary>
		/// <value> The current expectation. </value>
		internal IExpectation CurrentExpectation
		{
			get
			{
				if (this.current >= this.Count)
					return null;

				return this[this.current];
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has next expectation.
		/// </summary>
		/// <value>
		/// <c> true </c> if this instance has next expectation; otherwise, <c> false </c>.
		/// </value>
		private bool HasNextExpectation
		{
			get
			{
				return this.current < this.Count - 1;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is active.
		/// </summary>
		/// <value> <c> true </c> if this instance is active; otherwise, <c> false </c>. </value>
		public override bool IsActive
		{
			get
			{
				//if there are no expectations then not active
				if (this.Count == 0)
					return false;

				//if there is no current expectation then not active
				return this.CurrentExpectation != null;
			}
		}

		public override bool IsValid
		{
			get
			{
				return this.All(_ => _.IsValid);
				//Count == 0
				// || (CurrentExpectation.IsValid); // && NextExpectationIsValid());
			}
		}

		/// <summary>
		/// Gets the next expectation.
		/// </summary>
		/// <value> The next expectation. </value>
		private IExpectation NextExpectation
		{
			get
			{
				return this[this.current + 1];
			}
		}

		#endregion

		#region Methods/Operators

		public override bool ContainsOrderedExpectationFor(Invocation invocation)
		{
			for (int i = this.current; i < this.Count; i++)
			{
				if (this[i] as IExpectationList != null)
					return ((IExpectationList)this[i]).ContainsOrderedExpectationFor(invocation);
				if (this[i].Matches(invocation))
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
			if (this.CurrentExpectation == null)
				return false;

			if (this.Count == 0)
				return false;

			return (this.CurrentExpectation.Matches(invocation)
					|| (this.CurrentExpectation.HasBeenMet
						&& this.NextExpectationMatches(invocation))); //look ahead at the next if the current is an "atleast" matcher
		}

		public override bool MatchesIgnoringIsActive(Invocation invocation)
		{
			if (this.CurrentExpectation == null)
				return false;

			if (this.Count == 0)
				return false;

			return (this.CurrentExpectation.MatchesIgnoringIsActive(invocation)
					|| (this.CurrentExpectation.HasBeenMet
						&& this.NextExpectationMatchesIgnoringIsActive(invocation))); //look ahead at the next if the current is an "atleast" matcher
		}

		private bool NextExpectationMatches(Invocation invocation)
		{
			if (this.HasNextExpectation)
			{
				//this helps skip over empty ordered expectations
				if (this.NextExpectation.IsValid && !this.NextExpectation.IsActive && this.NextExpectation.HasBeenMet)
				{
					this.current++;
					return this.NextExpectationMatches(invocation);
				}

				return this.NextExpectation.Matches(invocation);
			}

			return false;
		}

		private bool NextExpectationMatchesIgnoringIsActive(Invocation invocation)
		{
			return this.HasNextExpectation && this.NextExpectation.MatchesIgnoringIsActive(invocation);
		}

		public override bool Perform(Invocation invocation)
		{
			if (this.Count == 0)
				return false;

			//you would think it would be "if (CurrentExpectation.HasBeenMet)" but "atleast" matchers will have been met
			//when they can consume more, so increment after a match check
			//if (CurrentExpectation.HasBeenMet)
			if (!this.CurrentExpectation.Matches(invocation) && !invocation.MockObject.IgnoreUnexpectedInvocations)
				this.current++;

			//now if this one doesn't match then this is unexpected
			if (this.CurrentExpectation.Matches(invocation))
			{
				var result = this.CurrentExpectation.Perform(invocation);

				//this helps for adding breakpoints and troubleshooting when returning false
				if (result)
					return true;
				else
					return false;
			}

			return false;
		}

		#endregion

		/*
		private bool NextExpectationHasBeenMet()
		{
			return (!HasNextExpectation) || NextExpectation.HasBeenMet;
		}
		
		private bool NextExpectationIsValid()
		{
			return (!HasNextExpectation) || NextExpectation.IsValid;
		}
		*/
	}
}