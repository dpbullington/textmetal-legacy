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

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Castle.DynamicProxy.Generators.Emitters;

namespace Castle.DynamicProxy.Generators
{
	using System;

	public class MetaProperty : MetaTypeElement, IEquatable<MetaProperty>
	{
		#region Constructors/Destructors

		public MetaProperty(string name, Type propertyType, Type declaringType, MetaMethod getter, MetaMethod setter,
			IEnumerable<CustomAttributeBuilder> customAttributes, Type[] arguments)
			: base(declaringType)
		{
			this.name = name;
			this.type = propertyType;
			this.getter = getter;
			this.setter = setter;
			this.attributes = PropertyAttributes.None;
			this.customAttributes = customAttributes;
			this.arguments = arguments ?? Type.EmptyTypes;
		}

		#endregion

		#region Fields/Constants

		private readonly Type[] arguments;
		private readonly PropertyAttributes attributes;
		private readonly IEnumerable<CustomAttributeBuilder> customAttributes;
		private readonly MetaMethod getter;
		private readonly MetaMethod setter;
		private readonly Type type;
		private PropertyEmitter emitter;
		private string name;

		#endregion

		#region Properties/Indexers/Events

		public Type[] Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		public bool CanRead
		{
			get
			{
				return this.getter != null;
			}
		}

		public bool CanWrite
		{
			get
			{
				return this.setter != null;
			}
		}

		public PropertyEmitter Emitter
		{
			get
			{
				if (this.emitter == null)
				{
					throw new InvalidOperationException(
						"Emitter is not initialized. You have to initialize it first using 'BuildPropertyEmitter' method");
				}
				return this.emitter;
			}
		}

		public MethodInfo GetMethod
		{
			get
			{
				if (!this.CanRead)
					throw new InvalidOperationException();
				return this.getter.Method;
			}
		}

		public MetaMethod Getter
		{
			get
			{
				return this.getter;
			}
		}

		public MethodInfo SetMethod
		{
			get
			{
				if (!this.CanWrite)
					throw new InvalidOperationException();
				return this.setter.Method;
			}
		}

		public MetaMethod Setter
		{
			get
			{
				return this.setter;
			}
		}

		#endregion

		#region Methods/Operators

		public void BuildPropertyEmitter(ClassEmitter classEmitter)
		{
			if (this.emitter != null)
				throw new InvalidOperationException("Emitter is already created. It is illegal to invoke this method twice.");

			this.emitter = classEmitter.CreateProperty(this.name, this.attributes, this.type, this.arguments);
			foreach (var attribute in this.customAttributes)
				this.emitter.DefineCustomAttribute(attribute);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(MetaProperty))
				return false;
			return this.Equals((MetaProperty)obj);
		}

		public bool Equals(MetaProperty other)
		{
			if (ReferenceEquals(null, other))
				return false;

			if (ReferenceEquals(this, other))
				return true;

			if (!this.type.Equals(other.type))
				return false;

			if (!StringComparer.OrdinalIgnoreCase.Equals(this.name, other.name))
				return false;
			if (this.Arguments.Length != other.Arguments.Length)
				return false;
			for (var i = 0; i < this.Arguments.Length; i++)
			{
				if (this.Arguments[i].Equals(other.Arguments[i]) == false)
					return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((this.GetMethod != null ? this.GetMethod.GetHashCode() : 0) * 397) ^ (this.SetMethod != null ? this.SetMethod.GetHashCode() : 0);
			}
		}

		internal override void SwitchToExplicitImplementation()
		{
			this.name = string.Format("{0}.{1}", this.sourceType.Name, this.name);
			if (this.setter != null)
				this.setter.SwitchToExplicitImplementation();
			if (this.getter != null)
				this.getter.SwitchToExplicitImplementation();
		}

		#endregion
	}
}