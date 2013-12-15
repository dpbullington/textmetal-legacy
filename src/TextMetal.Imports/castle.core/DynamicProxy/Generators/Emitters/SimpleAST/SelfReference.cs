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

	[DebuggerDisplay("this")]
	public class SelfReference : Reference
	{
		#region Constructors/Destructors

		protected SelfReference()
			: base(null)
		{
		}

		#endregion

		#region Fields/Constants

		public static readonly SelfReference Self = new SelfReference();

		#endregion

		#region Methods/Operators

		public override void LoadAddressOfReference(ILGenerator gen)
		{
			throw new NotSupportedException();
		}

		public override void LoadReference(ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldarg_0);
		}

		public override void StoreReference(ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldarg_0);
		}

		#endregion
	}
}