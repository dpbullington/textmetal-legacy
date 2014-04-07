/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.SignUp
{
	public sealed class SignUpRequest : RequestBase
	{
		#region Fields/Constants

		private string emailAddress;
		private string familyAddressLine1;
		private string familyAddressLine2;
		private string familyAddressLine3;
		private string familyCityCountyLocality;
		private string familyCountryTerritory;
		private Guid? familyGuid;
		private string familyName;
		private string familyStateProvince;
		private string familyVoiceTelephoneNumber;
		private string familyZipPostalCode;

		private string parentFirstName;
		private string parentLastName;
		private string parentMiddleName;
		private string parentPrefixName;
		private string parentSmsTelephoneNumber;
		private string parentSuffixName;
		private string passwordClearText;
		private string securityAnswerClearText;
		private string securityQuestion;
		private string userName;

		#endregion

		#region Properties/Indexers/Events

		public string EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
			set
			{
				this.emailAddress = value;
			}
		}

		public string FamilyAddressLine1
		{
			get
			{
				return this.familyAddressLine1;
			}
			set
			{
				this.familyAddressLine1 = value;
			}
		}

		public string FamilyAddressLine2
		{
			get
			{
				return this.familyAddressLine2;
			}
			set
			{
				this.familyAddressLine2 = value;
			}
		}

		public string FamilyAddressLine3
		{
			get
			{
				return this.familyAddressLine3;
			}
			set
			{
				this.familyAddressLine3 = value;
			}
		}

		public string FamilyCityCountyLocality
		{
			get
			{
				return this.familyCityCountyLocality;
			}
			set
			{
				this.familyCityCountyLocality = value;
			}
		}

		public string FamilyCountryTerritory
		{
			get
			{
				return this.familyCountryTerritory;
			}
			set
			{
				this.familyCountryTerritory = value;
			}
		}

		public Guid? FamilyGuid
		{
			get
			{
				return this.familyGuid;
			}
			set
			{
				this.familyGuid = value;
			}
		}

		public string FamilyName
		{
			get
			{
				return this.familyName;
			}
			set
			{
				this.familyName = value;
			}
		}

		public string FamilyStateProvince
		{
			get
			{
				return this.familyStateProvince;
			}
			set
			{
				this.familyStateProvince = value;
			}
		}

		public string FamilyVoiceTelephoneNumber
		{
			get
			{
				return this.familyVoiceTelephoneNumber;
			}
			set
			{
				this.familyVoiceTelephoneNumber = value;
			}
		}

		public string FamilyZipPostalCode
		{
			get
			{
				return this.familyZipPostalCode;
			}
			set
			{
				this.familyZipPostalCode = value;
			}
		}

		public string ParentFirstName
		{
			get
			{
				return this.parentFirstName;
			}
			set
			{
				this.parentFirstName = value;
			}
		}

		public string ParentLastName
		{
			get
			{
				return this.parentLastName;
			}
			set
			{
				this.parentLastName = value;
			}
		}

		public string ParentMiddleName
		{
			get
			{
				return this.parentMiddleName;
			}
			set
			{
				this.parentMiddleName = value;
			}
		}

		public string ParentPrefixName
		{
			get
			{
				return this.parentPrefixName;
			}
			set
			{
				this.parentPrefixName = value;
			}
		}

		public string ParentSmsTelephoneNumber
		{
			get
			{
				return this.parentSmsTelephoneNumber;
			}
			set
			{
				this.parentSmsTelephoneNumber = value;
			}
		}

		public string ParentSuffixName
		{
			get
			{
				return this.parentSuffixName;
			}
			set
			{
				this.parentSuffixName = value;
			}
		}

		public string PasswordClearText
		{
			get
			{
				return this.passwordClearText;
			}
			set
			{
				this.passwordClearText = value;
			}
		}

		public string SecurityAnswerClearText
		{
			get
			{
				return this.securityAnswerClearText;
			}
			set
			{
				this.securityAnswerClearText = value;
			}
		}

		public string SecurityQuestion
		{
			get
			{
				return this.securityQuestion;
			}
			set
			{
				this.securityQuestion = value;
			}
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
			set
			{
				this.userName = value;
			}
		}

		#endregion
	}
}