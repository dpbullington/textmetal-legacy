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

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System;

	public class DefaultValueExpression : Expression
	{
		#region Constructors/Destructors

		public DefaultValueExpression(Type type)
		{
			this.type = type;
		}

		#endregion

		#region Fields/Constants

		private readonly Type type;

		#endregion

		#region Methods/Operators

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			// TODO: check if this can be simplified by using more of OpCodeUtil and other existing types
			if (this.IsPrimitiveOrClass(this.type))
				OpCodeUtil.EmitLoadOpCodeForDefaultValueOfType(gen, this.type);
			else if (this.type.GetTypeInfo().IsValueType || this.type.GetTypeInfo().IsGenericParameter)
			{
				// TODO: handle decimal explicitly
				var local = gen.DeclareLocal(this.type);
				gen.Emit(OpCodes.Ldloca_S, local);
				gen.Emit(OpCodes.Initobj, this.type);
				gen.Emit(OpCodes.Ldloc, local);
			}
			else if (this.type.GetTypeInfo().IsByRef)
				this.EmitByRef(gen);
			else
				throw new ProxyGenerationException("Can't emit default value for type " + this.type);
		}

		private void EmitByRef(ILGenerator gen)
		{
			var elementType = this.type.GetElementType();
			if (this.IsPrimitiveOrClass(elementType))
			{
				OpCodeUtil.EmitLoadOpCodeForDefaultValueOfType(gen, elementType);
				OpCodeUtil.EmitStoreIndirectOpCodeForType(gen, elementType);
			}
			else if (elementType.GetTypeInfo().IsGenericParameter || elementType.GetTypeInfo().IsValueType)
				gen.Emit(OpCodes.Initobj, elementType);
			else
				throw new ProxyGenerationException("Can't emit default value for reference of type " + elementType);
		}

		private bool IsPrimitiveOrClass(Type type)
		{
			if ((type.GetTypeInfo().IsPrimitive && type != typeof(IntPtr)))
				return true;
#if NETCORE
			return ((type.GetTypeInfo().IsClass || type.GetTypeInfo().IsInterface) &&
					type.IsGenericParameter == false &&
					type.IsByRef == false);
#else
			return ((type.GetTypeInfo().IsClass || type.GetTypeInfo().IsInterface) &&
			        type.GetTypeInfo().IsGenericParameter == false &&
			        type.GetTypeInfo().IsByRef == false);
#endif
		}

		#endregion
	}
}