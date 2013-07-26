// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.UiException.StackTraceAnalyzers
{
	public class RawError
	{
		#region Constructors/Destructors

		public RawError(string input)
		{
			UiExceptionHelper.CheckNotNull(input, "input");
			this._input = input;

			return;
		}

		#endregion

		#region Fields/Constants

		private string _function;
		private string _input;
		private int _line;
		private string _path;

		#endregion

		#region Properties/Indexers/Events

		public string Function
		{
			get
			{
				return (this._function);
			}
			set
			{
				this._function = value;
			}
		}

		public string Input
		{
			get
			{
				return (this._input);
			}
		}

		public int Line
		{
			get
			{
				return (this._line);
			}
			set
			{
				this._line = value;
			}
		}

		public string Path
		{
			get
			{
				return (this._path);
			}
			set
			{
				this._path = value;
			}
		}

		#endregion

		#region Methods/Operators

		public ErrorItem ToErrorItem()
		{
			UiExceptionHelper.CheckTrue(
				this._function != null,
				"Cannot create instance of ErrorItem without a valid value in Function",
				"Function");

			return (new ErrorItem(this._path, this._function, this._line));
		}

		#endregion
	}

	public interface IErrorParser
	{
		#region Methods/Operators

		bool TryParse(StackTraceParser parser, RawError args);

		#endregion
	}
}