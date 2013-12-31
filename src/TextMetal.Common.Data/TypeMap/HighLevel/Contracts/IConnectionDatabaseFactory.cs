/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap.Contracts
{
	/// <summary>
	/// Provides a factory for database instances which
	/// require a connection string and a connection type.
	/// </summary>
	public interface IConnectionDatabaseFactory
	{
		#region Methods/Operators

		/// <summary>
		/// Gets a database instance using the connection string and connection type.
		/// </summary>
		/// <param name="connectionString">The database connection string.</param>
		/// <param name="connectionType">The database connection type.</param>
		/// <returns>A database instance.</returns>
		IDatabase GetDatabase(string connectionString, Type connectionType);

		#endregion
	}
}