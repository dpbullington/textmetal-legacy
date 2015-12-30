#region Using

using System;
using System.IO;

using NMock.Monitoring;

#endregion

namespace NMock.Actions
{
	/// <summary>
	/// Represents an <see cref="IAction" /> that returns a result.
	/// </summary>
	public interface IReturnAction : IAction
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// The return type of this <see cref="IAction" />
		/// </summary>
		Type ReturnType
		{
			get;
		}

		#endregion
	}

	/// <summary>
	/// Action that sets the result value on an invocation.
	/// </summary>
	public class ReturnAction<T> : IReturnAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ReturnAction" /> class.
		/// </summary>
		/// <param name="result"> The result to set on the invocation as the return value. </param>
		public ReturnAction(T result)
		{
			this._result = result;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Stores the result to set on the invocation as the return value.
		/// </summary>
		private readonly T _result;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the type of the template parameter <typeparamref name="T" />.
		/// </summary>
		public Type ReturnType
		{
			get
			{
				return typeof(T);
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Describes this object.
		/// </summary>
		/// <param name="writer"> The text writer the description is added to. </param>
		public virtual void DescribeTo(TextWriter writer)
		{
			writer.Write("return ");
			//string description = _result.ToString();
			writer.Write(this._result);
		}

		/// <summary>
		/// Invokes this object. Sets the result value of the invocation.
		/// </summary>
		/// <param name="invocation"> The invocation. </param>
		public virtual void Invoke(Invocation invocation)
		{
			invocation.Result = this._result;
		}

		#endregion
	}

	/// <summary>
	/// Action that sets the result value on an invocation.
	/// </summary>
	public sealed class ReturnAction : ReturnAction<object>
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ReturnAction" /> class.
		/// </summary>
		/// <param name="result"> The result to set on the invocation as the return value. </param>
		public ReturnAction(object result)
			: base(result)
		{
		}

		#endregion
	}
}