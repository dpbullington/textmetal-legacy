// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Core.Builders
{
	internal class ProviderReference
	{
		#region Constructors/Destructors

		public ProviderReference(Type providerType, string providerName, string category)
		{
			if (providerType == null)
				throw new ArgumentNullException("providerType");
			if (providerName == null && providerType.GetInterface("System.Collections.IEnumerable") == null)
				throw new ArgumentNullException("providerName");

			this.providerType = providerType;
			this.providerName = providerName;
			this.category = category;
		}

		public ProviderReference(Type providerType, object[] args, string providerName, string category)
			: this(providerType, providerName, category)
		{
			this.providerArgs = args;
		}

		#endregion

		#region Fields/Constants

		private string category;
		private object[] providerArgs;
		private string providerName;
		private Type providerType;

		#endregion

		#region Properties/Indexers/Events

		public string Category
		{
			get
			{
				return this.category;
			}
		}

		public string Name
		{
			get
			{
				return this.providerName;
			}
		}

		#endregion

		#region Methods/Operators

		public IEnumerable GetInstance()
		{
			if (this.providerName != null)
			{
				MemberInfo[] members = this.providerType.GetMember(
					this.providerName,
					MemberTypes.Field | MemberTypes.Method | MemberTypes.Property,
					BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (members.Length == 0)
				{
					throw new Exception(string.Format(
						"Unable to locate {0}.{1}", this.providerType.FullName, this.providerName));
				}

				return (IEnumerable)this.GetProviderObjectFromMember(members[0]);
			}
			else
				return Reflect.Construct(this.providerType) as IEnumerable;
		}

		private object GetProviderObjectFromMember(MemberInfo member)
		{
			object providerObject = null;
			object instance = null;

			switch (member.MemberType)
			{
				case MemberTypes.Property:
					PropertyInfo providerProperty = member as PropertyInfo;
					MethodInfo getMethod = providerProperty.GetGetMethod(true);
					if (!getMethod.IsStatic)
						//instance = ProviderCache.GetInstanceOf(providerType);
						instance = Reflect.Construct(this.providerType, this.providerArgs);
					providerObject = providerProperty.GetValue(instance, null);
					break;

				case MemberTypes.Method:
					MethodInfo providerMethod = member as MethodInfo;
					if (!providerMethod.IsStatic)
						//instance = ProviderCache.GetInstanceOf(providerType);
						instance = Reflect.Construct(this.providerType, this.providerArgs);
					providerObject = providerMethod.Invoke(instance, null);
					break;

				case MemberTypes.Field:
					FieldInfo providerField = member as FieldInfo;
					if (!providerField.IsStatic)
						//instance = ProviderCache.GetInstanceOf(providerType);
						instance = Reflect.Construct(this.providerType, this.providerArgs);
					providerObject = providerField.GetValue(instance);
					break;
			}

			return providerObject;
		}

		#endregion
	}
}