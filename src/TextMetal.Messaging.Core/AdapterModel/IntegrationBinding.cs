/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public sealed class IntegrationBinding
	{
		#region Constructors/Destructors

		public IntegrationBinding(object transport, object format, object protocol)
		{
			if ((object)transport == null)
				throw new ArgumentNullException("transport");

			if ((object)format == null)
				throw new ArgumentNullException("format");

			if ((object)protocol == null)
				throw new ArgumentNullException("protocol");

			this.transport = transport;
			this.format = format;
			this.protocol = protocol;
		}

		#endregion

		#region Fields/Constants

		private readonly object format;
		private readonly object protocol;
		private readonly object transport;

		#endregion

		#region Properties/Indexers/Events

		public object Format
		{
			get
			{
				return this.format;
			}
		}

		public object Protocol
		{
			get
			{
				return this.protocol;
			}
		}

		public object Transport
		{
			get
			{
				return this.transport;
			}
		}

		#endregion
	}
}