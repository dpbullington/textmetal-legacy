/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.Common.WebApps.Mvc
{
	public abstract class BaseViewModel
	{
		#region Constructors/Destructors

		protected BaseViewModel()
		{
		}

		#endregion

		#region Fields/Constants

		private AssemblyInformation assemblyInformation;
		private long? currentUserId;
		private bool currentUserMustChangePassword;
		private bool currentUserWasAuthenticated;
		private IEnumerable<Message> messages;

		#endregion

		#region Properties/Indexers/Events

		public AssemblyInformation AssemblyInformation
		{
			get
			{
				return this.assemblyInformation;
			}
			set
			{
				this.assemblyInformation = value;
			}
		}

		public long? CurrentUserId
		{
			get
			{
				return this.currentUserId;
			}
			set
			{
				this.currentUserId = value;
			}
		}

		public bool CurrentUserMustChangePassword
		{
			get
			{
				return this.currentUserMustChangePassword;
			}
			set
			{
				this.currentUserMustChangePassword = value;
			}
		}

		public bool CurrentUserWasAuthenticated
		{
			get
			{
				return this.currentUserWasAuthenticated;
			}
			set
			{
				this.currentUserWasAuthenticated = value;
			}
		}

		public IEnumerable<Message> Messages
		{
			get
			{
				return this.messages;
			}
			set
			{
				this.messages = value;
			}
		}

		#endregion
	}
}