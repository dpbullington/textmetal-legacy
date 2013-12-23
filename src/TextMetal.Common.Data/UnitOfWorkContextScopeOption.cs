/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Common.Data
{
	/// <summary>
	/// Provides additional options for creating a unit of work context scope.
	/// </summary>
	public enum UnitOfWorkContextScopeOption
	{
		/// <summary>
		/// A unit of work context is required by the scope. It uses an ambient unit of work context if one already exists. Otherwise, it creates a new unit of work context before entering the scope. This is the default value.
		/// </summary>
		Required = 0,

		/// <summary>
		/// A new unit of work context is always created for the scope.
		/// </summary>
		RequiresNew,

		/// <summary>
		/// A new unit of work context is always created for the scope and no existing ambient unit of work context is permitted.
		/// (Differs from System.Transactions)
		/// </summary>
		RequiresNone,

		/// <summary>
		/// The ambient unit of work context is suppressed when creating the scope. All operations within the scope are done without an ambient unit of work context.
		/// </summary>
		Suppress
	}
}