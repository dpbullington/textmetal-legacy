/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

using TextMetal.Middleware.Solder.Injection;

namespace TextMetal.Messaging.Core
{
	public abstract class IntegrationComponent : /*MarshalByRefObject,*/ IIntegrationComponent
	{
		#region Constructors/Destructors

		protected IntegrationComponent()
			: this(Guid.NewGuid())
		{
		}

		internal IntegrationComponent(Guid runTimeId)
		{
			this.runTimeId = runTimeId;
		}

		#endregion

		#region Fields/Constants

		private readonly Guid designTimeId = Guid.Empty;
		private readonly Guid runTimeId;
		private readonly object syncLock = new object();
		private bool isDisposed;
		private bool isInitialized;
		private bool isMutable = true;

		#endregion

		#region Properties/Indexers/Events

		public Guid ComponentClassId
		{
			get
			{
				return this.GetType().GetTypeInfo().GUID;
			}
		}

		public Guid DesignTimeId
		{
			get
			{
				return this.designTimeId;
			}
		}

		protected IIntegrationFactory IntegrationFactory
		{
			get
			{
				return AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IIntegrationFactory>(string.Empty, false);
			}
		}

		public bool IsTerminated
		{
			get
			{
				return this.IsDisposed;
			}
		}

		public Guid RunTimeId
		{
			get
			{
				return this.runTimeId;
			}
		}

		protected object SyncLock
		{
			get
			{
				return this.syncLock;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
			private set
			{
				this.isDisposed = value;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return this.isInitialized;
			}
			private set
			{
				this.isInitialized = value;
			}
		}

		public bool IsMutable
		{
			get
			{
				return this.isMutable;
			}
			private set
			{
				this.isMutable = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected void AssertMutable()
		{
			if (!this.IsMutable)
				throw new InvalidOperationException("Component is immutable.");
		}

		protected void AssertReady()
		{
			if (!this.IsInitialized)
				throw new InvalidOperationException("Component is not ready.");

			if (this.IsTerminated)
				throw new InvalidOperationException("Component is not ready.");
		}

		protected virtual void CoreInitialize()
		{
		}

		protected virtual void CoreTerminate()
		{
		}

		public void Dispose()
		{
			if (this.IsDisposed)
				return;

			try
			{
				this.Terminate(); // pass-thru
			}
			finally
			{
				this.IsDisposed = true;
				GC.SuppressFinalize(this);
			}
		}

		public void Freeze()
		{
			this.AssertMutable();

			this.IsMutable = false;
		}

		public void Initialize()
		{
			try
			{
				this.CoreInitialize();
			}
			finally
			{
				this.IsInitialized = true;
			}
		}

		public void Terminate()
		{
			if (this.IsTerminated)
				return;

			this.CoreTerminate();
		}

		protected void WriteLogSynchronized(string format, params object[] args)
		{
			//lock (Console.Out)
			Console.WriteLine(format, args);
		}

		#endregion
	}
}