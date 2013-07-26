// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Specialized;

namespace NUnit.Core
{
	/// <summary>
	/// 	TestDecorator is used to add functionality to
	/// 	another Test, which it aggregates.
	/// </summary>
	public abstract class TestDecorator : TestMethod
	{
		#region Constructors/Destructors

		public TestDecorator(TestMethod test)
			//: base( (TestName)test.TestName.Clone() )
			: base(test.Method)
		{
			this.test = test;
			this.RunState = test.RunState;
			this.IgnoreReason = test.IgnoreReason;
			this.Description = test.Description;
			this.Categories = new ArrayList(test.Categories);
			if (test.Properties != null)
			{
				this.Properties = new ListDictionary();
				foreach (DictionaryEntry entry in test.Properties)
					this.Properties.Add(entry.Key, entry.Value);
			}
		}

		#endregion

		#region Fields/Constants

		protected TestMethod test;

		#endregion

		#region Properties/Indexers/Events

		public override int TestCount
		{
			get
			{
				return this.test.TestCount;
			}
		}

		#endregion
	}
}