/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Security.Principal;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public sealed class DefaultIdentity : IIdentity
	{
		#region Constructors/Destructors

		private DefaultIdentity()
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly DefaultIdentity instance = new DefaultIdentity();

		#endregion

		#region Properties/Indexers/Events

		public static DefaultIdentity Instance
		{
			get
			{
				return instance;
			}
		}

		public string AuthenticationType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string Name
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}