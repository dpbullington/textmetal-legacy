/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

using TextMetal.Common.Solder.RuntimeInterception;

namespace TextMetal.Common.UnitTests.TestingInfrastructure
{
	public class MockAspectDynamicInvoker : AspectDynamicInvoker
	{
		#region Constructors/Destructors

		public MockAspectDynamicInvoker()
			: this(null)
		{
		}

		public MockAspectDynamicInvoker(object interceptedInstance)
		{
			this.interceptedInstance = interceptedInstance;
		}

		#endregion

		#region Fields/Constants

		private string lastOperationName;
		private readonly object interceptedInstance;

		#endregion

		#region Properties/Indexers/Events

		public string LastOperationName
		{
			get
			{
				return this.lastOperationName;
			}
			private set
			{
				this.lastOperationName = value;
			}
		}

		public override object InterceptedInstance
		{
			get
			{
				return this.interceptedInstance;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void OnInterceptAfterInvoke(bool invocationPreceeded, Type proxiedType, MethodInfo invokedMethodInfo, object proxyInstance, object[] invocationParameters, ref object returnValue, ref Exception thrownException)
		{
			this.LastOperationName = string.Format("{0}::{1}", (object)proxiedType == null ? "<null>" : proxiedType.Name, (object)invokedMethodInfo == null ? "<null>" : invokedMethodInfo.Name);
		}

		protected override void OnInterceptBeforeInvoke(out bool proceedWithInvocation, Type proxiedType, MethodInfo invokedMethodInfo, object proxyInstance, object[] invocationParameters)
		{
			proceedWithInvocation = true;
		}

		protected override object OnInterceptProceedInvoke(Type proxiedType, MethodInfo invokedMethodInfo, object proxyInstance, object[] invocationParameters)
		{
			if (this.InterceptedInstance is Exception)
				throw (Exception)this.InterceptedInstance;

			return base.OnInterceptProceedInvoke(proxiedType, invokedMethodInfo, proxyInstance, invocationParameters);
		}

		#endregion
	}
}