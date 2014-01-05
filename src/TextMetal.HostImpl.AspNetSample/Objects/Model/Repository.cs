/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Text;

using TextMetal.Common.Cerealization;
using TextMetal.Common.Core;
using TextMetal.Common.Data;
using TextMetal.HostImpl.AspNetSample.Objects.Model.Tables;
using TextMetal.HostImpl.Web.Email;

namespace TextMetal.HostImpl.AspNetSample.Objects.Model
{
	public partial class Repository
	{
		#region Methods/Operators

		partial void OnPostInsertEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)emailAttachment == null)
				throw new ArgumentNullException("emailAttachment");
		}

		partial void OnPostInsertEmailMessage(IUnitOfWork unitOfWork, EmailMessage emailMessage)
		{
			bool result;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)emailMessage == null)
				throw new ArgumentNullException("emailMessage");

			if ((object)emailMessage.EmailAttachments != null)
			{
				foreach (EmailAttachment emailAttachment in emailMessage.EmailAttachments)
				{
					emailAttachment.EmailMessageId = emailMessage.EmailMessageId;
					result = this.SaveEmailAttachment(unitOfWork, emailAttachment);

					if (!result)
						throw new InvalidOperationException("bad stuff happened");
				}
			}
		}

		partial void OnPostInsertEventLog(IUnitOfWork unitOfWork, EventLog eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)eventLog == null)
				throw new ArgumentNullException("eventLog");
		}

		partial void OnPostUpdateEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)emailAttachment == null)
				throw new ArgumentNullException("emailAttachment");
		}

		partial void OnPostUpdateEmailMessage(IUnitOfWork unitOfWork, EmailMessage emailMessage)
		{
			bool result;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)emailMessage == null)
				throw new ArgumentNullException("emailMessage");

			if ((object)emailMessage.EmailAttachments != null)
			{
				foreach (EmailAttachment emailAttachment in emailMessage.EmailAttachments)
				{
					emailAttachment.EmailMessageId = emailMessage.EmailMessageId;
					result = this.SaveEmailAttachment(unitOfWork, emailAttachment);

					if (!result)
						throw new InvalidOperationException("bad stuff happened");
				}
			}
		}

		partial void OnPostUpdateEventLog(IUnitOfWork unitOfWork, EventLog eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)eventLog == null)
				throw new ArgumentNullException("eventLog");
		}

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
			Type templateResourceType;
			MessageTemplate messageTemplate;
			EmailMessage emailMessage;
			EmailAttachment emailAttachment;

			if ((object)templateResourceName == null)
				throw new ArgumentNullException("templateResourceName");

			if ((object)modelObject == null)
				throw new ArgumentNullException("modelObject");

			try
			{
				templateResourceType = typeof(Repository);
				if (!Cerealization.TryGetFromAssemblyResource<MessageTemplate>(templateResourceType, templateResourceName, out messageTemplate))
					throw new InvalidOperationException(string.Format("Unable to deserialize instance of '{0}' from the manifest resource name '{1}' in the assembly '{2}'.", typeof(MessageTemplate).FullName, templateResourceName, templateResourceType.Assembly.FullName));

				emailMessage = new EmailMessage();
				emailAttachment = new EmailAttachment( /*emailMessage*/);
				emailAttachment.FileName = "data.txt";
				emailAttachment.MimeType = "text/plain";
				emailAttachment.AttachmentBits = Encoding.UTF8.GetBytes("some sample data");

				emailMessage.EmailAttachments.Add(emailAttachment);

				new EmailHost().Host(messageTemplate, modelObject, emailMessage);

				this.SaveEmailMessage(emailMessage);

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