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
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;

using Castle.DynamicProxy.Generators;

#if !SILVERLIGHT

namespace Castle.DynamicProxy.Serialization
{
	using System;

	/// <summary>
	/// Applied to the assemblies saved by <see cref="ModuleScope" /> in order to persist the cache data included in the persisted assembly.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	[CLSCompliant(false)]
	public class CacheMappingsAttribute : Attribute
	{
		#region Constructors/Destructors

		public CacheMappingsAttribute(byte[] serializedCacheMappings)
		{
			this.serializedCacheMappings = serializedCacheMappings;
		}

		#endregion

		#region Fields/Constants

		private static readonly ConstructorInfo constructor =
			typeof(CacheMappingsAttribute).GetConstructor(new[] { typeof(byte[]) });

		private readonly byte[] serializedCacheMappings;

		#endregion

		#region Properties/Indexers/Events

		public byte[] SerializedCacheMappings
		{
			get
			{
				return this.serializedCacheMappings;
			}
		}

		#endregion

		#region Methods/Operators

		public static void ApplyTo(AssemblyBuilder assemblyBuilder, Dictionary<CacheKey, string> mappings)
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, mappings);
				var bytes = stream.ToArray();
				var attributeBuilder = new CustomAttributeBuilder(constructor, new object[] { bytes });
				assemblyBuilder.SetCustomAttribute(attributeBuilder);
			}
		}

		public Dictionary<CacheKey, string> GetDeserializedMappings()
		{
			using (var stream = new MemoryStream(this.SerializedCacheMappings))
			{
				var formatter = new BinaryFormatter();
				return (Dictionary<CacheKey, string>)formatter.Deserialize(stream);
			}
		}

		#endregion
	}
}

#endif