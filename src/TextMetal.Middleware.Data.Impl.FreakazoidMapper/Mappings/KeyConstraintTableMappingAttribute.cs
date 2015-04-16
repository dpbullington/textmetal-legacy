/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Mappings
{
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	public sealed class KeyConstraintTableMappingAttribute : Attribute
	{
		#region Constructors/Destructors

		public KeyConstraintTableMappingAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly IList<KeyConstraintColumnMappingAttribute> keyConstraintColumnMappingAttributes = new List<KeyConstraintColumnMappingAttribute>();
		private string keyConstraintName;
		private int keyConstraintTableOrdinal;
		private KeyConstraintType keyConstraintType;
		private Type targetType;

		#endregion

		#region Properties/Indexers/Events

		public IList<KeyConstraintColumnMappingAttribute> _KeyConstraintColumnMappingAttributes
		{
			get
			{
				return this.keyConstraintColumnMappingAttributes;
			}
		}

		public string KeyConstraintName
		{
			get
			{
				return this.keyConstraintName;
			}
			set
			{
				this.keyConstraintName = value;
			}
		}

		public int KeyConstraintTableOrdinal
		{
			get
			{
				return this.keyConstraintTableOrdinal;
			}
			set
			{
				this.keyConstraintTableOrdinal = value;
			}
		}

		public KeyConstraintType KeyConstraintType
		{
			get
			{
				return this.keyConstraintType;
			}
			set
			{
				this.keyConstraintType = value;
			}
		}

		#endregion
	}
}