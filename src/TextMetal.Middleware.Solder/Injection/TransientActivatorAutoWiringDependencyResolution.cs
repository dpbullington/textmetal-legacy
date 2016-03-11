/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Linq;
using System.Reflection;

using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// A dependency resolution implementation that uses Activator.CreateInstance(...)
	/// on the target type each time a dependency resolution occurs and is the only
	/// implementation that allows for auto-wiring using the DependencyInjectionAttribute.
	/// From 'Dependency Injection in ASP.NET MVC6':
	/// Transient lifetime services are created each time they are requested. This lifetime works best for lightweight, stateless service.
	/// </summary>
	public sealed class TransientActivatorAutoWiringDependencyResolution : IDependencyResolution
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TransientActivatorAutoWiringDependencyResolution class.
		/// </summary>
		/// <param name="actualType"> The actual type of the resolution. </param>
		public TransientActivatorAutoWiringDependencyResolution(Type actualType)
			: this(actualType, new Type[] { })
		{
		}

		/// <summary>
		/// Initializes a new instance of the TransientActivatorAutoWiringDependencyResolution class.
		/// </summary>
		/// <param name="actualType"> The actual type of the resolution. </param>
		/// <param name="parameterTypes"> The parameter types of the constructor overload to use or null for the default constructor. </param>
		private TransientActivatorAutoWiringDependencyResolution(Type actualType, Type[] parameterTypes)
		{
			if ((object)actualType == null)
				throw new ArgumentNullException(nameof(actualType));

			if ((object)parameterTypes == null)
				throw new ArgumentNullException(nameof(parameterTypes));

			this.actualType = actualType;
			this.parameterTypes = parameterTypes;
		}

		#endregion

		#region Fields/Constants

		private readonly Type actualType;
		private readonly Type[] parameterTypes;

		#endregion

		#region Properties/Indexers/Events

		private Type ActualType
		{
			get
			{
				return this.actualType;
			}
		}

		private Type[] ParameterTypes
		{
			get
			{
				return this.parameterTypes;
			}
		}

		#endregion

		#region Methods/Operators

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject>()
		{
			Type actualType;

			actualType = typeof(TObject);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0>()
		{
			Type actualType;
			Type parameterType0;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;
			Type parameterType5;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);
			parameterType5 = typeof(TParameter5);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;
			Type parameterType5;
			Type parameterType6;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);
			parameterType5 = typeof(TParameter5);
			parameterType6 = typeof(TParameter6);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;
			Type parameterType5;
			Type parameterType6;
			Type parameterType7;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);
			parameterType5 = typeof(TParameter5);
			parameterType6 = typeof(TParameter6);
			parameterType7 = typeof(TParameter7);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;
			Type parameterType5;
			Type parameterType6;
			Type parameterType7;
			Type parameterType8;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);
			parameterType5 = typeof(TParameter5);
			parameterType6 = typeof(TParameter6);
			parameterType7 = typeof(TParameter7);
			parameterType8 = typeof(TParameter8);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;
			Type parameterType5;
			Type parameterType6;
			Type parameterType7;
			Type parameterType8;
			Type parameterType9;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);
			parameterType5 = typeof(TParameter5);
			parameterType6 = typeof(TParameter6);
			parameterType7 = typeof(TParameter7);
			parameterType8 = typeof(TParameter8);
			parameterType9 = typeof(TParameter9);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;
			Type parameterType5;
			Type parameterType6;
			Type parameterType7;
			Type parameterType8;
			Type parameterType9;
			Type parameterType10;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);
			parameterType5 = typeof(TParameter5);
			parameterType6 = typeof(TParameter6);
			parameterType7 = typeof(TParameter7);
			parameterType8 = typeof(TParameter8);
			parameterType9 = typeof(TParameter9);
			parameterType10 = typeof(TParameter10);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10, TParameter11>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;
			Type parameterType5;
			Type parameterType6;
			Type parameterType7;
			Type parameterType8;
			Type parameterType9;
			Type parameterType10;
			Type parameterType11;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);
			parameterType5 = typeof(TParameter5);
			parameterType6 = typeof(TParameter6);
			parameterType7 = typeof(TParameter7);
			parameterType8 = typeof(TParameter8);
			parameterType9 = typeof(TParameter9);
			parameterType10 = typeof(TParameter10);
			parameterType11 = typeof(TParameter11);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10, parameterType11 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10, TParameter11, TParameter12>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;
			Type parameterType5;
			Type parameterType6;
			Type parameterType7;
			Type parameterType8;
			Type parameterType9;
			Type parameterType10;
			Type parameterType11;
			Type parameterType12;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);
			parameterType5 = typeof(TParameter5);
			parameterType6 = typeof(TParameter6);
			parameterType7 = typeof(TParameter7);
			parameterType8 = typeof(TParameter8);
			parameterType9 = typeof(TParameter9);
			parameterType10 = typeof(TParameter10);
			parameterType11 = typeof(TParameter11);
			parameterType12 = typeof(TParameter12);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10, parameterType11, parameterType12 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10, TParameter11, TParameter12, TParameter13>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;
			Type parameterType5;
			Type parameterType6;
			Type parameterType7;
			Type parameterType8;
			Type parameterType9;
			Type parameterType10;
			Type parameterType11;
			Type parameterType12;
			Type parameterType13;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);
			parameterType5 = typeof(TParameter5);
			parameterType6 = typeof(TParameter6);
			parameterType7 = typeof(TParameter7);
			parameterType8 = typeof(TParameter8);
			parameterType9 = typeof(TParameter9);
			parameterType10 = typeof(TParameter10);
			parameterType11 = typeof(TParameter11);
			parameterType12 = typeof(TParameter12);
			parameterType13 = typeof(TParameter13);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10, parameterType11, parameterType12, parameterType13 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10, TParameter11, TParameter12, TParameter13, TParameter14>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;
			Type parameterType5;
			Type parameterType6;
			Type parameterType7;
			Type parameterType8;
			Type parameterType9;
			Type parameterType10;
			Type parameterType11;
			Type parameterType12;
			Type parameterType13;
			Type parameterType14;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);
			parameterType5 = typeof(TParameter5);
			parameterType6 = typeof(TParameter6);
			parameterType7 = typeof(TParameter7);
			parameterType8 = typeof(TParameter8);
			parameterType9 = typeof(TParameter9);
			parameterType10 = typeof(TParameter10);
			parameterType11 = typeof(TParameter11);
			parameterType12 = typeof(TParameter12);
			parameterType13 = typeof(TParameter13);
			parameterType14 = typeof(TParameter14);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10, parameterType11, parameterType12, parameterType13, parameterType14 });
		}

		public static TransientActivatorAutoWiringDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10, TParameter11, TParameter12, TParameter13, TParameter14, TParameter15>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;
			Type parameterType3;
			Type parameterType4;
			Type parameterType5;
			Type parameterType6;
			Type parameterType7;
			Type parameterType8;
			Type parameterType9;
			Type parameterType10;
			Type parameterType11;
			Type parameterType12;
			Type parameterType13;
			Type parameterType14;
			Type parameterType15;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);
			parameterType3 = typeof(TParameter3);
			parameterType4 = typeof(TParameter4);
			parameterType5 = typeof(TParameter5);
			parameterType6 = typeof(TParameter6);
			parameterType7 = typeof(TParameter7);
			parameterType8 = typeof(TParameter8);
			parameterType9 = typeof(TParameter9);
			parameterType10 = typeof(TParameter10);
			parameterType11 = typeof(TParameter11);
			parameterType12 = typeof(TParameter12);
			parameterType13 = typeof(TParameter13);
			parameterType14 = typeof(TParameter14);
			parameterType15 = typeof(TParameter15);

			return new TransientActivatorAutoWiringDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10, parameterType11, parameterType12, parameterType13, parameterType14, parameterType15 });
		}

		private object CoreResolve(IDependencyManager dependencyManager)
		{
			object[] invocationArguments;
			int index = 0;

			ConstructorInfo[] constructorInfos;
			ParameterInfo[] parameterInfos;
			ParameterInfo parameterInfo;
			DependencyInjectionAttribute dependencyInjectionAttribute;

			if ((object)dependencyManager == null)
				throw new ArgumentNullException(nameof(dependencyManager));

			var _actualTypeInfo = this.ActualType.GetTypeInfo();

			// first check DepInjAttrib
			constructorInfos = this.ActualType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

			if ((object)constructorInfos != null)
			{
				if (constructorInfos.Length == 0)
				{
					if (!_actualTypeInfo.IsValueType)
						throw new DependencyException(string.Format("Constructor lookup failed for target type '{0}' and parameter types '{1}'.", this.ActualType.FullName, string.Join("|", this.ParameterTypes.Select(pt => pt.FullName).ToArray())));

					// it is a value type so just assume there can be no parameters
					if (this.ParameterTypes.Length != 0)
						throw new DependencyException(string.Format("Constructor parameter list mismatch occured for target type '{0}' and parameter types '{1}'.", this.ActualType.FullName, string.Join("|", this.ParameterTypes.Select(pt => pt.FullName).ToArray())));

					invocationArguments = new object[this.ParameterTypes.Length];

					return Activator.CreateInstance(this.ActualType, invocationArguments);
				}

				foreach (ConstructorInfo constructorInfo in constructorInfos)
				{
					string currentSelectorKey = string.Empty;
					dependencyInjectionAttribute = ReflectionFascade.Instance.GetOneAttribute<DependencyInjectionAttribute>(constructorInfo);

					if ((object)dependencyInjectionAttribute != null)
					{
						if ((currentSelectorKey ?? string.Empty) != string.Empty &&
							dependencyInjectionAttribute.SelectorKey == currentSelectorKey)
						{
							// absolute match??
						}
					}

					if (true)
					{
						parameterInfos = constructorInfo.GetParameters();

						if (parameterInfos == null || parameterInfos.Length != this.ParameterTypes.Length)
							throw new DependencyException(string.Format("Constructor parameter list mismatch occured for target type '{0}' and parameter types '{1}'.", this.ActualType.FullName, string.Join("|", this.ParameterTypes.Select(pt => pt.FullName).ToArray())));

						invocationArguments = new object[this.ParameterTypes.Length];

						foreach (Type parameterType in this.ParameterTypes)
						{
							parameterInfo = parameterInfos[index];

							if (!parameterInfo.ParameterType.IsAssignableFrom(parameterType))
								throw new DependencyException(string.Format("Constructor parameter '{2}' type '{3}' is not assignable to dependency type '{4}' for target type '{0}' and parameter types '{1}'.", this.ActualType.FullName, string.Join("|", this.ParameterTypes.Select(pt => pt.FullName).ToArray()), parameterInfo.Name, parameterInfo.ParameterType.FullName, parameterType.FullName));

							dependencyInjectionAttribute = ReflectionFascade.Instance.GetOneAttribute<DependencyInjectionAttribute>(parameterInfo);

							/*
								BACKLOG(dpbullington@gmail.com / 2015 - 12 - 18):
								Should lookup occur using parameterType or parameterInfo.ParameterType?
							*/
							if ((object)dependencyInjectionAttribute != null)
								invocationArguments[index] = dependencyManager.ResolveDependency(parameterType, dependencyInjectionAttribute.SelectorKey, true);
							else
								invocationArguments[index] = Activator.CreateInstance(parameterType);

							index++;
						}

						return Activator.CreateInstance(this.ActualType, invocationArguments);
					}
				}
			}

			// throw hands up!
			throw new DependencyException(string.Format("Cannot construct an object to resolve for target type '{0}' and parameter types '{1}'.", this.ActualType.FullName, string.Join("|", this.ParameterTypes.Select(pt => pt.FullName).ToArray())));
		}

		public void Dispose()
		{
			// do nothing
		}

		/// <summary>
		/// Resolves a dependency.
		/// </summary>
		/// <param name="dependencyManager"> The current in-effect dependency manager requesting this resolution. </param>
		/// <returns> An instance of an object or null. </returns>
		public object Resolve(IDependencyManager dependencyManager)
		{
			if ((object)dependencyManager == null)
				throw new ArgumentNullException(nameof(dependencyManager));

			try
			{
				return this.CoreResolve(dependencyManager);
			}
			catch (DependencyException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new DependencyException(string.Format("Exception occured during activator dependency resolution for target type '{0}' and parameter types '{1}'.", this.ActualType.FullName, string.Join("|", this.ParameterTypes.Select(pt => pt.FullName).ToArray())), ex);
			}
		}

		#endregion
	}
}