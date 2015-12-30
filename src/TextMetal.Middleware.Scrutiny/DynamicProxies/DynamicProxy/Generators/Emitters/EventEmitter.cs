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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;

	public class EventEmitter : IMemberEmitter
	{
		#region Constructors/Destructors

		public EventEmitter(AbstractTypeEmitter typeEmitter, string name, EventAttributes attributes, Type type)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (type == null)
				throw new ArgumentNullException("type");
			this.typeEmitter = typeEmitter;
			this.type = type;
			this.eventBuilder = typeEmitter.TypeBuilder.DefineEvent(name, attributes, type);
		}

		#endregion

		#region Fields/Constants

		private readonly EventBuilder eventBuilder;
		private readonly Type type;
		private readonly AbstractTypeEmitter typeEmitter;
		private MethodEmitter addMethod;
		private MethodEmitter removeMethod;

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
				return this.type;
			}
		}

		#endregion

		#region Methods/Operators

		public MethodEmitter CreateAddMethod(string addMethodName, MethodAttributes attributes, MethodInfo methodToOverride)
		{
			if (this.addMethod != null)
				throw new InvalidOperationException("An add method exists");

			this.addMethod = new MethodEmitter(this.typeEmitter, addMethodName, attributes, methodToOverride);
			return this.addMethod;
		}

		public MethodEmitter CreateRemoveMethod(string removeMethodName, MethodAttributes attributes,
			MethodInfo methodToOverride)
		{
			if (this.removeMethod != null)
				throw new InvalidOperationException("A remove method exists");
			this.removeMethod = new MethodEmitter(this.typeEmitter, removeMethodName, attributes, methodToOverride);
			return this.removeMethod;
		}

		public void EnsureValidCodeBlock()
		{
			this.addMethod.EnsureValidCodeBlock();
			this.removeMethod.EnsureValidCodeBlock();
		}

		public void Generate()
		{
			if (this.addMethod == null)
				throw new InvalidOperationException("Event add method was not created");
			if (this.removeMethod == null)
				throw new InvalidOperationException("Event remove method was not created");
			this.addMethod.Generate();
			this.eventBuilder.SetAddOnMethod(this.addMethod.MethodBuilder);

			this.removeMethod.Generate();
			this.eventBuilder.SetRemoveOnMethod(this.removeMethod.MethodBuilder);
		}

		#endregion
	}
}