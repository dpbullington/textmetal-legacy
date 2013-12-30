/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock2;

using NUnit.Framework;

using TextMetal.Common.Data;

namespace TextMetal.Common.UnitTests.Data
{
	[TestFixture]
	public class AmbientUnitOfWorkAwareDisposableWrapperTests
	{
		#region Constructors/Destructors

		public AmbientUnitOfWorkAwareDisposableWrapperTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			Mockery mockery;
			IDisposable mockDisposable;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareDisposableWrapper<IDisposable> ambientUnitOfWorkAwareDisposable;

			mockery = new Mockery();
			mockDisposable = mockery.NewMock<IDisposable>();
			mockUnitOfWork = mockery.NewMock<IUnitOfWork>();

			Expect.Once.On(mockDisposable).Method("Dispose").WithNoArguments();

			ambientUnitOfWorkAwareDisposable = new AmbientUnitOfWorkAwareDisposableWrapper<IDisposable>(mockUnitOfWork, mockDisposable);

			Assert.IsNotNull(ambientUnitOfWorkAwareDisposable);
			Assert.IsNotNull(ambientUnitOfWorkAwareDisposable.Disposable);

			Assert.IsFalse(ambientUnitOfWorkAwareDisposable.Disposed);
			ambientUnitOfWorkAwareDisposable.Dispose();
			Assert.IsTrue(ambientUnitOfWorkAwareDisposable.Disposed);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedGetInnerDisposableTest()
		{
			Mockery mockery;
			IDisposable mockDisposable;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareDisposableWrapper<IDisposable> ambientUnitOfWorkAwareDisposable;

			mockery = new Mockery();
			mockDisposable = mockery.NewMock<IDisposable>();
			mockUnitOfWork = mockery.NewMock<IUnitOfWork>();

			Expect.Once.On(mockDisposable).Method("Dispose").WithNoArguments();

			ambientUnitOfWorkAwareDisposable = new AmbientUnitOfWorkAwareDisposableWrapper<IDisposable>(mockUnitOfWork, mockDisposable);

			Assert.IsNotNull(ambientUnitOfWorkAwareDisposable);
			Assert.IsNotNull(ambientUnitOfWorkAwareDisposable.Disposable);

			Assert.IsFalse(ambientUnitOfWorkAwareDisposable.Disposed);
			ambientUnitOfWorkAwareDisposable.Dispose();
			Assert.IsTrue(ambientUnitOfWorkAwareDisposable.Disposed);

			mockDisposable = ambientUnitOfWorkAwareDisposable.Disposable;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDisposableCreateTest()
		{
			Mockery mockery;
			IDisposable mockDisposable;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareDisposableWrapper<IDisposable> ambientUnitOfWorkAwareDisposable;

			mockery = new Mockery();
			mockDisposable = null;
			mockUnitOfWork = mockery.NewMock<IUnitOfWork>();

			ambientUnitOfWorkAwareDisposable = new AmbientUnitOfWorkAwareDisposableWrapper<IDisposable>(mockUnitOfWork, mockDisposable);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullSourceUnitOfWorkCreateTest()
		{
			Mockery mockery;
			IDisposable mockDisposable;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareDisposableWrapper<IDisposable> ambientUnitOfWorkAwareDisposable;

			mockery = new Mockery();
			mockDisposable = mockery.NewMock<IDisposable>();
			mockUnitOfWork = null;

			ambientUnitOfWorkAwareDisposable = new AmbientUnitOfWorkAwareDisposableWrapper<IDisposable>(mockUnitOfWork, mockDisposable);
		}

		[Test]
		public void ShouldNotFailOnDoubleDisposeTest()
		{
			Mockery mockery;
			IDisposable mockDisposable;
			IUnitOfWork mockUnitOfWork;
			AmbientUnitOfWorkAwareDisposableWrapper<IDisposable> ambientUnitOfWorkAwareDisposable;

			mockery = new Mockery();
			mockDisposable = mockery.NewMock<IDisposable>();
			mockUnitOfWork = mockery.NewMock<IUnitOfWork>();

			Expect.Once.On(mockDisposable).Method("Dispose").WithNoArguments();

			ambientUnitOfWorkAwareDisposable = new AmbientUnitOfWorkAwareDisposableWrapper<IDisposable>(mockUnitOfWork, mockDisposable);

			Assert.IsNotNull(ambientUnitOfWorkAwareDisposable);
			Assert.IsNotNull(ambientUnitOfWorkAwareDisposable.Disposable);

			Assert.IsFalse(ambientUnitOfWorkAwareDisposable.Disposed);
			ambientUnitOfWorkAwareDisposable.Dispose();
			Assert.IsTrue(ambientUnitOfWorkAwareDisposable.Disposed);
			ambientUnitOfWorkAwareDisposable.Dispose();
			Assert.IsTrue(ambientUnitOfWorkAwareDisposable.Disposed);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}