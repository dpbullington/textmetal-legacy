#region Using

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using NMock.Matchers;
using NMock.Monitoring;

#endregion

namespace NMock.Internal
{
	internal class BuildableExpectation : IExpectation, IVerifyableExpectation
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BuildableExpectation" /> class.
		/// </summary>
		/// <param name="expectationDescription"> The expectation description. </param>
		/// <param name="requiredCountMatcher"> The required count matcher. </param>
		/// <param name="matchingCountMatcher"> The matching count matcher. </param>
		public BuildableExpectation(string expectationDescription, Matcher requiredCountMatcher, Matcher matchingCountMatcher)
		{
			this._expectationDescription = expectationDescription;
			this._requiredCountMatcher = requiredCountMatcher;
			this._matchingCountMatcher = matchingCountMatcher;

			this.ArgumentsMatcher = new ArgumentsMatcher();

			this.IsValid = false;
		}

		#endregion

		#region Fields/Constants

		private readonly List<IAction> _actions = new List<IAction>();
		private readonly string _expectationDescription;
		private readonly List<Matcher> _extraMatchers = new List<Matcher>();
		private readonly Matcher _matchingCountMatcher;
		private readonly Matcher _requiredCountMatcher;

		private Matcher _argumentsMatcher;
		private int _callCount;
		private string _expectationComment;

		private Matcher _methodMatcher;

		private string _methodSeparator = ".";

		#endregion

		#region Properties/Indexers/Events

		public bool HasBeenMet
		{
			get
			{
				var maximumMet = this._matchingCountMatcher.Matches(this._callCount);
				var minimumMet = this._requiredCountMatcher.Matches(this._callCount);

				return maximumMet && minimumMet;
			}
		}

		public bool IsActive
		{
			get
			{
				return this._matchingCountMatcher.Matches(this._callCount + 1);
			}
		}

		public Matcher ArgumentsMatcher
		{
			private get
			{
				return this._argumentsMatcher;
			}
			set
			{
				this._argumentsMatcher = value;
				this.Validate();
			}
		}

		public bool IsValid
		{
			get;
			set;
		}

		public Matcher MethodMatcher
		{
			internal get
			{
				return this._methodMatcher;
			}
			set
			{
				this._methodMatcher = value;
				this.Validate();
			}
		}

		public IMockObject Receiver
		{
			private get;
			set;
		}

		#endregion

		#region Methods/Operators

		private static void ProcessEventHandlers(Invocation invocation)
		{
			if (invocation.IsEventAccessor)
			{
				var mockObject = invocation.Receiver as IMockObject;
				if (mockObject != null)
					mockObject.ProcessEventHandlers(invocation);
			}
		}

		public void AddAction(IAction action)
		{
			this._actions.Add(action);
			//Validate();
		}

		public void AddComment(string comment)
		{
			this._expectationComment = comment;
		}

		public void AddInvocationMatcher(Matcher matcher)
		{
			this._extraMatchers.Add(matcher);
			this.Validate();
		}

		public void Assert()
		{
			if (!this.HasBeenMet)
			{
				var writer = new DescriptionWriter();
				this.DescribeTo(writer);
				throw new UnmetExpectationException(writer.ToString());
			}
		}

		public void DescribeActiveExpectationsTo(TextWriter writer)
		{
			if (this.IsActive)
				this.DescribeTo(writer);
		}

		public void DescribeAsIndexer()
		{
			this._methodSeparator = string.Empty;
		}

