/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.Runtime;

namespace TextMetal.Framework.Expression
{
	[FxSpackleTypes.SerializableAttribute]
	public enum NullaryOperator
	{
		[FxSpackleTypes.DescriptionAttribute("{nop}")]
		Nop = 0
	}
}