/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.HostImpl.AspNetSample.Common;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Shared
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class SecureActionAttribute : Attribute
	{
		#region Constructors/Destructors

		public SecureActionAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private SecurityRoleEnum allowedSecurityRole;

		#endregion

		#region Properties/Indexers/Events

		public SecurityRoleEnum AllowedSecurityRole
		{
			get
			{
				return this.allowedSecurityRole;
			}
			set
			{
				this.allowedSecurityRole = value;
			}
		}

		#endregion
	}
}