/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Data.Impl;
using TextMetal.Middleware.Data.UoW;

using _MockDataContext = System.IDisposable;

namespace TextMetal.Middleware.UnitTests.Data.Impl._
{
	[TestFixture]
	public class AmbientUnitOfWorkAwareContextWrapperTests
	{
		#region Constructors/Destructors

		public AmbientUnitOfWorkAwareContextWrapperTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			MockFactory mockFactory;
			_MockDataContext mockDataContext;
			IUnitOfWork mockUnitOfWork;
			MulticastDisposableContext<_MockDataContext> mockMulticastDisposableContext;
			AmbientUnitOfWorkAwareContextWrapper<_MockDataContext> ambientUnitOfWorkAwareContextWrapper;

			mockFactory = new MockFactory();
			mockDataContext = mockFactory.CreateInstance<_MockDataContext>();
			mockUnitOfWork = mockFactory.CreateInstance<IUnitOfWork>();
			mockMulticastDisposableContext = new MulticastDisposableContext<_MockDataContext>();

			Expect.On(mockDataContext).One.Method(x => x.Dispose());
			Expect.On(mockUnitOfWork).One.GetProperty(x => x.Context).WillReturn(mockMulticastDisposableContext);

			ambientUnitOfWorkAwareContextWrapper = new AmbientUnitOfWorkAwareContextWrapper<_MockDataContext>(mockUnitOfWork, mockDataContext);

			Assert.IsNotNull(ambientUnitOfWorkAwareContextWrapper);
			Assert.IsNotNull(ambientUnitOfWorkAwareContextWrapper.DisposableContext);

			Assert.IsFalse(ambientUnitOfWorkAwareContextWrapper.Disposed);
			ambientUnitOfWorkAwareContextWrapper.Dispose();
			Assert.IsTrue(ambientUnitOfWorkAwareContextWrapper.Disposed);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedGetInnerDisposableTest()
		{
			MockFactory mockFactory;
			_MockDataContext mockDataContext;
			IUnitOfWork mockUnitOfWork;
			MulticastDisposableContext<_MockDataContext> mockMulticastDisposableContext;
			AmbientUnitOfWorkAwareContextWrapper<_MockDataContext> ambientUnitOfWorkAwareContextWrapper;

			mockFactory = new MockFactory();
			mockDataContext = mockFactory.CreateInstance<_MockDataContext>();
			mockUnitOfWork = mockFactory.CreateInstance<IUnitOfWork>();
			mockMulticastDisposableContext = new MulticastDisposableContext<_MockDataContext>();

			Expect.On(mockDataContext).One.Method(x => x.Dispose());
			Expect.On(mockUnitOfWork).One.GetProperty(x => x.Context).WillReturn(mockMulticastDisposableContext);

			ambientUnitOfWorkAwareContextWrapper = new AmbientUnitOfWorkAwareContextWrapper<IDisposable>(mockUnitOfWork, mockDataContext);

			Assert.IsNotNull(ambientUnitOfWorkAwareContextWrapper);
			Assert.IsNotNull(ambientUnitOfWorkAwareContextWrapper.DisposableContext);

			Assert.IsFalse(ambientUnitOfWorkAwareContextWrapper.Disposed);
			ambientUnitOfWorkAwareContextWrapper.Dispose();
			Assert.IsTrue(ambientUnitOfWorkAwareContextWrapper.Disposed);

			mockDataContext = ambientUnitOfWorkAwareContextWrapper.DisposableContext;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDisposableCreateTest()
		{
			MockFactory mockFactory;
			_MockDataContext mockDataContext;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareContextWrapper<_MockDataContext> ambientUnitOfWorkAwareContextWrapper;

			mockFactory = new MockFactory();
			mockDataContext = null;
			mockUnitOfWork = mockFactory.CreateInstance<IUnitOfWork>();

			ambientUnitOfWorkAwareContextWrapper = new AmbientUnitOfWorkAwareContextWrapper<IDisposable>(mockUnitOfWork, mockDataContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullSourceUnitOfWorkCreateTest()
		{
			MockFactory mockFactory;
			_MockDataContext mockDataContext;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareContextWrapper<_MockDataContext> ambientUnitOfWorkAwareContextWrapper;

			mockFactory = new MockFactory();
			mockDataContext = mockFactory.CreateInstance<_MockDataContext>();
			mockUnitOfWork = null;

			ambientUnitOfWorkAwareContextWrapper = new AmbientUnitOfWorkAwareContextWrapper<IDisposable>(mockUnitOfWork, mockDataContext);
		}

		[Test]
		public void ShouldNotFailOnDoubleDisposeTest()
		{
			MockFactory mockFactory;
			_MockDataContext mockDataContext;
			IUnitOfWork mockUnitOfWork;
			MulticastDisposableContext<_MockDataContext> mockMulticastDisposableContext;
			AmbientUnitOfWorkAwareContextWrapper<_MockDataContext> ambientUnitOfWorkAwareContextWrapper;

			mockFactory = new MockFactory();
			mockDataContext = mockFactory.CreateInstance<_MockDataContext>();
			mockUnitOfWork = mockFactory.CreateInstance<IUnitOfWork>();
			mockMulticastDisposableContext = new MulticastDisposableContext<IDisposable>();

			Expect.On(mockDataContext).One.Method(x => x.Dispose());
			Expect.On(mockUnitOfWork).One.GetProperty(x => x.Context).WillReturn(mockMulticastDisposableContext);

			ambientUnitOfWorkAwareContextWrapper = new AmbientUnitOfWorkAwareContextWrapper<_MockDataContext>(mockUnitOfWork, mockDataContext);

			Assert.IsNotNull(ambientUnitOfWorkAwareContextWrapper);
			Assert.IsNotNull(ambientUnitOfWorkAwareContextWrapper.DisposableContext);

			Assert.IsFalse(ambientUnitOfWorkAwareContextWrapper.Disposed);
			ambientUnitOfWorkAwareContextWrapper.Dispose();
			Assert.IsTrue(ambientUnitOfWorkAwareContextWrapper.Disposed);
			ambientUnitOfWorkAwareContextWrapper.Dispose();
			Assert.IsTrue(ambientUnitOfWorkAwareContextWrapper.Disposed);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}