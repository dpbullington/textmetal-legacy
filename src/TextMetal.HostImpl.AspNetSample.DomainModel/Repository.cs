/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Cerealization;
using TextMetal.Common.Core;
using TextMetal.Common.Data;
using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel.Tables;
using TextMetal.HostImpl.AspNetSample.DomainModel.Views;
using TextMetal.HostImpl.Web.Email;

namespace TextMetal.HostImpl.AspNetSample.DomainModel
{
	public partial class Repository
	{
		#region Methods/Operators

		public IEnumerable<IListItem<int?>> GetSecurityRoles()
		{
			List<IListItem<int?>> securityRoles;

			securityRoles = new List<IListItem<int?>>();
			securityRoles.Add(new ListItem<int?>((int?)SecurityRoleEnum.None, SecurityRoleEnum.None.ToString()));
			securityRoles.Add(new ListItem<int?>((int?)SecurityRoleEnum.OrganizationVisitor, SecurityRoleEnum.OrganizationVisitor.ToString()));
			securityRoles.Add(new ListItem<int?>((int?)SecurityRoleEnum.OrganizationContributor, SecurityRoleEnum.OrganizationContributor.ToString()));
			securityRoles.Add(new ListItem<int?>((int?)SecurityRoleEnum.OrganizationDesigner, SecurityRoleEnum.OrganizationDesigner.ToString()));
			securityRoles.Add(new ListItem<int?>((int?)SecurityRoleEnum.OrganizationOwner, SecurityRoleEnum.OrganizationOwner.ToString()));

			return securityRoles;
		}

		public bool TrySendEmailTemplate(string templateResourceName, object modelObject)
		{
			Type templateResourceType;
			MessageTemplate messageTemplate;
			IEmailMessage emailMessage;
			IEmailAttachment emailAttachment;

			if ((object)templateResourceName == null)
				throw new ArgumentNullException("templateResourceName");

			if ((object)modelObject == null)
				throw new ArgumentNullException("modelObject");

			try
			{
				templateResourceType = typeof(Repository);
				if (!Cerealization.TryGetFromAssemblyResource<MessageTemplate>(templateResourceType, templateResourceName, out messageTemplate))
					throw new InvalidOperationException(string.Format("Unable to deserialize instance of '{0}' from the manifest resource name '{1}' in the assembly '{2}'.", typeof(MessageTemplate).FullName, templateResourceName, templateResourceType.Assembly.FullName));

				emailMessage = this.CreateModel<IEmailMessage>();
				/*emailAttachment = new EmailAttachment();
				emailAttachment.FileName = "data.txt";
				emailAttachment.MimeType = "text/plain";
				emailAttachment.AttachmentBits = Encoding.UTF8.GetBytes("some sample data");

				emailMessage.EmailAttachments.Add(emailAttachment);*/

				try
				{
					new EmailHost().Host(true, messageTemplate, modelObject, emailMessage);
				}
				finally
				{
					this.Save<IEmailMessage>(emailMessage);
				}

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
			IEventLog eventLog;
			bool result;

			if ((object)eventText == null)
				throw new ArgumentNullException("eventText");

			try
			{
				eventLog = this.CreateModel<IEventLog>();
				eventLog.EventText = eventText;

				result = this.Save<IEventLog>(eventLog);
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