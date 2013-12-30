/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock2;

using NUnit.Framework;

using TextMetal.Common.Data.LinqToSql;

namespace TextMetal.Common.UnitTests.Data.LinqToSql
{
	[TestFixture]
	public class MulticastContextTests
	{
		#region Constructors/Destructors

		public MulticastContextTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			Mockery mockery;
			IDisposable mockContext, storedContext;
			MulticastContext<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			Expect.Once.On(mockContext).Method("Dispose").WithNoArguments();

			context = new MulticastContext<IDisposable>();

			Assert.IsNotNull(context);

			Assert.IsFalse(context.HasContext(typeof(IDisposable)));
			context.SetContext(typeof(IDisposable), mockContext);
			Assert.IsTrue(context.HasContext(typeof(IDisposable)));
			storedContext = context.GetContext(typeof(IDisposable));

			Assert.AreSame(mockContext, storedContext);

			Assert.IsFalse(context.Disposed);
			context.Dispose();
			Assert.IsTrue(context.Disposed);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnActualInstanceSetContextTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			MulticastContext<IDisposable> context;

			mockery = new Mockery();
			mockContext = null;

			context = new MulticastContext<IDisposable>();

			Assert.IsNotNull(context);

			context.SetContext(typeof(IDisposable), mockContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnActualTypeHasContextTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			MulticastContext<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			context = new MulticastContext<IDisposable>();

			Assert.IsNotNull(context);

			context.HasContext(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnActualTypeSetContextTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			MulticastContext<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			context = new MulticastContext<IDisposable>();

			Assert.IsNotNull(context);

			context.SetContext(null, mockContext);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnAlreadyRegisteredSetContextTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			MulticastContext<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			context = new MulticastContext<IDisposable>();

			Assert.IsNotNull(context);

			context.SetContext(typeof(IDisposable), mockContext);
			context.SetContext(typeof(IDisposable), mockContext);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedGetContextTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			MulticastContext<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			context = new MulticastContext<IDisposable>();

			Assert.IsNotNull(context);

			Assert.IsFalse(context.Disposed);
			context.Dispose();
			Assert.IsTrue(context.Disposed);

			context.GetContext(typeof(IDisposable));
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedHasContextTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			MulticastContext<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			context = new MulticastContext<IDisposable>();

			Assert.IsNotNull(context);

			Assert.IsFalse(context.Disposed);
			context.Dispose();
			Assert.IsTrue(context.Disposed);

			context.HasContext(typeof(IDisposable));
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedSetContextTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			MulticastContext<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			context = new MulticastContext<IDisposable>();

			Assert.IsNotNull(context);

			Assert.IsFalse(context.Disposed);
			context.Dispose();
			Assert.IsTrue(context.Disposed);

			context.SetContext(typeof(IDisposable), mockContext);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnNotAlreadyRegisteredGetContextTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			MulticastContext<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			context = new MulticastContext<IDisposable>();

			Assert.IsNotNull(context);

			context.GetContext(typeof(IDisposable));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullActualTypeGetContextTest()
		{
			Mockery mockery;
			IDisposable mockContext;
			MulticastContext<IDisposable> context;

			mockery = new Mockery();
			mockContext = mockery.NewMock<IDisposable>();

			context = new MulticastContext<IDisposable>();

			Assert.IsNotNull(context);

			context.GetContext(null);
		}

		[Test]
		public void ShouldNotFailOnDoubleDisposeTest()
		{
			Mockery mockery;
			MulticastContext<IDisposable> context;

			mockery = new Mockery();

			context = new MulticastContext<IDisposable>();

			Assert.IsNotNull(context);

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