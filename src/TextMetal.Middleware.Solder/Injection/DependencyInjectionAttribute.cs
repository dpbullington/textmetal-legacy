/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// Marks a constructor and its parameter(s) or properties as dependency injection points.
	/// When specified on a constructor, the selector key is used to decide which constructor is resolved.
	/// When specified on a constructor parameter or a proeprty, the selector key is used to resolve that actual value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
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