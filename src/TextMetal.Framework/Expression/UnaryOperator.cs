/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.Runtime;

namespace TextMetal.Framework.Expression
{
	[FxSpackleTypes.SerializableAttribute]
	public enum UnaryOperator
	{
		[FxSpackleTypes.DescriptionAttribute("")]
		Undefined = 0,

		[FxSpackleTypes.DescriptionAttribute("!")]
		Not,

		[FxSpackleTypes.DescriptionAttribute("{is_null}")]
		IsNull,

		[FxSpackleTypes.DescriptionAttribute("{is_not_null}")]
		IsNotNull, // yes, it is redundant

		[FxSpackleTypes.DescriptionAttribute("{is_defined}")]
		IsDef,

		[FxSpackleTypes.DescriptionAttribute("-")]
		Neg,

		[FxSpackleTypes.DescriptionAttribute("+")]
		Pos,

		[FxSpackleTypes.DescriptionAttribute("++")]
		Incr,

		[FxSpackleTypes.DescriptionAttribute("--")]
		Decr,

		[FxSpackleTypes.DescriptionAttribute("~")]
		BComp
	}
}