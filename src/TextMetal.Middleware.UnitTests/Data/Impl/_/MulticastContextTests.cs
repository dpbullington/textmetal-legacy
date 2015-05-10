/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Data.Impl;

namespace TextMetal.Middleware.UnitTests.Data.Impl._
{
	[TestFixture]
	public class MulticastDisposableContextTests
	{
		#region Constructors/Destructors

		public MulticastDisposableContextTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			MockFactory mockFactory;
			IDisposable mockContext, storedContext;
			MulticastDisposableContext<IDisposable> context;

			mockFactory = new MockFactory();
			mockContext = mockFactory.CreateInstance<IDisposable>();

			Expect.On(mockContext).One.Method(x => x.Dispose());

			context = new MulticastDisposableContext<IDisposable>();

			Assert.IsNotNull(context);

			Assert.IsFalse(context.HasContext(typeof(IDisposable)));
			context.SetContext(typeof(IDisposable), mockContext);
			Assert.IsTrue(context.HasContext(typeof(IDisposable)));
			storedContext = context.GetContext(typeof(IDisposable));

			Assert.AreSame(mockContext, storedContext);

			Assert.IsFalse(context.Disposed);
			context.Dispose();
			Assert.IsTrue(context.Disposed);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnActualInstanceSetContextTest()
		{
			MockFactory mockFactory;
			IDisposable mockContext;
			MulticastDisposableContext<IDisposable> context;

			mockFactory = new MockFactory();
			mockContext = null;

			context = new MulticastDisposableContext<IDisposable>();

			Assert.IsNotNull(context);

			context.SetContext(typeof(IDisposable), mockContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnActualTypeHasContextTest()
		{
			MockFactory mockFactory;
			IDisposable mockContext;
			MulticastDisposableContext<IDisposable> context;

			mockFactory = new MockFactory();
			mockContext = mockFactory.CreateInstance<IDisposable>();

			context = new MulticastDisposableContext<IDisposable>();

			Assert.IsNotNull(context);

			context.HasContext(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnActualTypeSetContextTest()
		{
			MockFactory mockFactory;
			IDisposable mockContext;
			MulticastDisposableContext<IDisposable> context;

			mockFactory = new MockFactory();
			mockContext = mockFactory.CreateInstance<IDisposable>();

			context = new MulticastDisposableContext<IDisposable>();

			Assert.IsNotNull(context);

			context.SetContext(null, mockContext);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnAlreadyRegisteredSetContextTest()
		{
			MockFactory mockFactory;
			IDisposable mockContext;
			MulticastDisposableContext<IDisposable> context;

			mockFactory = new MockFactory();
			mockContext = mockFactory.CreateInstance<IDisposable>();

			context = new MulticastDisposableContext<IDisposable>();

			Assert.IsNotNull(context);

			context.SetContext(typeof(IDisposable), mockContext);
			context.SetContext(typeof(IDisposable), mockContext);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedGetContextTest()
		{
			MockFactory mockFactory;
			IDisposable mockContext;
			MulticastDisposableContext<IDisposable> context;

			mockFactory = new MockFactory();
			mockContext = mockFactory.CreateInstance<IDisposable>();

			context = new MulticastDisposableContext<IDisposable>();

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
			MockFactory mockFactory;
			IDisposable mockContext;
			MulticastDisposableContext<IDisposable> context;

			mockFactory = new MockFactory();
			mockContext = mockFactory.CreateInstance<IDisposable>();

			context = new MulticastDisposableContext<IDisposable>();

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
			MockFactory mockFactory;
			IDisposable mockContext;
			MulticastDisposableContext<IDisposable> context;

			mockFactory = new MockFactory();
			mockContext = mockFactory.CreateInstance<IDisposable>();

			context = new MulticastDisposableContext<IDisposable>();

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
			MockFactory mockFactory;
			IDisposable mockContext;
			MulticastDisposableContext<IDisposable> context;

			mockFactory = new MockFactory();
			mockContext = mockFactory.CreateInstance<IDisposable>();

			context = new MulticastDisposableContext<IDisposable>();

			Assert.IsNotNull(context);

			context.GetContext(typeof(IDisposable));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullActualTypeGetContextTest()
		{
			MockFactory mockFactory;
			IDisposable mockContext;
			MulticastDisposableContext<IDisposable> context;

			mockFactory = new MockFactory();
			mockContext = mockFactory.CreateInstance<IDisposable>();

			context = new MulticastDisposableContext<IDisposable>();

			Assert.IsNotNull(context);

			context.GetContext(null);
		}

		[Test]
		public void ShouldNotFailOnDoubleDisposeTest()
		{
			MockFactory mockFactory;
			MulticastDisposableContext<IDisposable> context;

			mockFactory = new MockFactory();

			context = new MulticastDisposableContext<IDisposable>();

			Assert.IsNotNull(context);

			Assert.IsFalse(context.Disposed);
			context.Dispose();
			Assert.IsTrue(context.Disposed);
			context.Dispose();
			Assert.IsTrue(context.Disposed);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}