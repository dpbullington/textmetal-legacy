/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

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

		private FieldType fieldType;
		private string headerName;

		private string valueFormat;

		#endregion

		#region Properties/Indexers/Events

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

		#endregion

		#region Methods/Operators

		public Type GetClrTypeFromFieldType()
		{
			switch (fieldType)
			{
				case FieldType.String:
					return typeof(String);
				case FieldType.Number:
					return typeof(Double?);
				case FieldType.DateTime:
					return typeof(DateTime?);
				case FieldType.TimeSpan:
					return typeof(TimeSpan?);
				case FieldType.Boolean:
					return typeof(Boolean?);
				default:
					return null;
			}
		}

		#endregion
	}
}