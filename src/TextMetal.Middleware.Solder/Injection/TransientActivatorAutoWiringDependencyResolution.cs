/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// A dependency resolution implementation that uses Activator.CreateInstance(...)
	/// on the activation type each time a dependency resolution occurs and is the only
	/// implementation that allows for auto-wiring using the DependencyInjectionAttribute.
	/// </summary>
	[Obsolete("TransientActivatorAutoWiringDependencyResolution`1 should be used instead.")]
	public sealed class TransientActivatorAutoWiringDependencyResolution : DependencyResolution
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TransientActivatorAutoWiringDependencyResolution class.
		/// </summary>
		/// <param name="activatorType"> The activator type of the resolution. </param>
		public TransientActivatorAutoWiringDependencyResolution(Type activatorType)
			: base(DependencyLifetime.Transient)
		{
			if ((object)activatorType == null)
				throw new ArgumentNullException(nameof(activatorType));

			this.activatorType = activatorType;
		}

		#endregion

		#region Fields/Constants

		private readonly Type activatorType;

		#endregion

		#region Properties/Indexers/Events

		private Type ActivatorType
		{
			get
			{
				return this.activatorType;
			}
		}

		#endregion

		#region Methods/Operators

		public static object AutoWireResolve(Type activatorType, IDependencyManager dependencyManager, Type resolutionType, string selectorKey)
		{
			object[] invocationArguments;

			ConstructorInfo constructorInfo;
			ConstructorInfo[] constructorInfos;
			ParameterInfo[] parameterInfos;
			ParameterInfo parameterInfo;
			Type parameterType;

			DependencyInjectionAttribute dependencyInjectionAttribute;

			if ((object)activatorType == null)
				throw new ArgumentNullException(nameof(activatorType));

			if ((object)dependencyManager == null)
				throw new ArgumentNullException(nameof(dependencyManager));

			if ((object)resolutionType == null)
				throw new ArgumentNullException(nameof(resolutionType));

			if ((object)selectorKey == null)
				throw new ArgumentNullException(nameof(selectorKey));

			var _activatorTypeInfo = activatorType.GetTypeInfo();

			// get public, instance .ctors for activation type
			constructorInfos = activatorType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

			if ((object)constructorInfos != null)
			{
				// value types do not emit a .ctor in CIL for default constructor
				if (constructorInfos.Length == 0 &&
					_activatorTypeInfo.IsValueType)
					return Activator.CreateInstance(activatorType);
				else
				{
					// search for best fit constructor
					constructorInfo = null;

					for (int index = 0; index < constructorInfos.Length; index++)
					{
						constructorInfo = constructorInfos[index];
						dependencyInjectionAttribute = ReflectionFascade.Instance.GetOneAttribute<DependencyInjectionAttribute>(constructorInfo);

						if ((object)dependencyInjectionAttribute != null)
						{
							// explicit selector key AND matches .ctor
							if (selectorKey != string.Empty &&
								dependencyInjectionAttribute.SelectorKey == selectorKey)
								break;
						}
						else
						{
							// TODO: perform scoring to match instead
							// parameter count + has resolution (parameter type) count
						}

						constructorInfo = null;
					}

					if ((object)constructorInfo != null)
					{
						parameterInfos = constructorInfo.GetParameters();
						invocationArguments = new object[parameterInfos.Length];

						for (int index = 0; index < parameterInfos.Length; index++)
						{
							parameterInfo = parameterInfos[index];
							parameterType = parameterInfo.ParameterType;

							dependencyInjectionAttribute = ReflectionFascade.Instance.GetOneAttribute<DependencyInjectionAttribute>(parameterInfo);

							if ((object)dependencyInjectionAttribute != null)
								invocationArguments[index] = dependencyManager.ResolveDependency(parameterType, dependencyInjectionAttribute.SelectorKey, true);
							else
								invocationArguments[index] = Activator.CreateInstance(parameterType);
						}

						return Activator.CreateInstance(activatorType, invocationArguments);
					}
				}
			}

			// throw hands up!
			throw new DependencyException(string.Format("Cannot find a best-fit constructor for activator type '{0}'.", activatorType.FullName));
		}

		protected override object CoreResolve(IDependencyManager dependencyManager, Type resolutionType, string selectorKey)
		{
			if ((object)dependencyManager == null)
				throw new ArgumentNullException(nameof(dependencyManager));

			if ((object)resolutionType == null)
				throw new ArgumentNullException(nameof(resolutionType));

			if ((object)selectorKey == null)
				throw new ArgumentNullException(nameof(selectorKey));

			return AutoWireResolve(this.ActivatorType, dependencyManager, resolutionType, selectorKey);
		}

		protected override void Dispose(bool disposing)
		{
			// do nothing
		}

		#endregion
	}
}