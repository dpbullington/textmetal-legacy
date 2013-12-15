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

using Castle.DynamicProxy.Generators.Emitters;

namespace Castle.DynamicProxy.Generators
{
	using System;

	public class MetaEvent : MetaTypeElement, IEquatable<MetaEvent>
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MetaEvent" /> class.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="declaringType"> Type declaring the original event being overriten, or null. </param>
		/// <param name="eventDelegateType"> </param>
		/// <param name="adder"> The add method. </param>
		/// <param name="remover"> The remove method. </param>
		/// <param name="attributes"> The attributes. </param>
		public MetaEvent(string name, Type declaringType, Type eventDelegateType, MetaMethod adder, MetaMethod remover,
			EventAttributes attributes)
			: base(declaringType)
		{
			if (adder == null)
				throw new ArgumentNullException("adder");
			if (remover == null)
				throw new ArgumentNullException("remover");
			this.name = name;
			this.type = eventDelegateType;
			this.adder = adder;
			this.remover = remover;
			this.Attributes = attributes;
		}

		#endregion

		#region Fields/Constants

		private readonly MetaMethod adder;
		private readonly MetaMethod remover;
		private readonly Type type;
		private EventEmitter emitter;
		private string name;

		#endregion

		#region Properties/Indexers/Events

		public MetaMethod Adder
		{
			get
			{
				return this.adder;
			}
		}

		public EventAttributes Attributes
		{
			get;
			private set;
		}

		public EventEmitter Emitter
		{
			get
			{
				if (this.emitter != null)
					return this.emitter;

				throw new InvalidOperationException(
					"Emitter is not initialized. You have to initialize it first using 'BuildEventEmitter' method");
			}
		}

		public MetaMethod Remover
		{
			get
			{
				return this.remover;
			}
		}

		#endregion

		#region Methods/Operators

		public void BuildEventEmitter(ClassEmitter classEmitter)
		{
			if (this.emitter != null)
				throw new InvalidOperationException();
			this.emitter = classEmitter.CreateEvent(this.name, this.Attributes, this.type);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(MetaEvent))
				return false;
			return this.Equals((MetaEvent)obj);
		}

		public bool Equals(MetaEvent other)
		{
			if (ReferenceEquals(null, other))
				return false;

			if (ReferenceEquals(this, other))
				return true;

			if (!this.type.Equals(other.type))
				return false;

			if (!StringComparer.OrdinalIgnoreCase.Equals(this.name, other.name))
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = (this.adder.Method != null ? this.adder.Method.GetHashCode() : 0);
				result = (result * 397) ^ (this.remover.Method != null ? this.remover.Method.GetHashCode() : 0);
				result = (result * 397) ^ this.Attributes.GetHashCode();
				return result;
			}
		}

		internal override void SwitchToExplicitImplementation()
		{
			this.name = string.Format("{0}.{1}", this.sourceType.Name, this.name);
			this.adder.SwitchToExplicitImplementation();
			this.remover.SwitchToExplicitImplementation();
		}

		#endregion
	}
}