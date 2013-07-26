// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.UiException.CodeFormatters
{
	/// <summary>
	/// 	ICodeFormatter is the interface to make the syntax
	/// 	coloring of a string for a specific developpment language.
	/// </summary>
	public interface ICodeFormatter
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// 	The language name handled by this formatter.
		/// 	Ex: "C#", "Java", "C++" and so on...
		/// </summary>
		string Language
		{
			get;
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Makes the coloring syntax of the given text.
		/// </summary>
		/// <param name="code"> The text to be formatted. This parameter cannot be null. </param>
		/// <returns> A FormattedCode instance. </returns>
		FormattedCode Format(string code);

		#endregion
	}
}