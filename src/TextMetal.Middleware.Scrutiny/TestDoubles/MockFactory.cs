#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NMock.Internal;
using NMock.Matchers;
using NMock.Monitoring;
using NMock.Proxy;
using NMock.Proxy.Castle;
using NMock.Proxy.Reflective;

#endregion

namespace NMock
{
	/// <summary>
	/// Tracks the expectations created by its <see cref="Mock{T}" />s and <see cref="Stub{T}" />s.
	/// </summary>
	/// <remarks>
	/// The <b> MockFactory </b> is a main component of the NMock3 API.  It is used to create <see cref="Mock{T}" />
	/// instances and <see cref="Stub{T}" /> instances of a <see langword="class" /> or <see langword="interface" />.
	/// </remarks>
	public class MockFactory : IDisposable
	{
		#region Constructors/Destructors

		/// <summary>
		/// Creates a default <see cref="MockFactory" />.
		/// </summary>
		/// <remarks>
		/// Default <see cref="MockFactory" /> classes do not ignore unexpected incovations.
		/// </remarks>
		public MockFactory()
		{
			this.ChangeDefaultMockObjectFactory(typeof(CastleMockObjectFactory));

			this.ClearExpectations();
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Holds all mapping from mocks/types to mock styles.
		/// </summary>
		private readonly StubMockStyleDictionary _stubMockStyleDictionary = new StubMockStyleDictionary();

		/// <summary>
		/// The mock object factory that is being used by this MockFactory instance.
		/// </summary>
		private IMockObjectFactory _currentMockObjectFactory;

		/// <summary>
		/// Expectations at current nesting level.
		/// </summary>
		private ExpectationListBase _expectations;

		//private bool _suppressExpectations;

		/// <summary>
		/// The delegate used to resolve the default type returned as return value in calls to mocks with stub behavior.
		/// </summary>
		private ResolveTypeDelegate _resolveTypeDelegate;

		/// <summary>
		/// If an unexpected invocation exception is thrown then it is stored here to re-throw it in the
		/// <see cref="VerifyAllExpectationsHaveBeenMet()" /> method - exception cannot be swallowed by tested code.
		/// </summary>
		private UnexpectedInvocationException _thrownUnexpectedInvocationException;

		/// <summary>
		/// A field to store <see cref="Exception" />s if and Expectation is incomplete
		/// </summary>
		protected internal IncompleteExpectationException FirstIncompleteExpectationException;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Casts the argument to <see cref="IMockObject" />.
		/// </summary>
		/// <param name="mock"> The object to cast. </param>
		/// <returns> The argument casted to <see cref="IMockObject" /> </returns>
		/// <throws cref="ArgumentNullException"> Thrown if <paramref name="mock" /> is null </throws>
		/// <throws cref="ArgumentException"> Thrown if <paramref name="mock" /> is not a <see cref="IMockObject" /> </throws>
		private static IMockObject CastToMockObject(object mock)
		{
			if (mock == null)
				throw new ArgumentNullException("mock", "mock must not be null");

			IMockObject mockObject = mock as IMockObject;

			if (mockObject != null)
				return mockObject;

			throw new ArgumentException("argument must be a mock", "mock");
		}

		/// <summary>
		/// Adds the expectation.
		/// </summary>
		/// <param name="expectation"> The expectation. </param>
		internal void AddExpectation(IExpectation expectation)
		{
			this._expectations.AddExpectation(expectation);
		}

		internal void AssertAll(IMockObject mockObject)
		{
			var result = new List<IExpectation>();
			this._expectations.QueryExpectationsBelongingTo(mockObject, result);

			result.ForEach(_ => _.Assert());
		}

		/// <summary>
		/// Allows the default <see cref="IMockObjectFactory" /> to be replaced with a different implementation.
		/// </summary>
		/// <param name="factoryType">
		/// The System.Type of the <see cref="IMockObjectFactory" /> implementation to use.
		/// This is expected to implement <see cref="IMockObjectFactory" /> and have a default constructor.
		/// </param>
		public void ChangeDefaultMockObjectFactory(Type factoryType)
		{
			if (!typeof(IMockObjectFactory).IsAssignableFrom(factoryType))
				throw new ArgumentException("Supplied factory type does not implement IMockObjectFactory", "factoryType");

			if (factoryType.GetConstructor(Type.EmptyTypes) == null)
				throw new ArgumentException("Supplied factory type does not have a default constructor.", "factoryType");

			this._currentMockObjectFactory = (IMockObjectFactory)Activator.CreateInstance(factoryType);
		}

		/// <summary>
		/// Changes the current MockObjectFactory to a user defined one.
		/// </summary>
		/// <param name="factory"> The new factory </param>
		public void ChangeDefaultMockObjectFactory(IMockObjectFactory factory)
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			this._currentMockObjectFactory = factory;
		}

