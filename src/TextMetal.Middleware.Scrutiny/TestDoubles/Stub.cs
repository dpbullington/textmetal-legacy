#region Using

using System;
using System.Reflection;

using NMock.Internal;
using NMock.Syntax;

#endregion

namespace NMock
{
	/// <summary>
	/// This class represents a stub.
	/// </summary>
	/// <typeparam name="T"> </typeparam>
	public class Stub<T>
		where T : class
	{
		#region Constructors/Destructors

		internal Stub(object proxy)
		{
			if (proxy == null)
				throw new ArgumentNullException("proxy");

			this._proxy = proxy;
		}

		#endregion

		#region Fields/Constants

		private readonly object _proxy;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// A syntax property used to stub out data for this instance.
		/// </summary>
		public IStubSyntax<T> Out
		{
			get
			{
				var _type = typeof(T).GetTypeInfo();

				if (_type.IsInterface || _type.IsClass)
					return new ExpectationBuilder<T>(ExpectationBuilder.STUB_DESCRIPTION, Is.AtLeast(0), Is.AtLeast(0), this._proxy) { IsStub = true };

				throw new InvalidOperationException("The type mocked is not a class or interface.");
			}
		}

		#endregion
	}
}