﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Framework.Hosting.Email
{
	public interface ITextMetalEmailHost : ITextMetalHost
	{
		#region Methods/Operators

		EmailMessage Host(bool strictMatching, EmailTemplate emailTemplate, object modelObject);

		#endregion
	}
}