using System;

using NMock.Actions;
using NMock.Matchers;
using NMock.Syntax;

namespace NMock.Internal
{
	internal class ExpectationSyntax<T> : IExpectMatchSyntax
		where T : class
	{
		#region Constructors/Destructors

		public ExpectationSyntax(ExpectationBuilder<T> expectationBuilder)
		{
			this._builder = expectationBuilder;
		}

		public ExpectationSyntax(ExpectationBuilder<T> expectationBuilder, string eventName)
			: this(expectationBuilder)
		{
			this._eventName = eventName;
		}

		#endregion

		#region Fields/Constants

		private readonly ExpectationBuilder<T> _builder;
		private readonly string _eventName;

		#endregion

		#region Methods/Operators

		void IVerifyableExpectation.Assert()
		{
			this._builder.Assert();
		}

		public IVerifyableExpectation Comment(string comment)
		{
			return this._builder.Comment(comment);
		}

		public ICommentSyntax Will(params IAction[] actions)
		{
			return this._builder.Will(actions);
		}

		public DelegateInvoker WillReturnDelegateInvoker()
		{
			return new DelegateInvoker(this._eventName, this);
		}

		public EventInvoker<TEventArgs> WillReturnEventInvoker<TEventArgs>() where TEventArgs : EventArgs
		{
			return new EventInvoker<TEventArgs>(this._eventName, this);
		}

		public IActionSyntax With(params object[] expectedArguments)
		{
			this._builder.With(expectedArguments);
			return this;
		}

		public IActionSyntax WithAnyArguments()
		{
			this._builder.WithAnyArguments();
			return this;
		}

		#endregion
	}

	internal class ExpectationSyntax<T, TResult> : IExpectMatchSyntax<TResult>
		where T : class
	{
		#region Constructors/Destructors

		public ExpectationSyntax(ExpectationBuilder<T> expectationBuilder)
		{
			this._builder = expectationBuilder;
		}

		#endregion

		#region Fields/Constants

		private readonly ExpectationBuilder<T> _builder;

		#endregion

		#region Methods/Operators

		void IVerifyableExpectation.Assert()
		{
			this._builder.Assert();
		}

		IVerifyableExpectation ICommentSyntax.Comment(string comment)
		{
			return this._builder.Comment(comment);
		}

		IExpectActionSyntax IExpectMatchSyntax<TResult>.To(Matcher matcher)
		{
			return this.To(matcher);
		}

		IExpectActionSyntax IExpectMatchSyntax<TResult>.To(TResult value)
		{
			return this.To(Is.EqualTo(value));
		}

		private IExpectActionSyntax To(Matcher matcher)
		{
			//make sure the method expectation was set up as a setter.
			//it may not be when the expectation is used like mock.Expect(_=>_.prop).To(...)

			string propertyName = this._builder.BuildableExpectation.MethodMatcher.Description;

			if (!propertyName.Contains("=")) //get properties will not have an "=", so the expectation needs to change
				this._builder.SetProperty(propertyName);

			this._builder.With(new ArgumentsMatcher(matcher));

			// capture the value during the assignment
			this._builder.Will(new CaptureValueAction());

			return this;
		}

		IExpectActionSyntax IExpectMatchSyntax<TResult>.ToAnything()
		{
			return this.To(Is.Anything);
		}

		ICommentSyntax IExpectActionSyntax.Will(params IAction[] actions)
		{
			this._builder.Will(actions);
			return this;
		}

		ICommentSyntax IExpectActionSyntax<TResult>.WillReturn(TResult actualValue)
		{
			this._builder.Will(Return.Value(actualValue));
			return this;
		}

		ICommentSyntax IExpectActionSetterSyntax.WillReturnSetterValue()
		{
			this._builder.Will(new ReturnPropertyValueAction(this._builder.MockObject));
			return this;
		}

		IExpectActionSyntax<TResult> IExpectMatchSyntax<TResult>.With(params object[] expectedArguments)
		{
			this._builder.With(expectedArguments);
			return this;
		}

		IExpectActionSyntax<TResult> IExpectMatchSyntax<TResult>.WithAnyArguments()
		{
			this._builder.WithAnyArguments();
			return this;
		}

		#endregion
	}
}