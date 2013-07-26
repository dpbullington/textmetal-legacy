// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// 	Base class for servers
	/// </summary>
	public class TestServer : ServerBase
	{
		#region Constructors/Destructors

		public TestServer(string uri, int port)
			: base(uri, port)
		{
			this.runner = new TestDomain();
		}

		#endregion

		#region Fields/Constants

		private TestRunner runner;

		#endregion

		#region Properties/Indexers/Events

		public TestRunner TestRunner
		{
			get
			{
				return this.runner;
			}
		}

		#endregion
	}
}