		/// <summary>
		/// Clears thrown unexpected exceptions so that a new exception will be thrown.
		/// </summary>
		public void ClearException()
		{
			this._thrownUnexpectedInvocationException = null;
		}

		/// <summary>
		/// Clears all expectation on the specified mock.
		/// </summary>
		/// <param name="mockObject"> The mock for which all expectations are cleared. </param>
		internal void ClearExpectations(IMockObject mockObject)
		{
			var result = new List<IExpectation>();
			this._expectations.QueryExpectationsBelongingTo(mockObject, result);

			result.ForEach(expectation => this._expectations.RemoveExpectation(expectation));
		}

		/// <summary>
		/// Resets the state of the factory.
		/// </summary>
		/// <remarks>
		/// Use this method after expected exceptions.
		/// </remarks>
		public void ClearExpectations()
		{
			this._currentMockObjectFactory = new CastleMockObjectFactory();
			this._expectations = new UnorderedExpectationList(null);
			this._thrownUnexpectedInvocationException = null;
			this.FirstIncompleteExpectationException = null;
		}

		/// <summary>
		/// Creates a new dynamic mock of the specified type using the supplied definition.
		/// </summary>
		/// <typeparam name="TMockedType"> The type to mock. </typeparam>
		/// <param name="definition"> An <see cref="IMockDefinition" /> to create the mock from. </param>
		/// <returns> A dynamic mock for the specified type. </returns>
		public TMockedType CreateInstance<TMockedType>(IMockDefinition definition)
		{
			return (TMockedType)definition.Create(typeof(TMockedType), this, this._currentMockObjectFactory);
		}

		/// <summary>
		/// Creates a new dynamic mock of the specified type using the supplied definition.
		/// </summary>
		/// <typeparam name="TMockedType"> The type to mock. </typeparam>
		/// <param name="name"> The name of the mock. </param>
		/// <returns> A dynamic mock for the specified type. </returns>
		public TMockedType CreateInstance<TMockedType>(string name)
		{
			return this.CreateInstance<TMockedType>(DefinedAs.Named(name));
		}

		/// <summary>
		/// Creates a new dynamic mock of the specified type.
		/// </summary>
		/// <typeparam name="TMockedType"> The type to mock. </typeparam>
		/// <param name="constructorArgs">
		/// The arguments for the constructor of the class to be mocked.
		/// Only applicable when mocking classes with non-default constructors.
		/// </param>
		/// <returns> A dynamic mock for the specified type. </returns>
		public TMockedType CreateInstance<TMockedType>(params object[] constructorArgs)
		{
			return this.CreateInstance<TMockedType>(DefinedAs.WithArgs(constructorArgs));
		}

		/// <summary>
		/// Creates a new dynamic mock of the specified type.
		/// </summary>
		/// <typeparam name="TMockedType"> The type to mock. </typeparam>
		/// <param name="mockStyle"> Specifies how the mock object should behave when first created. </param>
		/// <param name="constructorArgs">
		/// The arguments for the constructor of the class to be mocked.
		/// Only applicable when mocking classes with non-default constructors.
		/// </param>
		/// <returns> A dynamic mock for the specified type. </returns>
		public TMockedType CreateInstance<TMockedType>(MockStyle mockStyle, params object[] constructorArgs)
		{
			return this.CreateInstance<TMockedType>(DefinedAs.OfStyle(mockStyle).WithArgs(constructorArgs));
		}

		/// <summary>
		/// Creates a <see cref="Mock{T}" /> with all of the default values.
		/// </summary>
		/// <typeparam name="T"> The type of mock to create. </typeparam>
		/// <returns> </returns>
		public Mock<T> CreateMock<T>() where T : class
		{
			return this.CreateMock<T>(DefinedAs.Default);
		}

		/// <summary>
		/// Creates a <see cref="Mock{T}" /> with the specified name.
		/// </summary>
		/// <typeparam name="T"> The type of mock to create. </typeparam>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public Mock<T> CreateMock<T>(string name) where T : class
		{
			return this.CreateMock<T>(DefinedAs.Named(name));
		}

