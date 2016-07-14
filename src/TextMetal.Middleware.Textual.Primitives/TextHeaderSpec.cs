/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Globalization;

namespace TextMetal.Middleware.Textual.Primitives
{
	public class TextHeaderSpec : ITextHeaderSpec
	{
		#region Constructors/Destructors

		public TextHeaderSpec()
		{
		}

		#endregion

		#region Fields/Constants

		private string valueFormat;
		private FieldType fieldType;
		private string headerName;

		#endregion

		#region Properties/Indexers/Events

		public string ValueFormat
		{
			get
			{
				return this.valueFormat;
			}
			set
			{
				this.valueFormat = value;
			}
		}

		public FieldType FieldType
		{
			get
			{
				return this.fieldType;
			}
			set
			{
				this.fieldType = value;
			}
		}

		public string HeaderName
		{
			get
			{
				return this.headerName;
			}
			set
			{
				this.headerName = value;
			}
		}

		#endregion
	}
}