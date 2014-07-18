/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

namespace TextMetal.Framework.Core
{
	/// <summary>
	/// Provides for output mechanics.
	/// </summary>
	public interface IOutputMechanism : IDisposable
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the current text writer instance.
		/// </summary>
		TextWriter CurrentTextWriter
		{
			get;
		}

		/// <summary>
		/// Gets the current log text writer instance.
		/// </summary>
		TextWriter LogTextWriter
		{
			get;
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Enters (pushes) an output scope as deliniated by scope name. Scope name semantics are implementation specific.
		/// </summary>
		/// <param name="scopeName"> The scope name to push. </param>
		/// <param name="appendMode"> A value indicating whether to append or not. </param>
		void EnterScope(string scopeName, bool appendMode);

		/// <summary>
		/// Leaves (pops) an output scope as deliniated by scope name. Scope name semantics are implementation specific.
		/// </summary>
		/// <param name="scopeName"> The scope name to pop. </param>
		void LeaveScope(string scopeName);

		/// <summary>
		/// Writes a serialized object to a location specified by object name. Object name semantics are implementation specific.
		/// </summary>
		/// <param name="obj"> The object to serialize. </param>
		/// <param name="objectName"> The object name to write to. </param>
		void WriteObject(object obj, string objectName);

		#endregion
	}
}