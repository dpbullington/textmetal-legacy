/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using TextMetal.Framework.Core;
using TextMetal.Framework.InputOutput;
using TextMetal.Framework.Template;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Hosting.Email
{
	public sealed class EmailHost : IEmailHost
	{
		#region Constructors/Destructors

		public EmailHost()
		{
		}

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
		}

		public EmailMessage Host(bool strictMatching, EmailTemplate emailTemplate, object modelObject)
		{
			EmailMessage emailMessage;

			XmlPersistEngine xpe;
			TemplateConstruct template;
			ITemplatingContext templatingContext;
			XmlReader templateXmlReader;

			if ((object)emailTemplate == null)
				throw new ArgumentNullException("emailTemplate");

			emailMessage = new EmailMessage();

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
							using (templateXmlReader = XmlReader.Create(new StringReader(emailTemplate.FromXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlReader);

							templatingContext.IteratorModels.Push(modelObject);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							emailMessage.From = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// SENDER
							using (templateXmlReader = XmlReader.Create(new StringReader(emailTemplate.SenderXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlReader);

							templatingContext.IteratorModels.Push(modelObject);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							emailMessage.Sender = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// REPLYTO
							using (templateXmlReader = XmlReader.Create(new StringReader(emailTemplate.ReplyToXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlReader);

							templatingContext.IteratorModels.Push(modelObject);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							emailMessage.ReplyTo = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// TO
							using (templateXmlReader = XmlReader.Create(new StringReader(emailTemplate.ToXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlReader);

							templatingContext.IteratorModels.Push(modelObject);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							emailMessage.To = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// CC
							using (templateXmlReader = XmlReader.Create(new StringReader(emailTemplate.CarbonCopyXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlReader);

							templatingContext.IteratorModels.Push(modelObject);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							emailMessage.CarbonCopy = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// BCC
							using (templateXmlReader = XmlReader.Create(new StringReader(emailTemplate.BlindCarbonCopyXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlReader);

							templatingContext.IteratorModels.Push(modelObject);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							emailMessage.BlindCarbonCopy = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// SUBJECT
							using (templateXmlReader = XmlReader.Create(new StringReader(emailTemplate.SubjectXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlReader);

							templatingContext.IteratorModels.Push(modelObject);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							emailMessage.Subject = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();

							// ISBODYHTML
							emailMessage.IsBodyHtml = emailTemplate.IsBodyHtml;

							// BODY
							using (templateXmlReader = XmlReader.Create(new StringReader(emailTemplate.BodyXml.OuterXml)))
								template = (TemplateConstruct)xpe.DeserializeFromXml(templateXmlReader);

							templatingContext.IteratorModels.Push(modelObject);
							template.ExpandTemplate(templatingContext);
							templatingContext.IteratorModels.Pop();

							emailMessage.Body = stringWriter.ToString();
							stringWriter.GetStringBuilder().Clear();
						}
					}
				}
			}

			return emailMessage;
		}

		#endregion
	}
}