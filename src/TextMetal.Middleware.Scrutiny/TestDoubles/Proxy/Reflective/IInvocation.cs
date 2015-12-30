using System;
using System.Reflection;

namespace NMock.Proxy.Reflective
{
	public interface IInvocation
	{
		#region Properties/Indexers/Events

		object[] Arguments
		{
			get;
			set;
		}

		object InvocationTarget
		{
			get;
			set;
		}

		MethodBase Method
		{
			get;
			set;
		}

		object Proxy
		{
			get;
			set;
		}

		object ReturnValue
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		void Proceed();

		#endregion
	}
}