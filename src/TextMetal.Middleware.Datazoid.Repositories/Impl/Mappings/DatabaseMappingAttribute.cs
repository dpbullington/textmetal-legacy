/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Mappings
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
	public sealed class DatabaseMappingAttribute : Attribute
	{
		#region Constructors/Destructors

		public DatabaseMappingAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly IList<ProcedureMappingAttribute> procedureMappingAttributes = new List<ProcedureMappingAttribute>();
		private readonly IList<TableMappingAttribute> tableMappingAttributes = new List<TableMappingAttribute>();
		private string databaseName;
		private Type targetType;
		private int versionSerial;

		#endregion

		#region Properties/Indexers/Events

		public IList<ProcedureMappingAttribute> _ProcedureMappingAttributes
		{
			get
			{
				return this.procedureMappingAttributes;
			}
		}

		public IList<TableMappingAttribute> _TableMappingAttributes
		{
			get
			{
				return this.tableMappingAttributes;
			}
		}

		public Type _TargetType
		{
			get
			{
				return this.targetType;
			}
			set
			{
				this.targetType = value;
			}
		}

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

		public int VersionSerial
		{
			get
			{
				return this.versionSerial;
			}
			set
			{
				this.versionSerial = value;
			}
		}

		#endregion
	}
}