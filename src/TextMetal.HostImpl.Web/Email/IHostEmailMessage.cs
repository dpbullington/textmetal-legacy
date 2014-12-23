/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace TextMetal.HostImpl.Web.Email
{
	public interface IHostEmailMessage
	{
		#region Properties/Indexers/Events

		string BlindCarbonCopy
		{
			get;
			set;
		}

		string Body
		{
			get;
			set;
		}

		string CarbonCopy
		{
			get;
			set;
		}

		string From
		{
			get;
			set;
		}

		bool? IsBodyHtml
		{
			get;
			set;
		}

		bool? Processed
		{
			get;
			set;
		}

		string ReplyTo
		{
			get;
			set;
		}

		string Sender
		{
			get;
			set;
		}

		string Subject
		{
			get;
			set;
		}

		string To
		{
			get;
			set;
		}

		IList<IHostEmailAttachment> HostEmailAttachments
		{
			get;
		}

		#endregion
	}
}