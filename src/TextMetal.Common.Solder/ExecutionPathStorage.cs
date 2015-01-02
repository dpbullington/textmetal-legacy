/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Diagnostics;

using TextMetal.Common.Solder.AmbientExecutionContext;

namespace TextMetal.Common.Solder
{
	/// <summary>
	/// Manages execution path storage of objects in a manner which is safe in standard executables and libraries and ASP.NET code.
	/// </summary>
	public static class ExecutionPathStorage
	{
		#region Methods/Operators

		private static IExecutionPathStorage GetExecutionPathStorage()
		{
			IExecutionPathStorage executionPathStorage;

			// TODO: support others via DI or app.config

			if (HttpContextExecutionPathStorage.IsInHttpContext)
				executionPathStorage = new HttpContextExecutionPathStorage();
			else
				executionPathStorage = new ThreadStaticExecutionPathStorage();

			//Trace.WriteLine(string.Format("GetExecutionPathStorage(): '{0}'", executionPathStorage.GetType().FullName));

			return executionPathStorage;
		}

		/// <summary>
		/// Gets a named value from the current execution context storage mechanism.
		/// </summary>
		/// <param name="key"> The key to lookup in execution path storage. </param>
		/// <returns> An object value or null. </returns>
		public static T GetValue<T>(string key)
		{
			return GetExecutionPathStorage().GetValue<T>(key);
		}

		/// <summary>
		/// Checks the existence of a named value in the current execution context storage mechanism.
		/// </summary>
		/// <param name="key"> The key to lookup in execution path storage. </param>
		/// <returns>A value indicating whether the key exists in the current execution context storage mechanism.</returns>
		public static bool HasValue(string key)
		{
			return GetExecutionPathStorage().HasValue(key);
		}

		/// <summary>
		/// Remove a named value from the current execution context storage mechanism.
		/// </summary>
		/// <param name="key"> The key to remove in execution path storage. </param>
		public static void RemoveValue(string key)
		{
			GetExecutionPathStorage().RemoveValue(key);
		}

		/// <summary>
		/// Set a named value in the current execution context storage mechanism.
		/// </summary>
		/// <param name="key"> The key to store in execution path storage. </param>
		/// <param name="value"> An object instance or null. </param>
		public static void SetValue<T>(string key, T value)
		{
			GetExecutionPathStorage().SetValue<T>(key, value);
		}

		#endregion
	}
}