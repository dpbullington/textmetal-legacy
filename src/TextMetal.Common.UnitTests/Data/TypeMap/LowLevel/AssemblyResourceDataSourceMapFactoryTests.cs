/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Common.Data.TypeMap.LowLevel;
using TextMetal.Common.UnitTests.TestingInfrastructure;

namespace TextMetal.Common.UnitTests.Data.TypeMap.LowLevel
{
	[TestFixture]
	public class AssemblyResourceDataSourceMapFactoryTests
	{
		#region Constructors/Destructors

		public AssemblyResourceDataSourceMapFactoryTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			AssemblyResourceDataSourceMapFactory dataSourceMapFactory;
			DataSourceMap dataSourceMap;

			dataSourceMapFactory = new AssemblyResourceDataSourceMapFactory();

			Assert.IsNotNull(dataSourceMapFactory);

			dataSourceMap = dataSourceMapFactory.GetMap<MockPlainObject>(null);

			Assert.IsNotNull(dataSourceMap);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnNoManifestResourceMappingCreateTest()
		{
			AssemblyResourceDataSourceMapFactory dataSourceMapFactory;
			DataSourceMap dataSourceMap;

			dataSourceMapFactory = new AssemblyResourceDataSourceMapFactory();

			Assert.IsNotNull(dataSourceMapFactory);

			dataSourceMap = dataSourceMapFactory.GetMap<MockPlainObjectNoMap>(null);
		}

		#endregion
	}
}