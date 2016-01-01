using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMock.Proxy.Reflective
{
	public interface IInterceptor
	{
		void Intercept(IInvocation invocation);
	}
}
