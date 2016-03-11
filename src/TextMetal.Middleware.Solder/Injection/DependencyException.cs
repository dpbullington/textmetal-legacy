/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

/* CERTIFICATION OF UNIT TESTING: dpbullington@gmail.com / 2016-03-10 / 100% code coverage */

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// The exception thrown when a specific dependency resolution error occurs.
	/// </summary>
	public sealed class DependencyException : Exception
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the DependencyException class.
		/// </summary>
		public DependencyException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the DependencyException class.
		/// </summary>
		/// <param name="message"> The message that describes the error. </param>
		public DependencyException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the DependencyException class.
		/// </summary>
		/// <param name="message"> The message that describes the error. </param>
		/// <param name="innerException"> The inner exception. </param>
		public DependencyException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		#endregion
	}
}