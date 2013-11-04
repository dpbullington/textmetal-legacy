/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.Web.Email
{
	public interface IEmailAttachment
	{
		#region Properties/Indexers/Events

		byte[] AttachmentBits
		{
			get;
			set;
		}

		IEmailMessage EmailMessage
		{
			get;
		}

		string MimeType
		{
			get;
			set;
		}

		#endregion
	}
}