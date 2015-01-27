/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Globalization;

namespace TextMetal.Common.Core
{
	public sealed class HeaderSpec
	{
		#region Constructors/Destructors

		public HeaderSpec()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		public NumberFormatInfo FieldNumberFormatSpec
		{
			get;
			set;
		}

		public FieldType FieldType
		{
			get;
			set;
		}

		public string HeaderName
		{
			get;
			set;
		}

		#endregion
	}
}