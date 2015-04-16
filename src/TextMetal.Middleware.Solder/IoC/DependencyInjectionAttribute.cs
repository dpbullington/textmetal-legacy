/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.IoC
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

		private string selectorKey = string.Empty;

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
				this.selectorKey = (value ?? string.Empty).Trim();
			}
		}

		#endregion
	}
}