/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml;
using System.Xml.Serialization;

namespace TextMetal.Framework.Hosting.Email
{
	[Serializable]
	[XmlRoot(ElementName = "MessageTemplate", Namespace = "http://www.textmetal.com/api/v6.0.0")]
	public sealed class EmailTemplate
	{
		#region Constructors/Destructors

		public EmailTemplate()
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
	}
}