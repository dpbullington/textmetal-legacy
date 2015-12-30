// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Castle.Core.Internal
{
	using System;

	internal sealed class InterfaceAttributeUtil
	{
		#region Constructors/Destructors

		private InterfaceAttributeUtil(Type derivedType, Type[] baseTypes)
		{
			this.types = this.CollectTypes(derivedType, baseTypes);
			this.singletons = new Dictionary<Type, Aged<object>>();
			this.results = new List<object>();
		}

		#endregion

		#region Fields/Constants

		private static readonly object
			ConflictMarker = new object();

		private readonly List<object> results;
		private readonly Dictionary<Type, Aged<object>> singletons;
		private readonly Aged<Type>[] types; // in order from most to least derived

		private int index;

		#endregion

		#region Properties/Indexers/Events

		private int CurrentAge
		{
			get
			{
				return this.types[this.index].Age;
			}
		}

		private Type CurrentType
		{
			get
			{
				return this.types[this.index].Value;
			}
		}

		private bool IsMostDerivedType
		{
			get
			{
				return this.index == 0;
			}
		}

		#endregion

		#region Methods/Operators

		public static object[] GetAttributes(Type type, bool inherit)
		{
			if (type.GetTypeInfo().IsInterface == false)

				throw new ArgumentOutOfRangeException("type");
#if NETCORE
			var attributes = type.GetTypeInfo().GetCustomAttributes(false);
#else
			var attributes = type.GetCustomAttributes(false);
#endif
			var baseTypes = type.GetInterfaces();
			if (baseTypes.Length == 0 || !inherit)
#if NETCORE
				return (object[])attributes;
			return new InterfaceAttributeUtil(type, baseTypes)
				.GetAttributes((object[])attributes);
#else
				return attributes;

			return new InterfaceAttributeUtil(type, baseTypes)
				.GetAttributes(attributes);
#endif
		}

		private static bool ShouldConsiderType(Type type)
		{
			var ns = type.Namespace;
			return ns != "Castle.Components.DictionaryAdapter"
					&& ns != "System.ComponentModel";
		}

		private void AddSingleton(object attribute, Type attributeType)
		{
			Aged<object> singleton;
			if (this.singletons.TryGetValue(attributeType, out singleton))
			{
				if (singleton.Age == this.CurrentAge)
				{
					if (singleton.Value == ConflictMarker)
						return; // already in conflict
					else
						attribute = ConflictMarker;
				}
			}

			this.singletons[attributeType] = this.MakeAged(attribute);
		}

		private void CollectSingletons()
		{
			foreach (var entry in this.singletons)
			{
				var attribute = entry.Value.Value;

				if (attribute == ConflictMarker)
					this.HandleAttributeConflict(entry.Key);
				else
					this.results.Add(attribute);
			}
		}

		private Aged<Type>[] CollectTypes(Type derivedType, Type[] baseTypes)
		{
			var ages = new Dictionary<Type, int>();
			int age;

			ages[derivedType] = 0;

			foreach (var baseType in baseTypes)
			{
				if (ShouldConsiderType(baseType))
					ages[baseType] = 1;
			}

			foreach (var baseType in baseTypes)
			{
				if (ages.ContainsKey(baseType))
				{
					foreach (var type in baseType.GetInterfaces())
					{
						if (ages.TryGetValue(type, out age))
							ages[type] = ++age;
					}
				}
			}

			return ages
				.Select(a => new Aged<Type>(a.Key, a.Value))
				.OrderBy(t => t.Age)
				.ToArray();
		}

		private object[] GetAttributes(object[] attributes)
		{
			for (this.index = this.types.Length - 1; this.index > 0; this.index--)
#if NETCORE
				this.ProcessType((object[])this.CurrentType.GetTypeInfo().GetCustomAttributes(false));
#else
				ProcessType(CurrentType.GetCustomAttributes(false));
#endif

			this.ProcessType(attributes);

			this.CollectSingletons();
			return this.results.ToArray();
		}

		private void HandleAttributeConflict(Type attributeType)
		{
			var message = string.Format
				(
					"Cannot determine inherited attributes for interface type {0}.  " +
					"Conflicting attributes of type {1} exist in the inheritance graph.", this.CurrentType.FullName,
					attributeType.FullName
				);

			throw new InvalidOperationException(message);
		}

		private Aged<T> MakeAged<T>(T value)
		{
			return new Aged<T>(value, this.CurrentAge);
		}

		private void ProcessType(object[] attributes)
		{
			foreach (var attribute in attributes)
			{
				var attributeType = attribute.GetType();
#if NETCORE
				var attributeUsage = attributeType.GetTypeInfo().GetCustomAttribute<AttributeUsageAttribute>();
#else
				var attributeUsage = attributeType.GetAttributeUsage();
#endif
				if (this.IsMostDerivedType || attributeUsage.Inherited)
				{
					if (attributeUsage.AllowMultiple)
						this.results.Add(attribute);
					else
						this.AddSingleton(attribute, attributeType);
				}
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		[DebuggerDisplay("{Value}, Age: {Age}")]
		private sealed class Aged<T>
		{
			#region Constructors/Destructors

			public Aged(T value, int age)
			{
				this.Value = value;
				this.Age = age;
			}

			#endregion

			#region Fields/Constants

			public readonly int Age;
			public readonly T Value;

			#endregion
		}

		#endregion
	}
}