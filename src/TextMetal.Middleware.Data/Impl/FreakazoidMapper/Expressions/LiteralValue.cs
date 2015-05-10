/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Expressions
{
	/// <summary>
	/// Represents a literal value.
	/// </summary>
	[Serializable]
	public sealed class LiteralValue : IExpression
	{
		#region Constructors/Destructors

		public LiteralValue(object _)
		{
			this._ = _;
		}

		#endregion

		#region Fields/Constants

		private readonly object _;

		#endregion

		#region Properties/Indexers/Events

		public object __
		{
			get
			{
				return this._;
			}
		}

		#endregion
	}
}