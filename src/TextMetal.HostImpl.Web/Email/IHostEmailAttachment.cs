/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.Web.Email
{
	public interface IHostEmailAttachment
	{
		#region Properties/Indexers/Events

		byte[] AttachmentBits
		{
			get;
			set;
		}

		string FileName
		{
			get;
			set;
		}

		string MimeType
		{
			get;
			set;
		}

		IHostEmailMessage HostEmailMessage
		{
			get;
		}

		#endregion
	}
}