#region Using

using System;
using System.Reflection;

using Castle.DynamicProxy;

#endregion

namespace NMock.Proxy.Castle
{
	//used for testing
	//trying to remove as much DP logic as possible
	//use the other until this becomes stable
	internal class DynamicProxyMockObjectFactory : MockObjectFactoryBase
	{
		#region Constructors/Destructors

		public DynamicProxyMockObjectFactory()
		{
			this.builder = new DefaultProxyBuilder();
			this.generator = new ProxyGenerator(this.builder);
		}

		#endregion

		#region Fields/Constants

		protected IProxyBuilder builder;
		protected ProxyGenerator generator;

		#endregion

		#region Methods/Operators

		public override object CreateMock(MockFactory mockFactory, CompositeType typesToMock, string name, MockStyle mockStyle, object[] constructorArgs)
		{
			if (typesToMock == null)
				throw new ArgumentNullException("typesToMock");

			Type primaryType = typesToMock.PrimaryType;
			Type[] additionalInterfaces = BuildAdditionalTypeArrayForProxyType(typesToMock);
			IInterceptor mockInterceptor = new MockObjectInterceptor(mockFactory, typesToMock, name, mockStyle);
			object result;

			var _type = primaryType.GetTypeInfo();

			if (_type.IsInterface)
			{
				result = this.generator.CreateInterfaceProxyWithoutTarget(primaryType, additionalInterfaces, new ProxyGenerationOptions { BaseTypeForInterfaceProxy = typeof(InterfaceMockBase) }, mockInterceptor);
				((InterfaceMockBase)result).Name = name;
			}
			else
			{
				result = this.generator.CreateClassProxy(primaryType, additionalInterfaces, ProxyGenerationOptions.Default, constructorArgs, mockInterceptor);
				//return generator.CreateClassProxy(primaryType, new []{typeof(IMockObject)}, mockInterceptor);
			}

			return result;
		}

		#endregion
	}
}