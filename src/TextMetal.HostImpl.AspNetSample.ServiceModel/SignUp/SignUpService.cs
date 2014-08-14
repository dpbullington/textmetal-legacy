/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Common.Core;
using TextMetal.Common.Data;
using TextMetal.Common.Data.Framework;
using TextMetal.Common.Data.Framework.LinqToSql;
using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel;
using TextMetal.HostImpl.AspNetSample.DomainModel.Tables;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.SignUp
{
	public class SignUpService : ServiceBase, ISignUpService
	{
		#region Methods/Operators

		public SignUpResponse SignUp(SignUpRequest request)
		{
			SignUpResponse response;
			IOrganization organization;
			IUser user;
			IMember member;
			IEnumerable<ISecurityRole> securityRoles;
			IModelQuery modelQuery;

			using (var scope = new AmbientUnitOfWorkScope(this.Repository))
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

				if (!this.Repository.Save<IUser>(user))
				{
					response.Messages = new[] { new Message("", "A conflict error occured.", Severity.Error) };
					return response;
				}

				// +++
				organization = new DomainModel.Tables.Organization();
				organization.OrganizationName = request.OrganizationName;

				response.Messages = organization.Validate();

				if ((object)response.Messages == null)
					throw new InvalidOperationException();

				if (response.Messages.Count(m => m.Severity == Severity.Error) > 0)
					return response;

				if (!this.Repository.Save<IOrganization>(organization))
				{
					response.Messages = new[] { new Message("", "A conflict error occured.", Severity.Error) };
					return response;
				}

				// +++
				member = new DomainModel.Tables.Member();

				modelQuery = new LinqTableQuery<ISecurityRole>(sr => sr.SecurityRoleName == "OrganizationOwner" && sr.LogicalDelete == false);
				
				securityRoles = this.Repository.Find<ISecurityRole>(modelQuery);
				
				// techncial debt: not optimal but works
				member.SecurityRoleId = securityRoles.Select(x => x.SecurityRoleId).SingleOrDefault();
				
				member.MemberId = user.UserId;
				member.OrganizationId = organization.OrganizationId;
				member.MemberName = request.MemberName;
				member.CreationUserId = user.UserId; // IMPORTANT (SAME AS MEMBER ID)

				response.Messages = member.Validate();

				if ((object)response.Messages == null)
					throw new InvalidOperationException();

				if (response.Messages.Count(m => m.Severity == Severity.Error) > 0)
					return response;

				if (!this.Repository.Save<IMember>(member))
				{
					response.Messages = new[] { new Message("", "A conflict error occured.", Severity.Error) };
					return response;
				}

				response.MemberId = member.MemberId;
				response.OrganizationId = organization.OrganizationId;
				response.UserId = user.UserId;

				Stuff.Get<IRepository>("").TrySendEmailTemplate(EmailTemplateResourceNames.NEW_SIGN_UP, new
																										{
																											MemberEmailAddress = user.EmailAddress,
																											OrganizationName = organization.OrganizationName,
																											MemberName = member.MemberName
																										});

				scope.ScopeComplete();

				return response;
			}
		}

		#endregion
	}
}