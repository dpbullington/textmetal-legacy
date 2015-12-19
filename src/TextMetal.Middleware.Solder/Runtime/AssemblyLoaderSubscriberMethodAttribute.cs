/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.Runtime
{
	/// <summary>
	/// Marks a static void(void) method as an assembly loader method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public sealed class AssemblyLoaderSubscriberMethodAttribute : Attribute
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AssemblyLoaderSubscriberMethodAttribute class.
		/// </summary>
		public AssemblyLoaderSubscriberMethodAttribute()
		{
		}

		#endregion
	}
}