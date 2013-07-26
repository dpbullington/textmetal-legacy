// ****************************************************************
// Copyright 2012, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

#if CLR_2_0 || CLR_4_0
using System;

namespace NUnit.Framework
{
	/// <summary>
	/// 	Provide actions to execute before and after tests.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public abstract class TestActionAttribute : Attribute, ITestAction
	{
		#region Properties/Indexers/Events

		public virtual ActionTargets Targets
		{
			get
			{
				return ActionTargets.Default;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void AfterTest(TestDetails testDetails)
		{
		}

		public virtual void BeforeTest(TestDetails testDetails)
		{
		}

		#endregion
	}
}

#endif