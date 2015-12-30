using System;

namespace NMock.Proxy.Reflective
{
	public interface IInterceptor
	{
		#region Methods/Operators

		void Intercept(IInvocation invocation);

		#endregion
	}
}