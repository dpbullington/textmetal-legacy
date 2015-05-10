/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Data.Impl
{
	/// <summary>
	/// Used as a context object for a unit of work. Allows multiple contexts to be associated to a single transaction for differing actual types. An exception is throw if duplicate context actual types are registered. When disposed, all underlying contexts will also be disposed.
	/// </summary>
	/// <typeparam name="TDisposableContext"> The base type (not actual type) of the underlying context. </typeparam>
	public sealed class MulticastDisposableContext<TDisposableContext> : IDisposable
		where TDisposableContext : class, IDisposable
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the MulticastDisposableContext`1 class.
		/// </summary>
		public MulticastDisposableContext()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary<Type, TDisposableContext> contexts = new Dictionary<Type, TDisposableContext>();
		private bool disposed;

		#endregion

		#region Properties/Indexers/Events

		private IDictionary<Type, TDisposableContext> Contexts
		{
			get
			{
				return this.contexts;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current instance has been disposed.
		/// </summary>
		public bool Disposed
		{
			get
			{
				return this.disposed;
			}
			private set
			{
				this.disposed = value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Unsets (or unregisters) the specified actual context type.
		/// </summary>
		/// <param name="contextActualType"> The actual context type requested. </param>
		public void ClearContext(Type contextActualType)
		{
			if (this.Disposed)
				throw new ObjectDisposedException(typeof(MulticastDisposableContext<TDisposableContext>).FullName);

			if ((object)contextActualType == null)
				throw new ArgumentNullException("contextActualType");

			if (this.Contexts.ContainsKey(contextActualType))
				this.Contexts.Remove(contextActualType);
		}

		/// <summary>
		/// Disposes of the inner contexts. Once disposed, the instance cannot be reused.
		/// </summary>
		public void Dispose()
		{
			if (this.Disposed)
				return;

			try
			{
				foreach (TDisposableContext context in this.Contexts.Values)
					context.Dispose();
			}
			finally
			{
				this.Disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Gets the single context of the specified actual context type. An exception is thrown if the requested actual type has not previously been registered.
		/// </summary>
		/// <param name="contextActualType"> The actual context type requested. </param>
		/// <returns> An instance of an actual context type. </returns>
		public TDisposableContext GetContext(Type contextActualType)
		{
			TDisposableContext contextActualInstance;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(MulticastDisposableContext<TDisposableContext>).FullName);

			if ((object)contextActualType == null)
				throw new ArgumentNullException("contextActualType");

			if (!this.Contexts.ContainsKey(contextActualType))
				throw new InvalidOperationException(string.Format("The actual context type '{0}' is not yet registered on multicast context type '{1}'.", contextActualType.FullName, typeof(MulticastDisposableContext<TDisposableContext>).FullName));

			contextActualInstance = this.Contexts[contextActualType];

			return contextActualInstance;
		}

		/// <summary>
		/// Gets a value indicating whether a context of the specified actual context type has been previously registered.
		/// </summary>
		/// <param name="contextActualType"> The actual context type requested. </param>
		/// <returns> A value indicating whether a context of the specified actual context type has been previously registered. </returns>
		public bool HasContext(Type contextActualType)
		{
			if (this.Disposed)
				throw new ObjectDisposedException(typeof(MulticastDisposableContext<TDisposableContext>).FullName);

			if ((object)contextActualType == null)
				throw new ArgumentNullException("contextActualType");

			return this.Contexts.ContainsKey(contextActualType);
		}

		/// <summary>
		/// Sets (or registers) a single context instance of the specified actual context type. An exception is thrown if the requested actual type has already previously been registered.
		/// </summary>
		/// <param name="contextActualType"> The actual context type requested. </param>
		/// <param name="contextActualInstance"> The actual context instance to register. </param>
		public void SetContext(Type contextActualType, TDisposableContext contextActualInstance)
		{
			if (this.Disposed)
				throw new ObjectDisposedException(typeof(MulticastDisposableContext<TDisposableContext>).FullName);

			if ((object)contextActualType == null)
				throw new ArgumentNullException("contextActualType");

			if ((object)contextActualInstance == null)
				throw new ArgumentNullException("contextActualInstance");

			if (this.Contexts.ContainsKey(contextActualType))
				throw new InvalidOperationException(string.Format("The actual context type '{0}' is already registered on multicast context type '{1}'.", contextActualType.FullName, typeof(MulticastDisposableContext<TDisposableContext>).FullName));

			this.Contexts.Add(contextActualType, contextActualInstance);
		}

		#endregion
	}
}