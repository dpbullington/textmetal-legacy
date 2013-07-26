/*
	Copyright ©2002-2010 Daniel Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap.HighLevel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class TableMappingAttribute : Attribute
	{
		#region Constructors/Destructors

		public TableMappingAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private bool isView;
		private string name;
		private string schema;
		private Type targetType;

		#endregion

		#region Properties/Indexers/Events

		public bool IsView
		{
			get
			{
				return this.isView;
			}
			set
			{
				this.isView = value;
			}
		}

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

		public string Schema
		{
			get
			{
				return this.schema;
			}
			set
			{
				this.schema = value;
			}
		}

		public Type TargetType
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

		#endregion
	}
}