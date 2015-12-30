namespace NMock.Syntax
{
	internal interface IValueSyntaxBuilder : IValueSyntax
	{
		#region Methods/Operators

		void Will(params IAction[] action);

		#endregion
	}
}