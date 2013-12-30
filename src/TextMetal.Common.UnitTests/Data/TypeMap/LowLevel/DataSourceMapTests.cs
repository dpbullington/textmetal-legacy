/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Xml.Serialization;

using NUnit.Framework;

using TextMetal.Common.Data.TypeMap.LowLevel;

namespace TextMetal.Common.UnitTests.Data.TypeMap.LowLevel
{
	[TestFixture]
	public class DataSourceMapTests
	{
		#region Constructors/Destructors

		public DataSourceMapTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			DataSourceMap dataSourceMap;

			dataSourceMap = new DataSourceMap();

			Assert.IsNotNull(dataSourceMap);

			Assert.IsNull(dataSourceMap.Insert);
			dataSourceMap.Insert = new Command();
			Assert.IsNotNull(dataSourceMap.Insert);

			Assert.IsNull(dataSourceMap.Update);
			dataSourceMap.Update = new Command();
			Assert.IsNotNull(dataSourceMap.Update);

			Assert.IsNull(dataSourceMap.Delete);
			dataSourceMap.Delete = new Command();
			Assert.IsNotNull(dataSourceMap.Delete);

			Assert.IsNull(dataSourceMap.SelectAll);
			dataSourceMap.SelectAll = new Command();
			Assert.IsNotNull(dataSourceMap.SelectAll);

			Assert.IsNull(dataSourceMap.SelectOne);
			dataSourceMap.SelectOne = new Command();
			Assert.IsNotNull(dataSourceMap.SelectOne);

			Assert.IsNotNull(dataSourceMap.SelectFors);
		}

		[Test]
		public void ShouldXmlTest()
		{
			XmlSerializer xmlSerializer;
			DataSourceMap dataSourceMap;
			StringReader sr;
			StringWriter sw;

			string xml;

			dataSourceMap = new DataSourceMap();

			Assert.IsNotNull(dataSourceMap);

			dataSourceMap.Insert = new Command();

			dataSourceMap.Insert.Fields.Add(new Field());
			dataSourceMap.Insert.Fields.Add(new Field());
			dataSourceMap.Insert.Fields.Add(new Field());

			dataSourceMap.Insert.Parameters.Add(new Parameter());
			dataSourceMap.Insert.Parameters.Add(new Parameter());
			dataSourceMap.Insert.Parameters.Add(new Parameter());

			dataSourceMap.Update = new Command();
			dataSourceMap.Delete = new Command();
			dataSourceMap.SelectAll = new Command();
			dataSourceMap.SelectOne = new Command();

			dataSourceMap.SelectFors.Add(new For()
										{
											Command = new Command()
										});

			xmlSerializer = new XmlSerializer(typeof(DataSourceMap));

			using (sw = new StringWriter())
			{
				xmlSerializer.Serialize(sw, dataSourceMap);
				xml = sw.ToString();
				Console.WriteLine(xml);
			}

			sw = null;

			xmlSerializer = new XmlSerializer(typeof(DataSourceMap));

			using (sr = new StringReader(xml))
				dataSourceMap = (DataSourceMap)xmlSerializer.Deserialize(sr);

			sr = null;

			Assert.IsNotNull(dataSourceMap);
		}

		#endregion
	}
}