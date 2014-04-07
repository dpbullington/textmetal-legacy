/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.DomainModel.Tables
{
	public partial class PropertyBag
	{
		#region Methods/Operators

		public static bool Exists(PropertyBag propertyBag)
		{
			IEnumerable<PropertyBag> propertyBags;

			if ((object)propertyBag == null)
				throw new ArgumentNullException("propertyBag");

			propertyBags = new List<PropertyBag>(); // nothing to check here

			return propertyBags.Count() > 0;
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
			List<Message> messages;
			bool exists;

			messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(this.PropertyKey))
				messages.Add(new Message("", "Property key is required.", Severity.Error));

			if (messages.Count > 0)
				return messages.ToArray();

			exists = Exists(this);

			if (exists)
				messages.Add(new Message("", "Property bag must be unique.", Severity.Error));

			return messages.ToArray();
		}

		#endregion
	}
}