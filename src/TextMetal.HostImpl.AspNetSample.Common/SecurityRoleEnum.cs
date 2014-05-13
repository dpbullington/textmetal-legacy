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
		OrganizationOwner = 1,
		OrganizationDesigner = 2,
		OrganizationContributor = 3,
		OrganizationVisitor = 4,

		[Obsolete]
		Master = OrganizationOwner,

		Anonymous = 0xEEEEEEEE,
		Authenticated = 0xFFFFFFFF
	}
}