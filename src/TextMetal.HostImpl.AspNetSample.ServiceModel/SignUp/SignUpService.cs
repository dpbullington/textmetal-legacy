/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Linq;

using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel;
using TextMetal.HostImpl.AspNetSample.DomainModel.Tables;

using TextMetal.Common.Core;
using TextMetal.Common.Data;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.SignUp
{
	public class SignUpService : ServiceBase, ISignUpService
	{
		#region Methods/Operators

		public SignUpResponse SignUp(SignUpRequest request)
		{
			SignUpResponse response;
			Family family;
			DomainModel.Tables.User user;
			DomainModel.Tables.Parent parent;

			using (var scope = new AmbientUnitOfWorkScope(DomainModel.Repository.DefaultUnitOfWorkFactory.Instance))
			{
				if ((object)request == null)
					throw new ArgumentNullException("request");

				if ((object)request.UserName == null)
					throw new InvalidOperationException();

				response = new SignUpResponse()
							{
							};

				user = new DomainModel.Tables.User();
				user.SaltValue = DomainModel.Tables.User.GenerateCryptoSafeSaltValue();
				user.UserName = request.UserName;
				user.Question = request.SecurityQuestion;
				user.EmailAddress = request.EmailAddress;

				if (!DataType.IsNullOrWhiteSpace(request.PasswordClearText))
					user.SetPassword(request.PasswordClearText);

				if (!DataType.IsNullOrWhiteSpace(request.SecurityAnswerClearText))
					user.SetAnswer(request.SecurityAnswerClearText);

				response.Messages = user.Validate();

				if ((object)response.Messages == null)
					throw new InvalidOperationException();

				if (response.Messages.Count(m => m.Severity == Severity.Error) > 0)
					return response;

				if (!this.Repository.SaveUser(user))
				{
					response.Messages = new[] { new Message("", "A conflict error occured.", Severity.Error) };
					return response;
				}

				// +++
				family = new Family();
				family.FamilyGuid = request.FamilyGuid;
				family.FamilyName = request.FamilyName;
				family.AddressLine1 = request.FamilyAddressLine1;
				family.AddressLine2 = request.FamilyAddressLine2;
				family.AddressLine3 = request.FamilyAddressLine3;
				family.CityCountyLocality = request.FamilyCityCountyLocality;
				family.StateProvince = request.FamilyStateProvince;
				family.ZipPostalCode = request.FamilyZipPostalCode;
				family.CountryTerritory = request.FamilyCountryTerritory;
				family.VoiceTelephoneNumber = request.FamilyVoiceTelephoneNumber;

				response.Messages = family.Validate();

				if ((object)response.Messages == null)
					throw new InvalidOperationException();

				if (response.Messages.Count(m => m.Severity == Severity.Error) > 0)
					return response;

				if (!this.Repository.SaveFamily(family))
				{
					response.Messages = new[] { new Message("", "A conflict error occured.", Severity.Error) };
					return response;
				}

				// +++
				parent = new DomainModel.Tables.Parent();
				parent.ParentId = user.UserId;
				parent.FamilyId = family.FamilyId;
				parent.PrefixName = request.ParentPrefixName;
				parent.FirstName = request.ParentFirstName;
				parent.MiddleName = request.ParentMiddleName;
				parent.LastName = request.ParentLastName;
				parent.SuffixName = request.ParentSuffixName;
				parent.SmsTelephoneNumber = request.ParentSmsTelephoneNumber;
				parent.CreationUserId = user.UserId; // IMPORTANT (SAME AS PARENT ID)

				response.Messages = parent.Validate();

				if ((object)response.Messages == null)
					throw new InvalidOperationException();

				if (response.Messages.Count(m => m.Severity == Severity.Error) > 0)
					return response;

				if (!this.Repository.SaveParent(parent))
				{
					response.Messages = new[] { new Message("", "A conflict error occured.", Severity.Error) };
					return response;
				}

				response.ParentId = parent.ParentId;
				response.FamilyId = family.FamilyId;
				response.UserId = user.UserId;

				Stuff.Get<IRepository>("").TrySendEmailTemplate(EmailTemplateResourceNames.NEW_SIGN_UP, new
																										{
																											ParentEmailAddress = user.EmailAddress,
																											FamilyName = family.FamilyName,
																											ParentFullName = parent.FullName
																										});

				scope.ScopeComplete();

				return response;
			}
		}

		#endregion
	}
}