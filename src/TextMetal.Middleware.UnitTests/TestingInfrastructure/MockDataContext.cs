/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace TextMetal.Middleware.UnitTests.TestingInfrastructure
{
	public class MockDataContext : DataContext
	{
		#region Constructors/Destructors

		public MockDataContext(IDbConnection dbConnection, MappingSource mappingSource)
			: base(dbConnection, mappingSource)
		{
		}

		#endregion
	}
}