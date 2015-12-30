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

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System;

	public class MethodTokenExpression : Expression
	{
		#region Constructors/Destructors

		public MethodTokenExpression(MethodInfo method)
		{
			this.method = method;
			this.declaringType = method.DeclaringType;
		}

		#endregion

		#region Fields/Constants

		private readonly Type declaringType;
		private readonly MethodInfo method;

		#endregion

		#region Methods/Operators

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldtoken, this.method);
			if (this.declaringType == null)
				throw new GeneratorException("declaringType can't be null for this situation");
			gen.Emit(OpCodes.Ldtoken, this.declaringType);

			var minfo = MethodBaseMethods.GetMethodFromHandle2;
			gen.Emit(OpCodes.Call, minfo);
			gen.Emit(OpCodes.Castclass, typeof(MethodInfo));
		}

		#endregion
	}
}