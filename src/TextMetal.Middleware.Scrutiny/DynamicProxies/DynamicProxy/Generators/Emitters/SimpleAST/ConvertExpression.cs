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

	public class ConvertExpression : Expression
	{
		#region Constructors/Destructors

		public ConvertExpression(Type targetType, Expression right)
			: this(targetType, typeof(object), right)
		{
		}

		public ConvertExpression(Type targetType, Type fromType, Expression right)
		{
			this.target = targetType;
			this.fromType = fromType;
			this.right = right;
		}

		#endregion

		#region Fields/Constants

		private readonly Expression right;
		private Type fromType;
		private Type target;

		#endregion

		#region Methods/Operators

		private static void EmitCastIfNeeded(Type from, Type target, ILGenerator gen)
		{
			if (target.IsGenericParameter)
				gen.Emit(OpCodes.Unbox_Any, target);
			else if (from.GetTypeInfo().IsGenericParameter)
				gen.Emit(OpCodes.Box, from);
#if NETCORE
			else if (target.GetTypeInfo().IsGenericType && target != from)
#else
			else if (target.IsGenericType && target != from)
#endif
				gen.Emit(OpCodes.Castclass, target);
#if NETCORE
			else if (target.GetTypeInfo().IsSubclassOf(from))
#else
			else if (target.IsSubclassOf(from))
#endif
				gen.Emit(OpCodes.Castclass, target);
		}

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			this.right.Emit(member, gen);

			if (this.fromType == this.target)
				return;

			if (this.fromType.GetTypeInfo().IsByRef)
				this.fromType = this.fromType.GetElementType();

			if (this.target.IsByRef)
				this.target = this.target.GetElementType();
#if NETCORE
			if (this.target.GetTypeInfo().IsValueType)
			{
				if (this.fromType.GetTypeInfo().IsValueType)
#else
			if (target.IsValueType)
			{
				if (fromType.GetTypeInfo().IsValueType)
#endif
					throw new NotImplementedException("Cannot convert between distinct value types");
				else
				{
					// Unbox conversion
					// Assumes fromType is a boxed value
					// if we can, we emit a box and ldind, otherwise, we will use unbox.any
					if (LdindOpCodesDictionary.Instance[this.target] != LdindOpCodesDictionary.EmptyOpCode)
					{
						gen.Emit(OpCodes.Unbox, this.target);
						OpCodeUtil.EmitLoadIndirectOpCodeForType(gen, this.target);
					}
					else
						gen.Emit(OpCodes.Unbox_Any, this.target);
				}
			}
			else
			{
				if (this.fromType.GetTypeInfo().IsValueType)

				{
					// Box conversion
					gen.Emit(OpCodes.Box, this.fromType);
					EmitCastIfNeeded(typeof(object), this.target, gen);
				}
				else
				{
					// Possible down-cast
					EmitCastIfNeeded(this.fromType, this.target, gen);
				}
			}
		}

		#endregion
	}
}