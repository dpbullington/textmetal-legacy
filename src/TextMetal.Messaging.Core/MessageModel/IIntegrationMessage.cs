/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace TextMetal.Messaging.Core.MessageModel
{
	public interface IIntegrationMessage : IIntegrationComponent
	{
		#region Properties/Indexers/Events

		IDictionary<string, object> Metadata
		{
			get;
		}

		string ContentEncoding
		{
			get;
			set;
		}

		string ContentType
		{
			get;
			set;
		}

		Stream DataStream
		{
			get;
			set;
		}

		#endregion
	}
}