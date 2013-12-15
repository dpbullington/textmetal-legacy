// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;

using Castle.Core;

namespace Castle.Components.DictionaryAdapter
{
	using System;

	public sealed class VirtualSite<TNode, TMember> :
		IVirtualSite<TNode>,
		IEquatable<VirtualSite<TNode, TMember>>
	{
		#region Constructors/Destructors

		public VirtualSite(IVirtualTarget<TNode, TMember> target, TMember member)
		{
			this.target = target;
			this.member = member;
		}

		#endregion

		#region Fields/Constants

		private static readonly IEqualityComparer<TMember>
			MemberComparer = EqualityComparer<TMember>.Default;

		private static readonly IEqualityComparer<IVirtualTarget<TNode, TMember>>
			TargetComparer = ReferenceEqualityComparer<IVirtualTarget<TNode, TMember>>.Instance;

		private readonly TMember member;
		private readonly IVirtualTarget<TNode, TMember> target;

		#endregion

		#region Properties/Indexers/Events

		public TMember Member
		{
			get
			{
				return this.member;
			}
		}

		public IVirtualTarget<TNode, TMember> Target
		{
			get
			{
				return this.target;
			}
		}

		#endregion

		#region Methods/Operators

		public override bool Equals(object obj)
		{
			return this.Equals(obj as VirtualSite<TNode, TMember>);
		}

		public bool Equals(VirtualSite<TNode, TMember> other)
		{
			return other != null
					&& TargetComparer.Equals(this.target, other.target)
					&& MemberComparer.Equals(this.member, other.member);
		}

		public override int GetHashCode()
		{
			return 0x72F10A3D
					+ 37 * TargetComparer.GetHashCode(this.target)
					+ 37 * MemberComparer.GetHashCode(this.member);
		}

		public void OnRealizing(TNode node)
		{
			this.target.OnRealizing(node, this.member);
		}

		#endregion
	}
}