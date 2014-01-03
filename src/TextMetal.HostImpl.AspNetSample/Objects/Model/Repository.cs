/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Core;
using TextMetal.Common.Data;
using TextMetal.HostImpl.AspNetSample.Objects.Model.Tables;
using TextMetal.HostImpl.Web.Email;

namespace TextMetal.HostImpl.AspNetSample.Objects.Model
{
	public partial class Repository
	{
		#region Methods/Operators

		partial void OnPreInsertEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)emailAttachment == null)
				throw new ArgumentNullException("emailAttachment");

			emailAttachment.Mark();
		}

		partial void OnPreInsertEmailMessage(IUnitOfWork unitOfWork, EmailMessage emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)emailMessage == null)
				throw new ArgumentNullException("emailMessage");

			emailMessage.Mark();
		}

		partial void OnPreInsertEventLog(IUnitOfWork unitOfWork, EventLog eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)eventLog == null)
				throw new ArgumentNullException("eventLog");

			eventLog.Mark();
		}

		partial void OnPreUpdateEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)emailAttachment == null)
				throw new ArgumentNullException("emailAttachment");

			emailAttachment.Mark();
		}

		partial void OnPreUpdateEmailMessage(IUnitOfWork unitOfWork, EmailMessage emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)emailMessage == null)
				throw new ArgumentNullException("emailMessage");

			emailMessage.Mark();
		}

		partial void OnPreUpdateEventLog(IUnitOfWork unitOfWork, EventLog eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)eventLog == null)
				throw new ArgumentNullException("eventLog");

			eventLog.Mark();
		}

		public bool TrySendEmailTemplate(string templateResourceName, object modelObject)
		{
			EmailMessage emailMessage;

			if ((object)templateResourceName == null)
				throw new ArgumentNullException("templateResourceName");

			if ((object)modelObject == null)
				throw new ArgumentNullException("modelObject");

			try
			{
				emailMessage = MessageTemplate.SendEmailTemplate<EmailMessage>(typeof(Repository), templateResourceName,
					modelObject, (em) => this.SaveEmailMessage(em));

				if (emailMessage == null)
					throw new InvalidOperationException("bad stuff happended");

				return true;
			}
			catch (Exception ex)
			{
				// swallow intentionally
				this.TryWriteEventLogEntry(Reflexion.GetErrors(ex, 0));
				return false;
			}
		}

		public bool TryWriteEventLogEntry(string eventText)
		{
			EventLog eventLog;
			bool result;

			if ((object)eventText == null)
				throw new ArgumentNullException("eventText");

			try
			{
				eventLog = new EventLog();
				eventLog.EventText = eventText;

				result = this.SaveEventLog(eventLog);
				return result;
			}
			catch (Exception ex)
			{
				// swallow intentionally
				return false;
			}
		}

		#endregion
	}
}