		/// <summary>
		/// Creates a <see cref="Mock{T}" /> with the specified <see cref="MockStyle" />.
		/// </summary>
		/// <typeparam name="T"> The type of mock to create. </typeparam>
		/// <param name="mockStyle"> </param>
		/// <returns> </returns>
		public Mock<T> CreateMock<T>(MockStyle mockStyle) where T : class
		{
			return this.CreateMock<T>(DefinedAs.OfStyle(mockStyle));
		}

		/// <summary>
		/// Creates a <see cref="Mock{T}" /> with the specified <see cref="MockStyle" /> and additional types.
		/// </summary>
		/// <typeparam name="T"> The type of mock to create. </typeparam>
		/// <param name="mockStyle"> </param>
		/// <param name="additionalTypesToMock"> </param>
		/// <returns> </returns>
		public Mock<T> CreateMock<T>(MockStyle mockStyle, params Type[] additionalTypesToMock) where T : class
		{
			return this.CreateMock<T>(DefinedAs.OfStyle(mockStyle).Implementing(additionalTypesToMock));
		}

		/// <summary>
		/// Creates a <see cref="Mock{T}" /> with the specified name and <see cref="MockStyle" />.
		/// </summary>
		/// <typeparam name="T"> The type of mock to create. </typeparam>
		/// <param name="name"> </param>
		/// <param name="mockStyle"> </param>
		/// <returns> </returns>
		public Mock<T> CreateMock<T>(string name, MockStyle mockStyle) where T : class
		{
			return this.CreateMock<T>(DefinedAs.Named(name).OfStyle(mockStyle));
		}

		/// <summary>
		/// Creates a <see cref="Mock{T}" /> of the primary type and the specified additional types
		/// </summary>
		/// <typeparam name="T"> The type of mock to create. </typeparam>
		/// <param name="additionalTypesToMock"> </param>
		/// <returns> </returns>
		public Mock<T> CreateMock<T>(params Type[] additionalTypesToMock) where T : class
		{
			return this.CreateMock<T>(DefinedAs.Implementing(additionalTypesToMock));
		}

		/// <summary>
		/// Creates a <see cref="Mock{T}" /> with the specified constructor arguments.
		/// </summary>
		/// <typeparam name="T"> The type of mock to create. </typeparam>
		/// <param name="constructorArguments"> </param>
		/// <returns> </returns>
		public Mock<T> CreateMock<T>(params object[] constructorArguments) where T : class
		{
			return this.CreateMock<T>(DefinedAs.WithArgs(constructorArguments));
		}

		/// <summary>
		/// Creates a <see cref="Mock{T}" /> instance.  Use the <see cref="DefinedAs" /> class as a parameter.
		/// </summary>
		/// <typeparam name="T"> The type of mock to create. </typeparam>
		/// <param name="mockDefinition"> </param>
		/// <returns> </returns>
		/// <remarks>
		/// This method gives the developer the most control when creating the mock because they can
		/// specify all parts of the mock definition.
		/// </remarks>
		public Mock<T> CreateMock<T>(IMockDefinition mockDefinition) where T : class
		{
			if (mockDefinition == null)
				throw new ArgumentNullException("mockDefinition");

			return new Mock<T>(mockDefinition.Create(typeof(T), this, this._currentMockObjectFactory), this);
		}

		/// <summary>
		/// Creates a <see cref="Mock{T}" /> using the specified arguments.
		/// </summary>
		/// <typeparam name="T"> The type of mock to create. </typeparam>
		/// <param name="name"> </param>
		/// <param name="mockStyle"> </param>
		/// <param name="additionalTypesToMock"> </param>
		/// <param name="constructorArguments"> </param>
		/// <returns> </returns>
		public Mock<T> CreateMock<T>(string name, MockStyle mockStyle, Type[] additionalTypesToMock, params object[] constructorArguments) where T : class
		{
			return this.CreateMock<T>(DefinedAs.Named(name).OfStyle(mockStyle).Implementing(additionalTypesToMock).WithArgs(constructorArguments));
		}

		/// <summary>
		/// Dispatches the specified invocation.
		/// </summary>
		/// <param name="invocation"> The invocation. </param>
		internal void Dispatch(Invocation invocation)
		{
			if (!this._expectations.IsValid)
			{
				var expectation = this._expectations.First(_ => !_.IsValid);

				this.FirstIncompleteExpectationException = new IncompleteExpectationException(expectation);

				throw this.FirstIncompleteExpectationException;
			}
			if (!this._expectations.Perform(invocation))
			{
				var mock = invocation.Receiver as IMockObject;

				if (mock != null && mock.IgnoreUnexpectedInvocations && invocation.Method.ReturnType != typeof(void))
					throw new ExpectationException(string.Format("The method '{0}' requires a return value and can not be ignored using the IgnoreUnexpectedInvocations setting.", invocation.MethodSignature));

				this.FailUnexpectedInvocation(invocation);
			}
		}

