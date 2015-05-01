/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;
using TextMetal.Middleware.Data.Impl.MicrosoftLinq;

namespace TextMetal.Middleware.UnitTests.Data.Impl.MicrosoftLinq._
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
			IDisposable mockDisposable;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareContextWrapper<IDisposable> ambientUnitOfWorkAwareContextWrapper;

			mockFactory = new MockFactory();
			mockDisposable = mockFactory.CreateInstance<IDisposable>();
			mockUnitOfWork = mockFactory.CreateInstance<IUnitOfWork>();

			Expect.Once.On(mockDisposable).Method("Dispose").WithNoArguments();

			ambientUnitOfWorkAwareContextWrapper = new AmbientUnitOfWorkAwareContextWrapper<IDisposable>(mockUnitOfWork, mockDisposable);

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
			IDisposable mockDisposable;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareContextWrapper<IDisposable> ambientUnitOfWorkAwareContextWrapper;

			mockFactory = new MockFactory();
			mockDisposable = mockFactory.CreateInstance<IDisposable>();
			mockUnitOfWork = mockFactory.CreateInstance<IUnitOfWork>();

			Expect.Once.On(mockDisposable).Method("Dispose").WithNoArguments();

			ambientUnitOfWorkAwareContextWrapper = new AmbientUnitOfWorkAwareContextWrapper<IDisposable>(mockUnitOfWork, mockDisposable);

			Assert.IsNotNull(ambientUnitOfWorkAwareContextWrapper);
			Assert.IsNotNull(ambientUnitOfWorkAwareContextWrapper.DisposableContext);

			Assert.IsFalse(ambientUnitOfWorkAwareContextWrapper.Disposed);
			ambientUnitOfWorkAwareContextWrapper.Dispose();
			Assert.IsTrue(ambientUnitOfWorkAwareContextWrapper.Disposed);

			mockDisposable = ambientUnitOfWorkAwareContextWrapper.DisposableContext;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDisposableCreateTest()
		{
			MockFactory mockFactory;
			IDisposable mockDisposable;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareContextWrapper<IDisposable> ambientUnitOfWorkAwareContextWrapper;

			mockFactory = new MockFactory();
			mockDisposable = null;
			mockUnitOfWork = mockFactory.CreateInstance<IUnitOfWork>();

			ambientUnitOfWorkAwareContextWrapper = new AmbientUnitOfWorkAwareContextWrapper<IDisposable>(mockUnitOfWork, mockDisposable);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullSourceUnitOfWorkCreateTest()
		{
			MockFactory mockFactory;
			IDisposable mockDisposable;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareContextWrapper<IDisposable> ambientUnitOfWorkAwareContextWrapper;

			mockFactory = new MockFactory();
			mockDisposable = mockFactory.CreateInstance<IDisposable>();
			mockUnitOfWork = null;

			ambientUnitOfWorkAwareContextWrapper = new AmbientUnitOfWorkAwareContextWrapper<IDisposable>(mockUnitOfWork, mockDisposable);
		}

		[Test]
		public void ShouldNotFailOnDoubleDisposeTest()
		{
			MockFactory mockFactory;
			IDisposable mockDisposable;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareContextWrapper<IDisposable> ambientUnitOfWorkAwareContextWrapper;

			mockFactory = new MockFactory();
			mockDisposable = mockFactory.CreateInstance<IDisposable>();
			mockUnitOfWork = mockFactory.CreateInstance<IUnitOfWork>();

			Expect.Once.On(mockDisposable).Method("Dispose").WithNoArguments();

			ambientUnitOfWorkAwareContextWrapper = new AmbientUnitOfWorkAwareContextWrapper<IDisposable>(mockUnitOfWork, mockDisposable);

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