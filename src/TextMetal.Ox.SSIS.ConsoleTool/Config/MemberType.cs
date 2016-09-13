/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Commercial software distribution. May contain open source.
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