		/// <summary>
		/// Disposes the mockFactory be verifying that all expectations were met.
		/// </summary>
		public void Dispose()
		{
			this.VerifyAllExpectationsHaveBeenMet();
		}

		/// <summary>
		/// Throws an exception indicating that the specified invocation is not expected.
		/// </summary>
		/// <param name="invocation"> The invocation. </param>
		private void FailUnexpectedInvocation(Invocation invocation)
		{
			var writer = new DescriptionWriter();
			//var unexpectedWriter = new DescriptionWriter();
			var expectedWriter = new DescriptionWriter();

			//((ISelfDescribing)invocation).DescribeTo(unexpectedWriter);

			this._expectations.DescribeTo(expectedWriter); //expectations.DescribeActiveExpectationsTo(expectedWriter);

			string theexpectations = expectedWriter.ToString().Replace(EqualMatcher.EQUAL_TO, string.Empty);

			//if (theexpectations.Contains(unexpectedWriter.ToString()))
			//{
			//    writer.WriteLine();
			//    writer.WriteLine("What happened?  An expectation listed below is similar to the unexpected call above.");
			//    writer.WriteLine("Why did this happen?  A parameter reference in your expectation refers to a different instance than the reference used in the call.");
			//    writer.WriteLine("How to fix this:  Use a matcher, override .Equals() on the class, or adjust your API.");
			//    writer.WriteLine();
			//}

			if (theexpectations.Contains(", returning an invoker") && invocation.Method.IsEvent())
			{
				writer.WriteLine();
				writer.WriteLine("If this exception was unexpected, the delegates used in the expectation and invocation may be different.  To bind to *any* delegate, use 'null' in the expectation.");
				writer.WriteLine();
			}

			// try catch to get exception with stack trace.
			try
			{
				//throw new UnexpectedInvocationException(invocation, topOrdering, writer.ToString());
				throw new UnexpectedInvocationException(this, invocation, this._expectations.Root, writer.ToString());
			}
			catch (UnexpectedInvocationException e)
			{
				// remember only first exception
				if (this._thrownUnexpectedInvocationException == null)
					this._thrownUnexpectedInvocationException = e;

				//always rethrow the first exception (that could have caused the rest i.e. a dispose to be called when it shouldn't have)
				throw this._thrownUnexpectedInvocationException;
			}
		}

		/// <summary>
		/// Throws an exception listing all unmet expectations.
		/// </summary>
		private void FailUnmetExpectations()
		{
			var writer = new DescriptionWriter();
			writer.WriteLine("Not all expected invocations were performed.");
			this._expectations.DescribeUnmetExpectationsTo(writer);

			this.ClearExpectations();

			var message = writer.ToString();

			if (message != Constants.ERROR_OCCURED_IN_SETUP)
				throw new UnmetExpectationException(writer.ToString());
		}

		/// <summary>
		/// Gets the mock style to be used for a mock created for a return value of a call to mock with stub behavior.
		/// </summary>
		/// <param name="mock"> The mock that wants to create a mock. </param>
		/// <param name="requestedType"> The type of the requested mock. </param>
		/// <returns> The mock style to use on the created mock. Null if <see cref="MockStyle.Default" /> has to be used. </returns>
		internal MockStyle? GetDependencyMockStyle(object mock, Type requestedType)
		{
			IMockObject mockObject = CastToMockObject(mock);
			return this._stubMockStyleDictionary[mockObject, requestedType];
		}

		/// <summary>
		/// Determines whether there exist expectations for the specified invocation.
		/// </summary>
		/// <param name="invocation"> The invocation. </param>
		/// <returns>
		/// <c> true </c> if there exist expectations for the specified invocation; otherwise, <c> false </c>.
		/// </returns>
		internal bool HasExpectationFor(Invocation invocation)
		{
			return this._expectations.Matches(invocation);
		}

		internal bool HasExpectationForIgnoringIsActive(Invocation invocation)
		{
			return this._expectations.MatchesIgnoringIsActive(invocation);
		}

