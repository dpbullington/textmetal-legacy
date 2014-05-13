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
	public partial class EmailAttachment : IHostEmailAttachment
	{
		#region Fields/Constants

		private readonly IHostEmailMessage hostEmailMessage;

		#endregion

		#region Properties/Indexers/Events

		public IHostEmailMessage HostEmailMessage
		{
			get
			{
				return this.hostEmailMessage;
			}
		}

		#endregion

		#region Methods/Operators

		public static bool Exists(EmailAttachment emailAttachment)
		{
			return false; // TODO
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
		}

		partial void OnValidate(ref IEnumerable<Message> messages)
		{
			List<Message> _messages;
			bool exists;

			_messages = new List<Message>();

			if ((object)this.EmailMessageId == null)
				_messages.Add(new Message("", "Email message is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.FileName))
				_messages.Add(new Message("", "File name is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.MimeType))
				_messages.Add(new Message("", "MIME type is required.", Severity.Error));

			if (_messages.Count > 0)
			{
				messages = _messages;
				return;
			}

			exists = Exists(this);

			if (exists)
				_messages.Add(new Message("", "Email attachment must be unique.", Severity.Error));

			messages = _messages;
		}

		#endregion
	}
}