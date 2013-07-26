/*
	Copyright ©2002-2010 D. P. Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap.Contracts
{
	/// <summary>
	/// Configures a method return value on a command contact that is part of a database contract.
	/// </summary>
	[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
	public sealed class ReturnContractAttribute : Attribute
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ReturnContractAttribute class.
		/// </summary>
		public ReturnContractAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private string dataSetName;
		private string sourceTableName;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the name of the data set.
		/// </summary>
		public string DataSetName
		{
			get
			{
				return this.dataSetName;
			}
			set
			{
				this.dataSetName = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the source table.
		/// </summary>
		public string SourceTableName
		{
			get
			{
				return this.sourceTableName;
			}
			set
			{
				this.sourceTableName = value;
			}
		}

		#endregion
	}
}