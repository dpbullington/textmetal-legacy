/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap.Contracts
{
	/// <summary>
	/// Indicates that an interface defines a database contract.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public sealed class DatabaseContractAttribute : Attribute
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the DatabaseContractAttribute class.
		/// </summary>
		public DatabaseContractAttribute()
		{
		}

		#endregion
	}
}