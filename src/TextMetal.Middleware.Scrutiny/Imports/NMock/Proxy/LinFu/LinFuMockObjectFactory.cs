#if !SILVERLIGHT
using System;
using NMock.Internal;
using LinFu.Proxy;
using LinFu.Proxy.Interfaces;

namespace NMock.Proxy.LinFu
{
	/// <summary>
	/// A factory based on LinFu that creates proxy objects
	/// </summary>
	public class LinFuMockObjectFactory : MockObjectFactoryBase
	{
		ProxyFactory factory = new ProxyFactory();

		/// <summary>
		/// Creates a mock of the specified type(s).
		/// </summary>
		/// <param name="mockFactory">The mockFactory used to create this mock instance.</param>
		/// <param name="typesToMock">The type(s) to include in the mock.</param>
		/// <param name="name">The name to use for the mock instance.</param>
		/// <param name="mockStyle">The behaviour of the mock instance when first created.</param>
		/// <param name="constructorArgs">Constructor arguments for the class to be mocked. Only valid if mocking a class type.</param>
		/// <returns>A mock instance of the specified type(s).</returns>
		public override object CreateMock(MockFactory mockFactory, CompositeType typesToMock, string name, MockStyle mockStyle, object[] constructorArgs)
		{
			return InstantiateProxy(typesToMock, mockFactory, mockStyle, name, constructorArgs);
		}
		private object InstantiateProxy(CompositeType compositeType, MockFactory mockFactory, MockStyle mockStyle, string name, object[] constructorArgs)
		{
			LinFuMockObjectInterceptor interceptor = new LinFuMockObjectInterceptor(mockFactory, compositeType, name, mockStyle);
			object proxy;

			if (compositeType.PrimaryType.IsClass)
			{
				Type[] additionalInterfaceTypes = BuildAdditionalTypeArrayForProxyType(compositeType);
				if (compositeType.PrimaryType.IsSealed)
				{
					throw new ArgumentException("Cannot mock sealed classes.");
				}

				proxy = factory.CreateProxy(compositeType.PrimaryType, interceptor, additionalInterfaceTypes);
			}
			else
			{
				Type[] additionalInterfaceTypes = BuildAdditionalTypeArrayForProxyType(compositeType);
				proxy = factory.CreateProxy(typeof(InterfaceMockBase), interceptor, additionalInterfaceTypes);
			}

			return proxy;
		}
	}
}
#endif