		public void DescribeTo(TextWriter writer)
		{
			if (this.MethodMatcher is MethodMatcher)
			{
				writer.Write(((MethodMatcher)this.MethodMatcher).ReturnType);
				writer.Write(" ");
			}

			writer.Write(this.Receiver.MockName);
			writer.Write(this._methodSeparator);
			if (this.MethodMatcher != null)
				this.MethodMatcher.DescribeTo(writer);
			this.ArgumentsMatcher.DescribeTo(writer);

			if (this._extraMatchers.Count > 0)
			{
				writer.Write(" Matching[");
				for (int i = 0; i < this._extraMatchers.Count; i++)
				{
					if (i != 0)
						writer.Write(", ");

					this._extraMatchers[i].DescribeTo(writer);
				}
				writer.Write("]");
			}

			if (this._actions.Count > 0)
			{
				writer.Write(" will ");
				(this._actions[0]).DescribeTo(writer);
				for (int i = 1; i < this._actions.Count; i++)
				{
					if (i != 0)
						writer.Write(", ");
					this._actions[i].DescribeTo(writer);
				}
			}

			if (this.IsValid)
			{
				writer.Write(" [EXPECTED: ");
				writer.Write(this._expectationDescription);

				writer.Write(" CALLED: ");
				writer.Write(this._callCount);
				writer.Write(" time");
				if (this._callCount != 1)
					writer.Write("s");
				writer.Write("]");
			}
			else
				writer.Write(" [EXPECTATION NOT VALID because of runtime error or incomplete setup]");

			if (!string.IsNullOrEmpty(this._expectationComment))
			{
				writer.Write(" Comment: ");
				writer.Write(this._expectationComment);
			}
			writer.WriteLine();
		}

		public void DescribeUnmetExpectationsTo(TextWriter writer)
		{
			if (!this.HasBeenMet)
				this.DescribeTo(writer);
		}

		private bool ExtraMatchersMatch(Invocation invocation)
		{
			return this._extraMatchers.All(matcher => matcher.Matches(invocation));
		}

		internal void IncreaseCallCount()
		{
			this._callCount++;
		}

		private bool IsArgumentsMatcherValidForProperties()
		{
			var matcher = this.RemoveDescriptionOverride(this._methodMatcher);

			var methodMatcher = matcher as MethodNameMatcher;

			if (methodMatcher != null)
			{
				if (methodMatcher.Description.StartsWith(Constants.SET)) //set prop needs an argument
				{
					var argumentsMatcher = this.ArgumentsMatcher as ArgumentsMatcher;
					if (argumentsMatcher != null && argumentsMatcher.Count == 0)
						return false;
				}
			}

			return true;
		}

		private bool IsMethodMatcherValid()
		{
			return this._methodMatcher != null;
		}

		/// <summary>
		/// Checks whether stored expectations matches the specified invocation.
		/// </summary>
		/// <param name="invocation"> The invocation to check. </param>
		/// <returns> Returns whether one of the stored expectations has met the specified invocation. </returns>
		public bool Matches(Invocation invocation)
		{
			if (!this.IsActive)
				return false;
			return this.MatchesIgnoringIsActive(invocation);
		}

		public bool MatchesIgnoringIsActive(Invocation invocation)
		{
			if (this.Receiver != invocation.Receiver)
				return false;
			if (!this.MethodMatcher.Matches(invocation.Method))
				return false;
			if (!this.ArgumentsMatcher.Matches(invocation))
				return false;
			if (!this.ExtraMatchersMatch(invocation))
				return false;
			return true;
		}

		public bool Perform(Invocation invocation)
		{
			this._callCount++;
			ProcessEventHandlers(invocation);
			foreach (IAction action in this._actions)
				action.Invoke(invocation);

			//check that return value was set
			if (invocation.Result == Missing.Value && invocation.Method.ReturnType != typeof(void))
			{
				string message = string.Empty;

				if (invocation.IsPropertyGetAccessor)
				{
					message = string.Format(
						"An expectation match was found but the expectation was incomplete.  A return value for property '{0}' on '{1}' mock must be set.",
						invocation.MethodName, this.Receiver.MockName);
				}
				else if (invocation.IsMethod)
				{
					message = string.Format(
						"An expectation match was found but the expectation was incomplete.  A return value for method '{0}' on '{1}' mock must be set.",
						invocation.MethodSignature, this.Receiver.MockName);
				}

				var exception = new IncompleteExpectationException(message);

				throw exception;
			}
			return true;
		}

