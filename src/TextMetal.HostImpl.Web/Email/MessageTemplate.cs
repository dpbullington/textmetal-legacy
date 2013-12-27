/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Net.Mail;
using System.Xml;
using System.Xml.Serialization;

using TextMetal.Common.Cerealization;
using TextMetal.Common.Core;
using TextMetal.Common.Core.StringTokens;
using TextMetal.Common.Xml;
using TextMetal.Framework.Core;
using TextMetal.Framework.HostingModel;
using TextMetal.Framework.InputOutputModel;
using TextMetal.Framework.TemplateModel;

namespace TextMetal.HostImpl.Web.Email
{
	[Serializable]
	[XmlRoot(ElementName = "MessageTemplate", Namespace = "http://www.textmetal.com/api/v5.0.0")]
	public class MessageTemplate
	{
		#region Constructors/Destructors

		public MessageTemplate()
		{
		}

		#endregion

		#region Fields/Constants

		private XmlElement blindCarbonCopyXml;
		private XmlElement bodyXml;
		private XmlElement carbonCopyXml;
		private XmlElement fromXml;
		private bool isBodyHtml;
		private XmlElement replyToXml;
		private XmlElement senderXml;
		private XmlElement subjectXml;
		private XmlElement toXml;

		#endregion

		#region Properties/Indexers/Events

		[XmlElement("BlindCarbonCopy", Order = 5)]
		public XmlElement BlindCarbonCopyXml
		{
			get
			{
				return this.blindCarbonCopyXml;
			}
			set
			{
				this.blindCarbonCopyXml = value;
			}
		}

		[XmlElement("Body", Order = 8)]
		public XmlElement BodyXml
		{
			get
			{
				return this.bodyXml;
			}
			set
			{
				this.bodyXml = value;
			}
		}

		[XmlElement("CarbonCopy", Order = 4)]
		public XmlElement CarbonCopyXml
		{
			get
			{
				return this.carbonCopyXml;
			}
			set
			{
				this.carbonCopyXml = value;
			}
		}

		[XmlElement("From", Order = 0)]
		public XmlElement FromXml
		{
			get
			{
				return this.fromXml;
			}
			set
			{
				this.fromXml = value;
			}
		}

		[XmlElement("IsBodyHtml", Order = 7)]
		public bool IsBodyHtml
		{
			get
			{
				return this.isBodyHtml;
			}
			set
			{
				this.isBodyHtml = value;
			}
		}

		[XmlElement("ReplyTo", Order = 2)]
		public XmlElement ReplyToXml
		{
			get
			{
				return this.replyToXml;
			}
			set
			{
				this.replyToXml = value;
			}
		}

		[XmlElement("Sender", Order = 1)]
		public XmlElement SenderXml
		{
			get
			{
				return this.senderXml;
			}
			set
			{
				this.senderXml = value;
			}
		}

		[XmlElement("Subject", Order = 6)]
		public XmlElement SubjectXml
		{
			get
			{
				return this.subjectXml;
			}
			set
			{
				this.subjectXml = value;
			}
		}

		[XmlElement("To", Order = 3)]
		public XmlElement ToXml
		{
			get
			{
				return this.toXml;
			}
			set
			{
				this.toXml = value;
			}
		}

		#endregion

		#region Methods/Operators

		public static TEmailMessage SendEmailTemplate<TEmailMessage>(Type templateResourceType, string templateResourceName, object modelObject, Func<TEmailMessage, bool> saveMethod)
			where TEmailMessage : class, IEmailMessage, new()
		{
			MessageTemplate messageTemplate;
			TEmailMessage emailMessage;
			SmtpClient smtpClient;
			string[] addresses;

			if ((object)templateResourceType == null)
				throw new ArgumentNullException("templateResourceType");

			if ((object)templateResourceName == null)
				throw new ArgumentNullException("templateResourceName");

			if ((object)modelObject == null)
				throw new ArgumentNullException("modelObject");

			if ((object)saveMethod == null)
				throw new ArgumentNullException("saveMethod");

			if (!Cerealization.TryGetFromAssemblyResource<MessageTemplate>(templateResourceType, templateResourceName, out messageTemplate))
				throw new InvalidOperationException(string.Format("Unable to deserialize instance of '{0}' from the manifest resource name '{1}' in the assembly '{2}'.", typeof(MessageTemplate).FullName, templateResourceName, templateResourceType.Assembly.FullName));

			emailMessage = messageTemplate.Resolve<TEmailMessage>(modelObject);

			if ((object)emailMessage == null)
				throw new InvalidOperationException(string.Format("Resolved email message was invalid."));

			if (!saveMethod(emailMessage))
				throw new InvalidOperationException(string.Format("Save method callback returned false during email template send processing; possible data concurrency failure."));

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

				try
				{
					smtpClient.Send(mailMessage);
					emailMessage.Processed = true;
				}
				finally
				{
					saveMethod(emailMessage);
				}
			}

			return emailMessage;
		}

		private TEmailMessage Resolve<TEmailMessage>(object source)
			where TEmailMessage : class, IEmailMessage, new()
		{
			XmlPersistEngine xpe;
			TemplateConstruct template;
			ITemplatingContext templatingContext;
			XmlTextReader templateXmlTextReader;

			TEmailMessage emailMessage;

			if ((object)source == null)
				throw new ArgumentNullException("source");

			emailMessage = new TEmailMessage();

			xpe = new XmlPersistEngine();
			xpe.RegisterWellKnownConstructs();

			using (NullInputMechanism nullInputMechanism = new NullInputMechanism())
			{
				using (StringOutputMechanism stringOutputMechanism = new StringOutputMechanism())
				{
					using (templatingContext = new TemplatingContext(xpe, new Tokenizer(true), nullInputMechanism, stringOutputMechanism))
					{
						// FROM
						using (templateXmlTextReader = new XmlTextReader(new StringReader(this.FromXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.From = stringOutputMechanism.RecycleOutput();

						// SENDER
						using (templateXmlTextReader = new XmlTextReader(new StringReader(this.SenderXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.Sender = stringOutputMechanism.RecycleOutput();

						// REPLYTO
						using (templateXmlTextReader = new XmlTextReader(new StringReader(this.ReplyToXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.ReplyTo = stringOutputMechanism.RecycleOutput();

						// TO
						using (templateXmlTextReader = new XmlTextReader(new StringReader(this.ToXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.To = stringOutputMechanism.RecycleOutput();

						// CC
						using (templateXmlTextReader = new XmlTextReader(new StringReader(this.CarbonCopyXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.CarbonCopy = stringOutputMechanism.RecycleOutput();

						// BCC
						using (templateXmlTextReader = new XmlTextReader(new StringReader(this.BlindCarbonCopyXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.BlindCarbonCopy = stringOutputMechanism.RecycleOutput();

						// SUBJECT
						using (templateXmlTextReader = new XmlTextReader(new StringReader(this.SubjectXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.Subject = stringOutputMechanism.RecycleOutput();

						// ISBODYHTML
						emailMessage.IsBodyHtml = this.IsBodyHtml;

						// BODY
						using (templateXmlTextReader = new XmlTextReader(new StringReader(this.BodyXml.OuterXml)))
							template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlTextReader);

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();

						emailMessage.Body = stringOutputMechanism.RecycleOutput();
					}
				}
			}

			return emailMessage;
		}

		#endregion
	}
}