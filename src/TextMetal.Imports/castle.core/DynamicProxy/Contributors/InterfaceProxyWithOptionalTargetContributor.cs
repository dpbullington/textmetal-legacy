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

using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Generators.Emitters;

namespace Castle.DynamicProxy.Contributors
{
	public class InterfaceProxyWithOptionalTargetContributor : InterfaceProxyWithoutTargetContributor
	{
		#region Constructors/Destructors

		public InterfaceProxyWithOptionalTargetContributor(INamingScope namingScope, GetTargetExpressionDelegate getTarget,
			GetTargetReferenceDelegate getTargetReference)
			: base(namingScope, getTarget)
		{
			this.getTargetReference = getTargetReference;
			this.canChangeTarget = true;
		}

		#endregion

		#region Fields/Constants

		private readonly GetTargetReferenceDelegate getTargetReference;

		#endregion

		#region Methods/Operators

		protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
			ProxyGenerationOptions options,
			OverrideMethodDelegate overrideMethod)
		{
			if (!method.Proxyable)
				return new OptionallyForwardingMethodGenerator(method, overrideMethod, this.getTargetReference);

			return base.GetMethodGenerator(method, @class, options, overrideMethod);
		}

		#endregion
	}
}