using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NMock.Proxy.Reflective
{
	public interface IInvocation
	{
		MethodBase Method { get; set; }
		object[] Arguments { get; set; }
		object ReturnValue { get; set; }
		object InvocationTarget { get; set; }
		object Proxy { get; set; }
		void Proceed();
	}
}
