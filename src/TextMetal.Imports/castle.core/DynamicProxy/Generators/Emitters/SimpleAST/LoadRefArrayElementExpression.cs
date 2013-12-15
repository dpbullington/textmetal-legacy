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

using System.Reflection.Emit;

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	public class LoadRefArrayElementExpression : Expression
	{
		#region Constructors/Destructors

		public LoadRefArrayElementExpression(int index, Reference arrayReference)
			: this(new ConstReference(index), arrayReference)
		{
		}

		public LoadRefArrayElementExpression(ConstReference index, Reference arrayReference)
		{
			this.index = index;
			this.arrayReference = arrayReference;
		}

		#endregion

		#region Fields/Constants

		private readonly Reference arrayReference;
		private readonly ConstReference index;

		#endregion

		#region Methods/Operators

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			ArgumentsUtil.EmitLoadOwnerAndReference(this.arrayReference, gen);
			ArgumentsUtil.EmitLoadOwnerAndReference(this.index, gen);
			gen.Emit(OpCodes.Ldelem_Ref);
		}

		#endregion
	}
}