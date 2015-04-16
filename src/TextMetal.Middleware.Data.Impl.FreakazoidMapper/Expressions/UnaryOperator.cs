/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Expressions
{
	[Serializable]
	public enum UnaryOperator
	{
		Undefined = 0,

		Not,

		IsNull,

		IsNotNull,

		Neg,

		Pos
	}
}