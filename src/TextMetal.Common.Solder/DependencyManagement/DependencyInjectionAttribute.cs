/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Solder.DependencyManagement
{
	/// <summary>
	/// Marks a constructor parameter as a dependency injection point.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public sealed class DependencyInjectionAttribute : Attribute
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the DependencyInjectionAttribute class.
		/// </summary>
		public DependencyInjectionAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private string selectorKey = "";

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the dependency selector key.
		/// </summary>
		public string SelectorKey
		{
			get
			{
				return this.selectorKey;
			}
			set
			{
				this.selectorKey = (value ?? "").Trim();
			}
		}

		#endregion
	}
}