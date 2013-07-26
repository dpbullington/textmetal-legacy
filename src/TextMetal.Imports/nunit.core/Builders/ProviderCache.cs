// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Core.Builders
{
	internal class ProviderCache
	{
		#region Fields/Constants

		private static IDictionary instances = new Hashtable();

		#endregion

		#region Methods/Operators

		public static void Clear()
		{
			foreach (object key in instances.Keys)
			{
				IDisposable provider = instances[key] as IDisposable;
				if (provider != null)
					provider.Dispose();
			}

			instances.Clear();
		}

		public static object GetInstanceOf(Type providerType)
		{
			CacheEntry entry = new CacheEntry(providerType);

			object instance = instances[entry];
			return instance == null
				       ? instances[entry] = Reflect.Construct(providerType)
				       : instance;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class CacheEntry
		{
			#region Constructors/Destructors

			public CacheEntry(Type providerType)
			{
				this.providerType = providerType;
			}

			#endregion

			#region Fields/Constants

			private Type providerType;

			#endregion

			#region Methods/Operators

			public override bool Equals(object obj)
			{
				CacheEntry other = obj as CacheEntry;
				if (other == null)
					return false;

				return this.providerType == other.providerType;
			}

			public override int GetHashCode()
			{
				return this.providerType.GetHashCode();
			}

			#endregion
		}

		#endregion
	}
}