/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Common.Core;
using TextMetal.Common.Data.Framework;
using TextMetal.Common.Data.Framework.LinqToSql;
using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel.Tables;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.User
{
	public class UserService : ServiceBase, IUserService
	{
		#region Fields/Constants

		public const int FAILED_LOGIN_COUNT_POLICY = 3;

		#endregion

		#region Methods/Operators

		public AuthenticateUserResponse AuthenticateUser(AuthenticateUserRequest request)
		{
			AuthenticateUserResponse response;
			IEnumerable<IUser> users;
			IUser user;
			IEnumerable<IMember> members;
			IMember member;
			IOrganization organization;
			bool failed, locked = false;
			IModelQuery modelQuery;

			if ((object)request == null)
				throw new ArgumentNullException("request");

			if ((object)request.UserName == null)
				throw new InvalidOperationException();

			if ((object)request.PasswordClearText == null)
				throw new InvalidOperationException();

			response = new AuthenticateUserResponse()
						{
							FailedLoginCount = request.FailedLoginCount
						};

			response.Messages = DomainModel.Tables.User.ValidateForAuthentication(request.UserName, request.PasswordClearText);

			if ((object)response.Messages == null)
				throw new InvalidOperationException();

			if (response.Messages.Count(m => m.Severity == Severity.Error) > 0)
				return response;

			modelQuery = new LinqTableQuery<DomainModel.L2S.Global_User>(u => u.UserName == request.UserName && u.UserId != 0 && u.LogicalDelete == false);

			users = this.Repository.Find<IUser>(modelQuery);

			user = users.SingleOrDefault();

			if ((object)user != null)
			{
				if (failed = !user.CheckPassword(request.PasswordClearText))
				{
					user.FailedLoginCount++;
					user.LastLoginFailureTimestamp = DateTime.UtcNow;
					locked = (user.FailedLoginCount >= FAILED_LOGIN_COUNT_POLICY);
				}
				else
				{
					locked = (user.FailedLoginCount >= FAILED_LOGIN_COUNT_POLICY);
					user.LastLoginSuccessTimestamp = !locked ? DateTime.UtcNow : user.LastLoginSuccessTimestamp;
					user.FailedLoginCount = !locked ? 0 : user.FailedLoginCount;
				}

				this.Repository.Save<IUser>(user);

				user = failed ? null : user;
			}

			if ((object)user == null)
			{
				if (!locked)
					response.Messages = new[] { new Message("", "The username and/or password did not match.", Severity.Error) };
				else
					response.Messages = new[] { new Message("", "The user account is disabled.", Severity.Error) };

				response.FailedLoginCount++;
			}
			else if (user.LogicalDelete ?? false)
				response.Messages = new[] { new Message("", "The user account is suspended.", Severity.Error) };
			else
			{
				response.FailedLoginCount = 0;
				response.UserId = user.UserId;
				response.MustChangePassword = user.MustChangePassword ?? false;

				modelQuery = new LinqTableQuery<DomainModel.L2S.Application_Member>(m => m.MemberId == user.UserId && m.LogicalDelete == false && m.Application_Organization.LogicalDelete == false);

				members = this.Repository.Find<IMember>(modelQuery);

				//members = this.Repository.LinqQuery((dc) => dc.Application_Members.Where(m => m.MemberId == user.UserId && m.LogicalDelete == false && m.Application_Organization.LogicalDelete == false).Select(m => new DomainModel.Tables.Member() { }));
				
				member = members.SingleOrDefault();

				if ((object)member == null)
				{
					response.Messages = new[] { new Message("", "The user is not associated with an active organization.", Severity.Error) };
					return response;
				}

				organization = this.Repository.Load<IOrganization>(new DomainModel.Tables.Organization() { OrganizationId = (int)member.OrganizationId });

				if ((object)organization == null)
				{
					response.Messages = new[] { new Message("", "The user is not associated with an active organization.", Severity.Error) };
					return response;
				}

				response.RememberMeToken = request.RememberMe ? new Tuple<int?, int?>(user.UserId, member.OrganizationId) : null;
				response.MemberId = member.MemberId;
				response.OrganizationId = member.OrganizationId;
			}

			return response;
		}

		public CreateUserResponse CreateUser(CreateUserRequest request)
		{
			CreateUserResponse response;
			IUser user;

			if ((object)request == null)
				throw new ArgumentNullException("request");

			if ((object)request.UserName == null)
				throw new InvalidOperationException();

			response = new CreateUserResponse()
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

			response.UserId = user.UserId;

			return response;
		}

		public EditUserResponse EditUser(EditUserRequest request)
		{
			EditUserResponse response;
			IUser user;

			if ((object)request == null)
				throw new ArgumentNullException("request");

			if ((object)request.UserId == null)
				throw new InvalidOperationException();

			response = new EditUserResponse()
						{
							MustChangePassword = request.MustChangePassword
						};

			user = this.Repository.Load<IUser>(new DomainModel.Tables.User() { UserId = (int)request.UserId });

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
			{
				if (request.MustChangePassword && user.CheckPassword(request.PasswordClearText))
				{
					response.Messages = new[] { new Message("", "Password cannot be the same password.", Severity.Error) };
					return response;
				}

				user.SetPassword(request.PasswordClearText);
				user.MustChangePassword = false;
			}

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

			response.MustChangePassword = false; // override request propagated value

			return response;
		}

		public ForgotPasswordResponse ForgotPassword(ForgotPasswordRequest request)
		{
			ForgotPasswordResponse response;
			IEnumerable<IUser> users;
			IUser user;
			bool failed, locked = false;
			string tempPasswordClearText;
			IModelQuery modelQuery;

			if ((object)request == null)
				throw new ArgumentNullException("request");

			if ((object)request.UserName == null)
				throw new InvalidOperationException();

			response = new ForgotPasswordResponse()
						{
							FailedLoginCount = request.FailedLoginCount,
							EmailSent = false
						};

			response.Messages = DomainModel.Tables.User.ValidateForForgotPassword(request.UserName);

			if ((object)response.Messages == null)
				throw new InvalidOperationException();

			if (response.Messages.Count(m => m.Severity == Severity.Error) > 0)
				return response;

			modelQuery = new LinqTableQuery<DomainModel.L2S.Global_User>(u => u.UserName == request.UserName && u.UserId != 0 && u.LogicalDelete == false);

			users = this.Repository.Find<IUser>(modelQuery);

			user = users.SingleOrDefault();

			if ((object)user != null)
			{
				if (failed = false /*!user.CheckAnswer(request.SecurityAnswerClearText)*/)
				{
					user.FailedLoginCount++;
					user.LastLoginFailureTimestamp = DateTime.UtcNow;

					locked = (user.FailedLoginCount >= FAILED_LOGIN_COUNT_POLICY);
				}
				else
				{
					locked = (user.FailedLoginCount >= FAILED_LOGIN_COUNT_POLICY);
					user.LastLoginSuccessTimestamp = !locked ? DateTime.UtcNow : user.LastLoginSuccessTimestamp;
					user.FailedLoginCount = !locked ? 0 : user.FailedLoginCount;
				}

				this.Repository.Save<IUser>(user);

				user = failed ? null : user;
			}

			if ((object)user == null)
			{
				if (!locked)
					response.Messages = new[] { new Message("", "The username does not belong to any registered user.", Severity.Error) };
				else
					response.Messages = new[] { new Message("", "The user account is disabled.", Severity.Error) };

				response.FailedLoginCount++;
			}
			else if (user.LogicalDelete ?? false)
				response.Messages = new[] { new Message("", "The user account is suspended.", Severity.Error) };
			else
			{
				tempPasswordClearText = Guid.NewGuid().ToString("N").Substring(0, 16);
				user.SetPassword(tempPasswordClearText);
				user.MustChangePassword = true;

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

				response.MustChangePassword = true; // override request propagated value
				response.FailedLoginCount = 0;
				response.EmailSent = this.Repository.TrySendEmailTemplate(EmailTemplateResourceNames.FORGOT_PASSWORD, new
																													{
																														UserEmailAddress = user.EmailAddress,
																														TemporaryPassword = tempPasswordClearText
																													});
			}

			return response;
		}

		public ForgotUsernameResponse ForgotUsername(ForgotUsernameRequest request)
		{
			ForgotUsernameResponse response;
			IEnumerable<IUser> users;
			IUser user;
			bool failed;
			IModelQuery modelQuery;

			if ((object)request == null)
				throw new ArgumentNullException("request");

			if ((object)request.EmailAddress == null)
				throw new InvalidOperationException();

			response = new ForgotUsernameResponse()
						{
							FailedLoginCount = request.FailedLoginCount,
							EmailSent = false
						};

			response.Messages = DomainModel.Tables.User.ValidateForForgotUserName(request.EmailAddress);

			if ((object)response.Messages == null)
				throw new InvalidOperationException();

			if (response.Messages.Count(m => m.Severity == Severity.Error) > 0)
				return response;

			modelQuery = new LinqTableQuery<DomainModel.L2S.Global_User>(u => u.EmailAddress == request.EmailAddress && u.UserId != 0 && u.LogicalDelete == false);

			users = this.Repository.Find<IUser>(modelQuery);

			user = users.SingleOrDefault();

			if ((object)user == null)
			{
				response.FailedLoginCount++;
				response.Messages = new[] { new Message("", "The email address does not belong to any registered user.", Severity.Error) };
				return response;
			}

			//if (failed = !user.CheckAnswer(request.SecurityAnswerClearText))
			//{
			//	user.FailedLoginCount++;
			//	user.LastLoginFailureTimestamp = DateTime.UtcNow;

			//	if (locked = (user.FailedLoginCount >= FAILED_LOGIN_COUNT_POLICY))
			//		user.IsAccountDisabled = true;
			//}
			//else
			//{
			//	user.LastLoginSuccessTimestamp = !(user.IsAccountDisabled ?? false) ? DateTime.UtcNow : user.LastLoginSuccessTimestamp;
			//	user.FailedLoginCount = !(user.IsAccountDisabled ?? false) ? 0 : user.FailedLoginCount;
			//}

			//this.Repository.SaveUser(user);

			//user = failed ? null : user;

			response.Messages = user.Validate();

			if ((object)response.Messages == null)
				throw new InvalidOperationException();

			if (response.Messages.Count(m => m.Severity == Severity.Error) > 0)
				return response;

			//if (!this.Repository.SaveUser(user))
			//{
			//	response.Messages = new[] { new Message("", "A conflict error occured.", Severity.Error) };
			//	return response;
			//}

			//response.MustChangePassword = false; // override request propagated value

			response.FailedLoginCount = 0;
			response.EmailSent = this.Repository.TrySendEmailTemplate(EmailTemplateResourceNames.FORGOT_USERNAME, new
																												{
																													UserEmailAddress = user.EmailAddress,
																													UserName = user.UserName
																												});

			return response;
		}

		public LoadUserResponse LoadUser(LoadUserRequest request)
		{
			LoadUserResponse response;
			IUser user;

			if ((object)request == null)
				throw new ArgumentNullException("request");

			if ((object)request.UserId == null)
				throw new InvalidOperationException();

			user = this.Repository.Load<IUser>(new DomainModel.Tables.User() { UserId = (int)request.UserId });

			if ((object)user == null)
				throw new InvalidOperationException("User was not found.");

			response = new LoadUserResponse()
						{
							AnswerHash = user.AnswerHash,
							CreationTimestamp = user.CreationTimestamp,
							CreationUserId = user.CreationUserId,
							EmailAddress = user.EmailAddress,
							FailedLoginCount = user.FailedLoginCount,
							LastLoginFailureTimestamp = user.LastLoginFailureTimestamp,
							LastLoginSuccessTimestamp = user.LastLoginSuccessTimestamp,
							LogicalDelete = user.LogicalDelete,
							ModificationTimestamp = user.ModificationTimestamp,
							MustChangePassword = user.MustChangePassword,
							PasswordHash = user.PasswordHash,
							Question = user.Question,
							SaltValue = user.SaltValue,
							SortOrder = user.SortOrder,
							UserId = user.UserId,
							UserName = user.UserName
						};

			return response;
		}

		public SuspendAccountResponse SuspendAccount(SuspendAccountRequest request)
		{
			SuspendAccountResponse response;
			IUser user;

			if ((object)request == null)
				throw new ArgumentNullException("request");

			if ((object)request.UserId == null)
				throw new InvalidOperationException();

			response = new SuspendAccountResponse()
						{
						};

			response.Messages = new List<Message>(); // do nothing

			if ((object)response.Messages == null)
				throw new InvalidOperationException();

			if (response.Messages.Count(m => m.Severity == Severity.Error) > 0)
				return response;

			user = this.Repository.Load<IUser>(new DomainModel.Tables.User() { UserId = (int)request.UserId });

			if ((object)user == null)
			{
				response.Messages = new[] { new Message("", "User was not found.", Severity.Error) };
				return response;
			}

			if (!user.CheckPassword(request.PasswordClearText))
			{
				response.Messages = new[] { new Message("", "The password was incorrect.", Severity.Error) };
				return response;
			}

			user.LogicalDelete = true; // suspend account (not the same as 'disable')

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

			return response;
		}

		#endregion
	}
}