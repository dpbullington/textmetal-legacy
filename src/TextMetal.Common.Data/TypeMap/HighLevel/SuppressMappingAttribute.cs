/*
	Copyright ©2002-2010 Daniel Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap.HighLevel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class SuppressMappingAttribute : Attribute
	{
		#region Constructors/Destructors

		public SuppressMappingAttribute()
		{
		}

		#endregion
	}
}