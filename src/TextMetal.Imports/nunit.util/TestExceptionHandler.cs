// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.Util
{
	/// <summary>
	/// Summary description for UnhandledExceptionCatcher.
	/// </summary>
	public class TestExceptionHandler : IDisposable
	{
		#region Constructors/Destructors

		public TestExceptionHandler(UnhandledExceptionEventHandler handler)
		{
			this.handler = handler;
			AppDomain.CurrentDomain.UnhandledException += handler;
		}

		~TestExceptionHandler()
		{
			if (this.handler != null)
			{
				AppDomain.CurrentDomain.UnhandledException -= this.handler;
				this.handler = null;
			}
		}

		#endregion

		#region Fields/Constants

		private UnhandledExceptionEventHandler handler;

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
			if (this.handler != null)
			{
				AppDomain.CurrentDomain.UnhandledException -= this.handler;
				this.handler = null;
			}

			GC.SuppressFinalize(this);
		}

		#endregion
	}
}