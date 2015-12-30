using System;

namespace NMock.Syntax
{
	/// <summary>
	/// This interface ties together the <see cref="IMatchSyntax" /> and <see cref="IAutoActionSyntax{TProperty}" />
	/// interfaces to provide syntax on <see cref="IStubSyntax{TInterface}.GetProperty{TProperty}(System.Linq.Expressions.Expression{System.Func{TInterface,TProperty}})" />.
	/// </summary>
	/// <typeparam name="TProperty"> The property type of the lambda expression. </typeparam>
	/// <remarks>
	/// This interface doesn't provide any new members.  It is used to tie together two existing
	/// interfaces so that the <see cref="IStubSyntax{TInterface}.GetProperty{TProperty}(System.Linq.Expressions.Expression{System.Func{TInterface,TProperty}})" />
	/// method will return the right syntax.
	/// </remarks>
	public interface IAutoMatchSyntax<TProperty> : IMatchSyntax, IAutoPropertyActionSyntax<TProperty>
	{
	}
}