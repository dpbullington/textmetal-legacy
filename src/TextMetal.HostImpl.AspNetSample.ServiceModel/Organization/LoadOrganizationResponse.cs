/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.Organization
{
	public sealed class LoadOrganizationResponse : ResponseBase
	{
		#region Properties/Indexers/Events

		public string AnswerHash
		{
			get;
			set;
		}

		public DateTime? CreationTimestamp
		{
			get;
			set;
		}

		public int? CreationOrganizationId
		{
			get;
			set;
		}

		public string EmailAddress
		{
			get;
			set;
		}

		public short? FailedLoginCount
		{
			get;
			set;
		}

		public DateTime? LastLoginFailureTimestamp
		{
			get;
			set;
		}

		public DateTime? LastLoginSuccessTimestamp
		{
			get;
			set;
		}

		public bool? LogicalDelete
		{
			get;
			set;
		}

		public DateTime? ModificationTimestamp
		{
			get;
			set;
		}

		public bool? MustChangePassword
		{
			get;
			set;
		}

		public string PasswordHash
		{
			get;
			set;
		}

		public string Question
		{
			get;
			set;
		}

		public string SaltValue
		{
			get;
			set;
		}

		public byte? SortOrder
		{
			get;
			set;
		}

		public int? OrganizationId
		{
			get;
			set;
		}

		public string OrganizationName
		{
			get;
			set;
		}

		#endregion
	}
}