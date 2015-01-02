/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
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

		object NoSetter
		{
			get;
		}

		#endregion
	}
}