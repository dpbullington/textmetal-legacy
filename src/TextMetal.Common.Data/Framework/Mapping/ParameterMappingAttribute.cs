/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.Reflection;

namespace TextMetal.Common.Data.Framework.Mapping
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ParameterMappingAttribute : Attribute
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ParameterMappingAttribute class.
		/// </summary>
		public ParameterMappingAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private DbType parameterDbType;
		private ParameterDirection parameterDirection;
		private string parameterName;
		private bool parameterNullable;
		private int parameterOrdinal;
		private byte parameterPrecision;
		private byte parameterScale;
		private int parameterSize;
		private string parameterSqlType;
		private PropertyInfo targetProperty;
		private Type targetType;

		#endregion

		#region Properties/Indexers/Events

		public PropertyInfo _TargetProperty
		{
			get
			{
				return this.targetProperty;
			}
			set
			{
				this.targetProperty = value;
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

		/// <summary>
		/// Gets or sets the type of the parameter.
		/// </summary>
		public DbType ParameterDbType
		{
			get
			{
				return this.parameterDbType;
			}
			set
			{
				this.parameterDbType = value;
			}
		}

		/// <summary>
		/// Gets or sets the direction of the parameter.
		/// </summary>
		public ParameterDirection ParameterDirection
		{
			get
			{
				return this.parameterDirection;
			}
			set
			{
				this.parameterDirection = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the parameter.
		/// </summary>
		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
			set
			{
				this.parameterName = value;
			}
		}

		/// <summary>
		/// Gets or sets the nullable-ness of the parameter.
		/// </summary>
		public bool ParameterNullable
		{
			get
			{
				return this.parameterNullable;
			}
			set
			{
				this.parameterNullable = value;
			}
		}

		public int ParameterOrdinal
		{
			get
			{
				return this.parameterOrdinal;
			}
			set
			{
				this.parameterOrdinal = value;
			}
		}

		/// <summary>
		/// Gets or sets the precision of the parameter.
		/// </summary>
		public byte ParameterPrecision
		{
			get
			{
				return this.parameterPrecision;
			}
			set
			{
				this.parameterPrecision = value;
			}
		}

		/// <summary>
		/// Gets or sets the scale of the parameter.
		/// </summary>
		public byte ParameterScale
		{
			get
			{
				return this.parameterScale;
			}
			set
			{
				this.parameterScale = value;
			}
		}

		/// <summary>
		/// Gets or sets the size of the parameter.
		/// </summary>
		public int ParameterSize
		{
			get
			{
				return this.parameterSize;
			}
			set
			{
				this.parameterSize = value;
			}
		}

		public string ParameterSqlType
		{
			get
			{
				return this.parameterSqlType;
			}
			set
			{
				this.parameterSqlType = value;
			}
		}

		#endregion
	}
}