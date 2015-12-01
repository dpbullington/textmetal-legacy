/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Framework.Hosting.Email
{
	public sealed class EmailMessage
	{
		#region Constructors/Destructors

		public EmailMessage()
		{
		}

		#endregion

		#region Fields/Constants

		private string blindCarbonCopy;
		private string body;
		private string carbonCopy;
		private string from;
		private bool? isBodyHtml;
		private bool? processed;
		private string replyTo;
		private string sender;
		private string subject;
		private string to;

		#endregion

		#region Properties/Indexers/Events

		public string BlindCarbonCopy
		{
			get
			{
				return this.blindCarbonCopy;
			}
			internal set
			{
				this.blindCarbonCopy = value;
			}
		}

		public string Body
		{
			get
			{
				return this.body;
			}
			internal set
			{
				this.body = value;
			}
		}

		public string CarbonCopy
		{
			get
			{
				return this.carbonCopy;
			}
			internal set
			{
				this.carbonCopy = value;
			}
		}

		public string From
		{
			get
			{
				return this.from;
			}
			internal set
			{
				this.from = value;
			}
		}

		public bool? IsBodyHtml
		{
			get
			{
				return this.isBodyHtml;
			}
			internal set
			{
				this.isBodyHtml = value;
			}
		}

		public bool? Processed
		{
			get
			{
				return this.processed;
			}
			internal set
			{
				this.processed = value;
			}
		}

		public string ReplyTo
		{
			get
			{
				return this.replyTo;
			}
			internal set
			{
				this.replyTo = value;
			}
		}

		public string Sender
		{
			get
			{
				return this.sender;
			}
			internal set
			{
				this.sender = value;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
			internal set
			{
				this.subject = value;
			}
		}

		public string To
		{
			get
			{
				return this.to;
			}
			internal set
			{
				this.to = value;
			}
		}

		#endregion
	}
}