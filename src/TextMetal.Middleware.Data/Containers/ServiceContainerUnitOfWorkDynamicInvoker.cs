/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;
using TextMetal.Middleware.Solder.SoC;

namespace TextMetal.Middleware.Data.Containers
{
	public class ServiceContainerUnitOfWorkDynamicInvoker : AspectDynamicInvoker
	{
		#region Constructors/Destructors

		public ServiceContainerUnitOfWorkDynamicInvoker(IServiceContainer serviceContainer)
		{
			if ((object)serviceContainer == null)
				throw new ArgumentNullException("serviceContainer");

			this.serviceContainer = serviceContainer;
		}

		#endregion

		#region Fields/Constants

		private readonly IServiceContainer serviceContainer;

		#endregion

		#region Properties/Indexers/Events

		public override object InterceptedInstance
		{
			get
			{
				return this.ServiceContainer;
			}
		}

		private IServiceContainer ServiceContainer
		{
			get
			{
				return this.serviceContainer;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void OnInterceptAfterInvoke(bool invocationPreceeded, Type proxiedType, MethodInfo invokedMethodInfo, object proxyInstance, object[] invocationParameters, ref object returnValue, ref Exception thrownException)
		{
		}

		protected override void OnInterceptBeforeInvoke(out bool proceedWithInvocation, Type proxiedType, MethodInfo invokedMethodInfo, object proxyInstance, object[] invocationParameters)
		{
			proceedWithInvocation = true;
		}

		protected override object OnInterceptProceedInvoke(Type proxiedType, MethodInfo invokedMethodInfo, object proxyInstance, object[] invocationParameters)
		{
			object returnValue = null;

			if ((object)proxiedType == null)
				throw new ArgumentNullException("proxiedType");

			if ((object)invokedMethodInfo == null)
				throw new ArgumentNullException("invokedMethodInfo");

			if ((object)proxyInstance == null)
				throw new ArgumentNullException("proxyInstance");

			if ((object)invocationParameters == null)
				throw new ArgumentNullException("invocationParameters");

			// TODO: add attribute with transactional flag
			using (AmbientUnitOfWorkScope scope = new AmbientUnitOfWorkScope(this.ServiceContainer, true))
				returnValue = base.OnInterceptProceedInvoke(proxiedType, invokedMethodInfo, proxyInstance, invocationParameters);

			return returnValue;
		}

		#endregion
	}
}