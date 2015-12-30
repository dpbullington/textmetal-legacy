#region Using

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using NMock.Internal;
using NMock.Monitoring;
using NMock.Syntax;

#endregion

namespace NMock
{
	/// <summary>
	/// Base class for Invoker classes that raise events.
	/// </summary>
	public abstract class EventInvokerBase : ICommentSyntax
	{
		#region Constructors/Destructors

		internal EventInvokerBase(string eventName, IActionSyntax expectation, bool isDelegate)
		{
			this.EventName = eventName;
			this.Expectation = expectation;
			this.Expectation.Will(this.Hookup(isDelegate));
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Holds a reference to the delegate that will be invoked.
		/// </summary>
		private Delegate Handler;

		#endregion

		#region Properties/Indexers/Events

		private string EventName
		{
			get;
			set;
		}

		private IActionSyntax Expectation
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		public void Assert()
		{
			this.Expectation.Assert();
		}

		/// <summary>
		/// Adds a comment for the expectation that is added to the error message if the expectation is not met.
		/// </summary>
		/// <param name="comment">
		/// The comment that is shown in the error message if this expectation is not met.
		/// You can describe here why this expectation has to be met.
		/// </param>
		public IVerifyableExpectation Comment(string comment)
		{
			return this.Expectation.Comment(comment);
		}

		/// <summary>
		/// Hooks up with the action that will be taken once a handler is added to the event.
		/// </summary>
		/// <returns> The action to hook the incoming handler to the event. </returns>
		[DebuggerStepThrough]
		private IAction Hookup(bool isDelegate)
		{
			return new MockEventHookup(this, isDelegate);
		}

		private void Initialize(Delegate handler)
		{
			this.Handler = handler;
		}

		/// <summary>
		/// Raises the event that created the expectations.
		/// </summary>
		/// <param name="args"> Arguments for the event. </param>
		protected void RaiseEvent(params object[] args)
		{
			var builder = (ExpectationBuilder)this.Expectation;
			var mockObject = builder.MockObject;

			var parameterCount = this.Handler.GetMethodInfo().GetParameters().Length;
			if (parameterCount != args.Length)
			{
				string eventName;
				if (this.EventName.Contains(Constants.ADD))
					eventName = this.EventName.Substring(Constants.ADD.Length);
				else if (this.EventName.Contains(Constants.REMOVE))
					eventName = this.EventName.Substring(Constants.REMOVE.Length);
				else
					eventName = this.EventName;

				throw new InvalidOperationException(eventName + " expects " + parameterCount + " parameter(s) but Invoke was supplied " + args.Length + " parameter(s).");
			}

			mockObject.RaiseEvent(this.EventName, args);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class MockEventHookup : IAction
		{
			#region Constructors/Destructors

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="invoker"> </param>
			/// <param name="isDelegate"> </param>
			public MockEventHookup(EventInvokerBase invoker, bool isDelegate)
			{
				this.invoker = invoker;
				this.isDelegateBinding = isDelegate;
			}

			#endregion

			#region Fields/Constants

			private readonly EventInvokerBase invoker;
			private readonly bool isDelegateBinding;

			#endregion

			#region Methods/Operators

			/// <summary>
			/// Describes this object.
			/// </summary>
			/// <param name="writer"> The text writer the description is added to. </param>
			void ISelfDescribing.DescribeTo(TextWriter writer)
			{
				string bindingKind = "n event";

				if (this.isDelegateBinding)
					bindingKind = " delegate";

				writer.Write(string.Format("bind a{0}, returning an invoker.", bindingKind));
			}

			/// <summary>
			/// Invokes this object.
			/// </summary>
			/// <param name="invocation"> The invocation. </param>
			void IAction.Invoke(Invocation invocation)
			{
				var eventHandler = invocation.Parameters[0] as Delegate;

				if (eventHandler != null)
				{
					this.invoker.Initialize(eventHandler);
					return;
				}

				throw new ArgumentNullException(string.Concat(@"invocation.", @"Parameters[0]"), string.Format("Event handler parameter is of the wrong type: {0}. EventHandler type expected.", invocation.Parameters[0].GetType()));
			}

			#endregion
		}

		#endregion
	}
}