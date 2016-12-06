/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using TextMetal.Middleware.Datazoid.Models.Tabular;

namespace TextMetal.Middleware.UnitTests.TestingInfrastructure
{
	public interface IMockModelObject : ITableModelObject
	{
		#region Properties/Indexers/Events

		object NoSetter
		{
			get;
		}

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

		#endregion
	}
}