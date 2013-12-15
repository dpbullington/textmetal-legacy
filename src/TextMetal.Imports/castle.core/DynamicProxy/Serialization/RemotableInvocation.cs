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
using System.Runtime.Serialization;

#if !SILVERLIGHT

namespace Castle.DynamicProxy.Serialization
{
	using System;
#if DOTNET40
	using System.Security;

#endif

	[Serializable]
	public class RemotableInvocation : MarshalByRefObject, IInvocation, ISerializable
	{
		private readonly IInvocation parent;

		public RemotableInvocation(IInvocation parent)
		{
			this.parent = parent;
		}

		protected RemotableInvocation(SerializationInfo info, StreamingContext context)
		{
			this.parent = (IInvocation)info.GetValue("invocation", typeof(IInvocation));
		}

		public void SetArgumentValue(int index, object value)
		{
			this.parent.SetArgumentValue(index, value);
		}

		public object GetArgumentValue(int index)
		{
			return this.parent.GetArgumentValue(index);
		}

		public Type[] GenericArguments
		{
			get
			{
				return this.parent.GenericArguments;
			}
		}

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		public void Proceed()
		{
			this.parent.Proceed();
		}

		public object Proxy
		{
			get
			{
				return this.parent.Proxy;
			}
		}

		public object InvocationTarget
		{
			get
			{
				return this.parent.InvocationTarget;
			}
		}

		public Type TargetType
		{
			get
			{
				return this.parent.TargetType;
			}
		}

		public object[] Arguments
		{
			get
			{
				return this.parent.Arguments;
			}
		}

		/// <summary>
		/// </summary>
		public MethodInfo Method
		{
			get
			{
				return this.parent.Method;
			}
		}

		public MethodInfo GetConcreteMethod()
		{
			return this.parent.GetConcreteMethod();
		}

		/// <summary>
		/// For interface proxies, this will point to the
		/// <see cref="MethodInfo" /> on the target class
		/// </summary>
		public MethodInfo MethodInvocationTarget
		{
			get
			{
				return this.parent.MethodInvocationTarget;
			}
		}

		public MethodInfo GetConcreteMethodInvocationTarget()
		{
			return this.parent.GetConcreteMethodInvocationTarget();
		}

		public object ReturnValue
		{
			get
			{
				return this.parent.ReturnValue;
			}
			set
			{
				this.parent.ReturnValue = value;
			}
		}

#if DOTNET40
		[SecurityCritical]
#endif
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.SetType(typeof(RemotableInvocation));
			info.AddValue("invocation", new RemotableInvocation(this));
		}
	}
}

#endif