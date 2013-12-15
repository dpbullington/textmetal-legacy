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
using System.Reflection;

using Castle.DynamicProxy.Internal;

namespace Castle.DynamicProxy.Generators
{
	using System;

	[DebuggerDisplay("{Method}")]
	public class MetaMethod : MetaTypeElement, IEquatable<MetaMethod>
	{
		#region Constructors/Destructors

		public MetaMethod(MethodInfo method, MethodInfo methodOnTarget, bool standalone, bool proxyable, bool hasTarget)
			: base(method.DeclaringType)
		{
			this.Method = method;
			this.name = method.Name;
			this.MethodOnTarget = methodOnTarget;
			this.Standalone = standalone;
			this.Proxyable = proxyable;
			this.HasTarget = hasTarget;
			this.Attributes = this.ObtainAttributes();
		}

		#endregion

		#region Fields/Constants

		private const MethodAttributes ExplicitImplementationAttributes = MethodAttributes.Virtual |
																		MethodAttributes.Public |
																		MethodAttributes.HideBySig |
																		MethodAttributes.NewSlot |
																		MethodAttributes.Final;

		private string name;

		#endregion

		#region Properties/Indexers/Events

		public MethodAttributes Attributes
		{
			get;
			private set;
		}

		public bool HasTarget
		{
			get;
			private set;
		}

		public MethodInfo Method
		{
			get;
			private set;
		}

		public MethodInfo MethodOnTarget
		{
			get;
			private set;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public bool Proxyable
		{
			get;
			private set;
		}

		public bool Standalone
		{
			get;
			private set;
		}

		#endregion

		#region Methods/Operators

		public bool Equals(MetaMethod other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;

			if (!StringComparer.OrdinalIgnoreCase.Equals(this.name, other.name))
				return false;

			var comparer = MethodSignatureComparer.Instance;
			if (!comparer.EqualSignatureTypes(this.Method.ReturnType, other.Method.ReturnType))
				return false;

			if (!comparer.EqualGenericParameters(this.Method, other.Method))
				return false;

			if (!comparer.EqualParameters(this.Method, other.Method))
				return false;

			return true;
		}

		private MethodAttributes ObtainAttributes()
		{
			var methodInfo = this.Method;
			var attributes = MethodAttributes.Virtual;

			if (methodInfo.IsFinal || this.Method.DeclaringType.IsInterface)
				attributes |= MethodAttributes.NewSlot;

			if (methodInfo.IsPublic)
				attributes |= MethodAttributes.Public;

			if (methodInfo.IsHideBySig)
				attributes |= MethodAttributes.HideBySig;
			if (InternalsUtil.IsInternal(methodInfo) &&
				InternalsUtil.IsInternalToDynamicProxy(methodInfo.DeclaringType.Assembly))
				attributes |= MethodAttributes.Assembly;
			if (methodInfo.IsFamilyAndAssembly)
				attributes |= MethodAttributes.FamANDAssem;
			else if (methodInfo.IsFamilyOrAssembly)
				attributes |= MethodAttributes.FamORAssem;
			else if (methodInfo.IsFamily)
				attributes |= MethodAttributes.Family;

			if (this.Standalone == false)
				attributes |= MethodAttributes.SpecialName;
			return attributes;
		}

		internal override void SwitchToExplicitImplementation()
		{
			this.Attributes = ExplicitImplementationAttributes;
			if (this.Standalone == false)
				this.Attributes |= MethodAttributes.SpecialName;

			this.name = string.Format("{0}.{1}", this.Method.DeclaringType.Name, this.Method.Name);
		}

		#endregion
	}
}