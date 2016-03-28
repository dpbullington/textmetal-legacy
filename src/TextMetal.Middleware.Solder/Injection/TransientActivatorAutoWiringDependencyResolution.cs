/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
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
			ConstructorInfo constructorInfo;
			ConstructorInfo[] constructorInfos;
			ParameterInfo[] parameterInfos;
			ParameterInfo parameterInfo;
			Type parameterType;

			DependencyInjectionAttribute dependencyInjectionAttribute;
			Tuple<ConstructorInfo, ulong, Lazy<Object>> candidateConstructorTrait;
			
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
					candidateConstructorTrait = null;

					for (int constructorIndex = 0; constructorIndex < constructorInfos.Length; constructorIndex++)
					{
						List<Lazy<object>> lazyConstructorArguments;
						const ulong MAGIC = 2UL;
						string parameterSelectorKey;
						ulong score = uint.MinValue;

						constructorInfo = constructorInfos[constructorIndex];
						dependencyInjectionAttribute = ReflectionFascade.Instance.GetOneAttribute<DependencyInjectionAttribute>(constructorInfo);
						
						if ((object)dependencyInjectionAttribute != null)
						{
							// explicit selector key AND matches .ctor
							if (selectorKey != string.Empty &&
								dependencyInjectionAttribute.SelectorKey == selectorKey)
							{
								if ((object)candidateConstructorTrait != null &&
									candidateConstructorTrait.Item2 == uint.MaxValue)
									throw new DependencyException(string.Format("More than one constructor for activator type '{0}' specified the '{1}'.", activatorType.FullName, nameof(DependencyInjectionAttribute)));

								score = uint.MaxValue;
							}
						}

						parameterInfos = constructorInfo.GetParameters();
						lazyConstructorArguments = new List<Lazy<object>>(parameterInfos.Length);

						// perform scoring to match: parameter count + has resolution (parameter type) count
						score = Math.Max(score, (ulong)Math.Pow(MAGIC, parameterInfos.Length));

						for (int parameterIndex = 0; parameterIndex < parameterInfos.Length; parameterIndex++)
						{
							Lazy<object> lazyConstructorArgument;

							parameterInfo = parameterInfos[parameterIndex];
							parameterType = parameterInfo.ParameterType;

							// on parameter
							dependencyInjectionAttribute = ReflectionFascade.Instance.GetOneAttribute<DependencyInjectionAttribute>(parameterInfo);

							if ((object)dependencyInjectionAttribute != null)
							{
								score = Math.Max(score, score * MAGIC);
								parameterSelectorKey = dependencyInjectionAttribute.SelectorKey;
							}
							else
							{
								// score = score;
								parameterSelectorKey = string.Empty;
							}

							if (dependencyManager.HasTypeResolution(parameterType, parameterSelectorKey, true))
								score = Math.Max(score, score + MAGIC);

							lazyConstructorArgument = new Lazy<object>(() =>
																		{
																			// prevent modified closure bug
																			var _parameterType = parameterType;
																			var _parameterSelectorKey = parameterSelectorKey;
																			return dependencyManager.ResolveDependency(_parameterType, _parameterSelectorKey, true);
																		});

							lazyConstructorArguments.Add(lazyConstructorArgument);
						}

						if (score >= ((object)candidateConstructorTrait != null ? candidateConstructorTrait.Item2 : uint.MinValue))
						{
							Lazy<object> lazyConstructorInvokation;

							lazyConstructorInvokation = new Lazy<object>(() =>
																		{
																			// prevent modified closure bug
																			var _activatorType = activatorType;
																			var _lazyConstructorArguments = lazyConstructorArguments;
																			return Activator.CreateInstance(_activatorType, _lazyConstructorArguments.Select(l => l.Value).ToArray());
																		});

							candidateConstructorTrait = new Tuple<ConstructorInfo, ulong, Lazy<object>>(constructorInfo, score, lazyConstructorInvokation);
						}

						Console.WriteLine("Constructor for target type '{0}' with parameter types '{1}'; match score {2}.", activatorType.FullName, string.Join("|", parameterInfos.Select(pi => pi.ParameterType.FullName).ToArray()), score);

						constructorInfo = null;
					}

					if ((object)candidateConstructorTrait != null)
					{
						return candidateConstructorTrait.Item3.Value; // lazy loads a cascading chain of Lazy's...
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