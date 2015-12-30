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

namespace Castle.DynamicProxy.Internal
{
	using System;

	public abstract class InheritanceInvocation : AbstractInvocation
	{
		#region Constructors/Destructors

		protected InheritanceInvocation(
			Type targetType,
			object proxy,
			IInterceptor[] interceptors,
			MethodInfo proxiedMethod,
			object[] arguments)
			: base(proxy, interceptors, proxiedMethod, arguments)
		{
			this.targetType = targetType;
		}

		#endregion

		#region Fields/Constants

		private readonly Type targetType;

		#endregion

		#region Properties/Indexers/Events

		public override object InvocationTarget
		{
			get
			{
				return this.Proxy;
			}
		}

		public override MethodInfo MethodInvocationTarget
		{
			get
			{
				return InvocationHelper.GetMethodOnType(this.targetType, this.Method);
			}
		}

		public override Type TargetType
		{
			get
			{
				return this.targetType;
			}
		}

		#endregion

		#region Methods/Operators

		protected abstract override void InvokeMethodOnTarget();

		#endregion
	}
}