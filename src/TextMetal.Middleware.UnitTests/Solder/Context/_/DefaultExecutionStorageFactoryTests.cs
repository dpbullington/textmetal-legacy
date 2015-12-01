/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Hosting;

using NUnit.Framework;

using TextMetal.Middleware.Common.Strategies.ContextualStorage;

namespace TextMetal.Middleware.UnitTests.Common.Strategies.ContextualStorage._
{
	[TestFixture]
	public static class DefaultExecutionStorageFactoryTests
	{
		#region Classes/Structs/Interfaces/Enums/Delegates

		[TestFixture]
		public class InteractiveContextualStorageTests
		{
			#region Constructors/Destructors

			public InteractiveContextualStorageTests()
			{
			}

			#endregion

			#region Methods/Operators

			[Test]
			public void ShouldCreateAddGetRemoveCallContextExecutionPathStorageTest()
			{
				Assert.IsNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				DefaultContextualStorageFactory.Instance.GetContextualStorage().SetValue("x", 1);

				Assert.IsNotNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				Assert.AreEqual(1, DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				DefaultContextualStorageFactory.Instance.GetContextualStorage().SetValue<object>("x", null);

				Assert.IsNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				DefaultContextualStorageFactory.Instance.GetContextualStorage().RemoveValue("x");

				Assert.IsNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));
			}

			[Test]
			public void ShouldThreadSafeRunCallContextExecutionPathStorageTest()
			{
				Thread t;

				Assert.IsNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				DefaultContextualStorageFactory.Instance.GetContextualStorage().SetValue("x", 1);

				t = new Thread(this.TlsOtherThreadCallContext);
				t.Start();
				t.Join();

				Assert.IsNotNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));
				Assert.AreEqual(1, DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));
			}

			[SetUp]
			public void TestSetUp()
			{
				HttpContext.Current = null;
			}

			[TearDown]
			public void TestTearDown()
			{
				HttpContext.Current = null;
			}

			private void TlsOtherThreadCallContext()
			{
				Thread.Sleep(1000);

				Assert.IsNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				DefaultContextualStorageFactory.Instance.GetContextualStorage().SetValue("x", 2);
			}

			#endregion
		}

		[TestFixture]
		public class WebContextualStorageTests
		{
			#region Constructors/Destructors

			public WebContextualStorageTests()
			{
			}

			#endregion

			#region Methods/Operators

			[Test]
			public void ShouldCreateAddGetRemoveExecutionPathStorageTest()
			{
				Assert.IsTrue(HttpContextContextualStorageStrategy.IsInHttpContext);

				Assert.IsNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				DefaultContextualStorageFactory.Instance.GetContextualStorage().SetValue("x", 1);

				Assert.IsNotNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				Assert.AreEqual(1, DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				DefaultContextualStorageFactory.Instance.GetContextualStorage().SetValue<object>("x", null);

				Assert.IsNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				DefaultContextualStorageFactory.Instance.GetContextualStorage().RemoveValue("x");

				Assert.IsNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));
			}

			[Test]
			public void ShouldThreadSafeRunExecutionPathStorageTest()
			{
				Thread t;

				Assert.IsNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				DefaultContextualStorageFactory.Instance.GetContextualStorage().SetValue("x", 1);

				t = new Thread(this.TlsOtherThreadHttpContext);
				t.Start();
				t.Join();

				Assert.IsNotNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));
				Assert.AreEqual(1, DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));
			}

			[SetUp]
			public void TestSetUp()
			{
				TextWriter tw;
				HttpWorkerRequest wr;

				// fake up HttpContext
				tw = new StringWriter();
				wr = new SimpleWorkerRequest("/webapp", "c:\\inetpub\\wwwroot\\webapp\\", "default.aspx", string.Empty, tw);

				HttpContext.Current = new HttpContext(wr);
			}

			[TearDown]
			public void TestTearDown()
			{
				HttpContext.Current = null;
			}

			private void TlsOtherThreadHttpContext()
			{
				TextWriter tw;
				HttpWorkerRequest wr;

				// fake up HttpContext
				tw = new StringWriter();
				wr = new SimpleWorkerRequest("/webapp", "c:\\inetpub\\wwwroot\\webapp\\", "default.aspx", string.Empty, tw);
				HttpContext.Current = new HttpContext(wr);

				Thread.Sleep(1000);

				Assert.IsNull(DefaultContextualStorageFactory.Instance.GetContextualStorage().GetValue<object>("x"));

				DefaultContextualStorageFactory.Instance.GetContextualStorage().SetValue("x", 2);

				HttpContext.Current = null;
			}

			#endregion
		}

		#endregion
	}
}