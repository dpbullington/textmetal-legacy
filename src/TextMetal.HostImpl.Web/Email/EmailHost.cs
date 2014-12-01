/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Xml;

using TextMetal.Common.Core;
using TextMetal.Common.Core.Tokenization;
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

		public void Host(bool strictMatching, MessageTemplate messageTemplate, object modelObject, IHostEmailMessage hostEmailMessage)
		{
			SmtpClient smtpClient;
			string[] addresses;

			if ((object)messageTemplate == null)
				throw new ArgumentNullException("messageTemplate");

			if ((object)hostEmailMessage == null)
				throw new ArgumentNullException("hostEmailMessage");

			if ((object)modelObject == null)
				throw new ArgumentNullException("modelObject");

			// run templating over message template and apply to email maessage
			this.ResolveApply(strictMatching, messageTemplate, hostEmailMessage, modelObject);

			// no longer hardcoded, uses standard config file
			smtpClient = new SmtpClient();

			using (MailMessage mailMessage = new MailMessage())
			{
				if (!DataType.Instance.IsNullOrWhiteSpace(hostEmailMessage.Subject))
					mailMessage.Subject = hostEmailMessage.Subject;

				if (!DataType.Instance.IsNullOrWhiteSpace(hostEmailMessage.Body))
					mailMessage.Body = hostEmailMessage.Body;

				if (!DataType.Instance.IsNullOrWhiteSpace(hostEmailMessage.To))
				{
					addresses = hostEmailMessage.To.Split(';');

					if ((object)addresses != null)
					{
						foreach (string address in addresses)
						{
							if (!DataType.Instance.IsNullOrWhiteSpace(address))
								mailMessage.To.Add(address);
						}
					}
				}

				if (!DataType.Instance.IsNullOrWhiteSpace(hostEmailMessage.From))
					mailMessage.From = new MailAddress(hostEmailMessage.From);

				if (!DataType.Instance.IsNullOrWhiteSpace(hostEmailMessage.Sender))
					mailMessage.Sender = new MailAddress(hostEmailMessage.Sender);

				if (!DataType.Instance.IsNullOrWhiteSpace(hostEmailMessage.ReplyTo))
				{
					addresses = hostEmailMessage.ReplyTo.Split(';');

					if ((object)addresses != null)
					{
						foreach (string address in addresses)
						{
							if (!DataType.Instance.IsNullOrWhiteSpace(address))
								mailMessage.ReplyToList.Add(address);
						}
					}

					// IF !NET40+
					// mailMessage.ReplyTo = emailMessage.ReplyTo;
				}

				if (!DataType.Instance.IsNullOrWhiteSpace(hostEmailMessage.BlindCarbonCopy))
				{
					addresses = hostEmailMessage.BlindCarbonCopy.Split(';');

					if ((object)addresses != null)
					{
						foreach (string address in addresses)
						{
							if (!DataType.Instance.IsNullOrWhiteSpace(address))
								mailMessage.Bcc.Add(address);
						}
					}
				}

				if (!DataType.Instance.IsNullOrWhiteSpace(hostEmailMessage.CarbonCopy))
				{
					addresses = hostEmailMessage.CarbonCopy.Split(';');

					if ((object)addresses != null)
					{
						foreach (string address in addresses)
						{
							if (!DataType.Instance.IsNullOrWhiteSpace(address))
								mailMessage.CC.Add(address);
						}
					}
				}

				mailMessage.IsBodyHtml = hostEmailMessage.IsBodyHtml ?? false;

				if ((object)hostEmailMessage.HostEmailAttachments != null)
				{
					foreach (IHostEmailAttachment hostEmailAttachment in hostEmailMessage.HostEmailAttachments)
					{
						Attachment attachment;
						MemoryStream memoryStream;

						// DO NOT WRAP STREAM IN USING BLOCK...NOT SURE WHO OWNS DISPOSAL?
						memoryStream = new MemoryStream(hostEmailAttachment.AttachmentBits);
						attachment = new Attachment(memoryStream, hostEmailAttachment.FileName, hostEmailAttachment.MimeType);

						mailMessage.Attachments.Add(attachment);
					}
				}

				// do the heavy lifting
				smtpClient.Send(mailMessage);
				hostEmailMessage.Processed = true;

				// SO WE WILL DO IT HERE AND SEE?
				foreach (Attachment attachment in mailMessage.Attachments)
					attachment.Dispose();
			}
		}

		private void ResolveApply(bool strictMatching, MessageTemplate messageTemplate, IHostEmailMessage hostEmailMessage, object source)
		{
			XmlPersistEngine xpe;
			TemplateConstruct template;
			ITemplatingContext templatingContext;
			XmlTextReader templateXmlTextReader;

			if ((object)messageTemplate == null)
				throw new ArgumentNullException("messageTemplate");

			if ((object)hostEmailMessage == null)
				throw new ArgumentNullException("hostEmailMessage");

			if ((object)source == null)
				throw new ArgumentNullException("source");

			xpe = new XmlPersistEngine();
			xpe.RegisterWellKnownConstructs();

			using (IInputMechanism inputMechanism = new NullInputMechanism())
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					using (IOutputMechanism outputMechanism = new TextWriterOutputMechanism(stringWriter, xpe))
					{
						using (templatingContext = new TemplatingContext(xpe, new Tokenizer(strictMatching), inputMechanism, outputMechanism, new Dictionary<string, IList<string>>()))
						{
							// FROM
							using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.FromXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

							templatingContext.IteratorModels.Push(source);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							hostEmailMessage.From = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// SENDER
							using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.SenderXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

							templatingContext.IteratorModels.Push(source);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							hostEmailMessage.Sender = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// REPLYTO
							using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.ReplyToXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

							templatingContext.IteratorModels.Push(source);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							hostEmailMessage.ReplyTo = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// TO
							using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.ToXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

							templatingContext.IteratorModels.Push(source);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							hostEmailMessage.To = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// CC
							using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.CarbonCopyXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

							templatingContext.IteratorModels.Push(source);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							hostEmailMessage.CarbonCopy = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// BCC
							using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.BlindCarbonCopyXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

							templatingContext.IteratorModels.Push(source);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							hostEmailMessage.BlindCarbonCopy = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// SUBJECT
							using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.SubjectXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

							templatingContext.IteratorModels.Push(source);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							hostEmailMessage.Subject = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// ISBODYHTML
							hostEmailMessage.IsBodyHtml = messageTemplate.IsBodyHtml;

							// BODY
							using (templateXmlTextReader = new XmlTextReader(new StringReader(messageTemplate.BodyXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

							templatingContext.IteratorModels.Push(source);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							hostEmailMessage.Body = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();
						}
					}
				}
			}
		}

		#endregion
	}
}