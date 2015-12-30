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

using System.Reflection;
using System.Reflection.Emit;

using Castle.DynamicProxy.Tokens;

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;

	public class PropertyEmitter : IMemberEmitter
	{
		#region Constructors/Destructors

		// private ParameterInfo[] indexParameters;

		public PropertyEmitter(AbstractTypeEmitter parentTypeEmitter, string name, PropertyAttributes attributes,
			Type propertyType, Type[] arguments)
		{
			this.parentTypeEmitter = parentTypeEmitter;

			// DYNPROXY-73 - AmbiguousMatchException for properties
			// This is a workaround for a framework limitation in CLR 2.0 
			// This limitation was removed in CLR 2.0 SP1, but we don't want to 
			// tie ourselves to that version. This perform the lookup for the new overload
			// dynamically, so we have a nice fallback on vanilla CLR 2.0

			if (TypeBuilderMethods.DefineProperty == null)
			{
				DefineProperty_Clr2_0 oldDefineProperty = parentTypeEmitter.TypeBuilder.DefineProperty;
				this.builder = oldDefineProperty(name, attributes, propertyType, arguments);
			}
			else
			{
#if NETCORE
				var newDefinedProperty = (DefineProperty_Clr_2_0_SP1)
					TypeBuilderMethods.DefineProperty.CreateDelegate(typeof(DefineProperty_Clr_2_0_SP1),
						parentTypeEmitter.TypeBuilder);
#else
				var newDefinedProperty = (DefineProperty_Clr_2_0_SP1)
				                         Delegate.CreateDelegate(typeof(DefineProperty_Clr_2_0_SP1),
				                                                 parentTypeEmitter.TypeBuilder,
				                                                 TypeBuilderMethods.DefineProperty);
#endif
				this.builder = newDefinedProperty(
					name, attributes, CallingConventions.HasThis, propertyType,
					null, null, arguments, null, null);
			}
		}

		#endregion

		#region Fields/Constants

		private readonly PropertyBuilder builder;
		private readonly AbstractTypeEmitter parentTypeEmitter;
		private MethodEmitter getMethod;
		private MethodEmitter setMethod;

		#endregion

		#region Properties/Indexers/Events

		public MemberInfo Member
		{
			get
			{
				return null;
			}
		}

		public Type ReturnType
		{
			get
			{
				return this.builder.PropertyType;
			}
		}

		#endregion

		#region Methods/Operators

		public MethodEmitter CreateGetMethod(string name, MethodAttributes attrs, MethodInfo methodToOverride,
			params Type[] parameters)
		{
			if (this.getMethod != null)
				throw new InvalidOperationException("A get method exists");

			this.getMethod = new MethodEmitter(this.parentTypeEmitter, name, attrs, methodToOverride);
			return this.getMethod;
		}

		public MethodEmitter CreateGetMethod(string name, MethodAttributes attributes, MethodInfo methodToOverride)
		{
			return this.CreateGetMethod(name, attributes, methodToOverride, Type.EmptyTypes);
		}

		public MethodEmitter CreateSetMethod(string name, MethodAttributes attrs, MethodInfo methodToOverride,
			params Type[] parameters)
		{
			if (this.setMethod != null)
				throw new InvalidOperationException("A set method exists");

			this.setMethod = new MethodEmitter(this.parentTypeEmitter, name, attrs, methodToOverride);
			return this.setMethod;
		}

		public MethodEmitter CreateSetMethod(string name, MethodAttributes attributes, MethodInfo methodToOverride)
		{
			var method = this.CreateSetMethod(name, attributes, methodToOverride, Type.EmptyTypes);
			return method;
		}

		public void DefineCustomAttribute(CustomAttributeBuilder attribute)
		{
			this.builder.SetCustomAttribute(attribute);
		}

		public void EnsureValidCodeBlock()
		{
			if (this.setMethod != null)
				this.setMethod.EnsureValidCodeBlock();

			if (this.getMethod != null)
				this.getMethod.EnsureValidCodeBlock();
		}

		public void Generate()
		{
			if (this.setMethod != null)
			{
				this.setMethod.Generate();
				this.builder.SetSetMethod(this.setMethod.MethodBuilder);
			}

			if (this.getMethod != null)
			{
				this.getMethod.Generate();
				this.builder.SetGetMethod(this.getMethod.MethodBuilder);
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		public delegate PropertyBuilder DefineProperty_Clr_2_0_SP1(string name,
			PropertyAttributes attributes,
			CallingConventions callingConvention,
			Type returnType,
			Type[] returnTypeRequiredCustomModifiers,
			Type[] returnTypeOptionalCustomModifiers,
			Type[] parameterTypes,
			Type[][] parameterTypeRequiredCustomModifiers,
			Type[][] parameterTypeOptionalCustomModifiers);

		private delegate PropertyBuilder DefineProperty_Clr2_0(
			string name, PropertyAttributes attributes, Type propertyType, Type[] parameters);

		#endregion
	}
}