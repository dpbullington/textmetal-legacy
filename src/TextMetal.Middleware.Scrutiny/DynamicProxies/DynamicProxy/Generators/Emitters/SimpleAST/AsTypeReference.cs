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

	[DebuggerDisplay("{reference} as {type}")]
	public class AsTypeReference : Reference
	{
		#region Constructors/Destructors

		public AsTypeReference(Reference reference, Type type)
		{
			if (reference == null)
				throw new ArgumentNullException("reference");
			if (type == null)
				throw new ArgumentNullException("type");
			this.reference = reference;
			this.type = type;
			if (reference == this.OwnerReference)
				this.OwnerReference = null;
		}

		#endregion

		#region Fields/Constants

		private readonly Reference reference;
		private readonly Type type;

		#endregion

		#region Methods/Operators

		public override void LoadAddressOfReference(ILGenerator gen)
		{
			// NOTE: Or maybe throw new NotSupportedException() ?
			this.reference.LoadAddressOfReference(gen);
		}

		public override void LoadReference(ILGenerator gen)
		{
			this.reference.LoadReference(gen);
			gen.Emit(OpCodes.Isinst, this.type);
		}

		public override void StoreReference(ILGenerator gen)
		{
			// NOTE: Or maybe throw new NotSupportedException() ?
			this.reference.StoreReference(gen);
		}

		#endregion
	}
}