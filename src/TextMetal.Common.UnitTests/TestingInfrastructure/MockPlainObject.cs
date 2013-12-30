/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Data.TypeMap;

namespace TextMetal.Common.UnitTests.TestingInfrastructure
{
	public class MockPlainObject : PlainObject
	{
		#region Constructors/Destructors

		public MockPlainObject()
		{
		}

		#endregion

		#region Fields/Constants

		private string firstName;
		private string lastName;
		private string middleName;
		private int? nameId;
		private string suffix;

		#endregion

		#region Properties/Indexers/Events

		public string FirstName
		{
			get
			{
				return this.firstName;
			}
			set
			{
				this.firstName = value;
			}
		}

		public override bool IsNew
		{
			get
			{
				return (object)this.nameId == null;
			}
			set
			{
				this.nameId = null;
			}
		}

		public string LastName
		{
			get
			{
				return this.lastName;
			}
			set
			{
				this.lastName = value;
			}
		}

		public string MiddleName
		{
			get
			{
				return this.middleName;
			}
			set
			{
				this.middleName = value;
			}
		}

		public string MyGetOnlyProp
		{
			get
			{
				return null;
			}
		}

		public string MySetOnlyProp
		{
			set
			{
				// do nothing
			}
		}

		public int? NameId
		{
			get
			{
				return this.nameId;
			}
			set
			{
				this.nameId = value;
			}
		}

		public string Suffix
		{
			get
			{
				return this.suffix;
			}
			set
			{
				this.suffix = value;
			}
		}

		#endregion
	}
}