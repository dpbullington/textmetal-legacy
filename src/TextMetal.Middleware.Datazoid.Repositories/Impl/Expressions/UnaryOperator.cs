/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Expressions
{
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