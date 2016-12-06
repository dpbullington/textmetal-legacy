/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Textual.Primitives
{
	public interface ITextHeaderSpec
	{
		#region Properties/Indexers/Events

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

		string ValueFormat
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		Type GetClrTypeFromFieldType();

		#endregion
	}
}