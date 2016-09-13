/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Commercial software distribution. May contain open source.
*/

using System;

namespace TextMetal.Ox.SSIS.ConsoleTool.Config
{
	/// <summary>
	/// The set of database objects supported by this framework.
	/// </summary>
	public enum ObjectType
	{
		Unknown = 0,

		Table,

		View,

		ProcedureRequest,

		ProcedureResult,

		ProcedureResponse
	}
}