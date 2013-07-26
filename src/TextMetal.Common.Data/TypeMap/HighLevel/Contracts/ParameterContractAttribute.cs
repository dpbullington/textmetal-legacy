/*
	Copyright ©2002-2010 D. P. Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

namespace TextMetal.Common.Data.TypeMap.Contracts
{
	/// <summary>
	/// Configures a method parameter on a command contact that is part of a database contract.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public sealed class ParameterContractAttribute : Attribute
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ParameterContractAttribute class.
		/// </summary>
		public ParameterContractAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private ParameterDirection direction;
		private string name;
		private bool nullable;
		private byte precision;
		private byte scale;
		private int size;
		private DbType type;
		private bool useInferredDirection = true;
		private bool useInferredType = true;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the direction of the database parameter.
		/// </summary>
		public ParameterDirection Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the database parameter.
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		/// <summary>
		/// Gets or sets the nullable-ness of the database parameter.
		/// </summary>
		public bool Nullable
		{
			get
			{
				return this.nullable;
			}
			set
			{
				this.nullable = value;
			}
		}

		/// <summary>
		/// Gets or sets the precision of the database parameter.
		/// </summary>
		public byte Precision
		{
			get
			{
				return this.precision;
			}
			set
			{
				this.precision = value;
			}
		}

		/// <summary>
		/// Gets or sets the scale of the database parameter.
		/// </summary>
		public byte Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		/// <summary>
		/// Gets or sets the size of the database parameter.
		/// </summary>
		public int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of the database parameter.
		/// </summary>
		public DbType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use the inferred direction of the database parameter.
		/// </summary>
		public bool UseInferredDirection
		{
			get
			{
				return this.useInferredDirection;
			}
			set
			{
				this.useInferredDirection = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use the inferred type of the database parameter.
		/// </summary>
		public bool UseInferredType
		{
			get
			{
				return this.useInferredType;
			}
			set
			{
				this.useInferredType = value;
			}
		}

		#endregion
	}
}