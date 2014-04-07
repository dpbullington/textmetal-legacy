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
	public partial class EmailAttachment : IEmailAttachment
	{
		#region Fields/Constants

		private readonly IEmailMessage emailMessage;

		#endregion

		#region Properties/Indexers/Events

		public IEmailMessage EmailMessage
		{
			get
			{
				return this.emailMessage;
			}
		}

		#endregion

		#region Methods/Operators

		public static bool Exists(EmailAttachment emailAttachment)
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
		}

		public virtual Message[] Validate()
		{
			List<Message> messages;
			bool exists;

			messages = new List<Message>();

			if ((object)this.EmailMessageId == null)
				messages.Add(new Message("", "Email message is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.FileName))
				messages.Add(new Message("", "File name is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.MimeType))
				messages.Add(new Message("", "MIME type is required.", Severity.Error));

			if (messages.Count > 0)
				return messages.ToArray();

			exists = Exists(this);

			if (exists)
				messages.Add(new Message("", "Email attachment must be unique.", Severity.Error));

			return messages.ToArray();
		}

		#endregion
	}
}