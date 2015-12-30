#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

using Castle.DynamicProxy;

#endregion

namespace NMock.Proxy.Castle
{
	/// <summary>
	/// Class that creates mocks for interfaces and classes (virtual members only) using the
	/// Castle proxy generator.
	/// </summary>
	public class CastleMockObjectFactory : MockObjectFactoryBase
	{
		#region Constructors/Destructors

		//protected ProxyGenerator Generator;

		/// <summary>
		/// Initializes a new instance of a <see cref="CastleMockObjectFactory" />
		/// </summary>
		public CastleMockObjectFactory()
			: this(false)
		{
		}

		/// <summary>
		/// Initializes a new instance of a <see cref="CastleMockObjectFactory" />
		/// </summary>
		/// <param name="usePersistentProxyBuilder"> A value indicating if the ProxyBuilder should be persistent </param>
		public CastleMockObjectFactory(bool usePersistentProxyBuilder)
		{
#if !SILVERLIGHT && !NETCORE // no PersistentProxyBuilder in Silverlight
				if(usePersistentProxyBuilder)
				{
					ProxyBuilder = new PersistentProxyBuilder();
				}
				else
				{
#endif
			this.ProxyBuilder = new DefaultProxyBuilder();
#if !SILVERLIGHT && !NETCORE
				}
#endif
			//Generator = new ProxyGenerator(ProxyBuilder);
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// A collection of types
		/// </summary>
		private static readonly Dictionary<CompositeType, Type> CachedProxyTypes = new Dictionary<CompositeType, Type>();

		private readonly IProxyBuilder ProxyBuilder;
		private object locker = new object();

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Creates a mock of the specified type(s).
		/// </summary>
		/// <param name="mockFactory"> The mockFactory used to create this mock instance. </param>
		/// <param name="typesToMock"> The type(s) to include in the mock. </param>
		/// <param name="name"> The name to use for the mock instance. </param>
		/// <param name="mockStyle"> The behaviour of the mock instance when first created. </param>
		/// <param name="constructorArgs"> Constructor arguments for the class to be mocked. Only valid if mocking a class type. </param>
		/// <returns> A mock instance of the specified type(s). </returns>
		public override object CreateMock(MockFactory mockFactory, CompositeType typesToMock, string name, MockStyle mockStyle, object[] constructorArgs)
		{
			Type proxyType = this.GetProxyType(typesToMock);

			return this.InstantiateProxy(typesToMock, proxyType, mockFactory, mockStyle, name, constructorArgs);
		}

		private Type GetProxyType(CompositeType compositeType)
		{
			if (compositeType == null)
				throw new ArgumentNullException("compositeType");

			var _type = compositeType.PrimaryType.GetTypeInfo();

			if (!CachedProxyTypes.ContainsKey(compositeType))
			{
				Type[] additionalInterfaceTypes = BuildAdditionalTypeArrayForProxyType(compositeType);
				Type proxyType;

				if (_type.IsClass)
				{
					if (_type.IsSealed)
						throw new ArgumentException("Cannot mock sealed classes.");

					proxyType = this.ProxyBuilder.CreateClassProxyType(compositeType.PrimaryType, additionalInterfaceTypes, ProxyGenerationOptions.Default);
				}
				else
					proxyType = this.ProxyBuilder.CreateInterfaceProxyTypeWithoutTarget(compositeType.PrimaryType, additionalInterfaceTypes, new ProxyGenerationOptions { BaseTypeForInterfaceProxy = typeof(InterfaceMockBase) });

				lock (this.locker)
				{
					if (!CachedProxyTypes.ContainsKey(compositeType))
						CachedProxyTypes[compositeType] = proxyType;
				}
				return proxyType;
			}

			return CachedProxyTypes[compositeType];
		}

		private object InstantiateProxy(CompositeType compositeType, Type proxyType, MockFactory mockFactory, MockStyle mockStyle, string name, object[] constructorArgs)
		{
			IInterceptor interceptor = new MockObjectInterceptor(mockFactory, compositeType, name, mockStyle);
			object[] activationArgs;

			var _type = compositeType.PrimaryType.GetTypeInfo();

			if (_type.IsClass)
			{
				activationArgs = new object[constructorArgs.Length + 1];
				constructorArgs.CopyTo(activationArgs, 1);
				activationArgs[0] = new[] { interceptor };
			}
			else
				activationArgs = new[] { new[] { interceptor }, new object(), name };

			return Activator.CreateInstance(proxyType, activationArgs);
		}

		#endregion
	}
}