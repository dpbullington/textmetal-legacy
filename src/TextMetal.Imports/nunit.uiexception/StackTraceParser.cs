// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

using NUnit.UiException.StackTraceAnalysers;
using NUnit.UiException.StackTraceAnalyzers;

namespace NUnit.UiException
{
	/// <summary>
	/// 	StackTraceParser is the entry class for analyzing and converting a stack
	/// 	trace - as given by .NET - into a manageable and ordered set of ErrorItem
	/// 	instances.
	/// 	StackTraceParser contains internaly a set of autonom, independent and
	/// 	interchangeable algorithms that makes the analysis of the stack robust and
	/// 	opened to changes. Its architecture is designed to abstract callers from
	/// 	secondary details such as the type of culture or file system that can
	/// 	both affect the format of the final stack as provided by .NET.
	/// 	In the future, this class could easily be extended by exposing a
	/// 	kind of register() method that would allow client code to append
	/// 	new algorithms of analysis in its internal list.
	/// </summary>
	public class StackTraceParser
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Build a new instance of StackTraceParser.
		/// </summary>
		public StackTraceParser()
		{
			this._items = new ErrorItemCollection();

			this._functionParsers = new FunctionParser();
			this._pathParsers = new PathCompositeParser();
			this._lineNumberParsers = new LineNumberParser();

			return;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// 	One or more algorithms designed to locate function names
		/// 	inside a stack trace line.
		/// </summary>
		private IErrorParser _functionParsers;

		/// <summary>
		/// 	Output list build from the StackTrace analyze .
		/// </summary>
		private ErrorItemCollection _items;

		/// <summary>
		/// 	One or more algorithms designed to locate line number
		/// 	information inside a stack strace line.
		/// </summary>
		private IErrorParser _lineNumberParsers;

		/// <summary>
		/// 	One or more algorithms designed to locate path names
		/// 	inside a stack strace line.
		/// </summary>
		private IErrorParser _pathParsers;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Gives access to the collection of ErrorItem
		/// 	build during the analyze of the StackTrace.
		/// </summary>
		public ErrorItemCollection Items
		{
			get
			{
				return (this._items);
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Reads and transforms the given stack trace into a manageable and ordered
		/// 	set of ErrorItem instances. The resulting set is stored into Items property.
		/// </summary>
		/// <param name="stackTrace"> A string value that should contain a .Net stack trace. </param>
		public void Parse(string stackTrace)
		{
			DefaultTextManager lines;
			RawError rawError;

			this._items.Clear();

			lines = new DefaultTextManager();
			lines.Text = stackTrace;

			foreach (string line in lines)
			{
				rawError = new RawError(line);

				if (!this._functionParsers.TryParse(this, rawError))
					continue;

				this._pathParsers.TryParse(this, rawError);
				this._lineNumberParsers.TryParse(this, rawError);

				this._items.Add(rawError.ToErrorItem());
			}

			return;
		}

		#endregion
	}
}