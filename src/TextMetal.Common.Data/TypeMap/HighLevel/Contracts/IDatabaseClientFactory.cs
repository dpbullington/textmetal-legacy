/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap.Contracts
{
	/// <summary>
	/// Represents a factory capable of creating instances of database contracts.
	/// </summary>
	public interface IDatabaseClientFactory : IDisposable
	{
		#region Methods/Operators

		/// <summary>
		/// Creates a new instance of the specified database contract,
		/// using the specified IDatabase implementation for command calls.
		/// The internal caching strategy is disabled.
		/// </summary>
		/// <typeparam name="TDatabaseContract">The type of the database contract to create.</typeparam>
		/// <param name="database">The database instance to use during the execution of database operations.</param>
		/// <returns>An instance of a database contract ready for execution of operations.</returns>
		TDatabaseContract CreateInstance<TDatabaseContract>(IDatabase database) where TDatabaseContract : class;

		/// <summary>
		/// Creates a new instance of the specified database contract,
		/// using the specified connection string to create an AdoNetDatabase instance for command calls.
		/// The internal caching strategy is enabled.
		/// </summary>
		/// <typeparam name="TDatabaseContract">The type of the database contract to create.</typeparam>
		/// <param name="connectionStringName">The database connection string name
		/// (with provider name set to a database connection type name) to use during
		/// the execution of database operations.</param>
		/// <returns>An instance of a database contract ready for execution of operations.</returns>
		TDatabaseContract CreateInstance<TDatabaseContract>(string connectionStringName) where TDatabaseContract : class;

		#endregion
	}
}