		/// <summary>
		/// Creates a new named dynamic mock of the specified type and allows the style
		/// of the mock to be specified.
		/// </summary>
		/// <param name="mockedType"> The type to mock. </param>
		/// <param name="name"> A name for the mock that will be used in error messages. </param>
		/// <param name="mockStyle"> Specifies how the mock object should behave when first created. </param>
		/// <param name="constructorArgs">
		/// The arguments for the constructor of the class to be mocked.
		/// Only applicable when mocking classes with non-default constructors.
		/// </param>
		/// <returns> A named mock. </returns>
		internal object NewMock(Type mockedType, string name, MockStyle mockStyle, params object[] constructorArgs)
		{
			return DefinedAs.Named(name).OfStyle(mockStyle).WithArgs(constructorArgs).Create(mockedType, this, this._currentMockObjectFactory);
		}

		/// <summary>
		/// Gets a disposable object and tells the mockFactory that the following expectations are ordered, i.e. they have to be met in the specified order.
		/// Dispose the returned value to return to previous mode.
		/// </summary>
		/// <value>
		/// Disposable object. When this object is disposed then the ordered expectation mode is set back to the mode it was previously
		/// to call to <see cref="Ordered" />.
		/// </value>
		public IDisposable Ordered()
		{
			return this.Push(new OrderedExpectationList(this._expectations));
		}

		/// <summary>
		/// Pops the specified old ordering from the expectations stack.
		/// </summary>
		/// <param name="oldOrdering"> The old ordering. </param>
		private void Pop(ExpectationListBase oldOrdering)
		{
			this._expectations = oldOrdering;
		}

		/// <summary>
		/// Pushes the specified new ordering on the expectations stack.
		/// </summary>
		/// <param name="newOrdering"> The new ordering. </param>
		/// <returns> Disposable popper. </returns>
		private Popper Push(ExpectationListBase newOrdering)
		{
			this._expectations.AddExpectation(newOrdering);
			ExpectationListBase oldOrdering = this._expectations;
			this._expectations = newOrdering;
			return new Popper(this, oldOrdering);
		}

		/// <summary>
		/// Resolves the return value to be used in a call to a mock with stub behavior.
		/// </summary>
		/// <param name="mock"> The mock on which the call is made. </param>
		/// <param name="requestedType"> The type of the return value. </param>
		/// <returns>
		/// The object to be returned as return value; or <see cref="Missing.Value" />
		/// if the default value should be used.
		/// </returns>
		internal object ResolveType(object mock, Type requestedType)
		{
			return this._resolveTypeDelegate != null ? this._resolveTypeDelegate(mock, requestedType) : Missing.Value;
		}

		/// <summary>
		/// Sets the resolve type handler used to override default values returned by stubs.
		/// </summary>
		/// <param name="resolveTypeHandler"> The resolve type handler. </param>
		public void SetResolveTypeHandler(ResolveTypeDelegate resolveTypeHandler)
		{
			this._resolveTypeDelegate = resolveTypeHandler;
		}

		/// <summary>
		/// Sets the mock style used for all properties and methods returning a value of any type of the <paramref name="mock" />.
		/// Can be overridden with a type specific mock style with <see cref="SetStubMockStyle{TStub}" />.
		/// </summary>
		/// <param name="mock"> The mock (with mock style Stub). </param>
		/// <param name="nestedMockStyle"> The nested mock style. </param>
		public void SetStubMockStyle(object mock, MockStyle nestedMockStyle)
		{
			IMockObject mockObject = CastToMockObject(mock);
			this._stubMockStyleDictionary[mockObject] = nestedMockStyle;
		}

		/// <summary>
		/// Sets the mock style used for all properties and methods returning a value of type <typeparamref name="TStub" />
		/// of the <paramref name="mock" />.
		/// </summary>
		/// <typeparam name="TStub"> The type of the stub. </typeparam>
		/// <param name="mock"> The mock (with mock style Stub). </param>
		/// <param name="nestedMockStyle"> The nested mock style. </param>
		public void SetStubMockStyle<TStub>(object mock, MockStyle nestedMockStyle)
		{
			this.SetStubMockStyle(mock, typeof(TStub), nestedMockStyle);
		}

		/// <summary>
		/// Sets the mock style used for all properties and methods returning a value of type <paramref name="nestedMockType" />
		/// of the <paramref name="mock" />.
		/// </summary>
		/// <param name="mock"> The mock (with mock style Stub). </param>
		/// <param name="nestedMockType"> Type of the nested mock. </param>
		/// <param name="nestedMockStyle"> The nested mock style. </param>
		public void SetStubMockStyle(object mock, Type nestedMockType, MockStyle nestedMockStyle)
		{
			IMockObject mockObject = CastToMockObject(mock);
			this._stubMockStyleDictionary[mockObject, nestedMockType] = nestedMockStyle;
		}

