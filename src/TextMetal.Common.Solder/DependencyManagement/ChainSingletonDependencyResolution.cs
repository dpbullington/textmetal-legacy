/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Solder.DependencyManagement
{
	/// <summary>
	/// Provides the Factory Method pattern used to resolve dependencies.
	/// This implementation allows only a single instance to be created, cached, and reused.
	/// The singleton instance is the result of a chained IDependencyResolution execution once.
	/// </summary>
	public sealed class ChainSingletonDependencyResolution : IDependencyResolution
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ChainSingletonDependencyResolution class.
		/// </summary>
		/// <param name="chainedDependencyResolution"> The singleton instance. </param>
		public ChainSingletonDependencyResolution(IDependencyResolution chainedDependencyResolution)
		{
			if ((object)chainedDependencyResolution == null)
				throw new ArgumentNullException("chainedDependencyResolution");

			this.chainedDependencyResolution = chainedDependencyResolution;
		}

		#endregion

		#region Fields/Constants

		private readonly IDependencyResolution chainedDependencyResolution;
		private bool frozen;
		private object instance;

		#endregion

		#region Properties/Indexers/Events

		private IDependencyResolution ChainedDependencyResolution
		{
			get
			{
				return this.chainedDependencyResolution;
			}
		}

		private bool Frozen
		{
			get
			{
				return this.frozen;
			}
			set
			{
				this.frozen = value;
			}
		}

		private object Instance
		{
			get
			{
				return this.instance;
			}
			set
			{
				this.instance = value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Resolves a dependency.
		/// </summary>
		/// <param name="dependencyManager"> The current in-effect dependency manager requesting this resolution. </param>
		/// <returns> An instance of an object or null. </returns>
		public object Resolve(IDependencyManager dependencyManager)
		{
			if ((object)dependencyManager == null)
				throw new ArgumentNullException("dependencyManager");

			if(this.Frozen)
				return this.Instance;

			try
			{
				this.Instance = this.ChainedDependencyResolution.Resolve(dependencyManager);
				return this.Instance;
			}
			finally 
			{
				this.Frozen = true;
			}
		}

		#endregion
	}
}