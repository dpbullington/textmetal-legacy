/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Common.Data.Framework.Syntax.Expressions
{
	/// <summary>
	/// Represents a sequence.
	/// </summary>
	public interface ISequence
	{
		#region Properties/Indexers/Events

		bool? SortDirection
		{
			get;
		}

		IExpression SortExpression
		{
			get;
		}

		#endregion
	}
}