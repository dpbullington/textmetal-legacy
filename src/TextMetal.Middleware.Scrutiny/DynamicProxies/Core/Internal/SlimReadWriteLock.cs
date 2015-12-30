// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
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

	internal class SlimReadWriteLock : Lock
	{
		#region Fields/Constants

		private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		#endregion

		#region Properties/Indexers/Events

		public bool IsReadLockHeld
		{
			get
			{
				return this.locker.IsReadLockHeld;
			}
		}

		public bool IsUpgradeableReadLockHeld
		{
			get
			{
				return this.locker.IsUpgradeableReadLockHeld;
			}
		}

		public bool IsWriteLockHeld
		{
			get
			{
				return this.locker.IsWriteLockHeld;
			}
		}

		#endregion

		#region Methods/Operators

		public override ILockHolder ForReading()
		{
			return this.ForReading(true);
		}

		public override ILockHolder ForReading(bool waitForLock)
		{
			if (this.locker.IsReadLockHeld || this.locker.IsUpgradeableReadLockHeld || this.locker.IsWriteLockHeld)
				return NoOpLock.Lock;

			return new SlimReadLockHolder(this.locker, waitForLock);
		}

		public override IUpgradeableLockHolder ForReadingUpgradeable()
		{
			return this.ForReadingUpgradeable(true);
		}

		public override IUpgradeableLockHolder ForReadingUpgradeable(bool waitForLock)
		{
			return new SlimUpgradeableReadLockHolder(this.locker, waitForLock, this.locker.IsUpgradeableReadLockHeld || this.locker.IsWriteLockHeld);
		}

		public override ILockHolder ForWriting()
		{
			return this.ForWriting(true);
		}

		public override ILockHolder ForWriting(bool waitForLock)
		{
			if (this.locker.IsWriteLockHeld)
				return NoOpLock.Lock;

			return new SlimWriteLockHolder(this.locker, waitForLock);
		}

		#endregion
	}

#endif
}