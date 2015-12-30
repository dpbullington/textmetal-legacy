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
using System.Reflection.Emit;

using Castle.Core.Internal;

namespace Castle.DynamicProxy
{
	using System;
#if FEATURE_SERIALIZATION
	using System.Runtime.Serialization;
#endif

#if DOTNET40
	using System.Security;
#endif

#if FEATURE_SERIALIZATION
	[Serializable]
#endif

	public class ProxyGenerationOptions
#if FEATURE_SERIALIZATION
		: ISerializable
#endif
	{
		public static readonly ProxyGenerationOptions Default = new ProxyGenerationOptions();

		private List<object> mixins;
		internal readonly IList<Attribute> attributesToAddToGeneratedTypes = new List<Attribute>();
		private readonly IList<CustomAttributeBuilder> additionalAttributes = new List<CustomAttributeBuilder>();

#if FEATURE_SERIALIZATION
		[NonSerialized]
#endif
		private MixinData mixinData; // this is calculated dynamically on proxy type creation

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyGenerationOptions" /> class.
		/// </summary>
		/// <param name="hook"> The hook. </param>
		public ProxyGenerationOptions(IProxyGenerationHook hook)
		{
			this.BaseTypeForInterfaceProxy = typeof(object);
			this.Hook = hook;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyGenerationOptions" /> class.
		/// </summary>
		public ProxyGenerationOptions()
			: this(new AllMethodsHook())
		{
		}

#if FEATURE_SERIALIZATION
		private ProxyGenerationOptions(SerializationInfo info, StreamingContext context)
		{
			Hook = (IProxyGenerationHook)info.GetValue("hook", typeof(IProxyGenerationHook));
			Selector = (IInterceptorSelector)info.GetValue("selector", typeof(IInterceptorSelector));
			mixins = (List<object>)info.GetValue("mixins", typeof(List<object>));
			BaseTypeForInterfaceProxy = Type.GetType(info.GetString("baseTypeForInterfaceProxy.AssemblyQualifiedName"));
		}
#endif

		public void Initialize()
		{
			if (this.mixinData == null)
			{
				try
				{
					this.mixinData = new MixinData(this.mixins);
				}
				catch (ArgumentException ex)
				{
					throw new InvalidMixinConfigurationException(
						"There is a problem with the mixins added to this ProxyGenerationOptions: " + ex.Message, ex);
				}
			}
		}

#if FEATURE_SERIALIZATION
#if DOTNET40
		[SecurityCritical]
#endif
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("hook", Hook);
			info.AddValue("selector", Selector);
			info.AddValue("mixins", mixins);
			info.AddValue("baseTypeForInterfaceProxy.AssemblyQualifiedName", BaseTypeForInterfaceProxy.AssemblyQualifiedName);
		}
#endif

		public IProxyGenerationHook Hook
		{
			get;
			set;
		}

		public IInterceptorSelector Selector
		{
			get;
			set;
		}

		public Type BaseTypeForInterfaceProxy
		{
			get;
			set;
		}

		[Obsolete(
			"This property is obsolete and will be removed in future versions. Use AdditionalAttributes property instead. " +
			"You can use AttributeUtil class to simplify creating CustomAttributeBuilder instances for common cases.")]
		public IList<Attribute> AttributesToAddToGeneratedTypes
		{
			get
			{
				return this.attributesToAddToGeneratedTypes;
			}
		}

		public IList<CustomAttributeBuilder> AdditionalAttributes
		{
			get
			{
				return this.additionalAttributes;
			}
		}

		public MixinData MixinData
		{
			get
			{
				if (this.mixinData == null)
					throw new InvalidOperationException("Call Initialize before accessing the MixinData property.");
				return this.mixinData;
			}
		}

		public void AddMixinInstance(object instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			if (this.mixins == null)
				this.mixins = new List<object>();

			this.mixins.Add(instance);
			this.mixinData = null;
		}

		public object[] MixinsAsArray()
		{
			if (this.mixins == null)
				return new object[0];

			return this.mixins.ToArray();
		}

		public bool HasMixins
		{
			get
			{
				return this.mixins != null && this.mixins.Count != 0;
			}
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
				return true;

			var proxyGenerationOptions = obj as ProxyGenerationOptions;
			if (ReferenceEquals(proxyGenerationOptions, null))
				return false;

			// ensure initialization before accessing MixinData
			this.Initialize();
			proxyGenerationOptions.Initialize();

			if (!Equals(this.Hook, proxyGenerationOptions.Hook))
				return false;
			if (!Equals(this.Selector == null, proxyGenerationOptions.Selector == null))
				return false;
			if (!Equals(this.MixinData, proxyGenerationOptions.MixinData))
				return false;
			if (!Equals(this.BaseTypeForInterfaceProxy, proxyGenerationOptions.BaseTypeForInterfaceProxy))
				return false;
			if (!CollectionExtensions.AreEquivalent(this.AdditionalAttributes, proxyGenerationOptions.AdditionalAttributes))
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			// ensure initialization before accessing MixinData
			this.Initialize();

			var result = this.Hook != null ? this.Hook.GetType().GetHashCode() : 0;
			result = 29 * result + (this.Selector != null ? 1 : 0);
			result = 29 * result + this.MixinData.GetHashCode();
			result = 29 * result + (this.BaseTypeForInterfaceProxy != null ? this.BaseTypeForInterfaceProxy.GetHashCode() : 0);
			result = 29 * result + CollectionExtensions.GetContentsHashCode(this.AdditionalAttributes);
			return result;
		}
	}
}