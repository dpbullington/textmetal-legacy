/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Common.UnitTests.TestingInfrastructure
{
	public interface IMockModelObject
	{
		#region Properties/Indexers/Events

		string FirstName
		{
			get;
			set;
		}

		string LastName
		{
			get;
			set;
		}

		string MiddleName
		{
			get;
			set;
		}

		int? NameId
		{
			get;
			set;
		}

		object NoGetter
		{
			set;
		}

		object NoSetter
		{
			get;
		}

		object PersistentId
		{
			get;
			set;
		}

		string Suffix
		{
			get;
			set;
		}

		#endregion
	}
}