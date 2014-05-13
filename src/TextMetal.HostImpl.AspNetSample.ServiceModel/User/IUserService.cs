/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.User
{
	public interface IUserService
	{
		#region Methods/Operators

		AuthenticateUserResponse AuthenticateUser(AuthenticateUserRequest request);

		CreateUserResponse CreateUser(CreateUserRequest request);

		EditUserResponse EditUser(EditUserRequest request);

		ForgotPasswordResponse ForgotPassword(ForgotPasswordRequest request);

		ForgotUsernameResponse ForgotUsername(ForgotUsernameRequest request);

		LoadUserResponse LoadUser(LoadUserRequest request);

		SuspendAccountResponse SuspendAccount(SuspendAccountRequest request);

		#endregion
	}
}