/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.DomainModel.Tables
{
	public partial class TabWithCompositePrimaryKeyNoIdentity
	{		
		#region Methods/Operators
		
		public static bool Exists(TabWithCompositePrimaryKeyNoIdentity tabWithCompositePrimaryKeyNoIdentity)
		{
			IEnumerable<TabWithCompositePrimaryKeyNoIdentity> tabWithCompositePrimaryKeyNoIdentities;

			if ((object)tabWithCompositePrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("tabWithCompositePrimaryKeyNoIdentity");

			tabWithCompositePrimaryKeyNoIdentities =
				Stuff.Get<IRepository>("").FindTabWithCompositePrimaryKeyNoIdentities(
					q =>
						q.Where(
							z =>
								(z.XXX == tabWithCompositePrimaryKeyNoIdentity.XXX) && ((object)tabWithCompositePrimaryKeyNoIdentity.TabWithCompositePrimaryKeyNoIdentityId == null || z.TabWithCompositePrimaryKeyNoIdentityId != tabWithCompositePrimaryKeyNoIdentity.TabWithCompositePrimaryKeyNoIdentityId)));

			return tabWithCompositePrimaryKeyNoIdentities.Count() > 0;
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

			this.CompanyId = this.CompanyId ?? (int)Current.CompanyId;
		}

		public virtual Message[] Validate()
		{
			bool exists;
			List<Message> messages;

			messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(this.XXX))
				messages.Add(new Message("", "XXX is required.", Severity.Error));
				
			if (messages.Count > 0)
				return messages.ToArray();

			exists = Exists(this);

			if (exists)
				messages.Add(new Message("", "TabWithCompositePrimaryKeyNoIdentity must be unique.", Severity.Error));

			return messages.ToArray();
		}

		#endregion
	}
}
