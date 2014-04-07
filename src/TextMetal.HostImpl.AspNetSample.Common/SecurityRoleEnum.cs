/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.Common
{
	public enum SecurityRoleEnum : uint
	{
		None = 0,
		FamilyOwner = 1,
		FamilyDesigner = 2,
		FamilyContributor = 3,
		FamilyVisitor = 4,

		Parent = FamilyOwner,

		Child = 0xAAAAAAAA,
		Anonymous = 0xEEEEEEEE,
		Authenticated = 0xFFFFFFFF
	}
}