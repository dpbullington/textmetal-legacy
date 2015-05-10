/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Mappings
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public sealed class KeyConstraintColumnMappingAttribute : Attribute
	{
		#region Constructors/Destructors

		public KeyConstraintColumnMappingAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private int keyConstraintColumnOrdinal;
		private PropertyInfo targetProperty;

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

		public int KeyConstraintColumnOrdinal
		{
			get
			{
				return this.keyConstraintColumnOrdinal;
			}
			set
			{
				this.keyConstraintColumnOrdinal = value;
			}
		}

		#endregion
	}
}