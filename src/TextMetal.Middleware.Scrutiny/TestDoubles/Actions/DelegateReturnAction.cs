using System;
using System.IO;

using NMock.Monitoring;

namespace NMock.Actions
{
	/// <summary>
	/// Represents an <see cref="IReturnAction" /> that can call a delegate to provide the return value.
	/// </summary>
	/// <typeparam name="T"> The type to return. </typeparam>
	public class DelegateReturnAction<T> : IReturnAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Creates an instance of this class with a <see cref="Func{T}" /> to call during invocation.
		/// </summary>
		/// <param name="func"> The <see cref="Func{T}" /> to invoke. </param>
		public DelegateReturnAction(Func<T> func)
		{
			this._func = func;
		}

		/// <summary>
		/// Creates an instance of this class with a <see cref="Func{T}" /> to call during invocation.
		/// </summary>
		/// <param name="func"> The <see cref="Func{T}" /> to invoke. </param>
		public DelegateReturnAction(Func<Invocation, T> func)
		{
			this._invokeFunc = func;
		}

		#endregion

		#region Fields/Constants

		private readonly Func<T> _func;
		private readonly Func<Invocation, T> _invokeFunc;

		#endregion

		#region Properties/Indexers/Events

		Type IReturnAction.ReturnType
		{
			get
			{
				return typeof(T);
			}
		}

		#endregion

		#region Methods/Operators

		void ISelfDescribing.DescribeTo(TextWriter writer)
		{
			writer.Write("returns a value from a Func");
		}

		void IAction.Invoke(Invocation invocation)
		{
			if (this._invokeFunc != null)
				invocation.Result = this._invokeFunc(invocation);
			else
				invocation.Result = this._func;
		}

		#endregion
	}
}