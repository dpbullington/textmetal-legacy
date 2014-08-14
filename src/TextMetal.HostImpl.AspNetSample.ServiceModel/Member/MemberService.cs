/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Common.Core;
using TextMetal.Common.Data;
using TextMetal.Common.Data.Framework;
using TextMetal.Common.Data.Framework.LinqToSql;
using TextMetal.Common.Data.Framework.PoPimp;
using TextMetal.HostImpl.AspNetSample.DomainModel;
using TextMetal.HostImpl.AspNetSample.DomainModel.L2S;
using TextMetal.HostImpl.AspNetSample.DomainModel.Tables;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.Member
{
	public class MemberService : ServiceBase, IMemberService
	{
		#region Methods/Operators

		public CreateMemberResponse CreateMember(CreateMemberRequest request)
		{
			CreateMemberResponse response;
			IUser user;
			IMember member;

			using (var scope = new AmbientUnitOfWorkScope(this.Repository))
			{
				if ((object)request == null)
					throw new ArgumentNullException("request");

				if ((object)request.UserName == null)
					throw new InvalidOperationException();

				response = new CreateMemberResponse()
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
				member = new DomainModel.Tables.Member();
				member.MemberId = user.UserId;
				member.OrganizationId = request.OrganizationId;
				member.MemberName = request.MemberName;
				member.CreationUserId = user.UserId; // IMPORTANT (SAME AS MEMBER ID)
				member.SecurityRoleId = request.MemberSecurityRoleId;

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
				response.UserId = user.UserId;

				scope.ScopeComplete();

				return response;
			}
		}

		public EditMemberResponse EditMember(EditMemberRequest request)
		{
			EditMemberResponse response;
			IUser user;
			IMember member;

			using (var scope = new AmbientUnitOfWorkScope(this.Repository))
			{
				if ((object)request == null)
					throw new ArgumentNullException("request");

				if ((object)request.UserId == null)
					throw new InvalidOperationException();

				response = new EditMemberResponse()
							{
							};

				user = this.Repository.Load<IUser>(new DomainModel.Tables.User() { UserId = request.UserId });

				if ((object)user == null)
				{
					response.Messages = new[] { new Message("", "User was not found.", Severity.Error) };
					return response;
				}

				if (!DataType.IsNullOrWhiteSpace(request.UserName))
					user.UserName = request.UserName;

				if (!DataType.IsNullOrWhiteSpace(request.SecurityQuestion))
					user.Question = request.SecurityQuestion;

				if (!DataType.IsNullOrWhiteSpace(request.EmailAddress))
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
				member = this.Repository.Load<IMember>(new DomainModel.Tables.Member() { MemberId = (int)request.MemberId });
				member.IsNew = false; // HACK DUE TO NON-AUTO PK
				//member.MemberId = user.UserId;
				//member.OrganizationId = request.OrganizationId;
				member.MemberName = request.MemberName;
				//member.CreationUserId = user.UserId; // IMPORTANT (SAME AS MEMBER ID)
				member.SecurityRoleId = request.MemberSecurityRoleId;

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

				scope.ScopeComplete();

				return response;
			}
		}

		public ListMembersResponse ListMembers(ListMembersRequest request)
		{
			ListMembersResponse response;

			if ((object)request == null)
				throw new ArgumentNullException("request");

			using (var scope = new AmbientUnitOfWorkScope(this.Repository))
			{
				response = new ListMembersResponse();

				response.Results = this.Repository.LinqQuery((dc) => dc.Application_Members.Where(e => e.OrganizationId == request.OrganizationId && e.LogicalDelete == false).OrderBy(e => e.MemberName).Select(p => new ListMembersResult()
				{
					OrganizationId = p.Application_Organization.OrganizationId,
					MemberId = p.MemberId,
					CreationTimestamp = p.CreationTimestamp,
					ModificationTimestamp = p.ModificationTimestamp,
					MemberName = p.MemberName,
					UserId = p.Global_User.UserId,
					EmailAddress = p.Global_User.EmailAddress,
					UserName = p.Global_User.UserName,
					SecurityQuestion = p.Global_User.Question
				})).ToList(); // force eager load here
				
				return response;
			}
		}

		public LoadMemberResponse LoadMember(LoadMemberRequest request)
		{
			LoadMemberResponse response;

			if ((object)request == null)
				throw new ArgumentNullException("request");

			using (var scope = new AmbientUnitOfWorkScope(this.Repository))
			{
				response = this.Repository.LinqQuery((dc) => dc.Application_Members.Where(e => (e.MemberId == request.MemberId) && (e.OrganizationId == request.OrganizationId) && e.LogicalDelete == false).Select(p => new LoadMemberResponse()
																																																										{
																																																											OrganizationId = p.Application_Organization.OrganizationId,
																																																											MemberId = p.MemberId,
																																																											MemberCreationTimestamp = p.CreationTimestamp,
																																																											MemberModificationTimestamp = p.ModificationTimestamp,
																																																											MemberName = p.MemberName,
																																																											UserId = p.Global_User.UserId,
																																																											EmailAddress = p.Global_User.EmailAddress,
																																																											UserName = p.Global_User.UserName,
																																																											SecurityQuestion = p.Global_User.Question,
																																																											MemberSecurityRoleId = p.SecurityRoleId
																																																										})).SingleOrDefault();

				return response;
			}
		}

		public RemoveMemberResponse RemoveMember(RemoveMemberRequest request)
		{
			RemoveMemberResponse response;
			IEnumerable<IMember> members;
			IMember member;
			IModelQuery modelQuery;

			using (var scope = new AmbientUnitOfWorkScope(this.Repository))
			{
				if ((object)request == null)
					throw new ArgumentNullException("request");

				if ((object)request.MemberId == null)
					throw new InvalidOperationException();

				response = new RemoveMemberResponse()
							{
							};

				modelQuery = new LinqTableQuery<IMember>(m => m.OrganizationId == (int)Current.OrganizationId && m.MemberId == (int)request.MemberId && m.LogicalDelete == false);
				
				members = this.Repository.Find<IMember>(modelQuery);

				member = members.SingleOrDefault();

				if ((object)member == null)
					throw new InvalidOperationException("not found");

				member.IsNew = false; // HACK DUE TO NON-AUTO PK
				if (!this.Repository.Discard<IMember>(member))
				{
					response.Messages = new[] { new Message("", "A conflict error occured.", Severity.Error) };
					return response;
				}

				scope.ScopeComplete();

				return response;
			}
		}

		#endregion
	}
}