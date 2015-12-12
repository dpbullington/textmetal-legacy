/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.UnitTests.TestingInfrastructure
{
	public class MockModelObject : IMockModelObject
	{
		#region Constructors/Destructors

		public MockModelObject()
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

		public object[] IdValues
		{
			get
			{
				return new object[] { this.NameId };
			}
		}

		public object NoSetter
		{
			get
			{
				return 1;
			}
		}

		public IDictionary<string, object> PropertyBag
		{
			get
			{
				throw new NotImplementedException();
			}
		}

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

		public bool IsNew
		{
			get
			{
				return this.NameId == default(int?);
			}
			set
			{
				this.NameId = default(int?);
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

		public object NoGetter
		{
			set
			{
			}
		}

		public object PersistentId
		{
			get
			{
				return this.nameId;
			}
			set
			{
				this.nameId = (int?)value;
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

		#region Methods/Operators

		public void Mark()
		{
			throw new NotImplementedException();
		}

		public virtual IEnumerable<Message> Validate()
		{
			return new Message[] { };
		}

		#endregion
	}
}