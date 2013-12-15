// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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


#if !SILVERLIGHT && !MONO

namespace Castle.Core.Internal
{
	using System;

	internal sealed class WeakKey : WeakReference
	{
		#region Constructors/Destructors

		public WeakKey(object target, int hashCode)
			: base(target)
		{
			this.hashCode = hashCode;
		}

		#endregion

		#region Fields/Constants

		private readonly int hashCode;

		#endregion

		#region Properties/Indexers/Events

		public override object Target
		{
			get
			{
				return base.Target;
			}
			set
			{
				throw new NotSupportedException("Dictionary keys are read-only.");
			}
		}

		#endregion

		#region Methods/Operators

		public override bool Equals(object other)
		{
			return WeakKeyComparer<object>.Default.Equals(this, other);
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		#endregion
	}
}

#endif