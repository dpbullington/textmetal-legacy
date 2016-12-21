/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Ox.SSIS.ConsoleTool.Config
{
	/// <summary>
	/// The set of database members supported by this framework.
	/// </summary>
	public enum MemberType
	{
		Unknown = 0,

		Column,

		Parameter
	}
}