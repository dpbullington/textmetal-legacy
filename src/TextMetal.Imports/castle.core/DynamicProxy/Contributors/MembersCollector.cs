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
using System.Linq;
using System.Reflection;

using Castle.Core.Logging;
using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Internal;

namespace Castle.DynamicProxy.Contributors
{
	using System;

	public abstract class MembersCollector
	{
		#region Constructors/Destructors

		protected MembersCollector(Type type)
		{
			this.type = type;
		}

		#endregion

		#region Fields/Constants

		private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private readonly IDictionary<EventInfo, MetaEvent> events = new Dictionary<EventInfo, MetaEvent>();
		private readonly IDictionary<MethodInfo, MetaMethod> methods = new Dictionary<MethodInfo, MetaMethod>();
		private readonly IDictionary<PropertyInfo, MetaProperty> properties = new Dictionary<PropertyInfo, MetaProperty>();

		protected readonly Type type;
		private ICollection<MethodInfo> checkedMethods = new HashSet<MethodInfo>();
		private ILogger logger = NullLogger.Instance;

		#endregion

		#region Properties/Indexers/Events

		public IEnumerable<MetaEvent> Events
		{
			get
			{
				return this.events.Values;
			}
		}

		public ILogger Logger
		{
			get
			{
				return this.logger;
			}
			set
			{
				this.logger = value;
			}
		}

		public IEnumerable<MetaMethod> Methods
		{
			get
			{
				return this.methods.Values;
			}
		}

		public IEnumerable<MetaProperty> Properties
		{
			get
			{
				return this.properties.Values;
			}
		}

		#endregion

		#region Methods/Operators

		private static bool IsInternalAndNotVisibleToDynamicProxy(MethodInfo method)
		{
			return method.IsInternal() &&
					method.DeclaringType.Assembly.IsInternalToDynamicProxy() == false;
		}

		/// <summary>
		/// Performs some basic screening and invokes the <see cref="IProxyGenerationHook" />
		/// to select methods.
		/// </summary>
		/// <param name="method"> </param>
		/// <param name="onlyVirtuals"> </param>
		/// <param name="hook"> </param>
		/// <returns> </returns>
		protected bool AcceptMethod(MethodInfo method, bool onlyVirtuals, IProxyGenerationHook hook)
		{
			// we can never intercept a sealed (final) method
			if (method.IsFinal)
			{
				this.Logger.DebugFormat("Excluded sealed method {0} on {1} because it cannot be intercepted.", method.Name,
					method.DeclaringType.FullName);
				return false;
			}

			if (IsInternalAndNotVisibleToDynamicProxy(method))
				return false;

			if (onlyVirtuals && !method.IsVirtual)
			{
				if (
#if !SILVERLIGHT
					method.DeclaringType != typeof(MarshalByRefObject) &&
#endif
						method.IsGetType() == false &&
					method.IsMemberwiseClone() == false)
				{
					this.Logger.DebugFormat("Excluded non-virtual method {0} on {1} because it cannot be intercepted.", method.Name,
						method.DeclaringType.FullName);
					hook.NonProxyableMemberNotification(this.type, method);
				}
				return false;
			}

			//can only proxy methods that are public or protected (or internals that have already been checked above)
			if ((method.IsPublic || method.IsFamily || method.IsAssembly || method.IsFamilyOrAssembly) == false)
				return false;

#if !SILVERLIGHT
			if (method.DeclaringType == typeof(MarshalByRefObject))
				return false;
#endif
			if (method.IsFinalizer())
				return false;

			return hook.ShouldInterceptMethod(this.type, method);
		}

		private void AddEvent(EventInfo @event, IProxyGenerationHook hook)
		{
			var addMethod = @event.GetAddMethod(true);
			var removeMethod = @event.GetRemoveMethod(true);
			MetaMethod adder = null;
			MetaMethod remover = null;

			if (addMethod != null)
				adder = this.AddMethod(addMethod, hook, false);

			if (removeMethod != null)
				remover = this.AddMethod(removeMethod, hook, false);

			if (adder == null && remover == null)
				return;

			this.events[@event] = new MetaEvent(@event.Name,
				@event.DeclaringType, @event.EventHandlerType, adder, remover, EventAttributes.None);
		}

		private MetaMethod AddMethod(MethodInfo method, IProxyGenerationHook hook, bool isStandalone)
		{
			if (this.checkedMethods.Contains(method))
				return null;
			this.checkedMethods.Add(method);

			if (this.methods.ContainsKey(method))
				return null;
			var methodToGenerate = this.GetMethodToGenerate(method, hook, isStandalone);
			if (methodToGenerate != null)
				this.methods[method] = methodToGenerate;

			return methodToGenerate;
		}

		private void AddProperty(PropertyInfo property, IProxyGenerationHook hook)
		{
			MetaMethod getter = null;
			MetaMethod setter = null;

			if (property.CanRead)
			{
				var getMethod = property.GetGetMethod(true);
				getter = this.AddMethod(getMethod, hook, false);
			}

			if (property.CanWrite)
			{
				var setMethod = property.GetSetMethod(true);
				setter = this.AddMethod(setMethod, hook, false);
			}

			if (setter == null && getter == null)
				return;

			var nonInheritableAttributes = property.GetNonInheritableAttributes();
			var arguments = property.GetIndexParameters();

			this.properties[property] = new MetaProperty(property.Name,
				property.PropertyType,
				property.DeclaringType,
				getter,
				setter,
				nonInheritableAttributes,
				arguments.Select(a => a.ParameterType).ToArray());
		}

		private void CollectEvents(IProxyGenerationHook hook)
		{
			var eventsFound = this.type.GetEvents(Flags);
			foreach (var @event in eventsFound)
				this.AddEvent(@event, hook);
		}

		public virtual void CollectMembersToProxy(IProxyGenerationHook hook)
		{
			if (this.checkedMethods == null) // this method was already called!
			{
				throw new InvalidOperationException(
					string.Format("Can't call 'CollectMembersToProxy' method twice. This usually signifies a bug in custom {0}.",
						typeof(ITypeContributor)));
			}
			this.CollectProperties(hook);
			this.CollectEvents(hook);
			// Methods go last, because properties and events have methods too (getters/setters add/remove)
			// and we don't want to get duplicates, so we collect property and event methods first
			// then we collect methods, and add only these that aren't there yet
			this.CollectMethods(hook);

			this.checkedMethods = null; // this is ugly, should have a boolean flag for this or something
		}

		private void CollectMethods(IProxyGenerationHook hook)
		{
			var methodsFound = MethodFinder.GetAllInstanceMethods(this.type, Flags);
			foreach (var method in methodsFound)
				this.AddMethod(method, hook, true);
		}

		private void CollectProperties(IProxyGenerationHook hook)
		{
			var propertiesFound = this.type.GetProperties(Flags);
			foreach (var property in propertiesFound)
				this.AddProperty(property, hook);
		}

		protected abstract MetaMethod GetMethodToGenerate(MethodInfo method, IProxyGenerationHook hook, bool isStandalone);

		#endregion
	}
}