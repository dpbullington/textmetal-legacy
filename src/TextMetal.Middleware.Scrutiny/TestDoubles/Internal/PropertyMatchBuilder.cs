#region Using

using NMock.Actions;
using NMock.Syntax;

#endregion

namespace NMock.Internal
{
	internal class PropertyMatchBuilder<T> : IAutoMatchSyntax<T>
	{
		#region Constructors/Destructors

		public PropertyMatchBuilder(ExpectationBuilder builder)
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

		public IActionSyntax Matching(Matcher matcher)
		{
			return this._builder.Matching(matcher);
		}

		public ICommentSyntax Will(params IAction[] actions)
		{
			return this._builder.Will(actions);
		}

		public ICommentSyntax WillReturn(T actualValue)
		{
			return this.Will(Return.Value(actualValue));
		}

		public ICommentSyntax WillReturnSetterValue()
		{
			return this._builder.Will(new ReturnPropertyValueAction(this._builder.MockObject));
		}

		#endregion
	}
}