/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Net.Mail;
using System.Xml;

using TextMetal.Common.Core;
using TextMetal.Common.Core.StringTokens;
using TextMetal.Common.Xml;
using TextMetal.Framework.Core;
using TextMetal.Framework.HostingModel;
using TextMetal.Framework.InputOutputModel;
using TextMetal.Framework.TemplateModel;

namespace TextMetal.HostImpl.Web.Email
{
	public sealed class EmailHost : IEmailHost
	{
		#region Constructors/Destructors

		public EmailHost()
		{
		}

		#endregion

		#region Methods/Operators

		public void Host(bool strictMatching, MessageTemplate messageTemplate, object modelObject, IEmailMessage emailMessage)
		{
			SmtpClient smtpClient;
			string[] addresses;

			if ((object)messageTemplate == null)
				throw new ArgumentNullException("messageTemplate");

			if ((object)emailMessage == null)
				throw new ArgumentNullException("emailMessage");

			if ((object)modelObject == null)
				throw new ArgumentNullException("modelObject");

			// run templating over message template and apply to email maessage
			this.ResolveApply(strictMatching, messageTemplate, emailMessage, modelObject);

			// no longer hardcoded, uses standard config file
			smtpClient = new SmtpClient();

			using (MailMessage mailMessage = new MailMessage())
			{
				if (!DataType.IsNullOrWhiteSpace(emailMessage.Subject))
					mailMessage.Subject = emailMessage.Subject;

				if (!DataType.IsNullOrWhiteSpace(emailMessage.Body))
					mailMessage.Body = emailMessage.Body;

				if (!DataType.IsNullOrWhiteSpace(emailMessage.To))
				{
					addresses = emailMessage.To.Split(';');

					if ((object)addresses != null)
					{
						foreach (string address in addresses)
						{
							if (!DataType.IsNullOrWhiteSpace(address))
								mailMessage.To.Add(address);
						}
					}
				}

				if (!DataType.IsNullOrWhiteSpace(emailMessage.From))
					mailMessage.From = new MailAddress(emailMessage.From);

				if (!DataType.IsNullOrWhiteSpace(emailMessage.Sender))
					mailMessage.Sender = new MailAddress(emailMessage.Sender);

				if (!DataType.IsNullOrWhiteSpace(emailMessage.ReplyTo))
				{
					addresses = emailMessage.ReplyTo.Split(';');

					if ((object)addresses != null)
					{
						foreach (string address in addresses)
						{
							if (!DataType.IsNullOrWhiteSpace(address))
								mailMessage.ReplyToList.Add(address);
						}
					}

					// IF !NET40+
					// mailMessage.ReplyTo = emailMessage.ReplyTo;
				}

				if (!DataType.IsNullOrWhiteSpace(emailMessage.BlindCarbonCopy))
				{
					addresses = emailMessage.BlindCarbonCopy.Split(';');

					if ((object)addresses != null)
					{
						foreach (string address in addresses)
						{
							if (!DataType.IsNullOrWhiteSpace(address))
								mailMessage.Bcc.Add(address);
						}
					}
				}

				if (!DataType.IsNullOrWhiteSpace(emailMessage.CarbonCopy))
				{
					addresses = emailMessage.CarbonCopy.Split(';');

					if ((object)addresses != null)
					{
						foreach (string address in addresses)
						{
							if (!DataType.IsNullOrWhiteSpace(address))
								mailMessage.CC.Add(address);
						}
					}
				}

				mailMessage.IsBodyHtml = emailMessage.IsBodyHtml ?? false;

				if ((object)emailMessage.EmailAttachments != null)
				{
					foreach (IEmailAttachment emailAttachment in emailMessage.EmailAttachments)
					{
						Attachment attachment;
						MemoryStream memoryStream;

						// DO NOT WRAP STREAM IN USING BLOCK...NOT SURE WHO OWNS DISPOSAL?
						memoryStream = new MemoryStream(emailAttachment.AttachmentBits);
						attachment = new Attachment(memoryStream, emailAttachment.FileName, emailAttachment.MimeType);

						mailMessage.Attachments.Add(attachment);
					}
				}

				// do the heavy lifting
				smtpClient.Send(mailMessage);
				emailMessage.Processed = true;

				// SO WE WILL DO IT HERE AND SEE?
				foreach (Attachment attachment in mailMessage.Attachments)
					attachment.Dispose();
			}
		}

		private void ResolveApply(bool strictMatching, MessageTemplate messageTemplate, IEmailMessage emailMessage, object source)
		{
			XmlPersistEngine xpe;
			TemplateConstruct template;
			ITemplatingContext templatingContext;
			XmlTextReader templateXmlTextReader;

			if ((object)messageTemplate == null)
				throw new ArgumentNullException("messageTemplate");

			if ((object)emailMessage == null)
				throw new ArgumentNullException("emailMessage");

			if ((object)source == null)
				throw new ArgumentNullException("source");

			xpe = new XmlPersistEngine();
			xpe.RegisterWellKnownConstructs();

			using (NullInputMechanism nullInputMechanism = new NullInputMechanism())
			{
				using (StringOutputMechanism stringOutputMechanism = new StringOutputMechanism())
				{
					using (templatingContext = new TemplatingContext(xpe, new Tokenizer(strictMatching), nullInputMechanism, stringOutputMechanism))
					{
						// FROM
						using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.FromXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.From = stringOutputMechanism.RecycleOutput();

						// SENDER
						using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.SenderXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.Sender = stringOutputMechanism.RecycleOutput();

						// REPLYTO
						using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.ReplyToXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.ReplyTo = stringOutputMechanism.RecycleOutput();

						// TO
						using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.ToXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.To = stringOutputMechanism.RecycleOutput();

						// CC
						using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.CarbonCopyXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.CarbonCopy = stringOutputMechanism.RecycleOutput();

						// BCC
						using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.BlindCarbonCopyXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.BlindCarbonCopy = stringOutputMechanism.RecycleOutput();

						// SUBJECT
						using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.SubjectXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.Subject = stringOutputMechanism.RecycleOutput();

						// ISBODYHTML
						emailMessage.IsBodyHtml = messageTemplate.IsBodyHtml;

						// BODY
						using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.BodyXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.Body = stringOutputMechanism.RecycleOutput();
					}
				}
			}
		}

		#endregion
	}
}