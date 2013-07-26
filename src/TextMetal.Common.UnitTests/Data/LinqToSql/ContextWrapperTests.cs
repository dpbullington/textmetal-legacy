/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock2;

using NUnit.Framework;

using TextMetal.Common.Data.LinqToSql;

namespace TextMetal.Common.UnitTests.Data.LinqToSql
{
	[TestFixture]
	public class ContextWrapperTests
	{
		#region Constructors/Destructors

		public ContextWrapperTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			ContextWrapper<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			Expect.Once.On(mockContext).Method("Dispose").WithNoArguments();

			context = new ContextWrapper<IDisposable>(mockContext);

			Assert.IsNotNull(context);
			Assert.IsNotNull(context.Context);

			Assert.IsFalse(context.Disposed);
			context.Dispose();
			Assert.IsTrue(context.Disposed);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedGetInnerContextTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			ContextWrapper<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			Expect.Once.On(mockContext).Method("Dispose").WithNoArguments();

			context = new ContextWrapper<IDisposable>(mockContext);

			Assert.IsNotNull(context);
			Assert.IsNotNull(context.Context);

			Assert.IsFalse(context.Disposed);
			context.Dispose();
			Assert.IsTrue(context.Disposed);

			mockContext = context.Context;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullContextCreateTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			ContextWrapper<IDisposable> context;

			mockery = new Mockery();
			mockContext = null;

			context = new ContextWrapper<IDisposable>(mockContext);
		}

		[Test]
		public void ShouldNotFailOnDoubleDisposeTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			ContextWrapper<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			Expect.Once.On(mockContext).Method("Dispose").WithNoArguments();

			context = new ContextWrapper<IDisposable>(mockContext);

			Assert.IsNotNull(context);
			Assert.IsNotNull(context.Context);

			Assert.IsFalse(context.Disposed);
			context.Dispose();
			Assert.IsTrue(context.Disposed);
			context.Dispose();
			Assert.IsTrue(context.Disposed);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}