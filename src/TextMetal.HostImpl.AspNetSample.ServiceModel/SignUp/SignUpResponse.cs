/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.SignUp
{
	public sealed class SignUpResponse : ResponseBase
	{
		#region Fields/Constants

		private int? familyId;
		private int? parentId;
		private int? userId;

		#endregion

		#region Properties/Indexers/Events

		public int? FamilyId
		{
			get
			{
				return this.familyId;
			}
			set
			{
				this.familyId = value;
			}
		}

		public int? ParentId
		{
			get
			{
				return this.parentId;
			}
			set
			{
				this.parentId = value;
			}
		}

		public int? UserId
		{
			get
			{
				return this.userId;
			}
			set
			{
				this.userId = value;
			}
		}

		#endregion
	}
}