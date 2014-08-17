/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Common.Core;
using TextMetal.Common.Data.Framework;
using TextMetal.Common.Data.Framework.LinqToSql;
using TextMetal.HostImpl.AspNetSample.Common;

namespace TextMetal.HostImpl.AspNetSample.DomainModel.Tables
{
	public partial class Organization
	{
		#region Methods/Operators

		public static bool Exists(Organization organization)
		{
			IEnumerable<IOrganization> organizations;
			IModelQuery modelQuery;

			if ((object)organization == null)
				throw new ArgumentNullException("organization");

			modelQuery = new LinqTableQuery<DomainModel.L2S.Application_Organization>(z =>
								(z.OrganizationName == organization.OrganizationName) &&
								((object)organization.OrganizationId == null || z.OrganizationId != organization.OrganizationId));

			organizations = Stuff.Get<IRepository>("").Find<IOrganization>(modelQuery);

			return organizations.Count() > 0;
		}

		partial void OnMark()
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

		partial void OnValidate(ref IEnumerable<Message> messages)
		{
			bool exists;
			List<Message> _messages;

			_messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(this.OrganizationName))
				_messages.Add(new Message("", "Organization name is required.", Severity.Error));

			if (_messages.Count > 0)
			{
				messages = _messages;
				return;
			}

			exists = Exists(this);

			if (exists)
				_messages.Add(new Message("", "Organization must be unique.", Severity.Error));

			messages = _messages;
		}

		#endregion
	}
}