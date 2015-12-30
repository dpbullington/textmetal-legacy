using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NMock.Proxy.Reflective
{
	public class ReflectiveMockObjectFactory : MockObjectFactoryBase
	{
		#region Fields/Constants

		private const string ASSEMBLY_NAME = "ReflectiveMock";
		private const TypeAttributes DEFAULT_TYPE_ATTRIBUTES = TypeAttributes.Public | TypeAttributes.BeforeFieldInit | TypeAttributes.Class | TypeAttributes.Serializable;
		private const string DYNAMIC_TYPE = "ReflectiveProxy";
		private const MethodAttributes IMPLICITIMPLEMENTATION = MethodAttributes.Public | MethodAttributes.Virtual; // | MethodAttributes.HideBySig;

		private AssemblyBuilder _assemblyBuilder;
		private ModuleBuilder _moduleBuilder;
		private TypeBuilder _typeBuilder;
		private CompositeType _typesToMock;

		#endregion

		#region Methods/Operators

		public override object CreateMock(MockFactory mockFactory, CompositeType typesToMock, string name, MockStyle mockStyle, object[] constructorArgs)
		{
			this._typesToMock = typesToMock;

			this._typesToMock.Add(typeof(IMockObject));

			var reflectiveInterceptor = new ReflectiveInterceptor(mockFactory, typesToMock, name, mockStyle);

			var proxy = this.CreateMock(reflectiveInterceptor, constructorArgs);

			var _type = this._typesToMock.PrimaryType.GetTypeInfo();

			if (_type.IsInterface)
				((InterfaceMockBase)proxy).Name = name;

			return proxy;
		}

		private object CreateMock(ReflectiveInterceptor reflectiveInterceptor, object[] constructorArgs)
		{
			var name = new AssemblyName(ASSEMBLY_NAME);

			this._assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
			this._moduleBuilder = this._assemblyBuilder.DefineDynamicModule(name.Name);

			this._typeBuilder = this.GetTypeBuilder();

			var type = this.ImplementType();

			return Activator.CreateInstance(type, constructorArgs);
		}

		private void DefineConstructors()
		{
			var _type = this._typesToMock.PrimaryType.GetTypeInfo();
			if (_type.IsClass)
			{
				var constructors = this._typesToMock.PrimaryType.GetConstructors();

				var defaultConstructor = this._typesToMock.PrimaryType.GetConstructor(Type.EmptyTypes);

				//if (defaultConstructor == null)
				//{
				//    var constructorBuilder = _typeBuilder.DefineConstructor(
				//        MethodAttributes.Public,
				//        CallingConventions.Standard,
				//        Type.EmptyTypes);
				//    var generator = constructorBuilder.GetILGenerator();

				//    //generator.Emit(OpCodes.Ldarg_0);
				//    generator.Emit(OpCodes.Ret);
				//}

				foreach (var constructorInfo in constructors)
				{
					if (constructorInfo == defaultConstructor)
						break;

					var constructorBuilder = this._typeBuilder.DefineConstructor(
						constructorInfo.Attributes, // | MethodAttributes.HideBySig,
						CallingConventions.Standard,
						constructorInfo.GetParameters().Select(_ => _.ParameterType).ToArray());
					var generator = constructorBuilder.GetILGenerator();

					generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Ldarg_1);
					generator.Emit(OpCodes.Ldarg_2);
					generator.Emit(OpCodes.Call, constructorInfo);
					generator.Emit(OpCodes.Ret);
				}
				/*
				 * 
IL_0000:  ldarg.0
  IL_0001:  ldarg.1
  IL_0002:  call       instance void NMockTests._TestStructures.ParentClass::.ctor(string)
  IL_0007:  nop
  IL_0008:  nop
  IL_0009:  nop
  IL_000a:  ret
				var defaultConstructor = _typesToMock.PrimaryType.GetConstructor(Type.EmptyTypes);

				if (defaultConstructor == null)
				{
					var constructorBuilder = _typeBuilder.DefineConstructor(
						MethodAttributes.Public,
						CallingConventions.Standard,
						Type.EmptyTypes);
					var generator = constructorBuilder.GetILGenerator();

					//generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Ret);
				}
				*/
			}
		}

		private void DefineMethods()
		{
			foreach (var type in this._typesToMock.All)
			{
				foreach (var methodInfo in type.GetMethods())
				{
					var methodBuilder = this._typeBuilder.DefineMethod(
						methodInfo.Name,
						IMPLICITIMPLEMENTATION,
						methodInfo.ReturnType,
						methodInfo.GetParameters().Select(_ => _.ParameterType).ToArray());
					var ilGenerator = methodBuilder.GetILGenerator();

					ilGenerator.Emit(OpCodes.Ret);
				}
			}
		}

		private TypeBuilder GetTypeBuilder()
		{
			var _type = this._typesToMock.PrimaryType.GetTypeInfo();

			if (_type.IsInterface)
				return this._moduleBuilder.DefineType(DYNAMIC_TYPE, DEFAULT_TYPE_ATTRIBUTES, typeof(InterfaceMockBase), this._typesToMock.All);
			else
				return this._moduleBuilder.DefineType(DYNAMIC_TYPE, DEFAULT_TYPE_ATTRIBUTES, this._typesToMock.PrimaryType, this._typesToMock.AdditionalInterfaceTypes);
		}

		private Type ImplementType()
		{
			this.DefineConstructors();
			this.DefineMethods();

			return this._typeBuilder.CreateTypeInfo().AsType();
		}

		#endregion
	}

	public class Test
	{
		#region Methods/Operators

		public void M()
		{
			var i = 1;
			i++;
		}

		public virtual void N()
		{
		}

		#endregion
	}

	public class Test2 : Test
	{
		#region Methods/Operators

		public new void M()
		{
			base.M();
		}

		public override void N()
		{
			base.N();
		}

		#endregion
	}
}