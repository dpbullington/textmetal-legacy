/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;
using TextMetal.HostImpl.Web.Email;

namespace TextMetal.HostImpl.AspNetSample.Objects.Model.Tables
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

		public void Mark()
		{
			DateTime now;

			now = DateTime.UtcNow;

			this.CreationTimestamp = this.CreationTimestamp ?? now;
			this.ModificationTimestamp = !this.IsNew ? now : this.CreationTimestamp;
			//this.CreationUserId = Current.UserId ?? null;
			//this.ModificationUserId = !this.IsNew ? Current.UserId : this.CreationUserId;
			this.LogicalDelete = this.LogicalDelete ?? false;

			this.Processed = this.Processed ?? false;
			this.IsBodyHtml = this.IsBodyHtml ?? false;
		}

		public virtual Message[] Validate()
		{
			List<Message> messages;

			messages = new List<Message>();

			return messages.ToArray();
		}

		#endregion
	}
}