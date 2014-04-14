/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Common.Core;
using TextMetal.HostImpl.AspNetSample.Common;

namespace TextMetal.HostImpl.AspNetSample.DomainModel.Tables
{
	public partial class SecurityRole
	{		
		#region Methods/Operators
		
		public static bool Exists(SecurityRole securityRole)
		{
			IEnumerable<SecurityRole> securityRoles;

			if ((object)securityRole == null)
				throw new ArgumentNullException("securityRole");

			securityRoles =
				Stuff.Get<IRepository>("").FindSecurityRoles(
					q =>
						q.Where(
							z =>
								(z.SecurityRoleName == securityRole.SecurityRoleName) && ((object)securityRole.SecurityRoleId == null || z.SecurityRoleId != securityRole.SecurityRoleId)));

			return securityRoles.Count() > 0;
		}

		public void Mark()
		{
			DateTime now;

			now = DateTime.UtcNow;

			this.CreationTimestamp = this.CreationTimestamp ?? now;
			this.ModificationTimestamp = !this.IsNew ? now : this.CreationTimestamp;
			this.CreationUserId = ((this.IsNew ? Current.UserId : this.CreationUserId) ?? this.CreationUserId) ?? User.SYSTEM_USER_ID;
			this.ModificationUserId = ((!this.IsNew ? Current.UserId : this.CreationUserId) ?? this.ModificationUserId) ?? User.SYSTEM_USER_ID;
			this.LogicalDelete = this.LogicalDelete ?? false;
		}

		public virtual Message[] Validate()
		{
			bool exists;
			List<Message> messages;

			messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(this.SecurityRoleName))
				messages.Add(new Message("", "Security role name is required.", Severity.Error));
				
			if (messages.Count > 0)
				return messages.ToArray();

			exists = Exists(this);

			if (exists)
				messages.Add(new Message("", "Security role must be unique.", Severity.Error));

			return messages.ToArray();
		}

		#endregion
	}
}
