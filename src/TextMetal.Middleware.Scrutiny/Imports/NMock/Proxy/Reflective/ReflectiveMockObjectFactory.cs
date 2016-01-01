using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NMock.Proxy.Reflective
{
	public class ReflectiveMockObjectFactory : MockObjectFactoryBase
	{
		private const string ASSEMBLY_NAME = "ReflectiveMock";
		private const string EXTENSION = ".dll";
		private const string DYNAMIC_TYPE = "ReflectiveProxy";
		private const TypeAttributes DEFAULT_TYPE_ATTRIBUTES = TypeAttributes.Public | TypeAttributes.BeforeFieldInit | TypeAttributes.Class | TypeAttributes.Serializable;
		private const MethodAttributes EXPLICITIMPLEMENTATION = MethodAttributes.Private | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot;
		private const MethodAttributes IMPLICITIMPLEMENTATION = MethodAttributes.Public | MethodAttributes.Virtual; // | MethodAttributes.HideBySig;

		AssemblyBuilder _assemblyBuilder;
		ModuleBuilder _moduleBuilder;
		TypeBuilder _typeBuilder;
		private CompositeType _typesToMock;

		public override object CreateMock(MockFactory mockFactory, CompositeType typesToMock, string name, MockStyle mockStyle, object[] constructorArgs)
		{
			_typesToMock = typesToMock;

			_typesToMock.Add(typeof(IMockObject));

			var reflectiveInterceptor = new ReflectiveInterceptor(mockFactory, typesToMock, name, mockStyle);

			var proxy = CreateMock(reflectiveInterceptor, constructorArgs);

			if (_typesToMock.PrimaryType.IsInterface)
				((InterfaceMockBase) proxy).Name = name;

			return proxy;
		}

		private object CreateMock(ReflectiveInterceptor reflectiveInterceptor, object[] constructorArgs)
		{
			var name = new AssemblyName(ASSEMBLY_NAME);

#if !SILVERLIGHT
			_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
			_moduleBuilder = _assemblyBuilder.DefineDynamicModule(name.Name, name.Name + EXTENSION);
#else
			_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
			_moduleBuilder = _assemblyBuilder.DefineDynamicModule(name.Name);
#endif
			_typeBuilder = GetTypeBuilder();

			var type = ImplementType();

			//Save();

			return Activator.CreateInstance(type, constructorArgs);
		}

		private TypeBuilder GetTypeBuilder()
		{
			if (_typesToMock.PrimaryType.IsInterface)
				return _moduleBuilder.DefineType(DYNAMIC_TYPE, DEFAULT_TYPE_ATTRIBUTES, typeof(InterfaceMockBase), _typesToMock.All);
			else
				return _moduleBuilder.DefineType(DYNAMIC_TYPE, DEFAULT_TYPE_ATTRIBUTES, _typesToMock.PrimaryType, _typesToMock.AdditionalInterfaceTypes);
		}

		private Type ImplementType()
		{
			DefineConstructors();
			DefineMethods();

			return _typeBuilder.CreateType();
		}

		private void DefineConstructors()
		{
			if (_typesToMock.PrimaryType.IsClass)
			{
				var constructors = _typesToMock.PrimaryType.GetConstructors();

				var defaultConstructor = _typesToMock.PrimaryType.GetConstructor(Type.EmptyTypes);

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

					var constructorBuilder = _typeBuilder.DefineConstructor(
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
			
			foreach (var type in _typesToMock.All)
			{
				foreach (var methodInfo in type.GetMethods())
				{
					var methodBuilder = _typeBuilder.DefineMethod(
						methodInfo.Name,
						IMPLICITIMPLEMENTATION,
						methodInfo.ReturnType,
						methodInfo.GetParameters().Select(_ => _.ParameterType).ToArray());
					var ilGenerator = methodBuilder.GetILGenerator();

					ilGenerator.Emit(OpCodes.Ret);
				}
			}
		}

#if !SILVERLIGHT
		private void Save()
		{
			Console.WriteLine(_moduleBuilder.FullyQualifiedName);

			if(File.Exists(_moduleBuilder.FullyQualifiedName))
			{
				File.Delete(_moduleBuilder.FullyQualifiedName);
			}
			_assemblyBuilder.Save(Path.GetFileName(_moduleBuilder.FullyQualifiedName));
		}
#endif
	}

	public class Test
	{
		public virtual void N()
		{
			

		}

		public void M()
		{
			var i = 1;
			i++;
		}
	}

	public class Test2 : Test
	{
		public new void M()
		{
			base.M();
		}

		public override void N()
		{
			base.N();
		}
	}
}