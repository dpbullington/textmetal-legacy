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
	public partial class Organization
	{
		#region Methods/Operators

		public static bool Exists(Organization organization)
		{
			IEnumerable<Organization> organizations;

			if ((object)organization == null)
				throw new ArgumentNullException("organization");

			organizations =
				Stuff.Get<IRepository>("").FindOrganizations(
					q =>
						q.Where(
							z =>
								(z.OrganizationName == organization.OrganizationName) && ((object)organization.OrganizationId == null || z.OrganizationId != organization.OrganizationId)));

			return organizations.Count() > 0;
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

			//this.OrganizationId = this.OrganizationId ?? (int)Current.OrganizationId;
		}

		public virtual Message[] Validate()
		{
			bool exists;
			List<Message> messages;

			messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(this.OrganizationName))
				messages.Add(new Message("", "Organization name is required.", Severity.Error));

			if (messages.Count > 0)
				return messages.ToArray();

			exists = Exists(this);

			if (exists)
				messages.Add(new Message("", "Organization must be unique.", Severity.Error));

			return messages.ToArray();
		}

		#endregion
	}
}