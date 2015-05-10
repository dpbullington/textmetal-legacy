/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextMetal.Framework.InputOutput
{
	public abstract class OutputMechanism : IOutputMechanism
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the OutputMechanism class.
		/// </summary>
		protected OutputMechanism()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly Stack<TextWriter> textWriters = new Stack<TextWriter>();
		private bool disposed;
		private TextWriter logTextWriter = Console.Out;

		#endregion

		#region Properties/Indexers/Events

		public TextWriter CurrentTextWriter
		{
			get
			{
				return this.TextWriters.Count > 0 ? this.TextWriters.Peek() : Console.Out;
			}
		}

		protected Stack<TextWriter> TextWriters
		{
			get
			{
				return this.textWriters;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current instance has been disposed.
		/// </summary>
		public bool Disposed
		{
			get
			{
				return this.disposed;
			}
			private set
			{
				this.disposed = value;
			}
		}

		public TextWriter LogTextWriter
		{
			get
			{
				return this.logTextWriter;
			}
			private set
			{
				this.logTextWriter = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected abstract void CoreEnter(string scopeName, bool appendMode, Encoding encoding);

		protected abstract void CoreLeave(string scopeName);

		protected abstract void CoreWriteObject(object obj, string objectName);

		/// <summary>
		/// Dispose of the data source transaction.
		/// </summary>
		public void Dispose()
		{
			if (this.Disposed)
				return;

			try
			{
				if ((object)this.TextWriters != null)
				{
					foreach (TextWriter textWriter in this.TextWriters)
					{
						textWriter.Flush();
						textWriter.Dispose();
					}

					this.TextWriters.Clear();
				}

				if ((object)this.LogTextWriter != null)
				{
					this.LogTextWriter.Flush();
					this.LogTextWriter.Dispose();
					this.LogTextWriter = null;
				}
			}
			finally
			{
				this.Disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Enters (pushes) an output scope as deliniated by scope name. Scope name semantics are implementation specific.
		/// </summary>
		/// <param name="scopeName"> The scope name to push. </param>
		/// <param name="appendMode"> A value indicating whether to append or not. </param>
		/// <param name="encoding"> A text encoding in-effect for this new scope. </param>
		public void EnterScope(string scopeName, bool appendMode, Encoding encoding)
		{
			this.CoreEnter(scopeName, appendMode, encoding);
		}

		/// <summary>
		/// Leaves (pops) an output scope as deliniated by scope name. Scope name semantics are implementation specific.
		/// </summary>
		/// <param name="scopeName"> The scope name to pop. </param>
		public void LeaveScope(string scopeName)
		{
			this.CoreLeave(scopeName);
		}

		protected void SetLogTextWriter(TextWriter activeLogTextWriter)
		{
			if ((object)this.LogTextWriter != null &&
				(object)this.LogTextWriter != activeLogTextWriter)
			{
				this.LogTextWriter.Flush();
				this.LogTextWriter.Dispose();
				this.LogTextWriter = null;
			}

			this.LogTextWriter = activeLogTextWriter ?? Console.Out;
		}

		/// <summary>
		/// Writes a serialized object to a location specified by object name. Object name semantics are implementation specific.
		/// </summary>
		/// <param name="obj"> The object to serialize. </param>
		/// <param name="objectName"> The object name to write to. </param>
		public void WriteObject(object obj, string objectName)
		{
			this.CoreWriteObject(obj, objectName);
		}

		#endregion
	}
}