/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;
using TextMetal.HostImpl.Web.Email;

namespace TextMetal.HostImpl.AspNetSample.DomainModel.Tables
{
	public partial class EmailMessage : IEmailMessage
	{
		#region Fields/Constants

		private readonly IList<IEmailAttachment> emailAttachments = new List<IEmailAttachment>();

		#endregion

		#region Properties/Indexers/Events

		public IList<IEmailAttachment> EmailAttachments
		{
			get
			{
				return this.emailAttachments;
			}
		}

		#endregion

		#region Methods/Operators

		public static bool Exists(EmailMessage emailMessage)
		{
			return false; // TODO
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

			this.Processed = this.Processed ?? false;
			this.IsBodyHtml = this.IsBodyHtml ?? false;
		}

		public virtual Message[] Validate()
		{
			List<Message> messages;
			bool exists;

			messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(this.From))
				messages.Add(new Message("", "From is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.To))
				messages.Add(new Message("", "To is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.Subject))
				messages.Add(new Message("", "From is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.Body))
				messages.Add(new Message("", "Body is required.", Severity.Error));

			if (messages.Count > 0)
				return messages.ToArray();

			exists = Exists(this);

			if (exists)
				messages.Add(new Message("", "Email message must be unique.", Severity.Error));

			return messages.ToArray();
		}

		#endregion
	}
}