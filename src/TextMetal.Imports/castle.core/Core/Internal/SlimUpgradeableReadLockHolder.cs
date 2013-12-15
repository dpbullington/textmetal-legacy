// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

using System.Threading;

namespace Castle.Core.Internal
{
#if !SILVERLIGHT

	internal class SlimUpgradeableReadLockHolder : IUpgradeableLockHolder
	{
		#region Constructors/Destructors

		public SlimUpgradeableReadLockHolder(ReaderWriterLockSlim locker, bool waitForLock, bool wasLockAlreadyHelf)
		{
			this.locker = locker;
			if (wasLockAlreadyHelf)
			{
				this.lockAcquired = true;
				this.wasLockAlreadyHeld = true;
				return;
			}

			if (waitForLock)
			{
				locker.EnterUpgradeableReadLock();
				this.lockAcquired = true;
				return;
			}

			this.lockAcquired = locker.TryEnterUpgradeableReadLock(0);
		}

		#endregion

		#region Fields/Constants

		private readonly ReaderWriterLockSlim locker;
		private bool lockAcquired;
		private bool wasLockAlreadyHeld;
		private SlimWriteLockHolder writerLock;

		#endregion

		#region Properties/Indexers/Events

		public bool LockAcquired
		{
			get
			{
				return this.lockAcquired;
			}
		}

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
			if (this.writerLock != null && this.writerLock.LockAcquired)
			{
				this.writerLock.Dispose();
				this.writerLock = null;
			}
			if (!this.LockAcquired)
				return;
			if (!this.wasLockAlreadyHeld)
				this.locker.ExitUpgradeableReadLock();
			this.lockAcquired = false;
		}

		public ILockHolder Upgrade()
		{
			return this.Upgrade(true);
		}

		public ILockHolder Upgrade(bool waitForLock)
		{
			if (this.locker.IsWriteLockHeld)
				return NoOpLock.Lock;

			this.writerLock = new SlimWriteLockHolder(this.locker, waitForLock);
			return this.writerLock;
		}

		#endregion
	}
#endif
}