		/// <summary>
		/// Suppresses when the factory would throw an exception on unmet or unexpected expectations until the next time
		/// VerifyAllExpectationsHaveBeenMet() is called.
		/// </summary>
		[Obsolete]
		public void SuppressUnexpectedAndUnmetExpectations()
		{
			//_suppressExpectations = true;
		}

		/// <summary>
		/// Gets a disposable object and tells the mockFactory that the following expectations are unordered, i.e. they can be met in any order.
		/// Dispose the returned value to return to previous mode.
		/// </summary>
		/// <value>
		/// Disposable object. When this object is disposed then the unordered expectation mode is set back to the mode it was previously
		/// to the call to <see cref="Unordered" />.
		/// </value>
		public IDisposable Unordered()
		{
			return this.Push(new UnorderedExpectationList(this._expectations));
		}

		/// <summary>
		/// Verifies that all expectations have been met.
		/// Will be called in <see cref="Dispose" />, too.
		/// </summary>
		/// <param name="rethrowThrownExceptions"> A value indicating if exceptions that have already been thrown should be thrown again. </param>
		public void VerifyAllExpectationsHaveBeenMet(bool rethrowThrownExceptions)
		{
			//if (_suppressExpectations)
			//{
			//	_suppressExpectations = false;
			//	return;  //should this 			ClearExpectations();
			//}

			if (!rethrowThrownExceptions && this._thrownUnexpectedInvocationException != null)
				return; //should this 			ClearExpectations();

			//if (thrownUnexpectedInvocationException != null)
			//    return;

			//check for ordering exceptions
			if (rethrowThrownExceptions && this.FirstIncompleteExpectationException != null)
			{
				Exception exceptionToBeRethrown = this.FirstIncompleteExpectationException;
				this.FirstIncompleteExpectationException = null; // only rethrow once
				this.ClearExpectations();
				throw exceptionToBeRethrown;
			}

			// check for swallowed exception
			if (rethrowThrownExceptions && this._thrownUnexpectedInvocationException != null)
			{
				Exception exceptionToBeRethrown = this._thrownUnexpectedInvocationException;
				this._thrownUnexpectedInvocationException = null; // only rethrow once
				this.ClearExpectations();
				throw exceptionToBeRethrown;
			}

			if (!this._expectations.HasBeenMet && this._expectations.IsValid)
				this.FailUnmetExpectations();
		}

		/// <summary>
		/// Verifies that all expectations have been met.
		/// Will be called in <see cref="Dispose" />, too.
		/// </summary>
		public void VerifyAllExpectationsHaveBeenMet()
		{
			this.VerifyAllExpectationsHaveBeenMet(false);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// A popper pops an expectation ordering from the expectations stack on disposal.
		/// </summary>
		private class Popper : IDisposable
		{
			#region Constructors/Destructors

			/// <summary>
			/// Initializes a new instance of the <see cref="Popper" /> class.
			/// </summary>
			/// <param name="mockFactory"> The mockFactory. </param>
			/// <param name="previous"> The previous. </param>
			public Popper(MockFactory mockFactory, ExpectationListBase previous)
			{
				this.previous = previous;
				this.mockFactory = mockFactory;
			}

			#endregion

			#region Fields/Constants

			/// <summary>
			/// The mockFactory.
			/// </summary>
			private readonly MockFactory mockFactory;

			/// <summary>
			/// The previous expectation ordering.
			/// </summary>
			private readonly ExpectationListBase previous;

			#endregion

			#region Methods/Operators

			/// <summary>
			/// Pops the expectation ordering from the stack.
			/// </summary>
			public void Dispose()
			{
				this.mockFactory.Pop(this.previous);
			}

			#endregion
		}

		#endregion
	}

	/// <summary>
	/// Delegate used to override default type returned in stub behavior.
	/// </summary>
	/// <param name="mock"> The mock that has to return a value. </param>
	/// <param name="requestedType"> Type of the return value. </param>
	/// <returns> The object to return as return value for the requested type. </returns>
	public delegate object ResolveTypeDelegate(object mock, Type requestedType);

	[Obsolete("Create an instance of MockFactory instead.")]
	public sealed class Mockery : MockFactory
	{
	}
}