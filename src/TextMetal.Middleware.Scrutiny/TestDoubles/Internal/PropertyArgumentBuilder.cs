#region Using

using NMock.Syntax;

#endregion

namespace NMock.Internal
{
	internal class PropertyArgumentBuilder<TResult> : IAutoArgumentSyntax<TResult>
	{
		#region Constructors/Destructors

		public PropertyArgumentBuilder(ExpectationBuilder builder)
		{
			this._builder = builder;
		}

		#endregion

		#region Fields/Constants

		private readonly ExpectationBuilder _builder;

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

		public ICommentSyntax WillReturn(TResult actualValue)
		{
			return this._builder.Will(Return.Value(actualValue));
		}

		public IAutoMatchSyntax<TResult> With(params object[] expectedArguments)
		{
			this._builder.With(expectedArguments);

			return new PropertyMatchBuilder<TResult>(this._builder);
		}

		public IAutoMatchSyntax<TResult> WithAnyArguments()
		{
			this._builder.WithAnyArguments();

			return new PropertyMatchBuilder<TResult>(this._builder);
		}

		public IAutoActionSyntax<TResult> WithArguments(params Matcher[] argumentMatchers)
		{
			this._builder.With(argumentMatchers);

			return this;
		}

		public IAutoMatchSyntax<TResult> WithNoArguments()
		{
			return new PropertyMatchBuilder<TResult>(this._builder);
		}

		#endregion
	}
}