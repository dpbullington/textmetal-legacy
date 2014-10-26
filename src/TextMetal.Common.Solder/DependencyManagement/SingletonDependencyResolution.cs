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
	public sealed class SingletonDependencyResolution : IDependencyResolution
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the SingletonDependencyResolution class.
		/// </summary>
		/// <param name="instance"> The singleton instance. </param>
		public SingletonDependencyResolution(object instance)
		{
			this.instance = instance;
		}

		#endregion

		#region Fields/Constants

		private readonly object instance;

		#endregion

		#region Properties/Indexers/Events

		private object Instance
		{
			get
			{
				return this.instance;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Gets an instance of an IDependencyResolution using the specified generic type.
		/// The default constructor will be used to initialize a lazy singled in the current application domain on first request.
		/// This method is never guaranteed to return a SingletonDependencyResolution instance.
		/// </summary>
		/// <typeparam name="TObject"> The target type of resolution. </typeparam>
		/// <returns> An IDependencyResolution instance. </returns>
		public static IDependencyResolution LazyConstructorOfType<TObject>()
			where TObject : new()
		{
			return DelegateDependencyResolution.FromFunc<object>(() => LazySingleton<TObject>.LazyInstance);
		}

		/// <summary>
		/// Gets an instance of SingletonDependencyResolution from the specified object instance.
		/// </summary>
		/// <typeparam name="TObject"> The target type of resolution. </typeparam>
		/// <param name="instance"> The singleton instance. </param>
		/// <returns> A SingletonDependencyResolution instance. </returns>
		public static SingletonDependencyResolution OfType<TObject>(TObject instance)
		{
			return new SingletonDependencyResolution(instance);
		}

		/// <summary>
		/// Resolves a dependency.
		/// </summary>
		/// <param name="dependencyManager"> The current in-effect dependency manager requesting this resolution. </param>
		/// <returns> An instance of an object or null. </returns>
		public object Resolve(IDependencyManager dependencyManager)
		{
			if ((object)dependencyManager == null)
				throw new ArgumentNullException("dependencyManager");

			return this.Instance;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class LazySingleton<TObject>
			where TObject : new()
		{
			#region Constructors/Destructors

			static LazySingleton()
			{
			}

			#endregion

			#region Fields/Constants

			private static readonly TObject lazyInstance = new TObject();

			#endregion

			#region Properties/Indexers/Events

			public static TObject LazyInstance
			{
				get
				{
					return lazyInstance;
				}
			}

			#endregion
		}

		#endregion
	}
}