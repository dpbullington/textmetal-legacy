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
	internal class MonitorLockHolder : ILockHolder
	{
		#region Constructors/Destructors

		public MonitorLockHolder(object locker, bool waitForLock)
		{
			this.locker = locker;
			if (waitForLock)
			{
				Monitor.Enter(locker);
				this.lockAcquired = true;
				return;
			}

			this.lockAcquired = Monitor.TryEnter(locker, 0);
		}

		#endregion

		#region Fields/Constants

		private readonly object locker;
		private bool lockAcquired;

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
			if (!this.LockAcquired)
				return;
			Monitor.Exit(this.locker);
			this.lockAcquired = false;
		}

		#endregion
	}
}