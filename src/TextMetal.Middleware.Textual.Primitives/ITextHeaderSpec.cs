/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Globalization;

namespace TextMetal.Middleware.Textual.Primitives
{
	public interface ITextHeaderSpec
	{
		#region Properties/Indexers/Events

		string ValueFormat
		{
			get;
			set;
		}

		FieldType FieldType
		{
			get;
			set;
		}

		string HeaderName
		{
			get;
			set;
		}

		#endregion
	}
}