		/// <summary>
		/// Adds itself to the <paramref name="result" /> if the <see cref="Receiver" /> matches
		/// the specified <paramref name="mock" />.
		/// </summary>
		/// <param name="mock"> The mock for which expectations are queried. </param>
		/// <param name="result"> The result to add matching expectations to. </param>
		public void QueryExpectationsBelongingTo(IMockObject mock, IList<IExpectation> result)
		{
			if (this.Receiver == mock)
				result.Add(this);
		}

		private Matcher RemoveDescriptionOverride(Matcher matcher)
		{
			var descriptionOverride = matcher as DescriptionOverride;

			return descriptionOverride != null ? this.RemoveDescriptionOverride(descriptionOverride.WrappedMatcher) : matcher;
		}

		public override string ToString()
		{
			var writer = new DescriptionWriter();
			this.DescribeTo(writer);
			return writer.ToString();
		}

		private void Validate()
		{
			//must have a method matcher
			if (!this.IsMethodMatcherValid())
			{
				this.IsValid = false;
				return;
			}

			if (!this.IsArgumentsMatcherValidForProperties())
			{
				this.IsValid = false;
				return;
			}

			this.IsValid = true;

			/*
			if (methodMatcher != null && methodMatcher._methodInfo.ReturnType != typeof(void))
			{
				var found = false;
				foreach (var action in _actions)
				{
					var returnAction = action as IReturnAction;
					if ((action is ThrowAction) || (returnAction != null && methodMatcher._methodInfo.ReturnType.IsAssignableFrom(returnAction.ReturnType)))
					{
						found = true;
					}
				}

				if (!found)
				{
					IsValid = false;
					return;
				}
			}

			//check argumentmatchers
			if (methodMatcher != null)
			{
				var @params = methodMatcher._methodInfo.GetParameters();
				var argumentsMatcher = ArgumentsMatcher as ArgumentsMatcher;
				if ((argumentsMatcher != null && @params.Count() != argumentsMatcher.Count) 
					|| (argumentsMatcher.Count > 0 && argumentsMatcher[0].GetType() != typeof(AlwaysMatcher)))
				{
					IsValid = false;
					return;
				}
			}

			IsValid = true;
			 */
		}

		/// <summary>
		/// Returns a list of validation errors
		/// </summary>
		/// <returns> </returns>
		public string ValidationErrors()
		{
			var errors = new StringBuilder();

			if (!this.IsMethodMatcherValid())
				errors.AppendLine("A method matcher is missing on the mock: " + this.Receiver.MockName);

			if (!this.IsArgumentsMatcherValidForProperties())
				errors.AppendLine("A property is missing a matcher on the mock: " + this.Receiver.MockName);

			/*
			var methodMatcher = _methodMatcher as MethodMatcher;
			if (methodMatcher != null && methodMatcher._methodInfo.ReturnType != typeof(void))
			{
				var found = false;
				foreach (var action in _actions)
				{
					var returnAction = action as IReturnAction;
					if ((action is ThrowAction) || (returnAction != null && methodMatcher._methodInfo.ReturnType.IsAssignableFrom(returnAction.ReturnType)))
					{
						found = true;
					}
				}

				if (!found)
				{
					errors.AppendLine("At least one IReturnAction or ThrowAction must be specified for a non-void method.  Matcher: " + methodMatcher);
				}
			}

			//check argumentmatchers
			if (methodMatcher != null)
			{
				var @params = methodMatcher._methodInfo.GetParameters();
				var argumentsMatcher = ArgumentsMatcher as ArgumentsMatcher;
				if ((argumentsMatcher != null && @params.Count() != argumentsMatcher.Count)
					|| (argumentsMatcher.Count > 0 && argumentsMatcher[0].GetType() != typeof(AlwaysMatcher)))
				{
					errors.AppendLine(string.Format("The number of argument matcher(s) specified is invalid.  The method expects {0} parameters and {1} matchers were specified.  (If you are matching a method, use the 'MethodWith' method or '.With(...)' after your method matcher.)", @params.Count(), argumentsMatcher.Count));
				}
			}

			*/

			return errors.ToString();
		}

		#endregion
	}
}