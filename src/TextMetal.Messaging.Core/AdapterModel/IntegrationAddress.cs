/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Security.Principal;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public sealed class IntegrationAddress
	{
		#region Constructors/Destructors

		public IntegrationAddress(Uri uri, IIdentity identity)
		{
			if ((object)uri == null)
				throw new ArgumentNullException(nameof(uri));

			if ((object)identity == null)
				throw new ArgumentNullException(nameof(identity));
			this.uri = uri;
			this.identity = identity;
		}

		#endregion

		#region Fields/Constants

		private readonly IIdentity identity;
		private readonly Uri uri;

		#endregion

		#region Properties/Indexers/Events

		public IIdentity Identity
		{
			get
			{
				return this.identity;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.uri;
			}
		}

		#endregion
	}
}