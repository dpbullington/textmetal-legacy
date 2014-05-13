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
	public partial class EmailMessage : IHostEmailMessage
	{
		#region Fields/Constants

		private readonly IList<IHostEmailAttachment> hostEmailAttachments = new List<IHostEmailAttachment>();

		#endregion

		#region Properties/Indexers/Events

		public IList<IHostEmailAttachment> HostEmailAttachments
		{
			get
			{
				return this.hostEmailAttachments;
			}
		}

		#endregion

		#region Methods/Operators

		public static bool Exists(EmailMessage emailMessage)
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

			this.Processed = this.Processed ?? false;
			this.IsBodyHtml = this.IsBodyHtml ?? false;
		}

		partial void OnValidate(ref IEnumerable<Message> messages)
		{
			List<Message> _messages;
			bool exists;

			_messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(this.From))
				_messages.Add(new Message("", "From is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.To))
				_messages.Add(new Message("", "To is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.Subject))
				_messages.Add(new Message("", "From is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.Body))
				_messages.Add(new Message("", "Body is required.", Severity.Error));

			if (_messages.Count > 0)
			{
				messages = _messages;
				return;
			}

			exists = Exists(this);

			if (exists)
				_messages.Add(new Message("", "Email message must be unique.", Severity.Error));

			messages = _messages;
		}

		#endregion
	}
}