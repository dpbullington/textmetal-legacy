using System;

/* CERTIFICATION OF UNIT TESTING: dpbullington@gmail.com / 2016-04-17 / 100% code coverage */

namespace TextMetal.Middleware.Solder.Utilities
{
	/// <summary>
	/// The exception thrown when a application configuration error occurs.
	/// </summary>
	public sealed class AppConfigException : Exception
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AppConfigException class.
		/// </summary>
		public AppConfigException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the AppConfigException class.
		/// </summary>
		/// <param name="message"> The message that describes the error. </param>
		public AppConfigException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the AppConfigException class.
		/// </summary>
		/// <param name="message"> The message that describes the error. </param>
		/// <param name="innerException"> The inner exception. </param>
		public AppConfigException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		#endregion
	}
}