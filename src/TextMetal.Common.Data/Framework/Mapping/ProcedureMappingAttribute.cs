/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.Framework.Mapping
{
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public sealed class ProcedureMappingAttribute : Attribute
	{
		#region Constructors/Destructors

		public ProcedureMappingAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private string databaseName;
		private bool isFunction;
		private string procedureName;
		private string schemaName;

		#endregion

		#region Properties/Indexers/Events

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
			set
			{
				this.databaseName = value;
			}
		}

		public bool IsFunction
		{
			get
			{
				return this.isFunction;
			}
			set
			{
				this.isFunction = value;
			}
		}

		public string ProcedureName
		{
			get
			{
				return this.procedureName;
			}
			set
			{
				this.procedureName = value;
			}
		}

		public string SchemaName
		{
			get
			{
				return this.schemaName;
			}
			set
			{
				this.schemaName = value;
			}
		}

		#endregion
	}
}