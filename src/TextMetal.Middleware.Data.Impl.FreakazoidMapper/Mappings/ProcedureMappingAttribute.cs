/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Mappings
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ProcedureMappingAttribute : Attribute
	{
		#region Constructors/Destructors

		public ProcedureMappingAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly IList<ParameterMappingAttribute> inputParameterMappingAttributes = new List<ParameterMappingAttribute>();
		private readonly IList<ParameterMappingAttribute> outputParameterMappingAttributes = new List<ParameterMappingAttribute>();
		private readonly IList<ColumnMappingAttribute> resultColumnMappingAttributes = new List<ColumnMappingAttribute>();
		private string databaseName;
		private bool isFunction;
		private string procedureName;
		private string schemaName;
		private Type targetType;

		#endregion

		#region Properties/Indexers/Events

		public IList<ParameterMappingAttribute> _InputParameterMappingAttributes
		{
			get
			{
				return this.inputParameterMappingAttributes;
			}
		}

		public IList<ParameterMappingAttribute> _OutputParameterMappingAttributes
		{
			get
			{
				return this.outputParameterMappingAttributes;
			}
		}

		public IList<ColumnMappingAttribute> _ResultColumnMappingAttributes
		{
			get
			{
				return this.resultColumnMappingAttributes;
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