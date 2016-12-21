/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Messaging.Core.MessageModel
{
	public sealed class MessageStateConstants
	{
		#region Fields/Constants

		public const string MESSAGE_SCHEMA_UNKNOWN = "{00000000-0000-0000-0000-000000000000}";
		public const string MESSAGE_STATE_ARCHIVED = "{818DDFE9-5DEF-44D2-8DE1-8B8E9C746DA1}";
		public const string MESSAGE_STATE_CREATED = "{987B97EF-D88F-4A3A-A5C8-872A0F1E6AA4}";
		public const string MESSAGE_STATE_PROCESSED = "{71A78253-F860-4F0A-B6CD-DA13668B0A58}";
		public const string MESSAGE_STATE_SUSPENDED = "{68416F11-941D-4B16-A30C-44EAF0E1E247}";
		public const string MESSAGE_STATE_UNKNOWN = "{00000000-0000-0000-0000-000000000000}";

		#endregion
	}
}