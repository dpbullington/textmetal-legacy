/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.DomainModel.Tables
{
	public partial interface IUser
	{
		#region Methods/Operators

		bool CheckPassword(string password);

		void SetAnswer(string answer);

		void SetPassword(string password);

		#endregion
	}
}