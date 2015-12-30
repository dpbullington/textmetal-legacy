#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using NMock.Monitoring;
using NMock.Proxy;

#endregion

namespace NMock.Internal
{
	/// <summary>
	/// </summary>
	internal class MockObject : IInvokable, IMockObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MockObject" /> class.
		/// </summary>
		/// <param name="mockFactory"> The mockFactory. </param>
		/// <param name="mockedType"> Type of the mocked. </param>
		/// <param name="name"> The name. </param>
		/// <param name="mockStyle"> The mock style. </param>
		protected MockObject(MockFactory mockFactory, CompositeType mockedType, string name, MockStyle mockStyle)
		{
			this.MockFactory = mockFactory;
			this.MockStyle = mockStyle;
			this.MockName = name;
			this.eventHandlers = new Dictionary<string, List<Delegate>>();
			this.MockedTypes = mockedType;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Results that have been explicitly assigned via a call to a property setter.
		/// These will be returned for all subsequent calls to the matching property getter.
		/// </summary>
		private readonly Dictionary<string, object> assignedPropertyResults = new Dictionary<string, object>();

		/// <summary>
		/// Stores the event handlers that could be added to the mock object.
		/// </summary>
		private readonly Dictionary<string, List<Delegate>> eventHandlers;

		/// <summary>
		/// Results that have been generated for methods or property getters.
		/// These will be returned for all subsequent calls to the same member.
		/// </summary>
		private readonly Dictionary<MethodInfo, object> rememberedMethodResults = new Dictionary<MethodInfo, object>();

		private readonly Dictionary<string, object> rememberedResults = new Dictionary<string, object>();

		private Dictionary<string, object> _interceptedValues;

		#endregion

		#region Properties/Indexers/Events

		public bool IgnoreUnexpectedInvocations
		{
			get;
			set;
		}

		Dictionary<string, object> IMockObject.InterceptedValues
		{
			get
			{
				return this._interceptedValues ?? (this._interceptedValues = new Dictionary<string, object>());
			}
			set
			{
				this._interceptedValues = value;
			}
		}

		public CompositeType MockedTypes
		{
			get;
			private set;
		}

		protected MockFactory MockFactory
		{
			get;
			private set;
		}

		public string MockName
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the mock style of this mock.
		/// </summary>
		protected MockStyle MockStyle
		{
			get;
			private set;
		}

		#endregion

		#region Methods/Operators

		public void AddEventHandler(string eventName, Delegate handler)
		{
			if (this.eventHandlers.ContainsKey(eventName))
			{
				var handlers = this.eventHandlers[eventName];

				if (!handlers.Contains(handler))
					handlers.Add(handler);
			}
			else
				this.eventHandlers.Add(eventName, new List<Delegate> { handler });
		}

		public void AddExpectation(IExpectation expectation)
		{
			this.MockFactory.AddExpectation(expectation);
		}

		private void DynamicInvoke(List<Delegate> delegates, object[] args)
		{
			foreach (Delegate d in delegates)
			{
				try
				{
					d.DynamicInvoke(args);
				}
				catch (TargetInvocationException tie)
				{
					// replace stack trace with original stack trace
					FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
					remoteStackTraceString.SetValue(tie.InnerException, tie.InnerException.StackTrace + Environment.NewLine);
					throw tie.InnerException;
				}
			}
		}

		/// <summary>
		/// Gets the name of the member to be used as the name for a mock returned an a call to a stub.
		/// </summary>
		/// <param name="invocation"> The invocation. </param>
		/// <returns> Name of the mock created as a result value on a call to a stub. </returns>
		private string GetMemberName(ISelfDescribing invocation)
		{
			var writer = new StringWriter();
			invocation.DescribeTo(writer);

			var name = writer.ToString();
			return name.Replace(Environment.NewLine, string.Empty);
		}

		public IList<MethodInfo> GetMethodsMatching(Matcher methodMatcher)
		{
			return this.MockedTypes.GetMatchingMethods(methodMatcher, false);
		}

		/// <summary>
		/// Gets the default result for an invocation.
		/// </summary>
		/// <param name="invocation"> The invocation. </param>
		/// <returns>
		/// The default value to return as result of the invocation.
		/// <see cref="Missing.Value" /> if no default value was provided.
		/// </returns>
		private object GetStubResult(Invocation invocation)
		{
			Type returnType = invocation.MethodReturnType;
			var _returnType = returnType.GetTypeInfo();

			// void method
			if (returnType == typeof(void))
				return Missing.Value;

			// see if developer provides a return value
			object returnValue = this.MockFactory.ResolveType(invocation.Receiver, returnType);

			if (returnValue != Missing.Value)
				return returnValue;

			if (_returnType.IsValueType)
			{
				// use default contructor for value types
				return Activator.CreateInstance(returnType);
			}

			if (returnType == typeof(string))
			{
				// string empty for strings
				return string.Empty;
			}

			if (_returnType.IsClass && _returnType.ImplementedInterfaces.Any(t => t == typeof(IEnumerable)))
			{
				// for enumerables (List, Dictionary) we create an empty object
				return Activator.CreateInstance(returnType);
			}

			if (_returnType.IsSealed)
			{
				// null for sealed classes
				return null;
			}

			// a mock for interfaces and all cases no covered above
			return this.MockFactory.NewMock(returnType, this.GetMemberName(invocation), this.MockFactory.GetDependencyMockStyle(invocation.Receiver, returnType) ?? this.MockStyle);
		}

		void IInvokable.Invoke(Invocation invocation)
		{
			switch (this.MockStyle)
			{
				case MockStyle.Default:
				case MockStyle.Transparent:
					try
					{
						//call up the the factory to route this invocation
						this.MockFactory.Dispatch(invocation);
					}
					catch (UnexpectedInvocationException)
					{
						if (this.IgnoreUnexpectedInvocations)
						{
							//clear the factory expectation
							this.MockFactory.ClearException();
						}
						else
							throw;
					}
					break;

				case MockStyle.Stub:
				{
					if (this.MockFactory.HasExpectationForIgnoringIsActive(invocation))
						goto case MockStyle.Default;

					// check whether we already have a value for this call
					object result;

					if (invocation.IsPropertySetAccessor) // remember values set in a property setter
					{
						if (this.assignedPropertyResults.ContainsKey(invocation.MethodName)) //TODO: stubs don't support indexers! need Item[int] or Item[string,int]
							this.assignedPropertyResults[invocation.MethodName] = invocation.Parameters[0];
						else
							this.assignedPropertyResults.Add(invocation.MethodName, invocation.Parameters[0]);

						return;
					}
					else if (invocation.IsEventAccessor)
					{
						this.ProcessEventHandlers(invocation);
						return;
					}
					else if (invocation.IsPropertyGetAccessor && this.assignedPropertyResults.ContainsKey(invocation.MethodName))
						result = this.assignedPropertyResults[invocation.MethodName];
					else if (this.rememberedMethodResults.ContainsKey(invocation.Method))
						result = this.rememberedMethodResults[invocation.Method];
					else
					{
						result = this.GetStubResult(invocation);
						this.rememberedMethodResults.Add(invocation.Method, result);
					}

					if (result != Missing.Value)
						invocation.Result = result;

					break;
				}
				case MockStyle.RecursiveStub:
				{
					if (this.MockFactory.HasExpectationFor(invocation))
						goto case MockStyle.Default;

					object result;
					if (this.rememberedResults.ContainsKey(invocation.MethodName))
						result = this.rememberedResults[invocation.MethodName];
					else
					{
						result = this.GetStubResult(invocation);
						this.rememberedResults.Add(invocation.MethodName, result);
					}

					if (result != Missing.Value)
						invocation.Result = result;

					break;
				}
			}
		}

		public void ProcessEventHandlers(Invocation invocation)
		{
			var handler = invocation.Parameters[0] as Delegate;
			var name = invocation.MethodName;

			if (name.StartsWith(Constants.ADD))
				this.AddEventHandler(name.Substring(Constants.ADD.Length), handler);
			else if (name.StartsWith(Constants.REMOVE))
				this.RemoveEventHandler(name.Substring(Constants.REMOVE.Length), handler);
		}

		public void RaiseEvent(string eventName, params object[] args)
		{
			if (eventName.StartsWith(Constants.ADD))
				eventName = eventName.Substring(Constants.ADD.Length);

			if (this.eventHandlers.ContainsKey(eventName))
			{
				IEnumerable handlers = this.eventHandlers[eventName];
				// copy handlers before invocation to prevent colection modified exception if event handler is removed within event handler itself.
				List<Delegate> delegates = new List<Delegate>();
				foreach (Delegate handler in handlers)
					delegates.Add(handler);

				this.DynamicInvoke(delegates, args);
			}
		}

		public void RemoveEventHandler(string eventName, Delegate handler)
		{
			if (this.eventHandlers.ContainsKey(eventName))
			{
				List<Delegate> handlers = this.eventHandlers[eventName];
				handlers.Remove(handler);

				if (handlers.Count == 0)
					this.eventHandlers.Remove(eventName);
			}
		}

		public override string ToString()
		{
			return this.MockName;
		}

		#endregion
	}
}