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

	public class ThrowStatement : Statement
	{
		#region Constructors/Destructors

		public ThrowStatement(Type exceptionType, String errorMessage)
		{
			this.exceptionType = exceptionType;
			this.errorMessage = errorMessage;
		}

		#endregion

		#region Fields/Constants

		private readonly string errorMessage;
		private readonly Type exceptionType;

		#endregion

		#region Methods/Operators

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			var ci = this.exceptionType.GetConstructor(new[] { typeof(String) });
			var constRef = new ConstReference(this.errorMessage);

			var creationStmt = new NewInstanceExpression(ci, constRef.ToExpression());

			creationStmt.Emit(member, gen);

			gen.Emit(OpCodes.Throw);
		}

		#endregion
	}
}