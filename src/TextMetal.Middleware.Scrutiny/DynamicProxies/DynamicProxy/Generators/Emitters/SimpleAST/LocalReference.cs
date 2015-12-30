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

using System.Diagnostics;
using System.Reflection.Emit;

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System;

	[DebuggerDisplay("local {Type}")]
	public class LocalReference : TypeReference
	{
		#region Constructors/Destructors

		public LocalReference(Type type)
			: base(type)
		{
		}

		#endregion

		#region Fields/Constants

		private LocalBuilder localbuilder;

		#endregion

		#region Methods/Operators

		public override void Generate(ILGenerator gen)
		{
			this.localbuilder = gen.DeclareLocal(this.Type);
		}

		public override void LoadAddressOfReference(ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldloca, this.localbuilder);
		}

		public override void LoadReference(ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldloc, this.localbuilder);
		}

		public override void StoreReference(ILGenerator gen)
		{
			gen.Emit(OpCodes.Stloc, this.localbuilder);
		}

		#endregion
	}
}