#region Using

using NMock.Actions;
using NMock.Syntax;

#endregion

namespace NMock.Internal
{
	internal class PropertyValueBuilder<T> : IAutoValueSyntax<T>
	{
		#region Constructors/Destructors

		public PropertyValueBuilder(IValueSyntaxBuilder builder)
		{
			this._builder = builder;
		}

		#endregion

		#region Fields/Constants

		private readonly IValueSyntaxBuilder _builder;

		#endregion

		#region Methods/Operators

		public IActionSyntax To(T equalValue)
		{
			return this.To(Is.EqualTo(equalValue));
		}

		public IActionSyntax To(Matcher valueMatcher)
		{
			// capture the value during the assignment
			this._builder.Will(new CaptureValueAction());

			return this._builder.To(valueMatcher);
		}

		public IActionSyntax ToAnything()
		{
			return this._builder.To(Is.Anything);
		}

		#endregion
	}
}