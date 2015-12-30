// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Internal;

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;

	public abstract class AbstractTypeEmitter
	{
		#region Constructors/Destructors

		protected AbstractTypeEmitter(TypeBuilder typeBuilder)
		{
			this.typebuilder = typeBuilder;
			this.nested = new NestedClassCollection();
			this.methods = new MethodCollection();
			this.constructors = new ConstructorCollection();
			this.properties = new PropertiesCollection();
			this.events = new EventCollection();
			this.name2GenericType = new Dictionary<String, GenericTypeParameterBuilder>();
		}

		#endregion

		#region Fields/Constants

		private const MethodAttributes defaultAttributes =
			MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public;

		private readonly ConstructorCollection constructors;
		private readonly EventCollection events;

		private readonly IDictionary<string, FieldReference> fields =
			new Dictionary<string, FieldReference>(StringComparer.OrdinalIgnoreCase);

		private readonly MethodCollection methods;

		private readonly Dictionary<String, GenericTypeParameterBuilder> name2GenericType;
		private readonly NestedClassCollection nested;
		private readonly PropertiesCollection properties;
		private readonly TypeBuilder typebuilder;

		private GenericTypeParameterBuilder[] genericTypeParams;

		#endregion

		#region Properties/Indexers/Events

		public Type BaseType
		{
			get
			{
				if (this.TypeBuilder.GetTypeInfo().IsInterface)
					throw new InvalidOperationException("This emitter represents an interface; interfaces have no base types.");
				return this.TypeBuilder.BaseType;
			}
		}

		public ConstructorCollection Constructors
		{
			get
			{
				return this.constructors;
			}
		}

		public GenericTypeParameterBuilder[] GenericTypeParams
		{
			get
			{
				return this.genericTypeParams;
			}
		}

		public NestedClassCollection Nested
		{
			get
			{
				return this.nested;
			}
		}

		public TypeBuilder TypeBuilder
		{
			get
			{
				return this.typebuilder;
			}
		}

		public TypeConstructorEmitter ClassConstructor
		{
			get;
			private set;
		}

		#endregion

		#region Methods/Operators

		public void AddCustomAttributes(ProxyGenerationOptions proxyGenerationOptions)
		{
			foreach (var attr in proxyGenerationOptions.attributesToAddToGeneratedTypes)
			{
				var customAttributeBuilder = AttributeUtil.CreateBuilder(attr);
				if (customAttributeBuilder != null)
					this.typebuilder.SetCustomAttribute(customAttributeBuilder);
			}

			foreach (var attribute in proxyGenerationOptions.AdditionalAttributes)
				this.typebuilder.SetCustomAttribute(attribute);
		}

		public virtual Type BuildType()
		{
			this.EnsureBuildersAreInAValidState();

			var type = this.CreateType(this.typebuilder);

			foreach (var builder in this.nested)
				builder.BuildType();

			return type;
		}

		public void CopyGenericParametersFromMethod(MethodInfo methodToCopyGenericsFrom)
		{
			// big sanity check
			if (this.genericTypeParams != null)
				throw new ProxyGenerationException("CopyGenericParametersFromMethod: cannot invoke me twice");

			this.SetGenericTypeParameters(GenericUtil.CopyGenericArguments(methodToCopyGenericsFrom, this.typebuilder, this.name2GenericType));
		}

		public ConstructorEmitter CreateConstructor(params ArgumentReference[] arguments)
		{
			if (this.TypeBuilder.GetTypeInfo().IsInterface)
				throw new InvalidOperationException("Interfaces cannot have constructors.");

			var member = new ConstructorEmitter(this, arguments);
			this.constructors.Add(member);
			return member;
		}

		public void CreateDefaultConstructor()
		{
			if (this.TypeBuilder.GetTypeInfo().IsInterface)
				throw new InvalidOperationException("Interfaces cannot have constructors.");

			this.constructors.Add(new ConstructorEmitter(this));
		}

		public EventEmitter CreateEvent(string name, EventAttributes atts, Type type)
		{
			var eventEmitter = new EventEmitter(this, name, atts, type);
			this.events.Add(eventEmitter);
			return eventEmitter;
		}

		public FieldReference CreateField(string name, Type fieldType)
		{
			return this.CreateField(name, fieldType, true);
		}

		public FieldReference CreateField(string name, Type fieldType, bool serializable)
		{
			var atts = FieldAttributes.Public;

			if (!serializable)
				atts |= FieldAttributes.NotSerialized;

			return this.CreateField(name, fieldType, atts);
		}

		public FieldReference CreateField(string name, Type fieldType, FieldAttributes atts)
		{
			var fieldBuilder = this.typebuilder.DefineField(name, fieldType, atts);
			var reference = new FieldReference(fieldBuilder);
			this.fields[name] = reference;
			return reference;
		}

		public MethodEmitter CreateMethod(string name, MethodAttributes attrs, Type returnType, params Type[] argumentTypes)
		{
			var member = new MethodEmitter(this, name, attrs, returnType, argumentTypes ?? Type.EmptyTypes);
			this.methods.Add(member);
			return member;
		}

		public MethodEmitter CreateMethod(string name, Type returnType, params Type[] parameterTypes)
		{
			return this.CreateMethod(name, defaultAttributes, returnType, parameterTypes);
		}

		public MethodEmitter CreateMethod(string name, MethodInfo methodToUseAsATemplate)
		{
			return this.CreateMethod(name, defaultAttributes, methodToUseAsATemplate);
		}

		public MethodEmitter CreateMethod(string name, MethodAttributes attributes, MethodInfo methodToUseAsATemplate)
		{
			var method = new MethodEmitter(this, name, attributes, methodToUseAsATemplate);
			this.methods.Add(method);
			return method;
		}

		public PropertyEmitter CreateProperty(string name, PropertyAttributes attributes, Type propertyType, Type[] arguments)
		{
			var propEmitter = new PropertyEmitter(this, name, attributes, propertyType, arguments);
			this.properties.Add(propEmitter);
			return propEmitter;
		}

		public FieldReference CreateStaticField(string name, Type fieldType)
		{
			return this.CreateStaticField(name, fieldType, FieldAttributes.Public);
		}

		public FieldReference CreateStaticField(string name, Type fieldType, FieldAttributes atts)
		{
			atts |= FieldAttributes.Static;
			return this.CreateField(name, fieldType, atts);
		}

		protected Type CreateType(TypeBuilder type)
		{
			try
			{
#if NETCORE
				return type.CreateTypeInfo().AsType();
#else
				return type.CreateType();
#endif
			}
			catch (BadImageFormatException ex)
			{
				if (Debugger.IsAttached == false)
					throw;

				if (ex.Message.Contains(@"HRESULT: 0x8007000B") == false)
					throw;

				if (type.GetTypeInfo().IsGenericTypeDefinition == false)
					throw;

				var message =
					"This is a DynamicProxy2 error: It looks like you enoutered a bug in Visual Studio debugger, " +
					"which causes this exception when proxying types with generic methods having constraints on their generic arguments." +
					"This code will work just fine without the debugger attached. " +
					"If you wish to use debugger you may have to switch to Visual Studio 2010 where this bug was fixed.";
				var exception = new ProxyGenerationException(message);
				exception.Data.Add("ProxyType", type.ToString());
				throw exception;
			}
		}

		public ConstructorEmitter CreateTypeConstructor()
		{
			var member = new TypeConstructorEmitter(this);
			this.constructors.Add(member);
			this.ClassConstructor = member;
			return member;
		}

		public void DefineCustomAttribute(CustomAttributeBuilder attribute)
		{
			this.typebuilder.SetCustomAttribute(attribute);
		}

		public void DefineCustomAttribute<TAttribute>(object[] constructorArguments) where TAttribute : Attribute
		{
			var customAttributeBuilder = AttributeUtil.CreateBuilder(typeof(TAttribute), constructorArguments);
			this.typebuilder.SetCustomAttribute(customAttributeBuilder);
		}

		public void DefineCustomAttribute<TAttribute>() where TAttribute : Attribute, new()
		{
			var customAttributeBuilder = AttributeUtil.CreateBuilder<TAttribute>();
			this.typebuilder.SetCustomAttribute(customAttributeBuilder);
		}

		public void DefineCustomAttributeFor<TAttribute>(FieldReference field) where TAttribute : Attribute, new()
		{
			var customAttributeBuilder = AttributeUtil.CreateBuilder<TAttribute>();
			var fieldbuilder = field.Fieldbuilder;
			if (fieldbuilder == null)
			{
				throw new ArgumentException(
					"Invalid field reference.This reference does not point to field on type being generated", "field");
			}
			fieldbuilder.SetCustomAttribute(customAttributeBuilder);
		}

		protected virtual void EnsureBuildersAreInAValidState()
		{
			if (!this.typebuilder.GetTypeInfo().IsInterface && this.constructors.Count == 0)
				this.CreateDefaultConstructor();

			foreach (IMemberEmitter builder in this.properties)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach (IMemberEmitter builder in this.events)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach (IMemberEmitter builder in this.constructors)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach (IMemberEmitter builder in this.methods)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
		}

		public IEnumerable<FieldReference> GetAllFields()
		{
			return this.fields.Values;
		}

		public FieldReference GetField(string name)
		{
			if (string.IsNullOrEmpty(name))
				return null;

			FieldReference value;
			this.fields.TryGetValue(name, out value);
			return value;
		}

		public Type GetGenericArgument(String genericArgumentName)
		{
			if (this.name2GenericType.ContainsKey(genericArgumentName))
#if NETCORE
				return this.name2GenericType[genericArgumentName].AsType();
#else
				return name2GenericType[genericArgumentName];
#endif
			return null;
		}

		public Type[] GetGenericArgumentsFor(Type genericType)
		{
			var types = new List<Type>();

			foreach (var genType in genericType.GetGenericArguments())
			{
				if (genType.GetTypeInfo().IsGenericParameter)
				{
#if NETCORE
					types.Add(this.name2GenericType[genType.Name].AsType());
#else
					types.Add(name2GenericType[genType.Name]);
#endif
				}
				else
					types.Add(genType);
			}

			return types.ToArray();
		}

		public Type[] GetGenericArgumentsFor(MethodInfo genericMethod)
		{
			var types = new List<Type>();
			foreach (var genType in genericMethod.GetGenericArguments())
			{
#if NETCORE
				types.Add(this.name2GenericType[genType.Name].AsType());
#else
				types.Add(name2GenericType[genType.Name]);
#endif
			}

			return types.ToArray();
		}

		public void SetGenericTypeParameters(GenericTypeParameterBuilder[] genericTypeParameterBuilders)
		{
			this.genericTypeParams = genericTypeParameterBuilders;
		}

		#endregion
	}
}