using System;

namespace Newtonsoft.Json.Serialization
{
	/// <summary>
	/// Represents a trace writer.
	/// </summary>
	public interface ITraceWriter
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the <see cref="TraceLevel" /> that will be used to filter the trace messages passed to the writer.
		/// For example a filter level of <code>Info</code> will exclude <code>Verbose</code> messages and include <code>Info</code>,
		/// <code>Warning</code> and <code>Error</code> messages.
		/// </summary>
		/// <value> The <see cref="TraceLevel" /> that will be used to filter the trace messages passed to the writer. </value>
		TraceLevel LevelFilter
		{
			get;
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Writes the specified trace level, message and optional exception.
		/// </summary>
		/// <param name="level"> The <see cref="TraceLevel" /> at which to write this trace. </param>
		/// <param name="message"> The trace message. </param>
		/// <param name="ex"> The trace exception. This parameter is optional. </param>
		void Trace(TraceLevel level, string message, Exception ex);

		#endregion
	}
}