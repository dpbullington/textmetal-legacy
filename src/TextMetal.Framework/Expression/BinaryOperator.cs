/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.Runtime;

namespace TextMetal.Framework.Expression
{
	[FxSpackleTypes.SerializableAttribute]
	public enum BinaryOperator
	{
		[FxSpackleTypes.DescriptionAttribute("")]
		Undefined = 0,

		[FxSpackleTypes.DescriptionAttribute("+")]
		Add,

		[FxSpackleTypes.DescriptionAttribute("-")]
		Sub,

		[FxSpackleTypes.DescriptionAttribute("/")]
		Div,

		[FxSpackleTypes.DescriptionAttribute("*")]
		Mul,

		[FxSpackleTypes.DescriptionAttribute("%")]
		Mod,

		[FxSpackleTypes.DescriptionAttribute("&&")]
		And,

		[FxSpackleTypes.DescriptionAttribute("||")]
		Or,

		[FxSpackleTypes.DescriptionAttribute("^^")]
		Xor,

		[FxSpackleTypes.DescriptionAttribute("==")]
		Eq,

		[FxSpackleTypes.DescriptionAttribute("!=")]
		Ne,

		[FxSpackleTypes.DescriptionAttribute("<")]
		Lt,

		[FxSpackleTypes.DescriptionAttribute("<=")]
		Le,

		[FxSpackleTypes.DescriptionAttribute(">")]
		Gt,

		[FxSpackleTypes.DescriptionAttribute(">=")]
		Ge,

		[FxSpackleTypes.DescriptionAttribute("{like}")]
		StrLk,

		[FxSpackleTypes.DescriptionAttribute("{as}")]
		ObjAs,

		[FxSpackleTypes.DescriptionAttribute("{is}")]
		ObjIs,

		[FxSpackleTypes.DescriptionAttribute(":=")]
		VarPut,

		[FxSpackleTypes.DescriptionAttribute("&")]
		Band,

		[FxSpackleTypes.DescriptionAttribute("|")]
		Bor,

		[FxSpackleTypes.DescriptionAttribute("^")]
		Bxor,

		[FxSpackleTypes.DescriptionAttribute("<<")]
		Bls,

		[FxSpackleTypes.DescriptionAttribute(">>")]
		Brs,

		[FxSpackleTypes.DescriptionAttribute(">^>")]
		Bsr,

		[FxSpackleTypes.DescriptionAttribute(">_>")]
		Bur,

		[FxSpackleTypes.DescriptionAttribute("{parse}")]
		Parse,
	}
}