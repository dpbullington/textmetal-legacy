/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Solder.DependencyManagement
{
	/// <summary>
	/// Provides the Factory Method pattern used to resolve dependencies.
	/// </summary>
	public sealed class ActivatorDependencyResolution : IDependencyResolution
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ConstructorDependencyResolution class.
		/// </summary>
		/// <param name="actualType"> The actual type of the resolution. </param>
		public ActivatorDependencyResolution(Type actualType)
			: this(actualType, false, new Type[] { })
		{
		}

		/// <summary>
		/// Initializes a new instance of the ConstructorDependencyResolution class.
		/// </summary>
		/// <param name="actualType"> The actual type of the resolution. </param>
		/// <param name="parameterTypes"> The parameter types of the constructor overload to use or null for the default constructor. </param>
		public ActivatorDependencyResolution(Type actualType, Type[] parameterTypes)
			: this(actualType, false, parameterTypes)
		{
		}

		/// <summary>
		/// Initializes a new instance of the ConstructorDependencyResolution class.
		/// </summary>
		/// <param name="actualType"> The actual type of the resolution. </param>
		/// <param name="useNonPublicDefault"> A value indicating whether to consider a default, non-public constructor. </param>
		/// <param name="parameterTypes"> The parameter types of the constructor overload to use or null for the default constructor. </param>
		private ActivatorDependencyResolution(Type actualType, bool useNonPublicDefault, Type[] parameterTypes)
		{
			if ((object)actualType == null)
				throw new ArgumentNullException("actualType");

			if ((object)parameterTypes == null)
				throw new ArgumentNullException("parameterTypes");

			this.actualType = actualType;
			this.useNonPublicDefault = useNonPublicDefault;
			this.parameterTypes = parameterTypes;
		}

		#endregion

		#region Fields/Constants

		private readonly Type actualType;
		private readonly Type[] parameterTypes;
		private readonly bool useNonPublicDefault;

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

		private bool UseNonPublicDefault
		{
			get
			{
				return this.useNonPublicDefault;
			}
		}

		#endregion

		#region Methods/Operators

		public static ActivatorDependencyResolution FromNonPublicDefault<TObject>()
		{
			Type actualType;

			actualType = typeof(TObject);

			return new ActivatorDependencyResolution(actualType, true, new Type[] { });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0>()
		{
			Type actualType;
			Type parameterType0;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2>()
		{
			Type actualType;
			Type parameterType0;
			Type parameterType1;
			Type parameterType2;

			actualType = typeof(TObject);
			parameterType0 = typeof(TParameter0);
			parameterType1 = typeof(TParameter1);
			parameterType2 = typeof(TParameter2);

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10, TParameter11>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10, parameterType11 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10, TParameter11, TParameter12>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10, parameterType11, parameterType12 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10, TParameter11, TParameter12, TParameter13>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10, parameterType11, parameterType12, parameterType13 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10, TParameter11, TParameter12, TParameter13, TParameter14>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10, parameterType11, parameterType12, parameterType13, parameterType14 });
		}

		public static ActivatorDependencyResolution OfType<TObject, TParameter0, TParameter1, TParameter2, TParameter3, TParameter4, TParameter5, TParameter6, TParameter7, TParameter8, TParameter9, TParameter10, TParameter11, TParameter12, TParameter13, TParameter14, TParameter15>()
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

			return new ActivatorDependencyResolution(actualType, new Type[] { parameterType0, parameterType1, parameterType2, parameterType3, parameterType4, parameterType5, parameterType6, parameterType7, parameterType8, parameterType9, parameterType10, parameterType11, parameterType12, parameterType13, parameterType14, parameterType15 });
		}

		/// <summary>
		/// Resolves a dependency.
		/// </summary>
		/// <param name="dependencyManager"> The current in-effect dependency manager requesting this resolution. </param>
		/// <returns> An instance of an object or null. </returns>
		public object Resolve(IDependencyManager dependencyManager)
		{
			object[] invocationArguments;
			int index = 0;

			if ((object)dependencyManager == null)
				throw new ArgumentNullException("dependencyManager");

			invocationArguments = new object[this.ParameterTypes.Length];

			foreach (Type parameterType in this.ParameterTypes)
			{
				// vCurrent: just assumes default selector
				// vNext: will use DependencyInjectionAttribute auto-wire on target type contructor for selector
				invocationArguments[index++] = dependencyManager.ResolveDependency(parameterType, "");
			}

			if (this.UseNonPublicDefault)
				return Activator.CreateInstance(this.ActualType, true);
			else
				return Activator.CreateInstance(this.ActualType, invocationArguments);
		}

		#endregion
	}
}