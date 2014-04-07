/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.SignUp
{
	public interface ISignUpService
	{
		#region Methods/Operators

		SignUpResponse SignUp(SignUpRequest request);

		#endregion
	}
}