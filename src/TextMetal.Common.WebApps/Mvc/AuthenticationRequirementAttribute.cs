/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.WebApps.Mvc
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class AuthenticationRequirementAttribute : Attribute
	{
		#region Constructors/Destructors

		public AuthenticationRequirementAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private AuthenticationOutcome authenticationOutcome;

		#endregion

		#region Properties/Indexers/Events

		public AuthenticationOutcome AuthenticationOutcome
		{
			get
			{
				return this.authenticationOutcome;
			}
			set
			{
				this.authenticationOutcome = value;
			}
		}

		#endregion
	}
}