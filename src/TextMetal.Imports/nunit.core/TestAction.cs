// ****************************************************************
// Copyright 2012, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

#if CLR_2_0 || CLR_4_0
using System;
using System.Reflection;

namespace NUnit.Core
{
	public class TestAction
	{
		#region Constructors/Destructors

		static TestAction()
		{
			_ActionInterfaceType = Type.GetType(NUnitFramework.TestActionInterface);
			_TestDetailsClassType = Type.GetType(NUnitFramework.TestDetailsClass);
		}

		public TestAction(object action)
		{
			if (action == null)
				throw new ArgumentNullException("action");

			this._Action = action;
			this._Targets = (int)Reflect.GetPropertyValue(action, "Targets");
		}

		#endregion

		#region Fields/Constants

		public static readonly int TargetsDefault = 0;
		public static readonly int TargetsSuite = 2;
		public static readonly int TargetsTest = 1;

		private static readonly Type _ActionInterfaceType = null;
		private static readonly Type _TestDetailsClassType = null;

		private readonly object _Action;
		private readonly int _Targets;

		#endregion

		#region Properties/Indexers/Events

		public int Targets
		{
			get
			{
				return this._Targets;
			}
		}

		#endregion

		#region Methods/Operators

		private static object CreateTestDetails(ITest test)
		{
			object fixture = null;
			MethodInfo method = null;

			var testMethod = test as TestMethod;
			if (testMethod != null)
				method = testMethod.Method;

			var testObject = test as Test;
			if (testObject != null)
				fixture = testObject.Fixture;

			return Activator.CreateInstance(_TestDetailsClassType,
			                                fixture,
			                                method,
			                                test.TestName.FullName,
			                                test.TestType,
			                                test.IsSuite);
		}

		public bool DoesTarget(int target)
		{
			if (target < 0)
				throw new ArgumentOutOfRangeException("target", "Target must be a positive integer.");

			if (target == 0)
				return this._Targets == 0;

			uint self = Convert.ToUInt32(target);
			return (this._Targets & self) == self;
		}

		private void Execute(ITest test, string methodPrefix)
		{
			var method = Reflect.GetNamedMethod(_ActionInterfaceType, methodPrefix + "Test");
			var details = CreateTestDetails(test);

			Reflect.InvokeMethod(method, this._Action, details);
		}

		public void ExecuteAfter(ITest test)
		{
			this.Execute(test, "After");
		}

		public void ExecuteBefore(ITest test)
		{
			this.Execute(test, "Before");
		}

		#endregion
	}
}

#endif